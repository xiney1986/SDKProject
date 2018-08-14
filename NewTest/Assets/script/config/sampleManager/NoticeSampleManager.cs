using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/**公告模板管理器
  *负责公告模板信息的初始化 
  *@author 汤琦
  **/
public class NoticeSampleManager : SampleConfigManager
{
	//单例
	private static NoticeSampleManager _Instance ;
	
	public NoticeSampleManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_NOTICE);
	}

	public static NoticeSampleManager Instance {
		get {
			if (_Instance == null) {
				_Instance = new NoticeSampleManager ();
				return _Instance;
			}
			return _Instance;
		}
		set {
			_Instance = value;
		}
	}

	// 加载公告配置
	public void loadNoticeSample (string content)
	{
		lock (this) {
			clear ();
			NoticeManagerment.Instance.clearNoticeArray ();
			resolveConfig (content);
		}
	}

	//解析模板数据
	public override void parseSample (int sid)
	{
		NoticeSample sample = new NoticeSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}
	
	//获得公告模板对象
	public NoticeSample getNoticeSampleBySid (int sid)
	{
		lock (this) {
			if (!isSampleExist (sid))
				createSample (sid); 
			return samples [sid] as NoticeSample;
		}
	} 
	
	//获得指定限制的所有公告sid
	public int[] getAllNotice ()
	{
		lock (this) {
			List<int> list = new List<int> ();
			foreach (int key in data.Keys) { 
				list.Add (key);
			}
			return list.ToArray ();
		}
	}
	
	//需要修改 data samples 如果存在就覆盖，不存在添加
	public void updataSample (int sid, string dataStr)
	{
		if (data.ContainsKey (sid)) { // 存在
			data [sid] = dataStr;
			if (samples [sid] != null)//若样本已经创建，需要修改
				(samples [sid] as NoticeSample).parse (sid, dataStr);
		} else {
			data.Add (sid, dataStr);
		}
	}
}
