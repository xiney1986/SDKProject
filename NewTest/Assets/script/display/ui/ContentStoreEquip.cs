using UnityEngine;
using System.Collections;

/**
 * 装备仓库 装备节点显示
 * */
public class ContentStoreEquip : dynamicContent
{
	ArrayList equips;
	private int intoType = 0;
	public GameObject equipButtonPrefab;
	public const int INTENSIFY = 1;//强化装备
	public void initInto (int type)
	{
		intoType = type;
	}

	
	public void reLoad (ArrayList _equips, int type, Card card)
	{
		equips = _equips;
		 base.reLoad (equips.Count);
	}
	
	public override void updateItem (GameObject item, int index)
	{
		//base.updateItem (item, index);
		ButtonStoreEquip button = item.GetComponent<ButtonStoreEquip> ();

		button.UpdateEquip (equips [index] as Equip, INTENSIFY);
        if (index == 0 && GuideManager.Instance.getOnTypp() == 30) {
            button.intensifyButton.gameObject.SetActive(false);
        }
	}

	public override void initButton (int  i)
	{
		if (nodeList [i] == null){
			nodeList [i] = NGUITools.AddChild (gameObject,equipButtonPrefab);
		}

		nodeList [i].name = StringKit.intToFixString (i + 1);
		ButtonStoreEquip button = nodeList [i].GetComponent<ButtonStoreEquip> ();
	    
		//装备仓库中只能强化
		button.UpdateEquip (equips [i] as Equip, INTENSIFY);
        if (i == 0 && GuideManager.Instance.getOnTypp() == 30) {
            button.intensifyButton.gameObject.SetActive(false);
        }
		button.fatherWindow = fatherWindow; 
		button.intensifyButton.fatherWindow = fatherWindow;
	}		
}

