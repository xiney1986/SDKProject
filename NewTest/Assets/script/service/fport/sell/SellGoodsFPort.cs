using UnityEngine;
using System.Collections;

public class SellGoodsFPort : BaseFPort
{
	private CallBack<int> callback;
	private ArrayList starSoulList =new ArrayList();
	
	public SellGoodsFPort ()
	{
		
	}
	
	public void sellGoods (string str, ArrayList list, CallBack<int> callback)
	{  
		this.starSoulList=list;
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.SELL_GOODS);  
		message.addValue ("sell", new ErlString (str));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		string str = (message.getValue ("msg") as ErlType).getValueString (); 

		if (StringKit.isNum (str)) {
			if(starSoulList!=null&&starSoulList.Count>0){
				StorageManagerment smanager=StorageManagerment.Instance;
				for(int i=0;i<starSoulList.Count;i++){
					(starSoulList[i] as Card).delStarSoulBoreByAll();
				}
			}
			callback (StringKit.toInt (str));
		} else {
            if (str == "money_full")
                MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("goldLimit"));
            else
			    MessageWindow.ShowAlert (str);
			if (callback != null)
				callback = null;
		}
	}
	
}
