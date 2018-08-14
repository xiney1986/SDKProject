using UnityEngine;
using System.Collections;

public class LaddersRecordsItem : MonoBehaviour {
	public string id;
	public string msg;
	public UILabel msgLabel;
	public ButtonBase recordButton;
	WindowBase win;

	public void init(WindowBase win,string msg,string id) {
		this.win = win;
		this.msg = msg;
		this.id = id;
		msgLabel.text = msg;
		initButtonInfo ();
	}

	public void initButtonInfo() {
		recordButton.fatherWindow = win;
		recordButton.onClickEvent = onClick;
	}

	private void onClick(GameObject gameObj) {
		if(win != null){
			(win as LaddersRecordsWindow).M_onClickUrl(this.id);
		}

	}
}
