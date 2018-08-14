using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class lotteryInputWindow : WindowBase
{
	public GameObject okBtn;
	public UIInput numInput;
	public GameObject buttonClose;
	

	protected override void begin ()
	{
		base.begin ();

		numInput.defaultText = LanguageConfigManager.Instance.getLanguage("lottery_inputNum");
		MaskWindow.UnlockUI ();
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		//  确定按钮//
		if(gameObj == okBtn)
		{
			clickOkBtn();
		}
		else if(gameObj == buttonClose)
		{
			finishWindow();
		}
	}

	private void clickOkBtn()
	{
		if(numInput.label.text == null || numInput.label.text.Trim() == "")
		{
			UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
				win.Initialize (LanguageConfigManager.Instance.getLanguage ("lottery_numNull"));
			});
			return;
		}
		if(numInput.label.text != null && numInput.label.text.Trim() != "" && numInput.label.text.Trim().Length < 4)
		{
			UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
				win.Initialize (LanguageConfigManager.Instance.getLanguage ("lottery_numNotEnough"));
			});
			return;
		}
		if(numInput.label.text != null && numInput.label.text.Trim() != "" && !isNum(numInput.label.text.Trim()))
		{
			UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
				win.Initialize (LanguageConfigManager.Instance.getLanguage ("lottery_numError"));
			});
			return;
		}
		if(!string.IsNullOrEmpty(numInput.label.text))
		{
			UiManager.Instance.getWindow<LotteryWindow>().setCostLabel(UserManager.Instance.self.getVipLevel());
			LotteryManagement.Instance.selectNumList.Add(numInput.label.text);
			UiManager.Instance.getWindow<LotteryWindow>().creatSelectedNumList(numInput.label.text);
			LotteryManagement.Instance.currentDayBuyCount++;
			UiManager.Instance.getWindow<LotteryWindow>().canSelectCount.text = LotteryManagement.Instance.getLotteryCount().ToString();
			UiManager.Instance.getWindow<LotteryWindow>().updateBtnState();
		}

		finishWindow();
	}

	private bool isNum(string str)
	{
		Regex regex = new Regex("^[0-9]+$");
		return regex.IsMatch(str);
	}

}
