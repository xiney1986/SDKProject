using UnityEngine;
using System.Collections;

/// <summary>
/// 工会战士气窗口
/// </summary>
public class GuildFightCourageWindow : WindowBase {

    /** 祈福 */
    public GuildBuffSample inspireSample;
    public UILabel inspireXiaohao;
    public UILabel inspireXiaoguo;
    public UILabel inspireJiangli;
    public ButtonBase buttonWish;
    /** 鼓舞 */
    public GuildBuffSample wishSample;
    public UILabel wishXiaohao;
    public UILabel wishXiaoguo;
    public UILabel wishJiangli;
    public ButtonBase buttonInspire;
    /** 领地数据 */
    public GuildArea data;
	protected override void begin ()
	{
		base.begin ();
        MaskWindow.UnlockUI();
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
        if (gameObj.name == "ButtonClose")
        {
            finishWindow();
        }
        else if (gameObj.name == "ButtonInspire") {
            inspire();
        } else if (gameObj.name == "ButtonWish") {
            wish();
        }
	}
    /// <summary>
    /// 鼓舞
    /// </summary>
    private void inspire()
    {
        /** 不在时间范围内 */
        if (!GuildFightSampleManager.Instance().isActivityBuffTime())
        {
            UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("GuildArea_35"));
            MaskWindow.UnlockUI();
            return;
        }
        /** 行动力不足 */
        if (UserManager.Instance.self.guildFightPower < inspireSample.getExpends())
        {
            UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("GuildArea_06"));
            MaskWindow.UnlockUI();
            return;
        }
        string des = inspireSample.getUseBuffDes(1);
        MessageWindow.ShowConfirm(des, (msg) =>
        {
            if (msg.msgEvent == msg_event.dialogOK)
            {
                GuildActiveAreaBuffFport port = FPortManager.Instance.getFPort("GuildActiveAreaBuffFport") as GuildActiveAreaBuffFport;
                port.access(2, inspireSample.getExpends(), inspireCallBack);
            }
        });
    }
    private void inspireCallBack(int expend)
    {
        UiManager.Instance.openDialogWindow<MessageLineWindow>((win) =>
        {
            win.dialogCloseUnlockUI = false;
            foreach (string s in inspireSample.getRewardDes())
            {
                win.Initialize(s, false);
            }
        });
        UserManager.Instance.self.guildFightPower -= expend;
        data.inspireNum++;
        initializeInfo(this.data);
        (fatherWindow as GuildFightMainWindow).getFightInfo();
        MaskWindow.UnlockUI();

    }
    /// <summary>
    /// 祝福
    /// </summary>
    private void wish()
    {
        /** 不在时间范围内 */
        if (!GuildFightSampleManager.Instance().isActivityBuffTime())
        {
            UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("GuildArea_35"));
            MaskWindow.UnlockUI();
            return;
        }
        /** 行动力不足 */
        if (UserManager.Instance.self.guildFightPower < wishSample.getExpends())
        {
            UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("GuildArea_06"));
            MaskWindow.UnlockUI();
            return;
        }
        string des = wishSample.getUseBuffDes(1);
        MessageWindow.ShowConfirm(des, (msg) =>
        {
            if (msg.msgEvent == msg_event.dialogOK)
            {
                GuildActiveAreaBuffFport port = FPortManager.Instance.getFPort("GuildActiveAreaBuffFport") as GuildActiveAreaBuffFport;
                port.access(1, wishSample.getExpends(), wishCallBack);
            }
        });
    }
    private void wishCallBack(int expend)
    {
        UiManager.Instance.openDialogWindow<MessageLineWindow>((win) =>
        {
            win.dialogCloseUnlockUI = false;
            foreach (string s in wishSample.getRewardDes())
            {
                win.Initialize(s, false);
            }
        });
        UserManager.Instance.self.guildFightPower -= expend;
        data.wishNum++;
        initializeInfo(this.data);
        (fatherWindow as GuildFightMainWindow).getFightInfo();
        MaskWindow.UnlockUI(true);
    }
    public void initializeInfo(GuildArea _data)
    {
        this.data = _data;
        if (this.data == null)
        {
            buttonWish.disableButton(true);
            buttonWish.textLabel.effectColor = new Color(0.22f, 0.22f, 0.22f);
            buttonInspire.disableButton(true);
            buttonInspire.textLabel.effectColor = new Color(0.22f, 0.22f, 0.22f);
        }
        else
        {
            buttonWish.disableButton(false);
            buttonInspire.disableButton(false);
        }
        inspireSample = GuildFightSampleManager.Instance().getSampleBySid<GuildBuffSample>(GuildFightSampleManager.INSPIRE_SID);
        wishSample = GuildFightSampleManager.Instance().getSampleBySid<GuildBuffSample>(GuildFightSampleManager.WISH_SID);
        inspireXiaohao.text = LanguageConfigManager.Instance.getLanguage("GuildArea_97", inspireSample.getExpends().ToString());
        inspireJiangli.text = inspireSample.getRewardDesString();
        inspireXiaoguo.text = LanguageConfigManager.Instance.getLanguage("GuildArea_99") + "+" + ((data == null ? 0 : data.inspireNum) * inspireSample.getEffect()[0]) + "%" + "[3A9663]+" + inspireSample.getEffect()[0] + "%[-]";
        wishXiaohao.text = LanguageConfigManager.Instance.getLanguage("GuildArea_97", wishSample.getExpends().ToString());
        wishJiangli.text = inspireSample.getRewardDesString();
        wishXiaoguo.text = LanguageConfigManager.Instance.getLanguage("GuildArea_98") + "+" + ((data == null ? 0 : data.wishNum) * wishSample.getEffect()[0]) + "%" + "[3A9663]+" + wishSample.getEffect()[0] + "%[-]";
    }
}
