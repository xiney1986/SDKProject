using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResolveCardChooseWindow : WindowBase {
	public ResolveCardContent content;
	public ResolveEquipContent equipContent;
    public ResolveMagicContent magicContent;
	public GameObject cardButtonPrefab;
	public GameObject equipButtonPrefab;
	public GameObject equipObj;
	public GameObject cardObj;
    public GameObject magicOjb;
	private string type;
	private ResolveWindow window;
	public void InitializeCard (ArrayList _cards, ResolveWindow _win) {
		window = _win;
		cardObj.SetActive (true);
		type = "card";
		content.Initialize (_cards, window, this);
		windowTitle = LanguageConfigManager.Instance.getLanguage ("resolve10");
		setTitle (LanguageConfigManager.Instance.getLanguage ("resolve10"));
	}
    public void InitializeMagic(ArrayList magicSc, ResolveWindow _win) {
        window = _win;
        magicOjb.SetActive(true);
        type = "magic";
        magicContent.Initialize(magicSc, window, this);
        windowTitle = LanguageConfigManager.Instance.getLanguage("resolve10l0");
        setTitle(LanguageConfigManager.Instance.getLanguage("resolve10l0"));
    }
	public void InitializeEquip(ArrayList _cards, ResolveWindow _win) {
		equipObj.SetActive (true);
		type = "equip";
		window = _win;
		equipContent.Initialize (_cards, window, this);
		windowTitle = LanguageConfigManager.Instance.getLanguage ("resolve15");
		setTitle (LanguageConfigManager.Instance.getLanguage ("resolve15"));
	}
	protected override void begin () {
		base.begin ();
		if (GuideManager.Instance.isEqualStep (114004000)) {
			GuideManager.Instance.doGuide ();
			GuideManager.Instance.guideEvent ();
		}
		MaskWindow.UnlockUI ();
	}
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "buttonConfirm" || gameObj.name == "close") {
			if (GuideManager.Instance.isEqualStep (114006000)) {
				GuideManager.Instance.doGuide ();
				GuideManager.Instance.guideEvent ();
			}
			if(type == "card"){
				window.updateCardContent();
			}else if(type == "equip"){
			window.updateEquipContent();
            } else if (type == "magic") {
                window.updateMagicContent();
            }
			finishWindow ();
		}
	}
}
