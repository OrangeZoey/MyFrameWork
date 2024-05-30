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

    }

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
        Debug.Log("µã»÷°´Å¥");
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

