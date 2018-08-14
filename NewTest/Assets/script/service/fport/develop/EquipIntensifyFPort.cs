using UnityEngine;
using System.Collections;

/**
 * 装备强化接口
 * @author 汤琦
 * */
public class EquipIntensifyFPort : BaseFPort
{

	private CallBack<string> callback;
	
	public EquipIntensifyFPort()
	{
		 
	}
	//副卡id（foodUID）以逗号分隔的字符串
	public void access (string mainUID,string foodUID,CallBack<string> back)
	{ 
		this.callback = back;
		ErlKVMessage message = new ErlKVMessage (FrontPort.EQUIP_INTENSIFY); 
		message.addValue ("main_uid", new ErlString (mainUID));
		message.addValue ("food_uid", new ErlString (foodUID));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		string str = (message.getValue ("msg") as ErlAtom).Value; 
		
		if(str == FPortGlobal.INTENSIFY_SUCCESS)
		{
			GuideManager.Instance.doGuide(); 
			GuideManager.Instance.closeGuideMask();
			string uid = (message.getValue ("main_uid") as ErlType).getValueString();
			//刷新卡片仓库
//			SerializeStorageFPort serialize = FPortManager.Instance.getFPort("SerializeStorageFPort") as SerializeStorageFPort;
//			serialize.access("equip_storage",callback,uid);
			if(callback != null){
				callback(uid);
				callback = null;
			}
		}
		else if(str == FPortGlobal.INTENSIFY_FOODUID_ERROR)//食物卡uid异常
		{
			UiManager.Instance.createMessageWindowByOneButton(LanguageConfigManager.Instance.getLanguage("Intensify16"),null);
		}
		else if(str == FPortGlobal.INTENSIFY_EQUIP_LLEVEL)//装备满级
		{
			UiManager.Instance.createMessageWindowByOneButton(LanguageConfigManager.Instance.getLanguage("Intensify17"),null);
		}
		else if(str == FPortGlobal.INTENSIFY_EQUIP_SERROR)//装备状态异常
		{
			UiManager.Instance.createMessageWindowByOneButton(LanguageConfigManager.Instance.getLanguage("Intensify18"),null);
		}
		else if(str == FPortGlobal.INTENSIFY_EQUIP_NOHAVE)//装备不存在
		{
			UiManager.Instance.createMessageWindowByOneButton(LanguageConfigManager.Instance.getLanguage("Intensify19"),null);
		}
		else if(str == FPortGlobal.SYSTEM_INFO_ERROR)//参数错误
		{
			UiManager.Instance.createMessageWindowByOneButton(LanguageConfigManager.Instance.getLanguage("Intensify20"),null);
		}
		else {
			UiManager.Instance.createMessageWindowByOneButton(LanguageConfigManager.Instance.getLanguage("Intensify20"),null);
		}
	}
}
