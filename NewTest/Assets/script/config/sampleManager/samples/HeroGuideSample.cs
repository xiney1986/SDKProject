using UnityEngine;
using System.Collections;

/// <summary>
/// 解锁英雄模板文件
/// </summary>
public class HeroGuideSample : Sample {

	public HeroGuideSample(){
	}
	public HeroGuideSample(string str){
		parse(str);
	}
	/**filed */
	/**模板sid */
	public int sampleSid;
	/**需要通关的副本SID(第一次进去) */
	public int missionSid;
	/**第几个点（走几步） */
	public int pointNum;
	/**显示解封动画0不显示1展示2真实解封 */
	public int showFlag;
	/**解封此英雄的步数 */
	public int stepNum;
	/**解锁的奖励 */
	public PrizeSample[] prizeSample;
	/**执行的最小等级 */
	public int minLv;
	/**执行的最大等级 */
	public int maxLv;
	/**是否一直显示图标 */
	public int isShow;
	/**描述 */
	public string dec;
	/**播放动画有没有对话 */
	public int haveTalk;
	public int beginChapter;
	public int endChapter;

	/**method*/
	public  void parse (string str)
	{
		string[] strArr = str.Split ('|');
		checkLength (strArr.Length, 12);
		this.sampleSid =StringKit.toInt (strArr [0]);
		this.missionSid=StringKit.toInt (strArr [1]);
		this.pointNum=StringKit.toInt(strArr[2]);
		this.showFlag=StringKit.toInt(strArr[3]);
		this.stepNum=StringKit.toInt(strArr[4]);
		this.minLv=StringKit.toInt(strArr[5]);
		this.maxLv=StringKit.toInt(strArr[6]);
		this.isShow=StringKit.toInt(strArr[7]);
		this.prizeSample=parsePrizes(strArr[8]);
		this.dec=strArr[9];
		this.haveTalk=StringKit.toInt(strArr[10]);
		this.beginChapter=StringKit.toInt(strArr[11]);
		this.endChapter=StringKit.toInt(strArr[12]);
	}
	private PrizeSample[] parsePrizes (string str)
	{
		string[] strArr = str.Split ('#');
		PrizeSample[] prizes = new PrizeSample[strArr.Length];
		for (int i = 0; i < strArr.Length; i++) {
			prizes[i]=new PrizeSample();
			string[] strs = strArr[i].Split(',');
			prizes[i].type = StringKit.toInt(strs[0]);
			prizes[i].pSid = StringKit.toInt(strs[1]);
			prizes[i].num = strs[2];
		}
		return prizes;
	}
}
