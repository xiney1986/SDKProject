using UnityEngine;
using System.Collections;

public class MonthCardBuyWindow : WindowBase
{
	public UILabel lab_intro;
	public GameObject go_itemListContent;
	public GameObject go_itemPrefab;
	public UILabel lab_rmbCount;
	public UIButton btn_close;


	protected override void begin()
	{
		base.begin (); 
		MaskWindow.UnlockUI ();

		EventDelegate.Add(btn_close.onClick,onButtonClose);
		M_creatCarList();
		lab_rmbCount.text=UserManager.Instance.self.getRMB().ToString();
	}
	private CallBack fatherCallBackFun;
	public void init(CallBack _callBack)
	{
		fatherCallBackFun=_callBack;
	}
	private void M_creatCarList()
	{
		int[] ids=MonthCardSampleManager.Instance.getAllSampleIds();
		MonthCardSample item;
		GameObject newItem;
		MonthCardBuyItemCtrl itemView;
		for(int i=0,length=ids.Length;i<length;i++)
		{
			item=MonthCardSampleManager.Instance.getSampleById(ids[i]);
			newItem= NGUITools.AddChild(go_itemListContent,go_itemPrefab);
			newItem.SetActive(true);
			itemView=newItem.GetComponent<MonthCardBuyItemCtrl>();
			itemView.init(item.sid,item.des,item.rmb,this,item.sid);
		}
		go_itemListContent.GetComponent<UIGrid>().Reposition();
	}


	  
	private void onButtonClose()
	{
		finishWindow();
	}
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		int sid=StringKit.toInt(gameObj.name);
		MonthCardSample sample=MonthCardSampleManager.Instance.getSampleById(sid);
		if(sample==null)
		{
			return;
		}
		if(UserManager.Instance.self.getVipLevel()< MonthCardSampleManager.Instance.getVipRequestMinLevel())
		{
			string msg=LanguageConfigManager.Instance.getLanguage ("monthCardIntro4");
//			MessageWindow.ShowAlert(msg);
			UiManager.Instance.openDialogWindow<MessageWindow> ((MessageWindow win) => {
				win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("s0315"), msg, (eventMsg) => {
					if (eventMsg.buttonID == MessageHandle.BUTTON_RIGHT) {
						finishWindow();
						UiManager.Instance.openWindow<rechargeWindow> (); 
					}
				});
			});
			return;
		}
		if(sample.rmb>UserManager.Instance.self.getRMB())
		{
			string msg=LanguageConfigManager.Instance.getLanguage("s0158");
			UiManager.Instance.openDialogWindow<MessageWindow> ((MessageWindow win) => {
				win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("s0315"), msg, (eventMsg) => {
					if (eventMsg.buttonID == MessageHandle.BUTTON_RIGHT) {
						finishWindow();
						UiManager.Instance.openWindow<rechargeWindow> (); 
					}
				});
			});
			//MessageWindow.ShowAlert(msg);
			//UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
			//	win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0093"),LanguageConfigManager.Instance.getLanguage ("s0315"),msg, closeLackRmb_MessageWindown);
			//});
			return;
		}

		tempSid=sid;
		string info = LanguageConfigManager.Instance.getLanguage ("monthCardTip", sample.rmb.ToString(), sample.month.ToString());
		UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
			win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("s0093"), info, closeBuyCard_MessageWindown);
		});
	}
	/// <summary>
	/// 关闭钻石不足提示窗口
	/// </summary>
	/// <param name="msg">Message.</param>
	private void closeLackRmb_MessageWindown(MessageHandle msg)
	{
		if (msg.buttonID == MessageHandle.BUTTON_RIGHT) {			
			NoticeMonthCardFPort sp=FPortManager.Instance.getFPort ("NoticeMonthCardFPort") as NoticeMonthCardFPort;
			sp.access_buy(onBuyCmp,tempSid);			
		}
	}
	private int tempSid;
	/// <summary>
	/// 关闭购买提示窗口
	/// </summary>
	/// <param name="msg">Message.</param>
	private void closeBuyCard_MessageWindown(MessageHandle msg)
	{
		if (msg.buttonID == MessageHandle.BUTTON_RIGHT) {			
			NoticeMonthCardFPort sp=FPortManager.Instance.getFPort ("NoticeMonthCardFPort") as NoticeMonthCardFPort;
			sp.access_buy(onBuyCmp,tempSid);			
		}
	}
	/// <summary>
	/// 购买后回调
	/// </summary>
	public void onBuyCmp()
	{
		if(fatherCallBackFun!=null)
		{
			fatherCallBackFun();
		}
		lab_rmbCount.text=UserManager.Instance.self.getRMB().ToString();
	}
}

