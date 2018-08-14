using UnityEngine;
using System.Collections;

public class Plot:CloneObject
{

	public	int plotType;//1 指定回合对话 2:NPC上场时触发对话
	public	int count;//  类型1:第几回触发对话 类型2:npc上场的次数
	public	int beginSid;//类型1的时候只用这个 类型2:npc上场后的说话ID
	public	int endSid;//类型3:npc下场前的说话ID

	public override void copy (object destObj)
	{
		base.copy (destObj);
	}
}
