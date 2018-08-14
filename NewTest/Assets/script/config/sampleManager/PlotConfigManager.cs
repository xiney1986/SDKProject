using System;
 
/**
 * 剧情模板管理器
 * @author longlingquan
 * */
public class PlotConfigManager:SampleConfigManager
{
	private static PlotConfigManager _Instance;
	private static bool _singleton = true;
	
	public static PlotConfigManager Instance {
		get { 
			if (_Instance == null) {
				_singleton = false;
				_Instance = new PlotConfigManager ();
				_singleton = true;
				return _Instance;
			} else
				return _Instance;
		}
		set { 
			_Instance = value;
		}
	}

	public PlotConfigManager ()
	{
		if (_singleton)
			return;  
		base.readConfig (ConfigGlobal.CONFIG_PLOT); 
	}
	
	//解析模板数据
	public override void parseSample (int sid)
	{
		PlotSample sample = new PlotSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr);
		samples.Add (sid, sample);
	}
	
	//获得装备模板对象
	public PlotSample getPlotSampleBySid (int sid)
	{
		if (!isSampleExist (sid))
			createSample (sid); 
		return samples [sid] as PlotSample;
	}   

} 

