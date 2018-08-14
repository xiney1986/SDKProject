using System;
using System.Reflection;
using System.Collections;
 
/**
 * 接口管理器
 * */
public class FPortManager
{ 
	//单例
	private static FPortManager _Instance;
	private static bool _singleton = true;
	private Hashtable ports = null;
	
	public static FPortManager Instance {
		get {
			
			if (_Instance == null) {
				_singleton = false;
				_Instance = new FPortManager ();
				_singleton = true;
				return _Instance;
			} else
				return _Instance;
		}
		set { 
			_Instance = value;
		}
	}
	
	public FPortManager ()
	{
		if (_singleton)
			throw new Exception ("this is singleton!"); 
	}
	
	public BaseFPort getFPort (string str)
	{
		if (ports == null)
			ports = new Hashtable ();
		if (ports [str] == null)
			ports.Add (str, DomainAccess.getObject (str)); 
		return ports [str] as BaseFPort;
	}

    public T getFPort<T>() where T : BaseFPort
    {
        Type type = typeof(T);
        string str = type.Name;
        if (ports == null)
            ports = new Hashtable ();

        if (ports [str] == null)
        {
            object obj = Activator.CreateInstance (type);
            ports.Add(str, obj); 
        }
        return ports [str] as T;
    }
	public void clearPorts () {
		if (ports != null)
			ports.Clear ();
	}
} 

