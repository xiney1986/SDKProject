using UnityEngine;
using System.Collections;

/**
 * PVP杯赛窗口
 * @author 汤琦
 * */
public class PvpCupWindow : WindowBase
{
	public PvpCupButton[] button;
	public UISprite lineUp;
	public UISprite lineDown;
	private const string COMMONLINE = "pk_line";
	private const string WINLINE = "pk_line2";
	private const string OPPSPRITE = "pvpBar_enemy";
	private CallBack callback;
	
	protected override void DoEnable ()
	{
		base.DoEnable ();
		UiManager.Instance.backGround.switchBackGround("ChouJiang_BeiJing");
		if (MissionManager.instance != null)
			MissionManager.instance.hideAll ();
		//UiManager.Instance.backGroundWindow.switchToDark();
		//TODO switchBackGround
	}

	protected override void begin ()
	{
		base.begin ();
		if (PvpInfoManagerment.Instance.getPvpInfo ().round == 1) {
//			title.text = LanguageConfigManager.Instance.getLanguage ("s0216");
			lineUp.spriteName = COMMONLINE;
			lineDown.spriteName = COMMONLINE;
		} else if (PvpInfoManagerment.Instance.getPvpInfo ().round == 2) {
//			title.text = LanguageConfigManager.Instance.getLanguage ("s0217");
			lineUp.spriteName = COMMONLINE;
			lineDown.spriteName = COMMONLINE;
		} else if (PvpInfoManagerment.Instance.getPvpInfo ().round == 3) {
//			title.text = LanguageConfigManager.Instance.getLanguage ("s0218");
			lineUp.spriteName = WINLINE;
			lineDown.spriteName = WINLINE;
		}
		PvpInfoManagerment.Instance.setCurrentRound ();
		lineUp.gameObject.SetActive (true);
		lineDown.gameObject.SetActive (true);
		loadInfo ();
		MaskWindow.UnlockUI ();
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {

			if(BattleManager.Instance!=null)
				BattleManager.Instance.awardFinfish();
			else
			finishWindow ();

			if (MissionManager.instance != null)
			{
				MissionManager.instance.showAll ();
				MissionManager.instance.setBackGround();
			}

		} else if (gameObj.name == "fightButton") {
			PvpInfoManagerment.Instance.setOppIndex ();
			UiManager.Instance.switchWindow<PvpCupFightWindow> (
				(win) => {
				win.initInfo (PvpInfoManagerment.Instance.getOpp ());
			});
		} 
	}
	
	private void fightOpp ()
	{
		if (PvpInfoManagerment.Instance.getPvpInfo ().round == 1) {
			
		} else if (PvpInfoManagerment.Instance.getPvpInfo ().round == 2) {
			
		} else if (PvpInfoManagerment.Instance.getPvpInfo ().round == 3) {
			
		}
	}

	
	private void loadInfo ()
	{
		
		PvpInfo info = PvpInfoManagerment.Instance.getPvpInfo ();

		for (int j = 0; j < info.oppInfo.Length; j++) {
			for (int i = 0; i < button.Length; i++) {
				if (button [i].getPvpOppInfo () == null) {
					button [i].initButton (info.oppInfo [j]);
					break;
				}
			}
		}
	}

}

