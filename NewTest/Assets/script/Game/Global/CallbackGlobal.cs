using System;
using System.Collections.Generic;

public class CallbackGlobal
{
	 
}

public delegate bool BCallBack();

public delegate void CallBack<T>(T arg1);

public delegate void CallBack<T, U>(T arg1, U arg2);

public delegate void CallBack<T, U, V>(T arg1, U arg2, V arg3);

public delegate void CallBack<T, U, V, R>(T arg1, U arg2, V arg3, R arg4);


public delegate R RCallBack<R,T>(T arg1);

public delegate R RCallBack<R,T, U>(T arg1, U arg2);

public delegate R RCallBack<R,T, U, V>(T arg1, U arg2, V arg3);

public delegate void CallBack ();


public delegate void  texLoaderCallBack (List<ResourcesData> _list);

public delegate void CallBackLuckyDrawResults (LuckyDrawResults results);

public delegate void CallAttr (List<AttrChange> attrs,int type);

public delegate void CallBackBuy (object item,int num);

public delegate void CallBackMsg (MessageHandle msg);

public delegate void CallBackBFC (BattleFormationCard[] cards1,BattleFormationCard[] cards2,int type); 

public delegate void CallBackBCards(BattleFormationCard[] cards);

public delegate void CallBackStrtArray(string [] arr);

public delegate void CallBackWindow(WindowBase window);
