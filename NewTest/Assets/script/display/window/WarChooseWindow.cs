using UnityEngine;
using System.Collections;
using System;

public class WarChooseWindow : WindowBase
{
	public ContentWarChoose content;
	public UILabel ruleDescript;
	public UILabel warCountLabel;
    public UILabel warNum;
    public UITexture luckyBanner;
    public GameObject info;
	int[] missionList;
    public GameObject warChooseBarPrefab;
    public const int LIMITLEVEL = 40;//讨伐幸运日开启等级
	[HideInInspector]
	public WarChooseButton newWar;
    /** 计时器 */
    private Timer timer;

	protected override void begin ()
	{
		base.begin ();
        timer = TimerManager.Instance.getTimer(UserManager.TIMER_DELAY);
        timer.addOnTimer(refreshBanner);
        timer.start();
		FuBenManagerment.Instance.isWarAttackBoss = false;
	}
    public override void DoDisable() {
        base.DoDisable();
        if (timer != null) {
            timer.stop();
            timer = null;
        }
    }
	public override void OnOverAnimComplete ()
	{
		base.OnOverAnimComplete ();
		//切换后清空容器内容
		content.cleanAll();
	}

	public override void OnNetResume ()
	{
		base.OnNetResume ();
		refreshData ();
	}

	protected override void DoEnable ()
	{
		base.DoEnable ();
		refreshData ();
        refreshBanner();
	}
    /// <summary>
    /// 更换banner
    /// </summary>
    public void refreshBanner() {
        if (this == null || !gameObject.activeInHierarchy) {
            if (timer != null) {
                timer.stop();
                timer = null;
            }
            return;
        }
        info.SetActive(true);
        luckyBanner.gameObject.SetActive(false);
        DateTime dt = TimeKit.getDateTimeMillis(ServerTimeKit.getMillisTime());//获取服务器时间
        string week = dt.DayOfWeek.ToString();
        if (UserManager.Instance.self.getUserLevel() > LIMITLEVEL) {
            if (week == "Friday" || week == "Saturday" || week == "Sunday") {
                ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.CHAPTERDESCIMAGEPATH + "20_luckyBanner", luckyBanner);
                luckyBanner.gameObject.SetActive(true);
                info.SetActive(false);
            }
        }
    }
	public void refreshData ()
	{
		showInfo ();
		content.Initialize (missionList);
		StartCoroutine(Utils.DelayRun(()=>{
			if (newWar != null && StringKit.toInt (newWar.name) - 1 >= 5) {
				//				SpringPanel.Begin (content.gameObject, new Vector3 (content.transform.localPosition.x, -newWar.transform.localPosition.y, content.transform.localPosition.z), 9);
				//猜测这里可能是为了跳转到最后一条,但是这里有bug..暂时去掉
				//content.reLoad (missionList.Length, StringKit.toInt (newWar.name) - 1); 
			}
			if(GuideManager.Instance.isEqualStep(126005000)){
				GuideManager.Instance.guideEvent();
			}
			MaskWindow.UnlockUI ();
		},0.3f));
	}

    public void showInfo() {
		Chapter 	chapter=FuBenManagerment.Instance.getWarChapter ();
		warCountLabel.text = chapter.getNum () + "/" + chapter.getMaxNum ();
        warNum.text = chapter.getNum() + "/" + chapter.getMaxNum();
		missionList = FuBenManagerment.Instance.getAllShowMissions (chapter.sid);
		ruleDescript.text = LanguageConfigManager.Instance.getLanguage ("s0159");
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow ();
		}
	}
}
