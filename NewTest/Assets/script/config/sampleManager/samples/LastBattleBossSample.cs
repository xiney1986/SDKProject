using System;
 
public class LastBattleBossSample:Sample
{
	public LastBattleBossSample()
	{
		
	}

	public string name = "";//Boss名字
	public string imageID = "";//Boss图标编号
    public string nameID;//boss名字美术字id

	override public void parse (int sid, string str)
	{ 
		this.sid = sid;
		string[] strArr = str.Split ('|');

		this.name = strArr [1];
        this.nameID = strArr[2];
		this.imageID = strArr [3]; 
	}
    public override void copy(object destObj) {
        base.copy(destObj);
    }
} 

