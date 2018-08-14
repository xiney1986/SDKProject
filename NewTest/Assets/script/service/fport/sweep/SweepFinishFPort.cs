
using System;

public class SweepFinishFPort : BaseFPort
{ 
	private CallBack callback;

	public void finish (CallBack callback)
	{  
		
		this.callback = callback;		
		ErlKVMessage message = new ErlKVMessage (FrontPort.SWEEP_FINISH);	
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		ArmyManager.Instance.getArmy (SweepManagement.Instance.useArrayID).state = 0;
		//不关心返回
		if (callback != null) {
			callback ();
			callback = null;
		}
	}
}



