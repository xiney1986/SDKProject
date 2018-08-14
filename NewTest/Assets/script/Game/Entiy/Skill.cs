using System;

/**
 * 技能实体
 * @author longlingquan
 * */
public class Skill:CloneObject
{
	public Skill (int sid, long exp, int type)
	{
		this.sid = sid;
		updateExp (exp);
		this.skillStateType = type;
	}


	//	public int id = 0;//技能编号
	public int sid = 0;//技能sid
	private long exp = 0;//技能经验值
	private int level = 0;//技能等级
	private int skillStateType = 0;//描述技能类型 SkillStateType

    //男战士技能
    public bool isFemaleSoldiersSkill()
    {
        if (sid == 168002 || sid == 168007 || sid == 168012 || sid == 168017 || sid == 168022 || sid == 168027)
            return true;
        return false;
    }

    //获得当前等级经验下限
	public long getEXPDown ()
	{
		if (level == 0)
			getLevel ();
		return EXPSampleManager.Instance.getEXPDown (getEXPSid (), level);
	}

	public long getEXPDown (int level)
	{
		return EXPSampleManager.Instance.getEXPDown (getEXPSid (), level);
	}

	//获得当前等级经验值上限
	public long getEXPUp ()
	{
		if (level == 0)
			getLevel ();
		return EXPSampleManager.Instance.getEXPUp (getEXPSid (), level);
	}

	public long getEXPUp (int level)
	{
		return EXPSampleManager.Instance.getEXPUp (getEXPSid (), level);
	}
	
	public int getSkillStateType ()
	{
		return skillStateType;
	}

	public void setSkillStateType (int type)
	{
		this.skillStateType = type;
	}

	//获得当前等级
	public int getLevel ()
	{ 
		return level;
	}

	public void setLevel (int leve)
	{
		this.level = leve;
	}

	//获得最高等级
	public int getMaxLevel ()
	{
		return SkillSampleManager.Instance.getSkillSampleBySid (sid).maxLevel;
	}

	public bool isMAxLevel ()
	{
		return level >= getMaxLevel ();
	}

	//改变经验值
	public void updateExp (long exp)
	{
		this.exp = exp;
		updateLevel ();
	}
	
	//更新等级
	private void updateLevel ()
	{ 
		SkillSampleManager.Instance.getSkillSampleBySid (sid);//初始化模板对象
		level = EXPSampleManager.Instance.getLevel (getEXPSid (), exp, level); 
	}
	
	public long getEXP ()
	{
		return exp;
	}
	
	public int getEXPSid ()
	{
		
		return SkillSampleManager.Instance.getSkillSampleBySid (sid).levelId;
	}
	
	//技能名字
	public string getName ()
	{
		return SkillSampleManager.Instance.getSkillSampleBySid (sid).name;
	}
	
	//图标路径+名字
	public string getIcon ()
	{
		return ResourcesManager.SKILLIMAGEPATH + SkillSampleManager.Instance.getSkillSampleBySid (sid).iconId;
	}
	
	//技能产生属性影响效果(影响角色本身属性) 同时对技能描述参数提供数值(影响描述信息数值)
	private AttrChangeSample[] getAttcChanges ()
	{
		return SkillSampleManager.Instance.getSkillSampleBySid (sid).effects;
	} 
	
	//技能类型
	public int getType ()
	{
		return SkillSampleManager.Instance.getSkillSampleBySid (sid).type;
	}

	/// <summary>
	/// 获得展示类型,1主动技能，2天赋，3特性
	/// </summary>
	/// <returns>1主动技能，2天赋，3特性.</returns>
	public int getShowType ()
	{
		return SkillSampleManager.Instance.getSkillSampleBySid (sid).showType; 
	}
	
	//技能品质
	public int getSkillQuality ()
	{
		return SkillSampleManager.Instance.getSkillSampleBySid (sid).quality;
	}
	
	//技能描述
	public string getDescribe ()
	{
		string desc = SkillSampleManager.Instance.getSkillSampleBySid (sid).describe;
		AttrChangeSample[] changes = SkillSampleManager.Instance.getSkillSampleBySid (sid).effects;
		return DescribeManagerment.getDescribe (desc, getLevel (), changes); 
	}
	
	//技能描述(根据等级获得不同描述)
	public string getDescribeByLv (int _Level)
	{
		string desc = SkillSampleManager.Instance.getSkillSampleBySid (sid).describe;
		AttrChangeSample[] changes = SkillSampleManager.Instance.getSkillSampleBySid (sid).effects;
		return DescribeManagerment.getDescribe (desc, _Level, changes); 
	}
	
	//技能时间触发类型 枚举
	public int getActiveType ()
	{
		return SkillSampleManager.Instance.getSkillSampleBySid (sid).activeType;
	}
	//环绕特效
	public string getAroundEffectPath ()
	{

		int id = SkillSampleManager.Instance.getSkillSampleBySid (sid).aroundEffect;
		return EffectConfigManager.Instance.getEffectPerfab (id);

	}
	//附带技能buff sid
	public int getBuffSid ()
	{
		return SkillSampleManager.Instance.getSkillSampleBySid (sid).buffSid;
	}
	
	//技能施法特效
	public string getSpellEffect ()
	{
		return SkillManagerment.Instance.getSpellEffect (sid);
	}

    public Skill getBackSkillSid()
    {
        UnityEngine.Debug.LogError("sid======="+sid);
        return new Skill(SkillSampleManager.Instance.getSkillSampleBySid(sid).spellEffect, 0, 0);
    }

    //获取单次攻击次数
	public int getAttackNum ()
	{
		return  SkillSampleManager.Instance.getSkillSampleBySid (sid).attackNum;
	}

	//是否需要吟唱
	public bool getIsNeedSpell ()
	{
		return SkillSampleManager.Instance.getSkillSampleBySid (sid).isNeedSpell;
	}
	
	//子弹特效
	public string getBulletEffect ()
	{
		return SkillManagerment.Instance.getSkillBulletPerfab (sid);
	}
	//子弹特效
	public bool CanHitBack ()
	{
		return SkillSampleManager.Instance.getSkillSampleBySid (sid).canHitBack;
	}

	//得到技能击中效果
	public String getDamageEffect ()
	{ 
		return SkillManagerment.Instance.getDamageEffect (sid);
	}
	
	//获得技能属性影响效果
	public CardBaseAttribute getSkillEffect ()
	{
		CardBaseAttribute attr = new CardBaseAttribute ();
		AttrChangeSample[] effects = getAttcChanges ();
		if (effects == null || effects.Length < 1)
			return attr;
		for (int i = 0; i < effects.Length; i++) {
			if (effects [i].getAttrType () == AttrChangeType.HP) {
				attr.hp += effects [i].getAttrValue (getLevel ());
			} else if (effects [i].getAttrType () == AttrChangeType.ATTACK) {
				attr.attack += effects [i].getAttrValue (getLevel ());
			} else if (effects [i].getAttrType () == AttrChangeType.DEFENSE) {
				attr.defecse += effects [i].getAttrValue (getLevel ());
			} else if (effects [i].getAttrType () == AttrChangeType.MAGIC) {
				attr.magic += effects [i].getAttrValue (getLevel ());
			} else if (effects [i].getAttrType () == AttrChangeType.AGILE) {
				attr.agile += effects [i].getAttrValue (getLevel ());
			} 
			//技能影响的属性百分比变化
			else if (effects [i].getAttrType () == AttrChangeType.PER_HP) {
				attr.perHp += effects [i].getAttrValue (getLevel ());
			} else if (effects [i].getAttrType () == AttrChangeType.PER_ATTACK) {
				attr.perAttack += effects [i].getAttrValue (getLevel ());
			} else if (effects [i].getAttrType () == AttrChangeType.PER_DEFENSE) {
				attr.perDefecse += effects [i].getAttrValue (getLevel ());
			} else if (effects [i].getAttrType () == AttrChangeType.PER_MAGIC) {
				attr.perMagic += effects [i].getAttrValue (getLevel ());
			} else if (effects [i].getAttrType () == AttrChangeType.PER_AGILE) {
				attr.perAgile += effects [i].getAttrValue (getLevel ());
			}
		}
		return attr;
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
	}
} 
