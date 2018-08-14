using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 图鉴窗口
/// </summary>
public class PictureWindow :  WindowBase {
	[HideInInspector]
	public bool	needReload = true; 
	/** 卡片面板 */
	public SampleDynamicContent  dynamicContent;
	/** 普通卡片面板 */
	public TapContentBase tabContent;
	public GameObject itemPrefab;
	/** 左箭头 */
	public UISprite leftArrow;
	/** 右箭头 */
	public UISprite rightArrow;
	/** 页数显示 */
	public UILabel pageLabel;
	/** 当前激活的图鉴页 */
	private PicturePage activeShowPage;
	/** 卡片数据缓存 */
	private List<List<Card>> [] cards = new List<List<Card>>[6];
	/** 每页的卡片数量 */
	private const int pagesize = 16;
	/** 当前Content的下标 */
	private int currentContentIndex = QualityType.LEGEND -1;
	protected override void begin () {
		if (!isAwakeformHide) {
			tabContent.changeTapPage(tabContent.getTapButtonByIndex(currentContentIndex));
			dynamicContent.callbackUpdateEach = updatePage;
			dynamicContent.onCenterItem = updateActivePage;
			UpdateContentView (QualityType.LEGEND);
		}
		MaskWindow.UnlockUI ();
	}
	
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow ();
		}
	}
	
	public override void tapButtonEventBase (GameObject gameObj, bool enable) {
		if (!enable)
			return;
		base.tapButtonEventBase (gameObj, enable);
		if (enable) {
			switch (gameObj.name) {
				case "COMMON"://普通 
					UpdateContentView (QualityType.COMMON);
					break;
				case "EXCELLENT"://优秀
					UpdateContentView (QualityType.EXCELLENT);
					break;
				case "GOOD"://精良
					UpdateContentView (QualityType.GOOD);
					break;
				case "EPIC"://史诗
					UpdateContentView (QualityType.EPIC);
					break;
				case "LEGEND"://传奇
					UpdateContentView (QualityType.LEGEND);
					break;
                case "MYTH"://神话
                    UpdateContentView(QualityType.MYTH);
                    //Debug.LogError("神话卡片");
                    break;
                    
			}
		}
	}

	/// <summary>
	/// 根据品质类型显示图鉴
	/// </summary>
	void UpdateContentView(int qualityType){
		currentContentIndex = qualityType-1 ;
		if(cards[currentContentIndex] == null)
			cards[currentContentIndex] = SortByQuality (qualityType);
		dynamicContent.maxCount = cards [currentContentIndex].Count;
		dynamicContent.transform.parent.gameObject.SetActive (true);
		dynamicContent.init ();
	}

	/// <summary>
	/// 更新当前页
	/// </summary>
	void updatePage (GameObject obj)
	{
		//更新当前显示的ShowItem;
		activeShowPage = dynamicContent.getCenterObj ().GetComponent<PicturePage> ();
		PicturePage bookitem = obj.GetComponent<PicturePage> ();
		int index = StringKit.toInt (obj.name) - 1;
		//不够3页.隐藏
		if (cards[currentContentIndex] == null || index >= cards[currentContentIndex].Count || cards[currentContentIndex] [index] == null) {
			return;
		}
		
		if (bookitem.getCards() != cards[currentContentIndex][index]) {
			bookitem.updatePage (cards[currentContentIndex][index]);
		}
	
		pageLabel.text = (index +1).ToString() + "/" + dynamicContent.maxCount;
	}

	
	void updateActivePage (GameObject obj)
	{
		//更新箭头
		int index = StringKit.toInt (obj.name) - 1;		
		if (cards[currentContentIndex].Count == 1) {
			leftArrow.gameObject.SetActive (false);
			rightArrow.gameObject.SetActive (false);
		} else if (index == 0) {
			leftArrow.gameObject.SetActive (false);
			rightArrow.gameObject.SetActive (true);
		} else if (index == cards[currentContentIndex].Count - 1) {
			leftArrow.gameObject.SetActive (true);
			rightArrow.gameObject.SetActive (false);
		} else {
			leftArrow.gameObject.SetActive (true);
			rightArrow.gameObject.SetActive (true);
		}
		pageLabel.text = (index +1).ToString() + "/" + dynamicContent.maxCount;
	}

	/// <summary>
	/// 响应图鉴中卡片点击
	/// </summary>
	/// <param name="item">Item.</param>
	public void OnItemClick (Card item) {
		MaskWindow.LockUI ();
		UiManager.Instance.openWindow<CardPictureWindow> (
			(window) => {
			int type = PictureManagerment.Instance.mapCard [item];
			window.init (PictureManagerment.Instance.mapType [type], 0);}
		);
	}

	/// <summary>
	/// 通过品质获取图鉴内容
	/// </summary>
	List<List<Card>> SortByQuality (int  qualityType)
	{
		SortCondition sc = new SortCondition ();
		Condition cd = new Condition (SortType.QUALITY);
		cd.addCondition (qualityType);
		sc.addSiftCondition (cd);
		ArrayList list = SortManagerment.Instance.cardSort (PictureManagerment.Instance.cardList, sc);
        list.Sort(new PictureCompare());
        int pageCount = Mathf.CeilToInt((float)(list.Count * 1f / pagesize));
		List<List<Card>> tempCards = new List<List<Card>> ();
		for (int i =0; i<pageCount; i++) {
			List<Card> tempList = new List<Card> ();
			int startIndex = i * pagesize;
            for (int j = startIndex; j < (startIndex + pagesize) && j < list.Count; j++)
            {
                Card card = list[j] as Card;
				tempList.Add (card);
			}
			tempCards.Add (tempList);
		}      
		return tempCards;
	}
	public override void DoDisable () {
		base.DoDisable ();
	}
}

public class PictureCompare : IComparer
{
	public int Compare (object a, object b)
	{
        
		Card itemA = a as Card;
		Card itemB = b as Card;
        int card1 = CardSampleManager.Instance.getRoleSampleBySid(itemA.sid).sFlagLevel;
        int card2 = CardSampleManager.Instance.getRoleSampleBySid(itemB.sid).sFlagLevel;

        if (card1 > card2)
            return -1;
        else if (card1 < card2)
            return 1;
        else
            return 0;
	}
}