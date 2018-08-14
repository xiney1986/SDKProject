using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonStoreResult : ButtonBase
{  
	public Equip equip;
	
	public void UpdateEquip (Equip _equip)
	{
		this.equip = _equip;
		
	}
	
	//穿装备和脱装备成功后的回调
	public void equipResult (List<AttrChange> attrs, int types)
	{ 
		if (UiManager.Instance.cardBookWindow != null) {
			UiManager.Instance.cardBookWindow.equipNewItem (attrs, types);
		}
		else {
			CardBookWindow.Show ();
		}
		//完成后清理
		EquipManagerment.Instance.finishEquipChange();
		if(fatherWindow is EquipChooseWindow){
			fatherWindow.finishWindow();
		}
	}
	
	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		if (this.textLabel.text == LanguageConfigManager.Instance.getLanguage ("s0013")) {
//			 点击装备
			EquipOperateFPort eof = FPortManager.Instance.getFPort ("EquipOperateFPort") as EquipOperateFPort;
			eof.access (EquipManagerment.Instance.activeEquipMan .uid, EquipManagerment.Instance.activeEquipMan.sid, equip.uid, equip.getPartId (),equipResult);
		}else  if (this.textLabel.text == LanguageConfigManager.Instance.getLanguage ("s0012")) {
			//强化
			UiManager.Instance.openWindow<IntensifyEquipWindow>((win)=>{
				win.Initialize (equip, IntensifyEquipWindow.EQUIPSTORE);
			});
		}else  if (this.textLabel.text == LanguageConfigManager.Instance.getLanguage ("equipStar02")) {
			//升星
			UiManager.Instance.openWindow<EquipUpStarWindow>((win)=>{
				win.Initialize (equip);
			});
		}
		else if (this.textLabel.text == LanguageConfigManager.Instance.getLanguage ("s0308")) {
			//聊天展示
			if(fatherWindow.GetType () == typeof(EquipChooseWindow))
			{
				//如果直接从装备选择点展示
				EquipChooseWindow fwin = fatherWindow as EquipChooseWindow;

				if(fwin.comeFrom == EquipChooseWindow.FROM_CHAT || fwin.comeFrom == EquipChooseWindow.FROM_CHAT_FRIEND)
				{
					sendMsgFPort(ChatManagerment.Instance.sendType);
					/*这里开始是可滑动聊天窗口展示的关闭后处理，暂时不删
					UiManager.Instance.openDialogWindow<NewChatWindow> ((win) => {
						win.initChatWindow (ChatManagerment.Instance.sendType - 1);
					});
					*/
					fatherWindow.finishWindow ();
				}
			}
		}
	}
	
	//聊天展示接口
	private void sendMsgFPort(int _chatChannelType)
	{
        EquipChooseWindow fwin = fatherWindow as EquipChooseWindow;
        ChatSendMsgFPort fport = FPortManager.Instance.getFPort("ChatSendMsgFPort") as ChatSendMsgFPort;
        if (fwin.comeFrom != EquipChooseWindow.FROM_CHAT_FRIEND)
        {
            fport.access(_chatChannelType, null, ChatManagerment.SHOW_Equip, equip.uid, null);
        }
        else
        {
            fport.access(ChatManagerment.Instance.CurrentFriendInfo.getUid(), UserManager.Instance.self.uid, _chatChannelType, null, ChatManagerment.SHOW_Equip, equip.uid, null);
        }
	}
}
