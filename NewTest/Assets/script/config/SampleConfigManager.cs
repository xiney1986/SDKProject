using System;
using System.Collections;

/**sid模板配置文件管理器
  *负责sid模板信息分块 不负责具体信息初始化
  *@author longlingquan
  **/
public class SampleConfigManager : ConfigManager, IGMUpdateConfigManager {
	//数据分块
	public Hashtable data;
	//模板集合
	public Hashtable samples;

	public SampleConfigManager () {
		data = new Hashtable ();
		samples = new Hashtable ();
	}

	//对配置文件进行分块处理 根据sid
	public override void parseConfig ( string str ) {
		base.parseConfig (str);
		//分割符为|
		string[] strArr = str.Split ('|');
		int sid = StringKit.toInt (strArr[0]);
		data.Add (sid, str);
	}

	//解析对应模板 子类必须覆盖
	public virtual void parseSample ( int sid ) {

	}

	//对应模板是否已经存在
	public bool isSampleExist ( int sid ) {
		return samples.Contains (sid);

	}

	//获得对应sid模板源数据
	public string getSampleDataBySid ( int sid ) {
		if (data[sid] == null) {
			throw new Exception (GetType () + " getSampleDataBySid error!  sid=" + sid);
		}
		return data[sid] as string;
	}

	public void createSample ( int sid ) {
		if (samples == null)
			samples = new Hashtable ();
		try {
			//获得模板数据
			parseSample (sid);
		} catch(Exception ex) {
//			MonoBase.print(ex);
		}
	}

	public void clear () {
		if (data != null)
			data.Clear ();
		if (samples != null)
			samples.Clear ();
	}

	public Hashtable getSamples () {
		return samples;
	}
}

