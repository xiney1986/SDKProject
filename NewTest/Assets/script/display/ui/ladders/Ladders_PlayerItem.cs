using UnityEngine;
using System.Collections;
/// <summary>
/// 天梯主窗口中——玩家
/// </summary>
public class Ladders_PlayerItem : ButtonBase
{
	public UITexture texture_HeadIcon;
	public UILabel label_Level;
	public UILabel label_Rank;
	public UISprite sprite_Flag;
	public UISprite sprite_Vip;
	public LaddersPlayerInfo data;
	[HideInInspector]
	public int index;
	private int rangeLevel = 0;//机器人随机等级

	public void M_update (LaddersPlayerInfo _data)
	{
		data = _data;
		M_updateView ();
	}

	private void M_updateView ()
	{
		if (data.level == 0 && rangeLevel == 0) {
			rangeLevel = Random.Range (LaddersConfigManager.Instance.config_Const.robotMinLv, LaddersConfigManager.Instance.config_Const.robotMaxLv);
		}

		if(LaddersManagement.Instance.CurrentOppPlayer != null)
		{
			if(data.rank == LaddersManagement.Instance.CurrentOppPlayer.rank)
			{
				data.isDefeated = LaddersManagement.Instance.CurrentOppPlayer.isDefeated;
			}
			LaddersManagement.Instance.CurrentOppPlayer = null;
		}

		if (data.isDefeated) {
			sprite_Flag.gameObject.SetActive (true);
			label_Rank.gameObject.SetActive (false);
		} else {
			sprite_Flag.gameObject.SetActive (false);
			label_Rank.gameObject.SetActive (true);
			label_Rank.text = Language ("laddersTip_13", data.rank.ToString ());
		}

		if (data.level == 0) {
			label_Level.text = "Lv." + rangeLevel;
		} else {
			label_Level.text = "Lv." + data.level;
		}
		if (data.vip > 0) {
			sprite_Vip.gameObject.SetActive (true);
			sprite_Vip.spriteName = "vip" + data.vip;
		} else {
			sprite_Vip.gameObject.SetActive (false);
		}
		ResourcesManager.Instance.LoadAssetBundleTexture (data.getHeadIconPath (), texture_HeadIcon);
	}

	public override void DoClickEvent ()
	{
		if (data.isDefeated) {//如果已经被打败 就不能再打了
			MaskWindow.UnlockUI ();
			return;
		}
		(fatherWindow as LaddersWindow).M_onClickPlayer (this);
	}
}

