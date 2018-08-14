using UnityEngine;
using System.Collections;

/// <summary>
/// 公会活动展示窗口
/// </summary>
public class GuildAwardWindow : WindowBase
{
	public GameObject prefab_awardItem;
	public ContentLaddersAwards root_award;
	public UILabel label_myRank;

	public override void OnStart ()
	{
		base.OnStart ();
		prefab_awardItem.SetActive(false);
		if(!isAwakeformHide)
		{
			root_award.init();
		}
	}
	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI();
		label_myRank.text="[6E473D]" +  Language("laddersPrefix_05")+ "[C65843]" + LaddersManagement.Instance.currentPlayerRank.ToString()+"[-][-]";
	}
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		string btnName=gameObj.name;
		switch(btnName)
		{
		case "close":
			finishWindow();
			break;

		}
	}

}

