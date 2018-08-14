using UnityEngine;
using System.Collections;

public class LinePowerShowWindow : WindowBase
{
	/* fields */
	/** Pve进度条 */
	public barCtrl pveBar;
	/** Pve描述 */
	public UILabel pveValue;
	/** Pve倒计时描述 */
	public UILabel pveTimeLabel;
	/** 坐骑Pve进度条 */
	public barCtrl mountsPveBar;
	/** 坐骑Pve描述 */
	public UILabel mountsPveValue;
	/** 坐骑Pve倒计时描述 */
	public UILabel mountsPveTimeLabel;
	/**关闭按钮 */
	public UILabel closeLabel;
	/** 计时器 */
	private Timer timer;

	/* methods */
	protected override void begin ()
	{
		base.begin ();
		timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY);
		refreshData();
		timer.addOnTimer (refreshData);
		timer.start ();
		MaskWindow.UnlockUI();
	}

	public override void DoDisable ()
	{
		base.DoDisable ();
		if (timer != null) {
			timer.stop ();
			timer = null;
		}
	}

	void refreshData ()
	{
		if (this == null || !gameObject.activeInHierarchy) {
			if (timer != null) {
				timer.stop ();
				timer = null;
			}
			return;
		}
		updatePve ();
		updateMountsPve ();
	}

	void updatePve ()
	{
		pveBar.updateValue (UserManager.Instance.self.getPvEPoint (), UserManager.Instance.self.getPvEPointMax ());
		pveValue.text = UserManager.Instance.self.getPvEPoint () + "/" + UserManager.Instance.self.getPvEPointMax ();
		if (UserManager.Instance.self.isPveMax ()) {
			pveTimeLabel.gameObject.SetActive (false);
		} else {
			pveTimeLabel.gameObject.SetActive (true);
			pveTimeLabel.text = UserManager.Instance.getNextPveTime ().Substring (3);
		}
		
	}

	void updateMountsPve ()
	{
		mountsPveBar.updateValue (UserManager.Instance.self.getStorePvEPoint (), UserManager.Instance.self.getStorePvEPointMax ());
		mountsPveValue.text = UserManager.Instance.self.getStorePvEPoint () + "/" + UserManager.Instance.self.getStorePvEPointMax ();
		if (!UserManager.Instance.self.isPveMax () || UserManager.Instance.self.isStorePveMax ()) {
			mountsPveTimeLabel.gameObject.SetActive (false);
		} else {
			mountsPveTimeLabel.gameObject.SetActive (true);
			mountsPveTimeLabel.text = UserManager.Instance.getNextMountsPveTime ().Substring (3);
		}
		
	}
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if(gameObj.name=="close") {
			finishWindow ();
		} 
	}
	/***/
	void Update () {
		UpdateCloseLable ();
	}
	private void UpdateCloseLable() {
		if(closeLabel.gameObject.activeSelf)
			closeLabel.alpha = sin ();
	}
}
