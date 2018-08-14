using System;
using System.Collections;
using UnityEngine;

public delegate void TimerHandle ();

/**
 * 时间器
 * 缺陷1:这玩意儿1个Timer只能支持一个事件
 * 缺陷2:不支持毫秒,秒,分级别的单独运行
 * @author longlingquan
 * */
public class Timer
{
	/** 当前执行的次数,通过次数差来计算是否满足执行条件 */
	public int currentCount = 0;
	/** 重复执行的次数,0无限 */
	private int repeatCount = 0;
	/** 每间隔多长时间执行一次 */
	public long delayTime = 0;
	/** 开始时间 */
	private long startTime = 0;
	/** 当前时间 */
	public long currentTime = 0;
	public bool running = false;
	/** 方法回调 */
	private TimerHandle onTimer = null;
	/** repeatCount执行完后进行回调 */
	private TimerHandle onComplete;
	/** 定时器自动移除标识,无限次数的需要手动调用移除方法 */
	private bool dispose = false;
	
	//delay timer时间间隔 repeatCount 倒计时次数  0 为一直循环
	public Timer (long delay, int repeatCount)
	{
		this.delayTime = delay;
		this.repeatCount = repeatCount;
	}

	public void addOnTimer (TimerHandle onTimer)
	{ 
		this.onTimer = onTimer;
	}
	 
	public void addOnComplete (TimerHandle onComplete)
	{
		this.onComplete = onComplete;	
	}
	 
	public void update ()
	{ 
		if (!running) {
			return;
		}
		currentTime = TimeKit.getMillisTime ();//不能用服务器时间,可能没登录就会用timer
		int count = getCurrentCount ();
		 
		if (count > currentCount) {
			currentCount = count;
			if (onTimer != null) { 
				onTimer ();
			}
		}
		if (repeatCount > 0 && currentCount >= repeatCount) {
			stop ();
			if (onComplete != null) {
				onComplete ();
			}
			dispose = true;
		} 
		if (onTimer == null && onComplete == null) {
			stop ();
		}
	}
	 
	public void reset ()
	{
		startTime = TimeKit.getMillisTime ();
		currentCount = 0;
		running = true;
	}
	 
	public void start (bool firstCall)
	{
		if(firstCall)
			currentCount=-1;
		else
			currentCount=0;

		running = true;
		startTime = TimeKit.getMillisTime ();
	}

	public void start ()
	{
		start(false);
	}

	 
	public void stop ()
	{ 
		running = false;
		startTime = 0;
		currentCount = 0;
		onTimer = null;
		onComplete = null;
		Dispose ();
	}
	 
	private int getCurrentCount ()
	{
		return (int)((currentTime - startTime) / delayTime);
	}
	
	private void  Dispose ()
	{
		dispose = true;
	}
	
	public bool isDispose ()
	{
		return dispose;
	}
}