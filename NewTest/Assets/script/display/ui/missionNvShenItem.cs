using UnityEngine;
using System.Collections;

public class missionNvShenItem : MonoBehaviour {

	public UISprite[] checkOn;//进度点
	public UISprite[] checkOnBg;//进度点背景
	public UITexture iconNvShen;//女神头像
	public UILabel jindu;
	public UISprite bg1;//底层BG
	public UISprite bg2;//上层BG
	public UILabel jinduTitle;
	public GameObject effectLight;//光效
	public GameObject effectFire;//火焰
	private int num;

	public void initWindow()
	{
		if(iconNvShen.gameObject.activeSelf)
			iconNvShen.gameObject.SetActive (false);
		for (int i = 0; i < checkOnBg.Length; i++) {
			checkOnBg[i].gameObject.SetActive (false);
		}
		for (int i = 0; i < checkOn.Length; i++) {
			checkOn[i].gameObject.SetActive (false);
		}
		jindu.text = "";
		bg1.gameObject.SetActive (false);
		bg2.gameObject.SetActive (false);
		jinduTitle.gameObject.SetActive (false);
	}

	public void initNvShen()
	{
		if(!iconNvShen.gameObject.activeSelf)
			iconNvShen.gameObject.SetActive (true);
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.NVSHENHEADPATH + HoroscopesManager.Instance.getStarByType(UserManager.Instance.self.star).getImageID() + "_head", iconNvShen);
		for (int i = 0; i < checkOnBg.Length; i++) {
			if(!checkOnBg[i].gameObject.activeSelf)
				checkOnBg[i].gameObject.SetActive (true);
		}
		for (int i = 0; i < checkOn.Length; i++) {
			if(!checkOn[i].gameObject.activeSelf)
				checkOn[i].gameObject.SetActive (true);
		}
		jindu.text = num + "/2";
		if(!bg1.gameObject.activeSelf)
			bg1.gameObject.SetActive (true);
		if(!bg2.gameObject.activeSelf)
			bg2.gameObject.SetActive (true);
		if(!jinduTitle.gameObject.activeSelf)
			jinduTitle.gameObject.SetActive (true);
	}

	public void initCheck(int num)
	{
		this.num = num;
		for (int i = 0; i < checkOn.Length; i++) {
			if (i < num) {
				checkOn[i].gameObject.SetActive (false);
			}
			else {
				checkOn[i].gameObject.SetActive (true);
			}
		}
	}

	public void showSuiPianEffect()
	{
		EffectManager.Instance.CreateEffect(iconNvShen.transform,"Effect/UiEffect/feature_open");
		initCheck(num + 1);
		jindu.text = num + "/2";
		TextTipWindow.Show(LanguageConfigManager.Instance.getLanguage("GuideError_04"));
		if (GuideManager.Instance.isDoesNotEqualStep(7001000) && GuideManager.Instance.isDoesNotEqualStep(12001000) && GuideManager.Instance.isDoesNotEqualStep(15001000)) {
			GuideManager.Instance.doGuide ();
		}
		GuideManager.Instance.guideEvent ();
		MaskWindow.UnlockUI();
	}

	//女神被解救动画
	public void showNvShenEffect(bool isTrue)
	{
		for (int i = 0; i < checkOn.Length; i++) {
			checkOn[i].gameObject.SetActive (true);
		}
		for (int i = 0; i < checkOnBg.Length; i++) {
			checkOnBg[i].gameObject.SetActive (true);
		}

		iconNvShen.gameObject.SetActive (true);
		bg1.gameObject.SetActive (true);
		bg2.gameObject.SetActive (true);
		jinduTitle.gameObject.SetActive (true);
		jindu.text = num + "/2";

		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.NVSHENHEADPATH + HoroscopesManager.Instance.getStarByType(UserManager.Instance.self.star).getImageID() + "_head", iconNvShen);

		iconNvShen.transform.localPosition = new Vector3(230,-150,0);
		TweenScale ts = TweenScale.Begin(iconNvShen.gameObject,0.4f,Vector3.one);
		ts.from = new Vector3(3f,3f,3f);
		EventDelegate.Add (ts.onFinished, () => {
			TweenPosition tp = TweenPosition.Begin(iconNvShen.gameObject,0.6f,new Vector3(0,24,0));
			EventDelegate.Add (tp.onFinished, () => {
				EffectManager.Instance.CreateEffect(iconNvShen.transform,"Effect/UiEffect/feature_open");

				if(isTrue) {
					initCheck(num + 1);
					jindu.text = num + "/2";
				}
				if (GuideManager.Instance.isDoesNotEqualStep(7001000) && GuideManager.Instance.isDoesNotEqualStep(12001000) && GuideManager.Instance.isDoesNotEqualStep(15001000)) {
					GuideManager.Instance.doGuide ();
				}
				GuideManager.Instance.guideEvent ();
				MaskWindow.UnlockUI();
			},true);
		},true);
	}

	//女神界面打开动画前奏
	public void showNvShenOpenEffect()
	{
		EffectManager.Instance.CreateEffect(iconNvShen.transform,"Effect/UiEffect/feature_open");
		TweenScale ts = TweenScale.Begin(iconNvShen.gameObject,0.4f,new Vector3(3f,3f,3f));
		TweenPosition tp = TweenPosition.Begin(iconNvShen.gameObject,0.4f,new Vector3(230,-150,0));
		EventDelegate.Add (tp.onFinished, () => {
			iconNvShen.transform.localPosition = new Vector3(0,24,0);
			iconNvShen.transform.localScale = Vector3.one;
		},true);
	}
}
