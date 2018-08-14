using System;
using UnityEngine;

/**
 * 充值 access端口
 * @author zhoujie
 * */
public class CashFPort:BaseFPort
{
	
	public CashFPort()
	{
	}
	/** 充值 access通讯 */
	public void cash (string userId,string platform,string server,int amount)
	{
		ErlKVMessage message = new ErlKVMessage("/yxzh/cash/order");
		string orderid = ServerTimeKit.getSecondTime ().ToString ();
		message.addValue ("orderid", new ErlString (orderid));//订单号
        message.addValue("game_platform", new ErlString(platform));//服ID
        message.addValue("game_server", new ErlString(server));//服ID
		message.addValue ("userid", new ErlString (userId));//用户账号
		message.addValue ("rmb", new ErlInt (0));
		message.addValue ("amount", new ErlInt (0));
        message.addValue("ext", new ErlString("mmcard"));
        //message.addValue ("amount", new ErlString (amount+""));//充值额
        //message.addValue ("sig", new ErlString ("0"));//签名
		access (message);
	}

    public override void read(ErlKVMessage message)
    {
        base.read(message);
    }

	public WWW httpCash (string userId,string platform,string server,int amount)
	{
		string orderid = ServerTimeKit.getSecondTime ().ToString ();
		return new WWW (ServerManagerment.Instance.lastServer.domainName + ":" + 870+"/yxzh/cash/order?orderid="+
		         orderid+"&userid="+userId+"&ext=mmcard&game_platform="+platform+"&game_server="+server+
		         "&rmb=0&amount=0");
	}
}

