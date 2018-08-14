using System;

/**
 * 筛选排序类型
 * */
public class SortType
{
	//筛选类型
	public const int JOB = 1, //职业
		QUALITY = 2, //品质 (卡片 装备 道具)
		EQUIP_SUIT = 3, //装备套装
		EQUIP_PART = 4, //装备部位
		PROP_TYPE = 5, //道具类型
		SORT = 6, //排序
		SID = 7, //模板编号
		CART_STATE = 8, //卡片状态
		EQUIP_STATE = 9, //装备状态
		EQUIP_JOB = 10, //装备职业 策划配置的 一件装备最适合的职业
		CARD_LEVEL_MAX = 11, // 卡片等级满级 1表示满级 0便是不是满级
		GOODS_TYPE = 12, //物品类型
		STARSOUL_TYPE=13; // 星魂类型
	
	
	//排序类型		
	public const int SORT_LEVELUP = 1, //等级升
		SORT_LEVELDOWN = 2, //等级降
		SORT_QUALITYUP = 3, //品质升
		SORT_QUALITYDOWN = 4, //品质降
		SORT_POWERUP = 5, //战力升
		SORT_POWERDOWN = 6, //战力降
		SORT_CONTRIBUTIONUP = 7, //贡献值升
		SORT_CONTRIBUTIONDOWN = 8, //贡献值降
		SORT_ORDER = 9,//配置顺序
        SORT_STRENG_LEVEUP=10,//强化等级升
        SORT_STENG_LEVEDOWN=11,//强化等级降
        SORT_PHASE_LEVEUP=12,//锻造等级升
        SPRT_PHASE_LEVEDOWN=13;//锻造等级降


	public const int SPLIT_FREE_STATE = 1, //自由状态
		SPLIT_EQUIP_NEW = 2, //新获得的装备
		SPLIT_EATEN = 3,
		SPLIT_USING_STATE = 4;//是否是祭品
}

