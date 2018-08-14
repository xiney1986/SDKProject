using System;
public class PracticetimeService:BaseFPort
{
	public PracticetimeService ()
	{
		
	}
	
	public override void read (ErlKVMessage message)
	{
		ErlType practicetime = message.getValue ("practicetime") as ErlType;//修炼副本倒计时
		if(practicetime!=null)
		{
			FuBenManagerment.Instance.practiceDueTime=StringKit.toInt (practicetime.getValueString ());
		}
		ErlType practicenum = message.getValue ("practicecount") as ErlType;//修炼副本使用次数
		if(practicenum!=null)
		{
			int times=StringKit.toInt (practicenum.getValueString ());
			FuBenManagerment.Instance.updatePracticeNum(times);
		}
	}
} 


