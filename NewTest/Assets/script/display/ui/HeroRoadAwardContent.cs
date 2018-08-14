using UnityEngine;
using System.Collections;

/// <summary>
/// 英雄之章通关奖励容器
/// </summary>
public class HeroRoadAwardContent : MonoBehaviour 
{

	/** 章 */
	public UISprite pass;
	/** 第N章 */
	public UILabel chapterLabel;
	/** 觉醒奖励容器 */
	public GameObject Awake;
	/** 觉醒奖励文字描述 */
	public UILabel awakeStr;
	/** 普通奖励容器 */
	public GameObject General;
	/** 荣誉容器 */
	public GameObject roadHonor;
	/** 荣誉数值 */
	public UILabel honorValue;
	/** 荣誉加号 */
	public UILabel honorValueLabel;
	/** 钻石奖励容器 */
	public GameObject roadRush;
	/** 钻石奖励 */
	public UILabel rushMoneyValue;
	/** 钻石加号 */
	public UILabel rushValueLabel;
	/** 卡片容器 */
	public GameObject roadRoles;
	/** 卡片图像 */
	public UITexture roadRoleTexture;
	/** 卡片品质图标 */
	public UISprite roadRoleQuality;
	/**  确定按钮 */
	public UIButton closeButton;
	/** 动画步骤帧 */
	int setp;
	int nextSetp;
	/** 荣誉奖励值 */
	int honorGap;
	/** 钻石奖励值 */
	int rmbGap;


	public void initHeroRoad (int honorGap,int rmbGap)
	{
		this.honorGap = honorGap;
		this.rmbGap = rmbGap;
		honorValue.text = honorGap.ToString ();
		rushMoneyValue.text = rmbGap.ToString ();
		HeroRoadManagerment.Instance.currentHeroRoad.conquestCount++;
		HeroRoad road = HeroRoadManagerment.Instance.currentHeroRoad;
		Card card = CardManagerment.Instance.createCard (road.sample.sid);
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + card.getImageID (), roadRoleTexture);
        chapterLabel.text = LanguageConfigManager.Instance.getLanguage("prefabzc16",road.conquestCount.ToString()); //string.Format (chapterLabel.text, road.conquestCount);
		roadRoleQuality.spriteName = QualityManagerment.qualityIDToString (card.getQualityId ())+"Bg";
		NextSetp ();
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
			roadRoles.gameObject.SetActive (true);		
			TweenScale ts = TweenScale.Begin (roadRoles.gameObject, 0.15f, Vector3.one);
			ts.method = UITweener.Method.EaseIn;
			ts.from = new Vector3 (5, 5, 1);
			EventDelegate.Add (ts.onFinished, () =>
			{
				iTween.ShakePosition (roadRoles.gameObject, iTween.Hash ("amount", new Vector3 (0.03f, 0.03f, 0.03f), "time", 0.4f));
				iTween.ShakePosition (roadRoles.gameObject, iTween.Hash ("amount", new Vector3 (0.01f, 0.01f, 0.01f), "time", 0.4f));
				StartCoroutine (Utils.DelayRun (() =>
				{
					roadRoleQuality.gameObject.SetActive (true);
					NextSetp ();
				}, 0.2f));
			}, true);
		} else if (setp == 1) {
			chapterLabel.gameObject.SetActive (true);
			TweenScale ts = TweenScale.Begin (chapterLabel.gameObject, 0.15f, chapterLabel.transform.localScale);
			ts.from = Vector3.zero;		
			EventDelegate.Add (ts.onFinished, () =>
			{			
				StartCoroutine (Utils.DelayRun (() =>
				{
					pass.gameObject.SetActive (true);
					TweenScale ts3 = TweenScale.Begin (pass.gameObject, 0.15f, Vector3.one);
					ts3.method = UITweener.Method.EaseIn;
					ts3.from = new Vector3 (5, 5, 1);
					EventDelegate.Add (ts3.onFinished, () =>
					{
						iTween.ShakePosition (pass.gameObject, iTween.Hash ("amount", new Vector3 (0.03f, 0.03f, 0.03f), "time", 0.4f));
						iTween.ShakePosition (pass.gameObject, iTween.Hash ("amount", new Vector3 (0.01f, 0.01f, 0.01f), "time", 0.4f));
						StartCoroutine (Utils.DelayRun (() =>
						{
							NextSetp ();
						}, 0.1f));
					}, true);
				}, 0.2f));
			}, true);
		}
		//honor
		else if (setp == 2) {
			General.SetActive(true);
			roadHonor.SetActive (true);
			roadHonor.transform.localPosition = new Vector3 (0, -162, 0);
			TweenPosition tp = TweenPosition.Begin (roadHonor, 0.15f, roadHonor.transform.localPosition);
			tp.from = new Vector3 (500, roadHonor.transform.localPosition.y, 0);
			int gap = honorGap;
			EventDelegate.Add (tp.onFinished, () =>
			{
				if (gap > 0) {
					honorValueLabel.gameObject.SetActive(true);
					TweenLabelNumber tln = TweenLabelNumber.Begin (honorValue.gameObject, 0.15f, gap);
					EventDelegate.Add (tln.onFinished, () =>
					{
						GameObject obj = MonoBase.Create3Dobj ("Effect/Other/Flash").obj;
						obj.transform.parent = roadHonor.transform;
						obj.transform.localScale = Vector3.one;
						obj.transform.localPosition = new Vector3 (0, 0, -600);
						StartCoroutine (Utils.DelayRun (() =>
						 {
							NextSetp ();
						}, 0.15f));
					}, true);
				} else {
					NextSetp ();
				}
			}, true);
		}
		//rush
		else if (setp == 3) {
			roadRush.SetActive (true);
			roadRush.transform.localPosition = new Vector3 (0, -246, 0);
			TweenPosition tp = TweenPosition.Begin (roadRush, 0.15f, roadRush.transform.localPosition);
			tp.from = new Vector3 (500, roadRush.transform.localPosition.y, 0);
			EventDelegate.Add (tp.onFinished, () =>
			{
				int num = rmbGap;
				if (num > 0) {
					rushValueLabel.gameObject.SetActive(true);
					TweenLabelNumber tln = TweenLabelNumber.Begin (rushMoneyValue.gameObject, 0.15f, num);
					EventDelegate.Add (tln.onFinished, () =>
					{
						GameObject obj = MonoBase.Create3Dobj ("Effect/Other/Flash").obj;
						obj.transform.parent = roadRush.transform;
						obj.transform.localScale = Vector3.one;
						obj.transform.localPosition = new Vector3 (0, 0, -600);
						StartCoroutine (Utils.DelayRun (() =>
						{
							NextSetp ();
						}, 0.1f));
					}, true);
				} else {
					NextSetp ();
				}
			}, true);
		} 
		else if (setp == 4) {
//			int count = HeroRoadManagerment.Instance.currentHeroRoad.conquestCount;
//			if (isShowRoadAwake()) {
//				General.SetActive(false);
//				Awake.SetActive (true);
//				awakeStr.text = HeroRoadManagerment.Instance.currentHeroRoad.getAwakeString (count - 1);
//				showHeroRoadAwake ();
//				closeButton.gameObject.SetActive (true);
//			} else {
//				Awake.SetActive(false);
//				closeButton.gameObject.SetActive (true);
//			}
			closeButton.gameObject.SetActive (true);
			MaskWindow.UnlockUI ();
		}
		setp++;
	}

	public bool isShowRoadAwake()
	{
		int[] array = HeroRoadManagerment.Instance.currentHeroRoad.getAwakeInfo ();
		int count = HeroRoadManagerment.Instance.currentHeroRoad.conquestCount;
		return array [count - 1] == 1;
	}

	//显示英雄之章觉醒内容
	private void showHeroRoadAwake ()
	{
		TweenScale ts = TweenScale.Begin (Awake.gameObject, 0.3f, Vector3.one);
		ts.method = UITweener.Method.EaseIn;
		ts.from = new Vector3 (5, 5, 1);
		EventDelegate.Add (ts.onFinished, () =>
		{
			iTween.ShakePosition (Awake.gameObject, iTween.Hash ("amount", new Vector3 (0.03f, 0.03f, 0.03f), "time", 0.4f));
			iTween.ShakePosition (Awake.gameObject, iTween.Hash ("amount", new Vector3 (0.01f, 0.01f, 0.01f), "time", 0.4f));
			StartCoroutine (Utils.DelayRun (() =>
			{
				awakeStr.gameObject.SetActive (true);
				TweenPosition tp = TweenPosition.Begin (awakeStr.gameObject, 0.3f, awakeStr.transform.localPosition);
				tp.from = new Vector3 (500, awakeStr.transform.localPosition.y, 0);
				closeButton.gameObject.SetActive (true);
			}, 0.3f));
		}, true);
	}
	public void NextSetp ()
	{
		nextSetp++;
	}
}
