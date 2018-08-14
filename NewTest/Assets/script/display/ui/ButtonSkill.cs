using UnityEngine;
using System.Collections;

public class ButtonSkill : ButtonBase
{
	public UILabel skillLevel;//技能等级
	public UISprite background;//技能背景
	public UITexture icon;//技能图标
	public UISprite learIcon;//是否可学习按钮
	public UILabel skillDesc;//技能说明
	public barCtrl expbar;//经验条
	public UILabel expLabel;//经验值

	[HideInInspector]
	public int
		skillType;//1篮buff 2红主动 3黄被动,  参考	 skillStateType
	[HideInInspector]
	public int
		skillState;//1已经学，2可以学，3未开放，4幻兽技能
	 
	public const int TYPE_BLUE = 1;//1篮buff
	public const int TYPE_RED = 2;//2红主动
	public const int TYPE_YELLOW = 3;//3黄被动
	public const int STATE_LEARNED = 1;//1已经学
	public const int STATE_CANLEARN = 2;//2可以学
	public const int STATE_NOOPEN = 3;//3未开放
	public const int STATE_BEAST = 4;//4幻兽技能
	public const float textPosUp = 6.4f;
	public const float textPosMid = -2.4f;
	public static Color EFFECTLINE_RED = new Color (1, 0.6f, 0f, 1f);
	public static Color EFFECTLINE_YELLOW = new Color (1, 0.9f, 0f, 1f);
	public static Color EFFECTLINE_BLUE = new Color (0.55f, 0.8f, 1f, 1f);
	public static Color EFFECTLINE_BROWN = new Color (0.44f, 0.4f, 0.1f, 1f);
	public Skill skillData;
	public Card owner;
	public bool  useInSkillChoose = false;//在技能选择面板中，忽略点击
	public bool useInPicture; //在图鉴界面中,只查看已有技能.
	public CallBack<ButtonSkill> OnClickCallback;
	public CallBack<ButtonSkill> OnLongPassCallback;
	private string names;
	private string desc;
	private long exp;
	private int type;
	private string level;
	
	public void initSkillData (Skill data, int _state)
	{
		initSkillData (data, _state, -1);
	}

	public void initSkillData (Skill data, int _state, Card card)
	{
		owner = card;
		initSkillData (data, _state, -1);
	}

	
	public void initBeastSkill (Skill data, int _state, string name, string desc, long exp, int type, string level)
	{
		this.names = name;
		this.desc = desc;
		this.exp = exp;
		this.type = type;
		this.level = level;
		initSkillData (data, _state, -1);
	}

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		if (OnClickCallback != null) {
			OnClickCallback (this);
			MaskWindow.UnlockUI ();
			return;
		}
		if (useInSkillChoose == true) {
			MaskWindow.UnlockUI ();
			return;
		}
		MaskWindow.LockUI ();


		//以下属性面板用
		if (skillData != null) {
			UiManager.Instance.openDialogWindow<SkillInfoWindow> ((win) => {
				if (skillState != STATE_BEAST) {
					Card showCard = owner != null ? owner : (fatherWindow is CardBookWindow) ? (fatherWindow as CardBookWindow).getShowCard () : null;
					win.Initialize (skillData, showCard);//暂时这么处理
					if (fatherWindow != null && fatherWindow.GetType () == typeof(CardBookWindow)) {
						CardBookWindow cwin = fatherWindow as CardBookWindow;
						Card card = cwin.getShowCard ();
						if (card != null && cwin.getShowType () == CardBookWindow.VIEW) {
							if (StorageManagerment.Instance.getAllRole ().Contains (card)) {
								//绑定技能无法替换
								if (SkillSampleManager.Instance.getSkillSampleBySid (skillData.sid).isBind) {
									//MaskWindow.UnlockUI ();
									return;
								}
								//小弟卡只能学习被动技能
								else if (card.uid != UserManager.Instance.self.mainCardUid && skillData.getSkillStateType () != SkillStateType.ATTR) {
									//MaskWindow.UnlockUI ();
									return;
								}
								//显示替换按钮
								win.ShowRepick (() => {
									if (UserManager.Instance.self.getUserLevel () < 25) {
										MessageWindow.ShowAlert (string.Format (LanguageConfigManager.Instance.getLanguage ("s0402"), 25));
									} else {
										UiManager.Instance.openWindow<LearnSkillWindow> ((win2) => {
											win2.init (card, skillData, skillType);
										});
									}
								});
							}
						}
					}
				} else
					win.Initialize (names, desc, exp, type, level);
				if (fatherWindow != null)
					win.GetComponent<UIPanel> ().depth = fatherWindow.GetComponent<UIPanel> ().depth + 10000;
			});
		} else {
			if (useInPicture) {
				//MaskWindow.UnlockUI ();
				return;
			}
			if (skillState == STATE_NOOPEN) {
				//MaskWindow.UnlockUI ();
				return;
			}
			//如果是可学习
			if (skillState == STATE_CANLEARN && fatherWindow.GetType () == typeof(CardBookWindow)) {
				//跳到学习技能
				CardBookWindow cwin = fatherWindow as CardBookWindow;
				Card card = cwin.getShowCard ();
				if (card != null && cwin.getShowType () < CardBookWindow.SHOW) {
					if (UserManager.Instance.self.getUserLevel () < 25) {
						MessageWindow.ShowAlert (string.Format (LanguageConfigManager.Instance.getLanguage ("s0402"), 25));
					} else {
						GuideManager.Instance.doGuide ();
						UiManager.Instance.openWindow<LearnSkillWindow> ((win2) => {
							win2.init (card, skillData, skillType);
						});

					}
				}
				//MaskWindow.UnlockUI();
			} else {
				if (fatherWindow != null && fatherWindow.GetType () == typeof(CardBookWindow)) {
					if ((fatherWindow as CardBookWindow).getShowType () == CardBookWindow.CHATSHOW) {
						//MaskWindow.UnlockUI ();
						return;
					}
				}
				UiManager.Instance.openDialogWindow<SkillInfoWindow> ((win) => {
					win.Initialize (skillLevel.text, textLabel.text);//共鸣之力,skillLevel连接的是按钮名字
				});
			}
		}
	}

 
	//_type 指定的话，一般用于skill为空的时候
	public	void initSkillData (Skill data, int _state, int _type)
	{
		skillData = data;
		skillState = _state;
		if (_type != -1)
			skillType = _type;
		else
			skillType = TYPE_YELLOW;
		if (skillData != null) {
			skillType = skillData.getSkillStateType ();
			if (skillState == STATE_BEAST) {
				int mainSkillLv = EXPSampleManager.Instance.getLevel(EXPSampleManager.SID_HALLOW_EXP,exp);
				if(mainSkillLv == 0)
					mainSkillLv = 1;
				if(mainSkillLv >= skillData.getMaxLevel())
					mainSkillLv = skillData.getMaxLevel();
				skillLevel.text = "Lv." + mainSkillLv;

			} else {
//				skillLevel.text = "Lv." + Mathf.Min (skillData.getMaxLevel (), (owner == null ? skillData.getLevel () : owner.getLevel () + 5));
				if(skillData.getShowType() != 1 || skillData.getMaxLevel () == 0){
					skillLevel.text = "";
				}else{
					skillLevel.text = "Lv." + skillData.getLevel ();
				}

			}
			//textLabel.text =(skillData.getSkillQuality () == 1?"[FFFFFF]": QualityManagerment.getQualityColor (skillData.getSkillQuality ())) + skillData.getName ();
            textLabel.text =   skillData.getName();
			ResourcesManager.Instance.LoadAssetBundleTexture (skillData.getIcon (), icon);

			//下面的代码不能乱走滴
			if(fatherWindow is BeastSummonShowWindow || fatherWindow is BeastAttrLevelInfo) {
				return;
			}
			if(skillDesc!=null)//被动技不显示描述
			{
				skillDesc.text = skillData.getDescribe ();
			}
			if(expbar!=null)//被动技不显示进度
			{
				expbar.gameObject.SetActive (true);
				expLabel.gameObject.SetActive (true);
				expbar.updateValue (skillData.getEXP () - skillData.getEXPDown (), skillData.getEXPUp () - skillData.getEXPDown ());
				expLabel.text = (skillData.getEXP () - skillData.getEXPDown ()) + "/" + (skillData.getEXPUp () - skillData.getEXPDown ());

				if (fatherWindow is CardBookWindow) {
					int showType = (fatherWindow as CardBookWindow).getShowType ();
					if (showType == CardBookWindow.CHATSHOW || showType == CardBookWindow.CLICKCHATSHOW) {
						expbar.gameObject.SetActive (false);
						expLabel.gameObject.SetActive (false);
					}
				} else if (fatherWindow is CardPictureWindow) {
					expbar.gameObject.SetActive (false);
					expLabel.gameObject.SetActive (false);
				}
			}

		} else {
			skillLevel.text = "";
			textLabel.text = "";

			if (skillType == TYPE_RED) {
				ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.SKILLIMAGEPATH + "Skill_Null", icon);
			} else if (skillType == TYPE_YELLOW) {
				ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.SKILLIMAGEPATH + "Skill_Null", icon);
			} else if (skillType == TYPE_BLUE) {
				ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.SKILLIMAGEPATH + "Skill_Null", icon);
			}
		}
		
		if (skillState == STATE_CANLEARN) {
			if (learIcon != null)
				learIcon.gameObject.SetActive (true);
		} else if (skillState == STATE_NOOPEN) {
			gameObject.SetActive (false);
		}

//		if (background != null) {
//			if (skillType == TYPE_BLUE)
//				background.color = Color.blue;
//			else if (skillType == TYPE_RED)
//				background.color = Color.red;
//			else
//				background.color = Color.yellow;
//		}
	}

	public override void DoLongPass ()
	{
		if (OnLongPassCallback != null) {
			OnLongPassCallback (this);
		}
	}
}
