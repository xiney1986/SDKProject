using UnityEngine;
using System.Collections;

public class ResolveGoodsFPort : BaseFPort
{
	private CallBack callback;
	private ArrayList starSoulList =new ArrayList();

	public ResolveGoodsFPort ()
	{
		
	}
	
	public void resolveGoods (string str,ArrayList list, CallBack callback)
	{  
		this.starSoulList=list;
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.RESOLVE_GOODS);  
		message.addValue ("destroy", new ErlString (str));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlType arr = message.getValue ("msg") as ErlType;
	    ErlArray cur = null;
	    if (arr is ErlArray)
	        cur = arr as ErlArray;
	    else
	    {
            MessageWindow.ShowAlert(arr.getValueString());
            if (callback != null)
                callback = null;
	        return;
	    }
	    ErlArray temp;
		ResolveWindow.resolveResult.Clear ();
		for (int i=0; i<cur.Value.Length; i++) {
			temp = cur.Value[i] as ErlArray;
			if(temp.Value[0].getValueString() == "money"){
				ResolveWindow.resolveResult.Add(new PrizeSample (PrizeType.PRIZE_MONEY, 0, StringKit.toInt(temp.Value[1].getValueString())));
			}else{
				ResolveWindow.resolveResult.Add(new PrizeSample (PrizeType.PRIZE_PROP, StringKit.toInt(temp.Value[0].getValueString()), StringKit.toInt(temp.Value[1].getValueString())));
			}
		}
		if (arr as ErlArray != null) {
			if(starSoulList!=null&&starSoulList.Count>0){
				StorageManagerment smanager=StorageManagerment.Instance;
				for(int i=0;i<starSoulList.Count;i++){
					(starSoulList[i] as Card).delStarSoulBoreByAll();
				}
			}
			callback ();
		} else {
			MessageWindow.ShowAlert (arr.getValueString ());
			if (callback != null)
				callback = null;
		}
		
	}
}
