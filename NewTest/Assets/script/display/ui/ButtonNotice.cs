using UnityEngine;
using System.Collections;

public class ButtonNotice : ButtonBase
{
	public Notice temp;
	public NoticeWindow win;
	public UISprite background;
	
	public void UpdateTemp (Notice _temp)
	{
		win = fatherWindow as NoticeWindow;
		this.temp = _temp;
	}
	 
	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
//		if(temp.readed == 0)
//		{
//			NoticeReadFPort fport = FPortManager.Instance.getFPort("NoticeReadFPort") as NoticeReadFPort;
//			fport.access(temp.sid,resultBack);
//		}
//		else
//		{
			resultBack();
//		}
	} 
	
	//通信成功后的回调
	private void resultBack()
	{
		 
		NoticeManagerment.Instance.setNoticeReaded(temp.sid);
		NoticeSample sample = NoticeSampleManager.Instance.getNoticeSampleBySid(temp.sid);
		if(sample.type == NoticeType.EXCHANGENOTICE)
		{
//			UiManager.Instance.openWindow<NoticeActivityExchangeWindow>((win)=>{
//				win.initWindow(temp,this.win);
//			});
		}
		else if(sample.type == NoticeType.TOPUPNOTICE)
		{
//			UiManager.Instance.openWindow<NoticeActivityRechargeWindow>((win)=>{
//				win.initWindow(temp,this.win);
//			});
		}
		else
		{
			MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("s0072"));
		}
		
	}
}
