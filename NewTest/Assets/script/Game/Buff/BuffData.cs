using System.Collections.Generic;

public class BuffData
{
	
	public BuffCtrl buffCtrl;
//	public BuffDataBase database;
	public Buff sample;
	public int serverDamage;//实际伤害
	public int serverAttr_attack;//属性改变
	public int serverAttr_defend;
	public int serverAttr_magic;
	public int serverAttr_dex;
	public int 	replaceID;//要替换掉的对应IDbuff
	public int 	id;//服务器唯一id
	public int 	sid;//本地样本id
	public	string damageEffect;//buff附身时候的效果
	
	public BuffData (int buffID, int damage)
	{
		sample = BuffManagerment.Instance.createBuff(buffID);
	//	database = BuffManager.Instance.getBuffDataBase (buffID);
		serverDamage = damage;
		sid = buffID;
	}

	public string[] getBuffDesc()
	{
		List<string> desc=new List<string>();
		string attribute=string.Empty;

		if(serverAttr_attack!=0)
		{
			attribute=LanguageConfigManager.Instance.getLanguage("s0457");
			//attribute+=(serverAttr_attack>0?"+"+serverAttr_attack:serverAttr_attack.ToString());
			attribute=getSuffixDesc(serverAttr_attack,attribute);
			desc.Add(attribute);
		}

		if(serverAttr_defend!=0)
		{
			attribute=LanguageConfigManager.Instance.getLanguage("s0458");
			//attribute+=(serverAttr_defend>0?"+"+serverAttr_defend:serverAttr_defend.ToString());
			attribute=getSuffixDesc(serverAttr_defend,attribute);
			desc.Add(attribute);
		}

		if(serverAttr_magic!=0)
		{
		    if (sample.sid == 28223)
		    {
                attribute = LanguageConfigManager.Instance.getLanguage("s04600l");
		        //attribute+=(serverAttr_magic>0?"+"+serverAttr_magic:serverAttr_magic.ToString());
		        attribute = getSuffixDesc(serverAttr_magic, attribute);
		        desc.Add(attribute);
		    }
		    else
		    {
                attribute = LanguageConfigManager.Instance.getLanguage("s0459");
                //attribute+=(serverAttr_magic>0?"+"+serverAttr_magic:serverAttr_magic.ToString());
                attribute = getSuffixDesc(serverAttr_magic, attribute);
                desc.Add(attribute);
		    }
			
		}

		if(serverAttr_dex!=0)
		{
		    if (sample.sid == 28223)
		    {
		        attribute = LanguageConfigManager.Instance.getLanguage("s04600l");
		        //attribute+=(serverAttr_magic>0?"+"+serverAttr_magic:serverAttr_magic.ToString());
		        attribute = getSuffixDesc(serverAttr_dex, attribute);
		        desc.Add(attribute);
		    }
		    else
		    {
                attribute = LanguageConfigManager.Instance.getLanguage("s0460");
                //attribute+=(serverAttr_dex>0?"+"+serverAttr_dex:serverAttr_dex.ToString());
                attribute = getSuffixDesc(serverAttr_dex, attribute);
                desc.Add(attribute);
		    }
			
		}
		return desc.ToArray();
	}	
	private string getSuffixDesc(int value,string desc)
	{
		if(value>0)
		{
			return "[FF9900]"+desc+LanguageConfigManager.Instance.getLanguage("s0455");
		}else
		{
			return "[99CC33]"+desc+LanguageConfigManager.Instance.getLanguage("s0456");
		}
	}	
}
