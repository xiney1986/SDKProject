using UnityEngine;
using System.Collections;

public class ButtonSkillLevelUp : ButtonBase
{
	/** 技能等级 */
	public UILabel skillLevel;
	/** 技能图标 */
	public UITexture skillIcon;
	/** 技能描述 */
	public UILabel skillDesc;
	/** 经验条 */
	public ExpbarCtrl barCtrl;
	/** 经验显示 */
	public UILabel expLabel;
	public const int TYPE_BLUE = 1;
	public const int TYPE_RED = 2;
	public const int TYPE_YELLOW = 3;
	public const int STATE_LEARNED = 1;
	public const int STATE_CANLEARN = 2;
	public const int STATE_NOOPEN = 3;
	public static Color EFFECTLINE_RED = new Color (1, 0.6f, 0f, 1f);
	public static Color EFFECTLINE_YELLOW = new Color (1, 0.9f, 0f, 1f);
	public static Color EFFECTLINE_BLUE = new Color (0.55f, 0.8f, 1f, 1f);
	public static Color EFFECTLINE_BROWN = new Color (0.44f, 0.4f, 0.1f, 1f);
	public Skill skillData;
	[HideInInspector]
	public int isShowUp = 10;
	int levelNow;
	private bool isplay = false;
	private Card card;

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		if (UserManager.Instance.self.getUserLevel () < 999) {
			return;
		}
		GuideManager.Instance.doGuide ();
		if (skillData == null)
			return;
		UiManager.Instance.openDialogWindow<SkillInfoWindow> ((win) => {
			win.Initialize (skillData, card);
		});
	}
    
	public void levelUpEffect ()
	{
		if (isplay)
			return;
		isplay = true;
		if (barCtrl.getIsUp ()) {
			StartCoroutine (Utils.DelayRun (() => {
				if (isShowUp < 3) {
					EffectManager.Instance.CreateEffectCtrlByCache (transform, "Effect/UiEffect/skillLevelUpWindow_numbereffect", (obj,effectCtrl) => {
						effectCtrl.transform.localPosition = new Vector3 (this.transform.localPosition.x + 160, this.transform.localPosition.y - 60, this.transform.localPosition.z);
					});
				}
				StartCoroutine (showRestorePrize ());
			}, 1f));
		} else {
			StartCoroutine (showRestorePrize ());
		}
	}
	/// <summary>
	/// 显示返回奖励
	/// </summary>
	public IEnumerator showRestorePrize ()
	{
		yield return new WaitForSeconds (1.2f);
		if (IntensifyCardManager.Instance.sacrificeRestorePrize != null) {
			UiManager.Instance.openDialogWindow<AllAwardViewWindow> ((win) => {	
				win.Initialize (IntensifyCardManager.Instance.sacrificeRestorePrize,LanguageConfigManager.Instance.getLanguage ("s0120"));
			});
			IntensifyCardManager.Instance.sacrificeRestorePrize = null;
		}
	}

	public void setNum (int num)
	{
		isShowUp = num;
	}
	
	void onLevelUp (int now)
	{
		levelNow += 1;
		skillLevel.text = "Lv." + levelNow + "/" + Mathf.Min (skillData.getMaxLevel (), card.getLevel () + 5);
		skillDesc.text = skillData.getDescribeByLv (levelNow);
	}
	//_type 指定的话，一般用于skill为空的时候
	public	void updateButton (LevelupInfo data, Card card)
	{
		this.card = card;
		skillData = (Skill)data.orgData;
		int skillType = 0;
		
		barCtrl.init (data);
		barCtrl.setLevelUpCallBack (onLevelUp);
		barCtrl.endCall = levelUpEffect;
		if (data != null) {
			skillType = skillData.getSkillStateType ();
			levelNow = data.oldLevel;
			skillLevel.text = "Lv." + levelNow + "/" + Mathf.Min (skillData.getMaxLevel (), card.getLevel () + 5);
			textLabel.text = skillData.getName ();
			skillDesc.text = skillData.getDescribeByLv (levelNow);
			expLabel.text = skillData.getEXP () + "/" + skillData.getEXPUp ();
		}

		if (skillData != null)
			ResourcesManager.Instance.LoadAssetBundleTexture (skillData.getIcon (), skillIcon);
		else {
			if (skillType == TYPE_RED) {
				ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.SKILLIMAGEPATH + "Skill_Null", skillIcon);
			} else if (skillType == TYPE_YELLOW) {
				ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.SKILLIMAGEPATH + "Skill_Null", skillIcon);
			} else if (skillType == TYPE_BLUE) {
				ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.SKILLIMAGEPATH + "Skill_Null", skillIcon);
			}

		}

	}
}
