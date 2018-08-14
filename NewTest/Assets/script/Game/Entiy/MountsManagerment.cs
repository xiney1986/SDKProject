using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 坐骑管理器
/// </summary>
public class MountsManagerment {

	/* static fields */

	/* static methods */
	public static MountsManagerment Instance {
		get{ return SingleManager.Instance.getObj ("MountsManagerment") as MountsManagerment;}
	}

	/* fields */
	/** 拥有的坐骑列表 */
	private List<Mounts> allMountsList;
	/** 坐骑仓库版本号 */
	private int mountsStorageVersion = -1;
	/** 骑术经验 */
	public long mountsExp;

	/* methods */
	public MountsManagerment () { 
	}
	/// <summary>
	/// 获得所有拥有的坐骑
	/// </summary>
	public List<Mounts> getAllMountsList () {
		if (allMountsList == null)
			allMountsList = new List<Mounts> ();
		if (mountsStorageVersion != StorageManagerment.Instance.mountsStorageVersion) {
			allMountsList.Clear();
			ArrayList allList = StorageManagerment.Instance.getAllMounts ();
			for (int i = 0; i < allList.Count; i++) {
				if(allList[i]==null) continue;
				allMountsList.Add (allList[i] as Mounts);
			}
			mountsStorageVersion=StorageManagerment.Instance.mountsStorageVersion;
		}
		return allMountsList;
	}
	/// <summary>
	/// 获得指定sid的坐骑
	/// </summary>
	public Mounts getMountsBySid(int sid) {
		allMountsList=getAllMountsList();
		Mounts mounts;
		for (int i = 0; i < allMountsList.Count; i++) {
			mounts=allMountsList[i];
			if (mounts.sid==sid) {
				return mounts;
			}
		}
		return null;
	}
	/// <summary>
	/// 获得指定uid的坐骑
	/// </summary>
	public Mounts getMountsByUid(string uid) {
		allMountsList=getAllMountsList();
		Mounts mounts;
		for (int i = 0; i < allMountsList.Count; i++) {
			mounts=allMountsList[i];
			if (mounts.uid==uid) {
				return mounts;
			}
		}
		return null;
	}
	/// <summary>
	/// 获取使用的坐骑属性
	/// </summary>
	public CardBaseAttribute getUseMountsAttribute () {
		Mounts mounts = MountsManagerment.Instance.getMountsInUse ();
		if (mounts != null) {
			return mounts.getMountsAddEffect ();
		}
		return new CardBaseAttribute ();
	}
	/// <summary>
	/// 获取使用的坐骑技能属性加成
	/// </summary>
	public CardBaseAttribute getUseMountsSkillEffect () {
		Mounts mounts = MountsManagerment.Instance.getMountsInUse ();
		if (mounts != null) {
			return mounts.getMountsSkillEffect ();
		}
		return new CardBaseAttribute ();
	}
	/// <summary>
	/// 获取使用的坐骑技能属性加成
	/// </summary>
	public CardBaseAttribute getUseMountsSkillEffectNum () {
		Mounts mounts = MountsManagerment.Instance.getMountsInUse ();
		if (mounts != null) {
			return mounts.getMountsSkillEffectNum ();
		}
		return new CardBaseAttribute ();
	}
	/// <summary>
	/// 获取使用的坐骑技能属性加成
	/// </summary>
	public CardBaseAttribute getUseMountsSkillEffectPer () {
		Mounts mounts = MountsManagerment.Instance.getMountsInUse ();
		if (mounts != null) {
			return mounts.getMountsSkillEffectPer ();
		}
		return new CardBaseAttribute ();
	}
	/// <summary>
	/// 获得正在出战的坐骑
	/// </summary>
	public Mounts getMountsInUse () {
		List<Mounts> allList = getAllMountsList ();
		if (allList==null || allList.Count == 0) {
			return null;
		}
		for (int i = 0; i < allList.Count; i++) {
			if (allList[i].isInUse ()) {
				return allList[i];
			}
		}
		return null;
	}
	/// <summary>
	/// 改变出战状态
	/// </summary>
	/// <param name="mounts">如果为空，则不管传什么，都全部设为不出战</param>
	/// <param name="isUse">如果设置 <c>true</c> 出战.</param>
	public void chagneUseType (Mounts mounts, bool isUse) {
		List<Mounts> allList = getAllMountsList ();
		if (mounts == null) {
			if (allList.Count == 0) {
				return;
			} else {
				for (int i = 0; i < allList.Count; i++) {
					if (allList[i].isInUse ()) {
						allList[i].setState (false);
					}
				}
			}
		} else {
			if (isUse) {
				for (int i = 0; i < allList.Count; i++) {
					if (allList[i].isInUse ()) {
						allList[i].setState (false);
					}
				}
			}
			mounts.setState (isUse);
		}
	}
	/// <summary>
	/// 拥有的坐骑数量
	/// </summary>
	public int getAllMountsCount () {
		return MountsManagerment.Instance.getAllMountsList ().Count;
	}
	/// <summary>
	/// 创建坐骑
	/// </summary>
	public Mounts createMounts () {
		return new Mounts ();
	}
	/// <summary>
	/// 创建坐骑
	/// </summary>
	public Mounts createMounts (int sid) {
		return createMounts("0",sid,0,Mounts.MOUNTS_DOWN_STATE,null,0);
	}
	/// <summary>
	/// 创建坐骑
	/// </summary>
	public Mounts createMounts (string uid, int sid, long exp, int state,Skill[] skills,int time) {
		return new Mounts (uid, sid, state, skills,time);
	}
	/// <summary>
	/// 创建坐骑
	/// </summary>
	public Mounts createMounts (ErlArray array) {
		Mounts mounts = new Mounts();
		mounts.bytesRead (0,array);
		return mounts;
	}
	/** 初始化经验条信息 */
	public LevelupInfo createLevelupInfo () {
		MountsManagerment manager=MountsManagerment.Instance;
		long oldExp = manager.getMountsExp ();
		LevelupInfo lvinfo = new LevelupInfo ();
		lvinfo.newExp = oldExp;
		lvinfo.newExpDown = manager.getEXPDown (expToLevel (oldExp));
		lvinfo.newExpUp = manager.getEXPUp (expToLevel (oldExp));
		lvinfo.newLevel = expToLevel (oldExp);
		lvinfo.oldExp = oldExp;
		lvinfo.oldExpDown = manager.getEXPDown (expToLevel (oldExp));
		lvinfo.oldExpUp = manager.getEXPUp (expToLevel (oldExp));
		lvinfo.oldLevel = expToLevel (oldExp);
		lvinfo.orgData = null;
		return lvinfo;
	}
	/** 经验对应的等级 */
	public int expToLevel (long _exp) {
		return EXPSampleManager.Instance.getLevel (EXPSampleManager.SID_MOUNTS_EXP, _exp);
	}
	/// <summary>
	/// 获得满级时的经验值
	/// </summary>
	public long getMaxExp () {
		return EXPSampleManager.Instance.getMaxExp (EXPSampleManager.SID_MOUNTS_EXP);
	}
	/// <summary>
	/// 获得当前等级经验值上限
	/// </summary>
	public long getEXPUp () {
		return getEXPUp (getMountsLevel ());
	}
	/// <summary>
	/// 获得当前等级经验值上限
	/// </summary>
	public long getEXPUp (int level) {
		return EXPSampleManager.Instance.getEXPUp (EXPSampleManager.SID_MOUNTS_EXP, level);
	}
	/// <summary>
	/// 获得当前等级经验下限
	/// </summary>
	public long getEXPDown () {
		return getEXPDown(getMountsLevel ());
	}
	/// <summary>
	/// 获取等级经验下标
	/// </summary>
	/// <param name="level">等级</param>
	public long getEXPDown (int level) {
		return EXPSampleManager.Instance.getEXPDown (EXPSampleManager.SID_MOUNTS_EXP, level);
	}
	/// <summary>
	/// 获得骑术等级
	/// </summary>
	public int getMountsLevel () {
		return EXPSampleManager.Instance.getLevel (EXPSampleManager.SID_MOUNTS_EXP, getMountsExp());
	}
	/// <summary>
	/// 根据样板获得对应等级技能
	/// </summary>
	/// <returns>实装后的技能.</returns>
	/// <param name="_skill">技能样板.</param>
	public Skill changeMineSkill (Skill _skill) {
		return new Skill(_skill.sid,getMountsExp (),_skill.getType ());
	}

	/* properties */
	/// <summary>
	/// 获得骑术经验
	/// </summary>
	public long getMountsExp () {
		return mountsExp;
	}
	/// <summary>
	/// 添加骑术经验
	/// </summary>
	public void addMountsExp (long _exp) {
		if(_exp<0) _exp=0;
		this.mountsExp += _exp;
	}
	/// <summary>
	/// 更新骑术经验
	/// </summary>
	public void updateMountsExp (long _exp) {
		this.mountsExp = _exp;
	}
}
