using UnityEngine;
using System.Collections;

public class PracticeAwardWindow : WindowBase
{
	public UIGrid content;
	public GameObject goodsViewPrefab;

	/** 挑战记录容器 */
	public GameObject generalAwardContent;
	/** 本次挑战记录数 */
	public UILabel currentNumValue;
	/** 历史挑战记录数 */
	public GameObject historyNumValue;
	/** 挑战结算特效点 */
	//public GameObject missionClearingPoint;
	/** 破记录图标 */
	public GameObject recordIcon;
	/** 破纪录特效 */
	public GameObject ratingContent;
	/** 确定按钮 */
	public GameObject closeButton;
	/** 进行的副本 */
	Mission mission;
	/** update帧 */
	int setp;
	int nextSetp;
	/** 是否突破历史挑战记录 */
	bool isUpdateRecord;
	/** 副本结束的当前挑战记录 */
	int currentPracticePoint;
	/** 副本结束的历史最高挑战记录 */
	int historyPracticeHightPoint;

	public void init(Mission mission){
		this.currentPracticePoint = mission.currentPracticePoint;
		this.historyPracticeHightPoint = mission.historyPracticeHightPoint;
		isUpdateRecord = currentPracticePoint > historyPracticeHightPoint;
        if (isUpdateRecord)
        {//因为bug,暂时修改
            mission.historyPracticeHightPoint = currentPracticePoint;
            UserManager.Instance.self.practiceHightPoint = mission.historyPracticeHightPoint;
        }
	}
	public void updateAward(Award[] awards)
	{
		if(awards==null||awards.Length==0)
			return;
		PrizeSample[] prizes = AllAwardViewManagerment.Instance.exchangeAwardsToPrize (awards).ToArray ();	
		if (prizes != null && prizes.Length > 0)
		{
			for (int i = 0; i < prizes.Length; i++) 
			{
				createGoodsView(prizes[i],i);
			}
			content.repositionNow = true;
		}
		MaskWindow.UnlockUI ();
	}
	
	private void createGoodsView(PrizeSample prizeSample,int index) {
		if (prizeSample == null)
			return;
		GameObject a = NGUITools.AddChild (content.gameObject, goodsViewPrefab);
		a.name = StringKit.intToFixString (index + 1);
		GoodsView goodsButton = a.GetComponent<GoodsView> ();
		goodsButton.fatherWindow = this;
		goodsButton.onClickCallback = goodsButton.DefaultClickEvent;
		goodsButton.init(prizeSample);
	}


	protected override void begin ()
	{
		base.begin ();
		//playFubenBattleAnim();
		playAudio();
		StartCoroutine (Utils.DelayRun (() => {
			NextSetp ();
		}, 0.2f));
		if (isSetpOver ())
			MaskWindow.UnlockUI ();
	}

	void Update ()
	{
		if (setp == nextSetp)
			return;
		//评级
		if (setp == 0) {
			if(isUpdateRecord){
				ratingContent.SetActive (true);
				TweenScale ts = TweenScale.Begin (recordIcon.gameObject, 0.15f, Vector3.one);
				ts.method = UITweener.Method.EaseIn;
				ts.from = new Vector3 (5, 5, 1);
				EventDelegate.Add (ts.onFinished, () =>
				{
					iTween.ShakePosition (recordIcon.gameObject, iTween.Hash ("amount", new Vector3 (0.03f, 0.03f, 0.03f), "time", 0.4f));
					iTween.ShakePosition (recordIcon.gameObject, iTween.Hash ("amount", new Vector3 (0.01f, 0.01f, 0.01f), "time", 0.4f));
					StartCoroutine (Utils.DelayRun (() =>
					{
						NextSetp ();
					}, 0.1f));
				}, true);
			}
			else{
				NextSetp ();
			}
		}
		else if(setp == 1){
			generalAwardContent.SetActive(true);
			TweenPosition tp = TweenPosition.Begin (generalAwardContent, 0.15f, generalAwardContent.transform.localPosition);
			tp.from = new Vector3 (-500, generalAwardContent.transform.localPosition.y, 0);
			EventDelegate.Add (tp.onFinished, () => {
				TweenLabelNumber tln = TweenLabelNumber.Begin (currentNumValue.gameObject, 0.15f, currentPracticePoint);
					EventDelegate.Add (tln.onFinished, () => {
						GameObject obj = Create3Dobj ("Effect/Other/Flash").obj;
					obj.transform.parent = currentNumValue.transform;
						obj.transform.localScale = Vector3.one;
						obj.transform.localPosition = new Vector3 (0, 0, -600);
					}, true);
				tln = TweenLabelNumber.Begin (historyNumValue.gameObject, 0.15f, historyPracticeHightPoint);
					EventDelegate.Add (tln.onFinished, () => {
						GameObject obj = Create3Dobj ("Effect/Other/Flash").obj;
					obj.transform.parent = historyNumValue.transform;
						obj.transform.localScale = Vector3.one;
						obj.transform.localPosition = new Vector3 (0, 0, -600);
					}, true);
				StartCoroutine(Utils.DelayRun(()=>{
					NextSetp();},0.15f));
			}, true);
		}
		else if(setp == 2){
			closeButton.SetActive(true);
			MaskWindow.UnlockUI ();
		}
		setp++;
	}
	public void NextSetp ()
	{
		nextSetp++;
	}
	/** 动画是否播放结束 */
	public bool isSetpOver(){
		if (setp != 0 && setp == nextSetp)
			return true;
		return false;
	}
//	private void playFubenBattleAnim(){
//		passObj _obj = MonoBase.Create3Dobj ("Effect/UiEffect/battleAnim");
//		if(missionClearingPoint.transform.childCount==0){
//			_obj.obj.transform.parent=missionClearingPoint.transform;
//			_obj.obj.transform.localPosition=Vector3.zero;
//			_obj.obj.transform.localScale=Vector3.one;
//			BattleAnimCtrl battleAnimCtrl = _obj.obj.GetComponent<BattleAnimCtrl>();
//			battleAnimCtrl.battleClearing.transform.localPosition=Vector3.zero;
//			battleAnimCtrl.battleClearing.SetActive (true);
//			missionClearingPoint.SetActive(true);
//		}
//	}
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow();
			EventDelegate.Add(OnHide,()=>{
				MissionManager.instance.exit();
			});
		}
	}
	/// <summary>
	/// 播放通关音效
	/// </summary>
	private void playAudio()
	{
		AudioManager.Instance.PlayAudio(123);
	}
}
