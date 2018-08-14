using UnityEngine;
using System.Collections;
/// <summary>
/// 图鉴获取途径模版
/// </summary>
public class PictureTipsSample : Sample
{
	/** 排序 */
	public int sort;
	/** 标题 */
	public string title;
	/** 描述 */
	public string des;
	/** 按钮名称 */
	public string buttonName;
	/** 是否可点击 */
	public bool isCanClick;
	/** 开放等级 */
	public int openLevel;
	/** 关联窗口的SID */
	public int windowLinkSid;

	public override void parse (int sid, string str)
	{
		string [] strs = str.Split ('|');
		this.sid = sid;
		sort = StringKit.toInt (strs [0]);
		title = strs [1];
		des = strs [2];
		buttonName = strs [3];
		isCanClick = strs[4] == "1";
		openLevel = StringKit.toInt (strs [5]);
		windowLinkSid = StringKit.toInt (strs [6]);
	}
}
