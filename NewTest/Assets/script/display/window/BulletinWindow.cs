using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 宣传栏窗口
/// </summary>
public class BulletinWindow : WindowBase {

	/** Tap预制体 */
	public GameObject bulletinPerfab;
	/** Tap容器 */
	public UIGrid tapContent;
	/** 标题文本 */
	public UILabel titleShow;
	/** 内容文本 */
	public UILabel descShow;
	/** 进度条 */
	public UIScrollBar scrollBar;
	/** 确认按钮 */
	public ButtonBase enterButton;

	/** 临时储存创建过的按钮 */
	private BulletinButton[] buttonList;
	/** 当前展示宣传 */
	private BulletinButton showButton;
	/** 关闭窗口回调 */
	private CallBack callback;
	/** 容器 */
	private UIPanel panel;
	/** 容器当前位置 */
	private float nowX;

	protected override void begin () {
		base.begin ();
		panel = tapContent.GetComponent<UIPanel> ();
	}

	public void initWin (CallBack callback) {
		this.callback = callback;
	}

	protected override void DoEnable () {
		base.DoEnable ();
		if (GameManager.Instance.isFirstLoginOpenBulletin) {
			enterButton.textLabel.text = Language ("enterGame");
		} else {
			enterButton.textLabel.text = Language ("s0093");
		}
		GameManager.Instance.isFirstLoginOpenBulletin = false;
		initTap ();
	}

	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "buttonOk") {
			if (callback != null) {
				callback ();
				callback = null;
			} else {
				if (UserManager.Instance.self.canDivine && UserManager.Instance.self.getUserLevel () >= 7 && GuideManager.Instance.isGuideComplete ()) {
					EventDelegate.Add (OnHide,()=>{
						UiManager.Instance.openDialogWindow<DivineWindow> ();
					});
				}
				finishWindow ();
			}
		}
	}

	/// <summary>
	/// 初始化宣传列表
	/// </summary>
	void initTap () {
		List<Bulletin> bulletinList = BulletinManager.Instance.getButtletinList ();
		if (bulletinList == null) {
			MaskWindow.UnlockUI ();
			return;
		}
		if (buttonList == null) {
			buttonList = new BulletinButton[bulletinList.Count];
			for (int i = 0; i < bulletinList.Count; i++) {
				buttonList [i] = NGUITools.AddChild (tapContent.gameObject, bulletinPerfab).GetComponent<BulletinButton> ();
				buttonList [i].name = (1000 + i) + "";
				buttonList [i].initButton (this, bulletinList [i]);
				buttonList [i].gameObject.SetActive (true);
			}
		}
		tapContent.repositionNow = true;

		StartCoroutine (Utils.DelayRun (()=>{
			if (showButton == null && buttonList != null) {
				showButton = buttonList [0];
				showButton.DoClickEvent ();
			}
			MaskWindow.UnlockUI ();
		},0.3f));
	}

	/// <summary>
	/// 添加标题内容
	/// </summary>
	public void setBulletinTitle (string str) {
		titleShow.text = str.Replace ("~", "\n");
	}

	/// <summary>
	/// 添加展示内容
	/// </summary>
	public void setBulletinDesc (string str) {
		scrollBar.value = 0;
		descShow.text = "[A65644]" + str.Replace ("~", "\n");
		descShow.gameObject.SetActive (false);
		descShow.gameObject.SetActive (true);
	}

	/// <summary>
	/// 获得当前展示按钮
	/// </summary>
	public BulletinButton getShowButton () {
		return showButton;
	}

	/// <summary>
	/// 设置当前展示按钮
	/// </summary>
	public void setShowButton (BulletinButton _button) {
		if (showButton != null) {
			UIButton butotnTmp1 = showButton.GetComponent<UIButton> ();
			butotnTmp1.normalSprite = "buttonTap_Normal";
			showButton.spriteBg.spriteName = "buttonTap_Normal";
		}
		showButton = _button;
		UIButton butotnTmp2 = showButton.GetComponent<UIButton> ();
		butotnTmp2.normalSprite = "buttonTap_ClickOn";
		showButton.spriteBg.spriteName = "buttonTap_ClickOn";
		updatePos ();
	}
	/// <summary>
	/// 按钮居中
	/// </summary>
	void updatePos () {
		int buttonLength = buttonList.Length;
		if (showButton != null && buttonList != null && buttonLength >= 5) {
			if (showButton.getIndex () > 2 && showButton.getIndex () < (buttonLength - 1)) {
				SpringPanel.Begin (tapContent.gameObject, new Vector3 (-showButton.transform.localPosition.x, tapContent.transform.localPosition.y, tapContent.transform.localPosition.z), 9);
			}
			else if (showButton.getIndex () == 1 || showButton.getIndex () == 2) {
				GameObject tempObj = buttonList [2].gameObject;
				SpringPanel.Begin (tapContent.gameObject, new Vector3 (-tempObj.transform.localPosition.x + 95, tapContent.transform.localPosition.y, tapContent.transform.localPosition.z), 9);
			}
			else if (showButton.getIndex () == (buttonList.Length - 1) || showButton.getIndex () == buttonList.Length) {
				GameObject tempObj = buttonList [buttonList.Length - 3].gameObject;
				SpringPanel.Begin (tapContent.gameObject, new Vector3 (-tempObj.transform.localPosition.x - 100, tapContent.transform.localPosition.y, tapContent.transform.localPosition.z), 9);
			}
		}
		else if (showButton != null && buttonList != null && buttonLength == 4) {
			if (showButton.getIndex () == 1 || showButton.getIndex () == 2) {
				GameObject tempObj = buttonList [0].gameObject;
				SpringPanel.Begin (tapContent.gameObject, new Vector3 (30, tapContent.transform.localPosition.y, tapContent.transform.localPosition.z), 9);
			}
			else if (showButton.getIndex () == (buttonList.Length - 1) || showButton.getIndex () == buttonList.Length) {
				GameObject tempObj = buttonList [buttonList.Length - 1].gameObject;
				SpringPanel.Begin (tapContent.gameObject, new Vector3 (-30, tapContent.transform.localPosition.y, tapContent.transform.localPosition.z), 9);
			}
		}
	}
}
