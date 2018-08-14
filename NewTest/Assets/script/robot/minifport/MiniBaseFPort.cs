using System;
using System.Collections.Generic;
/**
 * 基础通讯接口
 * 消息的收发 和 处理(需要子类重写)
 * @author longlingquan
 * */
public class MiniBaseFPort
{   
    public ConnectManager cm;
    
    public void access (ErlKVMessage message)
    {  
//		send (message);
		cm.sendMessage (MiniConnectManager.ip, MiniConnectManager.port, message, receive, null); 
    }
    
    public void send (ErlKVMessage message)
    {
		cm.sendMessage (MiniConnectManager.ip, MiniConnectManager.port, message, null, null); 
    }
    
    public void receive (Connect c, object obj)
    { 
        read (obj as ErlKVMessage); 
    }
    
    public virtual void read (ErlKVMessage message)
    {
        
    }
} 

