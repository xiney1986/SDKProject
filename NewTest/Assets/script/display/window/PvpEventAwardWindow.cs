using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 扫荡Pvp结算界面
/// </summary>
public class PvpEventAwardWindow : WindowBase {

	/* fields */
	/** 排序对象 */
	GoodsView.GoodsViewComp comp=new GoodsView.GoodsViewComp();
	/*胜利次数显示label */
	public UILabel winCountLabel;
	/*奖励物品显示容器 */
	public GameObject awardContent;
	/*关闭按钮 */
	public GameObject closeButton;
	/*奖励物品预制件 */
	public GameObject goodsViewPrefab;
	/** 连胜次数 */
	int winCount;
	/** 连胜奖励列表 */
	List<GameObject> awardList;
	int setp;
	int nextSetp;
	
	/* methods */
	/** 初始化 */
	public void init(int winCount,Award award){
		this.winCount = winCount;
		initAwards (award);
	}
	/** 初始化奖励 */
	void initAwards(Award awards)
	{
		if (awards == null) return;
		awardList = new List<GameObject> ();
		CreateGoodsByAward (awardList,awards);
		SortAwardItem ();
	}
	/** 奖励条目排序 */
	void SortAwardItem()
	{
		if (awardList==null||awardList.Count<=1) return;
		GameObject[] objs=awardList.ToArray ();
		SetKit.sort (objs,comp);
		awardList.Clear ();
		foreach(GameObject obj in objs)
		{
			awardList.Add(obj);
		}
	}
	/** 创建奖励对象 */
	private void CreateGoodsByAward (List<GameObject> awards,Award aw)
	{
		if (aw != null) {
			List<PrizeSample> awardListt = AllAwardViewManagerment.Instance.exchangeAwardToPrize (aw);
			if(awardListt!=null&&awardListt.Count>0){
				int nameIndex = 0;
				for(int i=0;i<awardListt.Count;i++){
					nameIndex++;
					GameObject obj=NGUITools.AddChild(awardContent,goodsViewPrefab) as GameObject;
					obj.SetActive(false);
					GoodsView view = obj.transform.GetComponent<GoodsView> ();
					view.linkQualityEffectPoint ();
					view.fatherWindow = this;
					view.init(awardListt[i]);
					obj.name="goodsbutton_"+nameIndex;
					awardList.Add(obj);
				}
			}
		}
	}
	protected override void begin ()
	{
		loadShow();
		StartCoroutine (Utils.DelayRun (() => {
			NextSetp ();
		}, 0.2f));
		MaskWindow.UnlockUI ();
	}
	void Update ()
	{
		if (setp == nextSetp)
			return;
		if (setp == 0) {
			winCountLabel.gameObject.SetActive (true);
			TweenScale ts = TweenScale.Begin(winCountLabel.gameObject,0.15f,new Vector3 (1.4f, 1.4f, 1f));
			ts.method = UITweener.Method.EaseIn;
			ts.from = new Vector3 (5, 5, 1);
			EventDelegate.Add (ts.onFinished, () =>
			                   {
				iTween.ShakePosition (winCountLabel.gameObject, iTween.Hash ("amount", new Vector3 (0.03f, 0.03f, 0.03f), "time", 0.4f));
				iTween.ShakePosition (winCountLabel.gameObject, iTween.Hash ("amount", new Vector3 (0.01f, 0.01f, 0.01f), "time", 0.4f));
				StartCoroutine(Utils.DelayRun(()=>{
					NextSetp ();
				},0.1f));
			}, true);
		}
		if (setp == 1) {
			if (awardList == null || awardList.Count == 0) {
				NextSetp ();
			} else {
				awardContent.SetActive (true);
				TweenPosition tp = TweenPosition.Begin (awardContent, 0.15f, awardContent.transform.localPosition);
				tp.from = new Vector3 (0, -500, 0);
				EventDelegate.Add (tp.onFinished, () =>
				                   {
					float time = GoodsInAnimation (awardList);
					StartCoroutine (Utils.DelayRun (() =>
					                                {
						NextSetp ();
					}, time));
				}, true);
			}
		} else if (setp == 2) {
			closeButton.SetActive (true);
		}
		setp++;
	}
	/** 物品显示动画*/
	private float GoodsInAnimation (List<GameObject> list)
	{
		float time = 0.3f;
		foreach (GameObject obj in list) {
			obj.SetActive (true);
			GoodsInFireworksEffect(obj);
			TweenScale.Begin (obj, time, obj.transform.localScale).from = new Vector3 (5, 5, 0);
			time += 0.1f;
		}
		return time;
	}
	/**特效　*/
	private void GoodsInFireworksEffect(GameObject obj)
	{
		GoodsView view = obj.transform.GetComponent<GoodsView> ();
		view.showFireworksEffectByQuality ();
	}
	/** 显示UI */
	void loadShow()
	{
		winCountLabel.text = winCount.ToString ();
		loadShowAward ();
	}
	/** 奖励显示 */
	void loadShowAward(){
		if (awardList == null)
			return;
		for (int i = 0; i < awardList.Count; i++) {
			GameObject obj = awardList [i];
			obj.transform.localPosition = new Vector3 (i * 120, 0, 0);
			obj.transform.localScale =  new Vector3(1,1,1);
		}
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close")
		{
			finishWindow();
			SweepManagement.Instance.clearPvpAward();
		}
	}
	public void NextSetp ()
	{
		nextSetp++;
	}
}
