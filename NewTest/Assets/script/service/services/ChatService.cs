using UnityEngine;
using System.Collections;

/**
 * 聊天消息服务
 * */
public class ChatService : BaseFPort {

	public ChatService ()
	{
		
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlArray list = message.getValue ("msg") as ErlArray;
		
		if(list != null) {
			/*
			int index = 0;
			string uid;
			string name;
			int vip = 0;
			int type = 0;
			int sender = 0;
			int stime = 0;
			int isShow = 1;
			int job = 0;
			string content;
			ErlArray goods = null;

			uid = (list.Value[index] as ErlType).getValueString();
			name = (list.Value[++index] as ErlType).getValueString();
			vip =  StringKit.toInt((list.Value[++index] as ErlType).getValueString());
			sender = StringKit.toInt((list.Value[++index] as ErlType).getValueString());
			stime = StringKit.toInt((list.Value[++index] as ErlType).getValueString());
			type = StringKit.toInt((list.Value[++index] as ErlType).getValueString());
			job = StringKit.toInt((list.Value[++index] as ErlType).getValueString());
			content = (list.Value[++index] as ErlType).getValueString();
			ErlArray listc = list.Value[++index] as ErlArray;
			
			if(listc != null)
			{
				string str = (listc.Value[0] as ErlType).getValueString();
				ErlArray arrays = listc.Value[1] as ErlArray;
				if(str==ChatManagerment.SHOW_CARD)
				{
					isShow = ChatManagerment.MSGTYPE_CARD;
					goods = arrays;
				}
				else if(str==ChatManagerment.SHOW_Equip)
				{
					isShow = ChatManagerment.MSGTYPE_EQUIP;
					goods = arrays;
				}
			}
			
			if(content == "none" || content == null)
				content = null;
			
			ChatManagerment.Instance.createChat(uid,name,vip,type,sender,stime,isShow,job,content,goods);
			*/
			readMsg(list);
		}
		ErlType msgList = message.getValue ("msg2") as ErlType;
		if (msgList != null) {
			ChatManagerment.Instance.clearChat();
			ErlArray arr = msgList as ErlArray;
			ChatManagerment.Instance.clearChat();
			for (int i= arr.Value.Length - 1; i > -1; i--) {
				ErlArray listItem = (arr.Value [i] as ErlType) as ErlArray;
				readMsg(listItem);
			}
		}

		/**
		if(ChatManagerment.Instance.updateMsg != null){
			
			ChatManagerment.Instance.updateMsg();

		}
		*/

	}

	private void readMsg(ErlArray list)
	{
		int index = 0;
		string uid;
		string name;
		int vip = 0;
		int type = 0;
		int sender = 0;
		int stime = 0;
		int isShow = 1;
		int job = 0;
        string friendReceiveUid = "";
        string friendReceiveName = "";
        int friendReceiveVip = 0;
		string content;
		ErlArray goods = null;

        ErlArray friendArr = list.Value[1] as ErlArray;
        friendReceiveUid = friendArr.Value[0].getValueString();
        friendReceiveName = friendArr.Value[1].getValueString();
        friendReceiveVip = StringKit.toInt(friendArr.Value[2].getValueString());

        list = list.Value[0] as ErlArray;
		uid = (list.Value[index] as ErlType).getValueString();
		name = (list.Value[++index] as ErlType).getValueString();
		vip =  StringKit.toInt((list.Value[++index] as ErlType).getValueString());
		sender = StringKit.toInt((list.Value[++index] as ErlType).getValueString());
		stime = StringKit.toInt((list.Value[++index] as ErlType).getValueString());
		type = StringKit.toInt((list.Value[++index] as ErlType).getValueString());
		job = StringKit.toInt((list.Value[++index] as ErlType).getValueString());
		content = (list.Value[++index] as ErlType).getValueString();
		ErlArray listc = list.Value[++index] as ErlArray;
		
		if(listc != null)
		{
			string str = (listc.Value[0] as ErlType).getValueString();
			ErlArray arrays = listc.Value[1] as ErlArray;
			if(str==ChatManagerment.SHOW_CARD)
			{
				isShow = ChatManagerment.MSGTYPE_CARD;
				goods = arrays;
			}
			else if(str==ChatManagerment.SHOW_Equip)
			{
				isShow = ChatManagerment.MSGTYPE_EQUIP;
				goods = arrays;
			}
		}
		
		if(content == "none" || content == null)
			content = null;
		
		ChatManagerment.Instance.createChat(uid,name,vip,type,sender,stime,isShow,job,content,goods, friendReceiveUid, friendReceiveName, friendReceiveVip);
		ChatManagerment.Instance.changeChatIco();
	}
	
}
