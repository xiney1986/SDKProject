using System;
 
/**boss信息模板   
 * 详见配置说明文件
 *@author longlingquan
 **/
public class BossInfoSample:Sample
{
    public BossInfoSample()
	{
		
	}

	public string name = "";//Boss名字
	public int imageID = 0;//Boss图标编号
    public string weakDesc = "";//Boss弱点描述
    public string nameID;//boss名字美术字id

	override public void parse (int sid, string str)
	{ 
		this.sid = sid;
		string[] strArr = str.Split ('|');
        checkLength(strArr.Length, 4);
		//strArr[0] is sid  
		//strArr[1] name
		this.name = strArr [1];
        this.nameID = strArr[2];
		this.imageID = StringKit.toInt (strArr [3]); 
        this.weakDesc = strArr[4];
	}
    public override void copy(object destObj) {
        base.copy(destObj);
    }
} 

