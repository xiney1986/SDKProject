using UnityEngine;
using System.Collections;

public class TimeListPerfab : MonoBase {
	/**刷新时间 */
	public UILabel tineText;
	public void init(string time){
		string[] arr=time.Split(':');
		if(arr.Length==3){
			if(arr[1]=="00"&&arr[2]=="00")tineText.text=arr[0]+LanguageConfigManager.Instance.getLanguage("shop_zheng");
			else tineText.text=time;
		}else{
			tineText.text=time;
		}
	}

}
