using UnityEngine;
using System.Collections;

public class GoodsView : ButtonBase
{
	/** 底部文本显示类型 */
	public const int BOTTOM_TEXT_NONE=-1, //不显示
						BOTTOM_TEXT_NAME=0, // 显示名
						BOTTOM_TEXT_NAME_LV=1, //名+等级
						BOTTOM_TEXT_NUM=2; //数量

	/** 自定义Icon图标点 */
	public GameObject customIconPoint;
	/** 图标 */
	public UITexture icon;
	/** bg */
	public UISprite backGround;
	/** 碎片标签 */
	public UISprite spriteScrap;
	/** 右下角图标 */
	public UISprite rightBottomSprite;
	/** 右下角文本 */
	public UILabel rightBottomText;
	/** 点击回调 */
	public CallBack onClickCallback;
	/** 长按回调 */
	[HideInInspector] public CallBack longPassCallback;
	/** 品质环绕特效路径 */
	string qualityEffectPath;
	/** 品质环绕特效名 */
	public string[]  qualityEffectPaths;
	/** 品质烟花特效点 */
	public GameObject[]  fireworksEffctProfab;	
	//这4个字段GoodsView本身并未使用,可用于关联
	[HideInInspector] public GameObject tempGameObj;
	/** 品质环绕特效点 */
	private GameObject  qualityEffectPoint;
	/** 品质烟花特效点 */
	private GameObject  fireworksEffctPoint;
		/** 新获得图标 */
	private GameObject firstSprite;
	/** 星魂 */
	public StarSoul starSoul;
	/** 装备 */
	public Equip equip;
    /**秘宝 */
    public MagicWeapon magicWeapon;
	//** 装备升星数字*/
	public UILabel starLevelState;
	//** */
	public UILabel specialName;
	/** 卡片 */
	public Card card;
	/** 卡片模板 */
	public CardSample cardSample;
	/** 道具 */
	public Prop prop;
	/** 奖品模板 */
	public PrizeSample prize;
	/** 图标路径 */
	string path;
	/** 数量 */
	int count;
	/** 品质 */
	int quality = 5;
	/** 资源类型 */
	int resource_type;
	/** 底部文本显示类型 */
	public int iconType;
	/**当数量为0时 是否还显示*/
	public bool showZeroNum=false;
	/** 当前展示的道具名字 */
	[HideInInspector]
	public string showName = "";
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
	void Update ()
	{
		if (starLevelState != null && starLevelState.gameObject != null && starLevelState.gameObject.activeSelf)
			starLevelState.alpha = sin ();
	}
	/* methods */
	/// <summary>
	/// init
	/// </summary>
	/// <param name="starSoul">星魂--修改星魂数据前请保证不是仓库中的星魂</param>
	public void init (StarSoul starSoul) {
		init (starSoul,BOTTOM_TEXT_NONE);
	}
	/// <summary>
	/// init
	/// </summary>
	/// <param name="starSoul">星魂--修改星魂数据前请保证不是仓库中的星魂</param>
	/// <param name="iconType">底本文本类型(-1=不显示,0=显示星魂名,1=星魂名+等级,2=星魂数量)</param>
	public void init (StarSoul starSoul,int iconType) {
		initClear ();
		this.starSoul = starSoul;
		this.iconType = iconType;
		UpdateInfo ();
	}
	public void init (Equip equip)
	{
		init (equip, 1);
	}
    public void init (MagicWeapon magicWeapon){
        initClear();
        this.magicWeapon = magicWeapon;
        this.count = 1;
        quality = magicWeapon.getMagicWeaponQuality();
        if (starLevelState != null && this.magicWeapon.getPhaseLv() > 0) {
            starLevelState.gameObject.SetActive(true);
            starLevelState.text = "+" + this.magicWeapon.getPhaseLv().ToString();
        } else {
            starLevelState.gameObject.SetActive(false);
        }
        UpdateInfo();
    }
    public void init(MagicWeapon magicWeapon,int num) {
        initClear();
        this.magicWeapon = magicWeapon;
        this.count = num;
        quality = magicWeapon.getMagicWeaponQuality();
        if (starLevelState != null && this.magicWeapon.getPhaseLv() > 0) {
            starLevelState.gameObject.SetActive(true);
            starLevelState.text = "+" + this.magicWeapon.getPhaseLv().ToString();
        } else {
            starLevelState.gameObject.SetActive(false);
        }
        UpdateInfo();
    }

	public void init(Equip equip, bool _need){
		if (_need) {
			//specialName.gameObject.SetActive(true);
			if(equip.getQualityId() == 5)
				specialName.color = color5;
			if(equip.getQualityId() == 4)
				specialName.color = color4;
			if(equip.getQualityId() == 3)
				specialName.color = color3;
			if(equip.getQualityId() == 2)
				specialName.color = color2;
			//specialName.text = equip.getName();
		}
		specialName.gameObject.SetActive(true);
		specialName.text = equip.getName();
		init (equip, 1);
	}
	public void init (Equip equip, int count)
	{
		initClear ();
		this.equip = equip;
		this.count = count;
		quality = equip.getQualityId ();
		if (starLevelState != null && this.equip.equpStarState > 0) {
			starLevelState.gameObject.SetActive (true);
			starLevelState.text = "+" + this.equip.equpStarState;
		}
		else {
			starLevelState.gameObject.SetActive(false);
		}
		UpdateInfo ();
	}
	public void init (Prop prop, int num)
	{
		initClear ();
		this.prop = prop;
		this.count = num;
		quality = prop.getQualityId ();
		UpdateInfo ();
	}

	public void init(Prop prop, int num, bool _need){
		initClear ();
		this.prop = prop;
		this.count = num;
		quality = prop.getQualityId ();
		if (_need) {
			//specialName.gameObject.SetActive(true);
			if(prop.getQualityId() == 5)
				specialName.color = color5;
			if(prop.getQualityId() == 4)
				specialName.color = color4;
			if(prop.getQualityId() == 3)
				specialName.color = color3;
			if(prop.getQualityId() == 2)
				specialName.color = color2;
			//specialName.text = prop.getName();
		}
		specialName.gameObject.SetActive(true);
		specialName.text = prop.getName();
		UpdateInfo ();
	}
	
	public void init (Prop prop)
	{
		initClear ();
		this.prop = prop;
		this.count = prop.getNum ();
		quality = prop.getQualityId ();
		UpdateInfo ();
	}
	
	public void init (int resource_type, string path, int count)
	{
		initClear ();
		this.path = path;
		this.count = count;
		this.resource_type = resource_type;
		UpdateInfo ();
	}

	public void init (string path, int count)
	{
		initClear ();
		this.path = path;
		this.count = count;
		UpdateInfo ();
	}
	
	public void init (Card card)
	{
		init (card, 1);
	}

	public void init (Card card, bool _need)
	{
		if (_need) {
			//specialName.gameObject.SetActive(true);
			if(card.getQualityId() == 5)
				specialName.color = color5;
			if(card.getQualityId() == 4)
				specialName.color = color4;
			if(card.getQualityId() == 3)
				specialName.color = color3;
			if(card.getQualityId() == 2)
				specialName.color = color2;
			//specialName.text = card.getName();
		}
		specialName.gameObject.SetActive(true);
		specialName.text = card.getName();
		init (card, 1);
	}
	
	public void init (Card card, int count)
	{
		initClear ();
		this.card = card;
		quality = card.getQualityId ();
		this.count = count;
		UpdateInfo ();
	}
	
	public void init (CardSample cardSample)
	{
		initClear ();
		this.cardSample = cardSample;
		this.count = cardSample.count;
		quality = cardSample.qualityId;
		UpdateInfo ();
	}
	
	public void init (string path, int count, int quality)
	{
		initClear ();
		this.path = path;
		this.count = count;
		this.quality = quality;
		UpdateInfo ();
	}
	
	public void init (PrizeSample prize)
	{
		initClear ();
		this.prize = prize;
		this.count = prize.getPrizeNumByInt ();
		quality = prize.getQuality ();
		UpdateInfo ();
	}
	public void init (PrizeSample prize, bool _need)
	{
		initClear ();
		this.prize = prize;
		this.count = prize.getPrizeNumByInt ();
		quality = prize.getQuality ();
		if (_need) {
			//specialName.gameObject.SetActive(true);
			if(quality == 5)
				specialName.color = color5;
			if(quality == 4)
				specialName.color = color4;
			if(quality == 3)
				specialName.color = color2;
			if(quality == 2)
				specialName.color = color3;	//这里颜色反了，继续反
			if(quality == 1)
				specialName.color = color1;	
			//specialName.text = prize.getPrizeName();
		}
		specialName.gameObject.SetActive(true);
		specialName.text = prize.getPrizeName();
		UpdateInfo ();
	}
	public void init (PrizeSample prize, int num){
		initClear ();
		this.prize = prize;
		this.count = num;
		quality = prize.getQuality ();
		UpdateInfo ();
	}

	public void init (int type,int sid,int num)
	{
		if (type == PrizeType.PRIZE_CARD) {
			init(CardManagerment.Instance.createCard (sid),num);
		} else if (type == PrizeType.PRIZE_EQUIPMENT) {
			init(EquipManagerment.Instance.createEquip (sid),num);
		} else if (type == PrizeType.PRIZE_PROP) {
			init(PropManagerment.Instance.createProp (sid),num);
		} else if (type == PrizeType.PRIZE_MONEY) {
			init(new PrizeSample(type,0,num));
		}else if (type == PrizeType.PRIZE_RMB) {
			init(new PrizeSample(type,0,num));
		}else if (type == PrizeType.PRIZE_STARSOUL) {
			init(new PrizeSample(type,sid,num));
		}else {
			init(new PrizeSample(type,0,num));
		}
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
		path = null;
		card = null;
        magicWeapon = null;
		cardSample = null;
		starSoul = null;
	}

	private void initObj ()
	{
		customIconPoint.SetActive(false);
		backGround.spriteName = "";
		if (spriteScrap != null)
			spriteScrap.spriteName = "";
		if (rightBottomSprite != null)
			rightBottomSprite.spriteName = "";
		rightBottomText.text = "";
		showName = "";
	}
	
	public void clear ()
	{
		initItem ();
		onClickCallback = null;
		longPassCallback = null;
	}

	/** 清理方法 */
	public void clean () {
		initObj ();
		clear ();
	}

	/** 连接环绕,烟花特效点 */
	public void linkQualityEffectPoint ()
	{
		linkQualityEffectPointByRotate ();
		linkQualityEffectPointByFireworks ();
	}

	/** 连接烟花特效点 */
	public void linkQualityEffectPointByFireworks ()
	{
		fireworksEffctPoint = transform.FindChild ("fireworksEffctPoint").gameObject;
		fireworksEffctPoint.SetActive (true);
	}

	/** 连接环绕特效点 */
	public void linkQualityEffectPointByRotate ()
	{
		qualityEffectPoint = transform.FindChild ("effectPoint").gameObject;
		qualityEffectPoint.SetActive (true);
	}

	/** 连接新图标精灵 */
	public void linkFirstSprite ()
	{
		firstSprite = transform.FindChild ("first").gameObject;
		firstSprite.SetActive (true);
	}

	public void reset() {
		resetRightBottomSprite ();
		if(customIconPoint!=null)
			customIconPoint.gameObject.SetActive (false);
	}

	public void resetRightBottomSprite() {
		if (rightBottomSprite != null) {
			rightBottomSprite.spriteName = "";
			rightBottomSprite.gameObject.SetActive(false);
			rightBottomSprite.transform.localScale = Vector3.one;
		}
	}
    /// <summary>
    /// 卡片星级星星显示
    /// </summary>
    public void showStar(int level,int type) {
        if (stars != null) {
            if (level <= 0) return;
            for (int i = 0; i < stars.transform.childCount; i++) {
                stars.transform.GetChild(i).gameObject.SetActive(false);
            }
            stars.transform.localPosition = new Vector3(0, 0, 0);
            for (int i = 0; i < level; i++) {
                stars.transform.GetChild(i).gameObject.SetActive(true);
            }
            if (type == CardSampleManager.USEDBYCARD) {
                if (level == CardSampleManager.ONESTAR) {
                    stars.transform.localPosition = new Vector3(33, -31, 0);
                } else if (level == CardSampleManager.TWOSTAR) {
                    stars.transform.localPosition = new Vector3(22, -31, 0);
                } else if (level == CardSampleManager.THREESTAR) {
                    stars.transform.localPosition = new Vector3(11, -31, 0);
                } else if (level == CardSampleManager.FOURSTAR) {
                    stars.transform.localPosition = new Vector3(0, -31, 0);
                }
            }
        }
    }
	private void UpdateInfo ()
	{
        if (stars != null) {//把星星全部隐藏
            for (int i = 0; i < stars.transform.childCount; i++) {
                stars.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
		reset ();
		if (equip != null) {
			showName = QualityManagerment.getQualityColor(equip.getQualityId ()) + equip.getName ();
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + equip.getIconId (), icon);
			if (spriteScrap != null)
				spriteScrap.spriteName = "sign_scrap";
			backGround.spriteName = QualityManagerment.qualityIDToIconSpriteName (quality);
			setCountActive (count > 0);
			if (count / 1000000 > 0)
				rightBottomText.text = "x" + count / 10000 + "W";
			else
				rightBottomText.text = "x" + count;
		} else if(starSoul != null) {
			UpdateStarSoulView(starSoul);
		}else if(magicWeapon!=null){
            UpdateMagicWeapon();
        } 
        else if (prop != null) {
			showName = QualityManagerment.getQualityColor(prop.getQualityId ()) + prop.getName ();
			if (spriteScrap != null) {
				if (prop.isScrap()) {
					spriteScrap.gameObject.SetActive (true);
					spriteScrap.spriteName = "sign_scrap";
				} else {
					spriteScrap.gameObject.SetActive (false);
				}
			}
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + prop.getIconId (), icon);
			backGround.spriteName = QualityManagerment.qualityIDToIconSpriteName (quality);
			setCountActive (count > 0);
			if (count / 1000000 > 0)
				rightBottomText.text = "x" + count / 10000 + "W";
			else
				rightBottomText.text = "x" + count;
            if (prop.isCardScrap() && stars != null) {
                Card tempCard = CardScrapManagerment.Instance.getCardByScrapSid(prop.sid);
                if (tempCard != null) {
                    int level = CardSampleManager.Instance.getRoleSampleBySid(tempCard.sid).sFlagLevel;
                    showStar(level, CardSampleManager.USEDBYCARD);
                }
            }
            if (prop.isMagicScrap() && stars != null) {
                MagicWeapon tmpMagic = MagicWeaponScrapManagerment.Instance.getMagicWeaponByScrapSid(prop.sid);
                if (tmpMagic != null) {
                    int level = MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(tmpMagic.sid).starLevel;
                    showStar(level, CardSampleManager.USEDBYCARD);
                }
            }
			
		} else if (prize != null) {
			showName = QualityManagerment.getQualityColor(prize.getQuality ()) + prize.getPrizeName ();
			if (prize.type == PrizeType.PRIZE_PROP) {
				Prop propTemp = PropManagerment.Instance.createProp(prize.pSid);
				if (spriteScrap != null) {
					if (propTemp.isScrap()) {
						spriteScrap.spriteName = "sign_scrap";
						spriteScrap.gameObject.SetActive (true);
					} else {
						spriteScrap.gameObject.SetActive (false);
					}
				}
                if (propTemp.isCardScrap() && stars != null) {
                    Card tempCard = CardScrapManagerment.Instance.getCardByScrapSid(propTemp.sid);
                    if (tempCard != null) {
                        int level = CardSampleManager.Instance.getRoleSampleBySid(tempCard.sid).sFlagLevel;
                        showStar(level, CardSampleManager.USEDBYCARD);
                    }
                }
				backGround.spriteName = QualityManagerment.qualityIDToIconSpriteName (prize.getQuality());
			}
			if(prize.type==PrizeType.PRIZE_STARSOUL) {
				StarSoul starsoulView = StarSoulManager.Instance.createStarSoul (prize.pSid);
				iconType=BOTTOM_TEXT_NUM;
				UpdateStarSoulView(starsoulView);
			}  else {
                if (prize.type == PrizeType.PRIZE_CARD) {
                    Card tempCard = CardManagerment.Instance.createCard(prize.pSid);
                    if (tempCard != null && stars != null) {
                        int level = CardSampleManager.Instance.getRoleSampleBySid(tempCard.sid).sFlagLevel;
                        showStar(level,CardSampleManager.USEDBYCARD);
                    }
                } else if (prize.type == PrizeType.PRIZE_MAGIC_WEAPON) {
                    MagicWeaponSample magic = MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(prize.pSid);
                    if (magic != null) {
                        if (stars != null)
                            showStar(magic.starLevel, CardSampleManager.USEDBYCARD);
                    }
                }
				ResourcesManager.Instance.LoadAssetBundleTexture (prize.getIconPath (), icon);
				if (spriteScrap != null)
					spriteScrap.spriteName = "sign_scrap";
                if (prize.type == PrizeType.PRIZE_MONEY) spriteScrap.gameObject.SetActive(false);
				backGround.spriteName = QualityManagerment.qualityIDToIconSpriteName (quality);
				setCountActive (count > 0);
				if (count / 1000000 > 0)
					rightBottomText.text = "x" + count / 10000 + "W";
				else
					rightBottomText.text = "x" + count;
			}
		} else if (path != null) {
			ResourcesManager.Instance.LoadAssetBundleTexture (path, icon);
			if (spriteScrap != null)
				spriteScrap.spriteName = "sign_scrap";
			setCountActive (count > 0);
			if (count / 1000000 > 0)
				rightBottomText.text = "x" + count / 10000 + "W";
			else
				rightBottomText.text = "x" + count;
			if (quality >= 0) {
				backGround.spriteName = QualityManagerment.qualityIDToIconSpriteName (quality);
			}
		} else if (card != null) {
			showName = QualityManagerment.getQualityColor(card.getQualityId ()) + card.getName ();
			if(card.sid<=10)
				ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.MAINCARD_ICONIMAGEPATH + card.getMainCardImageIDBysid(card.sid).ToString (), icon);
			else
				ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + card.getIconID ().ToString (), icon);
			if (spriteScrap != null)
				spriteScrap.spriteName = "sign_scrap";
			backGround.spriteName = QualityManagerment.qualityIDToIconSpriteName (quality);
			if (count == 0)
				count = 1;
			setCountActive (count > 0);
			rightBottomText.text = "x" + count;
            int level = CardSampleManager.Instance.getRoleSampleBySid(card.sid).sFlagLevel;
            showStar(level,CardSampleManager.USEDBYCARD);
		} else if (cardSample != null) {
			showName = QualityManagerment.getQualityColor(cardSample.qualityId) + cardSample.name;
			if (iconType == 1) {
				ResourcesManager.Instance.LoadAssetBundleTexture (UserManager.Instance.getIconPath (cardSample.iconID), icon);
			} else {
				ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + cardSample.iconID.ToString (), icon);
			}
			if (spriteScrap != null)
				spriteScrap.spriteName = "sign_scrap";
			if (cardSample.sid > 10) {
				backGround.spriteName = QualityManagerment.qualityIDToIconSpriteName (quality);
			}
			if (iconType != 1) {
				setCountActive (count > 0);
				rightBottomText.text = "x" + count;
			}
            showStar(cardSample.sFlagLevel, CardSampleManager.USEDBYCARD);
		}
		showEffectByQuality ();
	}
    /**更新秘宝的基本信息 */
    private void UpdateMagicWeapon(){
        showName = QualityManagerment.getQualityColor(magicWeapon.getMagicWeaponQuality()) + magicWeapon.getName();
        ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.ICONIMAGEPATH + magicWeapon.getIconId(), icon);
        backGround.spriteName = QualityManagerment.qualityIDtoMagicWeapon(magicWeapon.getMagicWeaponQuality());
        icon.SetRect(0f, 0f, 105, 105f);
        icon.transform.localPosition = new Vector3(0f,6f,1f);
        setCountActive(count > 0);
        if (count / 1000000 > 0)
            rightBottomText.text = "x" + count / 10000 + "W";
        else
            rightBottomText.text = "x" + count;
        rightBottomText.text = "";
        int level = MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(magicWeapon.sid).starLevel;
        showStar(level, CardSampleManager.USEDBYCARD);
				
    }

	/**  更新星魂视图 */
	private void UpdateStarSoulView(StarSoul starsoulView) {
		if (customIconPoint == null)
			return;
		backGround.spriteName = "iconback_3";
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
		if (starsoulView.isNew) {
			spriteScrap.gameObject.SetActive(true);
			if(fatherWindow.name=="StarSoulWindow"){
				spriteScrap.transform.localPosition=new Vector3(-48f,67f,0);
			spriteScrap.width=65;
			spriteScrap.height=66;
			}
			spriteScrap.spriteName="new3";
		} else {
			spriteScrap.gameObject.SetActive(false);
		}
		if (starsoulView.checkState (EquipStateType.LOCKED)) {
			if(rightBottomSprite!=null) {
				rightBottomSprite.gameObject.SetActive(true);
				rightBottomSprite.transform.localScale=new Vector3(1,2,1);
				rightBottomSprite.spriteName="lock";
			}
		} else {
			if(rightBottomSprite!=null) {
				rightBottomSprite.gameObject.SetActive(false);
			}
		}
		if (iconType == BOTTOM_TEXT_NAME) {
			rightBottomText.gameObject.SetActive (true);
			//rightBottomText.text = QualityManagerment.getQualityColor(starsoulView.getQualityId()) + starsoulView.getName ();
            rightBottomText.text = starsoulView.getName();
		} else if(iconType == BOTTOM_TEXT_NAME_LV){
			rightBottomText.gameObject.SetActive (true);
			//rightBottomText.text = QualityManagerment.getQualityColor(starsoulView.getQualityId()) + starsoulView.getName () +"[FFFFFF]"+"Lv."+starsoulView.getLevel();
            rightBottomText.text = starsoulView.getName() + "[FFFFFF]" + "Lv." + starsoulView.getLevel();
		} else if(iconType == BOTTOM_TEXT_NUM){
			rightBottomText.gameObject.SetActive (true);
			rightBottomText.text="x"+count;
		} else {
			rightBottomText.text="";
			rightBottomText.gameObject.SetActive (false);
		}
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
		effectCtrl.transform.parent = qualityEffectPoint.transform;
	}
	/** 根据外部品质进行环绕特效显示 */
	public void showEffectByQuality (int _quality)
	{
		if (qualityEffectPoint == null)
			return;
		_quality = _quality <= QualityType.GOOD ? QualityType.GOOD : _quality;
		Utils.RemoveAllChild (qualityEffectPoint.transform);
		EffectCtrl effectCtrl = EffectManager.Instance.CreateEffect(qualityEffectPoint.transform,qualityEffectPath,qualityEffectPaths [_quality - QualityType.GOOD]);
		effectCtrl.transform.localPosition = new Vector3(0,5,0);
		effectCtrl.transform.localScale = new Vector3(1.8f,1.8f,1);
		effectCtrl.transform.parent = qualityEffectPoint.transform;
	}
	
	/** 显示自身品质显示对应的烟花特效  */
	public void showFireworksEffectByQuality ()
	{
		if (fireworksEffctPoint == null)
			return;
		int quality = getQuality ();
		if (quality < QualityType.EPIC)
			return;
		Utils.RemoveAllChild (fireworksEffctPoint.transform);
		GameObject effectObj = fireworksEffctProfab [quality - QualityType.GOOD];
		if (effectObj != null) {
			GameObject fireworks = Instantiate (effectObj) as GameObject;
			fireworks.transform.parent = fireworksEffctPoint.transform;
			fireworks.transform.localPosition = Vector3.zero;
			fireworks.transform.localScale = Vector3.one;
		}
	}
	
	public void UpdateLableCount ()
	{
		if (equip != null) {
			setCountActive (count > 0);
			if (count / 1000000 > 0)
				rightBottomText.text = "x" + count / 10000 + "W";
			else
				rightBottomText.text = "x" + count;
		} else if (starSoul != null) {
			setCountActive (count > 0);
			rightBottomText.text = "x" + count;
		}  else if (prop != null) {
			setCountActive (count > 0);
			if (count / 1000000 > 0)
				rightBottomText.text = "x" + count / 10000 + "W";
			else
				rightBottomText.text = "x" + count;
			
		} else if (prize != null) {
			setCountActive (count > 0);
			if (count / 1000000 > 0)
				rightBottomText.text = "x" + count / 10000 + "W";
			else
				rightBottomText.text = "x" + count;
		} else if (path != null) {
			setCountActive (count > 0);
			if (count / 1000000 > 0)
				rightBottomText.text = "x" + count / 10000 + "W";
			else
				rightBottomText.text = "x" + count;
		} else if (card != null) {
			if (count == 0)
				count = 1;
			setCountActive (count > 0);
			rightBottomText.text = card.getName () + "x" + count;
		} else if (cardSample != null) {
			setCountActive (count > 0);
			rightBottomText.text = cardSample.name + "x" + count;
		}
		showLableCountEffect ();
	}
	
	private void showLableCountEffect ()
	{
		Vector3 v3 = rightBottomText.gameObject.transform.localScale;//初始大小
		TweenScale ts = TweenScale.Begin (rightBottomText.gameObject, 0.2f, new Vector3 (v3.x * 2, v3.y * 2, v3.z * 2));
		ts.PlayForward ();
		StartCoroutine (Utils.DelayRun (() => {
			ts.PlayReverse ();
		}, 0.2f));
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
	/** 执行长按事件 */
	public override void DoLongPass () {
		if (longPassCallback != null)
			longPassCallback ();
	}
	public void DefaultClickEvent ()
	{
		if (equip != null) {
			if(starLevelState != null){
				starLevelState.gameObject.SetActive(false);
			}
            MissionAwardWindow window = UiManager.Instance.getWindow<MissionAwardWindow>();
            if (window != null) {
                window.destoryWindow();
            }
			UiManager.Instance.openWindow<EquipAttrWindow> (
				(winEquip) => {
				winEquip.Initialize (equip, EquipAttrWindow.OTHER, null);
			});
		}  else if (starSoul != null) {
			UiManager.Instance.openDialogWindow<StarSoulAttrWindow> (
				(win) => {
				win.Initialize (starSoul, StarSoulAttrWindow.AttrWindowType.None);
			});
		} else if (prop != null) {
			UiManager.Instance.openDialogWindow <PropAttrWindow> (
				(winProp) => {
				winProp.Initialize (prop);
			});
		} else if (card != null) {
			CardBookWindow.Show (card, CardBookWindow.SHOW, null);
		} else  if (prize != null) {
			clickButton(prize);
		} else if (magicWeapon != null){
            UiManager.Instance.openWindow<MagicWeaponStrengWindow>((win) => {
                win.init(magicWeapon, MagicWeaponType.FORM_OTHER);
            });
        if (this.fatherWindow.transform.FindChild("root").gameObject.transform.FindChild("effectPoint") != null)
            this.fatherWindow.transform.FindChild("root").gameObject.transform.FindChild("effectPoint").gameObject.SetActive(false);
        }else  if (resource_type == PrizeType.PRIZE_RMB) {
			prize = new PrizeSample ();
			prize.type = PrizeType.PRIZE_RMB;
			prize.num = count.ToString ();
			UiManager.Instance.openDialogWindow <PropAttrWindow> (
				(winProp) => {
				winProp.Initialize (prize);
			});
		} else  if (resource_type == PrizeType.PRIZE_MONEY) {
			prize = new PrizeSample ();
			prize.type = PrizeType.PRIZE_MONEY;
			prize.num = count.ToString ();
			UiManager.Instance.openDialogWindow <PropAttrWindow> (
				(winProp) => {
				winProp.Initialize (prize);
			});
		} else  if (resource_type == PrizeType.PRIZE_MERIT) {
			prize = new PrizeSample ();
			prize.type = PrizeType.PRIZE_MERIT;
			prize.num = count.ToString ();
			UiManager.Instance.openDialogWindow <PropAttrWindow> (
				(winProp) => {
				winProp.Initialize (prize);
			});
		} else {
			MaskWindow.UnlockUI ();//如果都不是 则不弹出属性框
		} 
	}

	//创建可以点击的按钮
	private void clickButton (PrizeSample prize)
	{
		switch (prize.type) {
		case PrizeType.PRIZE_BEAST:
			Card beast = CardManagerment.Instance.createCard (prize.pSid);
			CardBookWindow.Show (beast, CardBookWindow.OTHER, null);
			if (fatherWindow != null && (fatherWindow is AllAwardViewWindow || fatherWindow is WarAwardWindow || fatherWindow is MissionAwardWindow || fatherWindow is ArenaIntegralAwardWindow)) {
				fatherWindow.finishWindow();
			}
			break;
		case PrizeType.PRIZE_CARD:
			Card card = CardManagerment.Instance.createCard (prize.pSid);
			CardBookWindow.Show (card, CardBookWindow.OTHER, null);
			if (fatherWindow != null && (fatherWindow is AllAwardViewWindow || fatherWindow is WarAwardWindow || fatherWindow is MissionAwardWindow || fatherWindow is ArenaIntegralAwardWindow)) {
				fatherWindow.finishWindow();
			}
			break;
        case PrizeType.PRIZE_MOUNT:
            UiManager.Instance.openWindow<MountShowWindow>((win) => {
                win.init(prize.pSid, MountStoreItem.IS_CAN_UNACTIVE);
            });
            break;
		case PrizeType.PRIZE_EQUIPMENT:
			Equip equip = EquipManagerment.Instance.createEquip (prize.pSid);
			UiManager.Instance.openWindow <EquipAttrWindow>((win)=>{
				win.Initialize (equip, EquipAttrWindow.OTHER, null);
			});
			if (fatherWindow != null && (fatherWindow is AllAwardViewWindow || fatherWindow is WarAwardWindow || fatherWindow is MissionAwardWindow || fatherWindow is ArenaIntegralAwardWindow)) {
				fatherWindow.finishWindow();
			}
			break;
		case PrizeType.PRIZE_STARSOUL:
			StarSoul starSoul = StarSoulManager.Instance.createStarSoul (prize.pSid);
			UiManager.Instance.openDialogWindow<StarSoulAttrWindow> (
				(win) => {
				win.Initialize (starSoul, StarSoulAttrWindow.AttrWindowType.None);
			});
			break;
		case PrizeType.PRIZE_STARSOUL_DEBRIS:
			//暂时处理，星魂碎片
			MaskWindow.UnlockUI();
			break;
		case PrizeType.PRIZE_MONEY:
			//暂时处理，有可能游戏币也显示详情
			MaskWindow.UnlockUI();
			break;
		case PrizeType.PRIZE_PROP:
			Prop prop = PropManagerment.Instance.createProp (prize.pSid);
            if (prop.isScrap()) {
                if (fatherWindow != null && fatherWindow is MissionAwardWindow) {
                    fatherWindow.finishWindow();
                }
            }
			UiManager.Instance.openDialogWindow<PropAttrWindow>((win)=>{
				win.Initialize (prop);
			});
			break;
		case PrizeType.PRIZE_RMB:
			//暂时处理，有可能软妹币也显示详情
			MaskWindow.UnlockUI();
			break;
		case PrizeType.PRIZE_PRESTIGE:
			MaskWindow.UnlockUI();
			break;
        case PrizeType.PRIZE_MAGIC_WEAPON:
             MagicWeapon mw=MagicWeaponManagerment.Instance.createMagicWeapon(prize.pSid);
             if (fatherWindow != null && fatherWindow is MissionAwardWindow) {
                 fatherWindow.finishWindow();
             }
             UiManager.Instance.openWindow<MagicWeaponStrengWindow>((win) => {
                 win.init(mw, MagicWeaponType.FORM_OTHER);
             });
            break;
		default:
			MaskWindow.UnlockUI();
			break;
		}
	}

	public class GoodsViewComp : Comparator
	{
		
		public int compare(object o1,object o2)
		{
			//显示物品从左到右按品质顺序显示,相同品质,按 装备,卡片,道具的顺序显示
			if(o1==null) return 1;
			if(o2==null) return -1;
			if(!(o1 is GameObject)||!(o2 is GameObject)) return 0;
			GameObject obj1=(GameObject)o1;
			GameObject obj2 = (GameObject)o2;
			GoodsView view1 =obj1.transform.GetComponent<GoodsView> ();
			GoodsView view2 = obj2.transform.GetComponent<GoodsView> ();
			if(view1==null||view2==null) return 0;
			if(view1.getQuality()==view2.getQuality())
			{
				if(view1.getStortType()>view2.getStortType())
					return -1;
				if(view1.getStortType()<view2.getStortType())
					return 1;
				return 0;
			}
			else
			{
				if(view1.getQuality()>view2.getQuality())
					return -1;
				if(view1.getQuality()<view2.getQuality())
					return 1;
				return 0;
			}
		}
	}
	//关闭特效
	public void closeEffectsShow(){
		qualityEffectPoint = transform.FindChild ("effectPoint").gameObject;
		qualityEffectPoint.SetActive (false);
	}
	//开启特效
	public void openEffectsShow(){
		qualityEffectPoint = transform.FindChild ("effectPoint").gameObject;
		qualityEffectPoint.SetActive (true);
	}
}