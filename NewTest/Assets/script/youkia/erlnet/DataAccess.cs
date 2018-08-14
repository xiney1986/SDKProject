using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 同步通讯类，需要等待返回，有超时
 * 
 * @author longlingquan
 * */
public class DataAccess:PortHandler
{
	
	/** 默认的等待时间为10秒 */
	public   const int TIMEOUT = 10 * 1000;
	/** 默认延迟秒数为5秒 */
	public const int DELAY = 5 * 1000;
	/** 实例化对象 */
	private static DataAccess _dataAccess;  
	/** 接收后台广播的默认处理函数 */
	public ReceiveFun defaultHandle = null;
	public Timer timeout;
	
	/** 条目列表 */
	private List<ErlEntry> _list = new List<ErlEntry> ();
	
	public DataAccess ()
	{
	}

	
	/** 获得实例 */
	public static DataAccess getInstance ()
	{
		if (_dataAccess == null) {
			_dataAccess = new DataAccess ();
		}
		return _dataAccess;
	}
	
	public void access (ErlConnect connect, ErlKVMessage message, ReceiveFun receiveFun, List<object> argus, long timeOut)
	{ 
		ByteBuffer data = new ByteBuffer ();
		message.bytesWrite (data);
		_list.Add (new ErlEntry (connect, message.getPort (), receiveFun, argus, timeOut + TimeKit.getMillisTime ()));  
		connect.sendErl (data, ErlConnect.ENCRYPTION, ErlConnect.CRC, ErlConnect.COMPRESS, ErlConnect.KV); 
		if(timeout==null)
		{
			timeout = TimerManager.Instance.getTimer(DELAY);
			timeout.addOnTimer(onTimer);
			timeout.start();
		}
	}
	
	override public void erlReceive (Connect connect, ErlKVMessage message)
    {
	    if (!MiniConnectManager.IsRobot && Debug.isDebugBuild == true)
	    {
	        if (message.Cmd == "match") Debug.Log("this is socketReceive! cmd=" + message.Cmd);
            else Debug.Log("this is socketReceive! cmd=" + message.Cmd + " jsonString " + message.toJsonString());
	    }
			  
		string cmd = message.Cmd; 
		if (cmd == "r_ok" || cmd == "r_err") {
			int port = message.getPort ();// 获取流水号
			ErlEntry entry = removeReciveFun (port);
			if (entry == null || entry.receiveFun == null) {
				return;
			} 
			entry.receiveFun (connect, message); 
		}
		else
		{// 服务器的广播消息
			message.addValue ("cmd", new ErlString (cmd));// 为js服务的代码 
			defaultHandle (connect, message); 
		}
	}
	
	/** 获取返回方法 */
	private ErlEntry removeReciveFun (int port)
	{
		ErlEntry entry;
		for (int i=0; i<_list.Count; i++) {
			entry = _list [i] as ErlEntry;
			if (entry.number == port) {
				_list.Remove (entry);
				return entry;
			}
		}
		return null;
	}
	public void clearDataAccess() {
		lock (_list) {
			_list.Clear();
		}
	}
	/**access超时 */
	public void onTimer ()
	{
		ErlEntry entry;
		lock (_list) {
			ErlEntry[] array = _list.ToArray ();
		
			for (int i  =0; i<array.Length; i++) {
				entry = array [i] as ErlEntry; 
				if (entry.timeOut <= TimeKit.getMillisTime ()) {
					_list.Remove (entry);
					if (entry.receiveFun == null)
						continue;
					ErlKVMessage message = new ErlKVMessage ("r_timeOut");  
					List<object> tempArray = entry.argus; 
					defaultHandle (entry.connect, message);
				}
			}
		}
	}  
} 

