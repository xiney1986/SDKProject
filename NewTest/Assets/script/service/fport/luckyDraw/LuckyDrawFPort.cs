using System;
 
public class LuckyDrawFPort:BaseFPort
{
	DrawWay drawWay;

	public LuckyDrawFPort ()
	{
		
	}

	private const string NOT_COUNT = "not_count";
	private CallBackLuckyDrawResults callback;
	
	public void luckyDraw (int times, int sid, int wayIndex, DrawWay way, CallBackLuckyDrawResults callback)
	{
		drawWay = way;
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.LUCKY_DRAW); 
		message.addValue ("times", new ErlInt (times));//次数
		message.addValue ("ldid", new ErlInt (sid));//抽奖条目id
		message.addValue ("choose", new ErlInt (wayIndex));//抽奖方式id
		access (message);
		if (GuideManager.Instance.isEqualStep (7004000)) {
			GuideManager.Instance.doGuide ();
		}
	}
	/** 限时抽奖活动 */
	public void luckyDrawByNotice (int times, int sid, int wayIndex, int noticeSid, DrawWay way, CallBackLuckyDrawResults callback)
	{
		drawWay = way;
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.LUCKY_DRAW); 
		message.addValue ("times", new ErlInt (times));//次数
		message.addValue ("ldid", new ErlInt (sid));//抽奖条目id
		message.addValue ("choose", new ErlInt (wayIndex));//抽奖方式id
		message.addValue ("noticeSid", new ErlInt (noticeSid));//活动sid
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;
		if (type is ErlArray) {
		
			ErlArray arr = type as ErlArray; 
			LuckyDrawResults result = new LuckyDrawResults ();
			result.drawWay = drawWay;
			for (int i = 0; i < arr.Value.Length; i++) { 
				result.parse (arr.Value [i] as ErlArray);
			}
		
			for (int i = 0; i < result.getSinglePrizes().Count; i++) {
				if (result.getSinglePrizes () [i].type == LuckyDrawPrize.TYPE_RMB) {
					UserManager.Instance.self.updateRMB (UserManager.Instance.self.getRMB () + result.getSinglePrizes () [i].num);
				} else if (result.getSinglePrizes () [i].type == LuckyDrawPrize.TYPE_MONEY) {
					UserManager.Instance.self.updateMoney (UserManager.Instance.self.getMoney () + result.getSinglePrizes () [i].num);
				}
			}
			LuckyDrawManagerment.Instance.updateNextTime(StringKit.toInt ((message.getValue ("sid") as ErlType).getValueString ()),
			                                             StringKit.toInt ((message.getValue ("next_time") as ErlType).getValueString ()));
			if (callback != null)
				callback (result);
		} else {
			MaskWindow.UnlockUI ();
			if (callback != null)
				callback = null;
		}
	} 
} 

