using UnityEngine;
using System.Collections;

/// <summary>
/// 类说明：坐骑实体类
/// </summary>
public class Mounts : StorageProp {

	/* const */
	/** 坐骑状态 */
	public const int MOUNTS_DOWN_STATE=0, // 休息
						MOUNTS_UP_STATE=1; // 上阵

	/* fields */
	/** 经验值 */
	private long exp;
	/** 等级 */
	private int level;
	/** 状态 0休息 1上阵 */
	private int state;
	/** 主动技能 */
	private Skill[] skills;
	private int closeTime;//坐骑的关闭时间

	/* methods */
	public Mounts () {
		this.isU = true;//写死了
	}
	public Mounts (string uid, int sid, int state,Skill[] skills,int closet) {
		this.uid = uid;
		this.sid = sid;
		this.state = state;
		this.skills = skills;
		this.closeTime=closet;
	}
	/// <summary>
	/// 获得技能
	/// </summary>
	public Skill[] getSkills () {
		if (skills != null && skills.Length > 0) {
			Skill[] newSkill = new Skill[skills.Length];
			for (int i = 0; i < skills.Length; i++) {
				newSkill[i] = getMountsManagerment().changeMineSkill (skills[i]);
			}
			return newSkill;
		} else {
			int[] tmpSkills = getMountsSample ().skills;
			if (tmpSkills == null || tmpSkills.Length <= 0) {
				return null;
			}
			Skill[] newSkill = new Skill[tmpSkills.Length];
			for (int i = 0; i < tmpSkills.Length; i++) {
				newSkill[i] = SkillManagerment.Instance.createSkill (tmpSkills[i]);
			}
			return newSkill;
		}
	}
	/// <summary>
	/// 获得技能属性全部加成
	/// </summary>
	public CardBaseAttribute getMountsSkillEffect () {
		CardBaseAttribute attr = new CardBaseAttribute ();
		Skill[] tempSkills = getSkills ();
		if (tempSkills == null || tempSkills.Length < 1)
			return attr;
		for (int i = 0; i < tempSkills.Length; i++) {
			attr.mergeCardBaseAttr (tempSkills [i].getSkillEffect ());
		}
		return attr;
	}
	/// <summary>
	/// 获得技能基础属性加成
	/// </summary>
	public CardBaseAttribute getMountsSkillEffectNum () {
		CardBaseAttribute attr = new CardBaseAttribute ();
		Skill[] tempSkills = getSkills ();
		if (tempSkills == null || tempSkills.Length < 1)
			return attr;
		for (int i = 0; i < tempSkills.Length; i++) {
			attr.mergeCardBaseNum (tempSkills [i].getSkillEffect ());
		}
		return attr;
	}
	/// <summary>
	/// 获得技能属性百分比加成
	/// </summary>
	public CardBaseAttribute getMountsSkillEffectPer () {
		CardBaseAttribute attr = new CardBaseAttribute ();
		Skill[] tempSkills = getSkills ();
		if (tempSkills == null || tempSkills.Length < 1)
			return attr;
		for (int i = 0; i < tempSkills.Length; i++) {
			attr.mergeCardBasePer (tempSkills [i].getSkillEffect ());
		}
		return attr;
	}
	/// <summary>
	/// 获得坐骑属性
	/// </summary>
	public CardBaseAttribute getMountsAddEffect () {
		CardBaseAttribute attr = new CardBaseAttribute ();
		attr.attack = getAttrByType (AttributeType.attack);
		attr.hp = getAttrByType (AttributeType.hp);
		attr.defecse = getAttrByType (AttributeType.defecse);
		attr.magic = getAttrByType (AttributeType.magic);
		attr.agile = getAttrByType (AttributeType.agile);
		return attr;
	}
	/// <summary>
	/// 获得对应基础属性值
	/// </summary>
	private int getBaseAttrNumByType (AttributeType type) {
		switch (type) {
		case AttributeType.hp:
			return getBaseLife ();
		case AttributeType.attack:
			return getBaseAttack ();
		case AttributeType.defecse:
			return getBaseDefecse ();
		case AttributeType.magic:
			return getBaseMagic ();
		case AttributeType.agile:
			return getBaseAgile ();
		}
		return 0;
	}
	/// <summary>
	/// 获得对应基础属性成长值
	/// </summary>
	private int getDevelopAttrNumByType (AttributeType type) {
		switch (type) {
		case AttributeType.hp:
			return getDevelopLife ();
		case AttributeType.attack:
			return getDevelopAttack ();
		case AttributeType.defecse:
			return getDevelopDefecse ();
		case AttributeType.magic:
			return getDevelopMagic ();
		case AttributeType.agile:
			return getDevelopAgile ();
		}
		return 0;
	}
	/// <summary>
	/// 根据等级获得对应类型
	/// </summary>
	private AttributeType getTypeByLevel (int level) {
		switch (getAddTypeByNowLevel(level)) {
		case 1:
			return AttributeType.hp;
		case 2:
			return AttributeType.attack;
		case 3:
			return AttributeType.defecse;
		case 4:
			return AttributeType.magic;
		case 5:
			return AttributeType.agile;
		default:
			return AttributeType.agile;
		}
	}
	/// <summary>
	/// 获得属性种类对应值
	/// </summary>
	private int getAttrNumByType (AttributeType type) {
		switch (type) {
		case AttributeType.hp:
			return 1;
		case AttributeType.attack:
			return 2;
		case AttributeType.defecse:
			return 3;
		case AttributeType.magic:
			return 4;
		case AttributeType.agile:
			return 5;
		}
		return 0;
	}
	/// <summary>
	/// Byteses the read.
	/// </summary>
	/// <param name="j">J.</param>
	/// <param name="ea">Ea.</param>
	public override void bytesRead (int j, ErlArray ea) {
		this.uid = ea.Value [j++].getValueString ();
		this.sid = StringKit.toInt (ea.Value [j++].getValueString ());
		this.state = StringKit.toInt (ea.Value [j++].getValueString ());
		this.closeTime = StringKit.toInt (ea.Value [j++].getValueString ());
	}

	/* properties */
	/// <summary>
	/// 获得坐骑模板
	/// </summary>
	public MountsSample getMountsSample () {
		return MountsSampleManager.Instance.getMountsSampleBySid (sid);
	}
	public MountsManagerment getMountsManagerment () {
		return MountsManagerment.Instance;
	}
	public MountsConfigManager getMountsConfigManager () {
		return MountsConfigManager.Instance;
	}
	/// <summary>
	/// 是否正在上阵中
	/// </summary>
	public bool isInUse () {
		return state == MOUNTS_UP_STATE;
	}
	/// <summary>
	/// 设置出战状态
	/// </summary>
	public void setState (bool bo) {
		state = bo ? MOUNTS_UP_STATE : MOUNTS_DOWN_STATE;		
	}
	/// <summary>
	/// 获得坐骑名字
	/// </summary>
	public string getName () {
		return getMountsSample ().name;
	}
	/// <summary>
	/// 获取战斗力
	/// </summary>
	public int getCombat () {
		return CombatManager.Instance.getMountsCombat (this);
	}
	/// <summary>
	/// 获得图标编号
	/// </summary>
	public int getImageID () {
		return getMountsSample ().imageID;
	}
	/// <summary>
	/// 3d模型路径
	/// </summary>
	public string getModelPath () {
		return "mounts/"+getMountsSample().modelID;
	}
	/// <summary>
	/// 获得品质编号
	/// </summary>
	public int getQualityId () {
		return getMountsSample ().qualityId;
	}
	/// <summary>
	/// 获得最高等级
	/// </summary>
	public int getMaxLevel () {
		return getMountsSample ().maxLevel;
	}
	/// <summary>
	/// 获得移动速度比率
	/// </summary>
	public float getSpeedPer () {
		return 1 + getMountsSample ().speed * 0.0001f;
	}
	/// <summary>
	/// 获得移动附加速度比率
	/// </summary>
	public float getAddSpeedPer () {
		return getMountsSample ().speed * 0.0001f;
	}
	/// <summary>
	/// 获得移动速度100倍
	/// </summary>
	public int getSpeed () {
		float f=getSpeedPer()*100;
		return (int)f;
	}
	/// <summary>
	/// 获得初始生命值
	/// </summary>
	public int getBaseLife () {
		return getMountsSample ().baseLife;
	}
	/// <summary>
	/// 获得初始攻击值
	/// </summary>
	public int getBaseAttack () {
		return getMountsSample ().baseAttack;
	}
	/// <summary>
	/// 获得初始防御值
	/// </summary>
	public int getBaseDefecse () {
		return getMountsSample ().baseDefecse;
	}
	/// <summary>
	/// 获得初始魔力值
	/// </summary>
	public int getBaseMagic () {
		return getMountsSample ().baseMagic;
	}
	/// <summary>
	/// 获得初始敏捷值
	/// </summary>
	public int getBaseAgile () {
		return getMountsSample ().baseAgile;
	}
	/// <summary>
	/// 获得生命成长值
	/// </summary>
	public int getDevelopLife () {
		return getMountsSample ().developLife;
	}
	/// <summary>
	/// 获得攻击成长值
	/// </summary>
	public int getDevelopAttack () {
		return getMountsSample ().developAttack;
	}
	/// <summary>
	/// 获得防御成长值
	/// </summary>
	public int getDevelopDefecse () {
		return getMountsSample ().developDefecse;
	}
	/// <summary>
	/// 获得魔力成长值
	/// </summary>
	public int getDevelopMagic () {
		return getMountsSample ().developMagic;
	}
	/// <summary>
	/// 获得敏捷成长值
	/// </summary>
	public int getDevelopAgile () {
		return getMountsSample ().developAgile;
	}
	/// <summary>
	/// 获得指定等级增加的属性类型对应值
	/// </summary>
	public int getAddAttrByLevel (int level) {
		return getAttrByType (getTypeByLevel (level));
	}
	/// <summary>
	/// 获得当前骑术等级影响的坐骑属性种类对应值
	/// </summary>
	public int getAddTypeByNowLevel (int level) {
		return (level - 1) % 5 + 1;
	}
	/// <summary>
	/// 获得指定坐骑指定类型属性值
	/// </summary>
	private int getAttrByType (AttributeType type) {
		//基础属性值 + int((骑术等级 + 基础属性种类对应值 - 2)/ 5)* 基础属性成长值
		return Mathf.FloorToInt ((getBaseAttrNumByType (type) + (int)((getMountsLevel () + getAttrNumByType (type) - 2) / 5) * getDevelopAttrNumByType(type)) * getAttrPerByOwn ());
	}
	/// <summary>
	/// 获得骑术等级
	/// </summary>
	public int getMountsLevel () {
//		return isMine () ? 1 : getMountsManagerment().getMountsLevel ();
		return getMountsManagerment().getMountsLevel (); //未激活的坐骑，展示显示战斗力时，也要算上骑术等级
	}
	/// <summary>
	/// 获得共鸣效果
	/// </summary>
	public float getAttrPerByOwn () {
		return isMine () ? 1 : getMountsConfigManager().getAttrPerByOwn ();
	}
	/// <summary>
	/// 是否是我的坐骑
	/// </summary>
	public bool isMine ()
	{
		return string.IsNullOrEmpty (this.uid) || this.uid == "0";
	}
	/// <summary>
	///  得到坐骑的关闭时间
	/// </summary>
	/// <returns>The mounts close time.</returns>
	public int getMountsCloseTime(){
		return closeTime;
	}

}
