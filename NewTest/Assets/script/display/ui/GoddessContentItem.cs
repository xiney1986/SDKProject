using UnityEngine;
using System.Collections;

public class GoddessContentItem : ButtonBase {
	public UITexture headIcon;
	public UITexture emptyIcon;
	public UISprite qualityIcon;
	public GoddessRotation gr;
	public UILabel evolutionTimes;
	public UISprite evolutionIcon;
	public UILabel level;
	Quaternion rot ;
	public Card beast;
	void Start(){
		rot = transform.rotation;
	}
	void Update () {
		transform.rotation = rot;
	}

	public override void DoClickEvent () {
		base.DoClickEvent ();
		if (GuideManager.Instance.isEqualStep (16003000)) {
			GuideManager.Instance.doGuide ();
		}
		BeastEvolveManagerment.Instance.BeastName = beast.getName();
		UiManager.Instance.openWindow<BeastAttrWindow> ((win) => {
			win.Initialize (beast, BeastAttrWindow.RESONANCE);
		});
	}
	protected override void OnPress (bool isDown) {
		base.OnPress (isDown);
		gr.isRotate = !isDown;
	}

}
