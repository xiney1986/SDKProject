using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 星魂实体对象
/// </summary>
public class StarSoul : StorageProp {

	/* fields */
	/** 经验值 */
	public long exp = 0;
	/** 等级 */
	public int level = 0;
	/** 状态(具体情况见:EquipStateType) */
	public int state = 0;
	/** 是否是新获得的星魂 */
	public bool isNew;
	/** 刻印 --功能暂时未开放*/
	public int partId = 0;

	/* methods */
	public StarSoul () {
		this.isU = true;//写死了
	}
	public StarSoul (string uid, int sid, long exp, int state) {
		this.uid = uid;
		this.sid = sid;
		updateExp (exp);
		this.state = state;
		this.isU = true;//写死了
	}
	/// <summary>
	/// 获得战力值
	/// </summary>
	public int getPower () {
		int tempPower=0;
		return 0;
	}
	/// <summary>
	/// 改变经验值
	/// </summary>
	/// <param name="exp">改变的经验值</param>
	public void updateExp (long exp) {
		this.exp = exp;
		updateLevel ();
	}
	/** 更新等级 */
	private void updateLevel () {
		level = EXPSampleManager.Instance.getLevel (getEXPSid (),exp, level); 
	}
	/// <summary>
	/// 获得满级时的经验值
	/// </summary>
	public long getMaxExp () {
		return EXPSampleManager.Instance.getMaxExp (getEXPSid());
	}
	/// <summary>
	/// 获得当前等级经验值上限
	/// </summary>
	public long getEXPUp () {
		if (level == 0)
			getLevel ();
		return EXPSampleManager.Instance.getEXPUp (getEXPSid (), level);
	} 
	/// <summary>
	/// 获得当前等级经验下限
	/// </summary>
	public long getEXPDown () {
		if (level == 0)
			getLevel ();
		return EXPSampleManager.Instance.getEXPDown (getEXPSid (), level);
	}
	/// <summary>
	/// 获取等级经验下标
	/// </summary>
	/// <param name="level">等级</param>
	public long getEXPDown (int level) {
		if (level == 0)
			getLevel ();
		return EXPSampleManager.Instance.getEXPDown (getEXPSid (), level);
	}
	/// <summary>
	/// 属性描述(根据等级获得不同描述)
	/// </summary>
	public string getDescribe () {
		string desc = getDesc();
		AttrChangeSample[] changes = getStarSoulSample().getAttrChangeSample();
		return DescribeManagerment.getDescribe (desc, getLevel (), changes); 
	}
	/// <summary>
	/// 获得刻印模板
	/// </summary>
	public StarSoulSuitSample getStarSoulSuitSample () {
		int suitSid = getSuitSid();
		StarSoulSuitSample starSoulSuitSample=StarSoulSuitSampleManager.Instance.getStarSoulSuitSampleBySid (suitSid);
		return starSoulSuitSample;
	}
	/// <summary>
	/// 获得指定类型,等级的星魂属性值
	/// </summary>
	/// <param name="type">类型</param>
	/// <param name="level">等级</param>
	public int getAttrChangesByType (string type,int level) {
		StarSoulSample starSoulSample=getStarSoulSample ();
		AttrChangeSample attrChangeSample=starSoulSample.getAttrChangeSampleByType (type);
		if (attrChangeSample == null)
			return 0;
		return attrChangeSample.getAttrValue (level);
	}
	/// <summary>
	/// 获得星魂非属性战斗力.
	/// </summary>
	/// <param name="level">星魂等级.</param>
	public int getUnpropertyAttr(int level) {
		StarSoulSample starSoulSample=getStarSoulSample ();
		return starSoulSample.nAttrComat+starSoulSample.nDevelopComat*(level-1);
	}
	/// <summary>
	/// 获得所有星魂属性
	/// </summary>
	public AttrChangeSample[] getAttrChangesByAll () {
		StarSoulSample starSoulSample=getStarSoulSample ();
		AttrChangeSample[] attrChangeSample=starSoulSample.getAttrChangeSample ();
		return attrChangeSample;
	}
	/// <summary>
	/// 检查星魂能否装配.
	/// </summary>
	public bool checkStarSoulCanbePut(Card card,StarSoul resStarSoul) {
		StarSoulManager manager=StarSoulManager.Instance;
		int[] arr=manager.getCardSoulExistType(card,resStarSoul);
		for(int i=0;i<arr.Length;i++){
			if(this.getStarSoulType()==arr[i])return false;
		}
		return true;
	}
	/// <summary>
	/// 是否达到满级
	/// </summary>
	public bool isMaxLevel () {
		if (getLevel () == getMaxLevel ())
			return true;
		else
			return false;
	}
	//检测装备状态 处于初始状态返回true
	public bool freeState () {
		return state == 0;
	}
	/// <summary>
	/// 检测星魂状态
	/// </summary>
	/// <param name="_state">_state.</param>
	public bool checkState (int _state) {
		if (state == _state)
			return true;
		else
			return (state & _state) > 0;
	}
	/// <summary>
	/// 设置星魂状态
	/// </summary>
	/// <param name="_state">二进制值</param>
	public void setState(int _state) {
		state |= _state;
	}
	/// <summary>
	/// 清理指定状态值
	/// </summary>
	/// <param name="_state">二进制值</param>
	public void unState(int _state) {
		state&=~_state;
	}
	/** 是否uid相等 */
	public override bool equal (StorageProp prop) {
		return this.uid == prop.uid;
	}
	/** 序列化读取可变属性数据 */
	public override void bytesRead (int j, ErlArray ea) {
		this.uid = ea.Value [j++].getValueString ();
		this.sid = StringKit.toInt (ea.Value [j++].getValueString ());
		updateExp (StringKit.toLong (ea.Value [j++].getValueString ()));
		this.state = StringKit.toInt (ea.Value [j++].getValueString ());
//		this.partId = StringKit.toInt (ea.Value [j++].getValueString ());
	}
	/** 克隆 */
	public override void copy (object destObj) {
		base.copy (destObj);
	}
	public string ToString() {
		return "[uid="+uid+",sid="+sid+",exp="+exp+",state="+state+"]";
	}

	/* properties */
	/// <summary>
	/// 获得星魂实体对象
	/// </summary>
	public StarSoulSample getStarSoulSample () {
		return StarSoulSampleManager.Instance.getStarSoulSampleBySid (sid);
	}
	/// <summary>
	/// 获取星魂属性激活映射编号
	/// </summary>
	public int getSuitSid () {
		return getStarSoulSample().suitSid;
	}
	/// <summary>
	/// 获取星魂卖出价格
	/// </summary>
	public int getSellPrice () {
		return getStarSoulSample().sell;
	}
	/** 获取经验映射编号 */
	public int getEXPSid () {
		return getStarSoulSample().levelSid;
	}
	/** 获得星魂名称 */
	public string getName () {
		return getStarSoulSample().name;
	}
	/** 获得星魂图标编号 */
	public int getIconId () {
		return getStarSoulSample().iconId;
	}
	/** 获得星魂品质编号 */
	public int getQualityId () { 
		return getStarSoulSample().qualityId;
	}
	/** 获得星魂最高等级 */
	public int getMaxLevel () {
		return getStarSoulSample().maxLevel;
	}
	/** 获得星魂属性描述 */
	public string getDesc () {
		return getStarSoulSample().desc;
	}
	/// <summary>
	/// 获得被吃经验 基础经验+当前经验
	/// </summary>
	public long getEatenExp () {
		return getStarSoulSample ().eatenExp + exp;
	}
	/** 获取星魂类型 */
	public int getStarSoulType () {
		return getStarSoulSample().starSoulType;
	}
	/** 获取状态 */
	public int getState () { 
		return state;
	}
	/** 获取当前经验 */
	public long getEXP () {
		return exp;
	}
	/** 获得当前等级 */
	public int getLevel () { 
		return level;
	}
}
