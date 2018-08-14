using System;
 
/**
 * 副本前往下一个点
 * */
public class FuBenGotoFPort:BaseFPort
{
	private const string NOT_GOTO = "not_goto";//不能前进 当前点有事件未完成
	private const string GOTO = "goto_point";//成功前往下一个点
	public const int NOT_GO = -1;//不能前进 当前点有事件未完成
	public const int NONE = -2;//空点
	
	private CallBack<int> callback;
	
	public FuBenGotoFPort ()
	{
		
	}
	
	public void gotoNext (CallBack<int> callback)
	{
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.FUBEN_GOTO);  
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		string str = (message.getValue ("msg") as ErlAtom).Value;  
		if (str == GOTO) { 
			ErlType t = message.getValue ("info") as ErlType; 
			int sid = 0;
			
			if (!(t  is ErlArray) && t.getValueString()=="none") {
				//空的休息点,没有pvp
				sid = NONE;
			}  
			if (MissionManager.instance != null)
				callback (sid);
		} else if (str == NOT_GOTO) {
			if (MissionManager.instance != null)
				callback (NOT_GO);
		}
	}
} 

