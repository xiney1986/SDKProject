using UnityEngine;
using System.Collections;

/// <summary>
/// 最大购买数量文件
/// </summary>
public class GoodsBuyCountSample : Sample {

	public GoodsBuyCountSample(){
	}
	public GoodsBuyCountSample(string str){
		parse(str);
	}
	/**filed */
	/**模板sid */
	public int sampleSid;
	/**对应goods配置的Sid */
	public int goodsSid;
	/**对应的vip限制的购买次数vip0 -----vip12 */
	public int[] vipMaxCount;
	public bool useThisConst;//是否使用这个价格
	public int[] prise;//对应的购买价格

	/**method*/
	public  void parse (string str)
	{
		string[] strArr = str.Split ('|');
		checkLength (strArr.Length, 4);
		this.sampleSid =StringKit.toInt (strArr [0]);
		this.goodsSid=StringKit.toInt (strArr [1]);
		parseMaxCount(strArr[2]);
		this.useThisConst=StringKit.toInt (strArr [3])==1;
		parsePrise(strArr[4]);
	}
	private void parseMaxCount(string str){
		string[] st=str.Split(',');
		vipMaxCount=new int[st.Length];
		for(int i=0;i<st.Length;i++){
			vipMaxCount[i]=StringKit.toInt(st[i]);
		}
	}
	private void parsePrise(string str){
		string[] st=str.Split(',');
		prise=new int[st.Length];
		for(int i=0;i<st.Length;i++){
			prise[i]=StringKit.toInt(st[i]);
		}
	}
}
