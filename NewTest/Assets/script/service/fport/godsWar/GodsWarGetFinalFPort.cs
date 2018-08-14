using System.Collections.Generic;
/**
 * 神魔大战接口
 * @author gc
 * */
using System.Diagnostics;

public class GodsWarGetFinalFPort : BaseFPort
{
	CallBack callback;
  
	public void access (int type,int index,CallBack callback)
	{   
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.GODSWAR_GETFINALINFO);
		message.addValue("big_id",new ErlInt(type));
		message.addValue("yu_ming",new ErlInt(index));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{ 
		ErlType erl = message.getValue("msg") as ErlType;
		if(erl is ErlArray)
		{
			GodsWarFinalPoint user;
			GodsWarFinalUserInfo finaluser;
			List<GodsWarFinalUserInfo> fianlInfo;
			List<GodsWarFinalPoint> infos;

			ErlArray erlarry = erl as ErlArray;
			int pos=0;
			ErlArray array = erlarry.Value[pos++] as ErlArray;
			if(array.Value.Length>0)
			{
				fianlInfo = new List<GodsWarFinalUserInfo>();
				for(int i=0;i<array.Value.Length;i++)
				{
					finaluser = new GodsWarFinalUserInfo();
					finaluser.bytesReadFive(array.Value[i] as ErlArray);
					if(finaluser.uid!=null)
						fianlInfo.Add(finaluser);
				}
				if(fianlInfo!=null)
					GodsWarManagerment.Instance.shenMoUserlist = fianlInfo;
			}

			ErlArray tmp = erlarry.Value[pos++] as ErlArray;
			if(tmp.Value.Length>0)
			{
				infos = new List<GodsWarFinalPoint>();

				for (int j = 0; j < tmp.Value.Length; j++) {
					user = new GodsWarFinalPoint();
					user.bytesRead(tmp.Value[j] as ErlArray);
					if(user.localID!=0)
						infos.Add(user);
				}
				if(infos!=null)
					GodsWarManagerment.Instance.shenMoPointlist = infos;
			}

			if(callback!=null)
				callback();
		}
		else
		{
			MessageWindow.ShowAlert(erl.getValueString());
			if(callback!=null)
				callback=null;
		}

	}

}
