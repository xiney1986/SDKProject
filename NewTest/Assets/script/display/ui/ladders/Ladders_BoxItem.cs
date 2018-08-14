using UnityEngine;
using System.Collections;
/// <summary>
/// 天梯主窗口中——宝箱
/// </summary>
public class Ladders_BoxItem : ButtonBase
{

	public UITexture texture_Box;

	public UILabel label_Info;
	public UISprite sprite_Flag;
	public UISprite sprite_BoxBg;

	public UILabel label_super;

	public GameObject ani_receive;


	public LaddersChestInfo data;
	[HideInInspector]
	public int index;

	/// <summary>
	/// 传入 数据
	/// </summary>
	/// <param name="_data">_data.</param>
	public void M_update(LaddersChestInfo _data)
	{
		data=_data;
		M_updateView();
	}
	/// <summary>
	/// 更新视图 不同索引，宝箱对应的外观不同
	/// </summary>
	private void M_updateView()
	{
		label_Info.text=data.index.ToString()+"_Chest";
		if(data.receiveEnble)
		{
			ani_receive.SetActive(true);
		}else
		{
			ani_receive.SetActive(false);
		}
		switch(data.index)
		{
		case 1:
			sprite_BoxBg.spriteName="qualityIconBack_5";
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH+"6",texture_Box);
			if(data.multiple>100)
			{
				EffectManager.Instance.CreateEffect (transform, "Effect/UiEffect/ChestEffect");

				label_super.gameObject.SetActive(true);
				label_super.text=Language("laddersTip_15");
				label_Info.text=Language("laddersChest_02");
			}else
			{

				label_super.gameObject.SetActive(false);
				label_Info.text=Language("laddersChest_01");
			}
			break;
		case 2:
			sprite_BoxBg.spriteName="qualityIconBack_4";
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH+"53",texture_Box);
			if(data.multiple>100)
			{
				EffectManager.Instance.CreateEffect (transform, "Effect/UiEffect/ChestEffect");

				label_super.gameObject.SetActive(true);
				label_super.text=Language("laddersTip_15");
				label_Info.text=Language("laddersChest_04");
			}else
			{

				label_super.gameObject.SetActive(false);
				label_Info.text=Language("laddersChest_03");
			}
			break;
		case 3:
			sprite_BoxBg.spriteName="qualityIconBack_3";
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH+"7",texture_Box);
			if(data.multiple>100)
			{
				EffectManager.Instance.CreateEffect (transform, "Effect/UiEffect/ChestEffect");
				label_super.gameObject.SetActive(true);
				label_super.text=Language("laddersTip_15");
				label_Info.text=Language("laddersChest_06");
			}else
			{
				label_super.gameObject.SetActive(false);
				label_Info.text=Language("laddersChest_05");
			}
			break;
		}
	}
	/// <summary>
	/// 鼠标点击
	/// </summary>
	public override void DoClickEvent ()
	{
		//base.DoClickEvent ();
		(fatherWindow as LaddersWindow).M_onClickBox(this);
	}
}

