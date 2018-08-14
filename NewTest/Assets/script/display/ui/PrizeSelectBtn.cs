using UnityEngine;
using System.Collections;

public class PrizeSelectBtn : ButtonBase
{
	public GoodsView good;
	PrizesSelectWindow win;
	public GameObject select;
	public GameObject noSelect;

	public override void begin ()
	{
		base.begin ();

		win = this.fatherWindow as PrizesSelectWindow;
	}

	public override void DoClickEvent ()
	{
		if(win.old_prizeSelectBtn == this)
		{
			return;
		}
		showSelect();

		win.old_prizeSelectBtn = this;
	}

	public void showSelect()
	{
		select.SetActive(true);
		if(win.old_prizeSelectBtn != null)
		{
			win.old_prizeSelectBtn.select.SetActive(false);
		}
	}

}
