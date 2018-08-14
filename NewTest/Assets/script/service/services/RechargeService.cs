using System;

/**
 * 任务服务
 * @author huangzhenghan
 * */
public class RechargeService:BaseFPort
{
	
	public RechargeService ()
	{
		
	}

	public override void read (ErlKVMessage message)
	{ 
		ErlArray array = message.getValue ("msg") as ErlArray;
		ErlArray array1;
		for (int i=0; i<array.Value.Length; i++) {
			array1 = array.Value [i] as ErlArray;
			int sid=StringKit.toInt (array1.Value [0].getValueString ());
			int num=StringKit.toInt (array1.Value [1].getValueString ());
			int count=StringKit.toInt (array1.Value [2].getValueString ());
			Recharge recharge=NoticeActiveManagerment.Instance.getActiveInfoBySid (sid) as Recharge;
			if(recharge!=null){
				recharge.setCount (count);
				recharge.setNum (num); 
			}
		}
		RechargeManagerment.Instance.updateRecharge ();
	}
}

