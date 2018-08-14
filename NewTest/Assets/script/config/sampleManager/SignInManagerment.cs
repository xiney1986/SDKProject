using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


/**签到信息
  *@author gc
  **/
public class SignInManagerment : SampleConfigManager
{
    private static SignInManagerment instance;
    public SignInManagerment()
	{

	}

    public static SignInManagerment Instance {
		get{
			if(instance==null)
                instance = new SignInManagerment();
			return instance;
		}
	}
    public List<int> stateList = new List<int>();//已签到的日期
    public int month;//月份
    public int sign_inTimes;//补签次数
    public List<int> getAllState() {
        return stateList;
    }
    public int getMonth() {
        return month;
    }
    public int getSignInTimes() {
        return sign_inTimes;
    }
}
