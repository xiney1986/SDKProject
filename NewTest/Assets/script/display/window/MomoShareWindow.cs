using UnityEngine;
using System.Collections;

public class MomoShareWindow : WindowBase {

	public UIInput inputText;

	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI();
		inputText.value = LanguageConfigManager.Instance.getLanguage("allAwardView01");
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);

		if (gameObj.name == "momoShareButton") {
			finishWindow ();
		} else if (gameObj.name == "close") {

			finishWindow ();

		}
	}
}
