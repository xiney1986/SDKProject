using System;
using System.Net.Sockets;
 
/**
 * 消息管理器
 * 主要处理系统级别消息
 * @author longlingquan
 * */
public class MessageManagerment
{
	public const int TYPE_CONNECT_ERROR = 1;//服务器连接异常
	public const int TYPE_CONNECT_OPEN_ERROR = 2;//服务器建立连接异常


	public MessageManagerment ()
	{ 

	}
	
	public static MessageManagerment Instance {
		get{return SingleManager.Instance.getObj("MessageManagerment") as MessageManagerment;}
	}
	
	//重新连接
	public void relogin ()
	{
		
	}
	
	private void loginOut (MessageHandle msg)
	{
		if(GameManager.Instance != null)
		{
			GameManager.Instance.OnLostConnect(false);
		}
	}
	
	
} 

