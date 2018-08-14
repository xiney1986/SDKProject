using UnityEngine;
using System.Collections;

/// <summary>
/// 竞技场积分奖励窗口
/// </summary>
public class ArenaIntegralAwardWindow : WindowBase {
	/** 我的当前积分 */
	public UILabel myIntergalLabel;
    //**奖励item*/
    public GameObject arenaIntegralItemPrefab;
    public CallBack<int> callback;
	private int myIntegral;
    public ArenaIntegralAwardContent content;
    public int inccc = 0;

	protected override void begin ()
	{
		base.begin ();
	}
	/// <summary>
	/// 初始化UI
	/// </summary>
	public void initUI( ){
        content.init();
        myIntegral = ArenaManager.instance.finalMyIntergal;
		myIntergalLabel.text = myIntegral.ToString ();
		MaskWindow.UnlockUI ();
	}
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
        if (gameObj.name == "ButtonClose")
        {
            finishWindow();
            if (fatherWindow is ArenaAuditionsWindow)
            {
                callback(inccc);
                (fatherWindow as ArenaAuditionsWindow).numIntegral.updateIntegralUI();
            }
        }
	}
}
