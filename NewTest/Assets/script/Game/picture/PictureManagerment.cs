using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Picture managerment.
/// </summary>
public class PictureManagerment
{
	public static PictureManagerment Instance {
		get {
			bool isExist=SingleManager.Instance.checkObj("PictureManagerment");
			PictureManagerment instance;
			if(!isExist) {
				instance = SingleManager.Instance.getObj ("PictureManagerment") as PictureManagerment;
				instance.init ();
			} else {
				instance=SingleManager.Instance.getObj ("PictureManagerment") as PictureManagerment;
			}
			return instance;
		}
	}

	public Dictionary<int,List<CardSample>> mapType = new Dictionary<int, List<CardSample>> ();
	public Dictionary<int,PictureSample> mapPic = new Dictionary<int, PictureSample>();
	public ArrayList cardList = new ArrayList ();
	public Dictionary<Card,int> mapCard = new Dictionary<Card, int> ();
	/** 缓存的图鉴模版,点击图鉴的具体卡片时获得 */
	public PictureSample currentSample = null;
	//初始化,加载卡牌数据,分类,排序
	public void init ()
	{
		List<PictureSample> picList = PictureSampleManager.Instance.pictureList;
		CardSample sample;
		Card card;
		int type = 0;
		List<CardSample> list;		
		foreach (PictureSample pic in picList) {
			foreach(int sid in pic.ids)
			{
				sample = CardSampleManager.Instance.getRoleSampleBySid (sid);
				type = sample.evolveSid;//进化类型
				//只有主角卡的进化类型小于100 主角卡不进入图鉴
				if (type < 100)
					continue;
				if (mapType.ContainsKey (type)) {
					list = mapType [type];
				} else {
					list = new List<CardSample> ();
					mapType.Add (type, list);					
					card = new Card ("", sid, 0, 0, 0, 0, 0, 0, 0, null, null, null, null, 0, 0, 0, 0, 0, 0,"",0);
					card.setLevel (sample.maxLevel);
					cardList.Add(card);
					mapCard.Add (card, type);
				}
				list.Add (sample);
			}
			if(type>100)
				mapPic.Add(type,pic);
		}
	}

}
