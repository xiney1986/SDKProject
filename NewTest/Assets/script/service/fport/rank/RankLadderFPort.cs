
using System;
using System.Collections.Generic;
using UnityEngine;

public class RankLadderFPort : BaseFPort
{
	private CallBack callback;
	private int type = RankManagerment.TYPE_LADDER;

	public void apply (CallBack callback)
	{

		if (RankManagerment.Instance.nextTime.ContainsKey (type) && ServerTimeKit.getSecondTime () < RankManagerment.Instance.nextTime [type]) {
			callback ();
			return;
		}

		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.RANK_RANK);
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		/*
		[[4000,38,[281479271678950,真龙·安拉,[],2,36500,1,[3,[],[[11442,0,281479271679959,0,0],[11344,0,281479271679971,0,0],[2,0,281479271678953,0,0],[11237,0,281479271679953,0,0],[11226,0,281479271679982,0,0]]],44560,9,10]],
		[290,0,[281479271678949,坠星的克劳瑞,[],4,911600,1,[3,[],[[],[],[4,0,281479271677953,0,0],[11442,0,281479271680955,0,0],[]]],13802,6,0]]]}
		*/
		ErlArray array = message.getValue ("msg") as ErlArray;

		ErlArray itemArray;
		PvpOppInfo player;
		int ladderRank;
		int prestige;
		int medalSid;
		RankManagerment.Instance.ladderList.Clear();
		for (int i=array.Value.Length-1; i>=0; i--) {
			itemArray = array.Value [i] as ErlArray;
			ladderRank = StringKit.toInt (itemArray.Value [0].getValueString ());
			prestige = StringKit.toInt (itemArray.Value [1].getValueString ());
			medalSid = StringKit.toInt (itemArray.Value [2].getValueString ());

			player = setOppInfo (itemArray.Value [3] as ErlArray);
			player.ladderRank = ladderRank;
			player.prestige = prestige;
			player.medalSid = medalSid;

			if (player.uid == UserManager.Instance.self.uid) {
				RankManagerment.Instance.myRank.Remove (type);
				RankManagerment.Instance.myRank.Add (type, ladderRank);
			}
			RankManagerment.Instance.ladderList.Add (player);
		}
		int nextUpdateTime = ServerTimeKit.getSecondTime () + 60;
		if (RankManagerment.Instance.nextTime.ContainsKey (type)) {
			RankManagerment.Instance.nextTime [type] = nextUpdateTime;
		} else {
			RankManagerment.Instance.nextTime.Add (type, nextUpdateTime);
		}


		//附加sdk信息
		if (RankManagerment.Instance.ladderList != null && RankManagerment.Instance.ladderList.Count > 0) {

			string str = "";
		    for (int i=0;i<RankManagerment.Instance.ladderList.Count;i++)
		    {
                if (i != RankManagerment.Instance.ladderList.Count-1) str += RankManagerment.Instance.ladderList[i].uid + ",";
                else str += RankManagerment.Instance.ladderList[i].uid ;
		    }
            //foreach (PvpOppInfo each in RankManagerment.Instance.ladderList) {
            //    str += each.uid;
            //}

			if (str != "") {
				str = str.Substring (0, str.Length - 1);
				SdkFPort fp = FPortManager.Instance.getFPort<SdkFPort> ();
               
				fp.getSdkInfo (str, getSdkInfoCallBack);
			} else {
				getSdkInfoCallBack (null);
			}

		} else {

			getSdkInfoCallBack (null);
		}

	}

	void getSdkInfoCallBack (Dictionary<string, PlatFormUserInfo> dic){
		if (RankManagerment.Instance.ladderList != null && RankManagerment.Instance.ladderList.Count > 0) {

			foreach (PvpOppInfo each in RankManagerment.Instance.ladderList) {

				if (dic.ContainsKey (each.uid))
					each.sdkInfo = dic [each.uid];

			}

		}

		if (callback != null) {
			callback ();
			callback = null;
		}

	}

	private PvpOppInfo setOppInfo (ErlArray list)
	{
		return  PvpOppInfo.pares(list);
	}

}


