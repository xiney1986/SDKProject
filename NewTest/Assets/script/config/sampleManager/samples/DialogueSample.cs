using System;
 
/**对白模板 
  *@author longlingquan
  **/

public class DialogueSample:Sample
{ 
	public DialogueSample ()
	{
	}

	public string name = "";//说话人
	public int iconId = 0;//图标id
	public int intoLoc = 0;//进入方式 1  从上面进入 2  从右边进入 3  从下面进入4  从左边进入
	public int loction = 0;//最终位置 1左 2右 3中间
	public int shake = 0;//震动级别 1-5 1最弱最短 5最强最长
	public string[] dialogues;//对白集合
	
	public override void parse (int sid, string str)
	{
		this.sid = sid;
		string[] strArr = str.Split ('|'); 
		checkLength (strArr.Length, 6);
		//strArr[0] is sid  
		//strArr[1] name
		this.name = strArr [1];
		//strArr[2] iconId
		this.iconId = StringKit.toInt (strArr [2]); 
		//strArr[3] intoLoc
		this.intoLoc = StringKit.toInt (strArr [3]); 	
		//strArr[4] loction
		this.loction = StringKit.toInt (strArr [4]); 
		//strArr[5] shake
		this.shake = StringKit.toInt (strArr [5]); 
		parseDialogue (strArr [6]);
	}
	
	private void parseDialogue (string str)
	{
		dialogues = str.Split ('#');
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
		DialogueSample dest = destObj as DialogueSample;
		if (this.dialogues != null) {
			dest.dialogues = new string[this.dialogues.Length];
			for (int i = 0; i < this.dialogues.Length; i++)
				dest.dialogues [i] = this.dialogues [i];
		}
	}
} 

