using UnityEngine;
using System.Collections;

/// <summary>
/// 积分奖励容器
/// </summary>
public class SourceForActiveContent : MonoBehaviour {

	/** 打开类型 */
	private int openType;
	/** 容器 */
	public SourceActiveContent content;
	/** 积分 */
	private int integral;
	/** 积分标签 */
	public UILabel mySourceLabel;
	//***/
	public int useType; //1,抽卡 2,抽装备 3,星魂 4,炼金

	//初始化方法
	public void init(WindowBase win,int noticeSid,int source,int uType){
		integral=source;
		content.fatherWindow=win;
		openType=noticeSid;
		useType = uType;
		if (useType == 1 || useType == 2) {
			LuckyNoticeShowAwardFPort fport = FPortManager.Instance.getFPort ("LuckyNoticeShowAwardFPort") as LuckyNoticeShowAwardFPort;
			fport.showAwardAccess (noticeSid, () => {
				RankAward[] awards = SortAward ();
				content.reLoad (awards, integral);
				updateUI ();
			});
		}
		else {
			LuckyXianshiShowAwardFPort fport = FPortManager.Instance.getFPort ("LuckyXianshiShowAwardFPort") as LuckyXianshiShowAwardFPort;
			fport.showAwardAccess (noticeSid, () => {
				RankAward[] awards = SortAward ();
				content.reLoad (awards, integral);
				updateUI ();
			});
		}
	}
	/***/
	public void updateUI() {
		mySourceLabel.text=LanguageConfigManager.Instance.getLanguage("Arena51")+integral.ToString();
	}
	/** button点击事件 */
	public void buttonEventBase (GameObject gameObj) {
		MaskWindow.UnlockUI();
	}
	/** 奖励 */
	RankAward[] SortAward () {
		RankAward[] awards = LucklyActivityAwardConfigManager.Instance.getSource (openType);
		return awards;
	}
}
