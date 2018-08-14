using UnityEngine;
using System.Collections;

/// <summary>
///  自定义#号编码规则，用#做为命令字符进行编码。主要用于编码文本中的换行符,和sql语句或脚本语言中的转义符和单引号,也可自定义转换关系。
/// </summary>
public class Sharp
{

	/* static fields */
	/** 命令字符 */
	static char SHARP='#';
	/** 默认的忽略字符 */
	static char[] IGNORE_CHARS={'\r'};
	/** 默认的替换字符1 */
	static char[] REPLACE_CHARS1={'\n','n'};
	/** 默认的替换字符2 */
	static char[] REPLACE_CHARS2={'\n','n','\\','b','\'','q'};
	/** Sharp编解码算法1 */
	public static Sharp SHARP1=new Sharp(IGNORE_CHARS,new char[]{'\n','n','\\','b','\'','q',';','s',':','c'});
	
	/* fields */
	/** 忽略字符数组 */
	char[] ignoreChars;
	/** 替换字符数组 */
	char[] replaceChars;
	
	/* constructors */
	/** 构造指定忽略字符数组和替换字符数组的Sharp编解码算法 */
	public Sharp(char[] ignoreChars,char[] replaceChars)
	{
		this.ignoreChars=ignoreChars;
		this.replaceChars=replaceChars;
	}

	/* methods */
	/** 编码方法 */
	public string encode(string str)
	{
		if(str==null) return "";
		int n = str.Length;
		if(n<=0) return str;
		CharBuffer cb=new CharBuffer(n+n/8);
		bool b=encode(str,cb);
		return b?cb.getString():str;
	}
	/** 编码方法，返回是否编码 */
	public bool encode(string str,CharBuffer cb)
	{
		if(str==null) return false;
		int n = str.Length;
		if(n<=0) return false;
		bool coding=false;
		char[] ignores=ignoreChars;
		char[] replaces=replaceChars;
		char c;
		for(int i=0,j=0,n1=ignores.Length,n2=replaces.Length-1;i<n;i++)
		{
			c=str[i];
			for(j=0;j<n1;j++)
			{
				if(c!=ignores[j]) continue;
				coding=true;
				break;
			}
			if(j<n1) continue;
			if(c==SHARP)
			{
				cb.append(c);
				cb.append(c);
				coding=true;
				continue;
			}
			for(j=0;j<n2;j+=2)
			{
				if(c!=replaces[j]) continue;
				cb.append(SHARP);
				c=replaces[j+1];
				coding=true;
				break;
			}
			cb.append(c);
		}
		return coding;
	}
	/** 解码方法，返回是否解码 */
	public string decode(string str)
	{
		if(str==null) return "";
		int n = str.Length;
		if(n<=0) return str;
		CharBuffer cb=new CharBuffer(n);
		bool b=decode(str,cb);
		return b?cb.getString():str;
	}
	/** 解码方法 */
	public bool decode(string str,CharBuffer cb)
	{
		if(str==null) return false;
		int n = str.Length;
		if(n<=0) return false;
		bool coding=false;
		char[] replaces=replaceChars;
		char c1,c2;
		for(int i=0,j=0,m=replaces.Length;i<n;i++)
		{
			c1=str[i];
			if(c1!=SHARP)
			{
				cb.append(c1);
				continue;
			}
			coding=true;
			i++;
			if(i>=n) break;
			c2=str[i];
			if(c2==SHARP)
			{
				cb.append(c2);
				continue;
			}
			for(j=1;j<m;j+=2)
			{
				if(c2!=replaces[j]) continue;
				cb.append(replaces[j-1]);
				break;
			}
		}
		return coding;
	}

	/* properties */
	/** 获得忽略字符数组 */
	public char[] getIgnoreChars()
	{
		return ignoreChars;
	}
	/** 设置忽略字符数组 */
	public void setIgnoreChars(char[] chars)
	{
		if(chars==null) chars=new char[0];
		ignoreChars=chars;
	}
	/** 获得替换字符数组 */
	public char[] getReplaceChars()
	{
		return replaceChars;
	}
	/** 设置替换字符数组 */
	public void setReplaceChars(char[] chars)
	{
		if(chars==null) chars=new char[0];
		replaceChars=chars;
	}
}
