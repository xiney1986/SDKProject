using System;
using UnityEngine;
 
/**
 * 登录接口
 * @author longlingquan
 * */
public class MiniLoginFPort : MiniBaseFPort
{
	 
	private CallBack<string> initUser;
 
	
    public void login (string platform, string server, string name, CallBack<string>  callback)
	{   
		this.initUser = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.LNGIN);  
		message.addValue ("platform", new ErlInt (StringKit.toInt(platform)));//平台
		message.addValue ("server", new ErlInt (StringKit.toInt(server)));//服务器
		message.addValue ("userid", new ErlString (name));//账号
		access (message);
	}

	public override void read (ErlKVMessage message)
	{ 
		string str = (message.getValue ("msg") as ErlAtom).Value;  
//		if (str == FPortGlobal.LOGIN_LOGIN_OK) { 
//			MonoBehaviour.print ("LOGIN_LOGIN_OK");
//			initUser (); 
//		} else if (str == FPortGlobal.SYSTEM_INFO_ERROR) { 
//			MonoBehaviour.print ("LOGIN_INFO_ERROR");
//		} else if (str == FPortGlobal.LOGIN_CREATE_USER_OK) { 
//			MonoBehaviour.print ("LOGIN_CREATE_USER_OK"); 
//			
//		} else if (str == FPortGlobal.LOGIN_NO_USER) { 
//			MonoBehaviour.print ("LOGIN_NO_USER");
//		} else if (str == FPortGlobal.LOGIN_PASSWORD_ERROR) {
//			MonoBehaviour.print ("LOGIN_PASSWORD_ERROR");
//		} else if (str == FPortGlobal.LOGIN_NO_ROLE) { 
//			MonoBehaviour.print ("LOGIN_NO_ROLE");
//		} else if (str == FPortGlobal.LOGIN_RELOGIN_OK) { 
//			MonoBehaviour.print ("LOGIN_RELOGIN_OK"); 
//			initUser ();
//		} 
		if (initUser != null)
			initUser (str);
	}
} 

