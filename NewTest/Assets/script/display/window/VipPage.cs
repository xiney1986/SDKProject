using UnityEngine;
using System.Collections;

public class VipPage : MonoBehaviour
{
	public Vip vipinfo;
	public UILabel[] descripts;
//	public UILabel titleLabel;

	public void updatePage (Vip info)
	{
		this.vipinfo = info;
//		titleLabel.text = "VIP " + vipinfo.vipLevel + LanguageConfigManager.Instance.getLanguage("s0314");		
		for (int i=0; i<descripts.Length; i++) {
			if (i < vipinfo.vipDescripts.Length) {
				descripts [i].gameObject.SetActive (true);
				descripts [i].text = "      "+vipinfo.vipDescripts [i];
			} else {
				descripts [i].gameObject.SetActive (false);
			}
		}
	}

	public Vip getVip()
	{
		return vipinfo;
	}
	
}
