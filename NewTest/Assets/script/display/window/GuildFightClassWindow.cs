using UnityEngine;
using System.Collections;

/// <summary>
/// 工会战评级窗口
/// </summary>
public class GuildFightClassWindow : WindowBase {

    /** 行动力 */
    public barCtrl scoreBarCtrl;
    /** 行动力标签 */
    public UILabel scoreBarLabel;
    //* 评级标签**/
    public UILabel classLabel;
    public UILabel nowLabel;
    public UILabel nextLabel;
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
	}
    public void Intialize()
    {
        updateBar();
        int score = getGuildScore();
        if (score > 0)
            classLabel.text = LanguageConfigManager.Instance.getLanguage("GuildArea_89") + GuildFightSampleManager.Instance().getJudgeString(score) + LanguageConfigManager.Instance.getLanguage("GuildArea_96");
        else
            classLabel.text = LanguageConfigManager.Instance.getLanguage("GuildArea_88");
    }
    /// <summary>
    /// 更新的得分bar
    /// </summary>
    public void updateBar()
    {
        //int max = GuildFightSampleManager.Instance().getMaxScore();
        int my = getGuildScore();
        int max = GuildFightSampleManager.Instance().getNextScore(my);
        scoreBarCtrl.updateValue(my, max);
        scoreBarLabel.text = my + "/" + max;
        nowLabel.text = GuildFightSampleManager.Instance().getJudgeString(my) + LanguageConfigManager.Instance.getLanguage("GuildArea_96");
        nextLabel.text = GuildFightSampleManager.Instance().getJudgeString(max) + LanguageConfigManager.Instance.getLanguage("GuildArea_96");
    }
    /// <summary>
    /// 获得公会得分
    /// </summary>
    /// <returns></returns>
    private int getGuildScore()
    {
        GuildGetInfoFPort fport = FPortManager.Instance.getFPort("GuildGetInfoFPort") as GuildGetInfoFPort;
        fport.access(null);
        //foreach (GuildAreaPreInfo info in GuildManagerment.Instance.guildFightInfo.areas)
        //{
        //    if (info.uid == UserManager.Instance.self.guildId)
        //        score = info.judgeScore;
        //}
        return GuildManagerment.Instance.selfScore;
    }
}
