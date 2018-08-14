using UnityEngine;
using System.Collections;

public class RoleView : ButtonBase
{
	public UISprite inbattle;
	public UITexture icon;
	public UISprite qualityBg;
	public UISprite quality;
	public GameObject jobBg;
	public UILabel job;
	public UISprite jobTextSprite;
	public UISprite jobSprite;
	public UISprite jobBian;
	public UILabel level;
	public UISprite levelBg;
	public bool hideInBattle = true;
	public UILabel evoLevel;
	public GameObject qualityEffect;


	//这4个字段RoleView本身并未使用,可用于关联
	public GameObject tempGameObj;
	public GameObject[] tempGameObjs;
	public object tempObj;
	public object[] tempObjs;
	public Card card;
	public CardSample sample;
	public PrizeSample prize;
	public CallBack<RoleView> onClickCallback;
	public int showType = CardBookWindow.SHOW;

	//公会战队伍 队员血量显示，该字段RoleView本身并未使用,可用于关联
	public HpbarCtrl hpBar;
	//是否阵亡
	private bool isdeath = false;
	//阵亡图标
	public GameObject deathMask;
    //卡片星级显示
    public GameObject starsPrefab;
	//新加血条显示，初始化方法
	public void init (Card card, WindowBase fatherWindow, CallBack<RoleView> onClickCallback,bool isShowHpBar,int [] hp)
	{
		this.card = card;
		this.fatherWindow = fatherWindow;
		this.onClickCallback = onClickCallback;
		if(isShowHpBar)
		{
			hpBar.gameObject.SetActive(true);
			hpBar.updateValue(hp[0],hp[1]);
			isdeath = hp[0]==0 ? true : false;
		}
		updateInfo ();
	}

	public void init (Card card, WindowBase fatherWindow, CallBack<RoleView> onClickCallback)
	{
		this.card = card;
		this.fatherWindow = fatherWindow;
		this.onClickCallback = onClickCallback;
		updateInfo ();
	}
	public void init (Card card, WindowBase fatherWindow, CallBack<RoleView> onClickCallback,bool bo)
	{
		this.card = card;
		this.fatherWindow = fatherWindow;
		this.onClickCallback = onClickCallback;
		updateStarSoulInfo ();
	}

	public void init (CardSample sample, WindowBase fatherWindow, CallBack<RoleView> onClickCallback)
	{
		this.sample = sample;
		this.fatherWindow = fatherWindow;
		this.onClickCallback = onClickCallback;
		updateInfo ();
	}

	public void init (PrizeSample prize, CallBack<RoleView> onClickCallback)
	{
		this.prize = prize;
		ResourcesManager.Instance.LoadAssetBundleTexture (prize.getIconPath (), icon);
		if(qualityBg!=null)
			qualityBg.spriteName = QualityManagerment.qualityIDToBackGround (prize.getQuality ());
	}
    /// <summary>
    /// 显示卡片的星级
    /// </summary>
    public void showStar() {
        if (starsPrefab != null) {
            if (this.gameObject.transform.FindChild("starContent(Clone)") != null) {
                DestroyImmediate(this.gameObject.transform.FindChild("starContent(Clone)").gameObject);
            }
            if (card != null && this.gameObject.transform.FindChild("starContent(Clone)") == null && CardSampleManager.Instance.getStarLevel(card.sid) > 0) {
                GameObject star = NGUITools.AddChild(this.gameObject, starsPrefab);
                ShowStars show = star.GetComponent<ShowStars>();
                show.initStar(CardSampleManager.Instance.getStarLevel(card.sid), CardSampleManager.USEDBYCARD);
            } else if (sample != null && this.gameObject.transform.FindChild("starContent(Clone)") == null && CardSampleManager.Instance.getStarLevel(sample.sid) > 0) {
                GameObject star = NGUITools.AddChild(this.gameObject, starsPrefab);
                ShowStars show = star.GetComponent<ShowStars>();
                show.initStar(sample.sFlagLevel, CardSampleManager.USEDBYCARD);
            }
        }
    }
	/// <summary>
	/// 从仓库中取出卡片刷新显示
	/// </summary>
	public void updateCard ()
	{
		if (card != null) {
			Card c = StorageManagerment.Instance.getRole (card.uid);
			if (c != null) {
				card = c;
				updateInfo ();
			}
		}
	}
	public void updateStarSoulInfo()
	{
		if (card != null) {
			if(inbattle!=null)
				inbattle.gameObject.SetActive (!hideInBattle && (ArmyManager.Instance.getAllArmyPlayersIds ().Contains (card.uid) || ArmyManager.Instance.getAllArmyAlternateIds ().Contains (card.uid)));

			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + card.getIconID ().ToString (), icon);
			if(quality!=null)quality.spriteName = QualityManagerment.qualityIDToString (card.getQualityId ());
			if(quality!=null)quality.gameObject.SetActive (true);
			if(qualityBg!=null) {
				qualityBg.spriteName = QualityManagerment.qualityIDToBackGround (card.getQualityId ());
				qualityBg.gameObject.SetActive (true);
			}
			//jobBg.gameObject.SetActive(true);
			if(job!=null)job.text = CardManagerment.Instance.jobIDToString (card.getJob ());
			if(job!=null)job.gameObject.SetActive (true);
			level.text = "Lv." + card.getLevel ();
			level.gameObject.SetActive (true);
			updateEvoLevel();
			showEffectByQuality(card.getQualityId());
		}
	}
	public void updateInfo ()
	{ 
		
		if (card != null) {
			if(inbattle!=null)
				inbattle.gameObject.SetActive (!hideInBattle && (ArmyManager.Instance.getAllArmyPlayersIds ().Contains (card.uid) || ArmyManager.Instance.getAllArmyAlternateIds ().Contains (card.uid)));
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + card.getImageID (), icon);
			if(deathMask != null){
				if(isdeath)
				{
					deathMask.gameObject.SetActive(true);
					deathMask.gameObject.transform.localScale = new Vector3 (1.4f,1.4f,1);
//					icon.color = Color.blue;
				}
				else
				{
					deathMask.gameObject.SetActive(false);
//					icon.color = Color.white;
				}
			}
			if(quality!=null)quality.spriteName = QualityManagerment.qualityIDToString (card.getQualityId ());
			if(quality!=null)quality.gameObject.SetActive (true);
			if(qualityBg!=null)qualityBg.spriteName = QualityManagerment.qualityIDToBackGround (card.getQualityId ());
			if(qualityBg!=null)qualityBg.gameObject.SetActive (true);
			if(job!=null)job.text = CardManagerment.Instance.jobIDToString (card.getJob ());
			if(job!=null)job.gameObject.SetActive (true);
			if(jobSprite!=null)jobSprite.gameObject.SetActive(true);
			if(jobSprite!=null)jobSprite.spriteName=QualityManagerment.qualityIconBgToBackGround(card.getQualityId());
			if(jobTextSprite!=null)jobTextSprite.gameObject.SetActive(true);
			if(jobTextSprite!=null){
				if(fatherWindow is IntensifyCardShowWindow){
					jobTextSprite.spriteName=CardManagerment.Instance.qualityIconTextToBackGround(card.getJob())+"s";
				}else{
					jobTextSprite.spriteName=CardManagerment.Instance.qualityIconTextToBackGround(card.getJob());
				}
			}
			if(jobBian!=null)jobBian.gameObject.SetActive(true);
			if(jobBian!=null)jobBian.spriteName=QualityManagerment.qualityBianToBackGround(card.getQualityId());
			if(levelBg!=null)levelBg.spriteName = QualityManagerment.qualityBianToBackGround(card.getQualityId());
			if(level!=null){level.text = "Lv." + card.getLevel ();level.gameObject.SetActive (true);}
			updateEvoLevel();
			showEffectByQuality(card.getQualityId());
            showStar();
		} else if (sample != null) {
			if(inbattle!=null)
				inbattle.gameObject.SetActive (false);
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + sample.imageID, icon);
			if(quality!=null)quality.spriteName = QualityManagerment.qualityIDToString (sample.qualityId);
			if(qualityBg!=null)qualityBg.spriteName = QualityManagerment.qualityIDToBackGround (sample.qualityId);
			if(job!=null)job.text = CardManagerment.Instance.jobIDToString (sample.job);
			if(jobSprite!=null)jobSprite.spriteName=QualityManagerment.qualityIconBgToBackGround(sample.qualityId);
			if(jobTextSprite!=null){
				if(fatherWindow is IntensifyCardShowWindow){
					jobTextSprite.spriteName=CardManagerment.Instance.qualityIconTextToBackGround(sample.job)+"s";
				}else{
					jobTextSprite.spriteName=CardManagerment.Instance.qualityIconTextToBackGround(sample.job);
				}
			}
			if(levelBg!=null)levelBg.spriteName = QualityManagerment.qualityBianToBackGround(sample.qualityId);
			level.text = "Lv." + sample.maxLevel.ToString ();//starLevel;//"Lv.1";
            showStar();
		}
	}

	public void updateEvoLevel(){
		if (evoLevel != null&&card!=null) {
			if (card.getEvoLevel () > 0) {
				evoLevel.gameObject.SetActive (true);
				if(card.isMainCard()){
					if(card.getSurLevel() > 0)
						evoLevel.text = "[FF0000]+" + card.getSurLevel().ToString();
					else
						evoLevel.gameObject.SetActive(false);
				}
				else
					evoLevel.text = "[FF0000]+" + card.getEvoLevel ();
			} else
				evoLevel.gameObject.SetActive (false);
		}
	}

	public override void DoClickEvent ()
	{
        if (this.fatherWindow.transform.FindChild("root").gameObject.transform.FindChild("effectPoint") != null)
        this.fatherWindow.transform.FindChild("root").gameObject.transform.FindChild("effectPoint").gameObject.SetActive(false);
		if (onClickCallback != null)
			onClickCallback (this);
		//8004000 8007000选择上阵卡片 135009000兑换碎片后上阵
		if (GuideManager.Instance.isEqualStep(8004000) || GuideManager.Instance.isEqualStep(8007000) || GuideManager.Instance.isEqualStep(135009000)) {
			GuideManager.Instance.doGuide ();
		}
	}

	public void DefaultClickEvent (RoleView view)
	{
		if (card != null) {
			if (fatherWindow != null) {
				fatherWindow.hideWindow ();
			}
			CardBookWindow.Show (card, showType, () => {
				fatherWindow.restoreWindow ();
			});
		} else {
//			Debug.LogError ("MaskWindow.UnlockUI();");
			MaskWindow.UnlockUI();
		}
	}

	public override void DoUpdate ()
	{
		if (evoLevel != null && evoLevel.gameObject != null && evoLevel.gameObject.activeSelf)
			evoLevel.alpha = sin ();
	}
	
	/** 连接卡片本身品质点 */
	public void linkEffectPoint ()
	{
		qualityEffect = transform.FindChild ("qualityEffect").gameObject;
		qualityEffect.SetActive (true);
	}
	/** 显示卡片本身品质  */
	public void showEffectByQuality (int qualityId)
	{
		if (qualityEffect == null)
			return;
		if (qualityId < QualityType.GOOD)
			return;
		Utils.RemoveAllChild (qualityEffect.transform);
		EffectManager.Instance.CreateEffect (qualityEffect.transform,"Effect/UiEffect/CardQualityEffect"+qualityId);
		if (qualityId == QualityType.GOOD)
			qualityEffect.transform.localPosition = new Vector3 (150,-20,1);
		else if(qualityId == QualityType.EPIC)
			qualityEffect.transform.localPosition = new Vector3 (167,-83,1);
		else if(qualityId == QualityType.LEGEND)
			qualityEffect.transform.localPosition = new Vector3 (160,-83,1);			
	}
}
