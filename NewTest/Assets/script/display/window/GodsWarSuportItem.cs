using UnityEngine;
using System.Collections;

public class GodsWarSuportItem : MonoBase 
{
	/// <summary>
	/// 玩家展示
	/// </summary>
	public UITexture icon;
	/// <summary>
	/// 玩家名称
	/// </summary>
	public UILabel lblName;
	/// <summary>
	/// 战斗力 
	/// </summary>
	public UILabel lblPower;
	/// <summary>
	/// 等级
	/// </summary>
	public UILabel lblLevel;
	/// <summary>
	/// 力挺消耗
	/// </summary>
	public UILabel lblCostRmb;
	/// <summary>
	/// 支持按钮
	/// </summary>
	public ButtonBase suportButton;
	/// <summary>
	/// 力挺
	/// </summary>
	public ButtonBase suportRMBButton;
	/// <summary>
	/// 支持人数
	/// </summary>
	public UILabel lblSuportNum;
	/// <summary>
	/// 玩家实体
	/// </summary>
	public GodsWarFinalUserInfo user;
	/// <summary>
	/// 父窗口
	/// </summary>
	public WindowBase fatherWindow;
	/// <summary>
	/// 位置点
	/// </summary>
	private int localID;
	/// <summary>
	/// 大组
	/// </summary>
	private int type;
	/// <summary>
	/// 域名
	/// </summary>
	private int index;

	CallBack callback;

	/// <summary>
	/// 初始化item
	/// </summary>
	public void initItem(GodsWarFinalUserInfo user,int type,int index,int localID,WindowBase win,CallBack callbck)
	{
		this.fatherWindow = win;
		this.user = user;
		this.type = type;
		this.index= index;
		this.localID = localID;
		this.callback = callbck;
		initButton();
		initUI();
	}
	/// <summary>
	/// 初始化按钮
	/// </summary>
	public void initButton()
	{
		suportButton.fatherWindow = fatherWindow;
		suportButton.onClickEvent = doSuportEvent;
		suportRMBButton.fatherWindow = fatherWindow;
		suportRMBButton.onClickEvent = doRMBSuportEvent;
	}
	/// <summary>
	/// 初始化UI
	/// </summary>
	public void initUI()
	{
		lblName.text = user.name;
        int a = GodsWarManagerment.Instance.getTypeByLocalId(localID);
	    int[] costM = GodsWarInfoConfigManager.Instance().getSampleBySid(5001).num;
	    int coost = costM[a >= costM.Length ? costM.Length - 1 : a - 1];
		lblCostRmb.text = coost.ToString();
		lblPower.text = LanguageConfigManager.Instance.getLanguage("godsWar_59",user.power.ToString());
		lblLevel.text = "Lv."+user.level;
		lblSuportNum.text = LanguageConfigManager.Instance.getLanguage("godsWar_60",user.suportPlayerNum.ToString());
		ResourcesManager.Instance.LoadAssetBundleTexture (UserManager.Instance.getIconPath (user.headIcon), icon);

	}
	/// <summary>
	/// 执行支持按钮
	/// </summary>
	public void doSuportEvent(GameObject obj)
	{
		FPortManager.Instance.getFPort<GodsWarSendSuportFPort>().access(user.serverName,user.uid,type,index,localID,0,updateUI);
	}

	/// <summary>
	/// 执行rmb支持按钮
	/// </summary>
	public void doRMBSuportEvent(GameObject obj)
	{
        int a = GodsWarManagerment.Instance.getTypeByLocalId(localID);
        int[] costM = GodsWarInfoConfigManager.Instance().getSampleBySid(5001).num;
        int costRmb = costM[a >= costM.Length ? costM.Length - 1 : a - 1];
		if(UserManager.Instance.self.getRMB() < costRmb){
			UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
				win.initWindow(2,LanguageConfigManager.Instance.getLanguage("s0094"),LanguageConfigManager.Instance.getLanguage("s0324"),
				               LanguageConfigManager.Instance.getLanguage("godsWar_105"),(msg)=>{
					if (msg.buttonID == MessageHandle.BUTTON_RIGHT) {
                       // fatherWindow.finishWindow();
						UiManager.Instance.openWindow<rechargeWindow> (); 
					}
					else
					{
						MaskWindow.UnlockUI();
					}		
				});
			});
            fatherWindow.finishWindow();
		}
		else{
			UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
				
				win.initWindow(2,LanguageConfigManager.Instance.getLanguage("s0094"),LanguageConfigManager.Instance.getLanguage("s0093"),
				               LanguageConfigManager.Instance.getLanguage("godsWar_106",costRmb.ToString(),user.name),(msg)=>{
					if (msg.buttonID == MessageHandle.BUTTON_RIGHT) {
						FPortManager.Instance.getFPort<GodsWarSendSuportFPort>().access(user.serverName,user.uid,type,index,localID,1,updateUI);
					}
					else
					{
						MaskWindow.UnlockUI();
					}
					
				});
			});
		}
	}
	/// <summary>
	/// 更新Ui
	/// </summary>
	public void updateUI()
	{
		UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("godsWar_57",user.name));
		suportButton.disableButton(true);
		suportRMBButton.disableButton(true);
		suportButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("godsWar_58");
		lblSuportNum.text = user.suportPlayerNum+1+"";
		if(callback!=null)
			callback();
	}
}
