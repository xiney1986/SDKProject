using System;

/**
 * 通讯测试
 * @author zhoujie
 * */
public class TestPort
{
	private static TestPort _Instance;

	private TestPort ()
	{
	}

	public static TestPort Instance {
		get { 
			if (_Instance == null) {
				_Instance = new TestPort ();
				return _Instance;
			} else
				return _Instance;
		}
		set { 
			_Instance = value;
		}
	}
	
	public void testPort ()
	{
		//测试道具使用
		//	UsePropPort port = FPortManager.Instance.getFPort ("UsePropPort") as UsePropPort;
		//	port.access (10004, 1, 1, usePropOK);
		FightFPort fport = FPortManager.Instance.getFPort ("FightFPort") as FightFPort;
	//	fport.send (5017);

	}
	
	
	//通讯接收
	public void usePropOK (string str)
	{
		MonoBase.print ("=usePropOK:" + str);
	}
	
	
}
