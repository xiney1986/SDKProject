using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IncreaseBeastEvolutionWindow : WindowBase {

	public IncreaseBeastEvolutionItem[] showButtons;
	public UITexture[] showTextures;
	private List<BeastEvolve> list;

	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}
	
	protected override void DoEnable ()
	{
		base.DoEnable ();
		initLabel();
	}

	public void initWin(List<BeastEvolve> _list)
	{
		list = _list;
		showLabel();
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);

		switch(gameObj.name)
		{
		case "button_close":
			destoryWindow();
			break;
		}
	}

	private void showLabel()
	{
		if(list == null || list.Count <= 0)
			return;

		//最多显示6个.
		int min=Mathf.Min(list.Count,6);


		for(int i = 0;i<min;i++)
		{
			showButtons[i].gameObject.SetActive (true);
			showButtons[i].textLabel.text = list[i].getBeast().getName();
			showButtons[i].initButton(list[i]);
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.NVSHENHEADPATH + list[i].getBeast().getImageID () + "_head", showTextures[i]);
		}
	}

	private void initLabel()
	{
		for(int i = 0;i<showButtons.Length;i++)
		{
			showButtons[i].gameObject.SetActive (false);
		}
	}
}
