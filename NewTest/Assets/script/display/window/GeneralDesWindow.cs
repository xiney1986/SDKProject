using UnityEngine;
using System.Collections;

public class GeneralDesWindow : WindowBase
{
	public ButtonBase screenButton;
	public UILabel    buttonLabel;
	public UILabel    desTitle;
	public UILabel    desContent;
	public UILabel    subTitleLabel;
	public UISprite   background;
	public UISprite   background2;
	private float time;
	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "screenButton") {
			finishWindow ();
		}
	}
	void Update ()
	{
		if (buttonLabel.gameObject.activeSelf) {
			float offset = Mathf.Sin (time * 6); 
			buttonLabel.alpha = sin ();
		}
	}
	///<summary>
	/// 初始化内容
	/// </summary>
	public void initialize(string content, string title, string subTitle){
		desContent.text = content;
		desTitle.text = title;
		subTitleLabel.text = subTitle;
		int count = (int)desContent.localSize.y / 34 - 1;
		if (count > 0) {
			desTitle.transform.localPosition = new Vector3(desTitle.transform.localPosition.x,desTitle.transform.localPosition.y + 17f * (float)count , desTitle.transform.localPosition.z);
			subTitleLabel.transform.localPosition = new Vector3(subTitleLabel.transform.localPosition.x,subTitleLabel.transform.localPosition.y + 17f * (float)count , subTitleLabel.transform.localPosition.z);
			desContent.transform.localPosition = new Vector3(desContent.transform.localPosition.x,desContent.transform.localPosition.y + 17f * (float)count , desContent.transform.localPosition.z);
			buttonLabel.transform.localPosition = new Vector3(buttonLabel.transform.localPosition.x,buttonLabel.transform.localPosition.y - 17f * (float)count , buttonLabel.transform.localPosition.z);
			background.height += 34 * count;
			background2.height += 34 * count;
		}
	}
}
