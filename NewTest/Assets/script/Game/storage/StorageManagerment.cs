using System;
using System.Collections;
using System.Collections.Generic;

/**
 * 仓库管理器
 * @author longlingquan
 * */
public class StorageManagerment
{  
	/** 卡片仓库版本号 */
	public int RoleStorageVersion = 0;
	/** 装备仓库版本号 */
	public int EquipStorageVersion = 0;
	/** 道具仓库版本号 */
	public int PropStorageVersion = 0;
    /**秘宝仓库版本号 */
    public int magicWeaponVersion = 0;
	/** 临时仓库版本号 */
	public int tmpStorageVersion = 0;
	/** 女神仓库版本号 */
	public int beastStorageVersion = 0;
	/** 星魂仓库版本号 */
	public int starSoulStorageVersion = 0;
	/** 猎魂仓库版本号 */
	public int huntStarSoulStorageVersion = 0;
	/** 坐骑仓库版本号 */
	public int mountsStorageVersion = 0;
	private RoleStorage roleStorage;//角色仓库
	private BeastStorage beastStorage;//召唤兽仓库
	private PropStorage propStorage;//道具仓库
	private EquipStorage equipStorage;//装备仓库
	private TemporaryStorage tempStorage;//临时仓库
    private MagicWeaponStore magicWeaponStorage;//秘宝仓库
	/** 星魂仓库 */
	private StarSoulStorage starSoulStorage;
	/**  裂魂仓库 */
	private StarSoulStorage huntStarSoulStorage;
	/** 坐骑仓库 */
	private MountsStorage mountsStorage;

	public StorageManagerment ()
	{
		
	}

	public static StorageManagerment Instance {
		get{ return SingleManager.Instance.getObj ("StorageManagerment") as StorageManagerment;}
	}

	public bool isTempStorageFull (int num)
	{
		if (getAllTemp ().Count + num > getTempStorageMaxSpace ())
			return true;
		return false;
	}
	//验证角色仓库是否满
	public bool isRoleStorageFull (int num)
	{
		return !roleStorage.checkSize (num);
	}
	//验证召唤兽仓库是否满
	public bool isBeastStorageFull (int num)
	{
		return !beastStorage.checkSize (num);
	}
	//验证道具仓库是否满 需要传sid数组，true满
	public bool isPropStorageFull (int[] sids)
	{
		return !propStorage.isAddProp (sids);
	}
	//验证道具仓库是否满 需要传Prop数组
	public bool isPropStorageFull (Prop[] props)
	{
		return !propStorage.isAddProp (props);
	}

	//验证道具仓库是否满 需要传Prop
	public bool isPropStorageFull (Prop prop)
	{
		return !propStorage.isAddProp (prop);
	}
	//验证道具仓库是否满  
	public bool isPropStorageFull (int  sid)
	{
		return !propStorage.isAddProp (sid);
	}
	//验证装备仓库是否满
	public bool isEquipStorageFull (int num)
	{
		return !equipStorage.checkSize (num);
	}
    public bool isMagicWeaponStorageFull(int num) {
        return !magicWeaponStorage.checkSize(num);
    }
	//检测是否可接受奖励
	public	bool checkStoreFull (PrizeSample[]  prizes, out string str)
	{
		foreach (PrizeSample each in prizes) {
			
			switch (each.type) {
			case PrizeType.PRIZE_CARD:
				if (StorageManagerment.Instance.isRoleStorageFull (each.getPrizeNumByInt ()) == true) {
					str = LanguageConfigManager.Instance.getLanguage ("s0192", LanguageConfigManager.Instance.getLanguage ("s0193"));
					return  true;
				}

				break;
			case PrizeType.PRIZE_BEAST:
				if (StorageManagerment.Instance.isBeastStorageFull (each.getPrizeNumByInt ()) == true) {
					str = LanguageConfigManager.Instance.getLanguage ("s0192", LanguageConfigManager.Instance.getLanguage ("s0194"));
					return true;
				}
				break;
			case PrizeType.PRIZE_EQUIPMENT:
				if (StorageManagerment.Instance.isEquipStorageFull (each.getPrizeNumByInt ()) == true) {
					str = LanguageConfigManager.Instance.getLanguage ("s0192", LanguageConfigManager.Instance.getLanguage ("s0195"));
					return  true;
				}
				break;
			case PrizeType.PRIZE_PROP:
				if (StorageManagerment.Instance.isPropStorageFull (each.pSid) == true) {
					str = LanguageConfigManager.Instance.getLanguage ("s0192", LanguageConfigManager.Instance.getLanguage ("s0196"));
					return true;
				}
				break;
			case PrizeType.PRIZE_MONEY:
							
				if (each.getPrizeNumByLong () + UserManager.Instance.self.getMoney () > int.MaxValue) {
					str = LanguageConfigManager.Instance.getLanguage ("s0198");
					return true;
				}
				break;
			case PrizeType.PRIZE_RMB:
			
				if (each.getPrizeNumByLong () + UserManager.Instance.self.getRMB () > int.MaxValue) {
					str = LanguageConfigManager.Instance.getLanguage ("s0199");
					return true;
				}
				break;
			}
		}
		//false 仓库没满
		str = "";
		return false;
	}
	//检测是否可接受奖励
	public	bool checkStoreFull (PrizeSample prize, out string str)
	{
		switch (prize.type) {
		case PrizeType.PRIZE_CARD:
			if (StorageManagerment.Instance.isRoleStorageFull (prize.getPrizeNumByInt ()) == true) {
				str = LanguageConfigManager.Instance.getLanguage ("s0192", LanguageConfigManager.Instance.getLanguage ("s0193"));
				return  true;
			}

			break;
		case PrizeType.PRIZE_BEAST:
			if (StorageManagerment.Instance.isBeastStorageFull (prize.getPrizeNumByInt ()) == true) {
				str = LanguageConfigManager.Instance.getLanguage ("s0192", LanguageConfigManager.Instance.getLanguage ("s0194"));
				return true;
			}
			break;
		case PrizeType.PRIZE_EQUIPMENT:
			if (StorageManagerment.Instance.isEquipStorageFull (prize.getPrizeNumByInt ()) == true) {
				str = LanguageConfigManager.Instance.getLanguage ("s0192", LanguageConfigManager.Instance.getLanguage ("s0195"));
				return  true;
			}
			break;
		case PrizeType.PRIZE_PROP:
			if (StorageManagerment.Instance.isPropStorageFull (prize.pSid) == true) {
				str = LanguageConfigManager.Instance.getLanguage ("s0192", LanguageConfigManager.Instance.getLanguage ("s0196"));
				return true;
			}
			break;
		case PrizeType.PRIZE_MONEY:
							
			if (prize.getPrizeNumByLong () + UserManager.Instance.self.getMoney () > int.MaxValue) {
				str = LanguageConfigManager.Instance.getLanguage ("s0198");
				return true;
			}
			break;
		case PrizeType.PRIZE_RMB:
			
			if (prize.getPrizeNumByLong () + UserManager.Instance.self.getRMB () > int.MaxValue) {
				str = LanguageConfigManager.Instance.getLanguage ("s0199");
				return true;
			}
			break;
		}
		//false 仓库没满
		str = "";
		return false;
	}
	//仓库中的剩余卡片
	public ArrayList getNoUseRolesBySid (int sid)
	{
		ArrayList list = new ArrayList ();
		List<string> temList = ArmyManager.Instance.recalculateAllArmyIds ();


		foreach (Card each in roleStorage.getStorageProp()) {
			if (each.uid == UserManager.Instance.self.mainCardUid)
				continue;
			if (temList.Contains (each.uid))
				continue;
			
			if (each.sid == sid) {
				list.Add (each);
			}

		}
		return list;
	}
	/** 获得指定sid的临时道具 */
	public TempProp getPropInTempPropsBySid (int sid)
	{
		if (tempStorage == null)
			return null;
		return tempStorage.getPropBySid (sid) as TempProp;
	}
	
	public ArrayList getEquipsBySid (int sid)
	{
		ArrayList list = new ArrayList ();
		
		foreach (Equip each in  equipStorage.getStorageProp()) {
			if (each.getState () == EquipStateType.LOCKED || each.getState () == EquipStateType.OCCUPY)
				continue;
			
			if (each.sid == sid) {
				list.Add (each);
			}
		}
		return list;
	}

	public Equip getEquipTypeBySid (int sid)
	{
		Equip tmpEquip = null;
		foreach (Equip each in  equipStorage.getStorageProp()) {
			if (each.sid == sid) {
				tmpEquip = each as Equip;
				break;
			}
		}
		return tmpEquip;
	}
	
	public ArrayList getPropsBySid (int sid)
	{
		ArrayList list = new ArrayList ();
		
		foreach (Prop each in  propStorage.getStorageProp()) {
			if (each.sid == sid) {
				list.Add (each);
			}
		}
		return list;
	}

	public int getPropsCount (int type, int level)
	{
		int count = 0;
		PropSample sample;
		foreach (Prop each in  propStorage.getStorageProp()) {
			sample = PropSampleManager.Instance.getPropSampleBySid (each.sid);	
			if (sample.type == type && level >= each.getUseLv ()) {
				if (each.getNum () > 0) 
					count += each.getNum ();
				else
					count++;
			}
		}
		return count;
	}

	public int getPropsCountByType (int type)
	{
		int count = 0;
		PropSample sample;
		foreach (Prop each in  propStorage.getStorageProp()) {
			sample = PropSampleManager.Instance.getPropSampleBySid (each.sid);	
			if (sample.type == type) {
				if (each.getNum () > 0) 
					count += each.getNum ();
				else
					count++;
			}
		}
		return count;
	}

	//仓库可使用最大长度
	public int getPropStorageMaxSpace ()
	{
		return propStorage.getSize ();
	}
	//仓库可使用最大长度
	public int getEquipStorageMaxSpace ()
	{
		return equipStorage.getSize ();
	}
    //神器仓库使用的最大长度
    public int getMagicWeaponStorageMaxSpace() {
        return magicWeaponStorage.getSize();
    }
	//仓库可使用最大长度
	public int getRoleStorageMaxSpace ()
	{
		return roleStorage.getSize ();
	}
	//仓库可使用最大长度
	public int getBeastStorageMaxSpace ()
	{
		return beastStorage.getSize ();
	}
	//仓库可使用最大长度
	public int getTempStorageMaxSpace ()
	{
		return tempStorage.getSize ();
	}
	//设置仓库可使用最大长度
	public void setPropStorageMaxSpace (int num)
	{
		propStorage.setSize (num);
	}
	//设置仓库可使用最大长度
	public void setEquipStorageMaxSpace (int num)
	{
		equipStorage.setSize (num);
	}
    //设置秘宝仓库使用的最大长度
    public void setMagicWeaponStorageMaxSpace(int num) {
        magicWeaponStorage.setSize(num);
    }
	//设置仓库可使用最大长度
	public void setRoleStorageMaxSpace (int num)
	{
		roleStorage.setSize (num);
	}
	//设置仓库可使用最大长度
	public void setBeastStorageMaxSpace (int num)
	{
		beastStorage.setSize (num);
	}
	//设置仓库可使用最大长度
	public void setTempStorageMaxSpace (int num)
	{
		tempStorage.setSize (num);
	}
	//设置仓库可使用最大长度
	public void updateEquipStorageMaxSpace (int num)
	{
		equipStorage.addSize (num);
	}
	//设置仓库可使用最大长度
	public void updateRoleStorageMaxSpace (int num)
	{
		roleStorage.addSize (num);
	}

	public ArrayList getAllTemp ()
	{
		return tempStorage.getStorageProp ();
	}

	//获得时间有效的临时物品列表 新的列表
	public ArrayList getValidAllTemp (ArrayList tempList)
	{
		ArrayList displayList = new ArrayList ();
		for (int i = 0; i < tempList.Count; i++) {
			if ((tempList [i] as TempProp).time > ServerTimeKit.getSecondTime ())
				displayList.Add (tempList [i]);
		}
		//测试代码
		//object test = new object ();
		//displayList.Add(test);
		return  displayList;
	}
	 
	public ArrayList getAllRole ()
	{
		return roleStorage.getStorageProp (); 
	}

	public ArrayList getRoleBySid (int sid)
	{
		return roleStorage.getRoleBySid (sid); 
	}

	//获得非祭品卡片
	public ArrayList getAllRoleByNotToEat ()
	{
		SortCondition sc = new SortCondition ();
		sc.sortCondition = new Condition (SortType.SORT, new int[]{SortType.SORT_QUALITYDOWN});
		return SortManagerment.Instance.cardSort (roleStorage.getAllRoleByNotToEat (), sc, CardStateType.STATE_USING);
//		return roleStorage.getAllRoleByNotToEat(); 
	}

	public ArrayList getAllRoleByNotToEatAndLevel (int level)
	{ 
		ArrayList result = new ArrayList ();
		ArrayList ori = getAllRoleByNotToEat ();
		foreach (Card tmp in ori) {
			if (tmp.getLevel () >= level) {
				result.Add (tmp);
			}
		}
		return result;
    
	}
	/// <summary>
	/// 获得战斗力最低的卡
	/// </summary>
	public Card getLeastCombatCard ()
	{
		Card leastCombat = null;
		ArrayList list = getAllRole ();
		for (int i = 0; i < list.Count; i++) {
			Card card = list [i] as Card;
			if (ChooseTypeSampleManager.Instance.isToEat (card))
				continue;
			if (leastCombat == null || card.getCardCombat () < leastCombat.getCardCombat ()) {
				leastCombat = card;
			}
		}
		return leastCombat;
	}

	/// <summary>
	/// 获得所有满足卡牌训练规则的
	/// </summary>
	/// <returns></returns>
	public ArrayList getAllRoleByTraining ()
	{
		ArrayList temp = new ArrayList ();
		ArrayList list = getAllRole ();
		int mainCardLv = StorageManagerment.Instance.getRole (UserManager.Instance.self.mainCardUid).getLevel ();
		for (int i = 0; i < list.Count; i++) {
			Card card = list [i] as Card;
			//UnityEngine.Debug.Log(card.uid + "|" + card.getQualityId() + "|" + (!card.isMainCard()) + "|" + card.getLevel() + "|" + card.getMaxLevel());
			if (card.getQualityId () > 2 &&
				!card.isMainCard () &&
				card.getLevel () < mainCardLv &&
				card.getLevel () < card.getMaxLevel () &&
				!ChooseTypeSampleManager.Instance.isToEat (card)) {
				temp.Add (card);
			}
		}

		return temp;
	}

	public ArrayList getAllSacrificeRoleOfCommon ()
	{
		return roleStorage.getAllRoleToSacrifice (QualityType.COMMON);
	}

	public ArrayList getAllSacrificeRoleOfExcellent ()
	{
		return roleStorage.getAllRoleToSacrifice (QualityType.EXCELLENT);
	}

	public ArrayList getAllSacrificeRoleOfGood ()
	{
		return roleStorage.getAllRoleToSacrifice (QualityType.GOOD);
	}

	public ArrayList getAllRoleOfCommon ()
	{
		return roleStorage.getAllRoleOfQuality (QualityType.COMMON); 
	}

	public ArrayList getAllRoleOfExcellent ()
	{
		return roleStorage.getAllRoleOfQuality (QualityType.EXCELLENT); 
	}

	public ArrayList getAllRoleOfGood ()
	{
		return roleStorage.getAllRoleOfQuality (QualityType.GOOD); 
	}

	public ArrayList getAllRoleOfEpic ()
	{
		return roleStorage.getAllRoleOfQuality (QualityType.EPIC); 
	}

	public ArrayList getAllRoleOfLegend ()
	{
		return roleStorage.getAllRoleOfQuality (QualityType.LEGEND); 
	}
	//获得所有装备
	public ArrayList getAllEquip ()
	{
		return equipStorage .getStorageProp (); 
	}
    public ArrayList getallResolveMagicWeapon() {
        ArrayList temps = magicWeaponStorage.getStorageProp();
        if(temps!=null&&temps.Count>=1){
            ArrayList te = new ArrayList();
            for (int i = 0; i < temps.Count;i++ ) {
                MagicWeapon mw = temps[i] as MagicWeapon;
                if (mw.state == 0) te.Add(temps[i]);
            }
            if (te.Count >= 1) return te;
        }
        return new ArrayList() ;
    }
	/** 获得所有非祭品装备 */
	public ArrayList getAllEquipByNotToEat ()
	{
		SortCondition sc = new SortCondition ();
		sc.sortCondition = new Condition (SortType.SORT, new int[]{SortType.SORT_QUALITYDOWN});
		return SortManagerment.Instance.equipSort (equipStorage .getAllEquipByNotToEat (), sc);
//		return equipStorage .getAllEquipByNotToEat(); 
	}

    public bool isHaveJingLianEquip()
    {
        ArrayList eqlist = equipStorage.getStorageProp();
        if(eqlist==null||eqlist.Count<1)return false;
        for (int i=0;i<eqlist.Count;i++)
        {
            Equip eq = eqlist[i] as Equip;
            if(eq==null) continue;
            if(RefineSampleManager.Instance.getRefineSampleBySid(eq.sid)!=null)return true;
        }
        return false;
    }
	/** 获得所有祭品装备 */
	public ArrayList getAllEquipByEat ()
	{
		return equipStorage.getAllEquipByEat (); 
	}
	public ArrayList getAllEquipByEatByQuiltyID(int id)
	{
		return equipStorage.getAllEquipByEatByQualityId (id); 
	}
	public ArrayList getAllRoleByEatByQuiltyID(int id)
	{
		return roleStorage.getAllRoleToSacrifice (id); 
	}

	//获得所有普通品质装备
	public ArrayList getAllEquipByCommon ()
	{
		return equipStorage.getAllEquipByQuality (QualityType.COMMON); 
	}

	//获得所有优秀品质装备
	public ArrayList getAllEquipByExcellent ()
	{
		return equipStorage.getAllEquipByQuality (QualityType.EXCELLENT); 
	}

	//获得所有精良品质装备
	public ArrayList getAllEquipByGood ()
	{
		return equipStorage.getAllEquipByQuality (QualityType.GOOD); 
	}

	//获得所有史诗品质装备
	public ArrayList getAllEquipByEpic ()
	{
		return equipStorage.getAllEquipByQuality (QualityType.EPIC); 
	}

	//获得所有传说品质装备
	public ArrayList getAllEquipByLegend ()
	{
		return equipStorage.getAllEquipByQuality (QualityType.LEGEND); 
	}

	/** 获得所有道具 */
	public ArrayList getAllProp ()
	{
		return propStorage.getStorageProp ();
	}

	/** 获得所有非碎片道具 */
	public ArrayList getAllPropExcludeScrap ()
	{
		return propStorage.getAllPropExcludeScrap ();
	}
    /**获得所有的秘宝 */
    public ArrayList getAllMagicWeapon() {
        return magicWeaponStorage.getStorageProp();//这里是秘宝方法
    }
    public ArrayList getAllMagicWeaponByType(int type) {
        ArrayList magicList = magicWeaponStorage.getStorageProp();
        if (magicList == null || magicList.Count < 1) return null;
        ArrayList mwlist=null;
        for (int i = 0; i < magicList.Count;i++ ) {
            MagicWeapon mw = magicList[i] as MagicWeapon;
            if (mw.state != 1 && (mw.getMgType() == type||mw.getMgType()==0)) {
                if (mwlist == null) mwlist = new ArrayList();
                mwlist.Add(mw);
            }
        }
        return mwlist;
    }
    //拿到指定类型的秘宝 还需要排除掉需要替换的
    public ArrayList getAllMagicWeaponByType(int type, string uid) {
        ArrayList magicList = magicWeaponStorage.getStorageProp();
        if (magicList == null || magicList.Count < 1) return null;
        ArrayList mwlist = null;
        for (int i = 0; i < magicList.Count; i++) {
            MagicWeapon mw = magicList[i] as MagicWeapon;
            if (mw.uid!=uid&&mw.state != 1 && (mw.getMgType() == type || mw.getMgType() == 0)) {
                if (mwlist == null) mwlist = new ArrayList();
                mwlist.Add(mw);
            }
        }
        return mwlist;
    }

	/** 获得所有卡片碎片 */
	public ArrayList getAllPropByCardScrap ()
	{
		return propStorage.getAllPropByCardScrap (); 
	}

	/** 获得所有装备碎片 */
	public ArrayList getAllPropByEquipScrap ()
	{
		return propStorage.getAllPropByEquipScrap (); 
	}
    /// <summary>
    /// 获得所有的秘宝碎片
    /// </summary>
    /// <returns></returns>
    public ArrayList getAllPropByMagicScrap() {
        return propStorage.getAllPropByMagicScrap();
    }
	
	public ArrayList getAllBeast ()
	{
		return beastStorage.getStorageProp ();
	}
	 
	
	//获得角色卡片
	public Card getRole (string id)
	{
		if (roleStorage == null)
			return null;
		return roleStorage.getPropByUid (id) as Card;
	}
	public Card getCardBySid(int id)
	{
		if (roleStorage == null)
			return null;
		return roleStorage.getPropBySid (id) as Card;
	}
	
	//获得召唤兽卡片
	public Card getBeast (string id)
	{
		if (beastStorage == null)
			return null;
		return beastStorage.getPropByUid (id) as Card;
	}
	
	//获得召唤兽卡片
	public Card getBeastBySid (int sid)
	{
		if (beastStorage == null)
			return null;
		return beastStorage.getPropBySid (sid) as Card;
	} 
	
	//获得装备
	public Equip getEquip (string id)
	{
		if (equipStorage == null)
			return null;
		return equipStorage.getPropByUid (id) as Equip;
	}
	// 通过sid找装备//
	public Equip getEquipBySid (int id)
	{
		if (equipStorage == null)
			return null;
		return equipStorage.getPropBySid (id) as Equip;
	}
	
	//获得道具 道具没有uid只有sid
	public Prop getProp (int sid)
	{
		if (propStorage == null)
			return null;
		return propStorage.getPropBySid (sid) as Prop;
	}
	//获得临时道具，根据临时道具uid
	public TempProp getTempPropByUid (string uid)
	{
		if (tempStorage == null)
			return null;
		return tempStorage.getPropByTempUid (uid);
	}
	
	public TempProp getTempPropByIndex (int index)
	{
		if (tempStorage == null)
			return null;
		return tempStorage.getPropByIndex (index) as TempProp;
	}
	
	//检查道具数量
	public bool checkProp (int sid, int num)
	{
		return propStorage.checkReducePropBySid (sid, num); 
	}
	
	//获取道具每日最大使用次数限制
	public int getPropMaxUseCount (int sid)
	{
		Prop prop = getProp (sid);
		if (prop == null)
			return 0;
		return prop.getMaxUseCount ();
	}

	//使用道具
	public void useProp (int sid, int num)
	{
		propStorage.reducePropBySid (sid, num);
	}
	//添加普通道具
	public bool addGoodsProp (Prop prop)
	{
		PropStorageVersion += 1;
		return propStorage.addProp (prop);
	}
	//添加卡片
	public bool addCardProp (Card card)
	{
		RoleStorageVersion += 1;
		IncreaseManagerment.Instance.clearData (IncreaseManagerment.TYPE_CARD);
		return roleStorage.addProp (card);
	}
	//添加召唤兽
	public bool addBeastProp (Card card)
	{
		RoleStorageVersion += 1;
		IncreaseManagerment.Instance.clearData (IncreaseManagerment.TYPE_BEAST);
		return beastStorage.addProp (card);
	}
	//添加装备
	public bool addEquipProp (Equip equip)
	{
		EquipStorageVersion += 1;
		return equipStorage.addProp (equip);
	}
    public bool addMagicWeaponProp(MagicWeapon mw) {
        magicWeaponVersion += 1;
        return magicWeaponStorage.addProp(mw);
    }
	//删除普通道具
	public bool delGoodsProp (Prop prop)
	{
		PropStorageVersion += 1;
		return propStorage.addProp (prop);
	}
	//删除卡片
	public bool delCardProp (Card card)
	{
		RoleStorageVersion += 1;
		IncreaseManagerment.Instance.clearData (IncreaseManagerment.TYPE_CARD);
		return roleStorage.addProp (card);
	}
	//删除装备
	public bool delEquipProp (Equip equip)
	{
		EquipStorageVersion += 1;
		return equipStorage.addProp (equip);
	}
	//修改召唤兽
	public void updateEquip (string uid, ErlArray ea)
	{
		EquipStorageVersion ++;
		Equip equip = equipStorage.getPropByUid (uid) as Equip;
		if (equip != null) {
			equip.bytesRead (0, ea);
		}
	}
    public void updateMagicWeapon(string uid,ErlArray ea) {
        magicWeaponVersion++;
        MagicWeapon mw = magicWeaponStorage.getPropByUid(uid) as MagicWeapon;
        if(mw!=null){
            mw.bytesRead(0, ea);
        }
    }
	//修改召唤兽
	public void updateBeast (string uid, ErlArray ea)
	{
		beastStorageVersion ++;
		Card beast = beastStorage.getPropByUid (uid) as Card;
		if (beast != null) {
			beast.bytesRead (0, ea);
		}
		IncreaseManagerment.Instance.clearData (IncreaseManagerment.TYPE_BEAST);
	}
	//修改卡片
	public void updateCard (string uid, ErlArray ea)
	{
		RoleStorageVersion ++;
		Card card = roleStorage.getPropByUid (uid) as Card;
		if (card != null) {
			card.bytesRead (0, ea);
		}
		IncreaseManagerment.Instance.clearData (IncreaseManagerment.TYPE_CARD);
	}
	//修改一组卡片
	public void updateCard (ErlArray ea)
	{
		RoleStorageVersion ++;
		Card card;
		ErlArray temp;
		for (int i = 0; i < ea.Value.Length; i++) {
			temp = ea.Value [i] as ErlArray;
			card = roleStorage.getPropByUid ((temp.Value [0] as ErlType).getValueString ()) as Card;
			if (card != null) {
				card.bytesRead (0, ea);
			}
		}
		IncreaseManagerment.Instance.clearData (IncreaseManagerment.TYPE_CARD);
	}
	//修改卡片
	public void updateStarSoul (string uid, ErlArray ea)
	{
		starSoulStorageVersion ++;
		StarSoul starSoul = starSoulStorage.getPropByUid (uid) as StarSoul;
		if (starSoul != null) {
			starSoul.bytesRead (0, ea);
		}
	}
	/// <summary>
	/// 验证星魂仓库是否满
	/// </summary>
	/// <param name="num">附加数量</param>
	public bool isStarSoulStorageFull (int num)
	{
		if (starSoulStorage == null)
			return true;
		return !starSoulStorage.checkSize (num);
	}
	/// <summary>
	/// 获得星魂仓库中指定uid的星魂
	/// </summary>
	/// <param name="uid">uid</param>
	public StarSoul getStarSoul (string uid)
	{
		if (starSoulStorage == null)
			return null;
		return starSoulStorage.getPropByUid (uid) as StarSoul;
	}
	/// <summary>
	/// 添加星魂到星魂仓库
	/// </summary>
	/// <param name="data">数据</param>
	public void addStarSoulStorage (ErlArray data)
	{
		StarSoul starSoul = StarSoulManager.Instance.createStarSoul ();
		starSoul.bytesRead (0, data);
		addStarSoulStorage (starSoul, true);
	}
	/// <summary>
	/// 添加星魂到星魂仓库
	/// </summary>
	/// <param name="starSoul">星魂</param>
	/// <param name="isNew">是否新的</param>
	public bool addStarSoulStorage (StarSoul starSoul, bool isNew)
	{
		starSoul.isNew = isNew;
		return starSoulStorage.addProp (starSoul);
	}
	/// <summary>
	/// 扣除星魂仓库中的星魂
	/// </summary>
	/// <param name="data">Data.</param>
	public void delStarSoulStorage (ErlType data)
	{
		if ((data as ErlArray) == null)
			starSoulStorage.reducePropByUid (data.getValueString ());
		else
			starSoulStorage.reducePropByUid (ErlKit.ErlArray2String (data as ErlArray));
	}
	/// <summary>
	/// 扣除星魂仓库中的指定uid星魂
	/// </summary>
	/// <param name="uid">uid</param>
	public void delStarSoulStorage (string uid)
	{
		starSoulStorage.reducePropByUid (uid);
	}
	/// <summary>
	/// 更新星魂仓库中的星魂数据
	/// </summary>
	/// <param name="data">数据</param>
	public void updateStarSoulStorage (ErlType data)
	{
		updateStarSoulStorage (((data as ErlArray).Value [0] as ErlType).getValueString (), data as ErlArray);
	}
	/// <summary>
	/// 更新星魂仓库中的星魂数据
	/// </summary>
	/// <param name="uid">uid</param>
	/// <param name="ea">数据</param>
	public void updateStarSoulStorage (string uid, ErlArray ea)
	{
		StarSoul starSoul = starSoulStorage.getPropByUid (uid) as StarSoul;
		if (starSoul != null) {
			starSoul.bytesRead (0, ea);
		}
	}
	/// <summary>
	/// 星魂仓库可使用最大长度
	/// </summary>
	public int getStarSoulStorageMaxSpace ()
	{
		if (starSoulStorage == null)
			return 0;
		return starSoulStorage.getSize ();
	}
	/// <summary>
	/// 将仓库中所有星魂设置为old装备
	/// </summary>
	public void clearAllStarSoulNew ()
	{
		ArrayList starSouls = getAllStarSoul ();
		StarSoul starSoul;
		for (int i=0,len=starSouls.Count; i<len; i++) {
			starSoul = (StarSoul)starSouls [i];
			starSoul.isNew = false;
		}
	}
	/// </summary>
	/// 获得星魂仓库所有星魂
	/// </summary>
	public ArrayList getAllStarSoul ()
	{
		return starSoulStorage.getStorageProp (); 
	}
	/** 获得星魂仓库 */
	public StarSoulStorage getStarSoulStorage ()
	{
		return starSoulStorage;
	}
	/// <summary>
	/// 验证猎魂仓库是否满
	/// </summary>
	/// <param name="num">附加数量</param>
	public bool isHuntSoulStorageFull (int num)
	{
		if (huntStarSoulStorage == null)
			return true;
		return !huntStarSoulStorage.checkSize (num);
	}
	/// <summary>
	/// 校验猎魂仓库中是否存在>=指定品质的星魂
	/// </summary>
	/// <param name="qualityId">星魂品质</param>
	public bool isStarSoulQualityByHuntStore (int qualityId)
	{
		StarSoul starSoul;
		ArrayList huntStarSouls = getAllHuntStarSoul ();
		for (int i=0,len=huntStarSouls.Count; i<len; i++) {
			starSoul = (StarSoul)huntStarSouls [i];
			if (starSoul == null)
				continue;
			if (starSoul.getQualityId () >= qualityId)
				return true;
		}
		return false;
	}
	/// <summary>
	/// 获得猎魂仓库中指定uid的星魂
	/// </summary>
	/// <param name="uid">uid</param>
	public StarSoul getHuntStarSoul (string uid)
	{
		if (huntStarSoulStorage == null)
			return null;
		return huntStarSoulStorage.getPropByUid (uid) as StarSoul;
	}
	/// <summary>
	/// 添加星魂到猎魂仓库
	/// </summary>
	/// <param name="data">数据</param>
	public void addHuntStarSoulStorage (ErlArray data)
	{
		StarSoul starSoul = StarSoulManager.Instance.createStarSoul ();
		starSoul.bytesRead (0, data);
		addHuntStarSoulStorage (starSoul, true);
	}
	/// <summary>
	///添加星魂到猎魂仓库
	/// </summary>
	/// <param name="starSoul">星魂</param>
	/// <param name="isNew">是否新的</param>
	public bool addHuntStarSoulStorage (StarSoul starSoul, bool isNew)
	{
		starSoul.isNew = isNew;
		return huntStarSoulStorage.addProp (starSoul);
	}
	/// <summary>
	/// 扣除猎魂仓库中的星魂
	/// </summary>
	/// <param name="data">Data.</param>
	public void delHuntStarSoulStorage (ErlType data)
	{
		if ((data as ErlArray) == null)
			huntStarSoulStorage.reducePropByUid (data.getValueString ());
		else
			huntStarSoulStorage.reducePropByUid (ErlKit.ErlArray2String (data as ErlArray));
	}
	/// <summary>
	/// 扣除猎魂仓库中的指定uid星魂
	/// </summary>
	/// <param name="uid">uid</param>
	public void delHuntStarSoulStorage (string uid)
	{
		huntStarSoulStorage.reducePropByUid (uid);
	}
	/// <summary>
	/// 猎魂仓库可使用最大长度
	/// </summary>
	public int getHuntStarSoulStorageMaxSpace ()
	{
		if (huntStarSoulStorage == null)
			return 0;
		return huntStarSoulStorage.getSize ();
	}
	/** 获得仓库剩余空间 */
	public int getFreeSize ()
	{
		if (huntStarSoulStorage == null)
			return 0;
		return huntStarSoulStorage.getFreeSize ();
	}
	/// <summary>
	/// 获得裂魂仓库所有星魂
	/// </summary>
	public ArrayList getAllHuntStarSoul ()
	{
		return huntStarSoulStorage.getStorageProp (); 
	}
	/** 获得猎魂仓库中非经验星魂数量 */
	public int getHuntStarSoulNumByType ()
	{
		int count = 0;
		ArrayList arrayList = huntStarSoulStorage.getStorageProp ();
		StarSoul starSoul;
		for (int i=0,len=arrayList.Count; i<len; i++) {
			starSoul = arrayList [i] as StarSoul;
			if (starSoul.getStarSoulType () != 0) {
				count++;
			}
		}
		return count;
	}
	/** 获得猎魂仓库的星魂数量 */
	public int getHuntStarSoulNum ()
	{
		return huntStarSoulStorage.getStorageProp ().Count;
	}
	/** 获得猎魂仓库 */
	public StarSoulStorage getHuntStarSoulStorage ()
	{
		return huntStarSoulStorage;
	}
	/// <summary>
	/// 获得仓库中指定uid的坐骑
	/// </summary>
	/// <param name="uid">uid</param>
	public Mounts getMounts (string uid)
	{
		if (huntStarSoulStorage == null)
			return null;
		return mountsStorage.getPropByUid (uid) as Mounts;
	}
    public MagicWeapon getMagicWeapon(string uid) {
        if (magicWeaponStorage == null) return null;
        return magicWeaponStorage.getPropByUid(uid) as MagicWeapon;
    }
	/// <summary>
	/// 获得所有坐骑
	/// </summary>
	public ArrayList getAllMounts ()
	{
		return mountsStorage.getStorageProp ();
	}
	/// <summary>
	/// 添加坐骑
	/// </summary>
	public void addMounts (Mounts mounts)
	{
		mountsStorage.addProp (mounts);
	}
	/// <summary>
	/// 修改坐骑
	/// </summary>
	public void updateMounts (string uid, ErlArray ea)
	{
		mountsStorageVersion ++;
		Mounts mounts = mountsStorage.getPropByUid (uid) as Mounts;
		if (mounts != null) {
			mounts.bytesRead (0, ea);
		}
	}
	/// <summary>
	/// 获得所有坐骑数量
	/// </summary>
	/// <returns>The mounts count.</returns>
	public int getMountsCount ()
	{
		return mountsStorage.getStorageProp ().Count;
	}
	/// <summary>
	/// 坐骑仓库可使用最大长度
	/// </summary>
	public int getMountsStorageMaxSpace ()
	{
		if (mountsStorage == null)
			return 0;
		return mountsStorage.getSize ();
	}
	/// <summary>
	/// 验证坐骑仓库是否满
	/// </summary>
	/// <param name="num">附加数量</param>
	public bool isMountsStorageFull (int num)
	{
		if (mountsStorage == null)
			return true;
		return !mountsStorage.checkSize (num);
	}
	//初始化仓库数据
	public void updateStorageInfo (string type, ErlArray arr)
	{ 
		if (type == StorageFPort.GOODS) {
			PropStorageVersion += 1;
			propStorage = new PropStorage ();
			propStorage.parse (arr);
		} else if (type == StorageFPort.CARD) {
			RoleStorageVersion += 1;
			roleStorage = new RoleStorage ();
			roleStorage.parse (arr);
			IncreaseManagerment.Instance.clearData (IncreaseManagerment.TYPE_CARD);
		} else if (type == StorageFPort.BEAST) {
			beastStorageVersion += 1;
			beastStorage = new BeastStorage ();
			beastStorage.parse (arr);
			IncreaseManagerment.Instance.clearData (IncreaseManagerment.TYPE_BEAST);
		} else if (type == StorageFPort.EQUIPMENT) {
			EquipStorageVersion += 1;
			equipStorage = new EquipStorage ();
			equipStorage.parse (arr);
		} else if (type == StorageFPort.TEMP) {
			tmpStorageVersion += 1;
			tempStorage = new TemporaryStorage ();
			tempStorage.parse (arr);
			IncreaseManagerment.Instance.clearData (IncreaseManagerment.TYPE_CARD);
			IncreaseManagerment.Instance.clearData (IncreaseManagerment.TYPE_BEAST);
		} else if (type == StorageFPort.STAR_SOUL_STORAGE) {
			starSoulStorageVersion += 1;
			starSoulStorage = new StarSoulStorage ();
			starSoulStorage.parse (arr);
		} else if (type == StorageFPort.STAR_SOUL_DRAW_STORAGE) {
			huntStarSoulStorageVersion += 1;
			huntStarSoulStorage = new StarSoulStorage ();
			huntStarSoulStorage.parse (arr);
		} else if (type == StorageFPort.MOUNTS) {
			mountsStorageVersion += 1;
			mountsStorage = new MountsStorage ();
			mountsStorage.parse (arr);
		} else if(type==StorageFPort.MAGIC_WEAPON){
            magicWeaponVersion += 1;
            magicWeaponStorage = new MagicWeaponStore();
            magicWeaponStorage.parse(arr);

        }
	}

	// [[goods,[71002,2]]]
	public void parseAddStorageProps (ErlArray array)
	{
		ErlArray temp;
		for (int i = 0; i < array.Value.Length; i++) {
			temp = array.Value [i] as ErlArray;
			addStorageProp ((temp.Value [0] as ErlType).getValueString (), temp.Value [1] as ErlArray);
		}
	}

	private void addStorageProp (string type, ErlArray data)
	{
		if (type == StorageFPort.GOODS) {
			PropStorageVersion++;
			Prop prop = PropManagerment.Instance.createProp ();
			prop.bytesRead (0, data);
			addGoodsProp (prop);
		} else if (type == StorageFPort.CARD) {
			RoleStorageVersion++;
			Card card = CardManagerment.Instance.createCard ();
			card.bytesRead (0, data);
			addCardProp (card);
			IncreaseManagerment.Instance.clearData (IncreaseManagerment.TYPE_CARD);
		} else if (type == StorageFPort.BEAST) {
			beastStorageVersion++;
			Card card = CardManagerment.Instance.createCard ();
			card.bytesRead (0, data);
			addBeastProp (card);
			IncreaseManagerment.Instance.clearData (IncreaseManagerment.TYPE_BEAST);
		} else if (type == StorageFPort.EQUIPMENT || type == StorageFPort.EQUIPMENT1) {
			EquipStorageVersion++;
			Equip equip = EquipManagerment.Instance.createEquip ();
			equip.bytesRead (0, data);
			equip.isNew = true;
			addEquipProp (equip);
		} else if (type == StorageFPort.STAR_SOUL_STORAGE) {
			starSoulStorageVersion++;
			StarSoul starSoul = StarSoulManager.Instance.createStarSoul (data);
			addStarSoulStorage (starSoul, true);
		} else if (type == StorageFPort.MOUNTS) {
			mountsStorageVersion++;
			Mounts mounts = MountsManagerment.Instance.createMounts (data);
			addMounts (mounts);
		} else if(type==StorageFPort.MAGIC_WEAPON){
            magicWeaponVersion++;
            MagicWeapon magic = new MagicWeapon();
            magic.bytesRead(0,data);
            addMagicWeaponProp(magic);
            
        }
	}

	public void parseDelStorageProps (ErlArray array)
	{
		ErlArray temp;
		for (int i = 0; i < array.Value.Length; i++) {
			temp = array.Value [i] as ErlArray;
			delStorageProp ((temp.Value [0] as ErlType).getValueString (), temp.Value [1] as ErlType);
		}
	}

    public bool checkHaveCommendMagicWeapon()
    {
        ArrayList mwList = getallResolveMagicWeapon();
        if (mwList == null || mwList.Count <= 0) return false;
        for (int i=0;i<mwList.Count;i++)
        {
            MagicWeapon mw = mwList[i] as MagicWeapon;
            if (mw.getMgType() == 0) return true;
        }
        return false;
    }

	private void delStorageProp (string type, ErlType data)
	{
		if (type == StorageFPort.GOODS) {
			PropStorageVersion++;
			int[] sidNum = ErlKit.ErlArray2Int (data as ErlArray);
			propStorage.reducePropBySid (sidNum [0], sidNum [1]);
		} else if (type == StorageFPort.CARD) {
			RoleStorageVersion++;
			if ((data as ErlArray) == null)
				roleStorage.reducePropByUid (data.getValueString ());
			else
				roleStorage.reducePropByUid (ErlKit.ErlArray2String (data as ErlArray));
			IncreaseManagerment.Instance.clearData (IncreaseManagerment.TYPE_CARD);
		} else if (type == StorageFPort.EQUIPMENT || type == StorageFPort.EQUIPMENT1) {
			EquipStorageVersion++;
			if ((data as ErlArray) == null)
				equipStorage.reducePropByUid (data.getValueString ());
			else
				equipStorage.reducePropByUid (ErlKit.ErlArray2String (data as ErlArray));
		} else if (type == StorageFPort.MOUNTS) {
			mountsStorageVersion++;
			if ((data as ErlArray) == null)
				mountsStorage.reducePropByUid (data.getValueString ());
			else
				mountsStorage.reducePropByUid (ErlKit.ErlArray2String (data as ErlArray));
		}else if(type==StorageFPort.MAGIC_WEAPON){
            magicWeaponVersion++;
            if ((data as ErlArray) == null) magicWeaponStorage.reducePropByUid(data.getValueString());
            else magicWeaponStorage.reducePropByUid(ErlKit.ErlArray2String(data as ErlArray));
        }
	}

	public void parseUpdateStorageProps (ErlArray array)
	{
		ErlArray temp;
		for (int i = 0; i < array.Value.Length; i++) {
			temp = array.Value [i] as ErlArray;
			updateStorageProp ((temp.Value [0] as ErlType).getValueString (), temp.Value [1] as ErlType);
		}
	}
	
	private void updateStorageProp (string type, ErlType data)
	{
		if (type == StorageFPort.CARD) {
			RoleStorageVersion++;
			updateCard (((data as ErlArray).Value [0] as ErlType).getValueString (), data as ErlArray);
			IncreaseManagerment.Instance.clearData (IncreaseManagerment.TYPE_CARD);
		} else if (type == StorageFPort.EQUIPMENT || type == StorageFPort.EQUIPMENT1) {
			EquipStorageVersion++;
			updateEquip (((data as ErlArray).Value [0] as ErlType).getValueString (), data as ErlArray);
		} else if (type == StorageFPort.BEAST) {
			beastStorageVersion ++;
			updateBeast (((data as ErlArray).Value [0] as ErlType).getValueString (), data as ErlArray);
		} else if (type == StorageFPort.MOUNTS) {
			mountsStorageVersion ++;
			updateMounts (((data as ErlArray).Value [0] as ErlType).getValueString (), data as ErlArray);
		} else if (type == StorageFPort.STAR_SOUL_STORAGE) {
			starSoulStorageVersion ++;
			updateStarSoul (((data as ErlArray).Value [0] as ErlType).getValueString (), data as ErlArray);
		}else if(type==StorageFPort.MAGIC_WEAPON){
            magicWeaponVersion++;
            updateMagicWeapon(((data as ErlArray).Value[0] as ErlType).getValueString(), data as ErlArray);

        }
	}

	public TemporaryStorage getTempStorage ()
	{
		return tempStorage;
	} 
	
	//清除仓库数据
	public void clear ()
	{
		RoleStorageVersion = 0;
		EquipStorageVersion = 0;
		PropStorageVersion = 0;
		tmpStorageVersion = 0;
		beastStorageVersion = 0;
		mountsStorageVersion = 0;
		if (roleStorage != null)
			roleStorage.clear ();
		if (propStorage != null)
			propStorage.clear ();
		if (beastStorage != null)
			beastStorage.clear ();
		if (equipStorage != null)
			equipStorage.clear ();
		if (tempStorage != null)
			tempStorage.clear ();
		if (starSoulStorage != null)
			starSoulStorage.clear ();
		if (huntStarSoulStorage != null)
			huntStarSoulStorage.clear ();
		if (mountsStorage != null)
			mountsStorage.clear ();
	}
}