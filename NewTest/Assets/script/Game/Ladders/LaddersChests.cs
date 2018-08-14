using System;
using System.Collections.Generic;

/// <summary>
/// 宝箱数据存储
/// </summary>
public class LaddersChests
{
	private LaddersChestInfo[] chests;
	public LaddersChests (int _count)
	{
		chests=new LaddersChestInfo[_count];
	}

	public void M_updateChest(LaddersChestInfo[] _chests)
	{
		for(int i=0,length=_chests.Length;i<length;i++)
		{
			chests[i]=_chests[i];
		}
	}
	public void M_updateChest(int _index,LaddersChestInfo _id)
	{
		chests[_index]=_id;
	}

	public LaddersChestInfo[] M_getChests()
	{
		return chests;
	}
	public LaddersChestInfo M_getChest(int _index)
	{
		return chests[_index];
	}
	public void M_clear()
	{
		for(int i=0,length=chests.Length;i<length;i++)
		{
			chests[i]=null;
		}
	}
	public bool M_checkHasReceiveChest()
	{
		for(int i=0,length=chests.Length;i<length;i++)
		{
			if(chests[i].receiveEnble)
			{
				return true;
			}
		}
		return false;
	}
}

