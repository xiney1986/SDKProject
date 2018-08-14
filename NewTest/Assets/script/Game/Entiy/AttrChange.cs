using System;
 
/**
 * 属性变化
 * @author longlingquan
 * */
public class AttrChange
{
	public string type;
	public int num = 0;

	public AttrChange (string type, int num)
	{
		this.type = type;
		this.num = num;
		
	}
	
	public string	typeToString ()
	{
		switch (type) {
		case AttrChangeType.HP:
			return LanguageConfigManager.Instance.getLanguage ("s0005");
			
		case AttrChangeType.ATTACK:
			return LanguageConfigManager.Instance.getLanguage ("s0006");

		case AttrChangeType.DEFENSE:
			return LanguageConfigManager.Instance.getLanguage ("s0007");


		case AttrChangeType.MAGIC:
			return LanguageConfigManager.Instance.getLanguage ("s0008");

		case AttrChangeType.AGILE:
			return LanguageConfigManager.Instance.getLanguage ("s0009");
		
		}
		return "";
	}
	
	
} 

