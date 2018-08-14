using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArenaTimeSampleManager : SampleConfigManager
{
	private static ArenaTimeSampleManager instance;
	public static ArenaTimeSampleManager Instance {
		get {
			if (instance == null)
				instance = new ArenaTimeSampleManager ();
			return instance;
		}
	}

	public ArenaTimeSampleManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_ARENA_TIME);
	}
	private void parseSample (int sid)
	{
		string str = data [sid].ToString ();
		ArenaTimeSample time = new ArenaTimeSample ();
		time.parse (sid, str);
		samples.Add (sid, time);
	}

	public ArenaTimeSample getSampleBySid (int sid)
	{
		if (samples [sid] == null)
			parseSample (sid);
		return samples [sid] as ArenaTimeSample;
	}
	/// <summary>
	/// 计算海选时间段
	/// </summary>
	public string getMassTimeString (int type, int endTime)
	{
		string timeString = "";
		int startTime = 0;
		/** 如果在海选阶段 */
		if (type == 1) {
			startTime = endTime - getSampleBySid (1).time;
			timeString = TimeKit.getDateTime (startTime).ToString ("yyyy.MM.dd") + "-" + TimeKit.getDateTime (endTime).ToString ("yyyy.MM.dd");
		} 
		/** 休赛期间显示下一届 */
		else if (type == 8) {
			startTime = endTime;
			endTime +=getSampleBySid (1).time;
			timeString = TimeKit.getDateTime (startTime).ToString ("yyyy.MM.dd") + "-" + TimeKit.getDateTime (endTime).ToString ("yyyy.MM.dd");
		} else {
			for (int i=2; i<=type; i++) {
				endTime -= getSampleBySid (i).time;
			}
			startTime = endTime - getSampleBySid (1).time;
			timeString = TimeKit.getDateTime (startTime).ToString ("yyyy.MM.dd") + "-" + TimeKit.getDateTime (endTime).ToString ("yyyy.MM.dd");
			
		}
		return timeString;
	}

	/// <summary>
	/// 获取决赛时间段
	/// </summary>
	public string getFinalTimeString (int type, int endTime)
	{
		string timeString = "";
		int startTime = 0;
		if (type == 1) {
			startTime = endTime;
			for (int i =2; i<8; i++) {
				endTime += getSampleBySid (i).time;
			}
			timeString = TimeKit.getDateTime (startTime).ToString ("yyyy.MM.dd") + "-" + TimeKit.getDateTime (endTime).ToString ("yyyy.MM.dd");
		}
		/** 休赛阶段显示下一届 */
		 else if (type == 8) {
			startTime = endTime + getSampleBySid (1).time;
			endTime = startTime;
			for(int i=2;i<8;i++){
				endTime += getSampleBySid (i).time;
			}
			timeString = TimeKit.getDateTime (startTime).ToString ("yyyy.MM.dd") + "-" + TimeKit.getDateTime (endTime).ToString ("yyyy.MM.dd");
		}
		else {
			startTime = endTime;
			for (int i=2; i<=type; i++) {
				startTime -= getSampleBySid (i).time;
			}

			for(int j=type+1;j<8;j++){
				endTime +=getSampleBySid (j).time;
			}
			timeString = TimeKit.getDateTime (startTime).ToString ("yyyy.MM.dd") + "-" + TimeKit.getDateTime (endTime).ToString ("yyyy.MM.dd");
		}
		return timeString;
	}
}
