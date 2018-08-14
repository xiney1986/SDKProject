using System;
 
/**
 * 仓库更新服务
 * @author longlingquan
 * */
public class StorageUpdateService:BaseFPort
{
	
	private const string KEY_CARD = "card";
	private const string KEY_EQUIP = "equip";
	private const string KEY_TOOL = "goods";//后台用的是goods
	private const string KEY_BEAST = "beast";
	private const string KEY_TEMP = "temp";
	public const string STAR_SOUL = "star_soul";// 星魂仓库
	public const string STAR_SOUL_DRAW = "star_soul_draw"; // 裂魂仓库
    public const string MAGIC_WEAPON = "artifact";//秘宝仓库

	public StorageUpdateService ()
	{
		
	}
	
	public override void read (ErlKVMessage message)
	{
//		MonoBase.print (GetType () + "=================read");	
		ErlArray tools = message.getValue (KEY_TOOL) as ErlArray;
		if (tools != null) { 
			StorageManagerment.Instance.updateStorageInfo (StorageFPort.GOODS, tools);
		}
		
		ErlArray cards = message.getValue (KEY_CARD) as ErlArray;
		if (cards != null) { 
			StorageManagerment.Instance.updateStorageInfo (StorageFPort.CARD, cards);
		}
		
		ErlArray equips = message.getValue (KEY_EQUIP) as ErlArray;
		if (equips != null) {
			StorageManagerment.Instance.updateStorageInfo (StorageFPort.EQUIPMENT, equips);
		}
		
		ErlArray beasts = message.getValue (KEY_BEAST) as ErlArray;
		if (beasts != null) {
			StorageManagerment.Instance.updateStorageInfo (StorageFPort.BEAST, beasts);
		}
		
		ErlArray temp = message.getValue (KEY_TEMP) as ErlArray;
		if (temp != null) {
			StorageManagerment.Instance.updateStorageInfo (StorageFPort.TEMP, temp);
		}

		ErlArray starSoul = message.getValue (STAR_SOUL) as ErlArray;
		if (starSoul != null) {
			StorageManagerment.Instance.updateStorageInfo (StorageFPort.STAR_SOUL_STORAGE, starSoul);
		}

		ErlArray hunStarSoul = message.getValue (STAR_SOUL_DRAW) as ErlArray;
        if (hunStarSoul != null) {
            StorageManagerment.Instance.updateStorageInfo(StorageFPort.STAR_SOUL_DRAW_STORAGE, hunStarSoul);
        }
        ErlArray magicWeapon = message.getValue(MAGIC_WEAPON) as ErlArray;
        if (magicWeapon != null) {
            StorageManagerment.Instance.updateStorageInfo(StorageFPort.MAGIC_WEAPON, magicWeapon);
        }
	}
} 

