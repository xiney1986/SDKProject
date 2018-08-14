using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 天梯奖励Item
/// </summary>
public class ArenaIntegralAwardItem : MonoBase
{
	public GameObject prefab_GoodsView;
	public GameObject root_prize;
    public GameObject bottons;
	public UILabel description;
    public ButtonBase receiveButton;
    public ButtonBase buttonMore;//多倍领取按钮
    public ButtonBase reciveCommonButton;
    private ArenaAwardSample data;
    private WindowBase fwin;
    private GoodsView[] awardButtons;
    private int num = 0;
	ArenaIntegralAwardContent content;

	/// <summary>
	/// 更新奖励
	/// </summary>
	public void initialize(ArenaAwardSample _data, WindowBase win,ArenaIntegralAwardContent content)
	{
        bottons.SetActive(false);
        fwin = win;
        data = _data;
		this.content = content;
	    if (data.getType == ShenGeManager.DOUBLEGET) {
            buttonMore.textLabel.text = LanguageConfigManager.Instance.getLanguage("NvShenShenGe_023");
            bottons.SetActive(true);
            receiveButton.gameObject.SetActive(false);
            reciveCommonButton.fatherWindow = win;
            buttonMore.fatherWindow = win;
	        reciveCommonButton.onClickEvent = onReceiveButtonClick;
	        buttonMore.onClickEvent = onMoreReceiveButtonClick;
	    } else if (data.getType == ShenGeManager.THREEGET)
        {
            buttonMore.textLabel.text = LanguageConfigManager.Instance.getLanguage("NvShenShenGe_024");
            bottons.SetActive(true);
            receiveButton.gameObject.SetActive(false);
            reciveCommonButton.fatherWindow = win;
            buttonMore.fatherWindow = win;
            reciveCommonButton.onClickEvent = onReceiveButtonClick;
            buttonMore.onClickEvent = onMoreReceiveButtonClick;
        }
	    receiveButton.fatherWindow = win;
        receiveButton.disableButton(!(ArenaAwardManager.Instance.awardCanReceive(data) && ArenaAwardManager.Instance.getArenaAwardInfo(data) != null && !ArenaAwardManager.Instance.getArenaAwardInfo(data).received));
        reciveCommonButton.disableButton(!(ArenaAwardManager.Instance.awardCanReceive(data) && ArenaAwardManager.Instance.getArenaAwardInfo(data) != null && !ArenaAwardManager.Instance.getArenaAwardInfo(data).received));
        buttonMore.disableButton(!(ArenaAwardManager.Instance.awardCanReceive(data) && ArenaAwardManager.Instance.getArenaAwardInfo(data) != null && !ArenaAwardManager.Instance.getArenaAwardInfo(data).received));
        if(ArenaAwardManager.Instance.getArenaAwardInfo(data)!=null&&ArenaAwardManager.Instance.getArenaAwardInfo(data).received){
            receiveButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("recharge02");
            reciveCommonButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("recharge02");
            buttonMore.textLabel.text = LanguageConfigManager.Instance.getLanguage("recharge02");
        } else {
            receiveButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("s0309");
            reciveCommonButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("s0309");
        }
        receiveButton.onClickEvent = onReceiveButtonClick;
        description.text = LanguageConfigManager.Instance.getLanguage("GuildLuckyNvShen_18") + data.condition;
        description.text += (ArenaAwardManager.Instance.getArenaAwardInfo(data) == null || !ArenaAwardManager.Instance.getArenaAwardInfo(data).received) ? "(1/1)" : "(0/1)";

		PrizeSample[] prizes = data.prizes;
		UIUtils.M_removeAllChildren (root_prize);
        awardButtons = new GoodsView[prizes.Length];
          for (int i = 0; i < awardButtons.Length; i++)
          {
                awardButtons[i] = NGUITools.AddChild(root_prize, prefab_GoodsView).GetComponent<GoodsView>();
                awardButtons[i].transform.localPosition = new Vector3(i * 96f, 0, 0);
                awardButtons[i].transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                awardButtons[i].fatherWindow = fwin;
                awardButtons[i].init(prizes[i]);
          }
	}
    /// <summary>
    /// 奖励领取回调
    /// </summary>
    /// <param name="result"></param>
    private void OnReceiveBack(bool result)
    {
        receiveButton.disableButton(result);
        reciveCommonButton.disableButton(result);
        buttonMore.disableButton(result);
        PrizeSample[] prizess = data.prizes;
        for (int i = 0; i < prizess.Length;i++ ) {
            if (prizess[i].type == PrizeType.PRIZE_MERIT) {
                (fwin as ArenaIntegralAwardWindow).inccc += StringKit.toInt(prizess[i].num) * num;
                //UserManager.Instance.self.merit += StringKit.toInt(prizess[i].num);
            } 
        }
        receiveButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("recharge02");
        reciveCommonButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("recharge02");
        buttonMore.textLabel.text = LanguageConfigManager.Instance.getLanguage("recharge02");
        int count = ArenaAwardManager.Instance.integralAwardInfos.IndexOf(ArenaAwardManager.Instance.getArenaAwardInfo(data));
        ArenaAwardManager.Instance.integralAwardInfos[count].received = result;
        description.text = LanguageConfigManager.Instance.getLanguage("GuildLuckyNvShen_18") + data.condition + (ArenaAwardManager.Instance.getArenaAwardInfo(data).received ?"(0/1)":"(1/1)");
        if (result)
        {
            TextTipWindow.Show(LanguageConfigManager.Instance.getLanguage("Arena30"));
        }
        else
        {
            MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage(result ? "Arena30" : "Arena31"));
        }
		content.init();
        MaskWindow.UnlockUI();
    }
    /// <summary>
    /// 普通领取积分按钮点击
    /// </summary>
    private void onReceiveButtonClick(GameObject obj)
    {
        if (UserManager.Instance.self.getMerit() + data.meritAwardNum > CommandConfigManager.Instance.getLimitOfMerit())
        {
            UiManager.Instance.openDialogWindow<MessageLineWindow>((win) =>
            {
                win.Initialize(LanguageConfigManager.Instance.getLanguage("NvShenShenGe_025"));
            });
            return;
        }
        num = 1;
        FPortManager.Instance.getFPort<ArenaReceiveAwardIntegralFPort>().access(OnReceiveBack, data.sid,1);
    }
    /// <summary>
    /// 多倍积分领取
    /// </summary>
    /// <param name="obj"></param>
    private void onMoreReceiveButtonClick(GameObject obj) {
        if (UserManager.Instance.self.getMerit() + data.meritAwardNum * (data.getType + 1) > CommandConfigManager.Instance.getLimitOfMerit()) {
            UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
                win.Initialize(LanguageConfigManager.Instance.getLanguage("NvShenShenGe_025"));
            });
            return;
        }
        UiManager.Instance.openDialogWindow<MessageWindow>((win) =>
        {
            string str = "";
            string cannotGetDesc = "";
            bool canGetAward = true;
            if (data.costType == 1)
            {
                if (data.needMoney > UserManager.Instance.self.getMoney())
                    canGetAward = false;
                str = "NvShenShenGe_027";
                cannotGetDesc = "NvShenShenGe_036";
            } else if (data.costType == 2)
            {
                if (data.needMoney > UserManager.Instance.self.getRMB())
                    canGetAward = false;
                str = "NvShenShenGe_026";
                cannotGetDesc = "NvShenShenGe_037";
            }
            string strs = "";
            if (data.getType == 1)
                strs = "NvShenShenGe_023";
            else if (data.getType == 2)
                strs = "NvShenShenGe_024";
            num = data.getType + 1;
            win.initWindow(2, LanguageConfigManager.Instance.getLanguage("s0094"), LanguageConfigManager.Instance.getLanguage("s0093"), LanguageConfigManager.Instance.getLanguage(str, data.needMoney.ToString(), LanguageConfigManager.Instance.getLanguage(strs)),
                (msg) =>
                {
                    if (msg.msgEvent == msg_event.dialogOK) {
                        if (canGetAward)
                            FPortManager.Instance.getFPort<ArenaReceiveAwardIntegralFPort>()
                                .access(OnReceiveBack, data.sid, (data.getType + 1));
                        else
                        {
                            UiManager.Instance.openDialogWindow<MessageLineWindow>((wins) =>
                            {
                                wins.Initialize(LanguageConfigManager.Instance.getLanguage(cannotGetDesc));
                            });
                        }
                    }
                    else
                    {
                        MaskWindow.UnlockUI();
                    }
                });
        });
    }
}

