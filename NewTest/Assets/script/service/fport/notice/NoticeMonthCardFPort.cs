
using System;

public class NoticeMonthCardFPort:BaseFPort
{

	private const string get_info="mmcard_info";
	private const string receive="mmcard_day_award";
	private const string buy="buy_mmcard";


	private CallBack callback;
	private string tempCmd;
	public void access_get (CallBack _callback )
	{   
		this.callback = _callback;
		tempCmd=get_info;
		ErlKVMessage message = new ErlKVMessage (FrontPort.MONTHCARD_GET);
		access (message);
	}
	public void access_receive(CallBack _callback )
	{
		this.callback = _callback;
		tempCmd=receive;
		ErlKVMessage message = new ErlKVMessage (FrontPort.MONTHCARD_RECEIVE);	
		access (message);
	}
	public void access_buy(CallBack _callback,int sid)
	{
		this.callback = _callback;
		tempCmd=buy;
		ErlKVMessage message = new ErlKVMessage (FrontPort.MONTHCARD_BUY);
		message.addValue ("goods_id", new ErlInt (sid));
		access (message);
	}
	 
	public override void read (ErlKVMessage message)
	{		
		ErlType msg = message.getValue ("msg") as ErlType;
		switch(tempCmd)
		{
			case get_info:
				readGetInfo(msg);				
			break;
			case receive:
				string infoMsg1=msg.getValueString();
				if(infoMsg1 == "ok")
				{
					NoticeManagerment.Instance.monthCardDayRewardEnable=false;
					TextTipWindow.ShowNotUnlock (LanguageConfigManager.Instance.getLanguage ("s0120"));
				}else
				{
					MessageWindow.ShowAlert (infoMsg1);
				}
			break;
			case buy:
				/*
				string infoMsg2=msg.getValueString();
				if(infoMsg2 == "ok")
				{
					MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage("monthCardBuySuccessTip"));
				}else
				{
					MessageWindow.ShowAlert (infoMsg2);
				}
				*/
				if(msg is ErlArray)
				{
					ErlArray parameters = msg as ErlArray;
					
					ErlArray receiveTime=parameters.Value[0] as ErlArray;
					int canReceiveTime=StringKit.toInt(parameters.Value[0].getValueString());
					DateTime time=TimeKit.getDateTimeMin(canReceiveTime);
					int receiveYear=time.Year;
					int receiveMonth=time.Month;
					int receiveDay=time.Day;
                    NoticeManagerment.Instance.monthCardDueDate = new int[3] { receiveYear, receiveMonth, receiveDay };
                    NoticeManagerment.Instance.monthCardDueSeconds = canReceiveTime;
					
					int canReceiveEnable=StringKit.toInt(parameters.Value[1].getValueString());
					NoticeManagerment.Instance.monthCardDayRewardEnable=canReceiveEnable==1;

					MessageWindow.ShowAlert (LanguageConfigManager.Instance.getLanguage("monthCardBuySuccessTip"));
				}else
				{
					string infoMsg2=msg.getValueString();
					MessageWindow.ShowAlert (infoMsg2);					
				}		
			break;
		}
		if (callback != null)
		{
			callback ();
		} 
	}

	private void readGetInfo(ErlType msg) {
		if(msg is ErlArray)
		{
			ErlArray parameters = msg as ErlArray;
			ErlArray receiveTime=parameters.Value[0] as ErlArray;
			int canReceiveTime=StringKit.toInt(parameters.Value[0].getValueString());
			DateTime time=TimeKit.getDateTime(canReceiveTime);
			int receiveYear=time.Year;
			int receiveMonth=time.Month;
			int receiveDay=time.Day;
			NoticeManagerment.Instance.monthCardDueDate=new int[3]{receiveYear,receiveMonth,receiveDay};
            NoticeManagerment.Instance.monthCardDueSeconds = canReceiveTime;
			
			int canReceiveEnable=StringKit.toInt(parameters.Value[1].getValueString());
			NoticeManagerment.Instance.monthCardDayRewardEnable=canReceiveEnable==1;
		} else {
			NoticeManagerment.Instance.monthCardDayRewardEnable=false;
			NoticeManagerment.Instance.monthCardDueDate=null;
		}						
	}

	//解析ErlKVMessgae
	public bool parseKVMsg (ErlKVMessage message) {
		ErlType msg = message.getValue ("msg") as ErlType;
		readGetInfo (msg);
		return true;
	}
}