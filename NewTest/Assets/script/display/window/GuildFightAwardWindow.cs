using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 公会战奖励
/// </summary>
public class GuildFightAwardWindow : WindowBase {
	public GameObject itemPrefab;
	public DelegatedynamicContent content;
	public TapContentBase tabContent;
	List<GuildFightAwardSample> list;
	List<GuildFightAwardSample> currentList;
	private string type = "S";
	protected override void begin ()
	{
		base.begin ();
		if (!isAwakeformHide) {
			content.SetUpdateItemCallback (OnContentUpdateItem);
			content.SetinitCallback (initItem);
			list = GuildFightAwardSampleManager.Instance.getAllSample();
			currentList = new List<GuildFightAwardSample>();
			tabContent.changeTapPage(tabContent.getTapButtonByIndex(0));
			updateUI();

		}
		MaskWindow.UnlockUI ();
	}

	public void getCurrentList(string type){
		currentList.Clear ();
		foreach (GuildFightAwardSample s in list) {
			if(s.type == type){
				currentList.Add(s);
			}
		}
	}

	public void updateUI(){
		getCurrentList (type);
		if (currentList != null && currentList.Count > 0)
			content.reLoad (currentList.Count);
	}

	GameObject OnContentUpdateItem(GameObject item,int index)
	{
		if (item == null)
		{
			item = NGUITools.AddChild(content.gameObject,itemPrefab);
			item.SetActive(true);
		}		
		GuildFightAwardItem sc = item.GetComponent<GuildFightAwardItem>();
		sc.fatherWindow = this;
		sc.init(currentList[index]);
		return item;
	}
	GameObject initItem(int index)
	{		
		GameObject	item = NGUITools.AddChild(content.gameObject,itemPrefab);
		item.SetActive(true);		
		GuildFightAwardItem sc = item.GetComponent<GuildFightAwardItem>();
		sc.fatherWindow = this;
		sc.init(currentList[index]);
		return item;
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			this.finishWindow ();
		} 

	}

	public override void tapButtonEventBase (GameObject gameObj, bool enable)
	{
		base.tapButtonEventBase (gameObj, enable);
		if (gameObj.name == "ButtonS") {
			type = "S";
			updateUI();
		}
		else if (gameObj.name == "ButtonA") {
			type = "A";
			updateUI();
		}
		else if (gameObj.name == "ButtonB") {
			type = "B";
			updateUI();
		}
		else if (gameObj.name == "ButtonC") {
			type = "C";
			updateUI();
		}
	}
}
