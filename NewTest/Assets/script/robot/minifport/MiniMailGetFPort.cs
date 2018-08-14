using UnityEngine;
using System.Collections;

/**
 * 获得所有邮件接口
 * @author 汤琦
 * */
public class MiniMailGetFPort : MiniBaseFPort
{ 
	private CallBack callback;
	
	public void access (CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.MAIL_GET);
		access (message);
	}

	public void parseKVMsg (ErlKVMessage message)
	{
	}

	public override void read (ErlKVMessage message)
	{
//		ErlArray list = message.getValue ("msg") as ErlArray; 
//		if(list == null)
//		{
//			callback();
//			return;
//		}
//		MailManagerment.Instance.clearMail();//先清理邮箱
//		for (int i=0; i<list.Value.Length; i++) {
//			ErlList conList = list.Value [i]as ErlList;
//			int index = 0;
//			string uid = "0";
//			int src = 0;
//			int type = 0;
//			int stime = 0;
//			int etime = 0;
//			string theme;
//			string content;
//			Annex[] annex = null;
//			int state = 0;
//			uid = ((conList.Value[index] as ErlArray).Value[1]).getValueString();
//			src = StringKit.toInt(((conList.Value[++index] as ErlArray).Value[1]).getValueString());
//			type = StringKit.toInt(((conList.Value[++index] as ErlArray).Value[1]).getValueString());
//			stime = StringKit.toInt(((conList.Value[++index] as ErlArray).Value[1]).getValueString());
//			etime = StringKit.toInt(((conList.Value[++index] as ErlArray).Value[1]).getValueString());
//			theme = ((conList.Value[++index] as ErlArray).Value[1]).getValueString();
//			content = ((conList.Value[++index] as ErlArray).Value[1]).getValueString();
//			ErlList listc = (conList.Value[++index] as ErlArray).Value[1] as ErlList;
//			annex = parse (annex,listc);
//			state = StringKit.toInt(((conList.Value[++index] as ErlArray).Value[1]).getValueString());
//			MailManagerment.Instance.createMail(uid,src,type,stime,etime,theme,content,annex,state);
//		}
		parseKVMsg (message);
		if(callback != null)
		{
			callback();
		}
		
		
	}
	
	private Annex[] parse (Annex[] annex, ErlList list)
	{
		if(list == null)
			return null;
		annex = new Annex[list.Value.Length];
		for (int i = 0; i < list.Value.Length; i++) {
			annex[i] = new Annex();
			string str = ((list.Value[i] as ErlArray).Value[0] as ErlAtom).getValueString();
			if(str=="exp")
			{
				string strs = PrizeType.PRIZE_EXP + "," + "0" + "," + ((list.Value[i] as ErlArray).Value[1] as ErlType).getValueString();
				annex[i].exp = new PrizeSample(strs,',');
			}
			else if(str=="rmb")
			{
				string strs = PrizeType.PRIZE_RMB + "," + "0" + "," + ((list.Value[i] as ErlArray).Value[1] as ErlType).getValueString();
				annex[i].rmb = new PrizeSample(strs,',');
			}
			else if(str=="money")
			{
				string strs = PrizeType.PRIZE_MONEY + "," + "0" + "," + ((list.Value[i] as ErlArray).Value[1] as ErlType).getValueString();
				annex[i].money = new PrizeSample(strs,',');
			}
			else if(str=="pve")
			{
				string strs = PrizeType.PRIZE_PVE + "," + "0" + "," + ((list.Value[i] as ErlArray).Value[1] as ErlType).getValueString();
				annex[i].pve = new PrizeSample(strs,',');
			}
			else if(str=="pvp")
			{
				string strs = PrizeType.PRIZE_PVP + "," + "0" + "," + ((list.Value[i] as ErlArray).Value[1] as ErlType).getValueString();
				annex[i].pvp = new PrizeSample(strs,',');
			}
			else if(str=="prop")
			{
				string strs = (((list.Value[i] as ErlArray).Value[1] as ErlArray).Value[0]).getValueString() + "," + (((list.Value[i] as ErlArray).Value[1] as ErlArray).Value[1]).getValueString() + "," + (((list.Value[i] as ErlArray).Value[1] as ErlArray).Value[2]).getValueString();
				annex[i].prop = new PrizeSample(strs,',');
			}
		}
		return annex;
	} 
	
	
}
