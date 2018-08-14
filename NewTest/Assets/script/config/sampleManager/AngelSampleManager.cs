using System;
using System.Collections;
using System.Collections.Generic;
/**
 * 样本管理器
 */
public class AngelSampleManager : SampleConfigManager {

	public AngelSampleManager ()
	{
		if (_singleton) {
			throw new Exception ("This is singleton!");
		}
		base.readConfig (ConfigGlobal.CONFIG_ANGEL); 
	}
	
	//单例
	private static AngelSampleManager _Instance;
	private static bool _singleton = true;
	
	public static AngelSampleManager Instance {
		get { 
			if (_Instance == null) {
				_singleton = false;
				_Instance = new AngelSampleManager ();
				_singleton = true;
				return _Instance;
			} else
				return _Instance;
		}
		set { 
			_Instance = value;
		}
	}
	
	//获得天使模板对象
	public AngelSample getAngelSampleBySid (int sid)
	{ 
		if (!isSampleExist (sid))
			createSample (sid); 
		return samples [sid] as AngelSample;
	}

	public override void parseSample (int sid)
	{
		AngelSample sample = new AngelSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}
	//获得所有天使模板对象
	public List<AngelSample> getAllAngelSamples(){
		List<AngelSample> angelList = new List<AngelSample> ();
		foreach (int key in data.Keys) {
			angelList.Add(getAngelSampleBySid(key));
		}
		return angelList;
	}
	///<summary>
	/// 根据VIP等级获取当前拥有的天使
	/// </summary>
	public AngelSample getAngelSampleByVipLevel(int Level){
		AngelSample tmp = null;
		AngelSample cur = null;
		foreach (int key in data.Keys) {
			cur = getAngelSampleBySid(key);
			if(cur.vipLevelRequired <= Level){
				if(tmp == null || tmp.vipLevelRequired < cur.vipLevelRequired){
					tmp = cur;
				}
			}
		}
		return tmp;
	}
    public string get3DObjPath(int Level)
    {
        AngelSample tmp = null;
        AngelSample cur = null;
        foreach (int key in data.Keys)
        {
            cur = getAngelSampleBySid(key);
            if (cur.vipLevelRequired <= Level)
            {
                if (tmp == null || tmp.vipLevelRequired < cur.vipLevelRequired)
                {
                    tmp = cur;
                }
            }
        }
        if (tmp == null) return "";
        else return "angel/" + tmp.modelID;
    }

}
