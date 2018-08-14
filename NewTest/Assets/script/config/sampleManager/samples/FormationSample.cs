using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/**阵型模板
  *@author 汤琦
  **/
public class FormationSample : Sample
{
	
	public FormationSample ()
	{
	}

	public string name = "";//阵型名
	public int iconId = 0;//阵型图Id
	public int effectId = 0;//特效类型Id
	public int[] formations;//阵型点位
	public int openLevel = 0;//开放
	public int closeLevel = 0;//关闭
	public int upSid = 0;//升级后sid
	public List<string> formationMap;//阵型图形
	int teamLength = 0;
	//解析阵型信息,分别存入相应的变量
	public int  getLength ()
	{
	 
		return teamLength;
	}
	
	public override void parse (int sid, string str)
	{
		this.sid = sid;
		string[] strArr = str.Split ('|');
		checkLength (strArr.Length, 8);
		this.name = strArr [1];
		this.iconId = StringKit.toInt (strArr [2]);
		this.effectId = StringKit.toInt (strArr [3]);
		parseForm (strArr [4]);
		this.openLevel = StringKit.toInt (strArr [5]);
		this.closeLevel = StringKit.toInt (strArr [6]);
		this.upSid = StringKit.toInt (strArr [7]);
		parseMap(strArr[8]);
		
	}
	private void parseMap(string str){
		string[] strArr = str.Split (',');
		for (int i = 0; i < strArr.Length; i++) {
			if(formationMap==null) formationMap=new List<string>();
			formationMap.Add(strArr [i]); 
		}
	}
	//解析阵型点位信息
	private void parseForm (string str)
	{
		string[] strArr = str.Split (',');
		formations = new int[strArr.Length];
		
		
		for (int i = 0; i < strArr.Length; i++) {
			formations [i] = StringKit.toInt (strArr [i]); 
			if (formations [i] != 0)
				teamLength += 1;
		}
		

	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
		FormationSample dest = destObj as FormationSample;
		if (this.formations != null) {
			dest.formations = new int[this.formations.Length];
			for (int i = 0; i < this.formations.Length; i++)
				dest.formations [i] = this.formations [i];
		}
	}
}
