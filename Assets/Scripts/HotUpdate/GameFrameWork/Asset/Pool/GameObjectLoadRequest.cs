using System;
using UnityEngine;

namespace TGame.Asset
{
    public class GameObjectLoadRequest<T> where T : GameObjectPoolAsset
    {
        public GameObjectLoadState State { get; private set; } //״̬
        public string Path { get; } //·��
        public Action<GameObject> CreateNewCallback { get; } //ί��

        private Action<T> callback; //�ص�

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        /// <param name="createNewCallback"></param>
        public GameObjectLoadRequest(string path, Action<T> callback, Action<GameObject> createNewCallback)
        {
            Path = path;
            this.callback = callback;
            CreateNewCallback = createNewCallback;
        }

        /// <summary>
        /// �������
        /// </summary>
        /// <param name="obj"></param>
        public void LoadFinish(T obj)
        {
            //�Ǽ�����״̬
            if (State == GameObjectLoadState.Loading)
            {
                //ִ�лص�����
                callback?.Invoke(obj);
                //����״̬
                State = GameObjectLoadState.Finish;
            }
        }
    }
}
