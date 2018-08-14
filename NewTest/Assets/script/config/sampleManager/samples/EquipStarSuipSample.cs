using System;

/**
 * 套装模板
 * @author longlingquan
 * */
public class EquipStarSuitSample:Sample
{
	public EquipStarSuitSample ()
	{
	}
	
	public string name = "";//装备名字
	public int iconId = 0;//图标编号
	public int[] parts = null;//部件sids
	public EquipStarSuitAttrChange[] infos = null;//技能产生属性影响效果(影响角色本身属性) 同时对技能描述参数提供数值(影响描述信息数值) 不会影响装备基础属性 
	
	public override void parse (int sid, string str)
	{
		this.sid = sid; 
		string[] strArr = str.Split ('|');  
		//strArr[0] is sid  
		//strArr[1] name
		this.name = strArr [1];
		//strArr[2] iconId
		this.iconId = StringKit.toInt (strArr [2]);
		//strArr[3] parts
		parseParts (strArr [3]);
		//strArr[3] attrChange
		parseSuitAttrChange (sid, strArr [4]);
	}
	
	private void parseParts (string str)
	{
		string[] strArr = str.Split (',');
		parts = new int[strArr.Length];
		for (int i = 0; i < strArr.Length; i++) {
			parts [i] = StringKit.toInt (strArr [i]);
		}
	}
	
	//解析套装属性变化信息
	private void parseSuitAttrChange (int sid, string str)
	{
		string[] strArr = str.Split ('#');
		infos = new EquipStarSuitAttrChange[strArr.Length];
		for (int i = 0; i<strArr.Length; i++) {
			infos [i] = new EquipStarSuitAttrChange (sid);
			infos [i].parse (strArr [i]);
		}
	}
	
	public override void copy (object destObj)
	{
		base.copy (destObj);
		EquipStarSuitSample dest = destObj as EquipStarSuitSample;
		if (this.parts != null) {
			dest.parts = new int[this.parts.Length];
			for (int i = 0; i < this.parts.Length; i++)
				dest.parts [i] = this.parts [i];
		}
		if (this.infos != null) {
			dest.infos = new EquipStarSuitAttrChange[this.infos.Length];
			for (int i = 0; i < this.infos.Length; i++)
				dest.infos [i] = this.infos [i].Clone () as EquipStarSuitAttrChange;
		}
	}
} 

/**
 * 套装属性变化模板(不是标准模板 没sid)
 * */
public class EquipStarSuitAttrChange:CloneObject
{
	public int sid = 0;//对应套装sid
	public int num = 0;//套装数量
	public AttrChangeSample[] effects;
	public string describe = "";//技能描述 模板
	
	public EquipStarSuitAttrChange (int sid)
	{
		this.sid = sid;
	}
	
	public void parse (string str)
	{
		string[] strArr = str.Split ('*'); 
		//strArr[0] num
		this.num = StringKit.toInt (strArr [0]);
		//strArr[1] attrChange
		parseAttrChange (strArr [1]);
		//strArr[2] describe
		this.describe = strArr [2];
	}
	
	private void parseAttrChange (string str)
	{ 
		string[] strArr = str.Split ('$');
		effects = new AttrChangeSample[strArr.Length]; 
		for (int i = 0; i<strArr.Length; i++) {
			AttrChangeSample attr = new AttrChangeSample ();
			attr.parse (strArr [i]);
			effects [i] = attr;
		} 
	}
	
	public override void copy (object destObj)
	{
		base.copy (destObj);
		EquipStarSuitAttrChange dest = destObj as EquipStarSuitAttrChange;
		if (this.effects != null) {
			dest.effects = new AttrChangeSample[this.effects.Length];
			for (int i = 0; i < this.effects.Length; i++)
				dest.effects [i] = this.effects [i].Clone () as AttrChangeSample;
		}
	}
}
