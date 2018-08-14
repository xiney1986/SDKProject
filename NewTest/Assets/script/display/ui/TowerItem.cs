using UnityEngine;
using System.Collections;

public class TowerItem : ButtonBase
{
	[HideInInspector]
	public Mission mission;
	/** 背景 */
	public UISprite backGround;
	/** Boss头像 */
	public UITexture bossTexure;
	/** 序号文字 */
	public UILabel indexLabel;
	/** 星星排版 */
	public UIGrid starGrid;
	/** 副本名字 */
	public UILabel nameLabel;
	/** 等级限制 */
	public UILabel levelLimit;
	/** 星星集合 */
	public UISprite[] starSpriteList;
	/** 线 */
	public GameObject[] line;
	/** 奖励聚集点 */
	public GameObject goodsViewPos;
	/** 奖励预制体 */
	public GameObject goodsViewPrefab;
    public RenderView render;
    public UITexture rendTexture;//3D投影
    public UILabel nameNum;
    public GameObject baoxiangObj;
	private int index;
	private bool isLevelComplete = false;//等级满足需求
	/** 展示的掉落 */
	private PrizeSample tmpPrize;
	/** 展示的掉落 */
	private GoodsView goodView;
    private PrizeSample[] awardList;

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
        //这里走开宝箱界面
        if (rendTexture.mainTexture != null) {
            (fatherWindow as ClmbTowerChooseWindow).clickAttack();
        }
	}
        void OnLoadingShow ()
	{ 
	//	UiManager.Instance.backGround.hideAllBackGround (true);
	}
     private void continueIntoMission ()
	{ 	
		int practiceId=MissionInfoManager.Instance.mission.sid;;
		int practiceLevel=MissionInfoManager.Instance.mission.starLevel;

		UiManager.Instance.openWindow<EmptyWindow> ((win) => {
			ScreenManager.Instance.loadScreen (4, OnLoadingShow, () => {
				UiManager.Instance.switchWindow<MissionMainWindow> ();});
		});
	}

	/// <summary>
	/// 初始化副本入口信息
	/// </summary>
	public void initButton (Mission mi, int index,WindowBase win)
	{
        this.mission = mi;
        this.awardList = CommandConfigManager.Instance.getMissionPrizeBySid(mission.sid);
		this.index = index;
        fatherWindow = win;
        gameObject.GetComponent<BoxCollider>().enabled = false;
		initUI ();
	}

	/// <summary>
	/// 初始化副本入口信息
	/// </summary>
	void initUI ()
	{
	    int cengNum = 0;
	    if (mission.sid < 151010)
	    {
	        cengNum = StringKit.toInt(mission.getMissionName().Substring(2, 1));
	    }
	    else
	    {
            cengNum = StringKit.toInt(mission.getMissionName().Substring(2,2));
	    }
       // if (mission.sid == 151010) cengNum = 10;
        nameLabel.text = LanguageConfigManager.Instance.getLanguage("towerShowWindow12", cengNum.ToString());
        render.gameObject.SetActive(false);
        //开放等级判断
        if (UserManager.Instance.self.getUserLevel() < MissionSampleManager.Instance.getMissionSampleBySid(mission.sid).level) {
            levelLimit.gameObject.SetActive(true);
            levelLimit.text = LanguageConfigManager.Instance.getLanguage("towerShowWindow13", MissionSampleManager.Instance.getMissionSampleBySid(mission.sid).level.ToString());
            isLevelComplete = false;
        } else {
            levelLimit.gameObject.SetActive(false);
            isLevelComplete = true;
        } 
        if (FuBenManagerment.Instance.getPassChapter() == mission.sid) {
            gameObject.GetComponent<BoxCollider>().enabled=true;
            baoxiangObj.SetActive(false);
            render.gameObject.SetActive(false);
            rendTexture.gameObject.SetActive(true);
            ResourcesManager.Instance.LoadAssetBundleTexture(UserManager.Instance.self.getImagePath(), rendTexture);
            //render.init(UserManager.Instance.self.getModelPath(), rendTexture, UserManager.Instance.self.getUserLevel() < MissionSampleManager.Instance.getMissionSampleBySid(mission.sid).level?true:false);
            if (FuBenManagerment.Instance.istheLashMission()) {
                render.gameObject.SetActive(false);
                rendTexture.gameObject.SetActive(false);
                indexLabel.text = index + "";
                indexLabel.gameObject.SetActive(true);
                backGround.MakePixelPerfect();
            } else {
                indexLabel.gameObject.SetActive(false);
                backGround.MakePixelPerfect();
            }
            return;
        }
        if (!FuBenManagerment.Instance.isPassThisChapter(mission.sid)) {
            
			backGround.spriteName = "towerDown";
            gameObject.GetComponent<BoxCollider>().enabled = false;
            baoxiangObj.SetActive(false);
            render.gameObject.SetActive(false);
            rendTexture.gameObject.SetActive(false);
			backGround.MakePixelPerfect ();
			return;
        } else if (FuBenManagerment.Instance.getPassChapter() != mission.sid&&awardList != null) {
            gameObject.GetComponent<BoxCollider>().enabled = false;
            baoxiangObj.SetActive(true);
            render.gameObject.SetActive(false);
            rendTexture.gameObject.SetActive(false);
        }
		indexLabel.text = index + "";
		indexLabel.gameObject.SetActive (true); 
        backGround.MakePixelPerfect ();
	}

	/// <summary>
	/// 更新副本难度信息
	/// </summary>
	void updateStar (int num, int allNum)
	{
		if (allNum == 0) {
			return;
		}
		if(allNum > starSpriteList.Length) {
			allNum = starSpriteList.Length;
		}
		for (int i = 0; i < allNum; i++) {
			if (i < num) {
				starSpriteList [i].spriteName = "star";
				starSpriteList [i].gameObject.SetActive (true);
			} else {
				starSpriteList [i].spriteName = "star_b";
				starSpriteList [i].gameObject.SetActive (true);
			}
		}

		starGrid.repositionNow = true;
	}

	/// <summary>
	/// 初始化奖励信息
	/// </summary>
	public void initAward () {
		Equip tmpEquip = null;
        if (awardList == null) return;
        if (tmpPrize == null) {
            for (int i = 0; i < awardList.Length; i++) {
                if (awardList[i].type == PrizeType.PRIZE_EQUIPMENT) {
                    tmpEquip = EquipManagerment.Instance.createEquip(awardList[i].pSid);
                    if (tmpEquip != null && !tmpEquip.isToEat()) {
                        tmpPrize = new PrizeSample(4, awardList[i].pSid, 1);
                        break;
                    } else {
                        continue;
                    }
                }
                if (awardList[i].type == PrizeType.PRIZE_CARD) {
                    tmpPrize = new PrizeSample(5, awardList[i].pSid, 1);
                    break;
                }
            }
            if (tmpPrize != null && goodView == null) {
                GameObject item = NGUITools.AddChild(goodsViewPos, goodsViewPrefab);
                GoodsView button = item.GetComponent<GoodsView>();
                button.fatherWindow = fatherWindow;
                button.onClickCallback = () => {
                    DoClickEvent();
                };
                button.init(tmpPrize);
                button.rightBottomText.text = "";
                goodView = button;
            }
        }
		awardDisplay ();
	}

	/// <summary>
	/// 奖励展示动画
	/// </summary>
	public void awardDisplay () {
		if (tmpPrize == null || goodView == null) {
			return;
		}
		goodsViewPos.gameObject.SetActive (true);
		TweenScale ts = TweenScale.Begin (goodsViewPos, 0.5f, new Vector3 (0.9f, 0.9f, 0.9f));
		ts.from = new Vector3 (0, 0, 0);
		int x = 0;
		if (this.transform.localPosition.x > 0) {
			x = -160;
		}
		else {
			x = 160;
			goodsViewPos.transform.localRotation = new Quaternion (0,180,0,0);
			goodView.transform.localRotation = new Quaternion (0,180,0,0);
		}
		TweenPosition tp = TweenPosition.Begin (goodsViewPos, 0.5f, new Vector3 (x, 0, 0));
		tp.from = new Vector3 (0, 0, 0);
		EventDelegate.Add (tp.onFinished, () => {
			TweenScale tp2 = TweenScale.Begin (goodsViewPos, 2f, new Vector3 (0.85f, 0.85f, 0.85f));
			tp2.from = new Vector3 (0.9f, 0.9f, 0.9f);
			tp2.style = UITweener.Style.PingPong;
		}, true);
	}

	/// <summary>
	/// 奖励隐藏
	/// </summary>
	public void awardHide () {
		goodsViewPos.transform.localPosition = Vector3.zero;
		goodsViewPos.transform.localScale = Vector3.zero;
		goodsViewPos.gameObject.SetActive (false);
	}

	/// <summary>
	/// 获得副本入口的序号
	/// </summary>
	public int getIndex () {
		return index;
	}

	/// <summary>
	/// 获得副本入口的坐标
	/// </summary>
	public Vector2 getPos () {
		return this.transform.localPosition;
	}
}
