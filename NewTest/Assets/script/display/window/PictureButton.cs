using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// 图鉴中的卡片
/// </summary>
public class PictureButton : ButtonBase
{
	public PictureWindow window;
	/** 卡片名称 */
	public UILabel name;
	/** 卡片图标 */
	public UITexture icon;
	/** 品质背景 */
	public UISprite qualityBg;
	/** 卡片数据 */
	public Card card;
	public void init(Card card)
	{
		this.card = card;
		name.text = card.getName();
		qualityBg.spriteName = QualityManagerment.qualityIDToIconSpriteName (card.getQualityId());
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + card.getIconID ().ToString (), icon);
	}

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		window.OnItemClick (card);
	}
}
