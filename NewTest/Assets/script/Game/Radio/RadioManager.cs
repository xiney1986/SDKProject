using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class RadioManager
{

	/* static fields */
	/** 公告消息类型 */
	public const int RADIO_MAIN_TYPE=0, // 主界面公告
						RADIO_LUCKY_CARD_TYPE=1, // 限时抽卡公告
						RADIO_LUCKY_EQUIP_TYPE=2, // 限时抽装备公告
						RADIO_LUCKY_STARSOUL_TYPE=3//限时抽星魂
							;
	private const string SUFFIX = ".txt";
	private static RadioManager instance;
	public static RadioManager Instance
	{
		get
		{
			if(instance==null)
			{
				instance=new RadioManager();
			}
			return instance;
		}
	}
	/// <summary>
	/// 随机获取小贴士信息
	/// </summary>
	/// <returns>The tip message.</returns>
	/// <param name="randomMin">Random minimum.</param>
	/// <param name="randomMax">Random max.</param>
	public static string RandomTipMessage(int randomMin,int randomMax){
		int randomNum=Random.Range(randomMin,randomMax+1);
		string sid="";
		if(randomNum < 10) {
			sid ="s5000"+randomNum;
		}else {
			sid="s500"+randomNum;
		}
		string msg=LanguageConfigManager.Instance.getLanguage(sid);
		if(msg==sid) msg="";
		return msg;
	}

	/* fields */
	/** 广播消息信息条目 */
	private static Dictionary<int,RadioInfo> radioInfoDic;

	/* methods */
	/***/
	public RadioManager ()
	{
		radioInfoDic = new Dictionary<int,RadioInfo> ();
		/*关闭缓存读取*/
		//M_readCacheMsg();
	}
	/** 初始化广播信息对象 */
	private RadioInfo initRadioInfo(int radioType) {
		RadioInfo radioInfo=null;
		switch (radioType) {
		case RADIO_MAIN_TYPE: 
				radioInfo=initMainRadioInfo();
				break;
			case RADIO_LUCKY_CARD_TYPE: 
				radioInfo=initLuckCardRadioInfo();
				break;
			case RADIO_LUCKY_EQUIP_TYPE: 
				radioInfo=initLuckEquipRadioInfo();
				break;
			case RADIO_LUCKY_STARSOUL_TYPE: 
				radioInfo=initLuckLiehunRadioInfo();
				break;
		}
		return radioInfo;
	}
	/** 初始化主界面广播信息对象 */
	private RadioInfo initMainRadioInfo() {
		int[] customIds=new int[]{1,6};
		RadioInfo radioInfo = new RadioInfo (10,false,customIds);
		return radioInfo;
	}
	/** 初始化限时抽卡广播信息对象 */
	private RadioInfo initLuckCardRadioInfo() {
		RadioInfo radioInfo = new RadioInfo (10,true,null);
		return radioInfo;
	}
	/** 初始化限时抽装备广播信息对象 */
	private RadioInfo initLuckEquipRadioInfo() {
		RadioInfo radioInfo = new RadioInfo (10,true,null);
		return radioInfo;
	}
	/** 初始化限时猎魂广播信息对象 */
	private RadioInfo initLuckLiehunRadioInfo() {
		RadioInfo radioInfo = new RadioInfo (10,true,null);
		return radioInfo;
	}
	/// <summary>
	/// 读取缓存广播
	/// </summary>
	private void M_readCacheMsg ()
	{   
		string path = ConfigGlobal.CONFIG_FOLDER + "/" + ConfigGlobal.CONFIG_RADIO_MSG;
		string newPath=PathKit.GetOSDataPath (path) + SUFFIX;
		if (!File.Exists (newPath)) {
			M_createFile (path);
			return;
		} else {
			using (StreamReader sr = new StreamReader(newPath,Encoding.UTF8)) {
				string str = sr.ReadLine ();
				if (str != null) {
					string[] strs = str.Split (new char[]{'#'},System.StringSplitOptions.RemoveEmptyEntries);
					M_addRadioMsg(RADIO_MAIN_TYPE,strs); // 暂时只支持保存主界面广播
				}
			}
		}
	}
	/// <summary>
	/// 创建文件
	/// </summary>
	/// <param name="path">Path.</param>
	private void M_createFile (string path)
	{ 
		string holePath = PathKit.GetOSDataPath (path) + SUFFIX;
		FileStream fs = new FileStream (holePath, FileMode.OpenOrCreate, FileAccess.Write);
		StreamWriter sw = new StreamWriter (fs);
		sw.Flush ();
		sw.BaseStream.Seek (0, SeekOrigin.Begin);
		sw.Close (); 
	}
	/// <summary>
	/// 保存广播信息 应该是还未来得及显示的
	/// </summary>
	public void M_saveRadioMsg(int radioType)
	{
		if (!radioInfoDic.ContainsKey (radioType))
			return;
		string saveContent=string.Empty;
		RadioInfo radioInfo=radioInfoDic [radioType];
		Queue<string> msgCache = radioInfo.msgCache;
		while(msgCache.Count>0)
		{
			saveContent+=("#"+msgCache.Dequeue());
		}
		if(saveContent==string.Empty)
		{
			return;
		}
		string path = ConfigGlobal.CONFIG_FOLDER + "/" + ConfigGlobal.CONFIG_RADIO_MSG;
		string holePath = PathKit.GetOSDataPath (path) + SUFFIX;
		FileStream fs = new FileStream (holePath, FileMode.OpenOrCreate, FileAccess.Write);
		StreamWriter sw = new StreamWriter (fs);
		sw.Flush ();
		sw.BaseStream.Seek (0, SeekOrigin.Begin);
		sw.Write(saveContent);
		sw.Close (); 
	}
	/// <summary>
	/// 添加即时的广播 消息 来自服务器 这样的消息只显示一次
	/// </summary>
	/// <param name="_msg">_msg.</param>
	public void M_addRadioMsg(int radioType,string _msg)
	{
		RadioInfo radioInfo;
		if (!radioInfoDic.ContainsKey (radioType)) {
			radioInfo=initRadioInfo(radioType);
			radioInfoDic.Add (radioType, radioInfo);
		} else {
			radioInfo=radioInfoDic [radioType];
		}
		if(radioInfo.msgCache.Count>=radioInfo.maxCount) {
			radioInfo.msgCache.Dequeue();
		}
		radioInfo.msgCache.Enqueue(_msg);
	}
	/// <summary>
	/// 添加即时的广播 消息 来自服务器 这样的消息只显示一次
	/// </summary>
	/// <param name="_msg">_msg.</param>
	public void M_addRadioMsg(int radioType,string[] _msgs)
	{
		for(int i=0,length=_msgs.Length;i<length;i++)
		{
			M_addRadioMsg(radioType,_msgs[i]);
		}
	}
	/// <summary>
	/// 添加展示性的广播 当没有即时性广播是 显示 循环显示
	/// </summary>
	public void M_addTipMsg(int radioType,string _msg)
	{
		RadioInfo radioInfo;
		if (!radioInfoDic.ContainsKey (radioType)) {
			radioInfo=initRadioInfo(radioType);
			radioInfoDic.Add (radioType, radioInfo);
		} else {
			radioInfo=radioInfoDic [radioType];
		}
		radioInfo.addTipCache(_msg);
	}
	/// <summary>
	/// 返回一条最新的广播消息
	/// </summary>
	/// <returns>The last radio message.</returns>
	public string M_getLastRadioMsg(int radioType)
	{
		if (!radioInfoDic.ContainsKey (radioType))
			return null;
		RadioInfo radioInfo=radioInfoDic [radioType];
		string msg=null;
		if(radioInfo.msgCache.Count>0) {
			msg=radioInfo.msgCache.Dequeue();
			if(radioInfo.isAutoAddTip)
				M_addTipMsg(radioType,msg);
		}
		return msg;
	}
	/// <summary>
	/// 获取一条循环的广播
	/// </summary>
	/// <returns>The last radio message.</returns>
	public string M_getLastRadioTipMsg(int radioType)
	{
		if (!radioInfoDic.ContainsKey (radioType))
			return null;
		RadioInfo radioInfo=radioInfoDic [radioType];
		string msg=radioInfo.getTipCacheMessage();
		return msg;
	}
	/// <summary>
	/// 返回一条小贴士
	/// </summary>
	/// <returns>The random tip.</returns>
	public string M_getRandomTip(int radioType) {
		if (!radioInfoDic.ContainsKey (radioType))
			return "";
		RadioInfo radioInfo=radioInfoDic [radioType];
		int[] randomValues=radioInfo.customIds;
		if (randomValues != null) {
			string msg=RandomTipMessage(randomValues[0],randomValues[1]);
			return msg;	
		}
		return "";
	}
	/** 获取指定类型的广播消息缓存列表 */
	public string[] getCacheListByType(int radioType) {
		if (!radioInfoDic.ContainsKey (radioType))
			return null;
		RadioInfo radioInfo=radioInfoDic [radioType];
		string[] tipMsgCache = radioInfo.tipCache.ToArray ();
		string[] msgCache=radioInfo.msgCache.ToArray ();
		string[] messages=new string[msgCache.Length+tipMsgCache.Length];
		System.Array.Copy(tipMsgCache,0,messages,0,tipMsgCache.Length);
		System.Array.Copy(msgCache,0,messages,tipMsgCache.Length,msgCache.Length);
		return messages;
	}
	/** 清理指定类型的广播 */
	public void clearByType(int radioType) {
		if (!radioInfoDic.ContainsKey (radioType))
			return;
		RadioInfo radioInfo=radioInfoDic [radioType];
		radioInfo.clear ();
		radioInfo = null;
	}

	/** 广播信息 */
	public class RadioInfo {

		/* fields */
		/** 广播消息缓存 */
		public Queue<string> msgCache;
		/** tip缓存-循环播放的广播 */
		public List<string> tipCache;
		/** 最大缓存数量 */
		public int maxCount;
		/** 是否自动添加tip消息缓存 */
		public bool isAutoAddTip;
		/** 小贴士ids */
		public int[] customIds;
		/** 当前循环播放广播下标 */
		public int tipCacheIndex;

		/* methods */
		/***/
		public RadioInfo(int maxCount,bool isAutoAddTip,int[] customIds) {
			msgCache=new Queue<string>(maxCount);
			tipCache=new List<string>();
			this.maxCount=maxCount;
			this.isAutoAddTip=isAutoAddTip;
			this.customIds = customIds;
		}
		/// <summary>
		/// 添加消息到tip缓存-循环播放的广播
		/// </summary>
		/// <param name="message">Message.</param>
		public void addTipCache(string message){
			lock (tipCache) {
				if (tipCache.Count >= maxCount){
					tipCache.RemoveAt (0);
					delTipCacheIndex();
				}
				tipCache.Add (message);		
			}
		}
		/** 获取当前下标的循环广播 */
		public string getTipCacheMessage() {
			if (tipCache.Count == 0)
				return "";
			if (tipCacheIndex > tipCache.Count -1)
				tipCacheIndex = tipCache.Count -1;
			if (tipCacheIndex < 0)
				tipCacheIndex = 0;
			string message=tipCache[tipCacheIndex];
			addTipCacheIndex ();
			return message;
		}
		/** 清理 */
		public void clear() {
			msgCache.Clear ();
			tipCache.Clear();
			customIds=null;
		}

		/* properties */
		public void addTipCacheIndex() {
			if (tipCacheIndex > tipCache.Count - 1)
				tipCacheIndex=0;
			else
				tipCacheIndex++;
		}
		public void delTipCacheIndex() {
			if (tipCacheIndex == 0)
				return;
			tipCacheIndex--;
		}
	}
}