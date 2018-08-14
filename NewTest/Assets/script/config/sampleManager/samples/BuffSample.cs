using System;
 
/**buff模板 
  *@author longlingquan
  **/
public class BuffSample:Sample
{
	public BuffSample ()
	{
	}
	
	public string name = "";//buff名称 
	public bool isDuration = false;//是否为持续型buff 
	public int displayType ;//buff显示类型
	public int resPath;//buff显示资源路径
	public int type;//buff类型 
	public int damageEffect;//持续buff触发时候的效果
	public int damageType;//buff持续伤害显示类型
	public string transformID;//如果有图片ID就是变形buff
    public string skillIcon;//技能图标

	public override void parse (int sid, string str)
	{ 
		this.sid = sid;
		string[] strArr = str.Split ('|'); 
		checkLength (strArr.Length, 9);
		
		//strArr[0] is sid  
		//strArr[1] name
		this.name = strArr [1];
		//strArr[2] buffSid
		parseIsDuration (strArr [2]); 
		//strArr[3] displayType
		this.displayType = StringKit.toInt (strArr [3]);
		//strArr[4] resPath
		this.resPath = StringKit.toInt (strArr [4]);
		//strArr[5] type
		this.type = StringKit.toInt (strArr [5]);
		//strArr[6] damageEffect
		this.damageEffect = StringKit.toInt (strArr [6]);
		//strArr[7] damageType
		this.damageType = StringKit.toInt (strArr [7]);
		this.transformID = strArr [8];
        this.skillIcon = strArr[9];
	}
	
	private void parseIsDuration (string str)
	{
		if (str == "1")
			this.isDuration = true;
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
	}
} 

