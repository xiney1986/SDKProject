using UnityEngine;
using System;

/**
 * 登陆用户信息初始化接口
 * @author zhoujie
 * */
public class UserNewFPort : BaseFPort
{
	
	private CallBack callback;

	public void access (CallBack callback)
	{
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.INIT_USER_NEW);
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlArray arr = message.getValue ("msg") as ErlArray;
		int i = 0;
		//FPortManager.Instance.getFPort<UserFPort> ().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		//FPortManager.Instance.getFPort<StorageFPort> ().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
//		FPortManager.Instance.getFPort<ArmyGetFPort> ().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		FPortManager.Instance.getFPort<TotalLoginFPort> ().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		FPortManager.Instance.getFPort<InitTaskFPort> ().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		//pvp信息量比较大 不和登陆协议合在一起 单独的一个协议存在
		//PvpInfoManagerment.Instance.checkPvp(FPortManager.Instance.getFPort<PvpGetInfoFPort> ().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray)));
		FPortManager.Instance.getFPort<ExchangeInfoFPort> ().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		FPortManager.Instance.getFPort<InitLuckyDrawFPort> ().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		//FPortManager.Instance.getFPort<FuBenInfoFPort> ().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray)); //剧情副本
		//FPortManager.Instance.getFPort<FuBenInfoFPort> ().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray)); //讨伐副本
		//FPortManager.Instance.getFPort<GuideGetInfoFPort> ().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		FPortManager.Instance.getFPort<PyxFPort> ().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		FPortManager.Instance.getFPort<BeastAddInfoFPort> ().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		//FPortManager.Instance.getFPort<FriendsFPort> ().parseKVMsgTypeInit (getKVMsg (arr.Value [i++] as ErlArray));
		GoddessAstrolabeManagerment.Instance.initInfoByServer (getKVMsg (arr.Value [i++] as ErlArray));
		FPortManager.Instance.getFPort<NoticeGetHappyTurnSpriteFPort> ().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		FPortManager.Instance.getFPort<FriendsShareFPort> ().parseKVMsgTypeInit (getKVMsg (arr.Value [i++] as ErlArray));
		FPortManager.Instance.getFPort<HeroRoadFPort> ().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		FPortManager.Instance.getFPort<ArenaGetStateFPort> ().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		//FPortManager.Instance.getFPort<GuildGetInfoFPort> ().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		FPortManager.Instance.getFPort<GuildGetApplyListFPort> ().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		FPortManager.Instance.getFPort<DivineGetInfoFPort> ().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		FPortManager.Instance.getFPort<NoticeGetTimeFPort> ().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		NoticeManagerment.Instance.setAlchemyNum (StringKit.toInt ((getKVMsg (arr.Value [i++] as ErlArray).getValue ("msg") as ErlType).getValueString ()));
		FPortManager.Instance.getFPort<NoticetHeroEatInfoFPort> ().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		//FPortManager.Instance.getFPort<NoticeConfigFPort> ().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		FPortManager.Instance.getFPort<NoticeMonthCardFPort> ().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		FPortManager.Instance.getFPort<SweepGetInfoFPort> ().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		//公会建筑等级未获取

		//限时翻翻乐特殊处理
		if(getNotice())
		{
			NoticeGetHappyTurnSpriteFPort fport = FPortManager.Instance.getFPort<NoticeGetHappyTurnSpriteFPort>();
			fport.access(notice.sid,callback);
		}
		else
			callback ();
        GetShenGeInfoFPort fports = FPortManager.Instance.getFPort("GetShenGeInfoFPort") as GetShenGeInfoFPort;
        fports.access(null);
	}
	//获得一个新的ErlKVMessage，ErlArray={{K,V},{K,V},...}
	private ErlKVMessage getKVMsg (ErlArray arr){
		ErlKVMessage msg = new ErlKVMessage (null);
		ErlArray arr2;
		for (int i=arr.Value.Length-1; i>=0; i--) {
			arr2=arr.Value[i] as ErlArray;
			msg.addValue(arr2.Value[0].getValueString(),arr2.Value[1]);
		}
		return msg;
	}

	private Notice notice;
	private bool getNotice()
	{
		notice = NoticeManagerment.Instance.getNoticeByType(NoticeType.XIANSHI_HAPPY_TURN);
		bool isvalid = notice.isValid();
		return notice!=null&&isvalid;
	}
}
