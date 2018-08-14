using UnityEngine;
using System.Collections;

public class MissionAwardItem : MonoBehaviour
{

	/** 登陆奖励集--节省消耗不通过配置动态读取 */
	public ButtonTotalLoginAward[] awardButtons;
	/** 是否已领取图标 */
	public UISprite receivedSprite;
	/** 领取奖励按钮 */
	public ButtonBase awardButton;
	/** 条目标题 */
	public UILabel needStarTitle;
	/** 条目标题底框 */
	public UISprite todayTitleSprite;
	/** 预制体 */
	public GameObject goodsPerfab;
	/** 预制体集合点 */
	public GameObject offsexGoods;

	private ChapterAwardSample chapterAward;
	private bool isGet;
	
	public void updateAwardItem (ChapterAwardSample _chapterAward, WindowBase fatherWindow, bool _isGet,int myStar)
	{
		this.chapterAward = _chapterAward;
		this.isGet = _isGet;
		receivedSprite.gameObject.SetActive (false);
		awardButton.gameObject.SetActive (false);
		awardButton.fatherWindow = fatherWindow;

//		int count = chapterAward.prizes.Length;
//		if (count > awardButtons.Length)
//			count = awardButtons.Length;
//
//		for (int i = 0; i < count; i++) {
//			awardButtons [i].gameObject.SetActive (true);
//			awardButtons [i].cleanData ();
//			awardButtons [i].updateButton (chapterAward.prizes [i]);
//			awardButtons[i].fatherWindow = fatherWindow;
//		}

		if (offsexGoods.transform.childCount > 0) {
			Utils.DestoryChilds (offsexGoods);
		}
		if (chapterAward.prizes != null && chapterAward.prizes.Length > 0) {
			GoodsView obj;
			for (int i = 0; i < chapterAward.prizes.Length; i++) {
				obj = NGUITools.AddChild (offsexGoods, goodsPerfab).GetComponent<GoodsView> ();
				obj.init (chapterAward.prizes[i]);
				obj.fatherWindow = fatherWindow;
				obj.transform.localPosition = new Vector3(i * 120,0,0);
			}
		}
		
		if (isGet) {
			receivedSprite.gameObject.SetActive (true);
			awardButton.gameObject.SetActive (false);
		}
		else {
			awardButton.gameObject.SetActive (true);
			if (myStar < chapterAward.needStarNum) {
				awardButton.disableButton (true);
			} else {
				awardButton.disableButton (false);
			}
		}

		needStarTitle.text = chapterAward.needStarNum.ToString();	
	}

	public int getNeedStar ()
	{
		return chapterAward.needStarNum;
	}

	public bool getIsGet ()
	{
		return isGet;
	}
}
