using System;
using System.Collections;

/**
 * 筛选和排序条件管理器
 * @author 汤琦
 * */

public class SortConditionManagerment
{
	private Hashtable conditions = new Hashtable ();
	
	public SortConditionManagerment ()
	{  
		
	}
	
	public static SortConditionManagerment Instance {
		get{return SingleManager.Instance.getObj("SortConditionManagerment") as SortConditionManagerment;}
	}

	//初始化信息
	public SortCondition initDefaultSort (int key)
	{
		SortCondition sc = null;
		switch (key) {
		case SiftWindowType.SIFT_PROPSTORE_WINDOW:
			sc = defaultPropSort ();
			break;
		case SiftWindowType.SIFT_EQUIPSTORE_WINDOW:
			sc = defaultEquipStoreSort ();
			break;
        case SiftWindowType.SIFT_MAGICWEAPON_WINDOW:
            sc = defaultMagicStoreSort();
            break;  
		case SiftWindowType.SIFT_EQUIPCHOOSE_WINDOW:
			sc = defaultEquipChooseSort ();
			break;
		case SiftWindowType.SIFT_CARDSTORE_WINDOW:
			sc = defaultCardStoreSort ();
			break;
		case SiftWindowType.SIFT_CARDCHOOSE_WINDOW:
			sc = defaultCardStoreSort ();
			break;
		case SiftWindowType.SIFT_SHOP_WINDOW:
			sc = defaultShopSort ();
			break;
		case SiftWindowType.SIFT_STARSOULSTORE_WINDOW:
			sc = defaultStarSoulStoreSort ();
			break;
		default:
			sc = new SortCondition();
			break;
		}
		if (!isConditionsExist (key))
			conditions.Add (key, sc);
		else {
			conditions[key] = sc;
		}
		return sc;
	}
	
	public SortCondition getConditionsByKey (int key)
	{
		if (!isConditionsExist (key))
			return initDefaultSort(key);
		return conditions [key] as SortCondition;
	}
	
	private bool isConditionsExist (int key)
	{
		return conditions.Contains (key); 
	}
	
	//默认的道具排序
	private SortCondition defaultPropSort ()
	{
//		SortCondition sc = getConditionsByKey (SiftWindowType.SIFT_PROPSTORE_WINDOW);
		SortCondition sc = new SortCondition ();
		sc.siftConditionArr = new Condition[0];
		sc.sortCondition = new Condition (SortType.SORT);
		sc.sortCondition.conditions.Add (SortType.SORT_QUALITYDOWN);
		return sc;
	}
	//默认的装备store排序
	private SortCondition defaultEquipStoreSort ()
	{
//		SortCondition sc = getConditionsByKey (SiftWindowType.SIFT_EQUIPSTORE_WINDOW);
		SortCondition sc = new SortCondition ();
		sc.siftConditionArr = new Condition[0];
		sc.sortCondition = new Condition (SortType.SORT);
		sc.sortCondition.conditions.Add (SortType.SORT_QUALITYDOWN);
		return sc;
	}
    /// <summary>
    /// 默认的神器排序
    /// </summary>
    /// <returns></returns>
    private SortCondition defaultMagicStoreSort() {
        SortCondition sc = new SortCondition();
        sc.siftConditionArr = new Condition[0];
        sc.sortCondition = new Condition(SortType.SORT);
        sc.sortCondition.conditions.Add(SortType.SORT_QUALITYDOWN);
        return sc;
    }
	//默认的星魂store排序
	private SortCondition defaultStarSoulStoreSort ()
	{
		SortCondition sc = new SortCondition ();
		sc.siftConditionArr = new Condition[0];
		sc.sortCondition = new Condition (SortType.SORT);
		sc.sortCondition.conditions.Add (SortType.SORT_QUALITYDOWN);
		return sc;
	}
	//默认的装备选择排序
	private SortCondition defaultEquipChooseSort ()
	{
//		SortCondition sc = getConditionsByKey (SiftWindowType.SIFT_EQUIPCHOOSE_WINDOW);
		SortCondition sc = new SortCondition ();
		sc.siftConditionArr = new Condition[0];
		sc.sortCondition = new Condition (SortType.SORT);
		sc.sortCondition.conditions.Add (SortType.SORT_QUALITYDOWN);
		return sc;
	}
	//默认的cardStore排序
	private SortCondition defaultCardStoreSort ()
	{
//		SortCondition sc = getConditionsByKey (SiftWindowType.SIFT_CARDSTORE_WINDOW);
		SortCondition sc = new SortCondition ();
		sc.siftConditionArr = new Condition[0];
		sc.sortCondition = new Condition (SortType.SORT);
		sc.sortCondition.conditions.Add (SortType.SORT_QUALITYDOWN);
		return sc;
	}
	
	//默认的cardChoose排序
	private SortCondition defaultCardChooseSort ()
	{
//		SortCondition sc = getConditionsByKey (SiftWindowType.SIFT_CARDCHOOSE_WINDOW);
		SortCondition sc = new SortCondition ();
		sc.siftConditionArr = new Condition[0];
		sc.sortCondition = new Condition (SortType.SORT);
		sc.sortCondition.conditions.Add (SortType.SORT_LEVELDOWN);
		return sc;
	}

	//默认的shop排序
	private SortCondition defaultShopSort ()
	{
//		SortCondition sc = getConditionsByKey (SiftWindowType.SIFT_SHOP_WINDOW);
		SortCondition sc = new SortCondition ();
		sc.sortCondition = new Condition (SortType.SORT);
		sc.sortCondition.conditions.Add (SortType.SORT_ORDER);
		return sc;
	}
}

