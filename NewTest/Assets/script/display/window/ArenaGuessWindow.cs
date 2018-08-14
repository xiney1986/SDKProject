using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArenaGuessWindow : WindowBase
{
	public GameObject[] item;
	public UITexture[] texIcon;
	public UILabel[] lblName;
	public UILabel[] lblGuild;
	public UILabel[] lblLevel;
	public GameObject[] button;
	public GameObject[] select;
	public UILabel[] lblButton;
	public UILabel[] lblSelect;
	public UILabel lblExplanation;
	ArenaGuessUser[] users;
	ArenaFinalInfo info;
	int selectIndex;
	ArenaFinalPoint point;

	public void init (ArenaFinalPoint point)
	{
		this.point = point;
		this.info = point.ParentPoint.info;
        
		string str = LanguageConfigManager.Instance.getLanguage ("Arena14_" + info.finalState);
		string award = ArenaAwardSampleManager.Instance.getGuessPrizeDescription (info.finalState) + "\n";
		string text = LanguageConfigManager.Instance.getLanguage ("Arena21", str, award);
		lblExplanation.text = text;

		for (int i = 0; i < lblButton.Length; i++) {
			lblButton [i].text = LanguageConfigManager.Instance.getLanguage ("Arena12");
		}

		for (int i = 0; i < lblSelect.Length; i++) {
			lblSelect [i].text = LanguageConfigManager.Instance.getLanguage ("Arena13");
		}
	}

	protected override void begin ()
	{
		base.begin ();
		FPortManager.Instance.getFPort<ArenaGetGuessFPort> ().access (OnDataLoad, info.finalState, info.index);
		MaskWindow.UnlockUI ();
	}

	void OnDataLoad (ArenaGuessUser[] users)
	{
		this.users = users;
		for (int i = 0; i < 2; i++) {
			item [i].SetActive (users [i] != null);
			if (users [i] != null) {
				ArenaGuessUser u = users [i];
				ResourcesManager.Instance.LoadAssetBundleTexture (UserManager.Instance.getIconPath (u.headIcon), texIcon [i]);
				lblName [i].text = u.name;
				if(u.guild!="none")
				{
					lblGuild [i].text = u.guild;
				}else
				{
					lblGuild [i].text="";
				}
				lblLevel [i].text = "LV." + u.level;
				select [i].SetActive (u.select);
				button [i].SetActive (!u.select);
			}
		}
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow ();
		} else if (gameObj.name.StartsWith ("checkbutton")) {
			selectIndex = StringKit.toInt (gameObj.name.Split ('_') [1]);
			string winid = users [selectIndex].uid;
			FPortManager.Instance.getFPort<ArenaSetGuessFPort> ().access (OnGuessResult, info.finalState, info.index, winid);
		} else if (gameObj.name.StartsWith ("head")) {
			selectIndex = StringKit.toInt (gameObj.name.Split ('_') [1]);
			string uid = users [selectIndex].uid;
			openUserInfoWindow (uid);
		}

	}

	private void openUserInfoWindow (string uid)
	{
		ChatGetPlayerInfoFPort fport = FPortManager.Instance.getFPort ("ChatGetPlayerInfoFPort") as ChatGetPlayerInfoFPort;
		fport.access (uid,finishWindow,null, PvpPlayerWindow.FROM_ARENA);
		
	}

	void OnGuessResult (bool result)
	{
		if (result) {
			point.ParentPoint.info.guessed = true;
			point.UpdateInfo ();
			updateFocusPointInfo();
			if (selectIndex == 0) {
				users [0].select = true;
				if (users [1] != null)
					users [1].select = false;
			} else {

				users [1].select = true;
				if (users [0] != null)
					users [0].select = false;
			}

			for (int i = 0; i < 2; i++) {
				if (users [i] != null) {
					select [i].SetActive (users [i].select);
					button [i].SetActive (!users [i].select);
				}
			}
		}
		MaskWindow.UnlockUI ();
	}

	/// <summary>
	/// 更新聚焦决赛区域竞猜点信息
	/// </summary>
	private void updateFocusPointInfo()
	{
		if(point.window!=null&&point.window.focusGuess!=null)
		{
			ArenaFocusGuess.FocusPointInfo focusPointInfo=point.window.focusGuess.getFocusPointByPointName(point.gameObject.name);
			if(focusPointInfo!=null)
			{
				focusPointInfo.setGuessd(false);
				point.window.focusGuess.SortFocusCuess();
			}
		}
	}

}
