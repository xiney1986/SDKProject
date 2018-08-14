using UnityEngine;
using System.Collections;

/// <summary>
/// Npc 结构体
/// </summary>
public class NpcData
{
	public string uid;
	public int  mountsSid;//坐骑sid
	public	string name ;
	public string style;
	public	int vipLevel ;
	public int level ;
	public int mPointIndex = -1;//当前所在逻辑点 -1代表没入场
	public int titleSid=1;//npc称号
	public int medalSid=1;//npc荣誉
	public const int NPC_STATE_WAITFORSTART = -1;
	public const int NPC_STATE_STANDY = 1;
	public const int NPC_STATE_MOVE = 2;
	public const int NPC_STATE_FINISH = 3;
	public int state = NPC_STATE_WAITFORSTART;
	public MissionNpcCtrl ctrl;
}
