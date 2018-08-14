using UnityEngine;
using System.Collections;

/** 
  * 战斗中的角色数据类[相当于战斗者Fighter]
  * @author 李程
  * */
public class CharacterData
{

	/** 角色显示控制器[显示层] */
	public CharacterCtrl characterCtrl;
	/** 角色实体对象 */
	public Card role;
	public int TeamPosIndex;
	/** 战斗角色在本次战斗中的唯一uid */
	public int serverID;
	/** 战斗角色在仓库中的唯一uid */
	public string storeID;
	/** 战斗最终的剩余hp */
	public int fixedServer_hp;
	/** 根据真实战斗不断改变的剩余hp */
	public int server_hp;
	/** 根据真实战斗不断改变的最大hp */
	public int server_max_hp;

	/// <summary>
	/// 阵营 1为自己 2为对手 
	/// </summary>
	public int camp;//TeamInfo.OWN_CAMP 
	public bool isNPC;
	public bool isBoss;
	//public CardLevelUpData levelUpData;
	public Vector3 orgPosition;
	public Vector3 orgScale;
	public BattleTeamManager parentTeam;
	public bool isInBattleField;
	public bool isGuardianForce = false;
	public int EvoLevel;//进化等级
}
