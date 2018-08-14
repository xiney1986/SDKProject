using UnityEngine;
using System.Collections;

public class MiniFport
{
    public static void access (ConnectManager cm,string message,CallBack<ErlKVMessage> callback)
    {  

        access(cm,new ErlKVMessage(message),callback);
    }

    public static void access (ConnectManager cm,ErlKVMessage message,CallBack<ErlKVMessage> callback)
    {  
        cm.sendMessage (MiniConnectManager.ip, MiniConnectManager.port, message, (c,o)=>{
            if(callback != null)
            {
                callback(o as ErlKVMessage);
            }
        }, null); 
    }
    
    public static void send (ConnectManager cm,ErlKVMessage message)
    {
        cm.sendMessage (MiniConnectManager.ip, MiniConnectManager.port, message, null, null); 
    }
}
