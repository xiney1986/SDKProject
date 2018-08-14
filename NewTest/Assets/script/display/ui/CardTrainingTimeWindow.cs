using UnityEngine;
using System.Collections.Generic;

public class CardTrainingTimeWindow : WindowBase
{
	public ButtonBase[] UI_TimeBtn;
	public UILabel UI_TipsExpLabel;
	public UILabel UI_TipsLvLabel;
	public GameObject UI_RmbSelectedMark;
	public GameObject UI_RmbSelecte;
	public UILabel UI_RmbLabel;
	private int mSelecteTime = 0;
	private string mBtnNormalSprite;
	private string mBtnHoverSprite;
	private Card mCard;
	private int mLocationIndex;
	private bool mExpOverflow;
	private bool mIsUpgrade;
	private long mAwardExp;
	private long mMaxAwardExp;

	protected override void begin ()
	{
		base.begin ();
		//UIEventListener.Get(UI_RmbSelecte).onClick = onClickSelectRmb;

		mBtnHoverSprite = UI_TimeBtn [0].GetComponent<UIButton> ().hoverSprite;
		mBtnNormalSprite = UI_TimeBtn [0].GetComponent<UIButton> ().normalSprite;
		updateTimeState ();
		MaskWindow.UnlockUI ();
	}
	/// <summary>
	/// 选择使用元宝训练
	/// </summary>
	/// <param name="go"></param>
	private void onClickSelectRmb ()
	{
		UI_RmbSelectedMark.SetActive (!UI_RmbSelectedMark.activeSelf);
		updateExpLv ();
	}

	/// <summary>
	/// 时间类型按钮状态更新
	/// </summary>
	private void updateTimeState ()
	{
		for (int i = 0; i < UI_TimeBtn.Length; i++) {
			UI_TimeBtn [i].setNormalSprite (UI_TimeBtn [i].name == "Time_" + mSelecteTime ? mBtnHoverSprite : mBtnNormalSprite);
		}
		updateExpLv ();
	}

	/// <summary>
	/// 更新奖励的经验和等级,双倍消耗
	/// </summary>
	private void updateExpLv ()
	{
		if (mCard == null)
			return;

		CardTrainingSample sample = CardTrainingSampleManager.Instance.getDataBySid (0);

		int vipLv = UserManager.Instance.self.vipLevel;
		int lv = mCard.getLevel ();
		mAwardExp = StringKit.toInt (sample.AwardExp [UserManager.Instance.self.getUserLevel () - 1]);
		int trainingTime = StringKit.toInt (sample.TrainingTime [mSelecteTime]);
		mAwardExp *= trainingTime;
		if (UI_RmbSelectedMark.activeSelf)
			mAwardExp *= 2;
		long exp = mCard.getEXP ();
        float vipAwardExp = vipLv > 0 ? VipManagerment.Instance.getVipbyLevel(vipLv).privilege.expAdd * 0.0001f + 1 : 1;
        mAwardExp = (long)(vipAwardExp * mAwardExp);

		//最大能获取的经验值
		long AwardMaxExp = mAwardExp;
		mExpOverflow = false;

		long tmpExp = mCard.checkExp (mAwardExp);
		if (tmpExp != -1) {
			AwardMaxExp = tmpExp;
			mExpOverflow = true;
		}
        
//        mExpOverflow = AwardMaxExp < awardExp + exp;
		mMaxAwardExp = AwardMaxExp;
		mIsUpgrade = mMaxAwardExp >= mCard.getNeedExp ();

		UI_TipsExpLabel.text = "[97ed8e]+" + mAwardExp;
        //if (vipLv > 0) {
        //    if (!mExpOverflow)
        //        UI_TipsExpLabel.text = "[97ed8e]+" + ((long)(mAwardExp / vipAwardExp));
        //    UI_TipsExpLabel.text += " X" + (vipAwardExp) + "(VIP" + vipLv.ToString() + ")";
        //}

		int nextLv = 0;

		if (tmpExp != -1) {
			nextLv = EXPSampleManager.Instance.getLevel (mCard.getEXPSid (), tmpExp);
		} else {
			nextLv = EXPSampleManager.Instance.getLevel (mCard.getEXPSid (), mMaxAwardExp + exp);
		}
		UI_TipsLvLabel.text = string.Format ("LV{0}             LV{1}", lv, nextLv);

		if (mExpOverflow) {
            UI_TipsExpLabel.text = UI_TipsExpLabel.text + " [FF0000]" + Language("cardtraining_expOverflow", mAwardExp-AwardMaxExp+exp );
			UI_TipsLvLabel.text = UI_TipsLvLabel.text ;
		}

		UI_RmbLabel.text = string.Format (LanguageConfigManager.Instance.getLanguage ("CardTraining_07"), sample.TimeRmb [mSelecteTime]);

	}

	/// <summary>
	/// 确定训练各种参数发送到服务端
	/// </summary>
	private void SureTraining ()
	{
		SubmitCardTraining fport = FPortManager.Instance.getFPort ("SubmitCardTraining") as SubmitCardTraining;
		fport.access (onReceiveInit, UI_RmbSelectedMark.activeSelf ? 1 : 0, mCard.uid, mLocationIndex, mSelecteTime);
	}
	///
	private void onReceiveInit (int time)
	{
		finishWindow ();
		CardTrainingWindow window = UiManager.Instance.getWindow<CardTrainingWindow> ();
		//window.getCardItem(mLocationIndex).SetCD(time);
		StartCoroutine (Utils.DelayRun (() => {
			window.getCardItem(mLocationIndex).showEffect(mIsUpgrade, mMaxAwardExp);
		}, 0.3f));
        //window.getCardItem(mLocationIndex).showEffect(mIsUpgrade, mMaxAwardExp);
		CardTrainingManagerment.Instance.UpdateTime (mLocationIndex, time);
	}
    /// <summary>
    /// 用于断线重连
    /// </summary>
    /// <param name="cardIndex"></param>
    /// <param name="time"></param>
    public void onReceiveInit(int cardIndex,int time) {
        finishWindow();
        CardTrainingWindow window = UiManager.Instance.getWindow<CardTrainingWindow>();
        CardTrainingManagerment.Instance.UpdateTime(cardIndex, time);
        if (CardTrainingManagerment.Instance.GetRemainingTime(mLocationIndex) > 0) {
            StartCoroutine(Utils.DelayRun(() => {
                window.getCardItem(mLocationIndex).showEffect(mIsUpgrade, mMaxAwardExp);
            }, 0.3f));
        }
    }

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name.IndexOf ("Time_") != -1) {
			int selecteTime = int.Parse (gameObj.name.Replace ("Time_", ""));
            
            int timeLimitLv =  StringKit.toInt(CardTrainingSampleManager.Instance.getDataBySid(0).TimeLimitLv[selecteTime]);
            if (UserManager.Instance.self.getUserLevel() < timeLimitLv)
            {
                UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                    win.Initialize(LanguageConfigManager.Instance.getLanguage("CardTraining_12", timeLimitLv.ToString()));
                });
                return;
            }
            mSelecteTime = selecteTime;
            updateTimeState();
        }
        

		switch (gameObj.name) {
		case "Cancel":
		case "Close":
			finishWindow ();
			break;
		case "Selecte_Rmb":
			onClickSelectRmb ();
			break;
		case "Start":
			if (mExpOverflow) {
				MessageWindow.ShowConfirm (Language ("cardtraining_expOverflowTips", (mAwardExp + mCard.getEXP ()) - mMaxAwardExp), ( msg ) => {
					if (msg.buttonID == MessageHandle.BUTTON_RIGHT){
						StartCoroutine (Utils.DelayRun (() => {
						}, 2.3f));
						SureTraining ();
					}
				});
			} else{
				StartCoroutine (Utils.DelayRun (() => {
				}, 0.3f));
				SureTraining ();
			}
			break;
		}
        
	}

	/// <summary>
	/// 设置数据
	/// </summary>
	/// <param name="card"></param>
	/// <param name="locationIndex"></param>
	public void SetData (Card card, int locationIndex)
	{
		mCard = card;
		mLocationIndex = locationIndex;
		updateExpLv ();
	}



}

