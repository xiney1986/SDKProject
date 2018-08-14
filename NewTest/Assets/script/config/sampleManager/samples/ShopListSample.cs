using UnityEngine;
using System.Collections;

/// <summary>
/// 商店列表配置模板 
/// </summary>
public class ShopListSample : Sample {

	public ShopListSample(){
	}
	public ShopListSample(string str){
		parse(str);
	}
	/**filed */
	public string name = "";//商店名称
	public int iconId=0;//商店的背景图标
	public int timeFlag=0;//是否有刷新倒计时 0没有 1有
	public int activeLv=0;//开放的等级
	public string dec="";//文字描述
	public string shopLag="";//打开对应商店的名字
	public int timee=0;

	public  void parse (string str)
	{
		string[] strArr = str.Split ('|');
		checkLength (strArr.Length, 7);
		
		//strArr[0] is sid  
		//strArr[1] name
		this.name = strArr [1];
		//strArr[2] iconId
		this.iconId = StringKit.toInt (strArr [2]); 
		//strArr[3] timeFlag
		this.timeFlag=StringKit.toInt(strArr[3]);
		//strArr[4] activeLv
		this.activeLv=StringKit.toInt(strArr[4]);
		//strArr[5] dec
		this.dec=strArr[5];
		this.shopLag=strArr[6];
		this.timee=StringKit.toInt(strArr[7]);
	}

}
