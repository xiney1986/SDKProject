using System;
using System.Collections;
using System.Collections.Generic;
 
/**爬塔章节奖励模板 
  *@author longlingquan
  **/
public class TowerAwardSample:Sample
{
    public TowerAwardSample()
	{
	}

    public PrizeSample[] prizes;//通关奖励
    public int havePassPrize;//是否有通过宝箱
	
	public override void parse (int sid, string str)
	{
		this.sid = sid;//这个aid对应章节的sid
		string[] strArr = str.Split ('|');
        this.havePassPrize = StringKit.toInt(strArr[1]);
        parsePrizes(strArr[2]);
		
	}
    //解析通关奖励
    private void parsePrizes(string str) {
        string[] strArr = str.Split('#');
        int max = strArr.Length;
        if (max == 0)
            return;
        prizes = new PrizeSample[max];
        for (int i = 0; i < max; i++) {
            prizes[i] = new PrizeSample(strArr[i], ',');
        }
    }
    public override void copy(object destObj) {
        base.copy(destObj);
    }
    public PrizeSample[] getPrizes() {
        return prizes;
    }
    public bool havePrize() {
        return havePassPrize == 1;
    }
} 

