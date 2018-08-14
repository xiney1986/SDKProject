using UnityEngine;
using System.Collections;

//装备选择容器

public class ContentEquipChoose : dynamicContent
{
	ArrayList equips;
	public const int INTENSIFY = 1;//强化装备
	public const int PUT_ON = 2;//穿装备
	public const int CHATSHOW = 3;//聊天展示
    public const int FROM_CHAT_FRIEND = 4;
	public const int FROM_TO_UPSTAR = 5;//装备升星
	private int intoType = 0;
	private int type = 0;
	public GameObject equipButtonPrefab;

	public void initInto (int type)
	{
		intoType = type;
	}
	
	public void Initialize (ArrayList _equips, int type)
	{
		equips = _equips;
		this.type = type;
		base.reLoad (equips.Count); 
	}
	
	public void reLoad (ArrayList _equips, int type )
	{

		this.type = type;
		equips = _equips;
		base.reLoad (equips.Count);
	}
	
	public override void updateItem (GameObject item, int index)
	{
		//base.updateItem (item, index);
		ButtonStoreEquip button = item.GetComponent<ButtonStoreEquip> ();
		button.UpdateEquip (equips [index] as Equip, type); 
	}

	public override void initButton (int  i)
	{
		if (nodeList [i] == null){
			nodeList [i] = NGUITools.AddChild (gameObject,equipButtonPrefab);
		}

		//nodeList [i].name = StringKit.intToFixString (i + 1);
		ButtonStoreEquip button = nodeList [i].GetComponent<ButtonStoreEquip> ();
	//	button.UpdateEquip (equips [i] as Equip, type);
		button.fatherWindow = fatherWindow; 
		button.intensifyButton.fatherWindow = fatherWindow;
	}
	
	//设置聊天窗口进入
	public void setChatWindowType (int _type)
	{
		this.type = _type;
	}
		
}

