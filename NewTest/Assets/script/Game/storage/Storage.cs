using System;
using System.Collections;

/**
 * 仓库
 * @author zhoujie
 * */
public class Storage
{

	// **********************const******************************** /

	// **********************variable******************************** /
	/** 最大可用长度 */
	private int maxSize = 9999;
	/** 仓库可用大小 */
	private int size;
	/** 仓库物品存储空间，存储Prop对象，不对外开发 */
	private ArrayList space;
	/** 往外提供的仓库存储空间 */
	private ArrayList storage;


	// **********************private method******************************** /
	/** 本类使用设置仓库长度，判断了仓库最大长度 */
	private void setS (int size)
	{
		if (size > maxSize)
			this.size = maxSize;
		else
			this.size = size;
	}

	// **********************protected method******************************** /
	/** 初始化仓库 */
	public void init (int size, ArrayList list)
	{
		init (this.maxSize, size, list);
	}

	protected void init (int maxSize, int size, ArrayList list)
	{
		this.maxSize = maxSize;
		setS (size);
		if (list == null)
			space = new ArrayList ();
		else
			space = list;
		storage = ArrayList.ReadOnly (space);
	}
	
	// **********************public method******************************** /
	/** 获得指定索引的道具 */
	public StorageProp getPropByIndex (int index)
	{
		if (space.Count > index) {
			StorageProp sp = space [index] as StorageProp;
			sp.index = index;
			return sp;
		} else
			return null;
	}
	/** 获得指定索引的道具，返回道具数组，没有找到的补null */
	public StorageProp[] getPropByIndex (int[] indexs)
	{
		int l = indexs.Length;
		StorageProp[] sps = new StorageProp[l];
		for (int i=0; i<l; i++) {
			sps [i] = getPropByIndex (indexs [i]);
		}
		return sps;
	}
	/** 获得指定sid的道具 */
	public StorageProp getPropBySid (int sid)
	{
		for (int i=0,l=space.Count; i<l; i++) {
			StorageProp sp = space [i] as StorageProp;
			if (sp.sid == sid) {
				sp.index = i;
				return sp;
			}
		}
		return null;
	}
	/** 获得指定sid的道具，返回道具数组，没有找到的补null */
	public StorageProp[] getPropBySid (int[] sids)
	{
		int l = sids.Length;
		StorageProp[] sps = new StorageProp[l];
		for (int i=0; i<l; i++) {
			sps [i] = getPropBySid (sids [i]);
		}
		return sps;
	}
	/** 获得指定uid的道具 */
	public StorageProp getPropByUid (string uid)
	{
		for (int i=0,l=space.Count; i<l; i++) {
			StorageProp sp = space [i]  as StorageProp;
			if (sp.uid == uid) {
				sp.index = i;
				return sp;
			}
		}
		return null;
	}

	/** 获得指定uid的道具，返回道具数组，没有找到的补null */
	public StorageProp[] getPropByUid (string[] uids)
	{
		int l = uids.Length;
		StorageProp[] sps = new StorageProp[l];
		for (int i=0; i<l; i++) {
			sps [i] = getPropByUid (uids [i]);
		}
		return sps;
	}
	/** 获得仓库道具，只读 */
	public ArrayList getStorageProp ()
	{
		return storage;
	}
	/** 获得仓库可用大小 */
	public int getSize ()
	{
		return size;
	}
	/** 获得仓库剩余空间 */
	public int getFreeSize ()
	{
		return size - space.Count;
	}
	/** 检查仓库剩余空间是否足够，true足够 */
	public bool checkSize (int size)
	{
		return getFreeSize () >= size;
	}
	/** 检查能否添加一个道具，
	 * 不考虑添加相同uid的道具，
	 * 不考虑堆叠上限问题 
	 */
	public bool checkAddProp (StorageProp prop)
	{
		if (checkSize (1))
			return true;
		if (prop.isU)
			return false;
		for (int i=0,l=space.Count; i<l; i++) {
			StorageProp sp = space [i]  as StorageProp;
			if (sp.equal (prop))
				return true;
		}
		return false;
	}
	/** 添加道具 */
	public bool addProp (StorageProp prop)
	{
		if (!prop.isU) {
			for (int i=0,l=space.Count; i<l; i++) {
				StorageProp sp = space [i]  as StorageProp;
				if (sp.equal (prop)) {
					sp.addNum (prop.getNum ());
					return true;
				}
			}
		}
		if (checkSize (1)) {
			space.Add (prop);
			return true;
		} else {
			return false;
		}
	}
	/** 添加道具在最后的位置 */
	public bool addPropLast (StorageProp prop)
	{
		if (!checkSize (1))
			return false;
		space.Add (prop);
		return true;
	}
	/** 增加包裹可用大小 */
	public void addSize (int size)
	{
		setS (this.size + size);
	}
	/** 设置包裹可用大小 */
	public void setSize (int size)
	{
		setS (size);
	}
	/** 检查能否删除指定sid指定数量的道具 */
	public bool checkReducePropBySid (int sid, int num)
	{
		StorageProp sp = getPropBySid (sid);
		if (sp != null && sp.getNum () >= num) {
			return true;
		}
		return false;
	}
	/** 检查能否删除一组指定sid指定数量的道具，
	 * 相同道具必须先合并，sidAndNums={{sid,num},{sid,num},...} 
	 */
	public bool checkReducePropBySid (int[][] sidAndNums)
	{
		for (int i=0,l=sidAndNums.Length; i<l; i++) {
			if (!checkReducePropBySid (sidAndNums [i] [0], sidAndNums [i] [1]))
				return false;
		}
		return true;
	}
	/** 删除指定sid指定数量的道具 */
	public bool reducePropBySid (int sid, int num)
	{
		StorageProp sp = getPropBySid (sid);
		if (sp != null && sp.getNum () >= num) {
			sp.reduceNum (num);
			if (sp.getNum () < 1)
				space.Remove (sp);
			return true;
		}
		return false;
	}
	/** 删除一组指定sid指定数量的道具，
	 * 必须先检查，sidAndNums={{sid,num},{sid,num},...} 
	 */
	public void reducePropBySid (int[][] sidAndNums)
	{
		for (int i=0,l=sidAndNums.Length; i<l; i++) {
			reducePropBySid (sidAndNums [i] [0], sidAndNums [i] [1]);
		}
	}
	/** 检查能否删除指定uid的道具 */
	public bool checkReducePropByUid (string uid)
	{
		StorageProp sp = getPropByUid (uid);
		if (sp != null)
			return true;
		return false;
	}
	/** 检查能否删除一组指定uid的道具，相同道具必须先合并  */
	public bool checkReducePropByUid (string[] uids)
	{
		for (int i=0,l=uids.Length; i<l; i++) {
			if (!checkReducePropByUid (uids [i]))
				return false;
		}
		return true;
	}
	/** 删除指定uid的道具 */
	public bool reducePropByUid (string uid)
	{
		StorageProp sp = getPropByUid (uid);
		if (sp != null) {
			if (sp is Card) {
				Card card=sp as Card;
				card.putOffEquip ();
//				card.delStarSoulBoreByAll ();
			}
			space.RemoveAt (sp.index);
			return true;
		}
		return false;
	}
	/** 删除一组指定uid的道具，必须先检查 */
	public void reducePropByUid (string[] uids)
	{
		for (int i=0,l=uids.Length; i<l; i++) {
			reducePropByUid (uids [i]);
		}
	}
	/** 检查能否删除指定索引的道具 */
	public bool checkReducePropByIndex (int index, int num)
	{
		StorageProp sp = getPropByIndex (index);
		if (sp != null) {
			if (sp.isU)
				return true;
			else if (sp.getNum () >= num)
				return true;
		}
		return false;
	}
	/** 删除指定索引的道具 */
	public bool reducePropByIndex (int index, int num)
	{
		StorageProp sp = getPropByIndex (index);
		if (sp != null) {
			if (sp.isU) {
				space.RemoveAt (index);
				return true;
			} else if (sp.getNum () >= num) {
				sp.reduceNum (num);
				return true;
			}
		}
		return false;
	}
	/** 检查能否删除指定道具 */
	public bool checkReduceProp (StorageProp prop)
	{
		for (int i=0,l=space.Count; i<l; i++) {
			StorageProp sp = space [i]  as StorageProp;
			if (sp.equal (prop)) {
				if (sp.isU)
					return true;
				else if (sp.getNum () >= prop.getNum ())
					return true;
				else
					return false;
			}
		}
		return false;
	}
	/** 检查能否删除一组道具，相同道具必须先合并 */
	public bool checkReduceProp (StorageProp[] props)
	{
		for (int i=0,l=props.Length; i<l; i++) {
			if (!checkReduceProp (props [i]))
				return false;
		}
		return true;
	}
	/** 删除指定道具 */
	public bool reduceProp (StorageProp prop)
	{
		for (int i=0,l=space.Count; i<l; i++) {
			StorageProp sp = space [i]  as StorageProp;
			if (sp.equal (prop)) {
				if (sp.isU) {
					space.RemoveAt (i);
					return true;
				} else if (sp.getNum () >= prop.getNum ()) {
					sp.reduceNum (prop.getNum ());
					return true;
				} else {
					return false;
				}
			}
		}
		return false;
	}
	/** 删除一组道具，必须提前检查 */
	public void reduceProp (StorageProp[] props)
	{
		for (int i=0,l=props.Length; i<l; i++) {
			reduceProp (props [i]);
		}
	}
	/** 解析数据，子类必须重写 */
	public virtual void parse (ErlArray arr)
	{

	}

	//清理数据
	public void clearStorgae ()
	{
		if (space != null)
			space.Clear ();
	}

	public void clear ()
	{
		if (space != null)
			space.Clear ();
		this.size = 0;
		this.maxSize = 9999;
		space = null;
	}
}