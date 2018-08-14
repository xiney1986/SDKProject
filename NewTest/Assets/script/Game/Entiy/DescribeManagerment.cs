using System;
 
/**
 * 描述信息管理器
 * */
public class DescribeManagerment
{
 
	public DescribeManagerment ()
	{ 
		 
	} 
	 
	//获得描述信息
	public static string getDescribe (string desc, int lv, AttrChangeSample[] changes)
	{
		string[] param = getDescribeParam (lv, changes);
		if (param == null || param.Length < 1)
			return desc;
		desc = replace (desc);
		return replaceParam (desc, param);
	}
	
	//替换内容
	private static string replaceParam (string desc, string[] param)
	{
		for (int i = 0; i < param.Length; i++) {
			string re = '%' + (i + 1).ToString ();
			desc = desc.Replace (re, param [i]);
		}
		return desc;
	}
	
	//替换换行符
	private static string replace (string desc) {
		return desc.Replace ('~', '\n');
	}
	
	//获得描述信息参数
	public static string[] getDescribeParam (int lv, AttrChangeSample[] changes)
	{
		if (changes == null || changes.Length < 1)
			return null;
		string[] strArr = new string[changes.Length];
		for (int i = 0; i < strArr.Length; i++) {
			strArr [i] = changes [i].getAttrValue (lv).ToString ();
		}
		return strArr;
	}
	
} 

