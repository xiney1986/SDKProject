using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
/** 
  * 连接管理器 
  * @author longlingquan 
  * */
public class ConnectManager
{
	/** 连接管理器的唯一实例 */
	private static ConnectManager _manager = null;
	private ErlConnectFactory factory;
	public ReceiveFun messageHandle;

	public static ConnectManager manager ()
	{
		if (_manager == null) {
			_manager = new ConnectManager ();
		}
		return _manager;
	}
	
	public ConnectManager ()
	{ 
		
	}
	
	public void init (GameObject gameobject)
	{
		factory = gameobject.AddComponent (typeof(ErlConnectFactory)) as ErlConnectFactory;
		factory.startTime ();
	}
	 
	/** 获得指定连接当前状态(返回0表示连接正常，1连接不存在，2连接存在但不可用) */
	public int getConnectStatus (string address, int port)
	{
		ErlConnect erlConnect = factory.checkInstance (address, port) as ErlConnect;
		if (erlConnect == null)
			return 1;
		if (!erlConnect.isActive)
			return 2;
		return 0;
	}
	
	/** 打开指定地址和端口号的连接 */
	public Connect getInstance (string address, int port)
	{
		return factory.getConnect (address, port);
	}

	//
	public Connect beginConnect (string address, int port, CallBackHandle handlel)
	{
		return factory.beginConnect (address, port, handlel);
	}

	/** 关闭并清空指定连接(如果发现有死连接也一并清空) */
	public void closeConnect (string address, int port)
	{
		factory.closeConnect (address, port);
	}
	/** 指定连接是否可用 */
	public Boolean isActive (string address, int port)
	{
		ErlConnect erlConnect = factory.checkInstance (address, port) as ErlConnect;
		if (erlConnect == null)
			return false;
		return erlConnect.isActive;
	}
	/** 获取指定连接的ping延迟时间 */
	public int getPing (string address, int port)
	{
		ErlConnect erlConnect = factory.checkInstance (address, port) as ErlConnect;
		if (erlConnect == null)
			return 0;
		return (int)erlConnect.ping;
	}

    public void ping()
    {
        factory.ping();
    }

	/** 关闭并清空所有连接 */
	public void closeAllConnects ()
	{
		factory.closeAllConnects ();
	} 
	
	/** 
	 * 向指定地址和端口的连接发送消息
	 * @param address-消息发送地址
	 * @param port-消息发送端口
	 * @param handle-执行回调的函数
	 * @param argus-执行回调的参数数组
	 * @param data-消息发送数据 ErlKVMessage对象
	 **/
	public void sendMessage (string address, int port, ErlKVMessage message, ReceiveFun handle, List<object> argus)
	{ 
//        if(!MiniConnectManager.IsRobot)
//        Debug.Log(message.Cmd + "," + message.toJsonString());

		ErlConnect connect = factory.getConnect (address, port) as ErlConnect;
		if (connect == null) {
			return;
		} 
		if (connect.isActive) {// 在连接可用的情况下才发送数据 
			if (handle != null)
            {
                long time = MiniConnectManager.now;/**
                DataAccess.getInstance ().access (connect, message, (c,o)=>{
                    time = MiniConnectManager.now - time;
                    if(time > maxTime)
                    {
                        maxTime = time;
                        maxCmd = message.Cmd;
                    }

                    if(MiniConnectManager.now - startTime > 1000)
                    {
                        startTime = MiniConnectManager.now;
                        if(MiniConnectManager.IsRobot)
                            Debug.LogError("max cmd use time:"+maxCmd+" : " + maxTime);
                    }
                    handle(c,o);
                    
                }, argus, DataAccess.TIMEOUT);
                */
                DataAccess.getInstance ().access (connect, message, handle, argus, DataAccess.TIMEOUT);
            }
			else
				DataAccess.getInstance ().access (connect, message, messageHandle, argus, DataAccess.TIMEOUT);
		} else {// 抛连接错误消息
			MonoBehaviour.print ("connect error!"+connect.isActive);
            if(GameManager.Instance != null)
            {
                GameManager.Instance.OnLostConnect(true);
            }
		}
	}  

    static long maxTime;
    static string maxCmd;
    static long startTime;
} 

