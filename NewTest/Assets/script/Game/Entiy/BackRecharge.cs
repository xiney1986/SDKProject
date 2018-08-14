using UnityEngine;
using System.Collections;

public class BackRecharge
{
	public int sid = 0;//sid
	public int num = 0;//回归累积充值的充值金额
	public int state = BackRechargeState.receve;// 领取状态//


	public BackRecharge(int id,int num)
	{
		this.sid = id;
		this.num = num;
	}

	public void setNum(int num)
	{
		this.num = num;
	}

	public RechargeSample getRechargeSample()
	{
		return BackRechargeConfigManager.Instance.getRechargeSampleByID (sid);
	}

	public void setState(int state)
	{
		this.state = state;
	}
}

public class BackRechargeState
{
	public static int recevied = 1;// 已领取//
	public static int receve = 0;// 未领取//
}
