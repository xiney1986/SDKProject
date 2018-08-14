using UnityEngine;
using System.Collections;

/**
 * PVP玩家信息窗口
 * @author 汤琦
 * */
public class MassPlayerWindow : WindowBase
{
	/** PVP信息 */
	private PvpOppInfo opp;
	/** 布局节点 */
	public GameObject layoutRoot;
	/** 内容预制体 */
	public ButtonPvpInfo buttonInfo;
	/** 排名 */
	public UILabel lblRank;
//	/** 姓名 */
//	public UILabel lblName;
//	/** 等级 */
//	public UILabel lblLevel;
	/** 积分 */
	public UILabel lblIntegral;
//	/** 头像 */
//	public UITexture headIcon;
//	/** VIP图标 */
//	public UISprite vipSprite;   
//	/** 女神等级 */
//	public UILabel beastLv;
//	/** 战斗力 */
//	public UILabel combat;
//	/** 女神图片 */
//	public UITexture beast;
	public GameObject formationRoot;
	public GameObject tenRoleRoot;
//	/** 经验条 */
//	public barCtrl expbar;
//	/** 女神背景 */
//	public UISprite beastBg;
//	/** 等级背景 */
//	public UISprite lvBg;
	/** 开始战斗按钮 */
	public ButtonBase ButtonBattleStart;
	/** 卡片预制体 */
	public GameObject cardPref;
	public int teamType ;
	CallBack battleCallback;

	protected override void DoEnable ()
	{
		//不切背景的话,战斗后竞技场点人出这个窗口再关闭会花屏,因为那时候backGroundWindow的背景空了
		UiManager.Instance.backGround.switchBackGround ("ChouJiang_BeiJing");
	}

	protected override void begin ()
	{
		base.begin ();

		if (isAwakeformHide) {
			MaskWindow.UnlockUI ();
			return;
		}

		if (opp != null) {
			FormationSample sample = FormationSampleManager.Instance.getFormationSampleBySid (opp.formation);
			buttonInfo.initInfo (opp,this);
//			lblName.text = opp.name;
			lblIntegral.text = LanguageConfigManager.Instance.getLanguage ("Arena05") + ": " + opp.arenaIntegral;
			lblRank.text = LanguageConfigManager.Instance.getLanguage ("Arena04", ArenaManager.instance.getTeamNameById (opp.arenaTeam), opp.arenaRank + "");
//			ResourcesManager.Instance.LoadAssetBundleTexture (UserManager.Instance.getIconPath (opp.headIcon), headIcon);
//			int level = EXPSampleManager.Instance.getLevel (EXPSampleManager.SID_USER_EXP, opp.exp, 0);
//			lblLevel.text = "Lv." + level;
//			combat.text = LanguageConfigManager.Instance.getLanguage ("s0368") + opp.combat;
//			if (level == 0) {
//				expbar.updateValue (opp.exp, 0);
//			} else {
//				expbar.updateValue (EXPSampleManager.Instance.getNowEXPShow (EXPSampleManager.SID_USER_EXP, opp.exp), EXPSampleManager.Instance.getMaxEXPShow (EXPSampleManager.SID_USER_EXP, opp.exp));
//			}
//			if (opp.vipLv > 0) {
//				vipSprite.spriteName = "vip" + opp.vipLv;
//				vipSprite.gameObject.SetActive (true);
//			} else {
//				vipSprite.gameObject.SetActive (false);
//			}

			//更新战斗按钮文字
			if (ArenaManager.instance.massBattleType == BattleType.BATTLE_SUBSTITUTE) {
				ButtonBattleStart.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0187");    
			} else if (ArenaManager.instance.massBattleType == BattleType.BATTLE_TEN) {
				//只有10v10人才有资格叫 大乱斗
				ButtonBattleStart.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0191");    
			} else {
				ButtonBattleStart.textLabel.text = LanguageConfigManager.Instance.getLanguage ("s0186");    
			}

			loadFormationGB (sample.getLength (), layoutRoot);
			CreateFormation (opp);
		}
//		CreateBeast ();
		MaskWindow.UnlockUI ();
	}
	
//	private void CreateBeast ()
//	{
//		if (opp != null && opp.beastSid != 0) {
//			beastBg.gameObject.SetActive (false);
//			beast.gameObject.SetActive (true);
//			CardSample cs = CardSampleManager.Instance.getRoleSampleBySid (opp.beastSid);
//			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.NVSHENHEADPATH + cs.imageID + "_head", beast);
//			beastLv.text = "Lv." + EXPSampleManager.Instance.getLevel (cs.levelId, opp.beastExp, 0).ToString ();
//			beast.gameObject.SetActive (true);
//			beastLv.gameObject.SetActive (true);
//			lvBg.gameObject.SetActive (true);
//		} else {
//			beastBg.gameObject.SetActive (true);
//			beast.gameObject.SetActive (false);
//			beast.gameObject.SetActive (false);
//			beastLv.gameObject.SetActive (false);
//			lvBg.gameObject.SetActive (false);
//		}
//	}
    public override void OnNetResume()
    {
        base.OnNetResume();
        finishWindow();
        if (UiManager.Instance.getWindow<ArenaAuditionsWindow>() != null)
        {
            UiManager.Instance.getWindow<ArenaAuditionsWindow>().updateArenaAuditionWindow();
        }
    }

    public void initInfo (PvpOppInfo opp, CallBack battleCallback)
	{
		this.opp = opp;
		this.battleCallback = battleCallback;
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow ();
		} else if (gameObj.name == "buttonBattleStart") {
			if (battleCallback != null) {
				battleCallback ();
				battleCallback=null;
			}
		}
	}

	//加载阵型对象
	private void loadFormationGB (int formationLength, GameObject root)
	{
		if (teamType == 10)
			return;
		passObj go = FormationManagerment.Instance.getPlayerInfoFormationObj (formationLength);
		go.obj.transform.parent = root.transform;
		go.obj .transform.localPosition = Vector3.zero;
		go.obj .transform.localScale = Vector3.one;		
		if (go.obj != null) {
			formationRoot = go.obj;
			go.obj.transform.localPosition = new Vector3 (0, 230, 0);
		}
		
	}
	
	void CreateFormation (PvpOppInfo info)
	{ 
		if (teamType == 10) {
			GameObject psObj;
			TeamPrepareCardCtrl card;

			for (int i =0; i<info.opps.Length; i++) {
				tenRoleRoot.gameObject.SetActive (true);
				psObj = NGUITools.AddChild (tenRoleRoot, cardPref);
				card = psObj.GetComponent<TeamPrepareCardCtrl> ();
				card.fatherWindow = this;
				//找到对应的阵形点位
				Transform formationPoint = null;
				formationPoint = tenRoleRoot.transform.FindChild ((info.opps [i].index + 1).ToString ());
				card.transform.position = formationPoint.position;
				card.updateButton (info.opps [i]);
				card.initInfo (info.uid, info.opps [i].uid, null);
			}

		} else {
			GameObject psObj;
			TeamPrepareCardCtrl card;

			for (int i = 0; i < info.opps.Length; i++) {
				psObj = NGUITools.AddChild (formationRoot, cardPref);
				if (psObj == null) {
					print ("contentTeamPrepare no res!");
					return;
				}
				card = psObj.GetComponent<TeamPrepareCardCtrl> ();
				//找到对应的阵形点位
				Transform formationPoint = null;
				card.fatherWindow = this;
				formationPoint = formationRoot.transform.FindChild ((info.opps [i].index + 1).ToString ());
				card.transform.position = formationPoint.position;
				card.updateButton (info.opps [i]);
				card.initInfo (info.uid, info.opps [i].uid, null);
			}
		}
	}
}
