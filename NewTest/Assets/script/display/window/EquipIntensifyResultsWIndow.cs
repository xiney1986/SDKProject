using UnityEngine;
using System.Collections;

public class EquipIntensifyResultsWIndow : WindowBase
{
 
	public	ExpbarCtrl expbar;
	public	UITexture equipImage;
	public	UISprite qualityBack;
	public UILabel nameLabel;
	public UILabel level;
	public UILabel attrLabel1;
	public UILabel attrLabel2;
	public GameObject audio;
	private Equip equip;
	public const int INTENEQUIP = 1;
	private int intoType;
	private int targetLv;
	/** 显示的当前等级 */
	private  int nowLv;
	/** 当前装备的经验模版SID */
	private int equipExpSID;
	/** 当前装备的总经验,随进度条动态更新 */
	private long tipNowExp;
	/** 当前装备的经验上限，随进度条动态更新 */
	private long tipMaxExp;
	/** 经验显示Label */
	public UILabel tipProgressLabel;


	protected override void begin ()
	{
		base.begin ();
		GuideManager.Instance.guideEvent ();
		MaskWindow.UnlockUI ();
	}
	//初始化，带入3大类技能的升级信息
	public void Initialize (Equip _equip, LevelupInfo info)
	{
		/* 初始化数据 */
		equip = _equip;
		equipExpSID =  EquipmentSampleManager.Instance.getEquipSampleBySid (equip.sid).levelId;	
		nowLv = info.oldLevel;
		targetLv = info.newLevel;
		tipNowExp = info.oldExp;
		updateProgressLabel ();
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + equip.getIconId (), equipImage);
		qualityBack.spriteName = QualityManagerment.qualityIDToIconSpriteName (equip.getQualityId ());
		level.text = "Lv." + info.oldLevel + "/" + equip.getMaxLevel ();
		nameLabel.text = equip.getName ();	
		AttrChange[] attrsOld = equip.getAttrChanges (info.oldLevel);
		AttrChange[] attrsNew = equip.getAttrChanges (info.newLevel);		
		tipProgressLabel.text = "";
		attrLabel1.text = "";
		attrLabel2.text = "";		
		if (attrsOld != null) {
			if (attrsOld.Length > 0 && attrsOld [0] != null)
                attrLabel1.text = "[6E483F]" + attrsOld[0].typeToString() + attrsOld[0].num + "[-][418159] + " + (attrsNew[0].num - attrsOld[0].num);	
			if (attrsOld.Length > 1 && attrsOld [1] != null)
                attrLabel2.text = "[6E483F]" + attrsOld[1].typeToString() + attrsOld[1].num + "[-][418159] + " + (attrsNew[1].num - attrsOld[1].num);	
		}
		expbar.init (info);
		/** 进度条升级回调 */
		expbar.setLevelUpCallBack (showLevelupSign);
		/** 进度条动画播完回调 */
		expbar.endCall = OnExpBarEnd;

	

	}

	public void OnExpBarEnd(){
		if (targetLv - nowLv > 0) {
			EffectManager.Instance.CreateEffectCtrlByCache (equipImage.transform, "Effect/UiEffect/equipIntensifyResults", null);
			if(!audio.activeSelf)
				audio.SetActive (true);
		}
		tipNowExp = equip.exp;
		updateProgressLabel ();
	}

	public void OnExpBarProgress (int sp)
	{
		tipNowExp += sp;
		updateProgressLabel ();
	}

	public void showLevelupSign (int now)
	{
		nowLv += 1;
		level.text = "Lv." + nowLv + "/" + equip.getMaxLevel ();
		//更新到下一级别 区间
		tipNowExp = EXPSampleManager.Instance.getEXPDown (equipExpSID, nowLv);
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			if (fatherWindow is IntensifyEquipWindow) {
				(fatherWindow as IntensifyEquipWindow).reloadAfterIntensify (equip);
			}
			finishWindow ();
			GuideManager.Instance.doGuide ();
			GuideManager.Instance.guideEvent ();
		}
	}

	private void updateProgressLabel( )
	{
		tipProgressLabel.text = EXPSampleManager.Instance.getExpBarShow (equipExpSID, tipNowExp);
	}


}
