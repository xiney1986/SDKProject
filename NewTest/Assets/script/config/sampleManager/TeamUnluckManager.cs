using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 替补解锁相关配置
/// </summary>
public class TeamUnluckManager : ConfigManager {

	/* static fields */
	private static TeamUnluckManager instance;

	/* static methods */
	public static TeamUnluckManager Instance {
		get {
			if (instance == null)
				instance = new TeamUnluckManager ();
			return instance;
		}
	}

	/* fields */
	/** 解锁对应的位置顺序 */
	private int[] indexs;
	/** 对应的解锁等级 */
	private int[] unluckLv;
	/** 对应解锁需要的人民币 */
	private int[] unluckRMB;
	/** 对应解锁需要的物品% */
	private int[] unluckProp;
	/** 对应解锁需要的物品数量 */
	private int[] unluckNum;

	/* methods */
	public TeamUnluckManager () {
		base.readConfig (ConfigGlobal.CONFUG_TEAM_UNLUCK);
	}
	public override void parseConfig (string str) {
		string[] strs = str.Split ('|');
		// str[0] 配置文件说明
		indexs=parseIns(strs[1]);
		unluckLv=parseIns(strs[2]);
		unluckRMB=parseIns(strs[3]);
		unluckProp=parseIns(strs[4]);
		unluckNum=parseIns(strs[5]);
	}

	private int[] parseIns (string str)
	{
		string[] strs = str.Split (',');
		int[] ss = new int[strs.Length];
		for (int i = 0; i < strs.Length; i++) {
			ss[i] =  StringKit.toInt (strs [i]);
		}
		return ss;
	}
	/// <summary>
	/// 返回需要的rmb
	/// </summary>
	public int[] getNeedRMB(){
		return unluckRMB;
	}
	/// <summary>
	/// 返回需要的等级
	/// </summary>
	public int[] getNeedLV(){
		return unluckLv;
	}
	/// <summary>
	/// 返回需要的物品
	/// </summary>
	public int[] getNeedProp(){
		return unluckProp;
	}
	/// <summary>
	/// 返回需要的数量
	/// </summary>
	public int[] getNeedNum(){
		return unluckNum;
	}
	/// <summary>
	/// 返回解锁顺序
	/// </summary>
	public int[] getindex(){
		return indexs;
	}
	/// <summary>
	/// 返回最小的需求等级
	/// </summary>
	/// <returns>The minimum lv.</returns>
	public int getMinLv(){
		int lv=unluckLv[0];
		for(int i=0;i<unluckLv.Length;i++){
			if(unluckLv[i]<lv)lv=unluckLv[i];
		}
		return lv;
	}
	/// <summary>
	/// 返回最近的一个解锁栏位条件是否达成
	/// </summary>
	/// <returns><c>true</c>, if condition success was gotten, <c>false</c> otherwise.</returns>
	public bool getConditionSuccess(List<int> list){
		for(int i=0;i<indexs.Length;i++){
			if(indexs[i]==list.Count+1){
				if(UserManager.Instance.self.getUserLevel()>=unluckLv[i]&&StorageManagerment.Instance.getProp(unluckProp[i])!=null&&StorageManagerment.Instance.getProp(unluckProp[i]).getNum()>=unluckNum[i]){
					return true;
				}
			}
		}
		return false;

	}
	public int getIndexx(int index){
		for(int i=0;i<indexs.Length;i++){
			if(indexs[i]==index){
				return i;
			}
		}
		return 3;
	}

}