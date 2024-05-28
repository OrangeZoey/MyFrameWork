using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUIMeditor : UIMediator<TestUIView>
{
    protected override void OnInit(TestUIView view)
    {
        base.OnInit(view);
    }

    protected override void OnShow(object arg)
    {
        base.OnShow(arg);

        view.Text_Test.text = "≤‚ ‘√Ê∞Â";
    }
}
