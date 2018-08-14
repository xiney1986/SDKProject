using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 图鉴模版
/// </summary>
public class PictureSample : Sample {
	/** 图鉴SIDS */
	public List<int> ids = new List<int>();
	/** 排序值 */
	public int sortValue;
	/** 获取途径开关 */
	public List<int> isON = new List<int>();

	/** 获取途径-副本SIDS */
	public List<int> missionSids = new List<int>() ;
	public override void parse (int sid, string str)
	{
		base.parse (sid, str);
		ids = new List<int> ();
		string [] strs = str.Split ('|');
		ids.Add (StringKit.toInt (strs [0]));
		ids.Add (StringKit.toInt (strs [1]));
		ids.Add (StringKit.toInt (strs [2]));
		ids.Add (StringKit.toInt (strs [3]));
		sortValue = StringKit.toInt (strs [4]);
		parseItemIsON(strs[5]);

		if (strs.Length >= 7) {
			parseFuBenSid (strs [6]);
		}
	}
	/// <summary>
	/// 解析获取途径条目开关
	/// </summary>
	void parseItemIsON(string str){
		if (string.IsNullOrEmpty (str)) {
			return;
		}
		string [] strs = str.Split ('#');
		foreach (string s in strs) {
			isON.Add(StringKit.toInt(s));
		}
	}
	/// <summary>
	/// 解析副本sid
	/// </summary>
	void parseFuBenSid(string str){
		if (string.IsNullOrEmpty (str)) {
			return;
		}
		string [] strs = str.Split ('#');
		foreach (string s in strs) {
			missionSids.Add(StringKit.toInt(s));
		}
	}
}
