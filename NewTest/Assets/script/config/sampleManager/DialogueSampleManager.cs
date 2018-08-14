using System;
 
/**对白模板管理器
  *负责对白模板信息的初始化  
  *@author longlingquan
  **/
public class DialogueSampleManager:SampleConfigManager
{
	//单例
	private static DialogueSampleManager _Instance;
	private static bool _singleton = true;

	public DialogueSampleManager ()
	{
		if (_singleton) {
			throw new Exception ("This is singleton!");
		}
		base.readConfig (ConfigGlobal.CONFIG_DIALOGUE);
	}

	public static DialogueSampleManager Instance {
		get {
			
			if (_Instance == null) {
				_singleton = false;
				_Instance = new DialogueSampleManager ();
				_singleton = true;
				return _Instance;
			} else
				return _Instance;
		}
		set { 
			_Instance = value;
		}
	}
	
	//获得经验值模板对象
	public DialogueSample getDialogueSampleBySid (int sid)
	{
		if (!isSampleExist (sid))
			createSample (sid); 
		return samples [sid] as DialogueSample;
	}  
	
	//解析数据
	public override void parseSample (int sid)
	{
		DialogueSample sample = new DialogueSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}
} 

