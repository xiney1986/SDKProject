using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

/**
 * 商店管理器
 * @author longlingquan
 * */
public class ShopManagerment
{
	//购买商品数量不受限制
	public const int GOODS_MAX = -1;
	private BanJiaShop banjiaShop;// 半价商店 供七日狂欢活动使用//
	private RMBShop rmbShop;//人民币商店
	private GuildShop guildShop;//公会商店
	private SuperDrawShop superDrawShop;//超级奖池商店
	private MeritShop meritShop; //功勋商店
	private GodsWarShop godsWarShop; //诸神战商店
    public BossAttackShop bossWarShop;//恶魔商店
	private LadderShop ladderShop;//爭霸商店
	private StarSoulDebrisShop starSoulDebrisShop; //星魂碎片商店
	private MysticalShop mysticalShop;//神秘商店
	private TeHuiShop tehuiShop;//特惠商店
    private NvshenShop nvshenShop;//女神商店
	private int showButtonFlag;//是否显示神秘商店更新特效
	private string showRushTime;
	private bool canupdate;
	public bool isOpenOneKey= false;
	public LastBattleShop lastBattleShop;// 末日决战商店//
	public StarShop starShop;// 星屑商店//
	public ShopManagerment ()
	{
	}
	
	public static ShopManagerment Instance {
		get{ return SingleManager.Instance.getObj ("ShopManagerment") as ShopManagerment;}
	}
	
	//初始化商店
	private void initShop (int shopType)
	{
		if (shopType == ShopType.RMB) {
			int[] ids = GoodsSampleManager.Instance.getAllShopGoods (shopType);//获取指定商店所有商品的id
			rmbShop = new RMBShop ();
			rmbShop.space = new ArrayList ();
			rmbShop.initAllGoods (ids);
		} else if (shopType == ShopType.GUILD) {
			int[] ids = GoodsSampleManager.Instance.getAllShopGoods (shopType);
			guildShop = new GuildShop ();
			guildShop.space = new ArrayList ();
			guildShop.initAllGoods (ids);
		}else if (shopType == ShopType.SUPERDRAW_SHOP) {
			int[] ids = GoodsSampleManager.Instance.getAllShopGoods (shopType);
			superDrawShop = new SuperDrawShop ();
			superDrawShop.space = new ArrayList ();
			superDrawShop.initAllGoods (ids);
		}else if (shopType == ShopType.GODSWAR_SHOP) {
			int[] ids = GoodsSampleManager.Instance.getAllShopGoods (shopType);
			godsWarShop = new GodsWarShop ();
			godsWarShop.space = new ArrayList ();
			godsWarShop.initAllGoods (ids);
        } else if (shopType == ShopType.HEROSYMBOL_SHOP) {
            int[] ids = GoodsSampleManager.Instance.getAllShopGoods(shopType);
            bossWarShop = new BossAttackShop();
            bossWarShop.space = new ArrayList();
            bossWarShop.initAllGoods(ids);
        } else if (shopType == ShopType.MERIT) {
			int[] ids = GoodsSampleManager.Instance.getAllShopGoods (shopType);
			meritShop = new MeritShop ();
			meritShop.space = new ArrayList ();
			meritShop.initAllGoods (ids);
		} else if (shopType == ShopType.STARSOUL_DEBRIS) {
			int[] ids = GoodsSampleManager.Instance.getAllShopGoods (shopType);
			starSoulDebrisShop = new StarSoulDebrisShop ();
			starSoulDebrisShop.space = new ArrayList ();
			starSoulDebrisShop.initAllGoods (ids);
		}else if(shopType==ShopType.MYSTICAL_SHOP){
			int[] ids = GoodsSampleManager.Instance.getAllShopGoods (shopType);
			mysticalShop = new MysticalShop ();
			mysticalShop.space = new ArrayList ();
			mysticalShop.initAllGoods (ids);
		} else if (shopType == ShopType.LADDER_HEGOMONEY) {

			int[] ids = GoodsSampleManager.Instance.getAllShopGoods (shopType);
			ladderShop = new LadderShop ();
			ladderShop.space = new ArrayList ();
			ladderShop.initAllGoods (ids);
		}else if(shopType==ShopType.TEHUI_SHOP){
			int[] ids = GoodsSampleManager.Instance.getAllShopGoods (shopType);
			tehuiShop = new TeHuiShop ();
			tehuiShop.space = new ArrayList ();
			tehuiShop.initAllGoods (ids);
        } else if (shopType == ShopType.NVSHEN_SHOP) {
            int[] ids = GoodsSampleManager.Instance.getAllShopGoods(shopType);
            nvshenShop = new NvshenShop();
            nvshenShop.space = new ArrayList();
            nvshenShop.initAllGoods(ids);
		}else if(shopType == ShopType.BANJIA_SHOP){
			int[] ids = GoodsSampleManager.Instance.getAllShopGoods(shopType);
			banjiaShop = new BanJiaShop();
			banjiaShop.space = new ArrayList();
			banjiaShop.initAllGoods(ids);
		}
		else if(shopType == ShopType.JUNGONG_SHOP){
			int[] ids = GoodsSampleManager.Instance.getAllShopGoods(shopType);
			lastBattleShop = new LastBattleShop();
			lastBattleShop.space = new ArrayList();
			lastBattleShop.initAllGoods(ids);
		}
		else if(shopType == ShopType.STAR_SHOP){
			int[] ids = GoodsSampleManager.Instance.getAllShopGoods(shopType);
			starShop = new StarShop();
			starShop.space = new ArrayList();
			starShop.initAllGoods(ids);
		}
	}
	
	//获得所有的RMB商品(已经剔除掉到期和达到购买上限的商品)
	public ArrayList getAllRmbGoods ()
	{
		return getRmbShop ().getShowList ();
	}
	//获得所有的神秘商品（剔除到期和达到购买上限的）
	public ArrayList getAllMysticalGoods()
	{
		return getMysticalShop().getShowList();
	}
	//获得所有的特惠商品（剔除到期和达到购买上限的）
	public ArrayList getAllTehuiGodds(){
		return getTeHuiShop().getShowList();;
	}

	//获得天梯争霸积分商品
	public ArrayList getAllLadderGoods()
	{
		 return getLadderShop ().getShowList ();
	}

	// 获得半价商店商品//
	public ArrayList getAllBanJiaGoods()
	{
		return getBanJiaShop().getShowList();
	}


	// 获取不同sid天梯争霸的商品列表
	public ArrayList getLadderSidGoods(int sid)
	{
		ArrayList sidGoods = new ArrayList ();
		foreach (Goods g in getLadderShop ().getShowList ())
		{
			if (g.getLadderGoodSid == sid)
			{
				sidGoods.Add(g);
			}
		}

		return sidGoods;
	}

	// 获取不同sid超级奖池的商品列表
	public ArrayList getSuperSidGoods(int sid)
	{
		ArrayList sidGoods = new ArrayList ();
		foreach (Goods g in getSuperDrawShop ().getShowList ())
		{
			if (g.getLadderGoodSid == sid)
			{
				sidGoods.Add(g);
			}
		}
		
		return sidGoods;
	}


	//获得所有的公会商店商品(已经剔除掉到期和达到购买上限的商品)
	public ArrayList getAllGuildGoods ()
	{
		return getGuildShop ().getShowList ();
	}

	//获得所有的超级奖池商店商品(已经剔除掉到期和达到购买上限的商品)
	public ArrayList getAllSuperDrawGoods ()
	{
		return getSuperDrawShop ().getShowList ();
	}

	public ArrayList getAllMeritGoods ()
	{
		return getMeritShop ().getShowList ();
	}

	//获得所有的星魂碎片商品(已经剔除掉到期和达到购买上限的商品)
	public ArrayList getAllStarSoulDebrisGoods ()
	{
		return getStarSoulDebrisShop ().getShowList ();
	}

    //获得女神商店的所有商品（已剔除购买达上限的商品）
    public ArrayList getAllNvshenGoods() 
    {
        return getNvshenShop().getShowList();
    }

	public ArrayList getAllGodsWarGoods() 
	{
		return getGodsWarShop().getShowList();
	}

    public ArrayList getAllBossAttackGoods() {
        return getBossAttackShop().getShowList();
    }

	public ArrayList getAllLastBattleGoods()
	{
		return getLastBattleShop().getShowList();
	}

	public ArrayList getAllStarGoods()
	{
		return getStarShop().getShowList();
	}

	//获得rmb商店
	private RMBShop getRmbShop ()
	{
		if (rmbShop == null)
			initShop (ShopType.RMB);

		return rmbShop;
	}
	//获得神秘商店
	private MysticalShop getMysticalShop ()
	{
		if(mysticalShop==null)initShop(ShopType.MYSTICAL_SHOP);
		return mysticalShop;
	}
	//获得诸神战商店
	private GodsWarShop getGodsWarShop ()
	{
		if(godsWarShop==null)initShop(ShopType.GODSWAR_SHOP);
		return godsWarShop;
	}
    //获得恶魔挑战商店
    private BossAttackShop getBossAttackShop() {
        if (bossWarShop == null) initShop(ShopType.HEROSYMBOL_SHOP);
        return bossWarShop;
    }
	// 获得末日决战商店//
	private LastBattleShop getLastBattleShop()
	{
		if (lastBattleShop == null) initShop(ShopType.JUNGONG_SHOP);
		return lastBattleShop;
	}
	// 获得星屑商店//
	private StarShop getStarShop()
	{
		if (starShop == null) initShop(ShopType.STAR_SHOP);
		return starShop;
	}
	//获得神秘商店
	private TeHuiShop getTeHuiShop ()
	{
		if(tehuiShop==null)initShop(ShopType.TEHUI_SHOP);
		return tehuiShop;
	}

	// 获得半价商店//
	private BanJiaShop getBanJiaShop()
	{
		if(banjiaShop == null)initShop(ShopType.BANJIA_SHOP);
		return banjiaShop;
	}

	//获得星魂碎片商店
	private StarSoulDebrisShop getStarSoulDebrisShop ()
	{
		if (starSoulDebrisShop == null)
			initShop (ShopType.STARSOUL_DEBRIS);
		return starSoulDebrisShop;
	}

	//获得公会商店
	private GuildShop getGuildShop ()
	{
		if (guildShop == null)
			initShop (ShopType.GUILD);
		
		return guildShop;
	}
	//获得超级奖池商店
	private SuperDrawShop getSuperDrawShop ()
	{
		if (superDrawShop == null)
			initShop (ShopType.SUPERDRAW_SHOP);
		
		return superDrawShop;
	}
    //获得女神商店
    public NvshenShop getNvshenShop() 
    {
        if (nvshenShop == null) {
            initShop(ShopType.NVSHEN_SHOP); 
        }
        return nvshenShop;
    }

	private LadderShop getLadderShop()
	{
		if (ladderShop == null)
			initShop (ShopType.LADDER_HEGOMONEY);
		return ladderShop;
	}

	private MeritShop getMeritShop ()
	{
		if (meritShop == null)
			initShop (ShopType.MERIT);
        
		return meritShop;
	}
	 
	//更新商品信息数据
	public void updateShop (ErlArray arr)
	{
		if (rmbShop == null)
			initShop (ShopType.RMB);
		if (guildShop == null)
			initShop (ShopType.GUILD);
		if (meritShop == null)
			initShop (ShopType.MERIT);
		if (ladderShop == null)
			initShop (ShopType.LADDER_HEGOMONEY);
		if(godsWarShop==null)
			initShop (ShopType.GODSWAR_SHOP);
        if (bossWarShop == null)
            initShop(ShopType.HEROSYMBOL_SHOP);
		if(starSoulDebrisShop==null)initShop(ShopType.STARSOUL_DEBRIS);
		if(tehuiShop==null)initShop(ShopType.TEHUI_SHOP);
        if (nvshenShop == null) initShop(ShopType.NVSHEN_SHOP);
		if(superDrawShop==null)initShop(ShopType.SUPERDRAW_SHOP);
		if(banjiaShop == null)initShop(ShopType.BANJIA_SHOP);
		if(lastBattleShop == null)initShop(ShopType.JUNGONG_SHOP);
		if(starShop == null)initShop(ShopType.STAR_SHOP);
		guildShop.updateGoods (arr);
		rmbShop.updateGoods (arr);
		meritShop.updateGoods (arr);
		ladderShop.updateGoods (arr);
		superDrawShop.updateGoods(arr);
		starSoulDebrisShop.updateGoods(arr);
		tehuiShop.updateGoods(arr);
        nvshenShop.updateGoods(arr);
		godsWarShop.updateGoods(arr);
		banjiaShop.updateGoods(arr);
        bossWarShop.updateGoods(arr);
		lastBattleShop.updateGoods(arr);
		starShop.updateGoods(arr);
	}
	//更新神秘商店数据
	public void updateMysticalShop(ErlList list)
	{
		if(mysticalShop==null)initShop(ShopType.MYSTICAL_SHOP);
		mysticalShop.updateGoods(list);
	}
	public void setMyRushCount(int num){
		mysticalShop.setConut(num);
	}
	public int getMyRushCount(){
		return  mysticalShop.getCount();
	}
	public void updateMysticalGood(int sid,int index){
		mysticalShop.updateGoodsBySid(sid,index);
	}
    public void updateTeHuiGood(int sid,int num)
    {
        tehuiShop.updateGoodsBySid(sid,num);
    }
	public void updateBanJiaGood(int sid,int num)
	{
		banjiaShop.updateGoodsBySid(sid,num);
	}
	//是否显示神秘商店特效
	public bool getMysticalEffect(){
		return showButtonFlag==1;
	}
	public void setMysticalEffect(int flag){
		showButtonFlag=flag;
	}
	public void setCanbeShow(bool bl){
		canupdate=bl;
	}
}

