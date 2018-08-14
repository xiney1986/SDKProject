using UnityEngine;
using System.Collections;

/// <summary>
/// 神格仓库
/// </summary>
public class ShenGeAloneWindow : WindowBase {


    /** 神格仓库*/
    public ShenGeContent contentShenGe;//神格容器
    private ArrayList propList;//神格列表
    private ArrayList typeList;
    private int type;
    private Prop prop;
    private int index;
	/** 激活window */
    protected override void begin() {
        base.begin();
        if (isAwakeformHide) {
            init(type, prop, index);
        }
        MaskWindow.UnlockUI();
    }
    public override void OnNetResume() {
        base.OnNetResume();
        finishWindow();
        if (UiManager.Instance.getWindow<NvShenShenGeWindow>() != null)
            UiManager.Instance.getWindow<NvShenShenGeWindow>().init();
        MaskWindow.UnlockUI();
    }

    public void init(int typee,Prop choosedProp,int local) {//初始化神格(替换和镶嵌有区别：镶嵌prop 是 null)
        type = typee;
        prop = choosedProp;
        index = local;
        ArrayList allList = ShenGeManager.Instance.getShowShenGeList(prop);
        contentShenGe.reLoad(allList,type,index);
    }

	/** 点击事件 */
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow ();
            MaskWindow.UnlockUI();
		}
	}
}
