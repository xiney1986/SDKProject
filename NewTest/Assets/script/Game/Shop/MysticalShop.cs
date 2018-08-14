using System;
using System.Collections;
/**
 * 神秘商店
 * @author longlingquan MysticalShop
 * */
public class MysticalShop:MemorySpace
{
	ArrayList showList ;
	int rushNum=0;//神秘商店刷新次数
	
	public MysticalShop ()
	{
	}
	
	public ArrayList getShowList ()
	{
		siftGoods ();
		return showList;
	}
	//初始化所有
	public void initAllGoods (int[] ids)
	{
		if (ids == null || ids.Length < 1)
			return;
		for (int i = 0; i < ids.Length; i++) {
			Goods goods = new Goods (ids [i]); 
			
			if (goods.isShow ()) {
				space.Add (goods);
			}
		}
	}
	//过滤掉过期的和达到购买上限的商品
	void  siftGoods ()
	{
		showList.Sort (new MysticalComp ());
		//showList = SortManagerment.Instance.shopSort (showList, SortConditionManagerment.Instance.getConditionsByKey (SiftWindowType.SIFT_SHOP_WINDOW));
	}
	//移除商品
	public void removeGoods (Goods goods)
	{
		space.Remove (goods);
	}
	public void  setConut(int num){
		rushNum=num;
	}
	public int getCount(){
		return rushNum;
	}
	/// <summary>
	/// 更新单个是否购买信息
	/// </summary>
	/// <param name="sid">Sid.</param>
	public void updateGoodsBySid(int sid,int index){
		Goods good;
		for (int j=0; j<showList.Count; j++) {
			good=showList[j] as Goods;
			if(sid==good.sid&&good.showIndex==index){
				good.isBuy=1;
			}
		}
	}
	public void updateGoods (ErlList list)
	{
		ErlArray arr1;
		Goods good;
		if(showList==null)showList = new ArrayList ();
		if(showList.Count>0)showList.Clear();
		for (int j=0; j<space.Count; j++) {
			good = space [j] as Goods;
			good.isShowInMytical=false;
		}
		for (int i=0; i<list.Value.Length; i++) {
			arr1 = list.Value [i] as ErlArray;
			Goods goods;
			for (int j=0; j<space.Count; j++) {
				goods = space [j] as Goods;
				if (StringKit.toInt ((arr1.Value [0]).getValueString ()) == goods.sid) {
					goods.showIndex = i;
					goods.isShowInMytical=true;
					goods.isBuy = StringKit.toInt ((arr1.Value [1] as ErlType).getValueString ());
					showList.Add(goods.Clone());
					break;
				}
			}
		}
	}

	//装备根据品质排序
	public class MysticalComp : IComparer
	{
		public int Compare(object o1,object o2)
		{
			if(o1==null) return 1;
			if(o2==null) return -1;
			Goods goods1 = o1 as Goods;
			Goods goods2 = o2 as Goods;
			if(goods1==null || goods2==null) return 0;
			if(goods1.showIndex > goods2.showIndex)
				return 1;
			if(goods1.showIndex < goods2.showIndex)
				return -1;
			return 0;
		}
	}
}


