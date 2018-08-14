using UnityEngine;
using System.Collections;

/// <summary>
/// 星魂图标弹出的属性窗口
/// </summary>
public class MoreAlchemyWindow : WindowBase {

	/* fields */
	/** 原始需要的钻石 */
	public UILabel needRMBnum;
	/**能够得到的最大金币数量 */
	public UILabel gaveMoney;
	public CallBack<bool> callback;
	private int needRMB=0;

	/***/
	protected override void begin () {
		base.begin ();
		//updateUI();
		MaskWindow.UnlockUI ();
	}
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if(gameObj.name=="close") {
			this.dialogCloseUnlockUI=true;
		}else if(gameObj.name=="enter"){

			callback (checkCondition());
		}
		finishWindow();
	}
	private bool checkCondition(){
		if(UserManager.Instance.self.getRMB()<needRMB){
			return false;
		}
		return true;
	}
	public void updateUI(){
		int vipLv=UserManager.Instance.self.getVipLevel();
		int reduceNum=vipLv > 0 ? VipManagerment.Instance.getVipbyLevel (vipLv).privilege.alchemyFactor : 0;
        needRMB = NoticeManagerment.Instance.getAlchemyConsumeAll(10);
        needRMBnum.text= needRMB.ToString();
		gaveMoney.text=NoticeManagerment.Instance.getAlchemyMoneyAll(10).ToString();
	}

}
