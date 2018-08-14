using UnityEngine;
using System.Collections;

public class Vip
{
	/** VIP等级 */
	public int vipLevel;
	/** VIP礼包名称 */
	public string vipAwardName;
	/** VIP礼包SID */
	public int vipAwardSid;
	/** VIP礼包中的奖品 */
	public PrizeSample[] prizes;//奖品
	/** VIP描述 */
	public string[] vipDescripts;
	/** VIP特权 */
	public vipPrivilege privilege;
	public Vip (string str)
	{
		parse (str);
	}

	private void parse (string str)
	{
		string[] strArr = str.Split ('|');
		//strArr[0] 
		vipLevel = StringKit.toInt (strArr [0]);
		//strArr[1] exp
		//	limitExp = StringKit.toInt (strArr [1]);
		//strArr[2] vipAwardName
		vipAwardName = strArr [1];
		//strArr[3] prizes
		vipAwardSid = StringKit.toInt (strArr [2]);
		parsePrizes (strArr [3]);
		parseDescript (strArr [4]);
		parsePrivilege (strArr [5]);
	}
	//读取特权数据
	private void parsePrivilege (string str)
	{
		string[] strArr = str.Split (',');
		if (strArr == null || strArr.Length <= 0)
			return;

		privilege = new vipPrivilege ();


		privilege.expAdd = StringKit.toInt (strArr [0]); 
		privilege.bossCountAdd = StringKit.toInt (strArr [1]); 
		privilege.bossCountBuyAdd = StringKit.toInt (strArr [2]); 
		privilege.cardStoreAdd = StringKit.toInt (strArr [3]); 
		privilege.equipStoreAdd = StringKit.toInt (strArr [4]); 
		privilege.friendAdd = StringKit.toInt (strArr [5]); 
		privilege.pveAdd = StringKit.toInt (strArr [6]); 
		privilege.pvePropUseCountAdd = StringKit.toInt (strArr [7]); 
		privilege.skillExpAdd = StringKit.toInt (strArr [8]); 
		privilege.unrealFreeDay = StringKit.toInt (strArr [9]); 
		privilege.alchemyFactor = StringKit.toInt (strArr [10]);
		privilege.alchemyAdd = StringKit.toInt (strArr [11]);
		privilege.laddersCountBuyAdd = StringKit.toInt (strArr [12]);
		privilege.areaCountBuyAdd = StringKit.toInt (strArr [13]);
		privilege.fubenResetTimes = StringKit.toInt (strArr[14]);
        privilege.ladderHelpTimes = StringKit.toInt(strArr[15]);
	}

	private void parsePrizes (string str)
	{
		string[] strArr = str.Split ('#');
		prizes = new PrizeSample[strArr.Length];
		for (int i = 0; i < strArr.Length; i++) {
			prizes [i] = new PrizeSample (strArr [i], ',');
		}
	}

	private void parseDescript (string str)
	{
		string[] strArr = str.Split ('#');
		vipDescripts = new string[strArr.Length];
		for (int i = 0; i < strArr.Length; i++) {
			vipDescripts [i] = strArr [i];
		}
	}	
}
public class VipReceivedComp : Comparator {
	
	public int compare(object o1,object o2) {
		if(!(o1 is Vip)||!(o2 is Vip)) return 0;
		Vip vip1=(Vip)o1;
		Vip vip2= (Vip)o2;
		bool isAward1 = VipManagerment.Instance.alreadyGetAward (vip1.vipAwardSid);
		bool isAward2 = VipManagerment.Instance.alreadyGetAward (vip2.vipAwardSid);
		if(isAward1&&!isAward2)
			return 1;
		if(!isAward1&&isAward2)
			return -1;
		return 0;
	}
}
//vip特权
public class vipPrivilege
{
	public int expAdd;//额外战斗经验
	public int bossCountAdd;//额外讨伐副本次数
	public int bossCountBuyAdd;//额外讨伐副本购买次数
	public int cardStoreAdd;//额外卡片仓库数
	public int equipStoreAdd;//额外装备仓库数
	public int friendAdd;//额外好友i仓库数
	public int pveAdd;//额外行动力
	public int pvePropUseCountAdd;//额外行动力药水使用次数
	public int skillExpAdd;//技能经验额外加成
	public int unrealFreeDay;//修炼没日免费
	public int alchemyFactor;//炼金减免系数
	public int alchemyAdd;//炼金暴击系数加成（万分）
	public int laddersCountBuyAdd;//天梯挑战额外次数
	public int areaCountBuyAdd;//竞技场额外可购买次数
	public int fubenResetTimes;//副本噩梦难度可重置次数
    public int ladderHelpTimes;//天梯好友助战购买次数
}
