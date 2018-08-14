using System;

public class ThreadKit
{
	public static void dumpStack()
	{
		try
		{
			throw new Exception();
		}
		catch (Exception ex)
		{
			 Log.error(null,ex);
		}
	}
}
