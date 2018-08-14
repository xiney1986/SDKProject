using UnityEngine;
using System.Collections.Generic;



public class CardTrainingItemView : RoleView
{

    public ButtonBase UI_Btn;
    public UILabel UI_Time;
    public UILabel UI_Condition;
    public GameObject UI_SelecteCard;
    public GameObject UI_EmptyCard;
    public GameObject UI_EmptyEnabled;
    public UILabel UI_ExpValue;
    public Vector2 UI_ExpValueTweenY;

    //private float mCd;
    private bool mIsEnabled;
    private int mLocationIndex;
    private bool mIsTimeOver;

    private int[] mEnabledCondition;


    public override void OnAwake()
    {
        base.OnAwake();

        CardTrainingSample sample = CardTrainingSampleManager.Instance.getDataBySid(0);
        mEnabledCondition = new int[] { StringKit.toInt(sample.EnabledCondition[0]), StringKit.toInt(sample.EnabledCondition[1]), StringKit.toInt(sample.EnabledCondition[2]) };

        UI_Btn.onClickEvent = onClickTrainingBtn;

		mLocationIndex = StringKit.toInt (name) - 1;
        mIsEnabled = (mLocationIndex == 0 && UserManager.Instance.self.getUserLevel() >= mEnabledCondition[0]) ||
            (mLocationIndex == 1 && UserManager.Instance.self.getUserLevel() >= mEnabledCondition[1]) ||
            (mLocationIndex == 2 && UserManager.Instance.self.getVipLevel() >= mEnabledCondition[2]);
		updateEnabled ();
    }



    public override void begin()
    {
        base.begin();
        SetData(null);
        timeOver();
    }


    public override void DoClickEvent()
    {
        base.DoClickEvent();
        onClickSelecteCard();
    }

    /// <summary>
    /// 点击选择卡牌
    /// </summary>
    /// <param name="go"></param>
    private void onClickSelecteCard()
    {
		if (CardTrainingManagerment.Instance.GetRemainingTime (mLocationIndex) > 0) return;
		string str = LanguageConfigManager.Instance.getLanguage ("CardTraining_01");
        if (mLocationIndex == 0 && UserManager.Instance.self.getUserLevel() < mEnabledCondition[0])
			MessageWindow.ShowAlert(str + LanguageConfigManager.Instance.getLanguage("CardTraining_09",mEnabledCondition[0].ToString ()));
        else if (mLocationIndex == 1 && UserManager.Instance.self.getUserLevel() < mEnabledCondition[1])
			MessageWindow.ShowAlert(str + LanguageConfigManager.Instance.getLanguage("CardTraining_09",mEnabledCondition[1].ToString ()));
        else if (mLocationIndex == 2 && UserManager.Instance.self.getVipLevel() < mEnabledCondition[2])
			MessageWindow.ShowAlert(str + LanguageConfigManager.Instance.getLanguage("CardTraining_11",mEnabledCondition[2].ToString ()));
        else
            ((CardTrainingWindow)fatherWindow).showChooseCard(mLocationIndex);
    }

	private void updateEnabled () {
		if (CardTrainingManagerment.Instance.GetRemainingTime (mLocationIndex) > 0) {
			UI_Condition.text = LanguageConfigManager.Instance.getLanguage ("s0584", mEnabledCondition[mLocationIndex].ToString ());
			UI_Condition.gameObject.SetActive (true);
			UI_EmptyEnabled.gameObject.SetActive (false);
		}
		else {
			if (mLocationIndex == 0 || mLocationIndex == 1) {
				UI_Condition.text = LanguageConfigManager.Instance.getLanguage ("CardTraining_09", mEnabledCondition[mLocationIndex].ToString ());
			}
			else {
				UI_Condition.text = LanguageConfigManager.Instance.getLanguage ("CardTraining_11", mEnabledCondition[mLocationIndex].ToString ());
			}
			UI_Condition.gameObject.SetActive (!mIsEnabled);
			UI_EmptyEnabled.gameObject.SetActive (mIsEnabled);
		}
	}

    /// <summary>
    /// 点击训练按钮
    /// </summary>
    /// <param name="go"></param>
    private void onClickTrainingBtn(GameObject go)
    {
        UiManager.Instance.openDialogWindow<CardTrainingTimeWindow>((win) => {
            win.SetData(card, mLocationIndex);
        });
    }

    /// <summary>
    /// 训练时间到
    /// </summary>
    private void timeOver()
    {
        if (!UI_Btn.isDisable())
            UI_Btn.disableButton(false);
        if (mIsEnabled)
        {
            UI_Time.text = Colors.GREEN + LanguageConfigManager.Instance.getLanguage("s0501") + "[-]";
        }
        else
        {
            UI_Time.text = Colors.RED + LanguageConfigManager.Instance.getLanguage("s0502") + "[-]";
        }

		updateEnabled ();
    }

    /// <summary>
    /// 
    /// </summary>
    public override void DoUpdate()
    {
        base.DoUpdate();
        int remainingTime = CardTrainingManagerment.Instance.GetRemainingTime(mLocationIndex);
        if (remainingTime <= 0 && !mIsTimeOver)
        {
            mIsTimeOver = true;
            timeOver();
        }
        if (remainingTime > 0)
            UI_Time.text = Colors.RED + LanguageConfigManager.Instance.getLanguage("s0473") + TimeKit.timeTransform(remainingTime * 1000) + "[-]";
    }


    /// <summary>
    /// 显示特效,+经验, 如果升级有升级特效
    /// </summary>
    public void showEffect(bool isUpgrade, long awardExp)
    {
		StartCoroutine (Utils.DelayRun (() => {
			UI_ExpValue.text = "+" + awardExp.ToString();
			UI_ExpValue.transform.localPosition = new Vector3(0, UI_ExpValueTweenY.x, 0);
			iTween.MoveTo(UI_ExpValue.gameObject, iTween.Hash("position", new Vector3(0, UI_ExpValueTweenY.y, 0), "time", 1.1f, "islocal", true, "oncomplete", "showEffectComplete", "oncompleteparams", isUpgrade, "oncompletetarget", gameObject));
			
			UI_Btn.disableButton(true);
			updateInfo();
		}, 0.8f));
//        UI_ExpValue.text = "+" + awardExp.ToString();
//        UI_ExpValue.transform.localPosition = new Vector3(0, UI_ExpValueTweenY.x, 0);
//        iTween.MoveTo(UI_ExpValue.gameObject, iTween.Hash("position", new Vector3(0, UI_ExpValueTweenY.y, 0), "time", 1.5f, "islocal", true, "oncomplete", "showEffectComplete", "oncompleteparams", isUpgrade, "oncompletetarget", gameObject));
//
//        UI_Btn.disableButton(true);
//        updateInfo();
    }

    private void showEffectComplete(bool isUpgrade)
    {
        UI_ExpValue.text = "";
        StartCoroutine(Utils.DelayRun(() =>
        {
            SetData(null);
        }, 2.5f));

        if (isUpgrade)
        {
            EffectManager.Instance.CreateEffectCtrlByCache(transform, "Effect/UiEffect/levelupEffect", (obj, ctrl) => { });
        }
    }


    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="card"></param>
    public void SetData(Card card)
    {
        this.card = card;
        icon.mainTexture = null;
        if (this.gameObject.transform.FindChild("starContent(Clone)") != null) {
            DestroyImmediate(this.gameObject.transform.FindChild("starContent(Clone)").gameObject);
        }
        evoLevel.gameObject.SetActive(false);
        updateInfo();

        UI_Btn.disableButton(!mIsEnabled || card == null || CardTrainingManagerment.Instance.GetRemainingTime(mLocationIndex) > 0);
		level.gameObject.SetActive(card != null);
        UI_EmptyCard.SetActive(card == null);
        qualityBg.gameObject.SetActive(card != null);
        jobTextSprite.gameObject.SetActive(card != null);
		jobSprite.gameObject.SetActive(card!=null);
        CardTrainingManagerment.Instance.CardsUid[mLocationIndex] = card == null ? "" : card.uid;
		updateEnabled ();
    }


}


