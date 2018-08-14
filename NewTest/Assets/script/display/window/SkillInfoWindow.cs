using UnityEngine;
using System.Collections;

public class SkillInfoWindow :WindowBase
{
//	public UITexture skillIcon;
	public UILabel nameLabel;
	public UILabel levelLabel;
	public UILabel ExpLabel;
	public UILabel descriptLabel;
	public UITexture icon;
	public barCtrl expbar;
	public GameObject buttonRepick;
	public GameObject buttonClose;
	Skill showSkill;
	CallBack repickButtonCallback;
	Card card;

	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI();
	}

	public void Initialize (Skill _skill, Card _card)
	{
		showSkill = _skill;
		card = _card;
	    //nameLabel.text = (showSkill.getSkillQuality () == 1 ? "[FFFFFF]" : QualityManagerment.getQualityColor (showSkill.getSkillQuality ())) + showSkill.getName ();
        nameLabel.text = showSkill.getName();
		ResourcesManager.Instance.LoadAssetBundleTexture (_skill.getIcon (), icon);
		ExpLabel.text = EXPSampleManager.Instance.getExpBarShow(showSkill.getEXPSid (),showSkill.getEXP ());

		descriptLabel.text = _skill.getDescribe ();
		if(_skill.getShowType() != 1 || _skill.getMaxLevel () == 0){
			expbar.gameObject.SetActive(false);
			levelLabel.text = "";
		}else{
			expbar.gameObject.SetActive(true);
			expbar.updateValue (showSkill.getEXP () - showSkill.getEXPDown (), showSkill.getEXPUp () - showSkill.getEXPDown ());
			levelLabel.text = "Lv." + showSkill.getLevel ();
		}
		//expbar.updateValue (showSkill.getEXP () - showSkill.getEXPDown (), showSkill.getEXPUp () - showSkill.getEXPDown ());
	}
	//共鸣之力用
	public void Initialize (string name, string  text)
	{
		showSkill = null;
		nameLabel.text = name;
		ExpLabel.text = "";
		levelLabel.text = "";
		descriptLabel.text = text;
		expbar.gameObject.SetActive (false);
	}
    /// <summary>
    /// 技能说明，坐骑使用
    /// </summary>
    public void Initialize(string name, string des, string iconPath) 
    {
        showSkill = null;
        nameLabel.text = name;
        ExpLabel.text = "";
        levelLabel.text = "";
        descriptLabel.text = des;
        expbar.gameObject.SetActive(false);
        ResourcesManager.Instance.LoadAssetBundleTexture(iconPath, icon);
    }
	//用于召唤兽技能显示,exp为总经验,type是经验类型
	public void Initialize (string name, string desc, long exp, int type, string level)
	{
		long expNow = EXPSampleManager.Instance.getNowEXPShow (type, exp);
		long expMax = EXPSampleManager.Instance.getMaxEXPShow (type, exp);
		
		showSkill = null;
		nameLabel.text = name;
		ExpLabel.text = EXPSampleManager.Instance.getExpBarShow (type, exp);
		levelLabel.text = level;
		descriptLabel.text = desc;
		expbar.updateValue (expNow, expMax);
	}

	public void ShowRepick (CallBack callback)
	{
		repickButtonCallback = callback;
		buttonRepick.SetActive (true);
		buttonRepick.transform.localPosition = new Vector3 (-133, buttonRepick.transform.localPosition.y, 0);
		buttonClose.transform.localPosition = new Vector3 (133, buttonClose.transform.localPosition.y, 0);
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "buttonOff") {
			finishWindow ();
		} else if (gameObj.name == "buttonRepick" && repickButtonCallback != null) {
			repickButtonCallback ();
			finishWindow ();
		}
	}
	
}
