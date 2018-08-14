using System;
using UnityEngine;
 
public class ErlConnectFactory:ConnectFactory
{
	 
	
	//打开指定地址的连接
	override public Connect openConnect(string localAddress,int localPort)
	{   
		ErlConnect c=new ErlConnect(); 
		c.open(localAddress,localPort); 
		c.portHandler=DataAccess.getInstance(); 
		return c;
	}
	
	/**ping方法*/
    override public void ping()
	{ 
		long time=TimeKit.getMillisTime();
		ErlConnect c;
		Connect[] array=connectArray.ToArray();
		for(int i=array.Length-1;i>=0;i--)
		{  
			c=array[i] as ErlConnect;
			if(c.Active)
			{
				c.PingTime=time;
				//发送ping值
				ErlKVMessage message=new ErlKVMessage("echo");
				DataAccess.getInstance().access(c,message,pingHandle,null,DataAccess.TIMEOUT);
			}
		}
	}

	
	/**执行ping通信返回消息的执行方法
	    * */
	protected void pingHandle(Connect erlConnect,object erlMessage)
	{	   
		long time=TimeKit.getMillisTime();
		erlConnect.ping=time-erlConnect.PingTime; 
        /**
        pingTime += erlConnect.ping;
        pingCount++;

        if (time - st > 1000)
        {
            st = time;
            Debug.LogError("ping:"+(pingTime / pingCount)+"," + pingCount);
            pingTime = pingCount = 0;
        }
        */
	}

    void Update2()
    {
        if (pingBack)
            return;
        ErlConnect c;
        for(int i = 0; i < connectArray.Count; i++)
        {  
            c=connectArray[i] as ErlConnect;
            if(c.Active)
            {
                ping();
                return;
            }
        }

    }
    bool pingBack;

    static int pingCount;
    static long pingTime;
    static long st;
} 

