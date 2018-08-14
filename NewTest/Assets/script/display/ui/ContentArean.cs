using UnityEngine;
using System.Collections;

public class ContentArean : dynamicContent
{
	ArenaActivityInfo[] arenaList;
	public Texture[] backgroundTextures;

	public void	reLoad ()
	{
		arenaList = FuBenManagerment.Instance.getArenaActivityArray ();
		if (arenaList == null || arenaList.Length <= 0)
			return; 
		base.reLoad (arenaList.Length);
	}
	
	public override void  updateItem (GameObject item, int index)
	{
		ButtonArenaItem button = item.GetComponent<ButtonArenaItem> ();		 
		ArenaActivityInfo info=arenaList[index];
		switch(info.type)
		{
		case EnumArena.arena:
			button.updateActive (info, backgroundTextures [0]);
			break;
		case EnumArena.ladders:
			button.updateActive (info, backgroundTextures [1]);
			break;
		case EnumArena.mineral:
			button.updateActive (info, backgroundTextures [2]);
			break;
		}

	}
	
	public override void initButton (int  i)
	{
		if (nodeList [i] == null) {
			nodeList [i] = NGUITools.AddChild (gameObject, (fatherWindow as ArenaNavigateWindow).arenaButtonPrefab);
		}
		nodeList [i].SetActive (true);
		ButtonArenaItem button = nodeList [i].GetComponent<ButtonArenaItem> ();
		button.fatherWindow = fatherWindow;
	}
	public override void jumpToPage (int index)
	{
		base.jumpToPage(index);
		if (GuideManager.Instance.isEqualStep(133003000)||GuideManager.Instance.isEqualStep(121003000)) {
			GuideManager.Instance.guideEvent ();
		}
        GuideManager.Instance.doFriendlyGuideEvent();
	}
}

