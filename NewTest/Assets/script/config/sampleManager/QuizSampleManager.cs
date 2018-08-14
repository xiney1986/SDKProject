using UnityEngine;
using System.Collections;

/**
 * 问题管理器
 * @author 陈世惟
 * */
public class QuizSampleManager : SampleConfigManager {

	public static QuizSampleManager Instance {
		get {
			return SingleManager.Instance.getObj("QuizSampleManager") as QuizSampleManager;
		}
	}

	public QuizSampleManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_QUIZ);
	}

	//获得模板对象
	public QuizSample getQuizSampleBySid (int sid)
	{
		if (!isSampleExist (sid))
			createSample (sid);
		return samples [sid] as QuizSample;
	}
	
	//解析模板数据
	public override void parseSample (int sid)
	{
		QuizSample sample = new QuizSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}
}
