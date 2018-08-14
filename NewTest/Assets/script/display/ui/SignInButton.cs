using UnityEngine;
using System.Collections;

public class SignInButton : ButtonBase
{
	/** 底部文本显示类型 */
	public const int BOTTOM_TEXT_NONE=-1, //不显示
						BOTTOM_TEXT_NAME=0, // 显示名
						BOTTOM_TEXT_NAME_LV=1, //名+等级
						BOTTOM_TEXT_NUM=2; //数量
    public const int BIG_PRIZE = 1;//大奖
    public const int COMMON_PRIZE = 0;

	/** 自定义Icon图标点 */
	public GameObject customIconPoint;
	/** 图标 */
	public UITexture icon;
	/** bg */
	public UISprite backGround;
	/** 碎片标签 */
	public UISprite spriteScrap;
	/** 已签到图标 */
	public UISprite signedInFlag;
    /** 补签图标 */
    public UISprite needSignFlag;
	/** 右下角文本 */
	public UILabel rightBottomText;
    /** 左上角文本 */
    public UILabel daysText;
	/** 点击回调 */
	public CallBack onClickCallback;
	/** 品质环绕特效路径 */
	string qualityEffectPath;
	/** 品质环绕特效名 */
	public string[]  qualityEffectPaths;
	/** 品质环绕特效点 */
	private GameObject  qualityEffectPoint;
	/** 星魂 */
	public StarSoul starSoul;
	/** 装备 */
	public Equip equip;
    /**秘宝 */
    public MagicWeapon magicWeapon;
	/** 卡片 */
	public Card card;
	/** 道具 */
	public Prop prop;
	/** 奖品模板 */
	public PrizeSample prize;
	/** 数量 */
	int count;
	/** 品质 */
	int quality = 5;
	/** 底部文本显示类型 */
    int signState;
    int dayOfMonth;
    int sid;
    int type;//用于背景的显示
    //是否开启新的英雄之章
    bool isActiveHeroRoad = false;
    NoticeActivitySignInContent signContent;
	/**当数量为0时 是否还显示*/
	public bool showZeroNum=false;
	/** 当前展示的道具名字 */
    public GameObject stars;//卡片星星

	private Color color5 = new Color(1f,184f/255f,61f/255f,1f);
	private Color color4 = new Color(207f/255f,113f/255f,1f,1f);
	private Color color3 = new Color(95f/255f,193f/255f,95f/255f,1f);
	private Color color2 = new Color(80f/255f,169f/255f,216f/255f,1f);
	private Color color1 = new Color(1f,1f,1f,1f);

	public override void OnAwake () {
		qualityEffectPath="Effect/UiEffect/Surroundeffect";
		qualityEffectPaths=new string[3];
		qualityEffectPaths[0]="Surroundeffect_b";
		qualityEffectPaths[1]="Surroundeffect_p";
		qualityEffectPaths[2]="Surroundeffect_y";

		if(showZeroNum)
		{
			rightBottomText.gameObject.SetActive(true);
		}
	}

	
	public int getCount ()
	{
		return count;
	}
	
	public void setCount (int _count)
	{
		count = _count;
	}
	
	public void addCount (int add)
	{
		count += add;
	}
	/** 0=普通道具,1=卡片,2=装备 */
	public int getStortType ()
	{
		if (prop != null)
			return 0;
		if (card != null)
			return 1;
		if (equip != null)
			return 2;
		if (starSoul != null)
			return 3;
		return 0;
	}
	
	public int getQuality ()
	{
		return quality;
	}

    public void init(NoticeActivitySignInContent content) {
        this.signContent = content;
    }
    public void init(PrizeSample prize,int state,int dayNum,int sid,int type) {
        this.count = StringKit.toInt(prize.num);
        this.signState = state;
        this.prize = prize;
        this.dayOfMonth = dayNum;
        this.sid = sid;
        this.type = type;
        if (prize.type == PrizeType.PRIZE_CARD) {
            card = CardManagerment.Instance.createCard(prize.pSid);
        } else if (prize.type == PrizeType.PRIZE_EQUIPMENT) {
            equip = EquipManagerment.Instance.createEquip(prize.pSid);
        } else if (prize.type == PrizeType.PRIZE_MAGIC_WEAPON) {
            magicWeapon = MagicWeaponManagerment.Instance.createMagicWeapon(prize.pSid);
        } else if (prize.type == PrizeType.PRIZE_PROP) {
            prop = PropManagerment.Instance.createProp(prize.pSid);
        } else if (prize.type == PrizeType.PRIZE_STARSOUL) {
            starSoul = StarSoulManager.Instance.createStarSoul(prize.pSid);
        }
        UpdateInfo();
    }

	/// <summary>
	/// 初始化时重置
	/// </summary>
	public void initClear ()
	{
		initItem ();
		initObj ();
	}

	private void initItem ()
	{
		equip = null;
		prop = null;
		prize = null;
		card = null;
        magicWeapon = null;
		starSoul = null;
	}

	private void initObj ()
	{
		customIconPoint.SetActive(false);
		backGround.spriteName = "";
		if (spriteScrap != null)
			spriteScrap.spriteName = "";
		rightBottomText.text = "";
	}
	
	public void clear ()
	{
		initItem ();
		onClickCallback = null;
	}

	/** 清理方法 */
	public void clean () {
		initObj ();
		clear ();
	}

	/** 连接环绕特效点 */
	public void linkQualityEffectPointByRotate ()
	{
		qualityEffectPoint = transform.FindChild ("effectPoint").gameObject;
		qualityEffectPoint.SetActive (true);
	}

	public void reset() {
		if(customIconPoint!=null)
			customIconPoint.gameObject.SetActive (false);
	}

    /// <summary>
    /// 卡片星级星星显示
    /// </summary>
    public void showStar(Card showCard) {
        if (stars != null) {
            for (int i = 0; i < stars.transform.childCount; i++) {
                stars.transform.GetChild(i).gameObject.SetActive(false);
            }
            stars.transform.localPosition = new Vector3(0, 0, 0);
            int cardStarLevel = CardSampleManager.Instance.getStarLevel(showCard.sid);
            for (int i = 0; i < cardStarLevel; i++) {
                stars.transform.GetChild(i).gameObject.SetActive(true);
            }
            if (cardStarLevel == CardSampleManager.ONESTAR) {
                stars.transform.localPosition = new Vector3(33, -31, 0);
            } else if (cardStarLevel == CardSampleManager.TWOSTAR) {
                stars.transform.localPosition = new Vector3(22, -31, 0);
            } else if (cardStarLevel == CardSampleManager.THREESTAR) {
                stars.transform.localPosition = new Vector3(11, -31, 0);
            } else if (cardStarLevel == CardSampleManager.FOURSTAR) {
                stars.transform.localPosition = new Vector3(0, -31, 0);
            }
        }
    }
    /// <summary>
    /// 签到状态表现更新
    /// </summary>
    public void updateSignState() {
        switch (signState) {
            case NoticeActivitySignInContent.SIGN_IN_NO://没签过
                signedInFlag.gameObject.SetActive(false);
                if (ServerTimeKit.getDayOfMonth() > dayOfMonth) {//当天之前
                    needSignFlag.gameObject.SetActive(true);
                } else if (ServerTimeKit.getDayOfMonth() == dayOfMonth) {//正是当天
                    needSignFlag.gameObject.SetActive(false);
                    linkQualityEffectPointByRotate();
                    showEffectByQuality();//显示环绕特效
                }
                break;
            case NoticeActivitySignInContent.SIGN_IN_YES://签过
                    signedInFlag.gameObject.SetActive(true);
                    needSignFlag.gameObject.SetActive(false);
                break;
        }
    }
    /// <summary>
    /// 更新所有的信息
    /// </summary>
	private void UpdateInfo ()
	{
        daysText.text = dayOfMonth + "";
        if (stars != null) {//把星星全部隐藏
            for (int i = 0; i < stars.transform.childCount; i++) {
                stars.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        //签到状态更新
        updateSignState();
		reset ();
        quality = prize.getQuality();
        if (type == BIG_PRIZE) {
            backGround.spriteName = "big_prize";
        } else if (type == COMMON_PRIZE) {
            backGround.spriteName = "common_prize";
        }
		if (equip != null) {
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + equip.getIconId (), icon);
			if (spriteScrap != null)
				spriteScrap.spriteName = "sign_scrap";
			//backGround.spriteName = QualityManagerment.qualityIDToIconSpriteName (quality);
			setCountActive (count > 0);
			if (count / 1000000 > 0)
				rightBottomText.text = "x" + count / 10000 + "W";
			else
				rightBottomText.text = "x" + count;
		} else if(starSoul != null) {
			UpdateStarSoulView(starSoul);
		}else if(magicWeapon!=null){
            UpdateMagicWeapon();
        }else if (prop != null) {
			if (spriteScrap != null) {
				if (prop.isScrap()) {
					spriteScrap.gameObject.SetActive (true);
					spriteScrap.spriteName = "sign_scrap";
				} else {
					spriteScrap.gameObject.SetActive (false);
				}
			}
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + prop.getIconId (), icon);
			//backGround.spriteName = QualityManagerment.qualityIDToIconSpriteName (quality);
			setCountActive (count > 0);
			if (count / 1000000 > 0)
				rightBottomText.text = "x" + count / 10000 + "W";
			else
				rightBottomText.text = "x" + count;
        } else if (card != null) {
            if (card.sid <= 10)
                ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.MAINCARD_ICONIMAGEPATH + card.getMainCardImageIDBysid(card.sid).ToString(), icon);
            else
                ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.ICONIMAGEPATH + card.getIconID().ToString(), icon);
            if (spriteScrap != null)
                spriteScrap.spriteName = "sign_scrap";
            //backGround.spriteName = QualityManagerment.qualityIDToIconSpriteName(quality);
            if (count == 0)
                count = 1;
            setCountActive(count > 0);
            rightBottomText.text = "x" + count;
            if(stars != null)
            showStar(card);
        } else if (prize != null) {
			ResourcesManager.Instance.LoadAssetBundleTexture (prize.getIconPath (), icon);
			if (spriteScrap != null)
				spriteScrap.spriteName = "sign_scrap";
            if (prize.type == PrizeType.PRIZE_MONEY) spriteScrap.gameObject.SetActive(false);
			//backGround.spriteName = QualityManagerment.qualityIDToIconSpriteName (quality);
			setCountActive (count > 0);
			if (count / 1000000 > 0)
				rightBottomText.text = "x" + count / 10000 + "W";
			else
				rightBottomText.text = "x" + count;
		}
	}
    /**更新秘宝的基本信息 */
    private void UpdateMagicWeapon(){
        ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.ICONIMAGEPATH + magicWeapon.getIconId(), icon);
        quality = magicWeapon.getMagicWeaponQuality();
        //backGround.spriteName = QualityManagerment.qualityIDtoMagicWeapon(magicWeapon.getMagicWeaponQuality());
        icon.SetRect(0f, 0f, 105, 105f);
        icon.transform.localPosition = new Vector3(0f,6f,1f);
        setCountActive(count > 0);
        if (count / 1000000 > 0)
            rightBottomText.text = "x" + count / 10000 + "W";
        else
            rightBottomText.text = "x" + count;
    }

	/**  更新星魂视图 */
	private void UpdateStarSoulView(StarSoul starsoulView) {
		if (customIconPoint == null)
			return;
		//backGround.spriteName = "iconback_3";
		icon.gameObject.SetActive (false);
		customIconPoint.SetActive (true);
		if (customIconPoint.transform.childCount > 0)
			Utils.RemoveAllChild (customIconPoint.transform);
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.STARSOUL_ICONPREFAB_PATH+starsoulView.getIconId (),customIconPoint.transform,(obj)=>{
			GameObject gameObj=obj as GameObject;
			if(gameObj!=null) {
				Transform childTrans=gameObj.transform;
				if(childTrans!=null){
					StarSoulEffectCtrl effectCtrl=childTrans.gameObject.GetComponent<StarSoulEffectCtrl>();
					effectCtrl.setColor(starsoulView.getQualityId ());
				}
			}
		});
        rightBottomText.text = "x" + count;
	}

	/** 根据自身品质显示对应的环绕特效 */
	void showEffectByQuality ()
	{
		if (qualityEffectPoint == null)
			return;
		if (quality < QualityType.EPIC)
			return;
		Utils.RemoveAllChild (qualityEffectPoint.transform);
		EffectCtrl effectCtrl = EffectManager.Instance.CreateEffect(qualityEffectPoint.transform,qualityEffectPath,qualityEffectPaths [quality - QualityType.GOOD]);
		effectCtrl.transform.localPosition = new Vector3(0,5,0);
		effectCtrl.transform.localScale = new Vector3(1.8f,1.8f,1);
	}
	public void setCountActive (bool isActive)
	{
		if(showZeroNum)
			return;
		rightBottomText.gameObject.SetActive (isActive);
	}
	
	public override void DoClickEvent ()
	{
		if (onClickCallback != null)
			onClickCallback ();
		if (onClickCallback == null)
			DefaultClickEvent ();
	}
	public void DefaultClickEvent ()
	{
        if (ServerTimeKit.getDayOfMonth() < dayOfMonth) {//还没到签到时间，可点击查看奖励信息
            if (equip != null) {
                UiManager.Instance.openWindow<EquipAttrWindow>(
                    (winEquip) => {
                        winEquip.Initialize(equip, EquipAttrWindow.OTHER, null);
                    });
            } else if (starSoul != null) {
                UiManager.Instance.openDialogWindow<StarSoulAttrWindow>(
                    (win) => {
                        win.Initialize(starSoul, StarSoulAttrWindow.AttrWindowType.None);
                    });
            } else if (prop != null) {
                UiManager.Instance.openDialogWindow<PropAttrWindow>(
                    (winProp) => {
                        winProp.Initialize(prop);
                    });
            } else if (card != null) {
                CardBookWindow.Show(card, CardBookWindow.SHOW, null);
            } else if (magicWeapon != null) {
                UiManager.Instance.openWindow<MagicWeaponStrengWindow>((win) => {
                    win.init(magicWeapon, MagicWeaponType.FORM_OTHER);
                });
            } else if (prize != null) {
                if ((prize.type == PrizeType.PRIZE_MONEY || prize.type == PrizeType.PRIZE_RMB)) {
                    MaskWindow.UnlockUI();
                } else {
                    UiManager.Instance.openDialogWindow<PropAttrWindow>((winProp) => {
                        winProp.Initialize(prize);
                    });
                }
            } else {
                MaskWindow.UnlockUI();
            }
        } else if (ServerTimeKit.getDayOfMonth() > dayOfMonth) {//过了当前签到日期的
            if (signState == NoticeActivitySignInContent.SIGN_IN_NO) {//没签过的
                //打开补签窗口
                UiManager.Instance.openDialogWindow<SignInWindow>((win) => {
                    win.Initialize(prize, (msg) => {
                        getPrize(msg); 
                    });
                });
            } else if (signState == NoticeActivitySignInContent.SIGN_IN_YES) {//签过的
                MaskWindow.UnlockUI();
            }
        } else if (ServerTimeKit.getDayOfMonth() == dayOfMonth) {
            if (signState == NoticeActivitySignInContent.SIGN_IN_YES) {
                MaskWindow.UnlockUI();
            } else {
                getPrize(null);
            }
        }
	}
    public void getPrize(MessageHandle msg) {
        // 检测是否有足够的空间容纳奖励
        if (isStorageFull(prize) || !StorageManagerment.Instance.isTempStorageFull(count)) {
            //和后台通讯拿奖励
            SignInFport fport = FPortManager.Instance.getFPort("SignInFport") as SignInFport;
            //补签 type= 2，正常签到 type = 1
            if (dayOfMonth != ServerTimeKit.getDayOfMonth()) {//补签 
                fport.signIn(sid, 2, showAwardInfo);
                return;
            }
            fport.signIn(sid, 1, showAwardInfo);
        } else {//飘字提示临时仓库空间不足
            UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                win.Initialize(LanguageConfigManager.Instance.getLanguage("signInTips2"));
            });
        }
    }
    /// <summary>
    /// 检测是否有足够的空间容纳奖励
    /// </summary>
    /// <param name="prize"></param>
    /// <returns></returns>
    private bool isStorageFull(PrizeSample prize) {
        switch (prize.type) { 
            case PrizeType.PRIZE_CARD:
                if (StorageManagerment.Instance.isRoleStorageFull(count)) return false;
                break;
            case PrizeType.PRIZE_EQUIPMENT:
                if (StorageManagerment.Instance.isEquipStorageFull(count)) return false; 
                break;
            case PrizeType.PRIZE_MAGIC_WEAPON:
                if (StorageManagerment.Instance.isMagicWeaponStorageFull(count)) return false;
                break;
            case PrizeType.PRIZE_PROP:
                if (StorageManagerment.Instance.isPropStorageFull(0)) return false;
                break;
            case PrizeType.PRIZE_STARSOUL:
                if (StorageManagerment.Instance.isStarSoulStorageFull(count)) return false;
                break;
        }
        return true;
    }
    //领取奖励回调
    public void showAwardInfo() {
        UiManager.Instance.createPrizeMessageLintWindow(prize);
        if (!isStorageFull(prize)) {
            UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                win.Initialize(LanguageConfigManager.Instance.getLanguage("signInTips1"));
            });
        }
        if(card != null){
            if (HeroRoadManagerment.Instance.activeHeroRoadIfNeed(card)) {
                StartCoroutine(Utils.DelayRun(() => {
                    UiManager.Instance.openDialogWindow<TextTipWindow>((win) => {
                        win.init(LanguageConfigManager.Instance.getLanguage("s0418"), 0.8f);
                    });
                },0.7f));
            }
        }
        //刷新界面
        needSignFlag.gameObject.SetActive(false);
        signedInFlag.gameObject.SetActive(true);
        signState = NoticeActivitySignInContent.SIGN_IN_YES;
        if (qualityEffectPoint != null) {
            if (qualityEffectPoint.transform.childCount > 0) {
                Utils.RemoveAllChild(qualityEffectPoint.transform);
            }
        }
        GetSignInInfoFport fport = FPortManager.Instance.getFPort("GetSignInInfoFport") as GetSignInInfoFport;
        fport.getSignInInfo(signContent.updateUI);
    }
}