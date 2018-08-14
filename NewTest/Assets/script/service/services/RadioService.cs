using System;
 
/**
 * 广播消息服务
 * */
public class RadioService:BaseFPort
{
	public RadioService ()
	{
		
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlType type = message.getValue ("radio") as ErlType;
 
		string uid = "0";//唯一索引
		string name = LanguageConfigManager.Instance.getLanguage ("s0306");//发件人
		int vip = 0;//vip等级
		int channelType = ChatManagerment.CHANNEL_RADIO;//渠道类型Channel 
		int sender = 2;//寄信人类型 GM2，玩家1
		int stime = ServerTimeKit.getSecondTime ();//收到时间
		int isShow = 1;//是否为展示，非后台传送，前台预留判断用，聊天1，装备2，卡片3
		int job = 0;//公会职务，世界频道为0
		string content = type.getValueString ();//内容
		ErlArray goods = null;//装备或者卡片
		ChatManagerment.Instance.createChat (uid, name, vip, channelType, sender, stime, isShow, job, content, goods, "", "", 0);
        //ChatManagerment.Instance.changeChatIco ();
		RadioManager.Instance.M_addRadioMsg (RadioManager.RADIO_MAIN_TYPE,content);
	}
} 

