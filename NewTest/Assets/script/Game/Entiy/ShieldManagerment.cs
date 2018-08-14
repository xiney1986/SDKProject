using UnityEngine;
using System.Collections;

/**
 * 屏蔽字管理器
 * @author 汤琦
 * */
public class ShieldManagerment 
{
	//单例
  
	public static ShieldManagerment Instance {
		get{return SingleManager.Instance.getObj("ShieldManagerment") as ShieldManagerment;}
	}

	/// <summary>
	/// 检测是否含有屏蔽字
	/// </summary>
	public bool isContainShield(string str)
	{
		for (int i = 0; i < ShieldConfigManager.Instance.shields.Length; i++) {
			if(str.Contains(ShieldConfigManager.Instance.shields[i]))
			{
				return true;
			}
		}
		return false;
	}

	public bool isContainShield2(string str)
	{
		for (int i = 0; i < ShieldConfigManager.Instance.shields2.Length; i++) {
			if(str.Contains(ShieldConfigManager.Instance.shields2[i]))
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// 过滤屏蔽字为指定字符串
	/// </summary>
	/// <returns>需要检测的文本.</returns>
	/// <param name="str">屏蔽字替换后的文本.</param>
	public string replaceString (string str,string newStr)
	{
		for (int i = 0; i < ShieldConfigManager.Instance.shields.Length; i++) {
			str.Replace (ShieldConfigManager.Instance.shields[i],newStr);
		}
		return str;
	}
}
