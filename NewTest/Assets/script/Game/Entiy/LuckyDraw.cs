using System;
using System.Collections.Generic;
 
/**
 * 抽奖实体对象
 * @author longlingquan
 * */
public class LuckyDraw
{
	public int sid;
	public DrawWay[] ways;
	public int lastFreeNum = 0;//花费掉的免费抽奖次数
	private int nextFreeTime = -1; //下次免费CD时间，-1表示没有免费CD
	public bool freeTimeChange; //免费抽奖CD改变
	
	public const string OVER = "over";//奖励时间结束

	/** true表示可以免费抽奖了 */
	public bool canFreeCD()
	{
		return nextFreeTime == -1 ? false : ServerTimeKit.getSecondTime () >= nextFreeTime;
	}

	public LuckyDraw (int sid)
	{
		this.sid = sid;
		initDrawWays ();		
	}
	
	public void updateFreeNum (int times)
	{
		this.lastFreeNum = times;
	}

	public int getNextFreeTime ()
	{
		return nextFreeTime;
	}

	public void setNextFreeTime (int _time)
	{
		nextFreeTime = _time;
	}

	//不同时间表示执行了免费次数
	public void updateNextFreeTime (int _time)
	{
		if (nextFreeTime != _time) {
			nextFreeTime = _time;
			freeTimeChange = true;
		}
	}

	public LuckyDrawSample getSample ()
	{
		return LuckyDrawSampleManager.Instance.getLuckyDrawSampleBySid (sid);
	}

	public int getLuckyIndex ()
	{
		return LuckyDrawSampleManager.Instance.getLuckyDrawSampleBySid (sid).luckyIndex;
	}
	
	public int getIconId ()
	{
		return LuckyDrawSampleManager.Instance.getLuckyDrawSampleBySid (sid).iconId;
	}
	
	//获得当前免费抽奖次数
	public int getFreeNum ()
	{
		return lastFreeNum;
	}
	
	//是否显示
	public bool isShow ()
	{
		if (getTimeInfo () == OVER)
			return false;
		if (getDrawNum () != 0 && getCostDrawNum () == getDrawNum ())
			return false;
		return true;
	}
	
	//初始化抽奖方式信息
	private void initDrawWays ()
	{ 
		DrawWaySample[] samples = LuckyDrawSampleManager.Instance.getLuckyDrawSampleBySid (sid).ways;
		ways = new DrawWay[samples.Length];
		for (int i=0; i<ways.Length; i++) {
			ways [i] = new DrawWay (samples [i]);
		} 
	}
	
	//获得抽奖币 当前数量
	public int getShowCostNum ()
	{
		if (ways [0].getCostType () == PrizeType.PRIZE_RMB)
			return UserManager.Instance.self.getRMB ();
		else if (ways [0].getCostType () == PrizeType.PRIZE_MONEY)
			return UserManager.Instance.self.getMoney ();
		else if (ways [0].getCostType () == PrizeType.PRIZE_PROP) {
			Prop p = StorageManagerment.Instance.getProp (ways [0].getCostToolSid ());
			if (p == null)
				return 0;
			else 
				return p.getNum ();
		}
		return -1;
	}

	public int getCostToolSid ()
	{
		return ways [0].getCostToolSid ();
	}
	
	//描述
	public string getDescribe ()
	{
		return LuckyDrawSampleManager.Instance.getLuckyDrawSampleBySid (sid).describe;
	}
	
	//标题
	public string getTitle ()
	{
		return LuckyDrawSampleManager.Instance.getLuckyDrawSampleBySid (sid).name;
	}
	
	//获得奖励信息id数组
	public int[] getPrizesInfo ()
	{
		return LuckyDrawSampleManager.Instance.getLuckyDrawSampleBySid (sid).ids;
	}
	
	//获得配置抽奖次数
	public int getDrawNum ()
	{
		return LuckyDrawSampleManager.Instance.getLuckyDrawSampleBySid (sid).drawNum;
	}
	
	public bool isFreeDraw ()
	{
//		if (lastFreeNum > 0) {
//			return true;
//		} else {
//			return false;
//		}
		return lastFreeNum > 0 || canFreeCD ();
	}
	
	//获得消耗掉的抽奖次数
	public int getCostDrawNum ()
	{
		int num = 0;
		for (int i = 0; i < ways.Length; i++) {
			num += ways [i].getNum ();
		}
		return num;
	}
	
	public bool isCost (int index)
	{
		if (getDrawNum () == -1)
			return true;
		int num = ways [index].getDrawTimes ();//抽奖条目消耗次数
		int remainNum = getDrawNum () - getCostDrawNum ();//剩余抽奖次数
		if (remainNum < num)
			return false;
		else
			return true;
	}
	
	public bool isTimeStart ()
	{
		if (LuckyDrawSampleManager.Instance.getLuckyDrawSampleBySid (sid).startTime > ServerTimeKit.getSecondTime ()) 
			return false;
		return true;
	}
	 
	//获得时间描述信息
	public string getTimeInfo ()
	{
		int startTime = LuckyDrawSampleManager.Instance.getLuckyDrawSampleBySid (sid).startTime;
		int endTime = LuckyDrawSampleManager.Instance.getLuckyDrawSampleBySid (sid).endTime;
		if (startTime != 0 && endTime == 0) {
			if (startTime > ServerTimeKit.getSecondTime ()) 
				return timeTransform (startTime - ServerTimeKit.getSecondTime ()) + LanguageConfigManager.Instance.getLanguage ("s0022");
			else
				return "";
		} else if (startTime == 0 && endTime != 0) {
			if (endTime > ServerTimeKit.getSecondTime ()) 
				return timeTransform (endTime - ServerTimeKit.getSecondTime ()) + LanguageConfigManager.Instance.getLanguage ("s0024");
			else
				return "";
		} else if (startTime != 0 && endTime != 0) {
			//未开启
			if (startTime > ServerTimeKit.getSecondTime ()) 
				return timeTransform (startTime - ServerTimeKit.getSecondTime ()) + LanguageConfigManager.Instance.getLanguage ("s0022");
			//结束
			if (ServerTimeKit.getSecondTime () > endTime)
				return OVER;
			else 
				return timeTransform (endTime - ServerTimeKit.getSecondTime ()) + LanguageConfigManager.Instance.getLanguage ("s0024");
		} else {
			return "";
		}
	}
	
	//转换时间格式 单位:秒  
	private string timeTransform (double time)
	{  
		int days = (int)(time / (3600 * 24));
		string dStr = "";
		if (days != 0)
			dStr = days + LanguageConfigManager.Instance.getLanguage ("s0018");
		
		int hours = (int)(time % (3600 * 24) / 3600);
		string hStr = "";
		if (hours != 0)
			hStr = hours + LanguageConfigManager.Instance.getLanguage ("s0019");
		 
		int minutes = (int)(time % (3600 * 24) % 3600 / 60);
		string mStr = "";
		if (minutes != 0)
			mStr = minutes + LanguageConfigManager.Instance.getLanguage ("s0020");
		 
		int seconds = (int)(time % (3600 * 24) % 3600 % 60);
		string sStr = "";
		if (seconds != 0)
			sStr = seconds + LanguageConfigManager.Instance.getLanguage ("s0021");
		 
		return dStr + hStr + mStr + sStr;
	}
	//获取相关公告sid
	public int getNoticeSid ()
	{
		return LuckyDrawSampleManager.Instance.getLuckyDrawSampleBySid (sid).noticeSid;
	}
	//获得奖励信息类型
	public string getPrizesType ()
	{
		string type = LuckyDrawSampleManager.Instance.getLuckyDrawSampleBySid (sid).idsType;
		if (type == "1")
			return "card";
		else if (type == "2")
			return "equip";
		else if (type == "3")
			return "prop";
		return "";
	}
	 
	public void updateLuckyWay (int typeId, int num)
	{
		if (ways == null) {
			throw new Exception (GetType () + "updateLuckyWay ways is null");
		}
		for (int i = 0; i < ways.Length; i++) {
			if (ways [i].getDrawTypeId () == typeId) {
				ways [i].updateNum (num);
			}
		}	
	}
	
	//获得下一次抽奖花费货币数量
	public string getCostNumInfo ()
	{
		if (getFreeNum () > 0)
			return LanguageConfigManager.Instance.getLanguage ("s0023", lastFreeNum + "");
		else
			return "x" + ways [0].getCostPrice (lastFreeNum); 
	}
	//jordenwu
	public int getCostNum ()
	{
		return ways [0].getCostPrice (lastFreeNum);
	}

	
	//刷新抽奖计数 (抽奖成功后)
	public void drawLucky (int index)
	{
		//免费次数也要计数
		if (lastFreeNum > 0) {
			int dTimes = ways [index].getDrawTimes ();
			if (lastFreeNum > dTimes) {
				lastFreeNum -= dTimes;
			} else {
				lastFreeNum = 0;
			}
		} 
		ways [index].drawLucky ();
	}
	 
}

//抽奖具体方式
public class DrawWay
{ 
	private int num = 0;//抽奖次数(只包含花钱抽奖次数)
	private DrawWaySample sample;

	public DrawWay (DrawWaySample sample)
	{ 
		this.sample = sample; 
	}

	public void updateNum (int num)
	{
		this.num = num;
	}
	
	public int getWaySid ()
	{
		return sample.drawTypeId;
	}
	
	public int getNum ()
	{
		return num;
	}
	
	public void drawLucky ()
	{
		num = num + sample.drawTimes;
	}
	 
	/**获得花费金钱*/
	public int getCostPrice (int freeNum)
	{
		if (freeNum > 0)
			return 0; 
		return FormulaManagerment.formula (sample.formulaId, (num / sample.drawTimes) + 1, sample.factors);
	}
	 
	//获得抽奖类型
	public int getDrawTypeId ()
	{
		return sample.drawTypeId;
	}
	
	public int getDrawTimes ()
	{
		return sample.drawTimes;
	}
	 
	//获得消耗类型
	public int getCostType ()
	{
		return sample.costType;
	}
	  
	//获得消耗道具sid
	public int getCostToolSid ()
	{
		return sample.costToolSid;
	}
	 
	//获得抽奖按钮描述
	public string getInfo (int freeNum)
	{
		//免费次数用完 返回配置数字
		if (freeNum <= 0)
			return LanguageConfigManager.Instance.getLanguage ("s0026", sample.drawTimes.ToString ()); 
		//配置次数大于免费次数 显示免费次数
		if (sample.drawTimes > freeNum)
			return LanguageConfigManager.Instance.getLanguage ("s0026", freeNum.ToString ());
		else
			return LanguageConfigManager.Instance.getLanguage ("s0026", sample.drawTimes.ToString ()); 
	}   
	 
}

