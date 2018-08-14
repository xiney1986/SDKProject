using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MysticalComp : IComparer {
	public int Compare (object o1, object o2) {
		if (o1 == null)
			return 1;
		if (o2 == null)
			return -1;
		Goods goods1 = o1 as Goods;
		Goods goods2 = o2 as Goods;
		if (goods1 == null || goods2 == null)
			return 0;
		if (goods1.getOrder () > goods2.getOrder ())
			return 1;
		if (goods1.getOrder () < goods2.getOrder ())
			return -1;
		return 0;
	}
}

public class LadderHegeMoneyManager {

	public static LadderHegeMoneyManager Instance {
		get{ return SingleManager.Instance.getObj ("LadderHegeMoneyManager") as LadderHegeMoneyManager;}
	}

	public int myPort;//我的积分
	public int[] ladderTime;//争霸时间
	public int myRank;//我的排名
	public int myPrize;//我的奖励
	public int laddersid;//当前天梯争霸的sid

	private ArrayList goodlists;
	private bool isChallenge;

	public bool IsChallenge {
		get {
			if (true)
				return true;
			else 
				return false;
		}
	}

	public ArrayList GoodsList {
		get {
			return goodlists;
		}
	}

	private  int SortA (Goods a1, Goods a2) {
		return a1.showIndex.CompareTo (a2.showIndex);
	}

	public void initMsg (CallBack callBack) {
		LadderHegeMoneyFport fpor = FPortManager.Instance.getFPort ("LadderHegeMoneyFport") as LadderHegeMoneyFport;
		fpor.getladderMsg ((msg) => {
			if (msg.Value.Length < 1)
				return;
			
			myRank = StringKit.toInt (msg.Value [0].getValueString ());
			myPort = StringKit.toInt (msg.Value [1].getValueString ());
			
		});
		InitShopFPort fport = FPortManager.Instance.getFPort ("InitShopFPort") as InitShopFPort;		
		fport.access (() => 
		{
			goodlists = ShopManagerment.Instance.getLadderSidGoods (laddersid);
			goodlists.Sort (new MysticalComp ());
			if (callBack != null)
				callBack ();
		});
	}

	public void initMsg (int lsid, CallBack callBack) {
		this.laddersid = lsid;
		LadderHegeMoneyFport fpor = FPortManager.Instance.getFPort ("LadderHegeMoneyFport") as LadderHegeMoneyFport;
		fpor.getladderMsg ((msg) => {
			if (msg.Value.Length < 1)
				return;

			myRank = StringKit.toInt (msg.Value [0].getValueString ());
			myPort = StringKit.toInt (msg.Value [1].getValueString ());
		});

		InitShopFPort fport = FPortManager.Instance.getFPort ("InitShopFPort") as InitShopFPort;		
		fport.access (() => 
		{
			goodlists = ShopManagerment.Instance.getLadderSidGoods (laddersid);
			if (callBack != null)
				callBack ();
		});
	}
}

public class LadderHegeMoneyFport:BaseFPort {
	public const int TYPE_INIT = 1;//获取初始化信息
	public const int TYPE_RULE = 2;//规则问题
	public const int TYPE_CHALLENGE = 3; //挑战信息

	private int sendType;
	private CallBack callBack;
	private CallBack<bool> callbackt;
	private CallBack<int> callbacki;
	private CallBack<string> callbacks;
	private CallBack<ErlArray> callbackA;
	
	public void getladderMsg (CallBack<ErlArray> call) {
		sendType = TYPE_INIT;
		
		ErlKVMessage msg = new ErlKVMessage (FrontPort.LADDERHEGEMONEY_GET_INT);
		access (msg);
		this.callbackA = call;
	}
	
	public void sendsdkFriendsMsg (string uid, CallBack<string> callback) {
		sendType = TYPE_RULE;
		ErlKVMessage msg = new ErlKVMessage (FrontPort.SDK_FRIEND_SEND);
		msg.addValue ("uid", new ErlString (uid));
		access (msg);
		this.callbacks = callback;
	}
	
	public void sendAddsdkFriendsMsg (string uid, CallBack<string> callback) {
		sendType = TYPE_CHALLENGE;
		ErlKVMessage msg = new ErlKVMessage (FrontPort.SDK_ADD_SDK_FRIEND);
		msg.addValue ("uid", new ErlString (uid));
		access (msg);
		this.callbacks = callback;
	}
	
	public override void read (ErlKVMessage message) {
		if (sendType == TYPE_INIT) {
			ErlType msg = message.getValue ("msg") as ErlType;
			if (msg is ErlArray) {
				ErlArray array = msg as ErlArray;
				
				if (array == null)
					return;
				
				if (callbackA != null)
					callbackA (array);
			}
		}
		else if (sendType == TYPE_RULE) {
			
			string msg = (message.getValue ("msg") as ErlType).getValueString ();
			if (msg != null) {
				
				if (callbacks != null) {
					callbacks (msg);
				}
			}
			
		}
		else if (sendType == TYPE_CHALLENGE) {
			
			string msg = (message.getValue ("msg") as ErlType).getValueString ();
			if (msg != null) {
				
				if (callbacks != null) {
					callbacks (msg);
				}
			}
			
		}
	}
}