using UnityEngine;
using System.Collections;

public class CombatSample : Sample
{

	public CombatSample ()
	{
	}
	
	public int type;//卡片类型
	public int hpCoe;//生命值系数
	public int attCoe;//攻击值系数
	public int defCoe;//防御值系数
	public int magCoe;//魔法值系数
	public int agiCoe;//敏捷值系数
	public int[] beastSkillCoe;//圣器技能系数（幻兽技能）
	
	public override void parse (int sid, string str)
	{
		this.sid = sid;
		string[] strArr = str.Split ('|');
		this.type = StringKit.toInt (strArr [0]);
		this.hpCoe = StringKit.toInt (strArr [1]);
		this.attCoe = StringKit.toInt (strArr [2]);
		this.defCoe = StringKit.toInt (strArr [3]);
		this.magCoe = StringKit.toInt (strArr [4]);
		this.agiCoe = StringKit.toInt (strArr [5]);
		parseResolve (strArr [6]);
	}

	//解析分解结果
	private void parseResolve (string str)
	{
		string[] strArr = str.Split (',');
		if (strArr.Length == 1) {
			beastSkillCoe = null;
			return;
		}
		beastSkillCoe = new int[strArr.Length];
		for (int i = 0; i < strArr.Length; i++) {
			beastSkillCoe [i] = StringKit.toInt (strArr [i]);
		}
		
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
		CombatSample dest = destObj as CombatSample;
		if (this.beastSkillCoe != null) {
			dest.beastSkillCoe = new int[this.beastSkillCoe.Length];
			for (int i = 0; i < this.beastSkillCoe.Length; i++)
				dest.beastSkillCoe [i] = this.beastSkillCoe [i];
		}
	}
}
