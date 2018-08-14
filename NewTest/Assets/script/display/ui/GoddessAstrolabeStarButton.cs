using UnityEngine;
using System.Collections;

public class GoddessAstrolabeStarButton : ButtonBase {

	private GoddessAstrolabeSample info;
	public GameObject arrow;//箭头


	/// <summary>
	/// 开关箭头
	/// </summary>
	public void setArrow(bool isShow)
	{
		arrow.SetActive(isShow);
	}

	public void init(GoddessAstrolabeSample _info)
	{
		info = _info;
	}

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		if(info == null)
			return;
		GuideManager.Instance.doGuide();
		UiManager.Instance.openDialogWindow<GoddessAstrolabeStarInfoWindow>((win)=>{
			win.initUI((fatherWindow as GoddessAstrolabeWindow), info);
		});
	}


}
