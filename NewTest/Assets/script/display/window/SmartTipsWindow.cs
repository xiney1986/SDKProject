using UnityEngine;
using System.Collections.Generic;

public class SmartTipsWindow : WindowBase
{

    public UILabel UI_Content;
    public GameObject UI_Arrow1;
    public GameObject UI_Arrow2;


    protected override void begin()
    {
        base.begin();


        MaskWindow.UnlockUI();
    }


    protected override void DoUpdate()
    {
        base.DoUpdate();
        if (Input.GetMouseButtonUp(0))
            finishWindow();
    }


    public void init(string content)
    {
        UI_Content.text = content;
    }


    






}

