using UnityEngine;
using System.Collections;

/**
 * 账号服务
 * @author huangzhenghan
 * */
public class AccountService:BaseFPort
{
	private const string OFFLINE = "offline";//顶号下线
	private const string SERVERDOWN="serverDown";//停服

	public AccountService ()
	{
		
	}

	public override void read (ErlKVMessage message)
	{ 
		string type = (message.getValue ("msg") as ErlAtom).Value;
		if (type == OFFLINE) {
			//下线操作 
			if(GameManager.Instance != null)
			{
#if UNITY_IPHONE || UNITY_STANDALONE_OSX
				UiManager.Instance.openDialogWindow<SystemMessageWindow>((win)=>{
					win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("GameManager_accountRelogin"), "",LanguageConfigManager.Instance.getLanguage ("GameManager_accountError"), msgcallback);
				});
#else
				UiManager.Instance.openDialogWindow<SystemMessageWindow>((win)=>{
					win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("GameManager_accountOver"), LanguageConfigManager.Instance.getLanguage ("GameManager_accountRelogin"), LanguageConfigManager.Instance.getLanguage ("GameManager_accountError"), msgcallback);
				});
#endif
			}
		}else if(type==SERVERDOWN){

			ServerManagerment.Instance.lastServer.isRunning=false;

			//停服处理
			#if UNITY_IPHONE || UNITY_STANDALONE_OSX
			UiManager.Instance.openDialogWindow<SystemMessageWindow>((win)=>{
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), "",LanguageConfigManager.Instance.getLanguage ("GameManager_serverDown"), msgcallback2);
			});
			#else
			UiManager.Instance.openDialogWindow<SystemMessageWindow>((win)=>{
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, LanguageConfigManager.Instance.getLanguage ("GameManager_serverDown"), msgcallback2);
			});
			#endif

		}
	}

	void msgcallback2(MessageHandle msg){
	
		GameManager.Instance.logOut();
		return;

		
	}



	void msgcallback(MessageHandle msg){
#if UNITY_IPHONE ||UNITY_STANDALONE_OSX
		GameManager.Instance.logOut();
		return;
#endif
		if(msg.buttonID==MessageHandle.BUTTON_LEFT)
		{
			#if UNITY_ANDROID
			Application.Quit ();
			return;
			#endif
	
		}else{
			GameManager.Instance.logOut();
		}

	}
}

