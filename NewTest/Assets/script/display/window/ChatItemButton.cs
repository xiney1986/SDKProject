using UnityEngine;
using System;
using System.Collections;

public class ChatItemButton : ButtonBase {
	
	public Chat chat;
	public ChatShowButton showSender;
	public ChatShowButton showItem;
	public UILabel showLabel;
	public UISprite  background;
	private string uid;
	private	string time;
	private	string type;
	private	string sname;
	private	string content;
	private string vip;
	private string job;
	public void dynamicHeight()
	{
		float h = showLabel.CalculateBounds().extents.y * 2 + 30;
		background.height = (int)h;
		background.MakePixelPerfect();
		BoxCollider box=GetComponent<BoxCollider>();
		box .size=new Vector3(box .size.x,h,box .size.z);

		/**
		if(showLabel.CalculateBounds().extents.y<=11)
		{
			background.gameObject.transform.localScale=new Vector3(1,0.35f,1);
			showSender.gameObject.transform.localScale=new Vector3(1,0.35f,1);
			showItem.gameObject.transform.localScale=new Vector3(1,0.35f,1);
			BoxCollider box=GetComponent<BoxCollider>();
			box .size=new Vector3(box .size.x,42f,box .size.z);
		}else if(showLabel.CalculateBounds().extents.y>11 )
		{
			background.gameObject.transform.localScale=new Vector3(1,0.6f,1);
			showSender.gameObject.transform.localScale=new Vector3(1,0.6f,1);
			showItem.gameObject.transform.localScale=new Vector3(1,0.6f,1);
			BoxCollider box=GetComponent<BoxCollider>();
			box .size=new Vector3(box .size.x,66f,box .size.z);
		}
		*/
	}
	public void initUI(Chat _chat)
	{

		chat = _chat;
		uid = chat.uid;
		DateTime dt = TimeKit.getDateTime(chat.stime);
		string hourInfo=dt.Hour<10?"0"+dt.Hour:dt.Hour.ToString();
		string minInfo=dt.Minute<10?"0"+dt.Minute:dt.Minute.ToString();
		time = "[" + hourInfo + ":" + minInfo + "]";
		type = getChatType(chat.channelType);
		sname = "[" + chat.name + "]";
		job = chat.channelType != 2 ? "" : "[" + getJob(chat.job) + "]";
		content = chat.content;
		if(EXPSampleManager.Instance.getLevel(3,chat.vip) != 0)
			vip = "[Vip" + EXPSampleManager.Instance.getLevel(3,chat.vip) + "]";
		else
			vip = "";
		
		showSender.fatherWindow = this.fatherWindow;
		showItem.fatherWindow = this.fatherWindow;
		showSender.showType = 0;
		showSender.uid = chat.uid;

		if(ChatManagerment.Instance.getMsgType(chat) == ChatManagerment.MSGTYPE_SAY)
		{
			showItem.showType = 999;
			if(!ChatManagerment.Instance.isSystemMsg(chat))
				showLabel.text = type + sname + job + vip + ":" + content + time;
			else
			{
				showSender.gameObject.SetActive (false);
				showItem.gameObject.SetActive (false);
				showLabel.text = type + content + time;
			}
		}
		else
		{	
			if(ChatManagerment.Instance.getMsgType(chat) == ChatManagerment.MSGTYPE_EQUIP)
			{
				showItem.showType = 1;
				showItem.equip = EquipManagerment.Instance.createEquip(chat.goods);
				showLabel.text = type + sname + job + vip + ":" + LanguageConfigManager.Instance.getLanguage("s0305")
					+ QualityManagerment.getQualityColor(showItem.equip.getQualityId()) + "[" + showItem.equip.getName() + "]" + "[-]" + time;
			}
			else if(ChatManagerment.Instance.getMsgType(chat) == ChatManagerment.MSGTYPE_CARD)
			{
				showItem.showType = 2;
				showItem.card = CardManagerment.Instance.createCardByChatServer(chat.goods);
				showLabel.text = type + sname + job + vip + ":" + LanguageConfigManager.Instance.getLanguage("s0304")
					+ QualityManagerment.getQualityColor(showItem.card.card.getQualityId()) + "[" + showItem.card.card.getName() + "]" + "[-]" + time;
			}
		}
		dynamicHeight();
	}
	
	public string getChatType(int _type)
	{
		switch(_type)
		{
		case 1:
			return Colors.CHAT_WORLD +  "[" + LanguageConfigManager.Instance.getLanguage("s0302") + "]";//世界
		case 2:
			return Colors.CHAT_UNION + LanguageConfigManager.Instance.getLanguage("s0303");//公会
		case 3:
			return Colors.CHAT_SYSTEM + "[" + LanguageConfigManager.Instance.getLanguage("s0306") + "]";//系统消息
		case 4:
			return Colors.CHAT_RADIO + "[" + LanguageConfigManager.Instance.getLanguage("chat08") + "]";//广播
		default:
			return "";
		}
	}

	public string getJob(int _type)
	{
		return GuildJobType.getJobName(_type);
	}
}
