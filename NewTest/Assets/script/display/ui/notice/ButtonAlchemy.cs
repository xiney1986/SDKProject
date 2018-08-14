
using System;
using UnityEngine;

public class ButtonAlchemy:ButtonBase
{
	[HideInInspector]
	public AlchemyContent
		content;
	private int lastGetMoney;

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		if(gameObject.name=="continue"){
			//未勾选不提示
			if (!NoticeManagerment.Instance.alchemyNeverTip && content.getConsume () > 0) {
				UiManager.Instance.openDialogWindow<AlchemyBuyTipWindow> ((win) => {
					win.callback = alchemy;
					win.content = content;
					win.initCost();
				});
			} else {
				alchemy ();
			}
		}else if(gameObject.name=="continueten"){
			UiManager.Instance.openDialogWindow<MoreAlchemyWindow>((win)=>{
				win.callback=alchemyAll;
				win.updateUI();
			});
		}
	}
	private void alchemyAll(bool bo){
		if(!bo){
			MessageWindow.ShowRecharge(LanguageConfigManager.Instance.getLanguage ("s0158"));
			return;
		}else{
			NoticeAlchemyFPort port = FPortManager.Instance.getFPort ("NoticeAlchemyFPort") as NoticeAlchemyFPort;
			port.access ((long num,int numm) => {
				if (num != -1) {
					content.refreshInfo ();
					UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("AlchemyContent08ll",num.ToString(), numm.ToString()));
					content.alchemyEffect.SetActive(false);
					content.bombEffect.SetActive (true);
					content.bombEffect.particleSystem.Play ();
					AudioManager.Instance.PlayAudio (147);
					StartCoroutine (Utils.DelayRun (() => {
						content.alchemyEffect.SetActive(true);
						content.bombEffect.SetActive(false);
                        content.refreshInfo();
					}, 0.75f));
				}
			},1);
		}
		
	}
	private void alchemy ()
	{
		if (content.getConsume () > UserManager.Instance.self.getRMB ()) {
			MessageWindow.ShowRecharge(LanguageConfigManager.Instance.getLanguage ("s0158"));
			return;
		}
		lastGetMoney = NoticeManagerment.Instance.getAlchemyMoney ();
		NoticeAlchemyFPort port = FPortManager.Instance.getFPort ("NoticeAlchemyFPort") as NoticeAlchemyFPort;
		port.access ((long num,int index) => {
			if (num == 1) {
				content.refreshInfo ();
				UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("AlchemyContent07",lastGetMoney.ToString()));
				content.alchemyEffect.SetActive(false);
				content.bombEffect.SetActive (true);
				content.bombEffect.particleSystem.Play ();

				AudioManager.Instance.PlayAudio (147);
				StartCoroutine (Utils.DelayRun (() => {
					content.alchemyEffect.SetActive(true);
					content.bombEffect.SetActive(false);
                    content.refreshInfo();
				}, 0.75f));
			}
			else if (num == 2) {
				content.refreshInfo ();
				UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage ("AlchemyContent08",(lastGetMoney*2).ToString()));
				content.alchemyEffect.SetActive(false);
				content.bombEffect.SetActive (true);
				content.bombEffect.particleSystem.Play ();
				NGUITools.AddChild(content.effectPos,content.doubleEffect);

				AudioManager.Instance.PlayAudio (147);
				AudioManager.Instance.PlayAudio (147);
				StartCoroutine (Utils.DelayRun (() => {
					content.alchemyEffect.SetActive(true);
					content.bombEffect.SetActive(false);
                    content.refreshInfo();
				}, 0.75f));
			}

		},0);//初始化用户
	}

	//去充值
	private void gotoShop (MessageHandle msg)
	{
		if (msg.buttonID == MessageHandle.BUTTON_RIGHT) {
			UiManager.Instance.openWindow<rechargeWindow> (); 
		}
	}
}

