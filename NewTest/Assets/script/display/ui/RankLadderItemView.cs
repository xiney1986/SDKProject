using UnityEngine;
using System.Collections;

public class RankLadderItemView : MonoBase
{
	public UILabel label_name;
	public UILabel label_guild;
	public UILabel label_combat;
	public UILabel label_title;
	public UILabel label_rank;
	public UILabel label_level;
	public UILabel label_horoscope;
	public WindowBase fatherWindow;
	public ButtonBase childButton;

	public UITexture texture_head;


//	public UISprite sprite_horoscope_icon;
	public UISprite sprite_title_bg;
	public UISprite sprite_vip;
	public UISprite sprite_rankFlag;

	public PvpOppInfo data;

	public void setFatherWindow(WindowBase father){
		childButton.setFatherWindow (father);
	}

	public void M_update(PvpOppInfo _data,int index)
	{
		data=_data;
		index=_data.ladderRank;

		label_name.text = _data.name;
		if (string.IsNullOrEmpty (data.guildName)) {
			label_guild.text=Language("pvpPlayerWindow02")+ Language("GuildLuckyNvShen_16");
		} else {
			label_guild.text=Language("pvpPlayerWindow02")+ _data.guildName;
		}

		label_combat.text=Language("laddersPrefix_02")+_data.combat.ToString();

		label_rank.text=index.ToString();


		if(index<=3)
		{
			label_rank.text = "";
			sprite_rankFlag.gameObject.SetActive(true);
			sprite_rankFlag.spriteName="rank_"+index.ToString();
		}else
		{
			sprite_rankFlag.gameObject.SetActive(false);
		}

		ResourcesManager.Instance.LoadAssetBundleTexture (UserManager.Instance.getIconPath (_data.headIcon), texture_head);
		if(_data.vipLv>0)
		{
			sprite_vip.gameObject.SetActive(true);
			sprite_vip.spriteName="vip"+_data.vipLv;
		}else
		{
			sprite_vip.gameObject.SetActive(false);
		}
		int level = EXPSampleManager.Instance.getLevel (1, _data.exp, 0);
		label_level.text=level.ToString();

		label_horoscope.text = HoroscopesManager.Instance.getStarByType (_data.star).getName ();
//		sprite_horoscope_icon.spriteName = HoroscopesManager.Instance.getStarByType (_data.star).getSpriteName ();


		LaddersTitleSample sample_1=LaddersConfigManager.Instance.config_Title.M_getTitle(_data.prestige);
		if(sample_1==null)
		{
			label_title.text=Language("laddersTip_20");
		}else
		{
			label_title.text=sample_1.name;
		}

		LaddersMedalSample sample_2=LaddersConfigManager.Instance.config_Medal.M_getMedalBySid(_data.medalSid);
		if(sample_2==null)
		{
			sprite_title_bg.spriteName="medal_0";
		}else
		{
			sprite_title_bg.spriteName="medal_"+Mathf.Min(sample_2.index+1,5);
		}
	}
}

