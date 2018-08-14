using System;
 
/**
 * 累积登陆奖励信息接口
 * @author longlingquan
 * */
public class TotalLoginFPort:BaseFPort
{
	private CallBack callback;
	
	public TotalLoginFPort ()
	{
		
	}
	
	public void access (CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.TOTAL_LOGIN_INFO);
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		if (parseKVMsg (message) && callback != null)
			callback ();
	}
	//解析ErlKVMessgae
	public bool parseKVMsg (ErlKVMessage message)
	{
		//ErlType type = message.getValue ("msg") as ErlType;
		//if (type is ErlArray) {
		//	ErlArray array = type as ErlArray;
		//	int total = StringKit.toInt (array.Value[0].getValueString ());
		//	ErlArray AlreadyGet = array.Value[1] as ErlArray;
		//	// 当前已经领取的奖励
		//	int[] receivedAward = new int[AlreadyGet.Value.Length];
		//	for (int i=0; i < receivedAward.Length; i++) {
		//		receivedAward[i] = StringKit.toInt (AlreadyGet.Value[i].getValueString ());
		//	}
		//	TotalLoginManagerment.Instance.update (total, receivedAward);
		//	return true;
		//}
		//else {
		//	MonoBase.print (GetType () + "=============" + type.getValueString ());
		//}
		//return false;


		ErlType type = message.getValue ("msg") as ErlType;
		if (type is ErlArray) {
			ErlArray stateArray=type as ErlArray;
			string state =stateArray.Value[0].getValueString ();
			if (state == "ok") {
				ErlArray array = stateArray.Value[1] as ErlArray;
				ErlType awardType=array.Value[0] as ErlType;
				if(awardType is ErlInt||awardType is ErlByte){//老天天把后台数据更新到前台来
					TotalLoginManagerment.Instance.NeweverydayState=false;
					int total = StringKit.toInt (array.Value[0].getValueString ());
					ErlArray AlreadyGet = array.Value[1] as ErlArray;
					int[] receivedAward = new int[AlreadyGet.Value.Length];
					for (int i=0; i < receivedAward.Length; i++) {
						receivedAward[i] = StringKit.toInt (AlreadyGet.Value[i].getValueString ());
					}
					TotalLoginManagerment.Instance.update (total, receivedAward);
				}else if(awardType is ErlArray){//新天天送把后台数据更新到前台来（星期几登陆过，那个sid被领取过）
					TotalLoginManagerment.Instance.NeweverydayState=true;
					ErlArray loginWeek=array.Value[0] as ErlArray;
					ErlArray awardWeek=array.Value[1]as ErlArray;
					int total = StringKit.toInt (array.Value[2].getValueString ());
					TotalLoginManagerment.Instance.updateNewdayLoginDate(loginWeek);//更新那些星期登陆过
					TotalLoginManagerment.Instance.updateNewDayLoginAward(awardWeek);//更新那些星期的奖励领取过了
					TotalLoginManagerment.Instance.update (total, new int[0]);
				}
			}
			else if (state == "close") {
				TotalLoginManagerment.Instance.EverydayState = false;
				TotalLoginManagerment.Instance.NeweverydayState=false;
			}
			return true;
		}
		else {
			MonoBase.print (GetType () + "==" + type.getValueString ());
		}
		return false;
	}
} 

