using UnityEngine;
using System.Collections;

/// <summary>
/// 圣器强化的道具按钮
/// </summary>
public class ButtonHallows : ButtonBase
{
	[HideInInspector]
	public int propId;

	public UITexture propIcon;

	public override void DoClickEvent ()
	{
		Prop prop = PropManagerment.Instance.createProp (propId);
		if(prop!=null)
		{
			UiManager.Instance.openDialogWindow<PropAttrWindow>((win)=>{
				win.Initialize (prop);
			});
		}
	}
}

