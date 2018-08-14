using UnityEngine;
using System.Collections;

/// <summary>
/// 时间信息显示窗口
/// </summary>
public class TimeShowWindow : WindowBase {
	/**列表容器 */
	public MysticlShopTimeContent content;
	public UILabel nowTime;
	public UILabel buttonLabel;
	private Timer timer;
	protected override void begin ()
	{
		base.begin ();
		nowTime.text=TimeKit.timeTransform(ServerTimeKit.getCurrentSecond()*1000);
		content.reLoad(MysticalShopConfigManager.Instance.updateData);
		timer = TimerManager.Instance.getTimer (UserManager.TIMER_DELAY);
		timer.addOnTimer (refreshData);
		timer.start ();
		MaskWindow.UnlockUI ();
	}
	void Update ()
	{
		if (buttonLabel.gameObject.activeSelf) {
			buttonLabel.alpha = sin ();
		}
	}
	private void refreshData(){
		nowTime.text=TimeKit.timeTransform(ServerTimeKit.getCurrentSecond()*1000);
	}
	public override void buttonEventBase (GameObject gameObj)
	{ 
		base.buttonEventBase (gameObj);
		
		if (gameObj.name == "colseButotn") { 
			timer.stop();
			finishWindow();
		}
	}
}
