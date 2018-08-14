using UnityEngine;
using System.Collections;

public class LevelupInfo   {
	/** 旧等级 */
	public int oldLevel;
	/** 旧经验 */
	public long oldExp;
	/** 旧经验上限 */
	public long oldExpUp;
	/** 旧经验下限 */
	public long oldExpDown;
	/** 新等级 */
	public int newLevel;
	/** 新经验 */
	public long newExp;
	/** 新经验上限 */
	public long newExpUp;
	/** 新经验下限 */
	public long newExpDown;
	/** 缓存实体 */
	public object orgData;
	/** 旧战斗力 */
	public int oldCardCombat;
	/** 新战斗力 */
	public int newCardCombat;
	
	public string ToString()
	{
		return "oldLevel="+oldLevel+",newLevel="+newLevel+",oldExp="+oldExp+",newExp="+newExp+",oldExpUp="+oldExpUp+",oldExpDown="+oldExpDown+",newExpUp="+newExpUp+",newExpDown="+newExpDown;
	}
}
