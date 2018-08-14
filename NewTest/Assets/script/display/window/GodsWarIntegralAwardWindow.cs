using UnityEngine;
using System.Collections;

/// <summary>
/// 诸神战积分奖励窗口
/// </summary>
public class GodsWarIntegralAwardWindow : WindowBase {
	/** 我的当前积分 */
	//public UILabel myIntergalLabel;
    //**奖励item*/
    public GameObject godsWarIntegralItemPrefab;
	public GodsWarIntegralAwardContent content;
	CallBack callback;

	protected override void begin ()
	{
		base.begin ();
	}
	/// <summary>
	/// 初始化UI
	/// </summary>
	public void initUI(CallBack callback){
        content.init(this,callback);
		content.fatherWindow = this;
		MaskWindow.UnlockUI ();
	}
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
        if (gameObj.name == "ButtonClose")
        {
            finishWindow();
        }
	}
}
