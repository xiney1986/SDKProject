
using System;
/// <summary>
/// 单个宝箱刷新
/// </summary>
public class LaddersChestRefreshFPort:BaseFPort
{
	public LaddersChestRefreshFPort ()
	{
	}
	
	private CallBack callback;
	public void apply(int _chestIndex,CallBack _callback)
	{  		
		this.callback = _callback;	
		ErlKVMessage message = new ErlKVMessage (FrontPort.LADDERS_CHEST_REFRESH);	
		message.addValue("group",new ErlInt(_chestIndex));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlArray itemData=message.getValue("msg") as ErlArray;

		int index = 0;
		int refreshTime;
		int multiple;
		int userRankAtRefresh;
		LaddersChestInfo chestItem;

		index = StringKit.toInt (itemData.Value [0].getValueString ());
		refreshTime = StringKit.toInt (itemData.Value [1].getValueString ());
		multiple = StringKit.toInt (itemData.Value [2].getValueString ());
		userRankAtRefresh = StringKit.toInt (itemData.Value [3].getValueString ());
		chestItem = new LaddersChestInfo ();
		chestItem.index = index;
		chestItem.multiple = multiple;
		chestItem.canReceiveRank = userRankAtRefresh;

		LaddersManagement.Instance.Chests.M_updateChest(index-1,chestItem);
		LaddersManagement.Instance.M_updateChestStatus();
		if (callback != null)
		{
			callback();
			callback = null;
		}
	}
}