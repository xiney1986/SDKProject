using System;
 
/**装备模板管理器
  *负责装备模板信息的初始化 
  *@author longlingquan
  **/
public class EquipmentSampleManager:SampleConfigManager
{
	//单例
	private static EquipmentSampleManager instance;

	public EquipmentSampleManager ()
	{

		base.readConfig (ConfigGlobal.CONFIG_EQUIP);
	}
	
	public static EquipmentSampleManager Instance {
		get{
			if(instance==null)
				instance=new EquipmentSampleManager();
			return instance;
		}
	} 
	
	//获得装备模板对象
	public EquipSample getEquipSampleBySid (int sid)
	{
		if (!isSampleExist (sid))
			createSample (sid); 
		return samples [sid] as EquipSample;
	}

    public int getNextQualityEquipSampleSid(int sid)
    {
        return getEquipSampleBySid(sid).redEquipSid;
    }

    //获得装备的精炼对象
    public RefineSample getEquipSampleRefineByEXPSid(int sid)
    {
        if (!isSampleExist(sid))
            createSample(sid);
        return samples[sid] as RefineSample;
    }
	 
	 
	
	//获得装备模板基础属性值
	public int getBaseAttribute (int sid, AttributeType attr)
	{
		if (attr == AttributeType.attack) {
			return getEquipSampleBySid (sid).baseAttack;
		} else if (attr == AttributeType.hp) {
			return getEquipSampleBySid (sid).baseLife;
		} else if (attr == AttributeType.defecse) {
			return getEquipSampleBySid (sid).baseDefecse;
		} else if (attr == AttributeType.magic) {
			return getEquipSampleBySid (sid).baseMagic;
		} else if (attr == AttributeType.agile) {
			return getEquipSampleBySid (sid).baseAgile;
		} else {
			throw new Exception ("RoleSampleManager getRoleBaseAttribute role attribute error! attr = " + attr);
		} 
	}
	
	//获得装备属性等级成长值
	public int getLevelUpAttribute (int sid, AttributeType attr)
	{
		if (attr == AttributeType.attack) {
			return getEquipSampleBySid (sid).developAttack;
		} else if (attr == AttributeType.hp) {
			return getEquipSampleBySid (sid).developLife;
		} else if (attr == AttributeType.defecse) {
			return getEquipSampleBySid (sid).developDefecse;
		} else if (attr == AttributeType.magic) {
			return getEquipSampleBySid (sid).developMagic;
		} else if (attr == AttributeType.agile) {
			return getEquipSampleBySid (sid).developAgile;
		} else {
			throw new Exception ("RoleSampleManager getRoleLevelUpAttribute role attribute error! attr = " + attr);
		}
	} 
	
	//解析模板数据
	public override void parseSample (int sid)
	{
		EquipSample sample = new EquipSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}
} 

