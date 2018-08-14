using System;
using System.Collections.Generic;
/**
 * 装备实体类
 * @author longlingquan
 * */
using System.Linq;

public class Equip:StorageProp
{
	public Equip ()
	{
		this.isU = true;//写死了
	}

	public Equip (string uid, int sid, long exp, int state, int starAttState,long reexp)
	{

		this.uid = uid;
		this.sid = sid;
		updateExp (exp);
        updatereExp(reexp);
		this.state = state;
		this.isU = true;//写死了
		this.equpStarState = starAttState;
		equipSample = EquipmentSampleManager.Instance.getEquipSampleBySid (sid);
		if(equipSample.equipStarSid != 0)
			starSample = EquipStarAttrSampleManager.Instance.getEquipStarAttrSampleBySid(equipSample.equipStarSid);
	}

	public long exp = 0;//经验值 
	public int level = 0;//等级
	public int state = 0;//状态
	public bool isNew;//是否是新获得的装备
	public int equpStarState;  //升星状态
    public int refineLevel = 0;//精炼等级
    public long reexp = 0;//精炼经验值

	private EquipSample equipSample;
	private EquipStarAttrSample starSample;
	//** 属性效果 */
	private AttrChangeSample[] starEffects;
	
	//获得当前等级
	public int getLevel ()
	{ 
		return level;
	}
    //获得当前的精炼等级
    public int getrefineLevel()
    {
        return refineLevel;
    }
	public PrizeSample[] getResolveResults ()
	{
		PrizeSample[] sr = EquipmentSampleManager.Instance.getEquipSampleBySid (sid).resolveResults;
		if (sr == null)
			return null;
		List<PrizeSample> pList = new List<PrizeSample> ();
		for (int i=0; i<sr.Length; i++) {
			pList.Add(sr[i]);
		}
		List<PrizeSample> tmp;
		if (equpStarState > 0) {
			tmp = EquipStarConfigManager.Instance.getCrystalConsume(sid,equpStarState);
			ListKit.AddRange(pList,tmp);
			tmp = EquipStarConfigManager.Instance.getStoneConsume(sid,equpStarState);
			ListKit.AddRange(pList,tmp);

            //升红后，10-18星消耗金币
		    if (equpStarState > 9)
		    {
		        tmp = EquipStarConfigManager.Instance.getMoneyConsume(sid, equpStarState);
                ListKit.AddRange(pList,tmp);
		    }
		}
        List<PrizeSample> rePList = getReexpResolveResults(reexp);
        ListKit.AddRange(pList, rePList);
		return pList.ToArray();
	}

    public List<PrizeSample> getReexpResolveResults(long reexp) {
        List<PrizeSample> pList = new List<PrizeSample>();
        int[] reexpValue =CommandConfigManager.Instance.getRefinePropEXP();
        int[] reeexpPropSid = CommandConfigManager.Instance.getRefinePropSid();
        long exp = (long)(reexp*0.8f);
        for (int i = 0; i < reexpValue.Length; i++) {
            long num = exp / reexpValue[reexpValue.Length - 1 - i];
            if (num <= 0)
                continue;
            PrizeSample tmp = new PrizeSample();
            exp = exp % reexpValue[reexpValue.Length - 1 - i];
            tmp.type = 3;
            tmp.pSid = reeexpPropSid[reeexpPropSid.Length - 1 - i];
            tmp.num = num.ToString();
            pList.Add(tmp);
        }
        return pList;
    }
	public int getIntensifyCast ()
	{
		return  IntensifyCostManager.Instance.getCostListBySid (3) [getQualityId () - 1];
	}	
	
	//改变经验值
	public void updateExp (long exp)
	{
		this.exp = exp;
		updateLevel ();
	}
    /// <summary>
    /// 得到装备的精炼经验
    /// </summary>
    /// <param name="reexp"></param>
    public void updatereExp(long reexp)
    {
        if (RefineSampleManager.Instance.getRefineSampleBySid(sid) == null)
        {
            this.reexp = 0;
            return;
        }
        this.reexp = reexp;
        updaterefineLevel();
    }
	
	//更新等级
	private void updateLevel ()
	{
		level = EXPSampleManager.Instance.getLevel (getEXPSid (), exp, level); 
	}
    
    public int getRefineExpSid()
    {
        if (RefineSampleManager.Instance.getRefineSampleBySid(sid)==null)
        {
            return 0;
        }
        return RefineSampleManager.Instance.getRefineSampleBySid(sid).equipRefineLevelId;
    }
	
	public long getEXP ()
	{
		return exp;
	}
    public long getrefineEXP()
    {
        return reexp;
    }
	
	public int getEXPSid ()
	{
		return EquipmentSampleManager.Instance.getEquipSampleBySid (sid).levelId;
	}
	//获得装备名称
	public string getName ()
	{
		return EquipmentSampleManager.Instance.getEquipSampleBySid (sid).name;
	}
	
	//获得装备图标编号
	public int getIconId ()
	{
		return EquipmentSampleManager.Instance.getEquipSampleBySid (sid).iconId;
	}
	
	//获得品质编号
	public int getQualityId ()
	{ 
		return EquipmentSampleManager.Instance.getEquipSampleBySid (sid).qualityId;
	}
	
	//获得装备最高等级
	public int getMaxLevel ()
	{
		return EquipmentSampleManager.Instance.getEquipSampleBySid (sid).maxLevel;
	}
   
	
	//获得满级时的经验值
	public long getMaxExp ()
	{
		return EXPSampleManager.Instance.getMaxExp (EquipmentSampleManager.Instance.getEquipSampleBySid (sid).levelId);
	}
	
	//获得当前等级经验值上限
	public long getEXPUp ()
	{
		if (level == 0)
			getLevel ();
		return EXPSampleManager.Instance.getEXPUp (getEXPSid (), level);
	} 
	//获得精炼当前等级经验上限
    public long getRefineEXPUp()
    {
        if (refineLevel == 0)
            getrefineLevel();
        return EXPSampleManager.Instance.getRefineEXPUp(getRefineExpSid(), refineLevel);
            
    }
		
	//获得当前等级经验下限
	public long getEXPDown ()
	{
		if (level == 0)
			getLevel ();
		return EXPSampleManager.Instance.getEXPDown (getEXPSid (), level);
	}
    //获取精炼当前等级下限
    public long getRefineEXPDown()
    {
        if (refineLevel == 0)
            getrefineLevel();
        return EXPSampleManager.Instance.getEXPDown(getRefineExpSid(), refineLevel);
    }
	
	public long getEXPDown (int level)
	{
		if (level == 0)
			getLevel ();
		return EXPSampleManager.Instance.getEXPDown (getEXPSid (), level);
	}
	
	//获得被吃经验 基础经验+当前经验
	public long getEatenExp ()
	{
		return EquipmentSampleManager.Instance.getEquipSampleBySid (sid).eatenExp + exp;
	}

	/// <summary>
	/// 是否是祭品
	/// </summary>
	public bool isToEat ()
	{
		return ChooseTypeSampleManager.Instance.isToEat (this,ChooseTypeSampleManager.TYPE_EQUIP_EXP);
	}
	
	//得到装备附带技能
	public Skill[] getSkills ()
	{ 
		return null;
	}
	//得到装备state
	public int getState ()
	{ 
		return state;
	}
	//是否能够被指定角色装备装备
	public bool isPutOn (int rolesid)
	{
		if (ChooseTypeSampleManager.Instance.isToEat (this, ChooseTypeSampleManager.TYPE_EQUIP_EXP))
			return false;
		int[] e_id = EquipmentSampleManager.Instance.getEquipSampleBySid (sid).exclusive; 
		//0表示无角色限制
		if (e_id == null || e_id.Length < 1 || e_id [0] == 0) {
			return true;
		} else {
			for (int i = 0; i < e_id.Length; i++) {
				if (e_id [i] == rolesid) {
					return true;
				}
			}
		}
		return false;
		
	}
	
	//得到所属套装id
	public int getSuitSid ()
	{
		return EquipmentSampleManager.Instance.getEquipSampleBySid (sid).suitSid;
	}

    public bool isCanReine()
    {
        return RefineSampleManager.Instance.getRefineSampleBySid(sid) != null;
    }
	//获得所属部位id
	public int getPartId ()
	{
		return EquipmentSampleManager.Instance.getEquipSampleBySid (sid).partId;
	}
	
	//获得生命值
	public int getHP ()
	{
		return EquipManagerment.Instance.getEquipAttribute (sid, getLevel (), AttributeType.hp) + getNeedTypeValue(AttributeType.hp);
	}
	
	//获得防御值
	public int getDefecse ()
	{
		return EquipManagerment.Instance.getEquipAttribute (sid, getLevel (), AttributeType.defecse) + getNeedTypeValue(AttributeType.defense);
	}
	
	//获得攻击力
	public int getAttack ()
	{
		return EquipManagerment.Instance.getEquipAttribute (sid, getLevel (), AttributeType.attack) + getNeedTypeValue(AttributeType.attack);
	}
	
	//获得魔力
	public int getMagic ()
	{
		return EquipManagerment.Instance.getEquipAttribute (sid, getLevel (), AttributeType.magic) + getNeedTypeValue(AttributeType.magic);
	}
	
	//获得敏捷
	public int getAgile ()
	{
		return EquipManagerment.Instance.getEquipAttribute (sid, getLevel (), AttributeType.agile) + getNeedTypeValue(AttributeType.agile);
	}
	
	public int getJob ()
	{
		return EquipmentSampleManager.Instance.getEquipSampleBySid (sid).equipJob;
	}
	
	//获得装备卖出价格
	public int getSellPrice ()
	{
		return EquipmentSampleManager.Instance.getEquipSampleBySid (sid).sell;
	}
	
	//获得战力值
	public int getPower ()
	{
		return EquipManagerment.Instance.getEquipPower (this);
	}

    public int getBasePower()
    {
        return EquipManagerment.Instance.getEquipBasePower(this);
    }

    //技能描述(根据等级获得不同描述)
	public string getDescribe ()
	{
		string desc = EquipmentSampleManager.Instance.getEquipSampleBySid (sid).desc;
		AttrChangeSample[] changes = EquipmentSampleManager.Instance.getEquipSampleBySid (sid).effects;
		return DescribeManagerment.getDescribe (desc, getLevel (), changes); 
	}
	//获得装备属性
	public AttrChange[] getAttrChanges ()
	{
		return getAttrChanges (level);
	}
	//获得装备属性
	public AttrChange[] getAttrChanges (int level)
	{
		List<AttrChange> list = new List<AttrChange> ();
		int hp = EquipManagerment.Instance.getEquipAttribute (sid, level, AttributeType.hp);
		if (hp != 0) {
			AttrChange attrHp = new AttrChange (AttrChangeType.HP, hp);
			list.Add (attrHp);
		}
		
		int attack = EquipManagerment.Instance.getEquipAttribute (sid, level, AttributeType.attack);
		if (attack != 0) {
			AttrChange attrAtt = new AttrChange (AttrChangeType.ATTACK, attack);
			list.Add (attrAtt);
		}
		
		int agi = EquipManagerment.Instance.getEquipAttribute (sid, level, AttributeType.agile);
		if (agi != 0) {
			AttrChange attrAgi = new AttrChange (AttrChangeType.AGILE, agi);
			list.Add (attrAgi);
		}
		
		int mag = EquipManagerment.Instance.getEquipAttribute (sid, level, AttributeType.magic);
		if (mag != 0) {
			AttrChange attrMag = new AttrChange (AttrChangeType.MAGIC, mag);
			list.Add (attrMag);
		}
		
		int def = EquipManagerment.Instance.getEquipAttribute (sid, level, AttributeType.defecse);
		if (def != 0) {
			AttrChange attrDef = new AttrChange (AttrChangeType.DEFENSE, def);
			list.Add (attrDef);
		}
		return list.ToArray (); 
	}

	//检测装备状态 处于初始状态返回true
	public bool freeState ()
	{
		return state == 0;
	}
	//检测装备状态 处于对应状态返回true
	public bool checkState (int _state)
	{
		if (state == _state)
			return true;
		else
			return (state & _state) > 0;
	}

	public override bool equal (StorageProp prop)
	{
		return this.uid == prop.uid;
	}

	public override void bytesRead (int j, ErlArray ea)
	{
		this.uid = ea.Value [j++].getValueString ();
		this.sid = StringKit.toInt (ea.Value [j++].getValueString ());
		updateExp (StringKit.toLong (ea.Value [j++].getValueString ()));
		this.state = StringKit.toInt (ea.Value [j++].getValueString ());
		this.equpStarState = StringKit.toInt (ea.Value [j++].getValueString ());
        updatereExp(StringKit.toLong(ea.Value[j++].getValueString()));
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
	}
	///<summary>
	/// 检查升星条件是否满足
	/// </summary>
	private bool checkEquipStarConditions(){
		return false;
	}

	///<summary>
	/// 获得指定类型的升星加成值
	/// </summary>
	private int getNeedTypeValue(AttributeType type){
		int result = 0;
		equipSample = EquipmentSampleManager.Instance.getEquipSampleBySid (this.sid);
		if(equipSample.equipStarSid != 0)
			starSample = EquipStarAttrSampleManager.Instance.getEquipStarAttrSampleBySid(equipSample.equipStarSid);
		if (starSample == null||this.equpStarState ==0)
			return 0;
		starEffects = starSample.getAttrChangeSample(this.equpStarState);
		for (int i=0; i<starEffects.Length; i++) {
			if(starEffects[i].getAttrType() == type.ToString())
				result += starEffects[i].getAttrValue(0);
		}
		return result;
	}
    //更新精炼等级
    private void updaterefineLevel()
    {
        refineLevel = EXPSampleManager.Instance.getRefineLevel(getRefineExpSid(), reexp);
    }
    //获得装备精炼最高等级
    public int getRefineMaxLevel()
    {
        if (RefineSampleManager.Instance.getRefineSampleBySid(sid)==null)
        {
            return 0;
        }
        return RefineSampleManager.Instance.getRefineSampleBySid(sid).equipRefineMaxLevelId;
    }

}  