using System;
 
public class TestFPort:BaseFPort
{
	public TestFPort ()
	{
	}

	public void test ()
	{
		ErlKVMessage message = new ErlKVMessage ("/yxzh/test");   
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		/**	MonoBase.print ("==================");
		
		string str = (message.getValue ("msg") as ErlType).getValueString ();
		
		int thist = TimeKit.getSecondTime ();
		MonoBase.print ("this   " + thist);
		MonoBase.print ("server   " + str);
		
		MonoBase.print ("-------------------------  " + (StringKit.toInt (str) - thist));*/
	}
} 

