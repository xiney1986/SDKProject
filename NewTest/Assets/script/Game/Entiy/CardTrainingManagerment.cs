using UnityEngine;
using System.Collections;

public class CardTrainingManagerment 
{
    public static CardTrainingManagerment Instance
    {
        get { return SingleManager.Instance.getObj("CardTrainingManagerment") as CardTrainingManagerment; }
    }
    /// <summary>
    /// 目前3个栏位的卡牌uid, 目前可用作判断是否已经装备上
    /// </summary>
    public string[] CardsUid = new string[] { "", "", "" };

    private int[] mTrainingTime = new int[] { -1, -1, -1 };
    
    

    public CardTrainingManagerment()
    {
        initData();

        Timer timer = TimerManager.Instance.getTimer(1000, 0);
        timer.addOnTimer(timerHandler);
        timer.start();
    }

    private void initData()
    {
        InitCardTraining fport = FPortManager.Instance.getFPort("InitCardTraining") as InitCardTraining;
        fport.access(onReceiveInit);
    }


    private void onReceiveInit(int cardIndex, int time)
    {
        UpdateTime(cardIndex, time);
    }


    private void timerHandler()
    {
        for (int i = 0; i < mTrainingTime.Length; i++)
        {
            if (mTrainingTime[i] != -1 && GetRemainingTime(i) == 0)
            {
                mTrainingTime[i] = -1;
                cardTimeOver(i);
            }
        }
    }

    private void cardTimeOver(int cardIndex)
    {
        UiManager.Instance.openDialogWindow<MessageLineWindow>((win) =>
        {
            win.Initialize(LanguageConfigManager.Instance.getLanguage("s0505"));
        });

    }


    /// <summary>
    /// 
    /// </summary>
    public void UpdateTime(int index, int time)
    {
        if (time - ServerTimeKit.getSecondTime() <= 0) return;
        mTrainingTime[index] = time;
        if (UiManager.Instance.mainWindow != null)
            UiManager.Instance.mainWindow.UpdateCardTrainingTips();
    }

    /// <summary>
    /// 
    /// </summary>
    public int GetRemainingTime(int index)
    {
        int remainingTime = mTrainingTime[index] - ServerTimeKit.getSecondTime();
        return Mathf.Max(0, remainingTime);
    }

    /// <summary>
    /// 
    /// </summary>
    public int getTime(int index)
    {
        return mTrainingTime[index];
    }


    /// <summary>
    /// 得到目前可以使用的栏位
    /// </summary>
    /// <returns></returns>
    public int getCanUseLocation()
    {
        CardTrainingSample sample = CardTrainingSampleManager.Instance.getDataBySid(0);
        int[] enabledCondition = new int[] { StringKit.toInt(sample.EnabledCondition[0]), StringKit.toInt(sample.EnabledCondition[1]), StringKit.toInt(sample.EnabledCondition[2]) };
        int num = 0;
        if (UserManager.Instance.self.getUserLevel() < enabledCondition[1]) return num;
        if (UserManager.Instance.self.getUserLevel() >= enabledCondition[0] && GetRemainingTime(0) <= 0) num++;
        if (UserManager.Instance.self.getUserLevel() >= enabledCondition[1] && GetRemainingTime(1) <= 0) num++;
        if (UserManager.Instance.self.getVipLevel() >= enabledCondition[2] && GetRemainingTime(2) <= 0) num++;
        return num;
    }







}
