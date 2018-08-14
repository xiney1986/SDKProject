using UnityEngine;
using System.Collections;

/**
 * 召唤兽学习技能接口
 * @author 汤琦
 * */
public class BeastSkillsUpDateFPort : BaseFPort
{
	private CallBack<string> callback;
	
	public BeastSkillsUpDateFPort ()
	{
		
	}
	//fooduid格式："uid1,uid2,uid3"
	public void access (string beastuid, string fooduid, CallBack<string> back)
	{ 
		this.callback = back;
		ErlKVMessage message = new ErlKVMessage (FrontPort.BEAST_SKILL_UPDATE); 
		message.addValue ("beastuid", new ErlString (beastuid));
		message.addValue ("fooduid", new ErlString (fooduid));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		string str = (message.getValue ("msg") as ErlAtom).Value;
		if (str == FPortGlobal.INTENSIFY_SUCCESS) {
			string uid = (message.getValue ("main_uid") as ErlType).getValueString ();
			//刷新卡片仓库
//			SerializeStorageFPort serialize = FPortManager.Instance.getFPort("SerializeStorageFPort") as SerializeStorageFPort;
//			serialize.access("card_storage",callback,uid);
			if (callback != null) {
				callback (uid);
				callback = null;
			}
		} else if (str == FPortGlobal.INTENSIFY_CARD_NONENTITY) {//卡片不存在
			
		} else if (str == FPortGlobal.INTENSIFY_MAINCARD_ERROR) {//不能吃主卡
			
		} else if (str == FPortGlobal.INTENSIFY_OUTCASH) {//缺钱
			
		}
		
	}
}
