using UnityEngine;
using System.Collections;

public class GuildBossRankItem : MonoBehaviour
{
	public UILabel names;
	public UILabel prizeNum;
	public UILabel rank;
	public UISprite cup;

	public void init (GuildAltarRank guildRastInfo, int _rank)
	{
		names.text = guildRastInfo.playerName;
		if(guildRastInfo.playerName==UserManager.Instance.self.nickname)
		{
			this.rank.text ="[00FF00]" +_rank.ToString()+"[-]";
			prizeNum.text ="[00FF00]"+ "x"+GuildSinglePrizeSampleManager.Instance.getPrizesSumByHurtRank(_rank)+"[-]";
		}
		else
		{
			this.rank.text = _rank.ToString();
			prizeNum.text ="x"+GuildSinglePrizeSampleManager.Instance.getPrizesSumByHurtRank(_rank);

		}
		switch (_rank) 
		{
		case 1:
			cup.spriteName = "rank_1";
			break;
		case 2:
			cup.spriteName = "rank_2";
			break;
		case 3:
			cup.spriteName = "rank_3";
			break;
		default:
			cup.gameObject.SetActive (false);
			break;
		}
	}
}
