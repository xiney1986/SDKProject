using UnityEngine;
using System.Collections;

public class SoulEquiShow : MonoBase {
	public RoleView roleview;
	//public SoulInfoButtonItem[] soulinfoButton;
	//public StarSoulWindow fatherwin;
	public WindowBase fatherwin;
	public Card currectCard;

	//public 
	void Start()
	{
	}
	public void initInfo(Card card,WindowBase win)
	{
		currectCard=card;
		//fatherwin=win as StarSoulWindow;
		if(win is StarSoulWindow)
		{
	    	fatherwin = win as StarSoulWindow;
		}	
		else if (win is SoulHuntWindow)
		{
			fatherwin = win as SoulHuntWindow;
		}
		updataIcon(card);
	}
	private void updataIcon(Card card)
	{
		roleview.init(card,fatherwin,null);
	}
}
