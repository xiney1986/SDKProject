using System;

/// <summary>
/// 天梯记录信息封装
/// </summary>
public class LaddersRecordInfo
{
	public string description;
	public string oppUid = string.Empty;//对手信息
	public int time = 0;//时间
	public bool isWin;//敌人是否胜利
	public int rank;//排名变化

	public string enemyName;
	public int index;//是第几场
	public int vipLevel = 0;

	public LaddersRecordInfo ()
	{
	}

	public void creatDes ()
	{
		string timeInfo = TimeKit.dateToFormat (time, LanguageConfigManager.Instance.getLanguage ("notice06"));
		string vipInfo = vipLevel > 0 ? LanguageConfigManager.Instance.getLanguage ("laddersTip_06", vipLevel.ToString ()) : string.Empty;
		/*
		laddersTip_04|【%1】成功防守了玩家[EEFF00]%2[-]%3的挑战，名次不变[url=%4][00EEDD]【[u]重播[/u]】[-][/url];
		laddersTip_05|【%1】被玩家[EEFF00]%2[-]%3打败，名次降低到[EEFF00]%4[-]名[url=%5][00EEDD]【[u]重播[/u]】[-][/url];
		*/
		if (isWin) {
			description = LanguageConfigManager.Instance.getLanguage ("laddersTip_05", timeInfo, enemyName, vipInfo, rank.ToString ());
		} else {
			description = LanguageConfigManager.Instance.getLanguage ("laddersTip_04", timeInfo, enemyName, vipInfo);
		}
		//description += LanguageConfigManager.Instance.getLanguage ("laddersTip_07");
	}
}