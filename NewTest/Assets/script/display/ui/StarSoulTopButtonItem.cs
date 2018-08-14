using UnityEngine;
using System.Collections;

public class StarSoulTopButtonItem : ButtonBase {
	/**背景 */
	public UISprite bg_Sprite;
	/** 图标 */
	public UITexture icon;
	/**品质 */
	public UISprite quality;
	public ContentStarSoulEquiTop contentStarSoulEquitop;
	/**中间滑动面板 */
	public ContentShowItem contentShowItem;
	/** 品质环绕特效点 */
	private GameObject  qualityEffectPoint;
	private Card data;
	public float local_x;
	/** 品质环绕特效路径 */
	string qualityEffectPath;
	/** 品质环绕特效名 */
	public string[]  qualityEffectPaths;

	/** 初始化顶部的小头像图标按钮 */
	public void init(Card card) {
		this.onClickEvent=onItemMoveHander;
		if(contentStarSoulEquitop==null)contentStarSoulEquitop=transform.parent.GetComponent<ContentStarSoulEquiTop> ();
		data=card;
		if(data!=null)
		{
			if(data.isMainCard()){//如果是主卡
				ResourcesManager.Instance.LoadAssetBundleTexture (UserManager.Instance.self.getIconPath (), icon);
				icon.transform.localScale=new Vector3(0.9f,0.9f,0f);
			}else{
				ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + data.getIconID ().ToString (), icon);
				icon.transform.localScale=new Vector3(1f,1f,0f);
			}
			bg_Sprite.spriteName = QualityManagerment.qualityIDToIconSpriteName (data.getQualityId ());
			if (quality != null) {
				quality.spriteName = "quality_" + card.getQualityId ();
			}
		}		
	}
	/** 点击小头像图标跳转中间滑动面板 */
	public void onItemMoveHander(GameObject obj) {
		contentShowItem.jumpTo(StringKit.toInt(transform.gameObject.name)-1);
		MaskWindow.UnlockUI();
	}
	public override void OnAwake () {
		qualityEffectPath="Effect/UiEffect/Surroundeffect";
		qualityEffectPaths=new string[4];
		qualityEffectPaths[0]="Surroundeffect_b";
		qualityEffectPaths[1]="Surroundeffect_p";
		qualityEffectPaths[2]="Surroundeffect_y";
		qualityEffectPaths[3]="Surroundeffect_r";

	}
	/** 根据自身品质显示对应的环绕特效 */
	public void showEffectByQuality ()
	{
		if (qualityEffectPoint == null)
			return;
		if (data.getQualityId () < QualityType.COMMON)
			return;
		Utils.RemoveAllChild (qualityEffectPoint.transform);
		int index=(data.getQualityId () - QualityType.GOOD)<1?0:(data.getQualityId ()-QualityType.GOOD);
		EffectCtrl effectCtrl = EffectManager.Instance.CreateEffect(qualityEffectPoint.transform,qualityEffectPath,qualityEffectPaths [index]);
		effectCtrl.transform.localPosition = new Vector3(0,5,0);
		effectCtrl.transform.localScale = new Vector3(1.8f,1.8f,1);
		effectCtrl.transform.parent = qualityEffectPoint.transform;
	}
	/** 根据自身品质显示对应的环绕特效 */
	public void HideEffectByQuality ()
	{
		if (qualityEffectPoint == null)
			return;
		Utils.RemoveAllChild (qualityEffectPoint.transform);
	}
	/** 连接环绕特效点 */
	public void linkQualityEffectPointByRotate ()
	{
		qualityEffectPoint = transform.FindChild ("effectPoint").gameObject;
		qualityEffectPoint.SetActive (true);
	}
}
