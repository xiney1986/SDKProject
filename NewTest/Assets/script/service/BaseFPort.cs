using System;
using System.Collections.Generic;
/**
 * 基础通讯接口
 * 消息的收发 和 处理(需要子类重写)
 * @author longlingquan
 * */
public class BaseFPort
{   
    protected bool lockUI = true;

	CallBack callback;
	public BaseFPort ()
	{
		 
	}

    public void access (ErlKVMessage message)
    {  
		if (lockUI) {
			MaskWindow.NetLock();
		}
        ConnectManager.manager ().sendMessage (ServerManagerment.Instance.lastServer.domainName, ServerManagerment.Instance.lastServer.port, message, receive, null); 
    }
	
	public void send (ErlKVMessage message)
	{
		ConnectManager.manager ().sendMessage (ServerManagerment.Instance.lastServer.domainName, ServerManagerment.Instance.lastServer.port, message, null, null); 
	}
	
	public void receive (Connect c, object obj)
    { 
        if (lockUI && !GetType().Name.EndsWith("Service"))
        {
            MaskWindow.NetUnlock();
        }

        try
        {
            read (obj as ErlKVMessage); 
        }catch(System.Exception ex)
        {
            string str = ex.Message+"\n" + ex.Data+"\n" + ex.StackTrace;
            UnityEngine.Debug.LogError(str);
			MaskWindow.UnlockUI();
            throw ex;
        }
	}
	
	public virtual void read (ErlKVMessage message)
	{
		
	}

    public static bool flag = false;
} 

