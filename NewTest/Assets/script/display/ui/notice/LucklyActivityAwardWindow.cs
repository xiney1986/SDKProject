using UnityEngine;
using System.Collections;

/// <summary>
/// 限时活动奖励窗口
/// </summary>
public class LucklyActivityAwardWindow : WindowBase {

	/* const */
	/** 容器下标常量 */
	const int TAP_RINK_CONTENT=0, // 排名奖励
						TAP_SOURCE_CONTENT=1; // 积分奖励
	/** 预制件容器数组-排名奖励,积分奖励*/
	public GameObject[] contentPrefabs;
	/**预制件挂接点 */
	public GameObject[] contentPoints;
	/** 当前tap下标--0开始 */
	int currentTapIndex=0;
	/** tap容器 */
	public TapContentBase tapContent;
	/** 打开的类型(限时抽卡活动SID,限时抽装备活动SID) */
	private int openType=0;
	/** 源数据 */
	private int mySource;
	//***/
	public int useType;//1,抽卡 2,抽装备 3,星魂 4,炼金

	public string descInfo; //描述信息
	/**method */
	protected override void begin ()
	{
		base.begin ();
		doBegin();
		MaskWindow.UnlockUI ();
	}
	/** 执行begin */
	public void  doBegin() {
		if(!isAwakeformHide) {
			tapContent.changeTapPage(tapContent.tapButtonList[currentTapIndex]);
		} else{
			UpdateContent();
			MaskWindow.UnlockUI ();
		}
	}
	/// <summary>
	/// 初始化
	/// </summary>
	/// <param name="type">活动SID</param>
	/// <param name="source">源数据</param>
	public void init(int type,int source,int uType) {
		openType=type;
		mySource=source;
		useType = uType;
	}
	/// <summary>
	/// 初始化容器
	/// </summary>
	/// <param name="tapIndex">Tap index.</param> 
	public void initContent(int tapIndex) {
		resetContentsActive();
		GameObject content = getContent (tapIndex);
		switch (tapIndex) {
			case TAP_RINK_CONTENT:
				RankingForActiveContent csp = content.GetComponent<RankingForActiveContent> ();
				csp.init(this,openType);
				int minIntegral=0;
				RankAward rankAward=LucklyActivityAwardConfigManager.Instance.getFirstSource(openType);
				if(rankAward!=null) {
					minIntegral=rankAward.needSource;
				}
				csp.rankTextLabel.text=LanguageConfigManager.Instance.getLanguage("luckinfo",minIntegral.ToString());
				if(useType == 3 || useType ==4)
				csp.rankTextLabel.text = descInfo.Replace("%1",minIntegral.ToString());
				break;
			case TAP_SOURCE_CONTENT: 
				SourceForActiveContent cms = content.GetComponent<SourceForActiveContent> ();
				cms.init(this,openType,mySource,useType);
				break;
		}
	}
	/** 重置容器激活状态 */
	private void resetContentsActive() {
		foreach (GameObject item in contentPoints) {
			item.SetActive(false);
		}
	}
	/** 更新节点容器 */
	public void UpdateContent() {
		GameObject content = getContent (currentTapIndex);
		if(currentTapIndex==TAP_RINK_CONTENT){
			RankingForActiveContent csp=content.GetComponent<RankingForActiveContent>();
			csp.updateUI();
		}else if(currentTapIndex==TAP_SOURCE_CONTENT){
			SourceForActiveContent cms = content.GetComponent<SourceForActiveContent> ();
			cms.updateUI();
		}
		
	}
	/// <summary>
	/// 获取指定下标的容器
	/// </summary>
	/// <param name="contentPoint">容器点</param>
	/// <param name="tapIndex">下标</param>
	private GameObject getContent(int tapIndex) {
		GameObject contentPoint = contentPoints [tapIndex];
		contentPoint.SetActive (true);
		GameObject content;
		if (contentPoint.transform.childCount > 0) {
			Transform childContent=contentPoint.transform.GetChild (0);
			content = childContent.gameObject;
		} else {
			content = NGUITools.AddChild (contentPoint, contentPrefabs[tapIndex]);
		}
		return content;
	}
	//断线重新连接
	public override void OnNetResume ()
	{
		base.OnNetResume ();
		UpdateContent();
	}
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj); 
		if (gameObj.name == "close") {
			finishWindow ();
		} else {
			GameObject content = getContent (currentTapIndex);
			if (currentTapIndex == TAP_RINK_CONTENT) {
				RankingForActiveContent sshc = content.GetComponent<RankingForActiveContent> ();
				sshc.buttonEventBase(gameObj);
			} else if (currentTapIndex == TAP_SOURCE_CONTENT) {
				SourceForActiveContent ssec = content.GetComponent<SourceForActiveContent> ();
				ssec.buttonEventBase(gameObj);
			}
		}
	}
	public override void tapButtonEventBase (GameObject gameObj, bool enable)
	{ 
		if (!enable)
			return;
		base.buttonEventBase (gameObj);
		int tapIndex=int.Parse (gameObj.name)-1;
		this.currentTapIndex=tapIndex;
		initContent (tapIndex);
	}

}
