using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShareDrawManagerment {

    public int canDrawTimes = 0;
    public int isFirstShare = 0;

    public ShareDrawManagerment()
	{
	}
	//单例
    public static ShareDrawManagerment Instance {
        get { return SingleManager.Instance.getObj("ShareDrawManagerment") as ShareDrawManagerment; }
	}
    /// <summary>
    /// 获取可抽奖次数
    /// </summary>
    /// <returns></returns>
    public int getCanDrawTimes() {
        return canDrawTimes;
    }

}

