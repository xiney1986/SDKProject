using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 星魂对象管理器
/// </summary>
public class StarSoulManager {

	/* static fields */

	/* static methods */
	public static StarSoulManager Instance {
		get{return SingleManager.Instance.getObj("StarSoulManager") as StarSoulManager;}
	}

	/* fields */
	/** 星魂信息 */
	private StarSoulInfo starSoulInfo;
	/** 加锁状态字典<uid,加锁=true,解锁=false>(临时用)*/
	private static Dictionary<string,bool> lockStateDic;
	/** 转化经验状态字典<uid,转化=true,不转化=false>(临时用) */
	private static Dictionary<string,bool> changeExpStateDic;
	/** 当前操作时星魂激活的卡片(临时用) */
	private Card activeCard;
	/** 当前操作星魂时激活的卡片星魂槽的下标(临时用) */
	private int activeBoreIndex;
	/// <summary>
	/// 断线重连用(1:星魂替换 2：星魂装备)
	/// </summary>
	private int state=0;
	public string activeCard_uid;
	public string starSoul_uid;
	public int hole;
	//卸载是断线重连用
	public StarSoul soul;

	/* methods */
	public StarSoulManager () {
		lockStateDic = new Dictionary<string,bool> ();
		changeExpStateDic = new Dictionary<string,bool> ();
	}
	public void setState(int state)
	{
		this.state = state;
	}
	public int getState()
	{
		return state;
	}
	public void setActiveSoulStarInfo(string activeCard_uid,string starSoul_uid, int hole)
	{
		this.activeCard_uid = activeCard_uid;
		this.starSoul_uid = starSoul_uid;
		this.hole = hole;
	}
	/// <summary>
	/// 创建星魂对象
	/// </summary>
	public StarSoul createStarSoul () {
		return new StarSoul ();
	}
	/// <summary>
	/// 创建星魂对象
	/// </summary>
	/// <param name="sid"></param>
	public StarSoul createStarSoul (int sid) {
		return new StarSoul ("", sid, 0, 0);
	}
	/// <summary>
	/// 创建星魂对象
	/// </summary>
	/// <param name="sid"></param>
	public StarSoul createStarSoul (string uid,int sid) {
		return new StarSoul (uid, sid, 0, 0);
	}
	/// <summary>
	/// 创建星魂对象
	/// </summary>
	/// <param name="uid">Uid</param>
	/// <param name="sid">Sid</param>
	/// <param name="exp">经验</param>
	/// <param name="state">状态</param>
	public StarSoul createStarSoul (string uid, int sid, long exp, int state) {
		return new StarSoul (uid, sid, exp, state);
	}
	/// <summary>
	/// 创建星魂对象
	/// </summary>
	/// <param name="array">元组数据结构</param>
	public StarSoul createStarSoul (ErlArray array)
	{
		StarSoul starSoul=new StarSoul ();
		starSoul.bytesRead (0,array);
		return starSoul;
	}
	/// <summary>
	/// 创建星魂信息对象
	/// </summary>
	/// <param name="array">元组数据结构</param>
	public StarSoulInfo createStarSoulInfo (ErlArray array)
	{
		StarSoulInfo starSoulInfo=new StarSoulInfo ();
		starSoulInfo.bytesRead (0,array);
		return starSoulInfo;
	}
	/// <summary>
	/// 校验临时转换经验字典列表中是否存在>=指定品质的星魂
	/// </summary>
	/// <param name="qualityId">星魂品质</param>
	public bool isStarSoulQualityByChangeDic(int qualityId) {
		StarSoul starSoul;
		StorageManagerment manager=StorageManagerment.Instance;
		foreach (string key in changeExpStateDic.Keys) {
			bool isLock=changeExpStateDic [key];
			if(!isLock) continue;
			starSoul=manager.getStarSoul(key);
			if(starSoul==null) continue;
			if(starSoul.getQualityId()>=qualityId)
				return true;
		}
		return false;
	}
	/// <summary>
	/// 校验临时转换经验字典列表中是否存在指定state的星魂
	/// </summary>
	/// <param name="state">状态</param>
	public bool isStarSoulStateByChangeDic(int state) {
		StarSoul starSoul;
		StorageManagerment manager=StorageManagerment.Instance;
		foreach (string key in changeExpStateDic.Keys) {
			bool isLock=changeExpStateDic [key];
			if(!isLock) continue;
			starSoul=manager.getStarSoul(key);
			if(starSoul==null) continue;
			if(starSoul.checkState(state))
				return true;
		}
		return false;
	}
	/// <summary>
	/// 获取临时转换经验字典列表中的字符串数据
	/// </summary>
	public string getExchangeExpStateString() {
		StringBuilder sb = new StringBuilder();
		foreach (string key in changeExpStateDic.Keys) {
			bool isLock=changeExpStateDic [key];
			if(isLock)
				sb.Append(key+",");
		}
		string str=sb.ToString ();
		if(sb.Length>0)
			str=str.Substring(0, sb.Length-1);
		return str;
	}
	/// <summary>
	/// 获取临时锁定状态字典的字符串数据
	/// </summary>
	public string getLockStateString() {
		StringBuilder sb = new StringBuilder();
		string sign;
		StorageManagerment smanager = StorageManagerment.Instance;
		StarSoul starSoul;
		foreach (string key in lockStateDic.Keys) {
			starSoul=smanager.getStarSoul(key);
			if(starSoul==null) continue;
			bool isLock=lockStateDic [key];
			if(isLock&&starSoul.checkState(EquipStateType.LOCKED)) // 选择锁定,本身也是锁定状态不发送
				continue;
			if(!isLock&&!starSoul.checkState(EquipStateType.LOCKED)) // 非选择锁定,本身也是非锁定状态不发送
				continue;
			sb.Append(key+","+(isLock?1:0)+":");
		}
		string str=sb.ToString ();
		if(sb.Length>0)
			str=str.Substring(0, sb.Length-1);
		return str;
	}
	/// <summary>
	/// 获得指定加锁状态
	/// </summary>
	public void setLockState (string key,bool state) {
		if (!lockStateDic.ContainsKey (key))
			lockStateDic.Add (key, state);
		else
			lockStateDic [key] = state;
	}
	/// <summary>
	/// 校验是否存在指定key的加锁状态
	/// </summary>
	public bool checkLockState (string key) {
		if (!lockStateDic.ContainsKey (key))
			return false;
		return true;
	}
	/// <summary>
	/// 获得指定uid锁状态
	/// </summary>
	public bool getLockState (string key) {
		if (!lockStateDic.ContainsKey (key))
			return false;
		return lockStateDic [key];
	}
	/// <summary>
	/// 更新星魂锁状态
	/// </summary>
	public void updateStarSoulLockState() {
		StorageManagerment smanager = StorageManagerment.Instance;
		StarSoul starSoul;
		foreach (string key in lockStateDic.Keys) {
			starSoul=smanager.getStarSoul(key);
			if(starSoul==null) continue;
			bool isLock=lockStateDic [key];
			if(isLock) { // 加锁
				starSoul.setState(EquipStateType.LOCKED);
			} else { // 解锁
				starSoul.unState(EquipStateType.LOCKED);
			}
			starSoul.isNew=false;
		}
	}
	/// <summary>
	/// 得到飘字
	/// </summary>
	/// <returns>The star soul dese.</returns>
	/// <param name="starSoul">Star soul.</param>
	public string getStarSoulDese(StarSoul starSoul){
		if(starSoul!=null){
			StarSoulSample starSoulSamle=starSoul.getStarSoulSample();
			AttrChangeSample[] attrChangeSample=starSoulSamle.getAttrChangeSample ();
			return DescribeManagerment.getDescribe(starSoulSamle.desc,starSoul.getLevel(),attrChangeSample);
		}
		return null;
	}
	/// <summary>
	/// 得到活动的星魂的描述
	/// </summary>
	/// <returns>The active star soul dese.</returns>
	public string getActiveStarSoulDese()
	{
		Card card=getActiveCard();
		int index=getActiveBoreIndex();
		StarSoulBore starBore=card.getStarSoulBoreByIndex(index);
		if(starBore!=null){
			StarSoul ss=StorageManagerment.Instance.getStarSoul(starBore.getUid());
			if(ss!=null)return getStarSoulDese(ss);
		}
		return null;
	}
	/// <summary>
	/// 设置指定经验转化状态
	/// </summary>
	public void setChangeExpState (string key,bool state) {
		if (!changeExpStateDic.ContainsKey (key))
			changeExpStateDic.Add (key, state);
		else
			changeExpStateDic [key] = state;
	}
	/// <summary>
	/// 校验是否存在指定key的经验转化状态
	/// </summary>
	public bool checkChangeExpState (string key) {
		if (!changeExpStateDic.ContainsKey (key))
			return false;
		return true;
	}
	/// <summary>
	/// 获得指定uid经验转化状态
	/// </summary>
	public bool getChangeExpState (string key)
	{
		if (!changeExpStateDic.ContainsKey (key))
			return false;
		return changeExpStateDic [key];
	}
	/** 清理加锁状态字典 */
	public void clearLockStateDic () {
		lockStateDic.Clear ();
	}
	/** 清理经验转化状态字典 */
	public void clearChangeExpStateDic () {
		changeExpStateDic.Clear ();
	}
	/// <summary>
	/// 拿到指定队伍的卡片.
	/// </summary>
	/// <returns>The team card data.</returns>
	public List<Card> getTeamCardData(int teamNum) {
		List<Card> cardlist=new List<Card>();
		int index=1;
		//拿到激活的队伍
		if(teamNum==0){
			Army activeArmy=ArmyManager.Instance.getActiveArmy ();
			if(activeArmy==ArmyManager.Instance.getArmy(1)){
				index=1;
			}else if(activeArmy==ArmyManager.Instance.getArmy(2)){
				index=2;
			}else{
				index=3;
			}
		}else{
			index=teamNum;
		}
		if(index==ArmyManager.PVE_TEAMID||index==ArmyManager.PVP_TEAMID){
			string[] players=ArmyManager.Instance.getArmy(index).players;
			for(int i=0;i<players.Length;i++){
				Card c=StorageManagerment.Instance.getRole (players [i]);
				if(c!=null)cardlist.Add(c);
			}
			string[] alternates=ArmyManager.Instance.getArmy(index).alternate;
			for(int j=0;j<alternates.Length;j++){
				Card c=StorageManagerment.Instance.getRole (alternates [j]);
				if(c!=null)cardlist.Add(c);
			}
		}else{//拿到仓库中所有有星魂的卡片
			ArrayList ar= StorageManagerment.Instance.getAllRole ();
			ArrayList allPlayerSid=ArmyManager.Instance.getTeamCardUidList();
			foreach (Card each in ar) {
				if(each.getStarSoulByAll()!=null&&!allPlayerSid.Contains(each.uid)){
					cardlist.Add(each);
				}
			}
		}
		return cardlist;
	}
	/// <summary>
	/// 拿到特定卡片
	/// </summary>
	/// <returns>The team card data.</returns>
	public List<Card> getTeamCardData(int teamNum,Card card) {
		List<Card> cardlist=new List<Card>();
		cardlist.Add(card);
		return cardlist;
	}
	/// <summary>
	/// 得到卡片中所有星魂的类型 且分两种，一是装备不能装备已经有的类型,替换可以装备替换的类型 starSoul为替换的星魂,装备为null.
	/// </summary>
	public int[] getCardSoulExistType(Card card,StarSoul starSoul){
		List<int> typeArray = new List<int> ();
		StarSoul[] starSouls=card.getStarSoulByAll();
		if(starSouls==null)
			return typeArray.ToArray();
		for(int i=0;i<starSouls.Length;i++) {
			if(starSoul!=null&&starSouls[i].uid==starSoul.uid) continue;
			typeArray.Add(starSouls[i].getStarSoulType());
		}
		typeArray.Add(0);
		return typeArray.ToArray();
	}

	/* properties */
	/** 清理临时字典数据 */
	public void cleanDic() {
		clearLockStateDic ();
		clearChangeExpStateDic ();
	}
	/// <summary>
	/// 清理临时激活的卡片
	/// </summary>
	public void clearActiveCard() {
		activeCard = null;
		activeBoreIndex= -1 ;
	}
	/// <summary>
	/// 设置猎魂品质
	/// </summary>
	public void setHuntQuality(int huntQuality) {
		if (starSoulInfo == null)
			return;
		starSoulInfo.setHuntQuality(huntQuality);
	}
	/// <summary>
	/// 获取猎魂品质
	/// </summary>
	public int getHuntQuality() {
		if (starSoulInfo == null)
			return 0;
		return starSoulInfo.getHuntQuality ();
	}
	/// <summary>
	/// 获取存储的魂经验
	/// </summary>
	public long getStarSoulExp() {
		if (starSoulInfo == null)
			return 0;
		return starSoulInfo.getStarSoulExp ();
	}
	/// <summary>
	/// 添加星魂经验
	/// </summary>
	public void addStarSoulExp(long starSoulExp) {
		if (starSoulInfo == null)
			return;
		starSoulInfo.addStarSoulExp (starSoulExp);
	}
	/// <summary>
	/// 扣除星魂经验
	/// </summary>
	public bool delStarSoulExp(long starSoulExp) {
		if (starSoulInfo == null)
			return false;
		return starSoulInfo.delStarSoulExp (starSoulExp);
	}
	/// <summary>
	/// 获取星魂碎片数量
	/// </summary>
	public int getDebrisNumber() {
		if (starSoulInfo == null)
			return 0;
		return starSoulInfo.getDebrisNumber ();
	}
	/// <summary>
	/// 设置星魂碎片数量
	/// </summary>
	public void setDebrisNumber(int debrisNumber) {
		if (starSoulInfo == null)
			return;
		starSoulInfo.setDebrisNumber (debrisNumber);
	}
	/// <summary>
	/// 添加星魂碎片数量
	/// </summary>
	public void addDebrisNumber(int debrisNumber) {
		if (starSoulInfo == null)
			return;
		starSoulInfo.addDebrisNumber (debrisNumber);
	}
	/// <summary>
	/// 扣除星魂碎片数量
	/// </summary>
	public bool delDebrisNumber(int debrisNumber) {
		if (starSoulInfo == null)
			return false;
		return starSoulInfo.delDebrisNumber (debrisNumber);
	}
	/// <summary>
	/// 设置星魂信息对象
	/// </summary>
	/// <param name="starSoulInfo">Star soul info.</param>
	public void setStarSoulInfo(StarSoulInfo starSoulInfo){
		this.starSoulInfo = starSoulInfo;
	}
	/// <summary>
	/// 获得星魂信息对象
	/// </summary>
	public StarSoulInfo getStarSoulInfo() {
		return this.starSoulInfo;
	}
	/// <summary>
	/// 设置当前激活的卡片数据信息
	/// </summary>
	/// <param name="activeCard">当前激活的的卡片</param>
	/// <param name="activeBoreIndex">当前激活的卡片星魂槽的下标</param>
	public void setActiveInfo(Card activeCard,int activeBoreIndex){
		this.activeCard=activeCard;
		this.activeBoreIndex=activeBoreIndex;
	}
	/// <summary>
	/// 获得当前激活的的卡片(临时用)
	/// </summary>
	public Card getActiveCard(){
		return activeCard;
	}
	/// <summary>
	/// 获得当前激活的卡片星魂槽的下标(临时用)
	/// </summary>
	public int getActiveBoreIndex(){
		return this.activeBoreIndex;
	}
	/// 检查够等级开放不
	/// </summary>
	public bool checkBroeOpenLev(Card card,int index){
		if(UserManager.Instance.self.getUserLevel ()>=StarSoulConfigManager.Instance.getGrooveOpen()[index-1])return true ;
		return false;
	}

	public void setSoulStarState()
	{
		setSoulStarState(activeCard_uid,hole,starSoul_uid);
	}

	public void setSoulStarState(string cardUid,int hole,string starsoulUid)
	{		
		StorageManagerment smanager=StorageManagerment.Instance;
		Card card=smanager.getRole(cardUid);
		if(card!=null) {
			// 设置被替换的星魂状态为未装备(如果是直接穿装备则不执行)
			StarSoulBore oldStarSoulBore=card.getStarSoulBoreByIndex(hole);
			if(oldStarSoulBore!=null){
				StarSoul oldStarSoul=smanager.getStarSoul(oldStarSoulBore.getUid());
				if(oldStarSoul!=null) {
					oldStarSoul.unState(EquipStateType.OCCUPY);
					oldStarSoul.isNew=false;
				}
			}
			// 设置被穿的星魂状态为装备
			StarSoul starSoul=smanager.getStarSoul(starsoulUid);
			if(starSoul!=null) {
				starSoul.setState(EquipStateType.OCCUPY);
				starSoul.isNew=false;
			}
			card.addStarSoulBore(starsoulUid,hole);
			activeCard = card;
		}
		StorageManagerment.Instance.starSoulStorageVersion++;
	}
	string uid;
	int _hole;

	public void setInfo(string cardUid,int hole)
	{
		this.uid = cardUid;
		this._hole = hole;
	}
	public void delSoulStarState(CallBack callback)
	{
		if(uid!=""&&_hole!=0){
			delSoulStarState(uid,_hole);
			if(callback!=null)
				callback();
		}
			
	}
	public void delSoulStarState(string cardUid,int hole)
	{
		StorageManagerment smanager=StorageManagerment.Instance;
		Card card=smanager.getRole(cardUid);
		if(card!=null) {
			card.delStarSoulBoreByIndex(hole);
			activeCard = card;
		}
		StorageManagerment.Instance.starSoulStorageVersion++;
		uid = "";
		_hole =0;
	}

}