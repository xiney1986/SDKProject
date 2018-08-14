using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/**
 * 游戏参数模板管理器
  *@author yxl
  **/
public class GameConfig : SampleConfigManager
{
    public const int SID_ARENA_FIGHT_CD_VIP = 1;//竞技场免CD挑战所需VIP等级
    public const int SID_ARENA_OPEN_LEVEL = 2;//竞技场开放等级
    public const int SID_ARENA_TEAM_DIALOG = 3;//竞技场海选某等级前提示队伍
    public const int SID_ARENA_REFRESH_ENEMY_CD = 4;//竞技场刷新对手CD时间(秒)
    public const int SID_ARENA_REFRESH_CHALLENGE_CD = 5;//竞技场清挑战CD消耗钻石


	private static GameConfig instance;
	//单例

    public GameConfig ()
	{
        base.readConfig (ConfigGlobal.CONFIG_GAME);
	}

    public static GameConfig Instance {
		get{
			if(instance==null)
				instance=new GameConfig();
			return instance;
		}
	}

    Dictionary<int,string> data = new Dictionary<int, string>();
	
	//解析配置
	public override void parseConfig (string str)
	{  
        string[] strs = str.Split('|');
        int key = StringKit.toInt(strs[0]);
        string vale = strs [1];
        data.Add(key, vale);
	}

    public int getInt(int sid)
    {
        return StringKit.toInt(data[sid]);
    }

    public string getString(int sid)
    {
        return data [sid];
    }

    public bool getBool(int sid)
    {
        return data [sid] == "1";
    }

    public long getLong(int sid)
    {
        return StringKit.toLong(data[sid]);
    }
}
