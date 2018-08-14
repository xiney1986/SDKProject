using UnityEngine;
using System.Collections;

public class ArenaTimeItem : MonoBase {
	/** 比赛场次 */
	public UILabel nameLabel;
	/** 时间 */
	public UILabel timeLabel;

	public void initUI(string name,string time){
		nameLabel.text = name;
		timeLabel.text = time;
	}
}
