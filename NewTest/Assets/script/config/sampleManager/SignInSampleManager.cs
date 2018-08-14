using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


/**签到信息
  *@author gc
  **/
public class SignInSampleManager : SampleConfigManager
{
	//单例
	private static SignInSampleManager instance;
    private List<SignInSample> list;
    public SignInSampleManager()
	{

        base.readConfig(ConfigGlobal.CONFIG_SIGNINAWARD);
	}

    public static SignInSampleManager Instance {
		get{
			if(instance==null)
                instance = new SignInSampleManager();
			return instance;
		}
	}

    //获得指定Sid的节点信息
    public SignInSample getSignInSampleBySid(int sid) {
        if (list == null || list.Count <= 0) return null;
        for (int i = 0; i < list.Count; i++) {
            if (list[i].sid == sid) {
                return list[i];
            }
        }
        return null;
    }

    //解析配置
    public override void parseConfig(string str) {
        SignInSample be = new SignInSample(str);
        if (list == null)
            list = new List<SignInSample>();
        list.Add(be);
    }
}
