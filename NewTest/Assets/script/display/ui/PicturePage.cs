using UnityEngine;
using System.Collections.Generic;

public class PicturePage : MonoBase {
	public PictureButton [] buttons;
    public GameObject starPrefab;
	private List<Card> list;
	public void updatePage(List<Card> list){
		this.list = list;
		for (int i = 0; i<buttons.Length; i++) {
			if(i<list.Count){
				buttons[i].gameObject.SetActive(true);
				buttons[i].init(list[i]);
                if (buttons[i].gameObject.transform.FindChild("starContent(Clone)") != null) {
                    Transform star = buttons[i].gameObject.transform.FindChild("starContent(Clone)");
                    DestroyImmediate(star.gameObject);
                }
                if (CardSampleManager.Instance.getStarLevel(list[i].sid) > 0) {
                    GameObject starContent = NGUITools.AddChild(buttons[i].gameObject, starPrefab);
                    ShowStars showStar = starContent.GetComponent<ShowStars>();
                    showStar.initStar(CardSampleManager.Instance.getStarLevel(list[i].sid), CardSampleManager.USEDBYCARDITEM);
                }
			}
			else{
				buttons[i].gameObject.SetActive(false);
			}
		}
	}

	public List<Card> getCards(){
		return list;
	}

}
