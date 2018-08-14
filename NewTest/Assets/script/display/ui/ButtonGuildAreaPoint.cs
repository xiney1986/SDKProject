using UnityEngine;
using System.Collections;
/// <summary>
/// 公会战,领地节点(玩家,NPC)
/// </summary>
public class ButtonGuildAreaPoint : ButtonBase {
	/** 血条 */
	public barCtrl blood;
	public UILabel bloodLabel;
	/** 名字 */
	public UILabel name;
	/** 底座 */
	public UISprite baseBG;
	/** 底座左边装饰 */
	public UISprite baseBGL;
	/** 底座右边装饰 */
	public UISprite baseBGR;
	/** 玩家头像*/
	public UITexture headTex;
	/** 头像背景 */
	public UISprite headBG;

	/** vip*/
	public UISprite vip;
	private GuildAreaPoint point;
	public void updateUI(GuildAreaPoint point ){
		this.point = point;
		bloodLabel.text = point.bloodNow + "/" + point.bloodMax;
		blood.updateValue (point.bloodNow, point.bloodMax);
		vip.spriteName = "vip_" + point.vipLevel;
        name.text = point.getName();
        ResourcesManager.Instance.LoadAssetBundleTexture(point.getHeadIconPath(), headTex);	
		if (point.bloodNow == 0)
			showDeadUI ();
		
	}

	/// <summary>
	/// 死亡状态(各UI置灰)
	/// </summary>
	private void showDeadUI(){
		baseBG.spriteName = "mission_notopen";
		baseBGL.spriteName = "pattern_1_gray";
		baseBGR.spriteName = "pattern_1_gray";
		headBG.spriteName = "qualityIconBack_1";
		headTex.color = Color.gray;
		name.color = Color.gray;
		blood.updateValue (0f, 1.0f);
		bloodLabel.gameObject.SetActive (false);
	}
}
