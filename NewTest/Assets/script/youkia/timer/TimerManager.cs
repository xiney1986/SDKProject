using System;
using System.Collections.Generic;

/**
 * 时间器管理器
 * @author longlingquan
 * */
public class TimerManager
{
	private static TimerManager _instance;
	private List<Timer> timerList = new List<Timer> ();
	
	public static TimerManager Instance {
		get{
			if(_instance==null)
				_instance=new TimerManager();

			return _instance;
		}
		set{
			_instance=value;

		
		}
	}
	
	public void update ()
	{  
		for (int i=0; i<timerList.Count; i++) {
			if (timerList [i].isDispose ()) {
				timerList.RemoveAt (i);
				continue;
			} else {
				timerList [i].update ();
			}
		}
	}
	public void removeTimer(Timer timer){
		if(timer==null)
			return;

		if(timerList!=null && timerList.Contains(timer)){
			timer.stop();
			timerList.Remove(timer);
		}

	}
	//delay timer间隔时间 count循环次数
	public Timer getTimer (long delay, int count)
	{
		Timer t = new Timer (delay, count);
		timerList.Add (t);
		return t;
	}
	
	//delay timer间隔时间 一直循环
	public Timer getTimer (long delay)
	{
		Timer t = new Timer (delay, 0);
		timerList.Add (t);
		return t;
	} 

	public void clearAllTimer(){
		if (timerList != null || timerList.Count > 0) {
			for(int i = 0; i < timerList.Count;i++){
				if(timerList[i] != null){
					timerList[i].stop();
					timerList[i] = null;
				}
			}
		}
		timerList.Clear ();
	}
} 

