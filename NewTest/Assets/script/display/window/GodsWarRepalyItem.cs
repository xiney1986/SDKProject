using UnityEngine;
using System.Collections;

public class GodsWarRepalyItem : MonoBase 
{
	/// <summary>
	/// 玩家名称
	/// </summary>
	public UILabel lblName;
	/// <summary>
	/// 局数
	/// </summary>
	public UILabel lblValue;
	/// <summary>
	/// 查看战报按钮
	/// </summary>
	public ButtonBase replayButton;
	/// <summary>
	/// 战报id
	/// </summary>
	private string repalyId;
	/// <summary>
	/// 玩家实体
	/// </summary>
	public GodsWarFinalUserInfo winner;
	/// <summary>
	/// 父窗口
	/// </summary>
	public WindowBase fatherWindow;
	/// <summary>
	/// 局数
	/// </summary>
	private int value;
	
	CallBack callback;
	
	/// <summary>
	/// 初始化item
	/// </summary>
	public void initItem(GodsWarFinalUserInfo winner,int value,string replayId,WindowBase win,CallBack callbck)
	{
		this.fatherWindow = win;
		this.winner = winner;
		this.repalyId = replayId;
		this.value = value;
		this.callback = callbck;
		initButton();
		initUI();
	}
	/// <summary>
	/// 初始化按钮
	/// </summary>
	public void initButton()
	{
		replayButton.fatherWindow = fatherWindow;
		replayButton.onClickEvent = doRepalyEvent;
	}
	/// <summary>
	/// 初始化UI
	/// </summary>
	public void initUI()
	{
		lblName.text = "[87373e]【"+winner.name+winner.serverName +"】[-]"+"[0C8A0F]"+LanguageConfigManager.Instance.getLanguage("godsWar_62")+"[-]";
		setLBLValue(value);
	}
	/// <summary>
	/// 执行支持按钮
	/// </summary>
	public void doRepalyEvent(GameObject obj)
	{
		GameManager.Instance.battleReportCallback=	GameManager.Instance.intoBattleForGodsWar;
		FPortManager.Instance.getFPort<GodsWarFinalChallengeFport> ().access ((success) => {
			if (success) {
				MaskWindow.instance.setServerReportWait(true);
				if(callback!=null)
					callback();
			} else {
				GameManager.Instance.battleReportCallback=null;
				MaskWindow.instance.setServerReportWait(false);
			}
			//UiManager.Instance.clearWindows ();

		},repalyId);
	}
	/// <summary>
	/// 更新Ui
	/// </summary>
	public void updateUI()
	{
	}
	/// <summary>
	/// 获取局数标签
	/// </summary>
	public void setLBLValue(int value)
	{
		switch (value) {
		case 0:
			lblValue.text = LanguageConfigManager.Instance.getLanguage("godsWar_63");
			break;
		case 1:
			lblValue.text = LanguageConfigManager.Instance.getLanguage("godsWar_65");
			break;
		case 2:
			lblValue.text = LanguageConfigManager.Instance.getLanguage("godsWar_66");
			break;
		case 3:
			lblValue.text = LanguageConfigManager.Instance.getLanguage("godsWar_87");
			break;
		case 4:
			lblValue.text = LanguageConfigManager.Instance.getLanguage("godsWar_88");
			break;
		default:
			break;
		}
	}
}
