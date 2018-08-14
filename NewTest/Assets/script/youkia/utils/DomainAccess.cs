using System;
using System.Reflection;
 
public class DomainAccess
{ 
	
	public static object getObject (string classStr)
	{
		Type t = getType (classStr);
		if (t == null)
			return null;
		return loadObject (t);
	}

	private static Type getType (string classStr)
	{
		Assembly[] asses = AppDomain.CurrentDomain.GetAssemblies ();
		Type type = null;
		foreach (Assembly item in asses) {
			type = item.GetType (classStr);
			if (type != null) {
				break;
			}
		}
		return type;
	}
	
	private static object loadObject (Type type)
	{
		try {
			object obj = Activator.CreateInstance (type);
			return obj;
		} catch (Exception e) { 
			MonoBase.print("  "+e);
			return null;
		}
	}
}

namespace Youkia {
	public class DomainAccess {

		public DomainAccess(){

		}

		~DomainAccess(){

		}

		public virtual void Dispose(){

		}

		/// 
		/// <param name="classStr"></param>
		public static object getObject(string classStr){

			return null;
		}

		/// 
		/// <param name="classStr"></param>
		private static Type getType(string classStr){

			return null;
		}

		/// 
		/// <param name="type"></param>
		private static object loadObject(Type type){

			return null;
		}

	}//end DomainAccess

}//end namespace Youkia 

