using UnityEngine;
using System.Collections;

public class GodsWarRankAwardItem : MonoBehaviour
{
	/** 积分奖励用名字 */
	public UILabel lblName;
	/** 竞猜奖励用背景 */
	public UISprite bg00;
	public UISprite bg01;
	public GameObject completedIcon;
	public GameObject content;
	public UILabel lblCondition;
	public UISprite firstAwardLogo;
	public GameObject title;
	[HideInInspector]
	public WindowBase
		win;
	GodsWarPrizeSample info;

	public void init (GodsWarPrizeSample info, int index, WindowBase win)
	{
		this.info = info;
		this.win = win;
		bool canReceived = false;
		lblCondition.text = "";
		completedIcon.SetActive (false);
		bg01.gameObject.SetActive (false);
		firstAwardLogo.gameObject.SetActive (false);
		title.gameObject.SetActive (false);
		lblName.text = info.des;
		lblName.effectStyle = UILabel.Effect.Outline;
		lblName.effectColor = new Color (33f / 255, 59f / 255, 87f / 255);
		lblName.transform.localPosition = new Vector3 (0, 86, 0);
		string currentState = GodsWarManagerment.Instance.getGodsWarStateInfo ();
		if (currentState.EndsWith ("zige_group")) {
			if (index == 0) {
				firstAwardLogo.gameObject.SetActive (true);
				title.gameObject.SetActive (false);
				lblName.gameObject.SetActive (false);
			} else {
				firstAwardLogo.gameObject.SetActive (false);
				title.gameObject.SetActive (true);
				lblName.gameObject.SetActive (true);
				lblName.gameObject.transform.localPosition = new Vector3 (0, lblName.gameObject.transform.localPosition.y, lblName.gameObject.transform.localPosition.z);
			}
		} else if (currentState.EndsWith ("zige_final") || currentState.EndsWith ("zige_taotai")) {
			firstAwardLogo.gameObject.SetActive (true);
			firstAwardLogo.spriteName = "medal_" + (index + 1);
			if (index > 3)
				firstAwardLogo.spriteName = "medal_4";
			title.gameObject.SetActive (false);
			lblName.gameObject.SetActive (true);
			lblName.gameObject.transform.localPosition = new Vector3 (0, lblName.gameObject.transform.localPosition.y-10, lblName.gameObject.transform.localPosition.z);
		}
		int pos = 0;
		Utils.DestoryChilds (content.gameObject);
		for (int i = 0; i < info.item.Count; i++) {
			PrizeSample ps = info.item [i];

			if(win is GodsWarFinalRankWindow){
				GameObject obj = NGUITools.AddChild (content.gameObject, (win as GodsWarFinalRankWindow).goodsViewPrefab);
				GoodsView sc = obj.GetComponent<GoodsView> ();
				sc.init (ps);
				sc.fatherWindow = win;
				obj.transform.localScale = new Vector3 (0.9f, 0.9f, 1);
				obj.transform.localPosition = new Vector3 (pos * 100 - 10, 0, 0);
				pos++;
			}
				
			if(win is GodsWarIntegralRankAwardWindow){
				GameObject obj = NGUITools.AddChild (content.gameObject, (win as GodsWarIntegralRankAwardWindow).goodsViewPrefab);
				GoodsView sc = obj.GetComponent<GoodsView> ();
				sc.init (ps);
				sc.fatherWindow = win;
				obj.transform.localScale = new Vector3 (0.95f, 0.95f, 1);
				obj.transform.localPosition = new Vector3 (pos * 110 - 20, 0, 0);
				pos++;
			}
				

		}
	}

}
