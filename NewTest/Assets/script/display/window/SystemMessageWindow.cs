using UnityEngine;
using System.Collections;

/**
 * 信息提示窗口
 * @author 汤琦
 * */
public class SystemMessageWindow :MessageWindow
{
	protected override void begin () {
		base.begin ();
		if (UiManager.Instance.getWindow<SystemMessageWindow> () != null && UiManager.Instance.getWindow<SystemMessageWindow> ().GetHashCode()!=this.GetHashCode()) {
			UiManager.Instance.getWindow<SystemMessageWindow> ().destoryWindow ();
		}
	}

	public new static void ShowAlert(string msg,CallBackMsg callback)
	{
		UiManager.Instance.openDialogWindow<SystemMessageWindow>((win)=>{
			win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, msg, callback);
		});
	}

	public new static void ShowConfirm(string msg,CallBackMsg callback)
	{
		UiManager.Instance.openDialogWindow<SystemMessageWindow>((win)=>{
			if(win.fatherWindow.name == "LotteryWindow")
			{
				UiManager.Instance.getWindow<LotteryWindow>().buyBtnCollider.enabled = false;
				UiManager.Instance.getWindow<LotteryWindow>().randomBtnCollider.enabled = false;
				UiManager.Instance.getWindow<LotteryWindow>().handBtnCollider.enabled = false;
			}
			win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("s0093"), msg, callback);
		});
	}

}
