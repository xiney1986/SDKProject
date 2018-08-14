using UnityEngine;
using System.Collections.Generic;
using System;


public class CombatTipsSample
{
	/** 排序 */
    public int sort;
	/** 标题 */
    public string title;
	/** 描述 */
    public string desc;
	/** 按钮描述 */
    public string btnStr;
	/** 参与的最大等级 */
    public int maxLv;
	/** 功能开放等级 */
    public int funshow;
	/** 关联窗口的SID */
    public int windowLinkSid;

    public void parse(int id, string str)
    {
        string[] arr = str.Split('|');
        sort = StringKit.toInt(arr[0]);//Array.ConvertAll<string, int>(arr[0].Split(','), (a) => { return StringKit.toInt(a); });
        title = arr[1];
        desc = arr[2];
        btnStr = arr[3];
        maxLv = StringKit.toInt(arr[4]);
        funshow = StringKit.toInt(arr[5]);
		windowLinkSid = StringKit.toInt(arr[6]);



    }


}
