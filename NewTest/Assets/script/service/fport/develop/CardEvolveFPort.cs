using UnityEngine;
using System;

/**
 * 卡片进化接口
 * @author 汤琦
 * */
public class CardEvolveFPort : BaseFPort
{
	private CallBack<string> callbacki;
	
	public CardEvolveFPort ()
	{
		
	} 
	//技能ability,格式为（卡片UID,主卡被动技能;副卡UID,副卡被动技能）
	public void access (string mainUID, string foodUID, string ability, CallBack<string> back)
	{ 
		this.callbacki = back;
		ErlKVMessage message = new ErlKVMessage (FrontPort.CARD_EVOLVE); 
		message.addValue ("mainuid", new ErlString (mainUID));
		message.addValue ("fooduid", new ErlString (foodUID));
		if (ability != "")
			message.addValue ("ability", new ErlString (ability));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		string str = (message.getValue ("msg") as ErlAtom).Value; 
		
		if (str == FPortGlobal.INTENSIFY_SUCCESS) {
			string uid = (message.getValue ("main_uid") as ErlType).getValueString ();
			//刷新卡片仓库
//			SerializeStorageFPort serialize = FPortManager.Instance.getFPort ("SerializeStorageFPort") as SerializeStorageFPort;
//			serialize.access ("card_storage", callbacki, uid);
			if(callbacki != null){
				callbacki(uid);
				callbacki = null;
			}
		} else if (str == FPortGlobal.INTENSIFY_LIMIT) {//卡片状态限制
			
		} else if (str == FPortGlobal.INTENSIFY_MAINLOSE) {//主角卡不能提升
			
		} else if (str == FPortGlobal.INTENSIFY_PLENGTHLIMIT) {//被动技能长度限制
			
		} else if (str == FPortGlobal.INTENSIFY_OUTCASH) {//缺钱
			
		} else if (str == FPortGlobal.INTENSIFY_ERROR) {//其他信息错误
			
		}
		
	}
}
