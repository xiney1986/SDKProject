using UnityEngine;
using System.Collections;

/// <summary>
/// 宣传实体
/// </summary>
public class Bulletin {

	public Bulletin (int sid, int state, int type, string name, string title, string desc) {
		this.sid = sid;
		this.state = state;
		this.type = type;
		this.name = name;
		this.title = title;
		this.desc = desc;
	}

	/** 宣传sid */
	public int sid;
	/** 宣传标签类型，0无，1NEW，2HOT */
	public int state;
	/** 宣传类型：1公告，2活动，3更新 */
	public int type;//
	/** 宣传名称 */
	public string name;
	/** 宣传标题 */
	public string title;
	/** 宣传内容 */
	public string desc;
}
