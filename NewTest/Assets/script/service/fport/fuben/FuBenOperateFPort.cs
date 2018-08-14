using System;
 
/**
 * 副本操作
 * 副本保存 副本放弃
 * @author longlingquan
 * */
public class FuBenOperateFPort:BaseFPort
{
	private const string GIVE_UP = "abandon";//放弃副本
	private CallBack callback;
	public FuBenOperateFPort ()
	{
		
	}
	
	//保存
	public void save ()
	{
//		ErlKVMessage message = new ErlKVMessage (FrontPort.FUBEN_SUSPEND);  
//		access (message);
		//不需要通讯，后台完全不需要保存通讯
	}
	
	//放弃
	public void giveUp ()
	{
		ErlKVMessage message = new ErlKVMessage (FrontPort.FUBEN_ABANDON);  
		access (message);
	}

	public void giveUp(CallBack callback){
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage (FrontPort.FUBEN_ABANDON);  
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		if(callback != null){
			callback();
		}
		//前台不处理，不在乎返回
	}
	
} 

