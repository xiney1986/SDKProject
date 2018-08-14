using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 聊天信息实体类
 * @authro 陈世惟  
 * */
public class Chat {

	public Chat (string uid,string name,int vip,int channelType,int sender,int stime,int isShow,int job,string content,ErlArray goods, string friendReceiveUid, string friendReceiveName, int friendReceiveVip)
	{

		this.uid = uid;
		this.name = name;
		this.vip = vip;
		this.channelType = channelType;
		this.sender = sender;
		this.stime = stime;
		this.job = job;
		this.content = content;
		this.isShow = isShow;
		this.goods = goods;
        this.friendReceiveUid = friendReceiveUid;
        this.friendReceiveName = friendReceiveName;
        this.friendReceiveVip = friendReceiveVip;
	} 
	public string uid;//唯一索引
	public string name ;//发件人
	public int vip;//vip等级
    public int channelType;//渠道类型Channel 1世界、2公会、3私聊、4系统、5广播
	public int sender;//寄信人类型 GM2，玩家1
	public int stime;//收到时间
	public int isShow;//是否为展示，非后台传送，前台预留判断用，聊天1，装备2，卡片3
	public int job;//公会职务，世界频道为0
	public string content;//内容
	public ErlArray goods;//装备或者卡片
    public string friendReceiveUid; //如果是好友聊天,目标uid,接收聊天消息方
    public string friendReceiveName;//如果是好友聊天,目标名称
    public int friendReceiveVip;
}


