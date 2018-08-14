using UnityEngine;
using System.Collections;
/// <summary>
/// 公会战贡献排行榜窗口
/// </summary>
public class GuildAreaHurtRankWindow : WindowBase {
	/** 容器 */
	public UIGrid content;

	public GameObject RankItemPrafab;
	public UILabel myRank;
	protected override void begin ()
	{
		base.begin ();
		if (!isAwakeformHide) {
			getRankInfo();
		}
		MaskWindow.UnlockUI ();
	}

	private void getRankInfo(){
		GuildGetAreaHurtRankFPort port = FPortManager.Instance.getFPort ("GuildGetAreaHurtRankFPort") as GuildGetAreaHurtRankFPort;
		port.access (callBack);
	}

	private void callBack(){
		init ();
	}


	private void init(){
		updateContent ();
		updateMyRank ();

	}

	/// <summary>
	/// 更新容器
	/// </summary>
	private void updateContent(){
		UIUtils.M_removeAllChildren (content.gameObject);
		for( int i = 0 ; i< RankManagerment.Instance.guildAreaHurtList.Count;i++){
			GuildAreaHurtRankItem item = RankManagerment.Instance.guildAreaHurtList[i];
			GameObject go = NGUITools.AddChild(content.gameObject,RankItemPrafab);
			RankItemView view = go.GetComponent<RankItemView>();
			view.init(item,RankManagerment.TYPE_GUILD_AREA_CONTRIBUTION,i);
		}
		content.Reposition ();
	}

	/// <summary>
	/// 更新我的排名
	/// </summary>
	private void updateMyRank(){
		string noRank = LanguageConfigManager.Instance.getLanguage ("GuildArea_21");
		myRank.text = LanguageConfigManager.Instance.getLanguage ("GuildArea_22", noRank);
		for(int i=0;i<RankManagerment.Instance.guildAreaHurtList.Count;++i){
			if(RankManagerment.Instance.guildAreaHurtList[i].uid == UserManager.Instance.self.uid){
				myRank.text = LanguageConfigManager.Instance.getLanguage ("GuildArea_22", (i+1).ToString());
				break;
			}
		}
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			this.finishWindow();
		}
	}

	public override void OnNetResume ()
	{
		base.OnNetResume ();
		getRankInfo ();
	}
}
