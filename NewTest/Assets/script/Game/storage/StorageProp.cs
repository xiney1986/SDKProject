using System;

/**
 * 仓库道具
 * @author zhoujie
 * */
public class StorageProp:CloneObject
{
	/** 默认道具最大堆叠数量 */
	const int maxLimitNum = 999999999;
	/** 道具样本sid */
	public int sid;
	/** 道具唯一id */
	public string uid;
	/** 道具数量，默认1 */
	private int num = 1;
	/** 在包裹的索引，从包裹获得
	 * 指定sid(sid数组,uid,uid数组)的道具时才有效，
	 * 删除过包裹道具后失效 
	 */
	public int index;
	/** 唯一道具标示，true唯一道具 */
	public bool isU;

	/** 比较两个道具是否是相同道具，子类必须重写此方法 */
	public virtual bool equal (StorageProp prop)
	{
		return false;
	}

	/** 获得数量 */
	public int getNum ()
	{
		return this.num;
	}
	/** 设置数量 */
	public void setNum (int num)
	{
		int mn = getMaxNum ();
		this.num = num;
		if (this.num > mn)
			this.num = mn;
	}
	/** 减少数量 */
	public void reduceNum (int num)
	{
		this.num -= num;
	}

	/** 添加道具数量，超出最大堆叠数量的抛掉 */
	public void addNum (int num)
	{
		int mn = getMaxNum ();
		this.num += num;
		if (this.num > mn)
			this.num = mn;
	}
	/** 需要改变最大堆叠数量，子类覆盖此方法 */
	public int getMaxNum ()
	{
        //if (sid == CommandConfigManager.Instance.getHuiJiMoneySid()) {//英雄徽记（有上限）
        //    return CommandConfigManager.Instance.getMaxNum();
        //}
//		if(sid == CommandConfigManager.Instance.lastBattleData.junGongSid)// 军功堆叠上限//
//		{
//			return CommandConfigManager.Instance.lastBattleData.junGongMaxNum;
//		}
		return maxLimitNum;
	}
	/** 仓库反序列化道具 */
	public virtual void bytesRead(int j, ErlArray ea){

	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
	}
}
