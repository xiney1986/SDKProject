using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**跳转窗口sid (int)说明
	 * 炼金术界面        	90090
	 * 钻石召唤界面    	90091
	 * 女神摇一摇界面		90092
	 * 讨伐界面          90093
	 * 高级裂魂界面 	    90094
	 * 神秘商店			90095
	 */
/// <summary>
/// 每日返利Item
/// </summary>
public class DailyRebateItem : ButtonBase
{
	public UILabel taskName;//任务名称
	public UILabel taskDesc;//任务描述
	public Task task;
	public NoticeWindow win;
	public GoodsView[] showItems;
	public ButtonBase gotoButton;
	public ButtonBase receiveButton;
	private TaskSample mSample;
	private DailyRebateContent dailyContent;
	private List<PrizeSample> prizeList;
	public void initialize (Task _task, NoticeWindow _win, DailyRebateContent _dailyContent)
	{
		dailyContent = _dailyContent;
		win = _win;
		updateTask (_task);
		gotoButton.fatherWindow = win;
		gotoButton.onClickEvent = onClickGo;
		receiveButton.fatherWindow = win;
		receiveButton.onClickEvent = onGetAwards;
	}
	
	private void onClickGo (GameObject obj) {
		switch (mSample.windowLinkSid) {
		case 90090:  //炼金术界面
			{
			if(UserManager.Instance.self.getUserLevel() >= mSample.showLv){
				if(UserManager.Instance.self.getVipLevel() < mSample.condition.conditions[Mathf.Min((task.index-1)*2+1,mSample.condition.conditions.Length -1)]){
					UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage ("notice_dailyRebate_07",mSample.condition.conditions[(task.index-1)*2+1].ToString()));
					return;
				}
				if(TaskManagerment.Instance.isComplete(task) || (task.condition >= mSample.condition.conditions[mSample.condition.conditions.Length - 2 ]) ){
					UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage ("notice_dailyRebate_05"));
					return;
				}
				win.entranceId = NoticeSampleManager.Instance.getNoticeSampleBySid(NoticeType.ALCHEMY_SID).entranceId;
				win.updateSelectButton(NoticeType.ALCHEMY_SID);
				win.initTopButton();
				MaskWindow.UnlockUI ();
			}else {
				showTip(mSample.showLv);
			}
			break;
			}
		case 90091:   //钻石召唤界面
			{
			if(UserManager.Instance.self.getUserLevel() >= mSample.showLv){
				if(UserManager.Instance.self.getVipLevel() < mSample.condition.conditions[Mathf.Min((task.index-1)*2+1,mSample.condition.conditions.Length -1)]){
					UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage ("notice_dailyRebate_07",mSample.condition.conditions[(task.index-1)*2+1].ToString()));
					return;
				}
				if(TaskManagerment.Instance.isComplete(task)  || (task.condition >= mSample.condition.conditions[mSample.condition.conditions.Length - 2 ]) ){
					UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage ("notice_dailyRebate_05"));
					return;
				}
				UiManager.Instance.openWindow<LuckyDrawDetailWindow> ((win) => {
					win.setLuckyDraw (LuckyDrawManagerment.Instance.getLuckyDrawBySid(81001)); 
				});
			}else{
				showTip(mSample.showLv);
			}
			break;
			}
		case 90092:  //女神摇一摇界面
			{
			if(UserManager.Instance.self.getUserLevel() >= mSample.showLv){
				if(UserManager.Instance.self.getVipLevel() < mSample.condition.conditions[Mathf.Min((task.index-1)*2+1,mSample.condition.conditions.Length -1)]){
					UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage ("notice_dailyRebate_07",mSample.condition.conditions[(task.index-1)*2+1].ToString()));
					return;
				}
				if(TaskManagerment.Instance.isComplete(task)  || (task.condition >= mSample.condition.conditions[mSample.condition.conditions.Length - 2 ]) ){
					UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage ("notice_dailyRebate_05"));
					return;
				}
				win.entranceId = NoticeSampleManager.Instance.getNoticeSampleBySid(NoticeType.GODDNESS_SHAKE_SID).entranceId;
				win.updateSelectButton(NoticeType.GODDNESS_SHAKE_SID);
				win.initTopButton();
				MaskWindow.UnlockUI ();
			}else{
				showTip(mSample.showLv);
			}
			break;
			}
		case 90093:  //讨伐界面 
			{
			if(UserManager.Instance.self.getUserLevel() >= mSample.showLv && UserManager.Instance.self.getVipLevel() >= 3){
				//Mathf.Min((task.index-1)*2+1,mSample.condition.conditions.Length -1)
				if(UserManager.Instance.self.getVipLevel() < mSample.condition.conditions[Mathf.Min((task.index-1)*2+1,mSample.condition.conditions.Length -1)]){
					UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage ("notice_dailyRebate_07",mSample.condition.conditions[(task.index-1)*2+1].ToString()));
					return;
				}
				if(TaskManagerment.Instance.isComplete(task)  || (task.condition >= mSample.condition.conditions[mSample.condition.conditions.Length - 2 ]) ){
					UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage ("notice_dailyRebate_05"));
					return;
				}
				FuBenGetCurrentFPort port = FPortManager.Instance.getFPort ("FuBenGetCurrentFPort") as FuBenGetCurrentFPort;
				port.getInfo((bool b)=>{
					if(b){
						UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage ("notice_dailyRebate_03"));
						return;
					}
					else{
						UiManager.Instance.openWindow<WarChooseWindow> ();
						return;
					}
				});
			}
			else {
				showTip(mSample.showLv,3);
			}
			break;
			}
		case 90094:  //高级裂魂界面
			{
			if(UserManager.Instance.self.getVipLevel() < mSample.condition.conditions[Mathf.Min((task.index-1)*2+1,mSample.condition.conditions.Length -1)]){
				UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage ("notice_dailyRebate_07",mSample.condition.conditions[(task.index-1)*2+1].ToString()));
				return;
			}
			if(TaskManagerment.Instance.isComplete(task) || (task.condition >= mSample.condition.conditions[mSample.condition.conditions.Length - 2 ]) ){
				UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage ("notice_dailyRebate_05"));
				return;
			}
            if (UserManager.Instance.self.getUserLevel() >= mSample.showLv && UserManager.Instance.self.getVipLevel() >= CommonConfigSampleManager.Instance.getSampleBySid<DailyRebateSample>(6).getTimesInt(1))
            {
				UiManager.Instance.openWindow<StarSoulWindow> ((win) => {
					win.init (1);
				}); 
			}else {
				showTip(mSample.showLv,4);
			}
			break;
			}
		case 90095:  //神秘商店	
			{
			if(UserManager.Instance.self.getVipLevel() < mSample.condition.conditions[Mathf.Min((task.index-1)*2+1,mSample.condition.conditions.Length -1)]){
				UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage ("notice_dailyRebate_07",mSample.condition.conditions[(task.index-1)*2+1].ToString()));
				return;
			}
			if(TaskManagerment.Instance.isComplete(task)  || (task.condition >= mSample.condition.conditions[mSample.condition.conditions.Length - 2 ]) ){
				UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage ("notice_dailyRebate_05"));
				return;
			}
			if(UserManager.Instance.self.getUserLevel() >= mSample.showLv){
				UiManager.Instance.openWindow<ShopWindow> ((win)=>{
					win.setTitle(LanguageConfigManager.Instance.getLanguage("shop_mystical"));
					win.init(ShopWindow.TAP_MYSTICAL_CONTENT);
				});
			}else {
				showTip(mSample.showLv);
			}
			break;
			}
		case 90096:  //神魔宝库
		{
			if(UserManager.Instance.self.getVipLevel() < mSample.condition.conditions[Mathf.Min((task.index-1)*2+1,mSample.condition.conditions.Length -1)]){
				UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage ("notice_dailyRebate_07",mSample.condition.conditions[(task.index-1)*2+1].ToString()));
				return;
			}
			if(TaskManagerment.Instance.isComplete(task)  || (task.condition >= mSample.condition.conditions[mSample.condition.conditions.Length - 2 ]) ){
				UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage ("notice_dailyRebate_05"));
				return;
			}

			FuBenGetCurrentFPort port = FPortManager.Instance.getFPort ("FuBenGetCurrentFPort") as FuBenGetCurrentFPort;
			port.getInfo((bool b)=>{
				if(b){
					UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
						win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("teamEdit_err03"),
						                LanguageConfigManager.Instance.getLanguage ("winLinkErr01"),(msgHandle) => {
							if (msgHandle.buttonID == MessageHandle.BUTTON_RIGHT) {
								UiManager.Instance.clearWindows (UiManager.Instance.getWindow<MainWindow> ());
								FuBenManagerment.Instance.inToFuben ();
							}
						});	});
				}
				else{
					if(UserManager.Instance.self.getUserLevel() >= mSample.showLv){
						FuBenInfoFPort _port = FPortManager.Instance.getFPort("FuBenInfoFPort") as FuBenInfoFPort;
						_port.info(intoTowerFuben, ChapterType.TOWER_FUBEN);
					}else {
						showTip(mSample.showLv);
					}
					return;
				}
			});
			break;
		}
	}
	}

	public void intoTowerFuben()
	{//添加过程记录
		if (FuBenManagerment.Instance.getTowerChapter() == null) return;
		FuBenManagerment.Instance.selectedChapterSid = FuBenManagerment.Instance.getTowerChapter().sid;//爬塔副本章节sid
		FuBenManagerment.Instance.selectedMapSid = 1;
		UiManager.Instance.openWindow<ClmbTowerChooseWindow>();
	}
	private void onGetAwards (GameObject obj) {
		if (!TaskManagerment.Instance.isComplete (task)) {
			//receiveButton.disableButton(true);
			showMessage (false);
			return;
		}
		UiManager.Instance.applyMask ();
		TaskCompleteFPort fport = FPortManager.Instance.getFPort ("TaskCompleteFPort") as TaskCompleteFPort;
		fport.access (task.sid, dailyContent,showMessage2);
//		TaskManagerment.Instance.completeTask (task.sid);
//		updateTask (task);
	}
	private void showMessage2()
	{
        //UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage ("notice_dailyRebate_04"));
        UiManager.Instance.createPrizeMessageLintWindow(prizeList.ToArray());
		updateTask (task);
		dailyContent.m_reLoad();
	}
	private void showMessage(bool done)
	{
		if(done)
			UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage ("notice_dailyRebate_04"));
		else
			UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage ("notice_dailyRebate_08"));
	}
	private void showTip(int userLv)
	{
		UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage ("notice_dailyRebate_01",userLv.ToString()));
	}
	private void showTip(int userLv,int vipLv)
	{
		UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage ("notice_dailyRebate_02",userLv.ToString(),vipLv.ToString()));
	}
	public void updateTask (Task _task) {
		if (_task == null) {
			return;
		}
		else {
			task = _task;
			mSample = TaskSampleManager.Instance.getTaskSampleBySid (task.sid);
			taskName.text = mSample.name + TaskManagerment.Instance.setCompleteProShow (task);
			if (task.index > mSample.condition.conditions.Length / 2) {
				receiveButton.gameObject.SetActive(true);
				receiveButton.disableButton (true);
				receiveButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("notice_dailyRebate_06");
			}
			else if(!TaskManagerment.Instance.isComplete (task)){
				gotoButton.gameObject.SetActive(true);
				receiveButton.gameObject.SetActive(false);
			}else{
				gotoButton.gameObject.SetActive(false);
				receiveButton.gameObject.SetActive(true);
				receiveButton.disableButton (false);
				receiveButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("s0309");
			}
			TaskSample ts = TaskSampleManager.Instance.getTaskSampleBySid(task.sid);
			if(task.index <= ts.condition.conditions.Length / 2 && UserManager.Instance.self.getVipLevel() < ts.condition.conditions[(task.index-1)*2+1]){
				receiveButton.disableButton(true);
				taskDesc.gameObject.SetActive(true);
				taskDesc.text = LanguageConfigManager.Instance.getLanguage("notice_dailyRebate_07",ts.condition.conditions[(task.index-1)*2+1].ToString());
				receiveButton.gameObject.SetActive(false);
				gotoButton.gameObject.SetActive(true);
				gotoButton.textLabel.text =  LanguageConfigManager.Instance.getLanguage("notice_dailyRebate_09");
				gotoButton.disableButton(true);
			}
			int index = task.index  <= ts.condition.conditions.Length / 2 ? task.index  : ts.condition.conditions.Length / 2 ;
			initAward(index - 1);
		}
	}
	private void initAward(int index)
	{
		prizeList = new List<PrizeSample> ();
		string[] strs = mSample.dailyPrizes[index].Split('#');
		for (int i = 0; i < strs.Length; i++) {			
			PrizeSample prize = new PrizeSample (strs [i], ',');
			prizeList.Add (prize);
		}
		for (int i=0; i < prizeList.Count && i < 3; i++) {
			showItems[i].gameObject.SetActive(true);
			showItems[i].init(prizeList[i]);
			showItems[i].fatherWindow = win;
		}

        if (task.sid == 130605 && UserManager.Instance.self.getVipLevel() < CommonConfigSampleManager.Instance.getSampleBySid<DailyRebateSample>(6).getTimesInt(1))
        {
			taskDesc.gameObject.SetActive(true);
            taskDesc.text = LanguageConfigManager.Instance.getLanguage("notice_dailyRebate_07", (CommonConfigSampleManager.Instance.getSampleBySid<DailyRebateSample>(6).getTimesInt(1)).ToString());
			receiveButton.gameObject.SetActive(false);
			gotoButton.gameObject.SetActive(true);
			gotoButton.textLabel.text =  LanguageConfigManager.Instance.getLanguage("notice_dailyRebate_09");
			gotoButton.disableButton(true);
			//receiveButton.disableButton(true);
			//receiveButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("notice_dailyRebate_09");
		}
		if(task.sid == 130604 && UserManager.Instance.self.getVipLevel() < 3){
			taskDesc.gameObject.SetActive(true);
			taskDesc.text = LanguageConfigManager.Instance.getLanguage("notice_dailyRebate_07","3");
			receiveButton.gameObject.SetActive(false);
			gotoButton.gameObject.SetActive(true);
			gotoButton.textLabel.text =  LanguageConfigManager.Instance.getLanguage("notice_dailyRebate_09");
			gotoButton.disableButton(true);
			//receiveButton.disableButton(true);
			//receiveButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("notice_dailyRebate_09");
		}
	}
	void Update()
	{
		updateTask (task);
	}
}
