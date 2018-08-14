using UnityEngine;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System;

public class UrlKit
{

//	public static void Test  )
//	{
//		string pageURL = "http://s1.mobile.pvz.youkia.com/openapi/index_android.php?userid=7561718&nickname=&face=&time=1379830939&inviteuser=0&callback=&sig=fabca3bbbe1c5fa026f33bf9e933ca6&sig2=fabca3bbbe1c5fa026f33bf9e933ca6&vip=0&isfangchenmi=0&OS=Android&version=4.1.2";
//		 Uri uri = new Uri(pageURL);
//		string queryString = uri.Query;
//		NameValueCollection col = GetQueryString (queryString);
//		string searchKey = col ["sig"];
//
//	}
	public static NameValueCollection GetQueryString(string queryString)
{
    return GetQueryString(queryString, null, true);
}
	public static NameValueCollection GetQueryString (string queryString, Encoding encoding, bool isEncoded)
	{
		queryString = queryString.Replace ("?", "");
		NameValueCollection result = new NameValueCollection (StringComparer.OrdinalIgnoreCase);
		if (!string.IsNullOrEmpty (queryString)) {
			int count = queryString.Length;
			for (int i = 0; i < count; i++) {
				int startIndex = i;
				int index = -1;
				while (i < count) {
					char item = queryString [i];
					if (item == '=') {
						if (index < 0) {
							index = i;
						}
					} else if (item == '&') {
						break;
					}
					i++;
				}
				string key = null;
				string value = null;
				if (index >= 0) {
					key = queryString.Substring (startIndex, index - startIndex);
					value = queryString.Substring (index + 1, (i - index) - 1);
				} else {
					key = queryString.Substring (startIndex, i - startIndex);
				}
				if (isEncoded) {
					result [MyUrlDeCode (key, encoding)] = MyUrlDeCode (value, encoding);                    
				} else {
					result [key] = value;
				}
				if ((i == (count - 1)) && (queryString [i] == '&')) {
					result [key] = string.Empty;
				}
			}
		}
		return result;
	}

/// <summary>
/// 解码URL.
/// </summary>
/// <param name="encoding">null为自动选择编码</param>
/// <param name="str"></param>
/// <returns></returns>
	public static string MyUrlDeCode (string str, Encoding encoding)
	{
		if (encoding == null) {
			Encoding utf8 = Encoding.UTF8;
			//首先用utf-8进行解码                     
			string code = WWW.UnEscapeURL (str.ToUpper (), utf8);
			//将已经解码的字符再次进行编码.
			string encode = WWW.EscapeURL (code, utf8).ToUpper ();
			if (str == encode)
				encoding = Encoding.UTF8;
			else
				encoding = System.Text.Encoding.Default;
		}
		return WWW.UnEscapeURL (str, encoding);
	}
	
}