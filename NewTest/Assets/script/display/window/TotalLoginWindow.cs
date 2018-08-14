using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TotalLoginWindow : WindowBase
{ 
	/* const */
	/** 容器下标常量 */
	const int TAP_EVERYDAY_CONTENT=0, // 天天送
			  TAP_WEEKLY_CONTENT=1, // 周末送
			  TAP_HOLV_CONTENT=2;// 节日送
	/* fields */
	/** 预制件容器数组-天天送容器,周末送容器,节日送容器 */
	public GameObject[] contentPrefabs;
	public GameObject[] contents;
	public GameObject holidayNum;
	public GameObject weeklyNum;
	public UILabel holidayNumShow;
	public UILabel weeklyNumShow;
	public ButtonBase weeklyButton;
	public ButtonBase holidayButton;
	public ButtonBase everydayButton;

	/** 当前tap下标--0开始 */
	int currentTapIndex;
	public LoginAwardContent awardContent;
	/** tap容器 */
	public TapContentBase tapContent;
	/** 正在领取的登陆奖励 */
	TotalLogin receiveTotalLogin;
	public bool isFirstBoot = false;

	public bool IsFirstBoot{
		get{return isFirstBoot;}
		set{isFirstBoot=value;}
	}
	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}

	public void Initialize ()
	{ 
		updateShow (null);
	}

	public void Initialize (bool firstLogin)
	{ 
		isFirstBoot = firstLogin;
		updateShow (null);
	}
	//断线重连
	public override void OnNetResume ()
	{
		base.OnNetResume ();
		//updateShow (null);
		updateAwardContentAfterOnNetResume();
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			if (isFirstBoot) {
				UiManager.Instance.openWindow<MainWindow>();
				//2014.8.21 杨大侠说不弹出活动了，太烦
//				if(!UserManager.Instance.openNoticeWindow()){
//					finishWindow();
//					UiManager.Instance.openWindow<MainWindow>();
//				}
			} else {
				finishWindow ();
			}
		}else{
			GameObject content = getContent (currentTapIndex);
			if (currentTapIndex == TAP_EVERYDAY_CONTENT) {
				EverydayContent sshc = content.GetComponent<EverydayContent> ();
				sshc.buttonEventBase(gameObj);
			} else if (currentTapIndex == TAP_WEEKLY_CONTENT) {
				WeeklyAwardContent ssec = content.GetComponent<WeeklyAwardContent> ();
				ssec.buttonEventBase(gameObj);
			} else if (currentTapIndex == TAP_HOLV_CONTENT) {
				HolidayContent sssc = content.GetComponent<HolidayContent> ();
				sssc.buttonEventBase(gameObj);
			} 
		}
	}
	private void updateShow (MessageHandle msg)
	{
		if(!isFirstBoot){
			WeeklyAwardFPort fport=FPortManager.Instance.getFPort<WeeklyAwardFPort> ();
			fport.access(getHolidayAward);
		}else{
			updateAwardContent ();
		}		
	}
	private void getHolidayAward(){
		HolidayAwardFPort fport=FPortManager.Instance.getFPort<HolidayAwardFPort>();
		fport.access(TotalLoginManagerment.Instance.getHolidayActionsTate(),updateAwardContent);
	}
	public void updateAwardContent ()
	{	
		setTitle (LanguageConfigManager.Instance.getLanguage ("s0116", TotalLoginManagerment.Instance.getTotalDay ().ToString ()));
		weeklyButton.gameObject.SetActive(TotalLoginManagerment.Instance.WeeklyState);
		holidayButton.gameObject.SetActive(TotalLoginManagerment.Instance.HolidayState);
		everydayButton.gameObject.SetActive (TotalLoginManagerment.Instance.EverydayState);
		currentTapIndex=getSuitableContent();
		tapContent.changeTapPage(tapContent.tapButtonList[currentTapIndex]);

		//StartCoroutine (initContent(currentTapIndex));

	}
	// 断线重连以后刷新界面//
	public void updateAwardContentAfterOnNetResume()
	{
		setTitle (LanguageConfigManager.Instance.getLanguage ("s0116", TotalLoginManagerment.Instance.getTotalDay ().ToString ()));
		weeklyButton.gameObject.SetActive(TotalLoginManagerment.Instance.WeeklyState);
		holidayButton.gameObject.SetActive(TotalLoginManagerment.Instance.HolidayState);
		everydayButton.gameObject.SetActive (TotalLoginManagerment.Instance.EverydayState);
		currentTapIndex=getSuitableContent();

		StartCoroutine (initContent(currentTapIndex));
	}
	/// <summary>
	/// 得到合适的页面跳转容器//这里需要和后天沟通了再说啦
	/// </summary>
	/// <returns>The suitable content.</returns>
	private int getSuitableContent(){
		if(TotalLoginManagerment.Instance.getHaveHolidayAward()){
			return 2;
		}else{
			if(TotalLoginManagerment.Instance.getHaveWeeklyAwardShow()&&TotalLoginManagerment.Instance.getHaveWeeklyAward())
			return 1;
		}
		return 0;
	}
	TotalLogin[] SortLoginAward ()
	{
		TotalLogin[] awards = TotalLoginManagerment.Instance.getAvailableArray ();
		return awards;
	}

	protected override void DoEnable ()
	{
		base.DoEnable ();
		UiManager.Instance.backGround.switchBackGround ("loginBackGround");
	}
	/** 初始化容器 */
	private IEnumerator initContent(int tapIndex) {
		resetContentsActive ();
		GameObject content = getContent (tapIndex);
		switch (tapIndex) {
		case TAP_EVERYDAY_CONTENT:
			EverydayContent ssh = content.GetComponent<EverydayContent> ();
			ssh.init(this);
			break;
		case TAP_WEEKLY_CONTENT: 
			WeeklyAwardContent ec = content.GetComponent<WeeklyAwardContent> ();
			ec.init(this);
			break;
		case TAP_HOLV_CONTENT: 
			HolidayContent ssc = content.GetComponent<HolidayContent> ();
			ssc.init(this);
			break;
		}
//		GuideManager.Instance.guideEvent();
//		MaskWindow.UnlockUI ();
		yield break;
	}
	//重设激活状态
	public void resetContentsActive() {
		foreach (GameObject item in contents) {
			item.SetActive(false);
		}
	}
	/// <summary>
	/// 获取指定下标的容器
	/// </summary>
	/// <param name="contentPoint">容器点</param>
	/// <param name="tapIndex">下标</param>
	/// 
	private GameObject getContent(int tapIndex) {
		resetHolidayWeekShow();
		GameObject contentPoint = contents [tapIndex];
		contentPoint.SetActive (true);
		GameObject content;
		if (contentPoint.transform.childCount > 0) {
			Transform childContent=contentPoint.transform.GetChild (0);
			content = childContent.gameObject;
		} else {
			content = NGUITools.AddChild (contentPoint, contentPrefabs [tapIndex]);
		}
		return content;
	}
	/** 更新节点容器 */
	public void UpdateContent() {
		GameObject content = getContent (currentTapIndex);
		if (currentTapIndex == TAP_EVERYDAY_CONTENT) {
			EverydayContent sshc = content.GetComponent<EverydayContent> ();
			sshc.UpdateUI();
		} else if (currentTapIndex == TAP_WEEKLY_CONTENT) {
			WeeklyAwardContent ssec = content.GetComponent<WeeklyAwardContent> ();
			ssec.UpdateUI();
		} else if (currentTapIndex == TAP_HOLV_CONTENT) {
			HolidayContent sssc = content.GetComponent<HolidayContent> ();
			sssc.UpdateUI();
		} 
	}
	/** tap点击事件 */
	public override void tapButtonEventBase (GameObject gameObj, bool enable) {
		if (!enable)
			return;
		base.tapButtonEventBase (gameObj,enable);
		int tapIndex=int.Parse (gameObj.name)-1;
		StartCoroutine (initContent (tapIndex));
		this.currentTapIndex=tapIndex;
		
	}
	public  void resetHolidayWeekShow(){
		holidayNum.SetActive(false);
		weeklyNum.SetActive(false);
		int weekn=TotalLoginManagerment.Instance.getWeeklyAwardNum();
		int holidayn=TotalLoginManagerment.Instance.getHolidayAwardNum();
		if(weekn>0){
			weeklyNum.SetActive(true);
			weeklyNumShow.text=weekn.ToString();
		}
		if(holidayn>0){
			holidayNum.SetActive(true);
			holidayNumShow.text=holidayn.ToString();
		}
	}
}
