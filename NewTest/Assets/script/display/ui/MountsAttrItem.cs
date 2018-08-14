using UnityEngine;
using System.Collections;

/// <summary>
/// 坐骑属性条目
/// </summary>
public class MountsAttrItem : MonoBase {

	/* gameobj fields */
	/** 战斗力 */
	public UILabel combat;
	/** 生命 */
	public UILabel hp;
	/** 魔法 */
	public UILabel mag;
	/** 攻击 */
	public UILabel att;
	/** 敏捷 */
	public UILabel dex;
	/** 防御 */
	public UILabel def;
	/** 移动速度 */
	public UILabel speed;
	/** 生命 */
	public UILabel addHp;
	/** 魔法 */
	public UILabel addMag;
	/** 攻击 */
	public UILabel addAtt;
	/** 敏捷 */
	public UILabel addDex;
	/** 防御 */
	public UILabel addDef;
	/** 附加生命组 */
	public GameObject hpAddGroup;
	/** 附加魔法组 */
	public GameObject magAddGroup;
	/** 附加攻击组 */
	public GameObject attAddGroup;
	/** 附加敏捷组 */
	public GameObject dexAddGroup;
	/** 附加防御组 */
	public GameObject defAddGroup;
	/** 品质图标 */
	public UISprite qualitySprite;
	/** 3d模型点 */
	public GameObject mount3dModel;
	/** 坐骑模型阴影图 */
	public GameObject mountModelShadows;
	/** 父窗口 */
	WindowBase fatherWindow;

	/*  fields */
	/** 坐骑 */
	Mounts mounts;

	/* methods */
	/***/
	public void init(WindowBase fatherWindow,Mounts mounts) {
		this.fatherWindow=fatherWindow;
		this.mounts=mounts;
		UpdateUI();
	}
	/** 更新UI */
	public void UpdateUI() {
		update3DModel();
		updateAttr();
		updateQualitySprite();
		updateAddAttr();
		updateCombat();
	}
	/** 更新属性 */
	private void updateAttr() {
		if(mounts!=null) {
			CardBaseAttribute attr=mounts.getMountsAddEffect();
			hp.text=attr.hp.ToString();
			mag.text=attr.magic.ToString();
			att.text=attr.attack.ToString();
			dex.text=attr.agile.ToString();
			def.text=attr.defecse.ToString();
			speed.text=mounts.getSpeed()+"%";
		} else {
			hp.text="";
			mag.text="";
			att.text="";
			dex.text="";
			def.text="";
			speed.text="";
		}
	}
	/** 更新附加属性 */
	private void updateAddAttr() {
		hpAddGroup.SetActive(false);
		magAddGroup.SetActive(false);
		attAddGroup.SetActive(false);
		dexAddGroup.SetActive(false);
		defAddGroup.SetActive(false);
	}
	/** 更新战斗力 */
	private void updateCombat() {
		if(mounts!=null) {
			combat.text = "" + mounts.getCombat ();
		} else {
			combat.text="0";
		}
	}
	/** 更新品质图标 */
	public void updateQualitySprite() {
		if(mounts!=null)
			qualitySprite.spriteName = QualityManagerment.qualityIDToString (mounts.getQualityId ()) + "Bg";
		else
			qualitySprite.spriteName = "";
	}
	/** 更新3D模型 */
	private void update3DModel() {
		if(mount3dModel.transform.childCount>0)
			Utils.RemoveAllChild(mount3dModel.transform);
		if(mounts==null) {
			mountModelShadows.SetActive(true);
		} else {
			mountModelShadows.SetActive(false);
			createMountsModel((obj)=>{
				if(obj!=null) {
					FuBenCardCtrl mountsAnimCtrl=obj.transform.GetChild (0).GetComponent<FuBenCardCtrl> ();
					Utils.SetLayer (obj, UiManager.Instance.gameCamera.gameObject.layer);
					if (mountsAnimCtrl != null)
						mountsAnimCtrl.playStand();
					if(mounts.isInUse()) {
						//有坐骑的情况
						Transform saddle = mountsAnimCtrl.transform.FindChild ("saddle");
						Transform mountsRoot = mountsAnimCtrl.transform.parent;
						//把人插进马
						FuBenCardCtrl cardCtrl=createRoleModel(saddle);
						if(cardCtrl!=null) {
							cardCtrl.playMStand ();
                            //把精灵插进人
                            updateAngel(cardCtrl);
						}  
					}
				}
			});
		}
	}
    /**把天使插进人 */
    void updateAngel(FuBenCardCtrl cardCtrl)
    {
        if (PlayerPrefs.GetString(PlayerPrefsComm.ANGEL_USER_NAME + UserManager.Instance.self.uid) == "ok")
        {
            AngelSample angelsample=  AngelSampleManager.Instance.getAngelSampleByVipLevel(UserManager.Instance.self.getVipLevel());
            if (angelsample != null)
            {
                Transform angelPoint1 = cardCtrl.transform.FindChild("jingling");
                if (angelPoint1 != null)
                {
                    Transform angelPoint = angelPoint1.FindChild("jinglingPoint");
                    createangelModel(angelPoint, angelsample, (obj) => {
                        if (obj != null) {
                            FuBenCardCtrl angelAnimCtrl = obj.transform.GetChild(0).GetComponent<FuBenCardCtrl>();
                            Utils.SetLayer(obj, UiManager.Instance.gameCamera.gameObject.layer);
                            if (angelAnimCtrl != null)
                                angelAnimCtrl.playStand();
                        }
                    });
                }
               
            }
        }
    }
   
	/** 创建坐骑模型 */
	private void createMountsModel (CallBack<GameObject> callback) {
		ResourcesManager.Instance.LoadAssetBundleTexture (mounts.getModelPath(),mount3dModel.transform,(obj)=>{
			GameObject gameObj=obj as GameObject;
			Transform temp=gameObj.transform;	
            if (mounts.sid == 130208) temp.localScale = new Vector3(200, 200, 200);
            else temp.localScale = new Vector3(250, 250, 250);
			temp.localPosition = Vector3.zero;
			temp.localRotation = new Quaternion (0, 0, 0, 1);
			if (callback != null) {
				callback(gameObj);
			}
		});
	}
	/** 创建角色模型 */
	private FuBenCardCtrl createRoleModel (Transform parent) {
		passObj _obj = Create3Dobj (UserManager.Instance.self.getModelPath ()); 
		if (_obj.obj == null) {
			Debug.LogError ("role is null!!!");
			return null;
		} 
		_obj.obj.transform.parent = parent;
		_obj.obj.transform.localScale = new Vector3 (1.2f, 1.2f, 1.2f);
		_obj.obj.transform.localPosition = Vector3.zero;
		_obj.obj.transform.localRotation = new Quaternion (0, 0, 0, 1);
		Utils.SetLayer (_obj.obj, UiManager.Instance.gameCamera.gameObject.layer);
		return _obj.obj.transform.GetChild (0).GetComponent<FuBenCardCtrl> ();
	}
    /**创建天使模型 */
    private FuBenCardCtrl createAngelModel(Transform parent, AngelSample angel)
    {
       //"angel/"+angel.modelID
        passObj angelObj = Create3Dobj("angel/" + angel.modelID);
        angelObj.obj.transform.parent = parent;
        angelObj.obj.transform.localScale = new Vector3(1, 1, 1);
        angelObj.obj.transform.localPosition = Vector3.zero;
        angelObj.obj.transform.localRotation = new Quaternion(0, 0, 0, 1);
        Utils.SetLayer(angelObj.obj, UiManager.Instance.gameCamera.gameObject.layer);
        return angelObj.obj.transform.GetChild(0).GetComponent<FuBenCardCtrl>();
    }
    /** 创建坐骑模型 */
    private void createangelModel(Transform parent, AngelSample angel, CallBack<GameObject> callback) {
        ResourcesManager.Instance.LoadAssetBundleTexture("angel/" + angel.modelID, parent, (obj) => {
            GameObject gameObj = obj as GameObject;
            Transform temp = gameObj.transform;
            temp.localPosition = Vector3.zero;
            temp.localRotation = new Quaternion(0, 0, 0, 1);
            temp.localScale = new Vector3(1, 1, 1);
            if (callback != null) {
                callback(gameObj);
            }
        });
    }
}