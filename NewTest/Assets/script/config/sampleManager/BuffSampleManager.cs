using System;
  
/**buff模板管理器
  *负责buff模板信息的初始化  
  *@author longlingquan
  **/
public class BuffSampleManager:SampleConfigManager
{
	private static BuffSampleManager _Instance;
	private static bool _singleton = true;
	
	public const int SID_HP=-1;
	public const int SID_ANGER=-2;
	
	public static BuffSampleManager Instance {
		get { 
			if (_Instance == null) {
				_singleton = false;
				_Instance = new BuffSampleManager ();
				_singleton = true;
				return _Instance;
			} else
				return _Instance;
		}
		set { 
			_Instance = value;
		}
	}

	public BuffSampleManager ()
	{
		if (_singleton)
			return;  
		base.readConfig ( ConfigGlobal.CONFIG_BUFF);
		initSpecialBuffSample();
	}
	
	//获得技能模板对象
	public BuffSample getBuffSampleBySid(int sid)
	{
		if(!isSampleExist(sid))
			createSample(sid); 
		return samples[sid] as BuffSample;
	}  
	
	//解析模板数据
	public override void parseSample (int sid)
	{
		BuffSample sample = new BuffSample(); 
		string dataStr = getSampleDataBySid(sid); 
		sample.parse(sid,dataStr);
		samples.Add(sid,sample);
	}
	
	//添加前台特殊buff 前台专有 sid唯一 手动添加
	private void initSpecialBuffSample ()
	{ 
		BuffSample sample = new BuffSample (); 
		sample.sid = SID_HP;  
		sample.isDuration=false;
		sample.type = BuffType.damage;
		 
		samples.Add (sample.sid, sample);	 
		
		sample = new BuffSample (); 
		sample.sid = SID_ANGER;		 
		sample.displayType = BuffIconType.None;
		sample.isDuration = false;
		sample.type = BuffType.power;
		 
		samples.Add (sample.sid, sample);	 
		  
	}
} 

