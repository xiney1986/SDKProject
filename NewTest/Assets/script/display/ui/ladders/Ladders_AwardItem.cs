using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 天梯奖励Item
/// </summary>
public class Ladders_AwardItem : MonoBase
{
	public GameObject prefab_GoodsView;
	public UISprite sprite_title;
	public GameObject root_prize;
	public UILabel label_name;
	public UILabel rmbLabel;


	//奖章图标
	public UISprite sprite_medal_icon;
	//奖章描述1
	public UILabel label_des_01;
	//奖章描述2
	public UILabel label_des_02;
	//奖章奖励标题
	public UILabel label_medal_title;
	//没有奖章奖励时，纯文字显示
	public UILabel label_no_medal;
	private LaddersAwardSample data;
	public UIScrollView parentScrollView;

	/// <summary>
	/// 更新奖励
	/// </summary>
	/// <param name="_data">_data.</param>
	public void M_update (LaddersAwardSample _data)
	{
		data = _data;
		label_name.text = data.name;

		PrizeSample[] prizes = data.samples;
		GameObject newGo;
		GoodsView goodsButton;

		UIUtils.M_removeAllChildren (root_prize);
		for (int i = 0,length=prizes.Length; i<length; i++) {

			newGo = NGUITools.AddChild (root_prize, prefab_GoodsView);
			newGo.SetActive (true);
			newGo.name = StringKit.intToFixString (i + 1);
			newGo.GetComponent<UIDragScrollView> ().scrollView = parentScrollView;
			goodsButton = newGo.GetComponent<GoodsView> ();
			//goodsButton.fatherWindow = this;
			//goodsButton.onClickCallback = goodsButton.DefaultClickEvent;
			goodsButton.init (prizes [i]);
		}
		root_prize.GetComponent<UIGrid> ().Reposition ();


		LaddersMedalSample medal = LaddersConfigManager.Instance.config_Medal.M_getMedal (_data.minRank);

		//不存在奖章奖励时，显示：排名500名以外，无奖章奖励
		if (medal == null) {
			HideMedalInfo ();
		}
		//存在奖章奖励时，显示奖章奖励
		else {
			ShowMedalInfo (medal);
		}

		/*
		if(data.index>=5)
		{
			sprite_nomal_bg.gameObject.SetActive(true);
			sprite_medal_bg.gameObject.SetActive(false);
		}else
		{
			sprite_nomal_bg.gameObject.SetActive(false);
			sprite_medal_bg.gameObject.SetActive(true);
			sprite_medal_bg.spriteName="medal_"+(data.index+1);
		}
		*/
	}
	/// <summary>
	/// 有奖章奖励时，显示奖章奖励
	/// </summary>
	/// <param name="medal">Medal.</param>
	void ShowMedalInfo (LaddersMedalSample medal)
	{
		label_no_medal.gameObject.SetActive (false);
		sprite_medal_icon.gameObject.SetActive (true);
		label_des_01.gameObject.SetActive (true);
		label_des_02.gameObject.SetActive (true);
		label_medal_title.gameObject.SetActive (true);
		sprite_medal_icon.spriteName = "medal_" + Mathf.Min (medal.index + 1, 5);
		label_medal_title.text = Language ("laddersTip_36");
		label_des_01.text = medal.addDescriptions [0];
		label_des_02.text = medal.addDescriptions [1];

	}

	/// <summary>
	/// 没有奖章奖励时，隐藏奖章奖励
	/// </summary>
	void HideMedalInfo ()
	{
		label_no_medal.gameObject.SetActive (true);
		sprite_medal_icon.gameObject.SetActive (false);
		label_des_01.gameObject.SetActive (false);
		label_des_02.gameObject.SetActive (false);
		label_medal_title.gameObject.SetActive (false);

		label_no_medal.text = Language ("laddersTip_37");
	}
	
}

