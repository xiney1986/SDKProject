using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 英雄之章通关奖励容器
/// </summary>
public class TowerAwardContent : MonoBehaviour 
{

	/** 章 */
	public UISprite pass;
	/** 第N章 */
	public UILabel chapterLabel;
	/**  确定按钮 */
	public UIButton closeButton;
    /** 除宝箱外的奖励 */
    List<GameObject> AwardItems;
    List<int> awardNum;
    List<string> awardFlag;
    public GameObject AwardItemsContent;//放奖励的容器
    public UISprite[] numSprite;
    public UILabel[] numLabel;
    public GameObject goodsViewPrefab;
    public UILabel[] xiaoguoLabel;//解锁效果Label
    public UILabel awardType;//奖励类型（首通，普通）
	/** 动画步骤帧 */
	int setp;
	int nextSetp;
    int expGap;
    int starGap;
    int moneyGap;
    int honorGap;
    int rmbGap;
    Award aw;//合并以后的奖励
    private FubenAwardWindow wind;


    public void initTowerAward(Award firstAward,Award endAward,FubenAwardWindow win) {
        wind = win;
        aw = Award.mergeAward(firstAward, endAward);
        AwardItems = new List<GameObject>();
        if (aw != null) {
            moneyGap = aw.moneyGap > 0 ? aw.moneyGap : 0;
            expGap = aw.expGap > 0 ? aw.expGap : 0;
            honorGap = aw.honorGap > 0 ? aw.honorGap : 0;
            rmbGap = aw.rmbGap > 0 ? aw.rmbGap : 0;
            if (moneyGap + expGap + honorGap + rmbGap > 0) {
                awardNum = new List<int>();
                awardFlag=new List<string>();
                if (moneyGap > 0) {
                    awardNum.Add(moneyGap);
                    awardFlag.Add("icon_money");
                }
                if (expGap > 0) {
                    awardNum.Add(expGap);
                    awardFlag.Add("exp");
                }
                if (honorGap > 0) {
                    awardNum.Add(honorGap);
                    awardFlag.Add("Honor");
                }
                if (rmbGap > 0) {
                    awardNum.Add(rmbGap);
                    awardFlag.Add("rmb");
                }
 
            }
            if(awardNum!=null&&awardNum.Count>0){
                for (int m = 0; m < awardNum.Count;m++ ) {
                    numSprite[m].gameObject.SetActive(true);
                    numSprite[m].spriteName = awardFlag[m];
                    numLabel[m].text = awardNum[m].ToString();
                }
            }
            starGap = aw.starGap;
            CreateGoodsByAward(AwardItems, aw);
        }
        for (int i = 0; i < AwardItems.Count; i++) {
            GameObject obj = AwardItems[i];
            obj.transform.parent = AwardItemsContent.transform;
            obj.transform.localPosition = new Vector3(i * 94, 0, 0);
            obj.transform.localScale = new Vector3(0.7f, 0.7f, 1);
        }
        int missionSid = MissionInfoManager.Instance.mission.sid;
        int numm =0;
        if (missionSid< 151010) {
            numm = StringKit.toInt(MissionSampleManager.Instance.getMissionSampleBySid(missionSid).name.Substring(2, 1));
        } else {
            numm = StringKit.toInt(MissionSampleManager.Instance.getMissionSampleBySid(missionSid).name.Substring(2, 2));
        }
        if (FuBenManagerment.Instance.isPassThisChapter(missionSid)) {
            awardType.text = LanguageConfigManager.Instance.getLanguage("towerShowWindow17");
        } else {
            awardType.text = LanguageConfigManager.Instance.getLanguage("towerShowWindow51");
        }
        chapterLabel.text = LanguageConfigManager.Instance.getLanguage("towerShowWindow15", numm + "");
        string[] dec=CommandConfigManager.Instance.getTowerPassDec();
        string[] spDec = dec[numm - 1].Split('#');
        if (spDec.Length==1)
        {
            xiaoguoLabel[0].transform.localPosition=new Vector3(0f,-65f,0f);
        }else if (spDec.Length==2)
        {
            xiaoguoLabel[0].transform.localPosition = new Vector3(0f, -55f, 0f);
            xiaoguoLabel[1].transform.localPosition = new Vector3(0f, -88f, 0f);
        }
        for (int i = 0; i < spDec.Length; i++)
        {
            xiaoguoLabel[i].text = spDec[i];
        }
        NextSetp();
    }

	/** 动画是否播放结束 */
	public bool isSetpOver(){
		if (setp != 0 && setp == nextSetp)
			return true;
		return false;
	}

	public void heroRoadAnimation ()
	{
		if (setp == nextSetp)
			return;
		if (setp == 0) {
            NextSetp();
        } else if (setp == 1) {
            chapterLabel.gameObject.SetActive(true);
            TweenScale ts = TweenScale.Begin(chapterLabel.gameObject, 0.15f, chapterLabel.transform.localScale);
            ts.from = Vector3.zero;
            EventDelegate.Add(ts.onFinished, () => {
                StartCoroutine(Utils.DelayRun(() => {
                    pass.gameObject.SetActive(true);
                    TweenScale ts3 = TweenScale.Begin(pass.gameObject, 0.15f, Vector3.one);
                    ts3.method = UITweener.Method.EaseIn;
                    ts3.from = new Vector3(5, 5, 1);
                    EventDelegate.Add(ts3.onFinished, () => {
                        iTween.ShakePosition(pass.gameObject, iTween.Hash("amount", new Vector3(0.03f, 0.03f, 0.03f), "time", 0.4f));
                        iTween.ShakePosition(pass.gameObject, iTween.Hash("amount", new Vector3(0.01f, 0.01f, 0.01f), "time", 0.4f));
                        StartCoroutine(Utils.DelayRun(() => {
                            NextSetp();
                        }, 0.1f));
                    }, true);
                }, 0.2f));
            }, true);
        } else if (setp == 2) {
            closeButton.gameObject.SetActive(true);
            MaskWindow.UnlockUI();
        }
		setp++;
	}

	public bool isShowRoadAwake()
	{
		int[] array = HeroRoadManagerment.Instance.currentHeroRoad.getAwakeInfo ();
		int count = HeroRoadManagerment.Instance.currentHeroRoad.conquestCount;
		return array [count - 1] == 1;
	}

	public void NextSetp ()
	{
		nextSetp++;
	}
    private void CreateGoodsByAward(List<GameObject> awards, Award aw) {
        GameObject obj;
        int nameIndex = 0;
        if (aw.props != null && aw.props.Count > 0) {
            Dictionary<int, int> map = new Dictionary<int, int>();
            foreach (PropAward o in aw.props) {
                if (map.ContainsKey(o.sid))
                    map[o.sid] += o.num;
                else
                    map.Add(o.sid, o.num);
            }
            foreach (int key in map.Keys) {
                obj = CreateGoodsItem(key, map[key], 0);
                nameIndex++;
                obj.name = "goodsbutton_" + nameIndex;
                awards.Add(obj);
            }
        }
        if (aw.equips != null && aw.equips.Count > 0) {
            Dictionary<int, int> map = new Dictionary<int, int>();
            foreach (EquipAward o in aw.equips) {
                if (map.ContainsKey(o.sid))
                    map[o.sid] += 1;
                else
                    map.Add(o.sid, 1);
            }
            foreach (int key in map.Keys) {
                obj = CreateGoodsItem(key, map[key], 1);
                nameIndex++;
                obj.name = "goodsbutton_" + nameIndex;
                awards.Add(obj);
            }
        }
        if (aw.magicWeapons != null && aw.magicWeapons.Count > 0) {
            Dictionary<int, int> map = new Dictionary<int, int>();
            foreach (MagicwWeaponAward o in aw.magicWeapons) {
                if (map.ContainsKey(o.sid))
                    map[o.sid] += 1;
                else
                    map.Add(o.sid, 1);
            }
            foreach (int key in map.Keys) {
                obj = CreateGoodsItem(key, map[key], 3);
                nameIndex++;
                obj.name = "goodsbutton_" + nameIndex;
                awards.Add(obj);
            }
        }
        if (aw.cards != null && aw.cards.Count > 0) {
            Dictionary<int, int> map = new Dictionary<int, int>();
            foreach (CardAward o in aw.cards) {
                if (map.ContainsKey(o.sid))
                    map[o.sid] += 1;
                else
                    map.Add(o.sid, 1);
            }
            foreach (int key in map.Keys) {
                obj = CreateGoodsItem(key, map[key], 2);
                nameIndex++;
                obj.name = "goodsbutton_" + nameIndex;
                awards.Add(obj);
            }
        }
        if (aw.starsouls != null && aw.starsouls.Count > 0) {
            Dictionary<int, int> map = new Dictionary<int, int>();
            foreach (StarSoulAward o in aw.starsouls) {
                if (map.ContainsKey(o.sid))
                    map[o.sid] += 1;
                else
                    map.Add(o.sid, 1);
            }
            foreach (int key in map.Keys) {
                obj = CreateStarSoulGoodsItem(key, map[key]);
                nameIndex++;
                obj.name = "goodsbutton_" + nameIndex;
                awards.Add(obj);
            }
        }
    }
    private GameObject CreateGoodsItem(int sid, int count, int type) {
        GameObject obj = Instantiate(goodsViewPrefab) as GameObject;
        obj.transform.localScale = new Vector3(0.7f, 0.7f, 1);
        GoodsView view = obj.transform.GetComponent<GoodsView>();
        view.linkQualityEffectPoint();
        view.fatherWindow = wind;
        if (type == 0) {
            Prop p = PropManagerment.Instance.createProp(sid, count);
            view.init(p);
        } else if (type == 1) {
            Equip e = EquipManagerment.Instance.createEquip(sid);
            view.init(e);
            view.onClickCallback = () => {
                UiManager.Instance.openWindow<EquipAttrWindow>((winEquip) => {
                    winEquip.Initialize(e, EquipAttrWindow.OTHER, null);
                });
            };
        } else if (type == 2) {
            Card c = CardManagerment.Instance.createCard(sid);
            view.init(c);
            view.onClickCallback = () => {
                CardBookWindow.Show(c, CardBookWindow.SHOW, null);
            };
        } else if (type == 3) {
            MagicWeapon mc = MagicWeaponManagerment.Instance.createMagicWeapon(sid);
            view.init(mc);
            view.onClickCallback = () => {
                UiManager.Instance.openWindow<MagicWeaponStrengWindow>((win) => {
                    win.init(mc, MagicWeaponType.FORM_OTHER);
                });
            };
        }
        return obj;
    }
    //创建星魂奖励
    private GameObject CreateStarSoulGoodsItem(int sid, int num) {
        GameObject obj = Instantiate(goodsViewPrefab) as GameObject;
        obj.transform.localScale = new Vector3(0.7f, 0.7f, 1);
        GoodsView view = obj.transform.GetComponent<GoodsView>();
        StarSoul starSoul = StarSoulManager.Instance.createStarSoul(sid);
        view.init(starSoul);
        view.fatherWindow = wind;
        return obj;
    }
	
}
