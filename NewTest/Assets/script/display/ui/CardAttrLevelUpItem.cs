using UnityEngine;
using System.Collections;

/// <summary>
/// 类说明：卡片属性升级条目
/// </summary>
public class CardAttrLevelUpItem : MonoBehaviour
{

    /* fields */
    /** 属性经验条 */
    public ExpbarCtrl expBar;
    /** 属性等级标签 */
    public UILabel levelLabel;
    /** 附加属性名 */
    public UISprite textNameLabel;
    /** 附加属性值 */
    public UILabel textValueLabel;
	/** 等级提升特效 */
	public GameObject upLevelEffect;

    /* methods */
    /// <summary>
    /// 设置附加属性名
    /// </summary>
    /// <param name="textName"></param>
    public void setTextNameLabel(string textName)
    {
        textNameLabel.spriteName = textName;
    }
    /// <summary>
    /// 设置附加属性值
    /// </summary>
    /// <param name="textValue"></param>
    public void setTextValueLabel(string textValue)
    {
        textValueLabel.text = textValue;
    }
    /// <summary>
    /// 设置等级标签
    /// </summary>
    /// <param name="currentLevel">当前属性等级</param>
    /// <param name="maxLevel">最大属性等级</param>
    public void setLevelLabel(int currentLevel, int maxLevel)
    {
        levelLabel.text = "Lv." + currentLevel + "/" + maxLevel;
    }
	/// <summary>
	/// 更新属性
	/// </summary>
	/// <param name="index">次序</param>
	/// <param name="textName">附加属性文本</param>
	/// <param name="oldExp">老附加属性经验</param>
	/// <param name="attrExp">新附加属性经验</param>
	/// <param name="oldAttr">原始属性值</param>
	/// <param name="attr">附加属性值</param>
	public bool updateAttributes(int index,string textName,int oldExp,long newExp,int oldAttr, int attr)
	{
		if (newExp >= EXPSampleManager.Instance.getMaxExp(EXPSampleManager.SID_USER_ATTR_ADD_EXP)) {
			return false;
		}

		// 获得经验对于的卡片附加等级
		int currentGrade=Card.getAttrAddGrade (newExp);
		// 获得经验对于的卡片附加等级
		int oldGrade=Card.getAttrAddGrade (oldExp);
		// 增加等级
		int addGrade=currentGrade-oldGrade;
		// 最大等级
		int maxGrade = EXPSampleManager.Instance.getMaxLevel(EXPSampleManager.SID_USER_ATTR_ADD_EXP) - 1;
		// 老等级段经验下线
		long oldExpNow = EXPSampleManager.Instance.getEXPDown(EXPSampleManager.SID_USER_ATTR_ADD_EXP,oldGrade+1);
		// 老等级段经验上线
		long oldExpMax = EXPSampleManager.Instance.getEXPUp(EXPSampleManager.SID_USER_ATTR_ADD_EXP, oldGrade+1);
		
		// 当前等级段经验下线
		long currentExpNow = EXPSampleManager.Instance.getEXPDown(EXPSampleManager.SID_USER_ATTR_ADD_EXP,currentGrade+1);
		// 当前等级段经验上线
		long currentExpMax = EXPSampleManager.Instance.getEXPUp(EXPSampleManager.SID_USER_ATTR_ADD_EXP,currentGrade+1);
	
		LevelupInfo info = new LevelupInfo ();
		info.oldLevel = oldGrade;
		info.newLevel = currentGrade;
		info.oldExp = oldExp;
		info.newExp = newExp;
		info.oldExpUp = oldExpMax;
		info.oldExpDown = oldExpNow;
		info.newExpUp = currentExpMax;
		info.newExpDown = currentExpNow;
		expBar.init (info);
		setLevelLabel(currentGrade, maxGrade);
		setTextNameLabel("attr_"+textName);
		if (attr > 0)
		{
			setTextValueLabel(oldAttr.ToString() + "[64ED6E]+" + attr.ToString());
		}
		else
		{
			setTextValueLabel(oldAttr.ToString());
		}
		bool isUpLevel = false;
		if (newExp >= EXPSampleManager.Instance.getMaxExp(EXPSampleManager.SID_USER_ATTR_ADD_EXP))
			gameObject.SetActive(false);
		else
		{
			gameObject.SetActive(true);
			// 等级提升特效
			if (addGrade > 0)
			{
				if(index < 3)
				{
					expBar.endCall = ShowUpEffect;
				}
				else
				{
					textValueLabel.gameObject.SetActive(true);
				}
				isUpLevel=true;
			}
		}
		return isUpLevel;
	}
	/// <summary>
	/// 显示升级特效
	/// </summary>
	public void ShowUpEffect()
	{
		StartCoroutine(Utils.DelayRun(() => {
			EffectManager.Instance.CreateEffectCtrlByCache(upLevelEffect.transform,"Effect/UiEffect/skillLevelUpWindow_numbereffect",(obj,barCtrl)=>{
				StartCoroutine(Utils.DelayRun(() => {
					textValueLabel.gameObject.SetActive(true);
					textValueLabel.transform.localPosition = new Vector3 (30, textValueLabel.transform.localPosition.y, textValueLabel.transform.localPosition.z);
					TweenPosition tp = TweenPosition.Begin (textValueLabel.gameObject, 0.3f, textValueLabel.transform.localPosition);
					tp.from = new Vector3 (-212, textValueLabel.transform.localPosition.y, textValueLabel.transform.localPosition.z);
				}, 1.5f));
			});
		}, 1f));
	}
}
