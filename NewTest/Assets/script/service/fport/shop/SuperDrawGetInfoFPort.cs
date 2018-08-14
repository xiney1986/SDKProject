using UnityEngine;
using System.Collections;
/// <summary>
/// 获取超级奖池信息接口
/// </summary>
public class SuperDrawGetInfoFPort : BaseFPort {
	private CallBack callback;

	/// <summary>
	/// 活动sid
	/// </summary>
	private int sid = 0;
	public SuperDrawGetInfoFPort(){}
	public void access (int sid, CallBack callback)
	{  
		this.sid = sid;
		this.callback = callback;
        ErlKVMessage message = new ErlKVMessage(FrontPort.GET_SUPERDRAW_INFO);
        message.addValue("sid", new ErlInt(sid));
		access (message);
	}
	public override void read (ErlKVMessage message)
	{
		ErlType msgErl = message.getValue ("msg") as ErlType;
		if(msgErl is ErlArray)
		{
			SuperDrawManagerment manager = SuperDrawManagerment.Instance;
			manager.superDraw = new SuperDraw();
			SuperDraw superDraw = manager.superDraw;
			if(superDraw.list!=null)
				superDraw.list.Clear();
			int index = 0;
			ErlArray erlArray = msgErl as ErlArray;
			superDraw.canUseNum = StringKit.toInt(erlArray.Value[index++].getValueString());
			superDraw.score = StringKit.toInt(erlArray.Value[index++].getValueString());
			superDraw.poolSid = StringKit.toInt(erlArray.Value[index++].getValueString());
			superDraw.poolNum = StringKit.toInt(erlArray.Value[index++].getValueString());
			ErlType totalLog =erlArray.Value[index++] as ErlType;
			if( totalLog is ErlArray)
			{
				ErlArray s = totalLog as ErlArray;

				for(int i=0;i<s.Value.Length;i++)
				{
					int _index = 0;
					ErlArray array = s.Value[i] as ErlArray;
					SuperDrawAudio audio = new SuperDrawAudio();
					audio.serverName = array.Value[_index++].getValueString();
					audio.playerName = array.Value[_index++].getValueString();
					audio.DrawNum    = StringKit.toInt(array.Value[_index++].getValueString());
					superDraw.list.Add(audio);
				}
			}
			if(callback !=null)
			{
				callback();
				callback = null;
			}
		} else {
			UiManager.Instance.createMessageLintWindow(msgErl.getValueString());
		}
	}
}
