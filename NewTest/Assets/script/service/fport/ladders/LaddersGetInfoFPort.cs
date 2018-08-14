using System;
using System.Collections.Generic;

/// <summary>
/// 获得天梯所有信息的请求
/// </summary>
public class LaddersGetInfoFPort:BaseFPort
{
	public LaddersGetInfoFPort ()
	{
	}

	private CallBack<bool> callback;

	public void apply (CallBack<bool> _callback)
	{
		callback = _callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.LADDERS_GETPLAYERS);	
		access (message);
	}

	public override void read (ErlKVMessage message)
	{

		ErlArray data = message.getValue ("msg") as ErlArray;
		if (data == null) {
			if (callback != null) {
				callback (false);
				callback = null;
			}
			return;
		}	
		/*
		[
		[rank_award,[76,1403193600,0]排名，领取最后期限，是否已经领取
		[super_box,[[1,1,1,5501],[2,1,1,5501],[3,1,1,5501]]],index,refreshTimes,multiple,rankAtRefresh
		
		[enemy,[[1,[[0,1,5373],[0,1,5355],[0,0,5388]]],[2,[[0,0,4746],[0,0,4804],[0,0,4613]]],[3,[[0,0,4150],[0,0,4431],[0,0,4008]]]]],
		 uid(0 is robot),hasChallenge,rank
		[prestige,0],
		[rank,5355],
		[fight_num,9,168]time

		[war, 只有被人打我
		[
		[1403092973,281479271678949,2,0],time,uid,iswin(1 is win,2 lost),rankChange
		[1403092966,281479271678949,2,0],
		[1403092965,281479271678949,2,0],
		[1403092965,281479271678949,2,0],
		[1403092964,281479271678949,2,0]]]]
		]
		*/
		LaddersManagement.Instance.Award.M_clear();

		ErlArray itemData;
		bool needSdkInfo=false;//是否需要请求sdk
		string key = string.Empty;
		for (int i=0,length=data.Value.Length; i<length; i++) {
			itemData = data.Value [i] as ErlArray;
			key = itemData.Value [0].getValueString ();

			switch (key) {
			case "super_box":
				parseChest (itemData.Value [1] as ErlArray);
				break;
			case "enemy":
				parseEnemy (itemData.Value [1] as ErlArray);
				needSdkInfo=true;
				break;
			case "prestige":
				UserManager.Instance.self.prestige = StringKit.toInt (itemData.Value [1].getValueString ());
				break;
			case "rank":
				LaddersManagement.Instance.currentPlayerRank = StringKit.toInt (itemData.Value [1].getValueString ());
				break;
			case "fight_num":
				LaddersManagement.Instance.currentChallengeTimes = StringKit.toInt (itemData.Value [1].getValueString ());
				LaddersManagement.Instance.lastFightTime = StringKit.toInt (itemData.Value [2].getValueString ());
				break;
			case "rank_award":
				parseAward(itemData.Value[1] as ErlArray);
				break;
			case "war":
				parseRecord (itemData.Value [1] as ErlArray);
				break;
			case "free_refresh":
				LaddersManagement.Instance.currentRefreshEndTime=StringKit.toInt(itemData.Value[1].getValueString());
				break;
			case "friend_fight":
				LaddersManagement.Instance.currentFriendHelpTimes=StringKit.toInt(itemData.Value[1].getValueString());
				LaddersManagement.Instance.lastFriendHelpTime=StringKit.toInt(itemData.Value[2].getValueString());
				break;
			case "max_fight_num":
				LaddersManagement.Instance.maxFightTime=StringKit.toInt(itemData.Value[1].getValueString());
				break;
			case "buy_fight_num":
				LaddersManagement.Instance.buyFightCount = StringKit.toInt (itemData.Value [1].getValueString ());
				LaddersManagement.Instance.lastBuyFightTime = StringKit.toInt (itemData.Value [2].getValueString ());
				break;
			case "buy_invite_count":
				LaddersManagement.Instance.mBuyFriendFightCount = StringKit.toInt(itemData.Value[1].getValueString());
				break;
			case "max_friend_fight":
				LaddersManagement.Instance.TotalApplyFriendHelpTimes = StringKit.toInt (itemData.Value [1].getValueString ());
				break;
			case "max_invite_num":
				LaddersManagement.Instance.BeInviteMaxNum=StringKit.toInt(itemData.Value [1].getValueString ());
				break;
			}
		}

		if(needSdkInfo==true){

			SdkFPort sdkfp=FPortManager.Instance.getFPort<SdkFPort>();
			LaddersPlayerInfo[] info=LaddersManagement.Instance.Players.M_getPlayers();

			if(info!=null && info.Length>0 ){
				string str="";

				for(int i=0;i< info.Length;i++){
					str+=info[i].uid+",";
				}

				if(str!="")
					str=str.Substring(0,str.Length-1);

				sdkfp.getSdkInfo(str,sdkCallback);


			}else{
				sdkCallback(null);

			}

		}else{
			sdkCallback(null);
		}


	}

	void sdkCallback(Dictionary<string, PlatFormUserInfo> dic){

		LaddersPlayerInfo[] infos=LaddersManagement.Instance.Players.M_getPlayers();
		foreach(LaddersPlayerInfo each in infos){
			if(dic.ContainsKey(each.uid))
				each.sdkInfo=dic[each.uid];
		}

		LaddersManagement.Instance.M_updateChestStatus ();
		if (callback != null) {
			callback (true);
			callback = null;
		}

	}
	private void parseAward(ErlArray _data)
	{
		//[76,1403193600,0]排名，领取最后期限，是否已经领取
		int rank=StringKit.toInt(_data.Value[0].getValueString());
		int limitTime=StringKit.toInt(_data.Value[1].getValueString());
		bool hasReceive=StringKit.toInt(_data.Value[2].getValueString())==1;
		LaddersManagement.Instance.Award.M_update(hasReceive,limitTime,rank);
	}
	private void parseChest (ErlArray _data)
	{
		//[super_box,[[1,1,1,5501],[2,1,1,5501],[3,1,1,5501]]],index,refreshTimes,multiple,rankAtRefresh
		if (_data == null)
			return;
		ErlArray itemData;

		int index = 0;
		int refreshTime;
		int multiple;
		int userRankAtRefresh;
		int length = _data.Value.Length;
		LaddersChestInfo chestItem;
		LaddersChestInfo[] chests = new LaddersChestInfo[length];
		LaddersManagement.Instance.Chests.M_clear ();
		for (int i=0; i<length; i++) {
			itemData = _data.Value [i] as ErlArray;

			index = StringKit.toInt (itemData.Value [0].getValueString ());
			refreshTime = StringKit.toInt (itemData.Value [1].getValueString ());
			multiple = StringKit.toInt (itemData.Value [2].getValueString ());
			userRankAtRefresh = StringKit.toInt (itemData.Value [3].getValueString ());
			chestItem = new LaddersChestInfo ();
			chestItem.index = index;
			chestItem.multiple = multiple;
			chestItem.canReceiveRank = userRankAtRefresh;
			chests [i] = chestItem;

			LaddersManagement.Instance.Chests.M_updateChest(index-1,chestItem);
		}

		//LaddersManagement.Instance.Chests.M_updateChest (chests);
	}

	private void parseEnemy (ErlArray _data)
	{
		/*
		[
		[1,[[[0,0,0],1,5373],[[uid,vip,exp],1,5355],[100,0,5388,vip,]]],
		[2,[[0,0,4746],[0,0,4804],[0,0,4613]]],
		[3,[[0,0,4150],[0,0,4431],[0,0,4008]]],
		]
		*/
		// uid(0 is robot),hasChallenge,rank
		if (_data == null)
			return;
		ErlArray itemData;
		ErlArray subItemData;
		ErlArray subsubItemData;

		int chestIndex;
		int index;
		string uid;
		bool isDefeated;
		int rank;
		int vip;
		int exp;


		int length = _data.Value.Length;
		LaddersPlayerInfo playerItem;

		//LaddersPlayerInfo[] linePlayers;
		List<LaddersPlayerInfo> players = new List<LaddersPlayerInfo> ();

		LaddersManagement.Instance.Players.M_clear ();
		for (int i=0; i<length; i++) {
			itemData = _data.Value [i] as ErlArray;

			chestIndex = StringKit.toInt (itemData.Value [0].getValueString ());
			subItemData = itemData.Value [1] as ErlArray;

			//linePlayers=new LaddersPlayerInfo[subItemData.Value.Length];

			for (int j=0; j<subItemData.Value.Length; j++) {
				subsubItemData = subItemData.Value [j] as ErlArray;

				uid = subsubItemData.Value [0].getValueString ();
				isDefeated = StringKit.toInt (subsubItemData.Value [1].getValueString ()) == 1;
				rank = StringKit.toInt (subsubItemData.Value [2].getValueString ());

				playerItem = new LaddersPlayerInfo ();
				playerItem.uid = uid;
				playerItem.belongChestIndex = chestIndex;
				playerItem.index = j + 1;
				playerItem.isDefeated = isDefeated;
				playerItem.rank = rank;
				if(uid!="0")
				{
					playerItem.level=StringKit.toInt(subsubItemData.Value[3].getValueString());
					playerItem.headIconId=StringKit.toInt(subsubItemData.Value[4].getValueString());
					playerItem.vip=StringKit.toInt(subsubItemData.Value[5].getValueString());
				}else
				{
					playerItem.vip=0;
					playerItem.level=0;
					playerItem.headIconId=UnityEngine.Random.Range(1,7);
				}
				players.Add (playerItem);
				//linePlayers[j]=playerItem;
			}
			/*
			Array.Sort(linePlayers,CompareFunc);
			for(int k=0,lenght=linePlayers.Length;k<length;k++)
			{
				linePlayers[k].uiIndex=k+1;
			}
			ListKit.AddRange(players,linePlayers);
			players.Add (playerItem);
			*/
		}
		LaddersManagement.Instance.Players.M_updatePlayer (players.ToArray ());
	}

	static public int CompareFunc (LaddersPlayerInfo a, LaddersPlayerInfo b)
	{
		if (a != b && a != null && b != null)
		{
			if (a.rank < b.rank) return -1;
			if (a.rank > b.rank) return 1;
			return (a.index < b.index) ? -1 : 1;
		}
		return 0;
	}

	private void parseRecord (ErlArray _data)
	{
		/*
		[war, 只有被人打我
		 [
		 [1403092973,281479271678949,2,0],time,uid,iswin(1 is win,2 lost),rankChange
		 [1403092966,281479271678949,2,0],
		 [1403092965,281479271678949,2,0],
		 [1403092965,281479271678949,2,0],
		 [1403092964,281479271678949,2,0]]]]
		]
		*/
		if (_data == null)
			return;
		ErlArray itemData;
		
		int time;
		string uid;
		bool isWin;
		int rank;
		string enemyName;
		int vipLevel;

		int length = _data.Value.Length;
		LaddersRecordInfo recordItem;
		LaddersRecordInfo[] records = new LaddersRecordInfo[length];
		LaddersManagement.Instance.Records.M_clear ();
		for (int i=0; i<length; i++) {
			itemData = _data.Value [i] as ErlArray;
			
			time = StringKit.toInt (itemData.Value [0].getValueString ());
			uid = itemData.Value [1].getValueString ();
			isWin = itemData.Value [2].getValueString () == "true";
			rank = StringKit.toInt (itemData.Value [3].getValueString ());
			enemyName = itemData.Value [4].getValueString ();
			vipLevel = StringKit.toInt (itemData.Value [5].getValueString ());

			recordItem = new LaddersRecordInfo ();
			recordItem.time = time;
			recordItem.index = i + 1;
			recordItem.isWin = isWin;
			recordItem.oppUid = uid;
			recordItem.rank = rank;
			recordItem.enemyName = enemyName;
			recordItem.vipLevel = vipLevel;
			recordItem.creatDes ();
			records [i] = recordItem;
			LaddersManagement.Instance.Records.M_addRecord (recordItem);
		}
	}
}


