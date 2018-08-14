using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ServerCardMsg {
	
	public ServerCardMsg(Card _card,List<Equip> _showEquips,int _showCombat)
	{
		this.card = _card;
		this.showEquips = _showEquips;
		this.showCombat = _showCombat;
	}
	
	public Card card;//卡片
	public List<Equip> showEquips;//卡片包含的装备集合
	public int showCombat; //此处的卡片无需计算战力,直接显示由服务器提供的数据,1使用后台数据
}