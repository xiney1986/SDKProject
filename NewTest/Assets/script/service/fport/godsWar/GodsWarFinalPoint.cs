using UnityEngine;
using System.Collections;
/// <summary>
/// 淘汰赛(决赛)战报(支持)点位信息
/// </summary>
public class GodsWarFinalPoint 
{
	/// <summary>
	/// 点位id
	/// </summary>
	public int localID;
	/// <summary>
	/// 战报ids
	/// </summary>
	public string[] replayIDs;
	/// <summary>
	/// 对战玩家
	/// </summary>
	public GodsWarFinalUserInfo[] players;
	/// <summary>
	/// 最终赢家
	/// </summary>
	public GodsWarFinalUserInfo winner;
	/// <summary>
	/// 是否产出胜者
	/// </summary>
	public int isHaveWinner = 0;
	/// <summary>
	/// 支持的玩家
	/// </summary>
	public GodsWarFinalUserInfo suporter;
	/// <summary>
	/// 是否有支持过
	/// </summary>
	public int isSuport = 0;
	/// <summary>
	/// 最终输家
	/// </summary>
	public GodsWarFinalUserInfo loser;
	/// <summary>
	/// 单场胜利者列表
	/// </summary>
	public GodsWarFinalUserInfo[] winnerSingle;


	public  GodsWarFinalPoint()
    {
    }

	/// <summary>
	/// 解析元数据
	/// </summary>
    public void bytesRead(ErlArray erl)
    { 
		if(erl.Value.Length==0)return;
		int pos=0;
		//取得点位id
		localID = StringKit.toInt(erl.Value[pos++].getValueString());

		//取得对战玩家
		players = new GodsWarFinalUserInfo[2];
		for (int i = 0; i < players.Length; i++) {
			players[i] = new GodsWarFinalUserInfo();
			players[i].bytesReadTwo(erl.Value[pos++] as ErlArray);		
		}

		//取得赢家
		ErlType mp = erl.Value[pos++] as ErlType;
		if(mp is ErlArray){
			winner = new GodsWarFinalUserInfo();
			winner.bytesReadTwo(mp as ErlArray);
		}
		else {
			isHaveWinner = -1;
		}

		//取得输家
		for (int i = 0; i < players.Length; i++) {
			if(isHaveWinner!=-1)
			{
				if(players[i].serverName!=winner.serverName||players[i].uid != winner.uid)
				{
					loser = new GodsWarFinalUserInfo();
					loser = players[i];
				}
			}
		}

		//取得战报ids
		ErlArray arr = erl.Value[pos++] as ErlArray;
		replayIDs = new string[arr.Value.Length];
		for (int i = 0; i < replayIDs.Length; i++) {
			replayIDs[i] = arr.Value[i].getValueString();
		}

		//获得胜利者列表(与战报对应)
		ErlArray tt = erl.Value[pos++] as ErlArray;
		winnerSingle = new GodsWarFinalUserInfo[tt.Value.Length];
		for (int i = 0; i < tt.Value.Length; i++) {
			winnerSingle[i] = new GodsWarFinalUserInfo();
			winnerSingle[i].bytesReadTwo(tt.Value[i] as ErlArray);		
		}

		//取得被支持的玩家
		ErlType pt = erl.Value[pos++] as ErlType;
		if(pt is ErlArray){
			suporter = new GodsWarFinalUserInfo();
			suporter.bytesReadTwo(pt as ErlArray);
		}
		else {
			isSuport = -1;
		}
    }

}
