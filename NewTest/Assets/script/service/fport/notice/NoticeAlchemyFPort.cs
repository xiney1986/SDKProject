using UnityEngine;
using System.Collections;

public class NoticeAlchemyFPort : BaseFPort
{

	private CallBack<long,int> callback;
	private int t;
	
	public void access (CallBack<long,int> callback,int type)
	{   
		t=type;
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.NOTICE_ALCHEMY_BUY);
		if(t==0){
			message.addValue("num", new ErlInt(1));	
		}else if(t==1){
			message.addValue("num", new ErlInt(10));	
		}
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;
		if (type is ErlArray) {
			ErlArray stateArray=type as ErlArray;
			string state =stateArray.Value[0].getValueString();
			if(t==0){
				if (state == "0") {
					NoticeManagerment.Instance.setAlchemyNum (NoticeManagerment.Instance.getAlchemyNum () + 1);
					if (callback != null)
						callback (1,0);
				} else if (state == "1") {
					NoticeManagerment.Instance.setAlchemyNum (NoticeManagerment.Instance.getAlchemyNum () + 1);
					if (callback != null)
						callback (2,0);
				} else {
					MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("AlchemyContent03"));
					if (callback != null)
						callback (0,0);
				}
			}else{
				int index=StringKit.toInt(state);
				if(index<=10&&index>=0){
					NoticeManagerment.Instance.setAlchemyNum (NoticeManagerment.Instance.getAlchemyNum () + 10);
					if(callback!=null){
						long num=StringKit.toLong(stateArray.Value[1].getValueString());
						callback(num,index);
					}else{
						MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage ("AlchemyContent03"));
						if (callback != null)
							callback (-1,0);
					}
				}
			}

		}else {
			MessageWindow.ShowAlert (type.getValueString ());
			if (callback != null)
				callback = null;
		}
	}
}
