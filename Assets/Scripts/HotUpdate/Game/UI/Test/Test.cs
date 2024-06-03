using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Test : MessageModule
{
    public Button Btn_Test;
    public Button Btn_TestTWO;


    // Start is called before the first frame update
    void Start()
    {
        Btn_Test.onClick.AddListener(() =>
        {
            GameManager.Message.Post<MessageType.TestUIView>(new MessageType.TestUIView() { }).Coroutine();
        });
        Btn_TestTWO.onClick.AddListener(() =>
        {
            GameManager.UI.OpenUI(UIViewID.TestTWOUIView);
        });

        var result = GetGroupTicketPrice(1);
        Debug.Log(result); 
        var result2 = Classify(13);
        Debug.Log(result2);
    }

    public static decimal GetGroupTicketPrice(int visitorCount) => visitorCount switch
    {
        1 => 12.0m,
        2 => 20.0m,
        3 => 27.0m,
        0 => 0m,
        _ => throw new System.Exception($"一次只能团购1/2/3张票")

    };
    public static string Classify(double measurement) => measurement switch
    {
        < -4.0 => "Too low",
        > 10.0 => "Too high",
        double.NaN => "Unknow",
        _ => "Acceptable,"

    };

    // Update is called once per frame
    void Update()
    {

    }
}


public class TestMessageHandler : MessageHandler<MessageType.TestUIView>
{
    TestUIMeditor meditor;
    bool isOpen;

    public async override Task HandleMessage(MessageType.TestUIView arg)
    {
        Debug.Log("点击按钮");
        if (!isOpen)
        {
            isOpen = true;
            GameManager.UI.OpenUI(UIViewID.TestUIView);
        }
        else
        {
            isOpen = false;
            GameManager.UI.CloseUI(UIViewID.TestUIView);
        }


        await Task.Yield();
    }
}

