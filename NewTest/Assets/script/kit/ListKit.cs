using UnityEngine;
using System.Collections;

public class ListKit  {
 
	/**
	 * List<>的AddRange重写 因为AddRange在IOS系统有问题
	 * 有疑问询问hzh 或者自己看 
	 * http://stackoverflow.com/questions/9929388/c-sharp-listobject-to-ilist-cast-bug-in-unity3d
	 * http://answers.unity3d.com/questions/500170/c-list-bug-using-addrange-after-call-tostring-meth.html
	 * 
	 * @param 目标列表
	 * @param 源列表
	 * 
	 * 
	 * 不能传[]int 作为sourceList进来,不然ios挂
	 * 
	 */
	public static void AddRange(IList targetList,IList sourceList)
	{
		if (targetList == null || sourceList == null)
			return;
		if (sourceList.Count < 1)
			return;
		foreach (object obj in sourceList) {
			targetList.Add(obj);
		}
	}
	//泛型样例
//	public static void AddRange_B<T>(IList<T> targetList,T[] sourceList)
//	{
//		//ICollection
//	}
//	public static void AddRange_B<T>(IList<T> targetList,IList<T> sourceList)
//	{
//		//ICollection
//	}
//	public void a()
//	{
//		List<int> a=new List<int>();
//		int[] b=new int[]{1,5};
//		
//		List<string> c=new List<string>();
//		List<string> d=new List<string>();
//		
//		AddRange_B<int>(a,b);
//		AddRange_B<string>(c,d);
//	}
}
