using System;
 
/**
 * 用户数据更新服务
 * @author longlingquan
 * */
public class UserUpdateService:BaseFPort
{
	
	private const string KEY_PVE = "pve";//当前行动力
	private const string KEY_PVE_T = "pve_t";//当前行动力恢复时间
	private const string KEY_PVP = "pvp";//当前体力
	private const string KEY_PVP_T = "pvp_t";//当前体力恢复时间
	private const string KEY_PVE_M = "pve_m";//行动力最大值
	private const string KEY_PVP_M = "pvp_m";//体力最大值
	private const string KEY_CHV = "chv";//英雄之章挑战次数
	private const string KEY_CHV_T = "chv_t";//当前英雄之章挑战次数恢复时
	private const string KEY_CHV_M = "chv_m";//英雄之章挑战次数最大值
	private const string KEY_FRIEND_SIZE = "friend_size";//好友数量
	private const string KEY_CARD_STORAGE = "card_storage";//卡片包裹
	private const string KEY_EQUIP_STORAGE = "equip_storage";//装备包裹
	private const string KEY_GOODS_STORAGE = "goods_storage";//道具包裹
	private const string KEY_BEAST_STORAGE = "beast_storage";//召唤兽包裹
	private const string KEY_TEMP_STORAGE = "temp_storage";//临时包裹
	private const string KEY_LADDER_RANK = "ladder_rank";//玩家天梯排名
	private const string KEY_STORE = "pve_c";//当前存储行动力
	private const string KEY_STORE_T = "pve_c_t";//当前存储行动力恢复时间
	private const string KEY_STORE_M = "pve_c_m";//存储行动力最大值
    private const string KEY_STORE_MAGIC = "magicWeapon_storage";//秘宝包裹

	public UserUpdateService ()
	{
		
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlType pve = message.getValue (KEY_PVE) as ErlType;
		ErlType pvp = message.getValue (KEY_PVP) as ErlType;
		ErlType pve_t = message.getValue (KEY_PVE_T) as ErlType;
		ErlType pvp_t = message.getValue (KEY_PVP_T) as ErlType; 
		ErlType pve_m = message.getValue (KEY_PVE_M) as ErlType;
		ErlType pvp_m = message.getValue (KEY_PVP_M) as ErlType;
		ErlType chv = message.getValue (KEY_CHV) as ErlType;
		ErlType chv_t = message.getValue (KEY_CHV_T) as ErlType; 
		ErlType chv_m = message.getValue (KEY_CHV_M) as ErlType;
		ErlType friend_size = message.getValue (KEY_FRIEND_SIZE) as ErlType;
		ErlType card_size = message.getValue (KEY_CARD_STORAGE) as ErlType;
		ErlType equip_size = message.getValue (KEY_EQUIP_STORAGE) as ErlType;
		ErlType goods_size = message.getValue (KEY_GOODS_STORAGE) as ErlType;
		ErlType beast_size = message.getValue (KEY_BEAST_STORAGE) as ErlType;
		ErlType temp_size = message.getValue (KEY_TEMP_STORAGE) as ErlType;
		ErlType ladder_rank = message.getValue (KEY_LADDER_RANK) as ErlType;
		ErlType storePve = message.getValue (KEY_STORE) as ErlType;
		ErlType storePveT = message.getValue (KEY_STORE_T) as ErlType; 
		ErlType storePveM = message.getValue (KEY_STORE_M) as ErlType;
        ErlType magicWeapon_size = message.getValue(KEY_STORE_MAGIC) as ErlType;

		//最先设置行动力 体力上限值改变
		if (pve_m != null) {
			UserManager.Instance.updatePvEMax (StringKit.toInt (pve_m.getValueString ()));
		}
		
		if (pvp_m != null) {
			UserManager.Instance.updatePvPMax (StringKit.toInt (pvp_m.getValueString ()));
		}

		if (chv_m != null) {
			UserManager.Instance.updateChvMax (StringKit.toInt (chv_m.getValueString ()));
		}

		if (storePveM != null) {
			UserManager.Instance.updateMountsPvEMax (StringKit.toInt (storePveM.getValueString ()));
		}

		//设置行动力 体力倒计时
		if (pve_t != null) {
			UserManager.Instance.updateUserPvESpeed (UserManager.PVE_SPEED - StringKit.toInt (pve_t.getValueString ()));
		}

		if (storePveT != null) {
			UserManager.Instance.updateStorePvESpeed (MountsConfigManager.Instance.getPveSpeed () - StringKit.toInt (storePveT.getValueString ()));
		}
		
		if (pvp_t != null) {
			UserManager.Instance.updateUserPvPSpeed (UserManager.PVP_SPEED - StringKit.toInt (pvp_t.getValueString ()));
		}

		if (chv_t != null) {
			UserManager.Instance.updateUserChvSpeed (StringKit.toInt (chv_t.getValueString ()));
		} 
		//改变行动力 体力 如果 为满值 清空 体力行动力剩余倒计时
		if (pve != null) { 
			UserManager.Instance.updateUserPvE (StringKit.toInt (pve.getValueString ()));	
		}

		if (storePve != null) { 
			UserManager.Instance.updateMountsPvE (StringKit.toInt (storePve.getValueString ()));	
		}
		
		if (pvp != null) {
			UserManager.Instance.updateUserPvP (StringKit.toInt (pvp.getValueString ()));
		}

		if (chv != null) {
			UserManager.Instance.updateUserChv (StringKit.toInt (chv.getValueString ()));
		}

		if (friend_size != null) {
			FriendsManagerment.Instance.getFriends ().updateMaxSize (StringKit.toInt (friend_size.getValueString ()));
		}
		
		if (card_size != null) {
			StorageManagerment.Instance.setRoleStorageMaxSpace (StringKit.toInt (card_size.getValueString ()));
		}
		if (equip_size != null) {
			StorageManagerment.Instance.setEquipStorageMaxSpace (StringKit.toInt (equip_size.getValueString ()));
		}
		if (goods_size != null) {
			StorageManagerment.Instance.setPropStorageMaxSpace (StringKit.toInt (goods_size.getValueString ()));
		}
		if (beast_size != null) {
			StorageManagerment.Instance.setBeastStorageMaxSpace (StringKit.toInt (beast_size.getValueString ()));
		}
		if (temp_size != null) {
			StorageManagerment.Instance.setTempStorageMaxSpace (StringKit.toInt (temp_size.getValueString ()));
		}
		if (ladder_rank != null) {
			LaddersManagement.Instance.currentPlayerRank = StringKit.toInt (ladder_rank.getValueString ());
		}
        if(magicWeapon_size!=null){//后台广播秘宝仓库的长度
            StorageManagerment.Instance.setMagicWeaponStorageMaxSpace(StringKit.toInt(magicWeapon_size.getValueString()));
        }
	}
} 

