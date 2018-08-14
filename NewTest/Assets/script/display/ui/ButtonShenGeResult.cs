using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonShenGeResult : ButtonBase
{  
	public Prop prop;
    private int index;

    public void UpdateProp(Prop _prop,int indexx)
	{
        this.prop = _prop;
        index = indexx;
	}
	
	//镶嵌和替换神格成功的回调
	public void equipResult ()
	{
        ShenGeAloneWindow win = fatherWindow as ShenGeAloneWindow;
        win.finishWindow();
        //刷新神格界面
        if (UiManager.Instance.getWindow<NvShenShenGeWindow>() != null) {
            UiManager.Instance.getWindow<NvShenShenGeWindow>().init();
        }
	}
	
	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
	    PutOnShenGeFPort fPort = FPortManager.Instance.getFPort("PutOnShenGeFPort") as PutOnShenGeFPort;
        fPort.access(prop.sid,index,equipResult);
	}
}
