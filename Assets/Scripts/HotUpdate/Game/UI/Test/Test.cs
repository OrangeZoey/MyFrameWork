using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Test : MessageModule
{
    public Button Btn;

    // Start is called before the first frame update
    void Start()
    {
        Btn.onClick.AddListener(() =>
        {
            GameManager.Message.Post<MessageType.TestUIView>(new MessageType.TestUIView() { }).Coroutine();
        });
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


public class TestMessageHandler : MessageHandler<MessageType.TestUIView>
{
    public async override Task HandleMessage(MessageType.TestUIView arg)
    {
        Debug.Log("�����ť");
        GameManager.UI.OpenUI(UIViewID.TestUIView);
        await Task.Yield();
    }
}

