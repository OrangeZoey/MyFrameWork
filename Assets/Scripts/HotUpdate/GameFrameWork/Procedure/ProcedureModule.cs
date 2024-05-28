using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ProcedureModule : BaseGameModule
{
    
    public int value;

    //��˽���ֶΣ�private fields����������ʾ��Unity�༭���У�������ͨ��Inspector�������ú��޸���Щ�ֶε�ֵ
    [SerializeField]
    private string[] proceduresNames = null;//������
    [SerializeField]
    private string defaultProcedureName = null;//Ĭ������

    //��ǰ����
    public BaseProcedure CurrentProcedure { get; private set; }

    //�Ƿ�����
    public bool IsRunning { get; private set; }

    //�Ƿ��л�����
    public bool IsChangingProcedure { get; private set; }

    //�洢�������Ѵ���������ʵ�����������̵����ͣ�ֵ�����̵�ʵ��
    private Dictionary<Type, BaseProcedure> procedures;
    //Ĭ�����̵�ʵ��
    private BaseProcedure defaultProcedure;
    //����أ����ڴ洢�͹���ChangeProcedureRequest����Ĵ����ͻ��� 
    private ObjectPool<ChangeProcedureRequest> changeProcedureRequestPool = new ObjectPool<ChangeProcedureRequest>(null);
    //���У����ڴ洢������������л�����
    private Queue<ChangeProcedureRequest> changeProcedureQ = new Queue<ChangeProcedureRequest>();

    /// <summary>
    /// ��ʼ��
    /// </summary>
    protected internal override void OnModuleInit()
    {
        base.OnModuleInit();

        procedures = new Dictionary<Type, BaseProcedure>();
        bool findDefaultState = false;
        for (int i = 0; i < proceduresNames.Length; i++)
        {
            string procedureTypeName = proceduresNames[i];

            //���Ʋ�Ϊ��
            if (string.IsNullOrEmpty(procedureTypeName))
                continue;

            //��ȡ���Ӧ�����ͣ������������͵�ʵ��
            //procedureTypeName ��һ���ַ������������������ȡ�����͵���ȫ�޶����������������ռ�ͳ�����Ϣ�������Ҫ�Ļ�����Type.GetType �����᳢���ҵ��������ƥ������͡�
            // true ��ζ������Ҳ���ָ�������ͣ��÷������׳�һ�� TypeLoadException �쳣������㲻���׳��쳣��ֻ�������Ҳ�������ʱ���� null������Խ������������Ϊ false
            //Type.GetType Ĭ��ֻ����ҵ�ǰ����ִ�еĳ��򼯺��Ѽ��ص����ó����е����͡��������λ������δ���صĳ����У��������͵�����û�а����㹻�ĳ����޶���Ϣ��Type.GetType ���ܻ᷵�� null����ʹ������ʵ���ϴ�����ĳ��������
            Type procedureType = Type.GetType(procedureTypeName, true);

            if (procedureType == null)
            {
                Debug.LogError($"Can't find procedure:`{procedureTypeName}`");
                continue;
            }
            //ʹ�� Activator ���� MyProcedure ��ʵ����������ת��Ϊ BaseProcedure ����
            BaseProcedure procedure = Activator.CreateInstance(procedureType) as BaseProcedure;

            // ����ֵ��¼ ������defaultProcedureName�Ƿ���ͬ 
            bool isDefaultState = procedureTypeName == defaultProcedureName;

            //��ӽ��ֵ�
            procedures.Add(procedureType, procedure);

            //�����Ĭ�ϳ��� ����Ĭ�ϳ��� ����ֵ��¼
            if (isDefaultState)
            {
                defaultProcedure = procedure;
                findDefaultState = true;
            }
        }

        //���û���ҵ�Ĭ�����̣���ӡ������Ϣ
        if (!findDefaultState)
        {
            Debug.LogError($"You have to set a correct default procedure to start game");
        }
    }

    /// <summary>
    /// ����ģ��
    /// </summary>
    protected internal override void OnModuleStart()
    {
        base.OnModuleStart();
    }

    /// <summary>
    /// ֹͣģ��
    /// </summary>
    protected internal override void OnModuleStop()
    {
        base.OnModuleStop();

        //��ն����
        changeProcedureRequestPool.Clear();
        //��ն���
        changeProcedureQ.Clear();
        //����ֹͣ ��Ϊfalse
        IsRunning = false;
    }

    protected internal override void OnModuleUpdate(float deltaTime)
    {
        base.OnModuleUpdate(deltaTime);
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <returns></returns>
    public async Task StartProcedure()
    {
        //�Ѿ������� ֱ�ӽ���
        if (IsRunning)
            return;

        //û���� ����Ϊtrue 
        IsRunning = true;

        //�� changeProcedureRequestPool �л�ȡһ�� ChangeProcedureRequest ����
        ChangeProcedureRequest changeProcedureRequest = changeProcedureRequestPool.Obtain();

        //TargetProcedure ��������Ϊ defaultProcedure
        changeProcedureRequest.TargetProcedure = defaultProcedure;

        //�� changeProcedureRequest ������� changeProcedureQ ������
        changeProcedureQ.Enqueue(changeProcedureRequest);

        //�� ChangeProcedureInternal ���������ȴ������
        //���� await ��ʹ��ǰ�����ȴ� ChangeProcedureInternal ��ɣ��������������߳�
        await ChangeProcedureInternal();
    }

    /// <summary>
    /// �ı�����
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public async Task ChangeProcedure<T>() where T : BaseProcedure
    {
        //���� ChangeProcedure ����Ϊ��
        await ChangeProcedure<T>(null);
    }

    /// <summary>
    /// ���ط��� ChangeProcedure<T>(object value)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public async Task ChangeProcedure<T>(object value) where T : BaseProcedure
    {
        //����û�������� ����
        if (!IsRunning)
            return;

        //�� procedures �ֵ��и������� T ��ȡ��Ӧ�Ĺ��� procedure
        if (!procedures.TryGetValue(typeof(T),out BaseProcedure procedure))
        {
            //���һ��������־������
            Debug.Log($"Change Procedure Failed, Can't find Proecedure:${typeof(T).FullName}");
            return;
        }

        //�����������
        ChangeProcedureRequest changeProcedureRequest = changeProcedureRequestPool.Obtain();
        //��ֵ
        changeProcedureRequest.TargetProcedure = procedure;
        changeProcedureRequest.Value = value;
        changeProcedureQ.Enqueue(changeProcedureRequest);

        //�����ǰû�����ڸı�Ĺ��� ���÷���
        if (!IsChangingProcedure)
        {
            await ChangeProcedureInternal();
        }
    }

    private async Task ChangeProcedureInternal()
    {
        //�����ڸı�� ����
        if (IsChangingProcedure)
            return;

        IsChangingProcedure = true;
        //����ѭ�� ����ÿһ������
        while (changeProcedureQ.Count>0)
        {
            ChangeProcedureRequest request = changeProcedureQ.Dequeue();
            //�������������Ŀ������Ƿ�Ϊ null
            if (request == null || request.TargetProcedure == null)
                continue;

            //�����ǰ�г�����������
            if (CurrentProcedure != null)
            {
                //���õ�ǰ����� OnLeaveProcedure ����
                await CurrentProcedure.OnLeaveProcedure();
            }

            //����ǰ��������Ϊ�����Ŀ�����
            CurrentProcedure = request.TargetProcedure;
            //������ OnEnterProcedure����
            await CurrentProcedure.OnEnterProcedure(request.Value);
        }

        //������������������� IsChangingProcedure ���û� false��
        IsChangingProcedure = false;
    }
}

/// <summary>
/// �ı��������
/// </summary>
public class ChangeProcedureRequest
{
    public BaseProcedure TargetProcedure { get; set; }
    public object Value { get; set; }
}