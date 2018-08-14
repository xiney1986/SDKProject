using UnityEngine;
using System;

/**
 * 用户初始化接口
 * @author zhanghaishan
 * */
public class MiniUserNewFPort : MiniBaseFPort
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
		new MiniUserFPort().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		new MiniStorageFPort().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		new MiniArmyGetFPort().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		new MiniTotalLoginFPort().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		new MiniMailGetFPort().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		new MiniInitTaskFPort().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		//new MiniPvpGetInfoFPort().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		new MiniExchangeInfoFPort().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		new MiniInitLuckyDrawFPort().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		new MiniFuBenInfoFPort().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray)); //剧情副本
		new MiniFuBenInfoFPort().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray)); //讨伐副本
		new MiniGuideGetInfoFPort().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		new MiniPyxFPort ().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		new MiniBeastAddInfoFPort ().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		new MiniFriendsFPort ().parseKVMsgTypeInit (getKVMsg (arr.Value [i++] as ErlArray));
		new MiniGoddessAstrolabeFPort().initInfoByServer (getKVMsg(arr.Value [i++] as ErlArray));
		new MiniGetStarInfoFPort ().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		new MiniFriendsShareFPort().parseKVMsgTypeInit (getKVMsg (arr.Value [i++] as ErlArray));
		new MiniHeroRoadFPort ().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		new MiniArenaGetStateFPort ().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		new MiniGuildGetInfoFPort ().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		new MiniGuildGetApplyListFPort().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		new MiniDivineGetInfoFPort ().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		new MiniNoticeGetTimeFPort ().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		StringKit.toInt ((getKVMsg (arr.Value [i++] as ErlArray).getValue ("msg") as ErlType).getValueString ());
		new MiniNoticetHeroEatInfoFPort ().parseKVMsg (getKVMsg (arr.Value [i++] as ErlArray));
		//公会建筑等级未获取
		callback ();
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
}
