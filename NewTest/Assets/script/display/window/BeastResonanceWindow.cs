using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BeastResonanceWindow : WindowBase
{

	[HideInInspector]
	public List<BeastEvolve>
		beastList;
	public BeastResonanceItem[] beastItems;
	public UILabel labelSummomTitle;
	public UILabel labelSummomDesc;
	public UILabel labelEvolveTitle;
	public UILabel labelEvolveDesc;
	public UILabel labelAllTitle;
	public UILabel labelAllDesc;
	public const int EVOlveNUM = 4;//可进化次数

	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}

	protected override void DoEnable ()
	{
		//展示界面需要快些
		base.DoEnable ();
		showUI ();
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);

		if (gameObj.name == "close" || gameObj.name == "okButton") {
			UiManager.Instance.switchWindow<BeastAttrWindow>();
		} 
	}

	public void showUI ()
	{
		beastList = BeastEvolveManagerment.Instance.getAllBest (); 
		for (int i=0; i<beastItems.Length; i++) {
			beastItems [i].chooseItem = beastList [i];
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.NVSHENHEADPATH + beastList [i].getBeast ().getImageID () + "_head", beastItems [i].icoHead);
			if(i%2 == 1) {
				beastItems [i].icoHead.transform.rotation = new Quaternion(0,180,0,0);
			}

			beastItems [i].labelName.text = beastList [i].getBeast ().getName ();
			if (!beastList [i].isAllExist ()) {
				beastItems [i].labelLevel.text = LanguageConfigManager.Instance.getLanguage ("s0374");
				beastItems [i].GetComponent<Collider> ().enabled = true;
			} else {
				if (beastList [i].getBeast ().getEvoStarLevel () < beastList [i].getBeast ().getAllStarLevel ())
					beastItems [i].labelLevel.text = beastList [i].getBeast ().getEvoStarLevel () + "/" + beastList [i].getBeast ().getAllStarLevel ();
				else
					beastItems [i].labelLevel.text = LanguageConfigManager.Instance.getLanguage ("s0375");
				beastItems [i].GetComponent<Collider> ().enabled = false;
			}
		}
		int sum = BeastEvolveManagerment.Instance.getNumAdd ();
		int evo = BeastEvolveManagerment.Instance.getEvolveNumAdd ();
		labelSummomTitle.text = LanguageConfigManager.Instance.getLanguage ("s0370") + BeastEvolveManagerment.Instance.num + "/" + beastList.Count;
		labelSummomDesc.text = LanguageConfigManager.Instance.getLanguage ("s0373", sum.ToString ());
		labelEvolveTitle.text = LanguageConfigManager.Instance.getLanguage ("s0371") + BeastEvolveManagerment.Instance.evolveNum + "/" + beastList.Count * EVOlveNUM;
		labelEvolveDesc.text = LanguageConfigManager.Instance.getLanguage ("s0373", evo.ToString ());
		labelAllTitle.text = LanguageConfigManager.Instance.getLanguage ("s0372");
		labelAllDesc.text = LanguageConfigManager.Instance.getLanguage ("s0373", BeastEvolveManagerment.Instance.getBestResonance ().ToString ());

	}
}
