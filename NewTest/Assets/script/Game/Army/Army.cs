using System;
using System.Collections.Generic;

/**
 * 队伍 
 * @author 张海山
 * */
public class Army
{

	public int armyid;//队伍id
	public int formationID;//阵型id
	public string beastid;//召唤兽id
	public string[] players;//队员 
	public string[] alternate;//队员对应的替补
	public int state;//是否正在使用

	public Army ()
	{
		
	}
  
	public Army (int armyid, int arrayid, string beastid, string[] team, string[] alternate, int state)
	{
		this.armyid = armyid;
		this.formationID = arrayid; 
		this.beastid = beastid;
		this.players = team;
		this.alternate = alternate; 
		this.state = state;
	}

	public void ResetPlayerRole(){
		for(int i=0;i<this.players.Length ;i++){
			players[i] = "0";
		}
	}
	public void ResetAlternate(){
		for(int i=0;i<this.alternate.Length ;i++){
			alternate[i] = "0";
		}
	}

	public void AlternateTransformPlayer(){
		for(int i=0;i<this.players.Length ;i++){
			players[i] =alternate[i];
		}
        ResetAlternate();
	}
	/// <summary>
	/// 获取队伍所有卡片（主力和替补都算上）战斗力
	/// </summary>
	public int getAllCombat()
	{
		return CombatManager.Instance.getTeam_AllCombat(this);
	}
	/// <summary>
	/// 获取队伍主力战斗力
	/// </summary>
	public int getMainCombat()
	{
		return CombatManager.Instance.getTeam_MainCombat(this);
	}
	/// <summary>
	/// 获取队伍替补战斗力
	/// </summary>
	/// <returns>The sub combat.</returns>
	public int getSubCombat()
	{
        return CombatManager.Instance.getTeam_SubstituteCombat(this);
	}


	public List<Card> getPlayersByCard()
	{
		return getCardsFromStrings(players);
	}

	public List<Card> getAlternateByCard()
	{
		return getCardsFromStrings(alternate);
	}

	public List<Card> getCardList()
	{
		List<Card> newList = new List<Card>();
		newList = getPlayersByCard();
		ListKit.AddRange (newList, getAlternateByCard ());
		return newList == null ? null : newList;
	}

	//jordenwu 
	//判断队伍是否已经有此卡
	public bool IsHaveCard(Card aCard){
		if(aCard==null)return false;
		if(players.Length<=0)return false;
		if(alternate.Length<=0)return false;
		string cName=aCard.getName();
		bool retValue=false;
		for(int i=0;i<players.Length;i++){
			string temp=players[i];
			if(temp.EndsWith(cName)){
				retValue=true;
				break;
			}
		}
		//
		for(int j=0;j<alternate.Length;j++){
			string temp=players[j];
			if(temp.EndsWith(cName)){
				retValue=true;
				break;
			}
		}
		return retValue;
	}
	//
	public bool IsHaveSameSIDCard(Card aCard){
		//比较sid
		List<Card> clist=getCardList();
		if(clist==null||clist.Count<=0)return false;
		bool retValue=false;
		for(int i=0;i<clist.Count;i++){
			Card cc=clist[i];
			if(cc.sid==aCard.sid){
				return true;
			}
		}
	    int[] sids = CardSampleManager.Instance.getRoleSampleBySid(aCard.sid).sameCardSids;
	    if (sids!=null)
	    {
	        for (int j=0;j<sids.Length;j++)
	        {
                for (int i = 0; i < clist.Count; i++)
                {
                    Card lk = clist[i];
                    if (lk.sid == sids[j]) return true;
                }
	        }
	    }
		return false;
	}
	//

	public Card getBeast()
	{
		return StorageManagerment.Instance.getBeast(beastid);
	}

	/// <summary>
	/// 通过队伍卡片UID获得阵型中的位置1-15
	/// </summary>
	public int getLoctionByCardUid(string _uid)
	{
		for (int i=0; i<players.Length; i++) {
			if(players[i] == _uid) {
				return getLoctionByIndex(i);
			}
		}
		return 0;
	}

	/// <summary>
	/// 通过队伍卡片UID获得阵型中的前中后排1-2-3
	/// </summary>
	public int getFormationLoctionByCardUid(string _uid)
	{
		if(getLoctionByCardUid(_uid) < 6) {
			return 1;
		} else if(getLoctionByCardUid(_uid) >= 6 && getLoctionByCardUid(_uid) < 11) {
			return 2;
		} else {
			return 3;
		}
	}

	private List<Card> getCardsFromStrings(string[] str)
	{
		List<Card> cards = new List<Card>();
		for (int i=0; i<str.Length; i++) {
			if(str[i]!="0")
				cards.Add(StorageManagerment.Instance.getRole(str[i]));
		}
		return cards;
	}
	
	public string getPlayersToString (string[] array)
	{
		if (array == null || array.Length < 1)
			return "";
		//对队伍信息排序 按照站位顺序排序
		string[] arr = new string[array.Length];
		Array.Copy (array, arr, array.Length); 
		string str = "";
		for (int i=0; i<arr.Length; i++) {
			if (i == arr.Length - 1)
				str += arr [i];
			else
				str += arr [i] + ",";
		}
		return str;
	}

	/// <summary>
	/// 获取队伍内战斗力最低的
	/// </summary>
	public Card getLeastCombatCard () {
		Card leastCombatCard = null;
		for (int i = 0; i < players.Length; i++) {
			if (players[i] != "0") {
				Card card = StorageManagerment.Instance.getRole (players[i]);
				if (leastCombatCard == null || card.CardCombat < leastCombatCard.CardCombat) {
					leastCombatCard = card;
				}
			}
		}
		for (int i = 0; i < alternate.Length; i++) {
			if (alternate[i] != "0") {
				Card card = StorageManagerment.Instance.getRole (alternate[i]);
				if (leastCombatCard == null || card.CardCombat < leastCombatCard.CardCombat) {
					leastCombatCard = card;
				}
			}
		}
		return leastCombatCard;
	}
	/// <summary>
	/// 获取队伍内战斗力最低的
	/// </summary>
	public Card getLeastCombatCardExistMain () {
		Card leastCombatCard = null;
		for (int i = 0; i < players.Length; i++) {
			if (players[i] != "0") {
				Card card = StorageManagerment.Instance.getRole (players[i]);
				if(card.uid!=UserManager.Instance.self.mainCardUid){
					if (leastCombatCard == null || card.CardCombat < leastCombatCard.getCardCombat()) {
						leastCombatCard = card;
					}
				}
			}
		}
		for (int i = 0; i < alternate.Length; i++) {
			if (alternate[i] != "0") {
				Card card = StorageManagerment.Instance.getRole (alternate[i]);
				if (leastCombatCard == null || card.CardCombat < leastCombatCard.getCardCombat()) {
					leastCombatCard = card;
				}
			}
		}
		return leastCombatCard;
	}
	
	public void setTeam (string[] team)
	{
		this.players = team;
	}

	public int getLength ()
	{
		FormationSample sample = FormationSampleManager.Instance.getFormationSampleBySid (formationID);
		return sample.getLength ();
	}
	
	public string[] getAlternate ()
	{
		return this.alternate;
	}
	
	public int getLoctionByIndex (int index)
	{
		return FormationManagerment.Instance.getLoctionByIndex (formationID, index); 
	}
	
	//获得队员数量
	public int getPlayerNum ()
	{
		int num = 0;
		for (int i = 0; i < players.Length; i++) {
			if (String.IsNullOrEmpty(players [i])==false && players [i]!="0")
				num++;
		}
		return num;
	}
	//获得替补队员数量
	public int getAlternateNum ()
	{
		int num = 0;
		for (int i = 0; i < alternate.Length; i++) {
			if (!String.IsNullOrEmpty(alternate[i])&&alternate[i]!="0")
				num++;
		}
		return num;
	}
	/// <summary>
	/// 是否有出战女神
	/// </summary>
	public bool isFightBeast()
	{
		if (String.IsNullOrEmpty (beastid) || beastid=="0")
			return false;
		return true;
	}
}
