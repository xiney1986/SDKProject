using UnityEngine;
using System.Collections.Generic;

public class GuildDonateConfigManager : ConfigManager {

	public class GuildDonateItem
	{
		public string description; //描述
		public int maxTimesOneDay; //每天捐献次数限制
		public int consume; //消耗
		public int activity; //活跃度
		public int dedication;//贡献值
	}

	//单例
	private static GuildDonateConfigManager instance;
	private List<GuildDonateSample> samples;
	
	public static GuildDonateConfigManager Instance {
		get{
			if(instance==null)
				instance=new GuildDonateConfigManager();
			return instance;
		}
	}
	
	public GuildDonateConfigManager ()
	{   
		samples = new List<GuildDonateSample> ();
		base.readConfig (ConfigGlobal.CONFIG_GUILDDONATE);
	}
	
	public override void parseConfig (string str)
	{
		string[] arr = str.Split ('|'); 
		GuildDonateSample s = new GuildDonateSample ();
		int sid = StringKit.toInt (arr[0]);
		s.parse (sid, str);
		samples.Add (s);
	}

	public GuildDonateItem[] getGuildDonateItems(int guildLevel)
	{
		GuildDonateItem[] items = new GuildDonateItem[samples.Count];
		for (int i = 0; i < items.Length; i++) {
			GuildDonateSample s = samples[i];
			GuildDonateItem item = new GuildDonateItem();
			item.description = s.description;
			item.maxTimesOneDay = s.maxTimesOneDay;
			item.consume = s.consume[guildLevel - 1];
			item.activity = s.activity[guildLevel - 1];
			item.dedication = s.dedication[guildLevel - 1];
			items[i] = item;
		}
		return items;
	}


}
