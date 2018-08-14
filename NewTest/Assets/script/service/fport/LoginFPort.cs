using System;
using UnityEngine;
 
/**
 * 登录接口
 * @author longlingquan
 * */
public class LoginFPort:BaseFPort
{
	 
	private CallBack initUser;

	public LoginFPort ()
	{ 
		
	}
	
	public void login (string platform, string server, string name, CallBack callback)
	{   
		this.initUser = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.LNGIN);  
		message.addValue ("platform", new ErlInt (StringKit.toInt(platform)));//平台
		message.addValue ("server", new ErlInt (StringKit.toInt(server)));//服务器
		message.addValue ("userid", new ErlString (name));//账号
		access (message);
	}

	//下线
	public void closeContect ()
	{
		ErlKVMessage message = new ErlKVMessage (FrontPort.CLOSECONTECT);  
		send (message);
	}

	public void login (string url, int sid, CallBack callback)
	{   
		this.initUser = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.LNGIN);  
		message.addValue ("url", new ErlString (url));
		message.addValue ("server", new ErlInt (sid));
		access (message);
	}

	public void loginGM (string id,int sid,CallBack callback)
	{   
		this.initUser = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.LNGINGM);  
		message.addValue ("userid", new ErlString (id));
	//	message.addValue ("test", new ErlString ("test"));
		message.addValue ("server", new ErlInt (sid));
		access (message);
	}
	public void login (string uid, string vip, string platform, string serverid, string  time, string   inviteuser, string sig, CallBack callback)
	{   
		this.initUser = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.LNGIN);  
		message.addValue ("userid", new ErlString (uid));
		message.addValue ("vip", new ErlString (vip));
		message.addValue ("time", new ErlString (time));	
		message.addValue ("platform", new ErlString (platform));
		message.addValue ("server", new ErlString (serverid));
		message.addValue ("inviteuser", new ErlString (inviteuser));
		//message.addValue ("sig", new ErlString (sig));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{ 
        ErlAtom atom = message.getValue ("msg") as ErlAtom;
        string str = atom != null ? atom.Value : ""; 
		if (str == FPortGlobal.LOGIN_LOGIN_OK) { 
			Debug.LogWarning ("LOGIN_LOGIN_OK");
			initUser (); 
		} else if (str == FPortGlobal.SYSTEM_INFO_ERROR) { 
			Debug.LogWarning ("LOGIN_INFO_ERROR");
		} else if (str == FPortGlobal.LOGIN_CREATE_USER_OK) { 
			Debug.LogWarning ("LOGIN_CREATE_USER_OK");
			UiManager.Instance.openWindow<TitlesWindow> ();
			
		} else if (str == FPortGlobal.LOGIN_NO_USER) { 
			Debug.LogWarning ("LOGIN_NO_USER");
		} else if (str == FPortGlobal.LOGIN_PASSWORD_ERROR) {
			Debug.LogWarning ("LOGIN_PASSWORD_ERROR");
		} else if (str == FPortGlobal.LOGIN_NO_ROLE) { 
			Debug.LogWarning("LOGIN_NO_ROLE");
			UiManager.Instance.openWindow<TitlesWindow> ();
			
		} else if (str == FPortGlobal.LOGIN_RELOGIN_OK) { 
			Debug.LogWarning ("LOGIN_RELOGIN_OK"); 
			initUser ();
		} else if (str == FPortGlobal.LOGIN_COUNT) {
			Debug.LogWarning("LOGIN_COUNT");
			SystemMessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("serverState01"));
			ConnectManager.manager().closeConnect(ServerManagerment.Instance.lastServer.domainName, ServerManagerment.Instance.lastServer.port);
		} else if (str == FPortGlobal.LOGIN_CLOSE) {
			Debug.LogWarning("LOGIN_CLOSE");
			SystemMessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("serverState02"));
			ConnectManager.manager().closeConnect(ServerManagerment.Instance.lastServer.domainName, ServerManagerment.Instance.lastServer.port);
		} else if (str == FPortGlobal.LOGIN_TRUST) {
			Debug.LogWarning("LOGIN_TRUST");
			SystemMessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("serverState03"));
			ConnectManager.manager().closeConnect(ServerManagerment.Instance.lastServer.domainName, ServerManagerment.Instance.lastServer.port);
		}else if (str == FPortGlobal.LOGIN_SIGERROR) {
			Debug.LogWarning("LOGIN_SIGERROR");
			SystemMessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("serverState04"));
			ConnectManager.manager().closeConnect(ServerManagerment.Instance.lastServer.domainName, ServerManagerment.Instance.lastServer.port);
		}else if (str == FPortGlobal.LOGIN_LIMIT) {
			Debug.LogWarning("LOGIN_LIMIT");
			SystemMessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("serverState05"));
			ConnectManager.manager().closeConnect(ServerManagerment.Instance.lastServer.domainName, ServerManagerment.Instance.lastServer.port);
		}else if (str == FPortGlobal.LOGIN_CREATE) {
			Debug.LogWarning("LOGIN_CREATE");
			SystemMessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("serverState06"));
			ConnectManager.manager().closeConnect(ServerManagerment.Instance.lastServer.domainName, ServerManagerment.Instance.lastServer.port);
		}else if (str == FPortGlobal.LOGIN_USER_DISABLE) {
			Debug.LogWarning("LOGIN_USER_DISABLE");
			SystemMessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("serverState07"));
			ConnectManager.manager().closeConnect(ServerManagerment.Instance.lastServer.domainName, ServerManagerment.Instance.lastServer.port);
		}else{
			Debug.LogWarning("LOGIN_OTHER");
			//其他报错默认提示
			SystemMessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("serverState02"));
            ConnectManager.manager().closeConnect(ServerManagerment.Instance.lastServer.domainName, ServerManagerment.Instance.lastServer.port);
        }
	}
} 

