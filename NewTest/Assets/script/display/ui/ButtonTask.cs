using UnityEngine;
using System.Collections;

/**
 * 任务按钮
 * @author 汤琦
 * */
public class ButtonTask : ButtonBase
{
	public ButtonBase UI_GoBtn;
	public ButtonBase receiveButton;
	public UILabel taskName;//任务名称
	public UILabel taskDesc;//任务描述
	public UILabel progress;//任务进度
	public PrizesModule prizeIcon;//奖励图标
	public Task task;
	public TaskWindow win;
	private string prizeName;
	private int currentNum;
	private TaskSample mSample;
	
	public void initialize (Task _task)
	{
		win = fatherWindow as TaskWindow;
		prizeIcon.fatherWindow = win;
		updateTask (_task);

		UI_GoBtn.fatherWindow = fatherWindow;
		UI_GoBtn.onClickEvent = onClickGo;
		receiveButton.fatherWindow = fatherWindow;
		receiveButton.onClickEvent = onClickReceiveButton;
	}

	private void onClickGo (GameObject go) {
		if(GuideManager.Instance.isEqualStep(30002000))GuideManager.Instance.doGuide();
		WindowLinkManagerment.Instance.OpenWindow (mSample.windowLinkSid);
	}

	private void onClickReceiveButton (GameObject go) {
		if (isDrawPrizes ()) {
			Guild guild = GuildManagerment.Instance.getGuild ();
			GuildFightSampleManager instance = GuildFightSampleManager.Instance();
			if (mSample.taskType == 1 && guild != null && instance.isOverPower () && instance.isGuildFightOpen ()&&!TaskManagerment.Instance.isTips) {
				UiManager.Instance.openDialogWindow<GuildFightPowerTipWindow>( (win)=>{
					win.initWindow(UserManager.Instance.self.guildFightPower,instance.getTaskPowerBySid (GuildFightSampleManager.TASK_POWER),overTask);
				});
			} else {
				overTask (null);
			}

		} else {
			noDrawPrize ();
		}
	}

	private void overTask(MessageHandle msg) {
		if(msg != null && msg.buttonID == MessageHandle.BUTTON_LEFT)
			return;
		TaskCompleteFPort fport = FPortManager.Instance.getFPort ("TaskCompleteFPort") as TaskCompleteFPort;
		fport.access (task.sid, win, getAward);
	}
	private void overTask() {
		TaskCompleteFPort fport = FPortManager.Instance.getFPort ("TaskCompleteFPort") as TaskCompleteFPort;
		fport.access (task.sid, win, getAward);
	}
	private PrizeSample getPrize ()
	{
		TaskSample ts = TaskSampleManager.Instance.getTaskSampleBySid (task.sid);
		PrizeSample ps = ts.prizes [task.index - 1];
		currentNum = ps.getPrizeNumByInt ();
		return ps;
	}
    private PrizeSample getdrPrize() {
        TaskSample ts = TaskSampleManager.Instance.getTaskSampleBySid(task.sid);
        PrizeSample ps;
        if(!TaskManagerment.Instance.getTaskArr().Contains(task)){
             ps= ts.prizes[ts.prizes.Count-1];
        } else {
            ps = ts.prizes[task.index-2<=0?0:task.index-2];
        }
        currentNum = ps.getPrizeNumByInt();
        return ps;
    }
	
	public void updateTask (Task _task)
	{
		if (_task == null) {
			return;
		} else {
			task = _task;
			prizeIcon.initPrize (getPrize (), windowBack, win);
			mSample = TaskSampleManager.Instance.getTaskSampleBySid (task.sid);
			taskName.text = mSample.name;
			taskDesc.text = mSample.conditionDesc[task.index - 1];
			progress.text = TaskManagerment.Instance.setCompleteProShow (task);
			
			if (TaskManagerment.Instance.isComplete (task)) {
				receiveButton.gameObject.SetActive(true);
				//icon.spriteName = COMPLETE;
				UI_GoBtn.gameObject.SetActive (false);
			} else {
				//icon.spriteName = NOCOMPLETE;
				receiveButton.gameObject.SetActive(false);
				UI_GoBtn.gameObject.SetActive (true);
				if(mSample.windowLinkSid ==0){
					UI_GoBtn.disableButton(true);
					UI_GoBtn.textLabel.text = LanguageConfigManager.Instance.getLanguage("notice_dailyRebate_09");
				}
				else{
					UI_GoBtn.disableButton(false);
					UI_GoBtn.textLabel.text = LanguageConfigManager.Instance.getLanguage("combatTips_Go01");
				}
				//UI_GoBtn.gameObject.SetActive (mSample.windowLinkSid != 0);
			}
			//icon.gameObject.SetActive (!UI_GoBtn.gameObject.activeSelf);
		}
	}
	
	private void windowBack ()
	{
		UiManager.Instance.openWindow<TaskWindow> ((win)=>{
			win.Initialize(null);
		});
	}
	
	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		prizeIcon.DoClickEvent();
//		//GuideManager.Instance.doGuide(); 
//		if (icon.spriteName == COMPLETE) {
//			if (isDrawPrizes ()) {
//				UiManager.Instance.applyMask ();
//				TaskCompleteFPort fport = FPortManager.Instance.getFPort ("TaskCompleteFPort") as TaskCompleteFPort;
//				fport.access (task.sid, win, getAward);
//			} else {
//				noDrawPrize ();
//			}
//		} else {
//			prizeIcon.DoClickEvent();
//		}
	}

	void getAward ()
	{
		AwardManagerment.Instance.addFunc (AwardManagerment.AWARDS_ACHIEVE, drawPrize); 	
		
		PrizeSample ps = getPrize ();
		if (ps.type == PrizeType.PRIZE_CARD) {
			Card c = CardManagerment.Instance.createCard (ps.pSid);
			HeroRoadManagerment.Instance.activeHeroRoadIfNeed (c);
		}
	}

	private void drawPrize (Award[] award)
	{
		//jiar-1065 改为横幅提示
//		UiManager.Instance.openDialogWindow<MessageLineWindow>((winnn)=>{
//			winnn.Initialize(LanguageConfigManager.Instance.getLanguage ("s0177", prizeName, currentNum.ToString ()));
//		});
		UiManager.Instance.createPrizeMessageLintWindow(getdrPrize());
		drawPrizeBack();
	}
	
	private void drawPrizeBack ()
	{
		win.updateContent ();
		UiManager.Instance.cancelMask();
	}
	
	private void noDrawPrize ()
	{
		UiManager.Instance.openDialogWindow<MessageWindow>((window)=>{
			window.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, LanguageConfigManager.Instance.getLanguage ("s0111"), null);
		});
	}
	
	private bool isDrawPrizes ()
	{
		bool isDraw = true;
		TaskSample ts = TaskSampleManager.Instance.getTaskSampleBySid (task.sid);
		PrizeSample ps = ts.prizes [task.index - 1];
		if (ps.type == PrizeType.PRIZE_CARD) {
			Card card = CardManagerment.Instance.createCard (ps.pSid);
			prizeName = card.getName ();
			if (StorageManagerment.Instance.isRoleStorageFull (ps.getPrizeNumByInt ())) {
				isDraw = false;
			}
		} else if (ps.type == PrizeType.PRIZE_EQUIPMENT) {
			Equip equip = EquipManagerment.Instance.createEquip (ps.pSid);
			prizeName = equip.getName ();
			if (StorageManagerment.Instance.isEquipStorageFull (ps.getPrizeNumByInt ())) {
				isDraw = false;
			}
		} else if (ps.type == PrizeType.PRIZE_MONEY) {
			prizeName = LanguageConfigManager.Instance.getLanguage ("s0049");
			if (UserManager.Instance.self.getMoney () + ps.getPrizeNumByLong () > int.MaxValue) {
				isDraw = false;
			}
		} else if (ps.type == PrizeType.PRIZE_PROP) {
			Prop prop = PropManagerment.Instance.createProp (ps.pSid);
			prizeName = prop.getName ();
			if (StorageManagerment.Instance.isPropStorageFull (ps.pSid)) {
				isDraw = false;
			}
		} else if (ps.type == PrizeType.PRIZE_RMB) {
			prizeName = LanguageConfigManager.Instance.getLanguage ("s0048");
			if (UserManager.Instance.self.getRMB () + ps.getPrizeNumByLong () > int.MaxValue) {
				isDraw = false;
			}
		}
		return isDraw;
	}
}
