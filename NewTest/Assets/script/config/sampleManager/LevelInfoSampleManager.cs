using System;

/**
 * 经验模板管理器
 * @author longlingquan
 * */
public class LevelInfoSampleManager:SampleConfigManager
{
	//单例
	private static LevelInfoSampleManager instance;
	public const int SID_MOVE = 1;//行动力上限
	public const int SID_STRENGTH = 2;//体力上限
	public const int SID_GRIEND = 3;//好友上限
	
	public LevelInfoSampleManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_LEVELINFO);
	}
	
	public static LevelInfoSampleManager Instance {
		get{
			if(instance==null)
				instance=new LevelInfoSampleManager();
			return instance;
		}
	}

	public  int getInfo(int sid,int level)
	{
		return getBySid(sid).getInfoByLevel(level);
	}

	private  LevelInfoSample getBySid (int sid)
	{
		if (!isSampleExist (sid))
			createSample (sid); 
		return samples [sid] as LevelInfoSample;
	}  

	public override void parseSample (int sid)
	{
		LevelInfoSample sample = new LevelInfoSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}
}  