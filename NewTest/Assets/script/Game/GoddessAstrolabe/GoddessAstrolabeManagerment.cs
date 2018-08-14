using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 女神星盘管理器
 * @author 陈世惟
 * */
public class GoddessAstrolabeManagerment : SampleConfigManager
{

	private List<GoddessAstrolabeSample> infoByFront;//前台星盘配置
	private List<GoddessAstrolabeSample>[] infoBySid = new List<GoddessAstrolabeSample>[6];//各个星云的星星集合
	private GoddessAstrolabeInfo infoByServer;//后台激活信息

	public const int AWARD_ATTR = 1;//属性
	public const int AWARD_ADD = 2;//功能
	public const int AWARD_AWARD = 3;//奖励实物
	public const int AWARD_OPENSHOP = 4;// 开启星屑商店//
	
	public static GoddessAstrolabeManagerment Instance {
		get {
			return SingleManager.Instance.getObj("GoddessAstrolabeManagerment") as GoddessAstrolabeManagerment;
		}
	}
	
	public GoddessAstrolabeManagerment ()
	{
		base.readConfig (ConfigGlobal.CONFIG_GODDESSASTROLABE);
	}
	
	//解析配置
	public override void parseConfig (string str)
	{  
		GoddessAstrolabeSample be = new GoddessAstrolabeSample (str);
		if (infoByFront == null)
			infoByFront = new List<GoddessAstrolabeSample> ();
		infoByFront.Add (be);
	}

	/** 获得模板对象 */
	public GoddessAstrolabeSample getSampleBySid (int sid)
	{
		if (infoByFront == null) {
			return null;
		}
		for (int i = 0; i < infoByFront.Count; i++) {
			if (infoByFront [i].id == sid) {
				return infoByFront [i];
			}
		}
		return null;
	}

	/** 取得父星是否已激活 */
	public bool getFatherStarIsOpen (GoddessAstrolabeSample ga)
	{
		GoddessAstrolabeSample fa = getSampleBySid (ga.father);
		return fa == null ? true : fa.isOpen;
	}

	//取得所有前台配置
	public List<GoddessAstrolabeSample> getGoddessAstrolabeFrontInfo ()
	{
		return infoByFront;
	}

	public GoddessAstrolabeInfo getGoddessAstrolabeServerInfo ()
	{
		return infoByServer;
	}

	/** 取得指定星云中星星集合配置 */
	public List<GoddessAstrolabeSample> getStarByNebulaId (int sid)
	{
		if (sid <= 0 || (sid - 1) >= infoBySid.Length) {
			return null;
		}
		if (infoBySid[sid - 1] != null) {
			return infoBySid[sid - 1];
		}
        List<GoddessAstrolabeSample> newList = new List<GoddessAstrolabeSample>();
        if (getNebulaInfoById(sid) == null)
            return null;

        int[] stars = getNebulaInfoById(sid).stars;

        if (stars == null)
            return null;

        for (int i = 0; i < stars.Length; i++) {
            for (int j = 0; j < getGoddessAstrolabeFrontInfo().Count; j++) {
                if (getGoddessAstrolabeFrontInfo()[j].id == stars[i])
                    newList.Add(getGoddessAstrolabeFrontInfo()[j]);
            }
        }

        infoBySid[sid - 1] = newList;
        return newList;
	}

	/// <summary>
	/// 指定星云里面所有主星数目
	/// </summary>
	/// <param name="sid">Sid 1 - 5.</param>
	public int getMainStarNUmByNebulaId (int sid)
	{
		int num = 0;
		List<GoddessAstrolabeSample> info = getStarByNebulaId (sid);
		if (info == null)
			return num;
		for (int i = 0; i < info.Count; i++) {
			if (info [i].mainType == 1) {
				num++;
			}
		}
		return num;
	}

	/// <summary>
	/// 指定星云里激活的主星数目
	/// </summary>
	/// <param name="sid">Sid 1 - 5.</param>
	public int getOpenMainStarNumByNebulaId (int sid)
	{
		int num = 0;
		List<GoddessAstrolabeSample> info = getStarByNebulaId (sid);
		if (info == null)
			return num;
		for (int i = 0; i < info.Count; i++) {
			if (info [i].isOpen && info [i].mainType == 1) {
				num++;
			}
		}
		return num;
	}

	/// <summary>
	/// 指定星云里激活星星数目
	/// </summary>
	/// <param name="sid">Sid 1 - 5.</param>
	public int getOpenStarNumByNebulaId (int sid)
	{
		int num = 0;
		List<GoddessAstrolabeSample> info = getStarByNebulaId (sid);
		if (info == null)
			return num;
		for (int i = 0; i < info.Count; i++) {
			if (info [i].isOpen) {
				num++;
			}
		}
		return num;
	}
	/// <summary>
	/// 获得激活星星数量
	/// </summary>
	public int getAllStartNum(){
		int sum = 0;
		for (int i=0; i<6; i++)
			sum += getOpenStarNumByNebulaId (i + 1);
		return sum;
	}


	/** 指定星云里面是否存在可激活星星1-6 */
	public bool isHaveOpenStarByNebulaId (int sid)
	{
		List<GoddessAstrolabeSample> info = getStarByNebulaId (sid);
		if (info == null)
			return false;

		if (sid != 1) {
			List<GoddessAstrolabeSample> infoLast = getStarByNebulaId (sid - 1);
			if (infoLast == null)
				return false;
			FindGoddessAstrolabeSample findS = new FindGoddessAstrolabeSample();
			findS.id = getLastStarIdById (sid - 1);
			GoddessAstrolabeSample ga = infoLast.Find(findS.FindGoddessAstrolabeSampleByID);
			if (ga != null && ga.isOpen) {
				return true;
			} else {
				return false;
			}
		} else {
			for (int i = 0; i < info.Count; i++) {
				if (getFatherStarIsOpen (info [i])) {
					return true;
				}
			}
			return false;
		}
	}

	/** 是否存在可激活星星 */
	public bool isHaveStarCanOpen ()
	{
		if (infoByFront == null) {
			return false;
		}
		GoddessAstrolabeConditions[] conditions;

		for (int i = 0; i < infoByFront.Count; i++) {
			if (getFatherStarIsOpen (infoByFront [i])) {
				if (infoByFront [i].isOpen)
					continue;
				else {
					conditions = infoByFront [i].conditions;
					if (conditions != null && conditions.Length > 0) {
						bool canLight = true;//是否能点
						for (int j=0; j<conditions.Length; j++) {
							if (!getConBool (conditions [j])){
								canLight = false;
								break;
							}
						}
						if(canLight)
							return true;
					} else
						return true;
				}
			}
		}
		return false;
	}

	private bool getConBool (GoddessAstrolabeConditions gacinfo)
	{
		if (gacinfo == null) {
			return false;
		}
		if (gacinfo.type == PremiseType.LEVEL) {
			return UserManager.Instance.self.getUserLevel () >= StringKit.toInt (gacinfo.num);
		} else if (gacinfo.type == PremiseType.VIP_LEVEL) {
			return UserManager.Instance.self.getVipLevel () >= StringKit.toInt (gacinfo.num);
		} else if (gacinfo.type == PremiseType.FRIENDS_NUM) {
			return FriendsManagerment.Instance.getFriendAmount () >= StringKit.toInt (gacinfo.num);
		} else if (gacinfo.type == PremiseType.STAR) {
			return GoddessAstrolabeManagerment.Instance.getStarScore () >= StringKit.toInt (gacinfo.num);
		}else if(gacinfo.type==PremiseType.RMB){
			return UserManager.Instance.self.getRMB()>=StringKit.toInt(gacinfo.num);	
		}
		return false;
	}

	/** 取得指定星云最后一颗星*/
	public int getLastStarIdById (int id)
	{
		if (getNebulaInfoById (id) == null)
			return 0;
		return getNebulaInfoById (id).lastId;
	}

	/** 取得星云集合配置 */
	public GoddessAstrolabeStarArray getNebulaInfoById (int id)
	{
		return GoddessAstrolabeStarArrayManagerment.Instance.getNebulaBySid (id);
	}

	/** 取得激活的星星集合 */
	public int[] getOpenStars ()
	{
		return infoByServer.openStars;
	}

	/** 取得星尘碎片数量 */
	public int getStarScore ()
	{
		return infoByServer.star_score;
	}

	/** 增加星尘碎片数量 */
	public void setStarScore (int num)
	{
		infoByServer.star_score = num;
	}

	/** 取得前排属性加成 */
	public CardBaseAttribute getAttrByFrontInteger ()
	{
		return infoByServer.frontAddEffectInteger == null ? new CardBaseAttribute () : infoByServer.frontAddEffectInteger;
	}

	/** 取得前排属性百分比加成 */
	public CardBaseAttribute getAttrByFrontNumber ()
	{
		return infoByServer.frontAddEffectNumber == null ? new CardBaseAttribute () : infoByServer.frontAddEffectNumber;
	}

	/** 取得中排属性加成 */
	public CardBaseAttribute getAttrByMiddleInteger ()
	{
		return infoByServer.middleAddEffectInteger == null ? new CardBaseAttribute () : infoByServer.middleAddEffectInteger;
	}
	
	/** 取得中排属性百分比加成 */
	public CardBaseAttribute getAttrByMiddleNumber ()
	{
		return infoByServer.middleAddEffectNumber == null ? new CardBaseAttribute () : infoByServer.middleAddEffectNumber;
	}

	/** 取得后排属性加成 */
	public CardBaseAttribute getAttrByBehindInteger ()
	{
		return infoByServer.behindAddEffectInteger == null ? new CardBaseAttribute () : infoByServer.behindAddEffectInteger;
	}
	
	/** 取得后排属性百分比加成 */
	public CardBaseAttribute getAttrByBehindNumber ()
	{
		return infoByServer.behindAddEffectNumber == null ? new CardBaseAttribute () : infoByServer.behindAddEffectNumber;
	}

	/** 取得所有卡片属性加成 */
	public CardBaseAttribute getAttrByAllInteger ()
	{
		return infoByServer.allAddEffectInteger == null ? new CardBaseAttribute () : infoByServer.allAddEffectInteger;
	}
	
	/** 取得所有卡片属性百分比加成 */
	public CardBaseAttribute getAttrByAllNumber ()
	{
		return infoByServer.allAddEffectNumber == null ? new CardBaseAttribute () : infoByServer.allAddEffectNumber;
	}

	/** 取得增加好友上限 */
	public int getAddFriend ()
	{
		return infoByServer.addFriend;
	}

	/** 取得增加装备仓库上限 */
	public int getAddEquipStorage ()
	{
		return infoByServer.addEquipStorage;
	}

	/** 取得增加卡片仓库上限 */
	public int getAddCardStorage ()
	{
		return infoByServer.addCardStorage;
	}

	//处理后台信息
	public void initInfoByServer (ErlKVMessage message)
	{
		infoByServer = new GoddessAstrolabeInfo ();

		ErlType msgStar = message.getValue ("star_point") as ErlType;
		if (msgStar != null) {
			ErlArray arr = msgStar as ErlArray;
			infoByServer.openStars = new int[arr.Value.Length];
			for (int i=0; i <arr.Value.Length; i++) {
				infoByServer.openStars [i] = StringKit.toInt ((arr.Value [i] as ErlType).getValueString ());
			}
		}

		ErlType msgStarScore = message.getValue ("star_score") as ErlType;
		if (msgStarScore != null) {
			infoByServer.star_score = StringKit.toInt ((msgStarScore as ErlType).getValueString ());
		}

		ErlType msgAttr = message.getValue ("attr") as ErlType;
		if (msgAttr != null && msgAttr is ErlList) {
			ErlList arr = msgAttr as ErlList;
			ErlArray attrArray;
			string name;
			for (int i=0; i <arr.Value.Length; i++) {
				name = ((arr.Value [i] as ErlArray).Value [0] as ErlType).getValueString ();
				attrArray = (arr.Value [i] as ErlArray).Value [1] as ErlArray;
				if (name == "front") {
					CardBaseAttribute[] getAttr = getAttrByErlArray (attrArray);
					infoByServer.frontAddEffectInteger = getAttr [0];
					infoByServer.frontAddEffectNumber = getAttr [1];
				} else if (name == "middle") {
					CardBaseAttribute[] getAttr = getAttrByErlArray (attrArray);
					infoByServer.middleAddEffectInteger = getAttr [0];
					infoByServer.middleAddEffectNumber = getAttr [1];
				} else if (name == "behind") {
					CardBaseAttribute[] getAttr = getAttrByErlArray (attrArray);
					infoByServer.behindAddEffectInteger = getAttr [0];
					infoByServer.behindAddEffectNumber = getAttr [1];
				} else if (name == "all") {
					CardBaseAttribute[] getAttr = getAttrByErlArray (attrArray);
					infoByServer.allAddEffectInteger = getAttr [0];
					infoByServer.allAddEffectNumber = getAttr [1];
				} else if (name == "pve_attr") {
					infoByServer.addPveAttr = ((arr.Value [i] as ErlArray).Value [1] as ErlType).getValueString ();
				}
			}
		}

		ErlType msgFunc = message.getValue ("func") as ErlType;
		if (msgFunc != null && msgFunc is ErlList) {
			ErlList arr = msgFunc as ErlList;
			string name;
			int num;
			for (int i=0; i <arr.Value.Length; i++) {
				name = ((arr.Value [i] as ErlArray).Value [0] as ErlType).getValueString ();
				num = StringKit.toInt (((arr.Value [i] as ErlArray).Value [1] as ErlType).getValueString ());
				if (name == "friend") {
					infoByServer.addFriend = num;
				} else if (name == "equip_storage") {
					infoByServer.addEquipStorage = num;
				} else if (name == "card_storage") {
					infoByServer.addCardStorage = num;
				} else if (name == "max_pve") {
					infoByServer.addPveUse = num;
				}
			}
		}

		integrateOpenStar ();
	}

	public void openStarChange (int uid)
	{
//		MonoBase.print ("=========================111>>>>infoByServer.openStars.Length=" + infoByServer.openStars.Length);
		int num = infoByServer.openStars.Length;
		int[] newIn = new int[num + 1];
		int[] ar = infoByServer.openStars;
		newIn [0] = uid;
		System.Array.Copy (ar, 0, newIn, 1, num);
		infoByServer.openStars = newIn;

		integrateOpenStar ();
//		MonoBase.print ("=========================222>>>>infoByServer.openStars.Length=" + infoByServer.openStars.Length);
	}

	//后台推送更新
	public void updateInfoByServer (ErlKVMessage message)
	{
		ErlType msgStar = message.getValue ("star_point") as ErlType;
		if (msgStar != null) {
			ErlArray arr = msgStar as ErlArray;
			infoByServer.openStars = new int[arr.Value.Length];
			for (int i=0; i <arr.Value.Length; i++) {
				infoByServer.openStars [i] = StringKit.toInt ((arr.Value [i] as ErlType).getValueString ());
			}
		}
		
		ErlType msgStarScore = message.getValue ("star_score") as ErlType;
		if (msgStarScore != null) {
			infoByServer.star_score = StringKit.toInt ((msgStarScore as ErlType).getValueString ());
		}
		
		ErlType msgAttr = message.getValue ("attr") as ErlType;
		if (msgAttr != null && msgAttr is ErlList) {
			ErlList arr = msgAttr as ErlList;
			string name;
			ErlArray attrArray;
			for (int i=0; i <arr.Value.Length; i++) {
				name = ((arr.Value [i] as ErlArray).Value [0] as ErlType).getValueString ();
				attrArray = (arr.Value [i] as ErlArray).Value [1] as ErlArray;
				if (name == "front") {
					CardBaseAttribute[] getAttr = getAttrByErlArray (attrArray);
					infoByServer.frontAddEffectInteger = getAttr [0];
					infoByServer.frontAddEffectNumber = getAttr [1];
				} else if (name == "middle") {
					CardBaseAttribute[] getAttr = getAttrByErlArray (attrArray);
					infoByServer.middleAddEffectInteger = getAttr [0];
					infoByServer.middleAddEffectNumber = getAttr [1];
				} else if (name == "behind") {
					CardBaseAttribute[] getAttr = getAttrByErlArray (attrArray);
					infoByServer.behindAddEffectInteger = getAttr [0];
					infoByServer.behindAddEffectNumber = getAttr [1];
				} else if (name == "all") {
					CardBaseAttribute[] getAttr = getAttrByErlArray (attrArray);
					infoByServer.allAddEffectInteger = getAttr [0];
					infoByServer.allAddEffectNumber = getAttr [1];
				} else if (name == "pve_attr") {
					infoByServer.addPveAttr = ((arr.Value [i] as ErlArray).Value [1] as ErlType).getValueString ();
				}
			}
		}
		
		ErlType msgFunc = message.getValue ("func") as ErlType;
		if (msgFunc != null && msgFunc is ErlList) {
			ErlList arr = msgFunc as ErlList;
			string name;
			int num;
			int addNum = 0;
			for (int i=0; i <arr.Value.Length; i++) {
				name = ((arr.Value [i] as ErlArray).Value [0] as ErlType).getValueString ();
				num = StringKit.toInt (((arr.Value [i] as ErlArray).Value [1] as ErlType).getValueString ());
				addNum = 0;
				if (name == "friend") {
					addNum = num - infoByServer.addFriend;
					infoByServer.addFriend = num;
					FriendsManagerment.Instance.getFriends ().addMaxSize (addNum);
				} else if (name == "equip_storage") {
					addNum = num - infoByServer.addEquipStorage;
					infoByServer.addEquipStorage = num;
					StorageManagerment.Instance.updateEquipStorageMaxSpace (addNum);
				} else if (name == "card_storage") {
					addNum = num - infoByServer.addCardStorage;
					infoByServer.addCardStorage = num;
					StorageManagerment.Instance.updateRoleStorageMaxSpace (addNum);
				} else if (name == "max_pve") {
					addNum = num - infoByServer.addPveUse;
					infoByServer.addPveUse = num;
					UserManager.Instance.self.updatePvEPointMax (addNum);
				}
			}
		}
		
//		integrateOpenStar();
	}

	private CardBaseAttribute[] getAttrByErlArray (ErlArray _attr)
	{
		CardBaseAttribute attrArrayInteger = new CardBaseAttribute ();
		CardBaseAttribute attrArrayNumber = new CardBaseAttribute ();
		ErlArray arry;
		string type;
		string name;
		int attrNum;
		for (int i=0; i <_attr.Value.Length; i++) {
			//[[integer,magic],20] 
			arry = _attr.Value [i] as ErlArray;
			type = ((arry.Value [0] as ErlArray).Value [0] as ErlType).getValueString ();
			name = ((arry.Value [0] as ErlArray).Value [1] as ErlType).getValueString ();
			attrNum = StringKit.toInt ((arry.Value [1] as ErlType).getValueString ());

			if (type == "integer") {
				if (name == AttrChangeType.HP) {
					attrArrayInteger.hp += attrNum; 
				} else if (name == AttrChangeType.ATTACK) {
					attrArrayInteger.attack += attrNum;
				} else if (name == AttrChangeType.DEFENSE) {
					attrArrayInteger.defecse += attrNum;
				} else if (name == AttrChangeType.MAGIC) {
					attrArrayInteger.magic += attrNum;
				} else if (name == AttrChangeType.AGILE) {
					attrArrayInteger.agile += attrNum;
				}
			} else if (type == "number") {
				if (name == AttrChangeType.HP) {
					attrArrayNumber.perHp += attrNum;
				} else if (name == AttrChangeType.ATTACK) {
					attrArrayNumber.perAttack += attrNum;
				} else if (name == AttrChangeType.DEFENSE) {
					attrArrayNumber.perDefecse += attrNum;
				} else if (name == AttrChangeType.MAGIC) {
					attrArrayNumber.perMagic += attrNum;
				} else if (name == AttrChangeType.AGILE) {
					attrArrayNumber.perAgile += attrNum;
				}
			}
		}

		return new CardBaseAttribute[2]{attrArrayInteger,attrArrayNumber};
	}

	//更新星星激活状态
	private void integrateOpenStar ()
	{
		if (infoByFront != null && infoByServer.openStars != null) {
			int[] uids = infoByServer.openStars;
			for (int i = 0; i< infoByFront.Count; i++) {
				for (int j = 0; j< uids.Length; j++) {
					if (infoByFront [i].id == uids [j]) {
						infoByFront [i].isOpen = true;
					}
				}
			}
		}
	}

	//指定星星激活
	public void changeStarOpenById (int id)
	{
		for (int i = 0; i< infoByFront.Count; i++) {
			if (infoByFront [i].id == id) {
				infoByFront [i].isOpen = true;
			}
		}
	}
}
