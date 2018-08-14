using UnityEngine;
using System.Collections;

/**
 * 女神解锁
 * @authro 陈世惟  
 * */
public class OpenNvShenWindow : WindowBase {

	public UITexture iconTexure;//女神
	public UITexture bing;//冰背景
    public UITexture backGround;//
	public UILabel descLabel;
	public UILabel descLabel1;
	public UILabel blinkLabel;
	public GameObject fuwenObj;//符文常态
	public GameObject openFuwenObj;//副本爆裂
	public GameObject openIceObj;//破冰
	public GameObject faObj;//创建点
	private bool flag;
	private BeastEvolve  beastEvolve;
	private bool tkflag;

	protected override void DoEnable ()
	{
		base.DoEnable ();
		if (UiManager.Instance.getWindow<MissionMainWindow> () != null && UiManager.Instance.getWindow<MissionMainWindow> ().gameObject.activeSelf) {
			UiManager.Instance.getWindow<MissionMainWindow> ().TweenerGroupOut();
		}
	}

	protected override void begin ()
	{
		base.begin ();
        ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.BACKGROUNDPATH + "ChouJiang_BeiJing", backGround);
        ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.BACKGROUNDPATH + "nvshen_ice", bing);


	}
	public void initWindowsWrite(int id){
       // ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.BACKGROUNDPATH + "ChouJiang_BeiJing", backGround);
	    string iconId = "";
	    if (CommandConfigManager.Instance.getNvShenClothType() == 0)
	        iconId = HoroscopesManager.Instance.getStarByType(UserManager.Instance.self.star).getImageID() + "c";
	    else iconId = HoroscopesManager.Instance.getStarByType(UserManager.Instance.self.star).getImageID() + "";
        ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.CARDIMAGEPATH + iconId, iconTexure, (obj) => {
			flag=false;
			GameObject frist = null;
			if (id != 3) {
				frist = Instantiate(fuwenObj) as GameObject;
				frist.transform.parent = faObj.transform;
				frist.transform.localPosition = Vector3.zero;
				frist.transform.localScale = Vector3.one;
			}
			
			switch(id) {
			case 1:
				descLabel.text = LanguageConfigManager.Instance.getLanguage("GuideError_03");
				
				StartCoroutine (Utils.DelayRun(()=>{
					iTween.ShakePosition (this.gameObject, iTween.Hash ("amount", new Vector3 (0.03f, 0.03f, 0.03f), "time", 0.4f));
					iTween.ShakePosition (this.gameObject, iTween.Hash ("amount", new Vector3 (0.01f, 0.01f, 0.01f), "time", 0.4f));
				},0.5f));
				StartCoroutine (Utils.DelayRun(()=>{
					NGUITools.AddChild (faObj, openFuwenObj);
				},1f));
				StartCoroutine (Utils.DelayRun(()=>{
					if (frist != null)
						frist.SetActive (false);
					MaskWindow.UnlockUI();
				},2.5f));
				break;
				
			case 2:
				break;
				
			case 3:
				//			descLabel.text = LanguageConfigManager.Instance.getLanguage("GuideError_05");
				descLabel.gameObject.SetActive (false);
				
				StartCoroutine (Utils.DelayRun(()=>{
					iTween.ShakePosition (this.gameObject, iTween.Hash ("amount", new Vector3 (0.03f, 0.03f, 0.03f), "time", 0.4f));
					iTween.ShakePosition (this.gameObject, iTween.Hash ("amount", new Vector3 (0.01f, 0.01f, 0.01f), "time", 0.4f));
				},0.5f));
				StartCoroutine (Utils.DelayRun(()=>{
					NGUITools.AddChild (faObj, openIceObj);
				},1.2f));
				StartCoroutine (Utils.DelayRun(()=>{
					bing.gameObject.SetActive (false);
				},2.5f));
				StartCoroutine (Utils.DelayRun(()=>{
					MaskWindow.UnlockUI();
				},2.5f));
				break;
				
			default:
				descLabel.text = LanguageConfigManager.Instance.getLanguage("GuideError_02");
				MaskWindow.UnlockUI();
				break;
			}
		});

	}
	public void initWindow(int id)
	{
	    if (CommandConfigManager.Instance.getNvShenClothType() == 0)
	        ResourcesManager.Instance.LoadAssetBundleTexture(
	            ResourcesManager.CARDIMAGEPATH +
	            HoroscopesManager.Instance.getStarByType(UserManager.Instance.self.star).getImageID() + "c", iconTexure);
	    else
	        ResourcesManager.Instance.LoadAssetBundleTexture(
	            ResourcesManager.CARDIMAGEPATH +
	            HoroscopesManager.Instance.getStarByType(UserManager.Instance.self.star).getImageID(), iconTexure);
		flag=false;
		GameObject frist = null;
		if (id != 3) {
			frist = Instantiate(fuwenObj) as GameObject;
			frist.transform.parent = faObj.transform;
			frist.transform.localPosition = Vector3.zero;
			frist.transform.localScale = Vector3.one;
		}

		switch(id) {
		case 1:
			descLabel.text = LanguageConfigManager.Instance.getLanguage("GuideError_03");

			StartCoroutine (Utils.DelayRun(()=>{
				iTween.ShakePosition (this.gameObject, iTween.Hash ("amount", new Vector3 (0.03f, 0.03f, 0.03f), "time", 0.4f));
				iTween.ShakePosition (this.gameObject, iTween.Hash ("amount", new Vector3 (0.01f, 0.01f, 0.01f), "time", 0.4f));
			},0.5f));
			StartCoroutine (Utils.DelayRun(()=>{
				NGUITools.AddChild (faObj, openFuwenObj);
			},1f));
			StartCoroutine (Utils.DelayRun(()=>{
				if (frist != null)
					frist.SetActive (false);
				MaskWindow.UnlockUI();
			},2.5f));
			break;

		case 2:
			break;

		case 3:
//			descLabel.text = LanguageConfigManager.Instance.getLanguage("GuideError_05");
			descLabel.gameObject.SetActive (false);

			StartCoroutine (Utils.DelayRun(()=>{
				iTween.ShakePosition (this.gameObject, iTween.Hash ("amount", new Vector3 (0.03f, 0.03f, 0.03f), "time", 0.4f));
				iTween.ShakePosition (this.gameObject, iTween.Hash ("amount", new Vector3 (0.01f, 0.01f, 0.01f), "time", 0.4f));
			},0.5f));
			StartCoroutine (Utils.DelayRun(()=>{
				NGUITools.AddChild (faObj, openIceObj);
			},1.2f));
			StartCoroutine (Utils.DelayRun(()=>{
				bing.gameObject.SetActive (false);
			},2.5f));
			StartCoroutine (Utils.DelayRun(()=>{
				MaskWindow.UnlockUI();
			},2.5f));
			break;

		default:
			descLabel.text = LanguageConfigManager.Instance.getLanguage("GuideError_02");
			MaskWindow.UnlockUI();
			break;
		}
	}
	public void initWindow(Card card,int id,bool bo,string dec)
	{
		tkflag=bo;
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + card.getImageID(), iconTexure);
		flag=false;
		GameObject frist = null;
		if (id != 3) {
			frist = Instantiate(fuwenObj) as GameObject;
			frist.transform.parent = faObj.transform;
			frist.transform.localPosition = Vector3.zero;
			frist.transform.localScale = Vector3.one;
		}
		
		switch(id) {
		case 1:
			descLabel.text = dec;
			
			StartCoroutine (Utils.DelayRun(()=>{
				iTween.ShakePosition (this.gameObject, iTween.Hash ("amount", new Vector3 (0.03f, 0.03f, 0.03f), "time", 0.4f));
				iTween.ShakePosition (this.gameObject, iTween.Hash ("amount", new Vector3 (0.01f, 0.01f, 0.01f), "time", 0.4f));
			},0.5f));
			StartCoroutine (Utils.DelayRun(()=>{
				NGUITools.AddChild (faObj, openFuwenObj);
			},1f));
			StartCoroutine (Utils.DelayRun(()=>{
				if (frist != null)
					frist.SetActive (false);
				MaskWindow.UnlockUI();
			},2.5f));
			break;
			
		case 2:
			break;
			
		case 3:
			string[] sp=dec.Split('^');
			if(sp.Length>1){
				descLabel.text=sp[1];
				descLabel1.text=sp[0];
			}else{
				descLabel.text = dec;
			}

			//descLabel.gameObject.SetActive (false);
			
			StartCoroutine (Utils.DelayRun(()=>{
				iTween.ShakePosition (this.gameObject, iTween.Hash ("amount", new Vector3 (0.03f, 0.03f, 0.03f), "time", 0.4f));
				iTween.ShakePosition (this.gameObject, iTween.Hash ("amount", new Vector3 (0.01f, 0.01f, 0.01f), "time", 0.4f));
			},0.5f));
			StartCoroutine (Utils.DelayRun(()=>{
				NGUITools.AddChild (faObj, openIceObj);
			},1.2f));
			StartCoroutine (Utils.DelayRun(()=>{
				bing.gameObject.SetActive (false);
			},2.5f));
			StartCoroutine (Utils.DelayRun(()=>{
				MaskWindow.UnlockUI();
			},2.5f));
			break;
			
		default:
			descLabel.text = LanguageConfigManager.Instance.getLanguage("GuideError_02");
			MaskWindow.UnlockUI();
			break;
		}
	}
	public void initWindowWirte(BeastEvolve selectedEvolv,int id){
		beastEvolve=selectedEvolv;
		flag=true;
	    string iconId = "";
	    if (CommandConfigManager.Instance.getNvShenClothType() == 0)
	        iconId = selectedEvolv.getBeast().getImageID() + "c";
	    else iconId = selectedEvolv.getBeast().getImageID()+ "";
        ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.CARDIMAGEPATH + iconId, iconTexure, (obj) => {
			flag=true;
			GameObject frist = null;
			switch(id) {
			case 1:
				break;
				
			case 2:
				break;
				
			case 3:
				descLabel.gameObject.SetActive (false);		
				StartCoroutine (Utils.DelayRun(()=>{
					iTween.ShakePosition (this.gameObject, iTween.Hash ("amount", new Vector3 (0.03f, 0.03f, 0.03f), "time", 0.4f));
					iTween.ShakePosition (this.gameObject, iTween.Hash ("amount", new Vector3 (0.01f, 0.01f, 0.01f), "time", 0.4f));
				},0.5f));
				StartCoroutine (Utils.DelayRun(()=>{
					NGUITools.AddChild (faObj, openIceObj);
				},1.2f));
				StartCoroutine (Utils.DelayRun(()=>{
					bing.gameObject.SetActive (false);
				},2.5f));
				StartCoroutine (Utils.DelayRun(()=>{
					MaskWindow.UnlockUI();
				},2.5f));
				break;
			default:
				descLabel.text = LanguageConfigManager.Instance.getLanguage("GuideError_02");
				MaskWindow.UnlockUI();
				break;
			}
		});
	}
	public void initWindow(BeastEvolve selectedEvolv,int id)
	{
		beastEvolve=selectedEvolv;
		flag=true;
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH ,selectedEvolv.getBeast(), iconTexure);
		GameObject frist = null;
		switch(id) {
		case 1:
			break;
			
		case 2:
			break;
			
		case 3:
			descLabel.gameObject.SetActive (false);		
			StartCoroutine (Utils.DelayRun(()=>{
				iTween.ShakePosition (this.gameObject, iTween.Hash ("amount", new Vector3 (0.03f, 0.03f, 0.03f), "time", 0.4f));
				iTween.ShakePosition (this.gameObject, iTween.Hash ("amount", new Vector3 (0.01f, 0.01f, 0.01f), "time", 0.4f));
			},0.5f));
			StartCoroutine (Utils.DelayRun(()=>{
				NGUITools.AddChild (faObj, openIceObj);
			},1.2f));
			StartCoroutine (Utils.DelayRun(()=>{
				bing.gameObject.SetActive (false);
			},2.5f));
			StartCoroutine (Utils.DelayRun(()=>{
				MaskWindow.UnlockUI();
			},2.5f));
			break;
		default:
			descLabel.text = LanguageConfigManager.Instance.getLanguage("GuideError_02");
			MaskWindow.UnlockUI();
			break;
		}
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "screenButton") {
			if(flag&&beastEvolve!=null){
				BeastAttrWindow baw=UiManager.Instance.getWindow<BeastAttrWindow>();
				if(baw!=null) {
					baw.Initialize (beastEvolve.getBeast (), BeastAttrWindow.STOREVIEW);
					baw.UpdateUI();
				}
				finishWindow();
				return;
			}
			//强制引导召唤女神后直接返回主界面,同事提前清空队伍缓存
			if(GuideManager.Instance.guideSid == 16006000)
			{
				ArmyManager.Instance. cleanAllEditArmy ();
				GuideManager.Instance.doGuide();
				UiManager.Instance.openMainWindow();
				dialogCloseUnlockUI=false;
				finishWindow();
				return;
			}
			if (GuideManager.Instance.isDoesNotEqualStep(7001000) && GuideManager.Instance.isDoesNotEqualStep(12001000) && GuideManager.Instance.isDoesNotEqualStep(15001000)) {
				GuideManager.Instance.doGuide ();
			}
			if (GuideManager.Instance.isEqualStep(6004000) || GuideManager.Instance.isEqualStep(11004000) || GuideManager.Instance.isEqualStep(13008000)
			    || GuideManager.Instance.isEqualStep(7001000) || GuideManager.Instance.isEqualStep(12001000) || GuideManager.Instance.isEqualStep(15001000)) {
				if (fatherWindow is MissionMainWindow) {
					GameManager.Instance.playAnimationType = 1;
					(fatherWindow as MissionMainWindow).nvshen ();
				}
			}else if(tkflag&&fatherWindow is MissionMainWindow){
				(fatherWindow as MissionMainWindow).showEffectForHero(true);
			}else if(!tkflag&&fatherWindow is MissionMainWindow){
				(fatherWindow as MissionMainWindow).showEffectForHero(false);
			}
			finishWindow();
			if (UiManager.Instance.getWindow<MissionMainWindow> () != null && UiManager.Instance.getWindow<MissionMainWindow> ().gameObject.activeSelf) {
				UiManager.Instance.getWindow<MissionMainWindow> ().TweenerGroupIn();
			}
		}
	}

	void Update ()
	{
		blinkLabel.alpha = sin ();
	}
}
