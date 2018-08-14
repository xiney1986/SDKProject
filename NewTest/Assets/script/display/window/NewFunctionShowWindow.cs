using UnityEngine;
using System.Collections;

/**
 * 新功能开放
 * @authro 陈世惟  
 * */
public class NewFunctionShowWindow : WindowBase {

	public UILabel offLabel;
	public UISprite funIconSprite;
	public UISprite pveFunIconSprite;
	public UITexture funIconTexture;
	public UITexture zxTexture;
	public UITexture teamTexture;
	public UILabel funLabel;
    public GameObject magicWeaponGameObj;
    public GameObject towerGameObj;

	CallBack callback;
	float time = 0f;

	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}

	public void initWindow(CallBack call)
	{
		callback=call;

		switch(GuideManager.Instance.guideSid) {

		case GuideGlobal.NEWFUNSHOW01:
			iconTypeChange(1,"button_goddessAstrolabe");//女神星盘
			funLabel.text = LanguageConfigManager.Instance.getLanguage("Guide_02");
			break;

		case GuideGlobal.NEWFUNSHOW02:
			iconTypeChange(2,ResourcesManager.ICONIMAGEPATH + 43);//新手礼包
			funLabel.text = LanguageConfigManager.Instance.getLanguage("Guide_03");
			break;

		case GuideGlobal.NEWFUNSHOW03:
			iconTypeChange(4,"Practice");//修炼副本
			funLabel.text = LanguageConfigManager.Instance.getLanguage("Guide_04");
			break;

		case GuideGlobal.NEWFUNSHOW05:
			iconTypeChange(3,"4");//4人阵型
			funLabel.text = LanguageConfigManager.Instance.getLanguage("Guide_05");
			break;

		case GuideGlobal.NEWFUNSHOW06:
			iconTypeChange(1,"button_exchange");//兑换
			funLabel.text = LanguageConfigManager.Instance.getLanguage("Guide_06");
			break;

		case GuideGlobal.NEWFUNSHOW07:
			iconTypeChange(1,"button_mainEvo");//主角培养
			funLabel.text = LanguageConfigManager.Instance.getLanguage("Guide_07");
			break;

		case GuideGlobal.NEWFUNSHOW08:
			iconTypeChange(1,"button_heroRoad");//英雄之章
			funLabel.text = LanguageConfigManager.Instance.getLanguage("Guide_08");
			break;

		case GuideGlobal.NEWFUNSHOW10:
			iconTypeChange(1,"button_honor");//爵位
			funLabel.text = LanguageConfigManager.Instance.getLanguage("Guide_09");
			break;

		case GuideGlobal.NEWFUNSHOW11:
			iconTypeChange(1,"chat1");//聊天
			funLabel.text = LanguageConfigManager.Instance.getLanguage("Guide_10");
			break;

		case GuideGlobal.NEWFUNSHOW12:
			iconTypeChange(1,"icon_addon");//附加属性
			funLabel.text = LanguageConfigManager.Instance.getLanguage("Guide_11");
			break;

		case GuideGlobal.NEWFUNSHOW13:
			iconTypeChange(1,"button_friend");//好友
			funLabel.text = LanguageConfigManager.Instance.getLanguage("Guide_12");
			break;

		case GuideGlobal.NEWFUNSHOW14:
			iconTypeChange(1,"button_reslove");//分解
			funLabel.text = LanguageConfigManager.Instance.getLanguage("Guide_13");
			break;

		case GuideGlobal.NEWFUNSHOW15:
			iconTypeChange(1,"button_picture");//图鉴
			funLabel.text = LanguageConfigManager.Instance.getLanguage("Guide_14");
			break;

		case GuideGlobal.NEWFUNSHOW16:
			iconTypeChange(1,"button_task");//任务
			funLabel.text = LanguageConfigManager.Instance.getLanguage("Guide_15");
			break;

		case GuideGlobal.NEWFUNSHOW17:
			iconTypeChange(3,"5");//5人阵型
			funLabel.text = LanguageConfigManager.Instance.getLanguage("Guide_16");
			break;

		case GuideGlobal.NEWFUNSHOW18:
			iconTypeChange(1,"button_guild");//公会
			funLabel.text = LanguageConfigManager.Instance.getLanguage("Guide_17");
			break;

		case GuideGlobal.NEWFUNSHOW19:
			iconTypeChange(1,"button_rank");//排行榜
			funLabel.text = LanguageConfigManager.Instance.getLanguage("Guide_18");
			break;

		case GuideGlobal.NEWFUNSHOW20:
			iconTypeChange(4,"Activities");//限时活动
			funLabel.text = LanguageConfigManager.Instance.getLanguage("Guide_19");
			break;

		case GuideGlobal.NEWFUNSHOW21:
			iconTypeChange(1,"button_pvp");//竞技场
			funLabel.text = LanguageConfigManager.Instance.getLanguage("Guide_20");
			break;

		case GuideGlobal.NEWFUNSHOW22:
			iconTypeChange(1,"Contribution");//学技能
			funLabel.text = LanguageConfigManager.Instance.getLanguage("Guide_21");
			break;

		case GuideGlobal.NEWFUNSHOW23:
			iconTypeChange(1,"icon_intensify");//穿装备
			funLabel.text = LanguageConfigManager.Instance.getLanguage("Guide_22");
			break;

		case GuideGlobal.NEWFUNSHOW25:
			iconTypeChange(1,"button_notice");//公告
			funLabel.text = LanguageConfigManager.Instance.getLanguage("Guide_23");
			break;

		case GuideGlobal.NEWFUNSHOW26:
			iconTypeChange(4,"Crusade");//讨伐
			funLabel.text = LanguageConfigManager.Instance.getLanguage("Guide_24");
			break;

		case GuideGlobal.NEWFUNSHOW27:
			iconTypeChange(1,"fanfanl");//女神摇一摇
			funLabel.text = LanguageConfigManager.Instance.getLanguage("Guide_27");
			break;

		case GuideGlobal.NEWFUNSHOW28:
			iconTypeChange(3,"");//替补
			funLabel.text = LanguageConfigManager.Instance.getLanguage("Guide_25");
			break;

		case GuideGlobal.NEWFUNSHOW29:
			iconTypeChange(1,"icon_evo");//卡片进化
			funLabel.text = LanguageConfigManager.Instance.getLanguage("Guide_26");
			break;

		case GuideGlobal.NEWFUNSHOW30:
			iconTypeChange(1,"button_starSoul");//星魂
			funLabel.text = LanguageConfigManager.Instance.getLanguage("Guide_28");
			break;

		case GuideGlobal.NEWFUNSHOW31:
			iconTypeChange(1,"button_training");//挂机
			funLabel.text = LanguageConfigManager.Instance.getLanguage("Guide_29");
			break;

		case GuideGlobal.NEWFUNSHOW32:
			iconTypeChange(1,"button_shop");//神秘商店
			funLabel.text = LanguageConfigManager.Instance.getLanguage("Guide_30");
			break;

		case GuideGlobal.NEWFUNSHOW33:
			iconTypeChange(1,"button_pvp");//天梯
			funLabel.text = LanguageConfigManager.Instance.getLanguage("Guide_31");
			break;

		case GuideGlobal.NEWFUNSHOW34:
			iconTypeChange(1,"button_mounts");//坐骑
			funLabel.text = LanguageConfigManager.Instance.getLanguage("Guide_32");
			break;

		case GuideGlobal.NEWFUNSHOW35:
			iconTypeChange(1,"button_heroStore");//卡片碎片兑换
			funLabel.text = LanguageConfigManager.Instance.getLanguage("Guide_33");
			break;
        case GuideGlobal.NEWFUNSHOW40://神器开放
            iconTypeChange(5, "");
            funLabel.text = LanguageConfigManager.Instance.getLanguage("Guide_40");
            break;    
		default:
			funIconSprite.gameObject.SetActive (false);
			funIconTexture.gameObject.SetActive (false);
			break;
		}
	}

	private void iconTypeChange(int _type,string _spriteName)
	{
		switch(_type) {
		case 1:
            magicWeaponGameObj.SetActive(false);
            towerGameObj.SetActive(false);
			funIconSprite.gameObject.SetActive (true);
			funIconTexture.gameObject.SetActive (false);
			zxTexture.gameObject.SetActive (false);
			teamTexture.gameObject.SetActive (false);
			pveFunIconSprite.gameObject.SetActive (false);
			funIconSprite.spriteName = _spriteName;
			funIconSprite.MakePixelPerfect ();
			showEffect(funIconSprite.gameObject);

			break;
		case 2:
            magicWeaponGameObj.SetActive(false);
            towerGameObj.SetActive(false);
			funIconSprite.gameObject.SetActive (false);
			funIconTexture.gameObject.SetActive (true);
			zxTexture.gameObject.SetActive (false);
			teamTexture.gameObject.SetActive (false);
			pveFunIconSprite.gameObject.SetActive (false);
			ResourcesManager.Instance.LoadAssetBundleTexture (_spriteName, funIconTexture);
			showEffect(funIconTexture.gameObject);
			break;

		case 3:
            magicWeaponGameObj.SetActive(false);
            towerGameObj.SetActive(false);
			funIconSprite.gameObject.SetActive (false);
			funIconTexture.gameObject.SetActive (false);
			zxTexture.gameObject.SetActive (true);
			teamTexture.gameObject.SetActive (true);
			pveFunIconSprite.gameObject.SetActive (false);
			if (_spriteName == "4") {
				ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.TEXTURE_TEAM_FORMATION_PATH + 2, teamTexture);
			} else {
				ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.TEXTURE_TEAM_FORMATION_PATH + 3, teamTexture);
			}

			break;

		case 4:
            magicWeaponGameObj.SetActive(false);
            towerGameObj.SetActive(false);
			pveFunIconSprite.gameObject.SetActive (true);
			funIconSprite.gameObject.SetActive (false);
			funIconTexture.gameObject.SetActive (false);
			zxTexture.gameObject.SetActive (false);
			teamTexture.gameObject.SetActive (false);
			pveFunIconSprite.spriteName = _spriteName;
			pveFunIconSprite.MakePixelPerfect ();
			showEffect(funIconSprite.gameObject);
			break;
        case 5:
            magicWeaponGameObj.SetActive(true);
            towerGameObj.SetActive(true);
            pveFunIconSprite.gameObject.SetActive(false);
            funIconSprite.gameObject.SetActive(false);
            funIconTexture.gameObject.SetActive(false);
            zxTexture.gameObject.SetActive(false);
            teamTexture.gameObject.SetActive(false);
            pveFunIconSprite.spriteName = _spriteName;
            pveFunIconSprite.MakePixelPerfect();
            showEffectt(magicWeaponGameObj,74f,31f);
            showEffectt(towerGameObj,-71f,45f);
            break;
		}
	}

	private void showEffect(GameObject a)
	{
		a.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
		a.transform.localPosition = new Vector3(100000,100000,0);
		StartCoroutine (Utils.DelayRun (() => {
			a.transform.localPosition = Vector3.zero;
			TweenScale ts = TweenScale.Begin(a,0.5f,Vector3.one);
			ts.method = UITweener.Method.EaseOut;
			EventDelegate.Add(ts.onFinished, ()=> {
				iTween.ShakePosition (a, iTween.Hash ("amount", new Vector3 (0.03f, 0.03f, 0.03f), "time", 0.4f));
				iTween.ShakePosition (a, iTween.Hash ("amount", new Vector3 (0.01f, 0.01f, 0.01f), "time", 0.4f));
			},true);
		}, 0.5f));
	}
    private void showEffectt(GameObject a,float x,float y) {
        a.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        a.transform.localPosition = new Vector3(100000, 100000, 0);
        StartCoroutine(Utils.DelayRun(() => {
            a.transform.localPosition = new Vector3(x,y,0f);
            TweenScale ts = TweenScale.Begin(a, 0.5f, Vector3.one);
            ts.method = UITweener.Method.EaseOut;
            EventDelegate.Add(ts.onFinished, () => {
                iTween.ShakePosition(a, iTween.Hash("amount", new Vector3(0.03f, 0.03f, 0.03f), "time", 0.4f));
                iTween.ShakePosition(a, iTween.Hash("amount", new Vector3(0.01f, 0.01f, 0.01f), "time", 0.4f));
            }, true);
        }, 0.5f));
    }

	protected override void DoEnable ()
	{
		base.DoEnable ();
	}

	public override void DoDisable ()
	{
		base.DoDisable (); 
		GuideManager.Instance.isOpenNewFunWin = false;
		if (callback != null) {
			callback ();
			callback = null;
		} 
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "screenButton") {
			finishWindow ();
		}
	}

	void Update ()
	{
		float offset = Mathf.Sin (time * 6); 
		offLabel.alpha =sin();
	}


}
