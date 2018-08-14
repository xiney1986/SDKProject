using UnityEngine;
using System.Collections;

/// <summary>
/// 许愿弹出的属性窗口
/// </summary>
public class FestivalWishWindow : WindowBase {

	/** 许愿价*/
	public UILabel wishCostLabel;
	/** 拥有的钻石*/
	public UILabel ownRMBLabel;

	private int wishCost;

	private int sid;
	/** 确认许愿*/
	public ButtonBase wishButton;

	public CallBack callback;


	protected override void begin () {
		base.begin ();
		MaskWindow.UnlockUI ();
	}
	/// <summary>
	/// 初始化窗口
	/// </summary>
	public void initWindow(int wishCost,int sid,CallBack callback)
	{
		this.wishCost = wishCost;
		this.sid = sid;
		this.callback = callback;
		updateUI();
		if(!checkCondition())
		{
			ownRMBLabel.color = Color.red;
			wishButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("s0324");
			//wishButton.disableButton(true);
		}
	}
	/// <summary>
	/// 点击事件
	/// </summary>
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if(gameObj.name=="close")
		{
			this.dialogCloseUnlockUI=true;
			finishWindow();
		}
		else if(gameObj.name=="enter")
		{
			if(!checkCondition())
			{
				UiManager.Instance.openWindow<rechargeWindow>();
				finishWindow();
				return;
			}
			FestivalWishDoClickFPort fport = FPortManager.Instance.getFPort<FestivalWishDoClickFPort>();
			fport.access(sid,()=>{
//				updateUI();
				if(callback!=null)
				{
					callback();
				}
			});
			finishWindow();
		}

	}

	/// <summary>
	/// 检查钱是否够
	/// </summary>
	private bool checkCondition()
	{
		if(UserManager.Instance.self.getRMB()<wishCost)
		{
			return false;
		}
		return true;
	}
	/// <summary>
	/// 更新UI
	/// </summary>
	public void updateUI()
	{
		wishCostLabel.text = wishCost.ToString();
		ownRMBLabel.text = UserManager.Instance.self.getRMB().ToString();
	}

}
