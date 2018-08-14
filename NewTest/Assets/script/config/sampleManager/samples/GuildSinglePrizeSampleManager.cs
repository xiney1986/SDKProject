using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/**公会BOSS奖励模板管理器
  *负责公会BOSS个人奖励模板信息的初始化 
  *@author gc
  **/
public class GuildSinglePrizeSampleManager : SampleConfigManager
{
		private static GuildSinglePrizeSampleManager instance;
		private List<GuildBossSinglePrizeSample> prizes;
	
		public static GuildSinglePrizeSampleManager Instance {
				get {
						if (instance == null)
								instance = new GuildSinglePrizeSampleManager ();
						return instance;
				}
		}
	
		public GuildSinglePrizeSampleManager ()
		{
				base.readConfig (ConfigGlobal.CONFIG_GUILDBOSSSINGLEPRIZE);
		}
	
		//解析配置
		public override void parseConfig (string str)
		{  
				GuildBossSinglePrizeSample be = new GuildBossSinglePrizeSample (str);
				if (prizes == null)
						prizes = new List<GuildBossSinglePrizeSample> ();
				prizes.Add (be);
		}

		public int getPrizesSumByHurtRank (int hurtRank)
		{
				foreach (GuildBossSinglePrizeSample prize in prizes) {
						if (prize.hurtRank == hurtRank)
								return prize.prizeSum;
				}
				return 0;
		}


}
