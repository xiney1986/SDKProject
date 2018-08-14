using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BossAttackRankWindow : WindowBase
{
	public RankContent content;
	/** 横拖碰撞体，暂时没用到 */
	public UIDragScrollView dragView;
	/** 我的排名 */
	public UILabel lblMyRank;
	/** 更新提示 */
	public UILabel lab_intro;
	/** 暂无数据 */
	public UILabel label_tip;
    /** 奖励规则 */
    public GameObject helpInfoTips;
    /** 可获得的奖励 */
    public UILabel lalAward;
	int myRank;
    int awardNum;

	public Transform lblMyRankMidPosObj;
	public Transform buttonHelpMidPosObj;
	public Transform buttonHelp;
	public GameObject awardIcon;
	Vector3 lblMyRankOldPos;
	Vector3 buttonHelpOldPos;

	//计时器
	private Timer timer;
	DateTime dt;//获取服务器时间
	int dayOfWeek;
	int nowOfDay;
	int[] timeInfo;//开放时间
	int[] data;//开放日期

	protected override void DoEnable()
	{
		UiManager.Instance.backGround.switchBackGround("ChouJiang_BeiJing");
		if (MissionManager.instance != null)
			MissionManager.instance.hideAll ();
		timeInfo = CommandConfigManager.Instance.getOneOnOneBossTimeInfo();//开放时间
		data = CommandConfigManager.Instance.getOneOnOneBossData();//开放日期
		lblMyRankOldPos = lblMyRank.transform.localPosition;
		buttonHelpOldPos = buttonHelp.transform.localPosition;
	}
	protected override void begin ()
	{
        RankManagerment.Instance.loadData(RankManagerment.TYPE_BOSSDAMAGE, updateUI);
		MaskWindow.UnlockUI ();
	}

	void Update()
	{
		if(RankManagerment.Instance.updateRankItemTotalDamage)
		{
			RankManagerment.Instance.updateRankItemTotalDamage = false;
			RankManagerment.Instance.loadData(RankManagerment.TYPE_BOSSDAMAGE, updateUI);
		}

		dt = TimeKit.getDateTimeMillis(ServerTimeKit.getMillisTime());//获取服务器时间
		dayOfWeek = TimeKit.getWeekCHA(dt.DayOfWeek);
		nowOfDay = ServerTimeKit.getCurrentSecond();
		// 活动中//
		for (int i = 0; i < data.Length; i++)
		{
			if (dayOfWeek == data[i] && (nowOfDay >= timeInfo[0] && nowOfDay <= timeInfo[1]))
			{
				if(lblMyRank.transform.localPosition != lblMyRankOldPos && buttonHelp.localPosition != buttonHelpOldPos)
				{
					lalAward.gameObject.SetActive(true);
					awardIcon.SetActive(true);
					lblMyRank.transform.localPosition = lblMyRankOldPos;
					buttonHelp.localPosition = buttonHelpOldPos;
				}
			}
			// 活动未开//
			else
			{
				if(lblMyRank.transform.localPosition != lblMyRankMidPosObj.localPosition && buttonHelp.localPosition != buttonHelpMidPosObj.localPosition)
				{
					lalAward.gameObject.SetActive(false);
					awardIcon.SetActive(false);
					lblMyRank.transform.localPosition = lblMyRankMidPosObj.localPosition;
					buttonHelp.localPosition = buttonHelpMidPosObj.localPosition;
				}
			}
		}
	}
    void updateUI() {
        myRank = RankManagerment.Instance.getMyRank(RankManagerment.TYPE_BOSSDAMAGE);
        content.init(RankManagerment.TYPE_BOSSDAMAGE, RankManagerment.Instance.totalDamageList, this);
        showMyRank(string.Empty);
    }
    /// <summary>
    /// 根据排名获得奖励的英雄徽记数量
    /// </summary>
    /// <param name="rank"></param>
    /// <returns></returns>
    private int getNumByRank(int rank) {
        int[] temp = CommandConfigManager.Instance.getAwardNum();
        int[] limit = CommandConfigManager.Instance.getRankLimit();
        int num = 0;
        if (rank >= limit[0] && rank < limit[1]){
            num = temp[0];
        }else if (rank >= limit[1] && rank < limit[2]){
            num = temp[1];
        }else if (rank >= limit[2] && rank < limit[3]){
            num = temp[2];
        } else if (rank >= limit[3] && rank < limit[4]) {
            num = temp[3];
        } else if (rank >= limit[4] && rank < limit[5]) {
            num = temp[4];
        } else if (rank >= limit[5] && rank < limit[6]) {
            num = temp[5];
        } else if (rank >= limit[6] && rank < limit[7]) {
            num = temp[6];
		} else if (rank >= limit[7]) {
            num = temp[7];
        }
        return num;
    }
    /// <summary>
    /// 显示自己的排名信息
    /// </summary>
    /// <param name="rank"></param>
	private void showMyRank(string rank)
	{
		if (myRank == 0 || myRank > 200) 
            rank = LanguageConfigManager.Instance.getLanguage("rankWindow01");
        else
            rank = string.Format(LanguageConfigManager.Instance.getLanguage("s0414"), myRank);
        awardNum = getNumByRank(myRank);
		lblMyRank.text = rank;
        lalAward.text = LanguageConfigManager.Instance.getLanguage("OneOnOneBoss_013", "x" + awardNum);
		lab_intro.text = Language ("rankWindow_intro01");
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow ();
        } else if (gameObj.name == "buttonHelp") {
            if (!helpInfoTips.activeSelf) {
                helpInfoTips.SetActive(true);
				MaskWindow.UnlockUI();
			} else
				MaskWindow.UnlockUI();
        } else if (gameObj.name == "buttonCloseHelp") {
            helpInfoTips.gameObject.SetActive(false);
            MaskWindow.UnlockUI();
        }
	}
}
