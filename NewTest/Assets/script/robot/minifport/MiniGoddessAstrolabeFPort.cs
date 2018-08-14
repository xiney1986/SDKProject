using UnityEngine;
using System.Collections;

/**
 * 女神星盘端口
 * @author 陈世惟
 * */
public class MiniGoddessAstrolabeFPort : MiniBaseFPort
{

	private CallBack callback;
	private const int TYPE_GET = 1;//获得信息
	private const int TYPE_OPEN = 2;//激活星星
	private int sendType;
	
	public MiniGoddessAstrolabeFPort ()
	{
		
	}

	//获取星盘信息
	public void getInfo (CallBack callback)
	{
		this.callback = callback;
		sendType = TYPE_GET;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GODDESSASTROLABE_GET_INFO);
		access (message);
	}

	//激活星星
	public void openStar (int sid, CallBack callback)
	{
		this.callback = callback;
		sendType = TYPE_OPEN;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GODDESSASTROLABE_OPEN_STAR);
		message.addValue ("sid", new ErlInt (sid));
		access (message);
	}

	public void initInfoByServer (ErlKVMessage message)
	{
	}

	//{"empty":20,"star_point":[3,2,1],"attr":[{middle:[[[integer,magic],20]]},{front:[[[integer,attack],20]]},{all:[[[number,hp],20]]}],"func":[{friend:100},{equip_storage:100},{card_storage:100}]}
	public override void read (ErlKVMessage message)
	{
//		base.read (message);
//		if(sendType == TYPE_GET) {
//			GoddessAstrolabeManagerment.Instance.initInfoByServer(message);
//			callback();
//		}
//		else if(sendType == TYPE_OPEN) {
//			string str = (message.getValue ("msg") as ErlAtom).Value;
//
//			if(str == "ok") {
//				callback();
//			}
//			else {
//				MonoBase.print (GetType () + "============error:"+str);
//			}
//		}
		initInfoByServer (message);
		if (callback != null)
			callback ();
	}
}
