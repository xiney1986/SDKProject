using UnityEngine;
using System.Collections;

public class GrowupRebateItem : MonoBehaviour
{
    public UILabel level;
    public UILabel rebate;
    public UILabel invest500;
    public UILabel invest2000;
    /** 父窗口 */
    private WindowBase win;
    /// <summary>
    /// 更新奖励条目
    /// </summary>
    /// <param name="tl">排行奖励</param>
    /// <param name="win">父窗口</param>
    public void updateAwardItem(GrowupAwardSample tl, WindowBase win)
    {
        this.win = win;
        //		level.text =   LanguageConfigManager.Instance.getLanguage("gr_003",tl.needLevel,tl.backPercent.ToString());
        level.text = tl.needLevel;
        rebate.text = tl.backPercent + "%";
        invest500.text = LanguageConfigManager.Instance.getLanguage("s0309") + (tl.backPercent * 15000 / 100).ToString();
        invest2000.text = LanguageConfigManager.Instance.getLanguage("s0309") + (tl.backPercent * 50000 / 100).ToString();
    }
}
