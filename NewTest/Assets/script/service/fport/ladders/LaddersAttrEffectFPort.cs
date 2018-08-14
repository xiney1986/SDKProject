
using System;
/// <summary>
/// 天梯中属性加成的请求
/// </summary>
public class LaddersAttrEffectFPort:BaseFPort
{
	public LaddersAttrEffectFPort ()
	{
	}
	
	private CallBack callback;
	public void apply(CallBack _callback)
	{  		
		this.callback = _callback;	
		ErlKVMessage message = new ErlKVMessage (FrontPort.LADDERS_ATTR_EFFECT);	
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		//"title":[[prestige,140025,0],[ladder_rank,140027,1400000000]],
		//"effect":[[140025,[[[integer,hp],5000],[[integer,attack],1000],[[integer,defense],1000],[[integer,magic],1000],[[integer,agile],1000]]],
		//[140027,[[[integer,hp],5000],[[integer,attack],1000],[[integer,defense],1000],[[integer,magic],1000],[[integer,agile],1000]]]]

		if(message==null)
		{
			UnityEngine.Debug.LogError ("Title Attr Request Fail!");
		}

		int id_ladder_titles=0;
		int id_ladder_medal=0;

		ErlArray titles = message.getValue ("title") as ErlArray;
		ErlArray effectList = message.getValue ("effect") as ErlArray;

		ErlArray titleItem;
		string name=string.Empty;
		int dueTime=0;
		for(int j=0,length=titles.Value.Length;j<length;j++)
		{
			titleItem=titles.Value[j] as ErlArray;
			name=titleItem.Value[0].getValueString();
			switch(name)
			{
			case "prestige":
				id_ladder_titles=StringKit.toInt(titleItem.Value[1].getValueString());
				dueTime=StringKit.toInt(titleItem.Value[2].getValueString());
				LaddersManagement.Instance.TitleAttrEffect.DueTime=dueTime;
				break;
			case "ladder_rank":
				id_ladder_medal=StringKit.toInt(titleItem.Value[1].getValueString());
				dueTime=StringKit.toInt(titleItem.Value[2].getValueString());
				//如果奖章时间已经到期 则把奖章id设置0，相当于没有奖章，下面的加成效果自然也不能执行
				if(dueTime<ServerTimeKit.getSecondTime())
				{
					id_ladder_medal=0;
				}
				LaddersManagement.Instance.currentPlayerMedalSid=id_ladder_medal;				
				LaddersManagement.Instance.MedalAttrEffect.DueTime=dueTime;
				break;
			}
		}

		ErlArray effectItem;
		int id;
		for(int i=0,length=effectList.Value.Length;i<length;i++)
		{
			effectItem=effectList.Value[i] as ErlArray;
			id=StringKit.toInt(effectItem.Value[0].getValueString());

			if(id==id_ladder_titles)
			{
				LaddersManagement.Instance.TitleAttrEffect.initInfoByServer(effectItem.Value[1] as ErlArray);
			}else if(id==id_ladder_medal)
			{
				LaddersManagement.Instance.MedalAttrEffect.initInfoByServer(effectItem.Value[1] as ErlArray);
			}
		}

		if (callback != null)
		{
			callback();
			callback = null;
		}
	}
}


