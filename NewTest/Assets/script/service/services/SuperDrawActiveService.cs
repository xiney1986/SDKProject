using UnityEngine;
using System.Collections;

/**
 * gm修改活动
 * @author huangzhenghan
 * */
public class SuperDrawActiveService:BaseFPort
{
	SuperDrawManagerment manager;

	public SuperDrawActiveService ()
	{
		
	}

	public override void read (ErlKVMessage message)
	{ 
		manager = SuperDrawManagerment.Instance;
		if(manager.superDraw==null)
			manager.superDraw = new SuperDraw();
		if (message.getValue ("ticket") != null) {
			int num = StringKit.toInt ((message.getValue ("ticket") as ErlType).getValueString ());
			manager.superDraw.poolNum = num;
			sendMessage();
		}
		if (message.getValue ("log") != null) {
			ErlArray arr = message.getValue ("log") as ErlArray;
			SuperDrawAudio aduio = new SuperDrawAudio();
			int index = 0;
			aduio.serverName = arr.Value[index++].getValueString();
			aduio.playerName = arr.Value[index++].getValueString();
			aduio.DrawNum    = StringKit.toInt(arr.Value[index++].getValueString());

			manager.audio = aduio;
			sendMessage();
		}
		if (message.getValue ("score") != null) {
			int score = StringKit.toInt ((message.getValue ("score") as ErlType).getValueString ());
			manager.superDraw.score = score;
			sendMessage();
		}
		if (message.getValue ("times") != null) {
			int times = StringKit.toInt ((message.getValue ("times") as ErlType).getValueString ());
			manager.superDraw.canUseNum = times;
			sendMessage();
		}
	}
	public void sendMessage()
	{
		manager.isAudio = true;
	}
}

