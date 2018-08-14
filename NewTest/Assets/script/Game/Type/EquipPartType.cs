using System;
 
/**
 * 装备位置类型
 * @author longlingquan
 * */
public class EquipPartType
{
	public const int WEAPON = 1, //武器
	ARMOUR = 2, //铠甲
	SHOSE = 3, //裤子
	HELMET = 4, //头盔 
	RING = 5;//戒指
	
	public EquipPartType ()
	{
	}
	
	//获得部位对应描述
	public static string getPartName (int part)
	{
		if (part == WEAPON) {
			return LanguageConfigManager.Instance.getLanguage ("s0078");
		} else if (part == ARMOUR) {
			return LanguageConfigManager.Instance.getLanguage ("s0079");
		} else if (part == SHOSE) {
			return LanguageConfigManager.Instance.getLanguage ("s0080");
		} else if (part == HELMET) {
			return LanguageConfigManager.Instance.getLanguage ("s0081");
		} else if (part == RING) {
			return LanguageConfigManager.Instance.getLanguage ("s0082");
		}
		return "";
	}
} 

