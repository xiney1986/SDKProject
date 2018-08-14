using UnityEngine;
using System.Collections;

public class EblowView : ButtonBase {
	/** 骰子是否有加锁 */
	private bool isLock = false;
	/** 骰子的顶面,默认为心 */
	private string top = "score"; 
	/** 锁定的图标 */
	public UISprite lockSprite;
	/** 展示结果的ICON */
	public UISprite iconSprite;

	public void Init(string top){
		this.top = top;
		if (isLock) {
			lockSprite.spriteName = "img_lock1";
		} else {
			lockSprite.spriteName = "img_unlock";
		}
		updateIcon ();
	}

	public void setTop(string top){
		this.top = top;
		updateIcon ();
	}

	public void changeClockState(){
		if (isLock) {
			isLock = false;
			lockSprite.spriteName = "img_unlock";
		} else {
			isLock = true;
			lockSprite.spriteName = "img_lock1";
		}
	}


	/** 获取锁定状态 */
	public int getLockState()
	{
		if (isLock)
			return 1;
		else 
			return 0;
	}

	public void setLockState(bool isLock){
		this.isLock = isLock;
		if (isLock) {
			lockSprite.spriteName = "img_lock1";
		} else {
			lockSprite.spriteName = "img_unlock";
		}
	}

	//TODO  公会掷骰子替换图标
	public void updateIcon()
	{
		switch (top) {
		case "score":
			iconSprite.spriteName = "sezi_0";
			break;
		case "money":
			iconSprite.spriteName = "sezi_4";
			break;
		case "rmb":
			iconSprite.spriteName = "sezi_5";
			break;
		case "contribution":
			iconSprite.spriteName = "sezi_2";
			break;	
		case "honor":
			iconSprite.spriteName = "sezi_3";
			break;
		case "none":
			iconSprite.spriteName = "sezi_1";
			break;		
		}
	}

	
}
