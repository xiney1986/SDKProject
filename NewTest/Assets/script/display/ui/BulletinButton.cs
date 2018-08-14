using UnityEngine;
using System.Collections;

/// <summary>
/// 宣传栏按钮
/// </summary>
public class BulletinButton : ButtonBase {

	/** 标签 */
	public UISprite spriteTip;
	/** 类型 */
	public UISprite spriteType;
	/** 宣传标题 */
	public UILabel labelName;
	/** 父窗口 */
	private BulletinWindow fawin;
	private Bulletin bulletin;

	/// <summary>
	/// 初始化按钮
	/// </summary>
	public void initButton (BulletinWindow _win, Bulletin _bulletin) {
		this.fatherWindow = _win;
		this.bulletin = _bulletin;
		fawin = _win;
		showUI ();
	}

	void showUI () {
		//活动名称
		labelName.text = bulletin.name;
		//活动状态
		switch (bulletin.state) {
		case 1:
			spriteTip.gameObject.SetActive (true);
			spriteTip.spriteName = "Tip_New";
			break;
		case 2:
			spriteTip.gameObject.SetActive (true);
			spriteTip.spriteName = "Tip_Hot";
			break;
		default:
			spriteTip.gameObject.SetActive (false);
			break;
		}
		//活动类型
		switch (bulletin.type) {
		case 1:
			spriteType.spriteName = "Type_Notice";
			spriteType.gameObject.SetActive (true);
			break;
		case 2:
			spriteType.spriteName = "Type_Events";
			spriteType.gameObject.SetActive (true);
			break;
		case 3:
			spriteType.spriteName = "Type_Update";
			spriteType.gameObject.SetActive (true);
			break;
		default:
			spriteType.gameObject.SetActive (false);
			break;
		}
	}

	public override void DoClickEvent () {
		base.DoClickEvent ();
		fawin.setBulletinTitle (bulletin.title);
		fawin.setBulletinDesc (bulletin.desc);
		fawin.setShowButton (this);
		MaskWindow.UnlockUI ();
	}

	/// <summary>
	/// 获得对应序号
	/// </summary>
	public int getIndex () {
		return StringKit.toInt (this.name) - 1000 + 1;
	}
}
