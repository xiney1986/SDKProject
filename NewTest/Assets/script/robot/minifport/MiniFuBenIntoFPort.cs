using System;
 
/**
 * 进入副本接口
 * @author longlingquan
 * */
public class MiniFuBenIntoFPort:MiniBaseFPort
{
	private const string INTO = "into_fb";//成功进入副本
	private const string CONTINUE = "continue_fb";//继续副本
	private const string ALREADY = "already_in_fb";//已经在副本中
	private const string KEY = "currentfb";//继续副本 键
	private const string LIMIT = "conditions_do_not_meet";
	private int intoSid;
	private CallBack<int> callback;
	private CallBack<int,int> callback2;

	
	//进入副本
	public void intoFuben (int sid, int arrayId, CallBack<int> callback)
	{ 
		this.intoSid = sid;
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.FUBEN_INTO);
		message.addValue ("fbid", new ErlInt (sid));//fuben sid
		message.addValue ("arrayid", new ErlInt (arrayId));//fuben sid
		access (message);
	}
	
	//继续副本
    public void toContinue (CallBack<int,int> callback)
	{ 
		this.callback2 = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.FUBEN_CONTINUE);  
		access (message);
	}
	 
	public override void read (ErlKVMessage message)
	{
        object msg = message.getValue("msg");
        if (msg == null)
        {
            callback(-1);
            return;
        }
		string str = (message.getValue ("msg") as ErlAtom).Value;   
		if (str == INTO) {
			callback (intoSid); 
			intoSid = 0;
		} else if (str == CONTINUE) { 
			// "currentfb":[0,0,[],"none"]
			//p_sid step 宝箱信息 布阵信息	
			
			ErlArray arr = message.getValue (KEY) as ErlArray;
			int p_index = StringKit.toInt (arr.Value [0].getValueString ());//继续副本 点 索引 0表示起点
			int step = StringKit.toInt (arr.Value [1].getValueString ());//点上事件索引  0表示此点上无事件或者事件都完成

            /**
			MissionInfoManager.Instance.mission.updateMission (p_index, step);
			ErlArray treasures = arr.Value [2] as ErlArray;
			int embattle = StringKit.toInt (arr.Value [3].getValueString ());
			ArmyManager.Instance.setActive (embattle);
			MissionInfoManager.Instance.mission.initTreasures (StringKit.toInt (treasures.Value [0].getValueString ()),
				StringKit.toInt (treasures.Value [1].getValueString ()));
            */

			if(callback2 != null)
			callback2 (p_index,step); 
		} else if (str == ALREADY) {
            callback (-2);
		} else if (str == LIMIT) {
			callback (-1);
		} else {
			MonoBase.print ("into fuben error!");
		}
	}
}

