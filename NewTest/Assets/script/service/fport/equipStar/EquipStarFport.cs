/// <summary>
/// 装备升星通信
/// </summary>
public class EquipStarFport:BaseFPort
{
	
	private CallBack<bool> callback;
	public void sendMessage (string equipUid,CallBack<bool> back)
	{ 
		this.callback = back;
		ErlKVMessage message = new ErlKVMessage (FrontPort.EQUIP_STAR); 
		message.addValue ("equip_uid", new ErlString (equipUid));
		access (message);
	}
	public override void read (ErlKVMessage message)
	{
		ErlType str = message.getValue ("msg") as ErlType;
		string starResult = str.getValueString ();
		if (starResult == "ok") {
			if(callback !=null){
				callback(true);
			}
		}
		else{
			if(callback != null)
				callback (false);
		}
	}
}
