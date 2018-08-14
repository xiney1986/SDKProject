using UnityEngine;
using System.Collections;

public class ArenaReplayWindow : WindowBase
{
	public ArenaReplayButton[] buttons;
	public UILabel lblTitle;
	public GameObject role1;
	public GameObject role2;
	ArenaReplayInfo replayInfo;
	ArenaFinalInfo info;
	CallBack callback;
	ArenaReplayInfoUser winer;
	ArenaReplayInfoUser loser;

	protected override void DoEnable ()
	{
		base.DoEnable ();
	
	}
	public void init (ArenaReplayInfo replayInfo, ArenaFinalInfo info, CallBack callback)
	{
		this.callback = callback;
		this.info = info;
		this.replayInfo = replayInfo;

		if (replayInfo.user1.win) {
			winer = replayInfo.user1;
			loser = replayInfo.user2;
		} else {
			winer = replayInfo.user2;
			loser = replayInfo.user1;
		}

		lblTitle.text = winer.score + " : " + loser.score;
	}

	protected override void begin ()
	{
		if (isAwakeformHide) {
			MaskWindow.UnlockUI ();
			return;
		}

		int count = ArenaFinalSampleManager.Instance.getSample (info.finalState).fightCount;
		for (int i = 0; i < count && i < buttons.Length; i++) {
			buttons [i].gameObject.SetActive (true);
			buttons [i].lblName.text = LanguageConfigManager.Instance.getLanguage ("Arena22_" + (i + 1));

			if (i < replayInfo.winUids.Count) {
				string uid = replayInfo.winUids [i];
				if (uid == loser.uid) //ui弄反了,这里跟着反
					buttons [i].leftWin.SetActive (true);
				else
					buttons [i].rightWin.SetActive (true);
			}
		}
        
		FuBenCardCtrl anim1 = addRole (role1, winer.style);
		FuBenCardCtrl anim2 = addRole (role2, loser.style);

		if (anim1 != null)
			anim1.playHappy ();
		if (anim2 != null)
			anim2.playFail ();
		MaskWindow.UnlockUI ();
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
	
		finishWindow ();
		if (gameObj.name == "close") {
			return;
		} else {
			MaskWindow.instance.setServerReportWait (true);
			GameManager.Instance.battleReportCallback = GameManager.Instance.intoBattle;

			int index = StringKit.toInt (gameObj.name);
			FPortManager.Instance.getFPort<ArenaReplayFPort> ().access (() => {
				if (callback != null) {
					callback ();
				}
				UiManager.Instance.clearWindows ();
			}, info.finalState, info.index, index);
		}
	}

	private FuBenCardCtrl addRole (GameObject parent, int icon)
	{
		passObj _obj = Create3Dobj (UserManager.Instance.getModelPath (icon)); 
        
		if (_obj.obj == null) {
			Debug.LogError ("role is null!!!");
			return null;
		} 
		_obj.obj.transform.parent = parent.transform;
		_obj.obj.transform.localScale = new Vector3 (300, 300, 300);
		_obj.obj.transform.localPosition = Vector3.zero;
		_obj.obj.transform.localRotation = new Quaternion (0, 180, 0, 1);
		Utils.SetLayer (_obj.obj, UiManager.Instance.gameCamera.gameObject.layer);
		return _obj.obj.transform.GetChild (0).GetComponent<FuBenCardCtrl> ();
	}
}
