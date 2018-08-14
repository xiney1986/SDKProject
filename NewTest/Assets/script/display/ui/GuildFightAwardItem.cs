using UnityEngine;
using System.Collections;

public class GuildFightAwardItem : ButtonBase {
	public GameObject goodViewPrefab;
	public GameObject content;
	public UILabel title;
	GuildFightAwardSample sample;
	public void init(GuildFightAwardSample sample ){
		int pos = 0;
		title.text = sample.name;
		Utils.DestoryChilds(content.gameObject);
		for(int i = 0; i < sample.prizes.Length; i++)
		{
			PrizeSample ps = sample.prizes[i];
			if(ps.type != PrizeType.PRIZE_CARD)
			{
				GameObject obj = NGUITools.AddChild(content.gameObject,goodViewPrefab);
				GoodsView sc = obj.GetComponent<GoodsView>();
				sc.fatherWindow = fatherWindow;
				sc.init(ps);
				obj.transform.localScale = new Vector3(0.95f,0.95f,1);
				obj.transform.localPosition = new Vector3(pos*110 - 20,0,0);
				pos++;
			}
		}
	}
}
