using System;
 
/**
 * 属性改变模板(不是标准模板 没sid)
 * @author longlingquan
 * */
public class AttrChangeSample:CloneObject
{
	//属性类型
	private string type = "";
	//属性值
	private int init = 0; 
	//成长值
	private int grow = 0; 
	//等级影响数组（用于非线性增长的属性)
	private string[] lvValueArr = null;
	//类型,基础百分比,基础绝对值,成长百分比,成长绝对值
	public void parse (string str)
	{ 
		string[] strArr = str.Split (',');    
		if (strArr.Length < 2)
			throw new Exception ("skill effect error! str" + str);
		if (strArr.Length == 3) {
			this.type = strArr [0];
			this.init = StringKit.toInt (strArr [1]);
			this.grow = StringKit.toInt (strArr [2]); 
		} else { 
			this.type = strArr [0];
			this.lvValueArr = strArr;
		}
	} 
		
	//得到对应等级 属性值 
	public int getAttrValue (int level)
	{
		int c = 0;
		if (level > 0) {
			c = level - 1;
		} else {
			c = 0;
		}
		if (lvValueArr != null) //貌似有问题呢
			return StringKit.toInt (lvValueArr [c]);
		
		return init + (c) * grow; 
	}
	  
	//获得影响类型
	public string getAttrType ()
	{
		return this.type;
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
		AttrChangeSample dest = destObj as AttrChangeSample;
		if (this.lvValueArr != null) {
			dest.lvValueArr = new string[this.lvValueArr.Length];
			for (int i = 0; i < this.lvValueArr.Length; i++)
				dest.lvValueArr [i] = this.lvValueArr [i];
		}
	}
} 

