using System;
using UnityEngine;
 
/**
 * 创建角色接口
 * @author longlingquan
 * */
public class MiniCreateRoleFPort:MiniBaseFPort
{
	 
	private CallBack initUser;
	
	public MiniCreateRoleFPort ()
	{
		
	}
	 
	public void access (string name, int style, int star, int sid, CallBack callback)
	{ 
		this.initUser = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.CREATE_ROLE);  
		message.addValue ("name", new ErlString (name));//角色名字
		message.addValue ("style", new ErlInt (style));//角色头像编号
        message.addValue("star", new ErlInt(star));//角色星座
		//1-8
		message.addValue ("sid", new ErlInt (sid));//角色sid
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{ 
//		string str = (message.getValue ("msg") as ErlAtom).Value; 
//		if (str == FPortGlobal.ROLE_CREATE_ROLE_OK || str == FPortGlobal.ROLE_ALREADY_CREATE_ROLE) {
//			initUser (); 
//			UiManager.Instance.destoryWindowByName("roleNameWindow");
//			UiManager.Instance.destoryWindowByName("chooseHoroscopesWindow");
//			UiManager.Instance.destoryWindowByName("selectServerWindow");
//		} else if (str == FPortGlobal.SYSTEM_INFO_ERROR) { 
//			MonoBehaviour.print ("CREATE_ROLE_INFO_ERROR");
//		} else if (str == FPortGlobal.REGISTER_USER_EXIST) {//用户名存在
//			UiManager.Instance.openDialogWindow<MessageWindow>((window)=>{
//				window.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0040"), "", LanguageConfigManager.Instance.getLanguage ("s0071"), null);
//			});
//		}
		if(initUser != null) initUser ();
	}
}

