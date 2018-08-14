using UnityEngine;
using System.Collections;

public class TapButtonBase : ButtonBase
{
	//关联的父tap容器
	public  TapContentBase parentTapContent;
	public bool doEventButNoActive;//做事件但不激活样式
	[HideInInspector]
	public bool
		IsActiveTapPage = false;
//	[HideInInspector]
	public GameObject  tapWindowObj;

	public override void OnAwake () {
		textLabel.color = TapContentBase.normalLabelColor;
	}

	public virtual void setTapPageState ()
	{
		//空引用检测
		if (parentTapContent == null)
			parentTapContent = transform.parent.GetComponent<TapContentBase> ();
		if (parentTapContent == null)
			return;

		//关联窗口
		if (fatherWindow == null) 
			fatherWindow = parentTapContent.fatherwindow;

		if(isDisable()){
			return;
		}
		if(outLineColor==Colors.BACKGROUND_NONE) {
			outLineColor=textLabel.effectColor;
		}
		if (IsActiveTapPage == false) {
			setNormalSprite (parentTapContent.normalImageName);
			textLabel.color = TapContentBase.normalLabelColor;
			textLabel.effectColor = Colors.BUTTON_TEXT_OUTLINE_DISABLEL;
			UISprite uiSprite=ngui_buttonScript.tweenTarget.GetComponent<UISprite> ();
			uiSprite.width = (int)parentTapContent.normalSize.x;
			uiSprite.height = (int)parentTapContent.normalSize.y;
			uiSprite.color = TapContentBase.normalSpriteColor;
		} else {
			setNormalSprite (parentTapContent.activeImageName);
			textLabel.color = TapContentBase.activeLabelColor;
			textLabel.effectColor = outLineColor;
			UISprite uiSprite=ngui_buttonScript.tweenTarget.GetComponent<UISprite> ();
			uiSprite.color = TapContentBase.activeSpriteColor;
			uiSprite.width = (int)parentTapContent.activeSize.x;
			uiSprite.height = (int)parentTapContent.activeSize.y;

		}
	}

	void OnClick ()
	{


		//	base.OnClick ();
		if (parentTapContent == null)
			parentTapContent = transform.parent.GetComponent<TapContentBase> ();
		if (parentTapContent == null)
			return;

		try {
			parentTapContent.changeTapPage (this);  
		} catch (System.Exception ex) {
			if (GameManager.DEBUG) {
				throw ex;
			}
		}	
	}
}
