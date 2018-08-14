using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class CombatTipsItem : MonoBase {

	public UILabel UI_Title;
	public UILabel UI_Desc;
	public UISprite UI_Mark;
	public ButtonBase UI_Go;
	private CombatTipsWindow fawin;
	private CombatTipsSample mSample;
	//private Vector3 mPos;


	private void Start () {
		UI_Go.onClickEvent = onClickBtn;
	}

	public void setFawin (CombatTipsWindow win) {
		fawin = win;
		UI_Go.fatherWindow = win;
	}


	private void onClickBtn ( GameObject go ) {
		WindowLinkManagerment.Instance.OpenWindow (mSample.windowLinkSid);
	}
	public void setData ( CombatTipsSample sample, bool isShowMark ) {
		mSample = sample;
		UI_Mark.gameObject.SetActive (isShowMark);
		UI_Title.text = sample.title ;
		UI_Desc.text = sample.desc;
	}
}

