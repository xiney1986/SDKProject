using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 图鉴窗口
 * yxl
 * */
public class CardPictureWindow : WindowBase
{ 
	public UIScrollView scrollView;
	public UIPanel panel;
	public UIGrid content;
	public GameObject itemPrefab;
	public UITexture downBackGround;
	public UITexture topBackGround;
	public RadioScrollBar scrollBar;
	public GameObject qualityEffectPoint;
	public CardPictureItem showItem;
	//public Transform cardGrid;
	public Transform cardViewPanel;
	/** 右箭头 */
	public UISprite rightArrow;
	/** 左箭头 */
	public UISprite leftArrow;

	public GameObject buttonGetWay;
	float cardGridItemWidth;
	List<Card> cardList;
	List<CardSample> sampleList;
	int lastPage = -1;

	protected override void begin ()
	{
		base.begin ();
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.BACKGROUNDPATH + "card_downBackGround", downBackGround);
		MaskWindow.UnlockUI ();
	}

	void Start ()
	{
		cardGridItemWidth = content.GetComponent<UIGrid> ().cellWidth;
		content.GetComponent<UICenterOnChild> ().onFinished += OnDragFinished;
	}

	private void OnDragFinished ()
	{
		if (sampleList != null) {
			int index = (int)(Mathf.Abs (cardViewPanel.localPosition.x) / cardGridItemWidth + 1) - 1;
			if (index >= 0 && index <= sampleList.Count - 1) {
				showItem.init (cardList [index], sampleList [index], this);
			}
			if (index == 0) {
				leftArrow.alpha = 0;
			} else {
				leftArrow.alpha = 1;
			}
			
			if (index >= sampleList.Count - 1) {
				rightArrow.alpha = 0;
			} else {
				rightArrow.alpha = 1;
			}
		}
	}

	public void init (List<CardSample> list, int defaultIndex)
	{
		/** 每次点击，将图鉴的模版缓存下来，以便在跳转界(PictureTipsWindow)面使用 */
		PictureManagerment.Instance.currentSample = PictureManagerment.Instance.mapPic [list [0].evolveSid];
		sampleList = list;
		cardList = new List<Card> ();
		for (int i = 0; i < list.Count; i++) { 
			Card c = CardManagerment.Instance.createCard (list [i].sid);
			if (list [i].evoStarLevel > 0) {
				c = CardManagerment.Instance.createCardByEvoLevel (c, list [i].evoStarLevel);
			}
			c.setLevel (list [i].maxLevel);
			cardList.Add (c);

		}
		setTitle (list [0].name);
		/**
		if (!EvolutionManagerment.Instance.isMaxEvoLevel (cardList [cardList.Count - 1])) {
			Card c = CardManagerment.Instance.createCardByEvoLevel(cardList [0],EvolutionManagerment.Instance.getEvoInfoByType(cardList[0]).getMaxEvoLevel());
			c.setLevel(c.getMaxLevel());
			cardList.Add(c);
		}
*/
		scrollBar.pageCount = cardList.Count;
		initItems ();

		SpringPanel.Begin (scrollView.gameObject, new Vector3 (-640 * defaultIndex, 0, 0), 100);
		if (defaultIndex == 0) {
			leftArrow.alpha = 0;
		} else {
			leftArrow.alpha = 1;
		}
		
		if (defaultIndex >= sampleList.Count - 1) {
			rightArrow.alpha = 0;
		} else {
			rightArrow.alpha = 1;
		}
		//jordenwu
		initButton ();

	}

	private void initButton(){
		if (sampleList [0].qualityId == 4 ||sampleList [0].qualityId == 5 ) {
			buttonGetWay.SetActive(true);
		} else {
			buttonGetWay.SetActive(false);
		}
	}

	private void initItems ()
	{
		for (int i = 0; i < cardList.Count; i++) {
			GameObject obj = NGUITools.AddChild (content.gameObject, itemPrefab);
			obj.SetActive (true);
			obj.name = i.ToString ();
			CardPictureItem item = obj.GetComponent<CardPictureItem> ();
			item.initJustImage (cardList [i], sampleList [i], this);

		}
		content.Reposition ();
		if (cardList.Count > 0) {
			UpdateQualityEF (cardList [0]);
			int jobId = cardList [0].getJob ();
			//属性界面“力”系背景（力系和毒系职业用）
			if (jobId == 1 || jobId == 4) {
				ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.BACKGROUNDPATH + "card_topBackGround_1", topBackGround);
			}
			//属性界面“敏”系背景（反和敏职业用）
			else if (jobId == 3 || jobId == 5) {
				ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.BACKGROUNDPATH + "card_topBackGround_2", topBackGround);
			}
			//属性界面“魔”系背景（魔和辅职业用）
			else {
				ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.BACKGROUNDPATH + "card_topBackGround_3", topBackGround);
			}
			showItem.init (cardList [0], sampleList [0], this);
		}
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			if(SevenDaysHappyManagement.Instance.getIsSevendayshappyHelpWin())
			{
				SevenDaysHappyManagement.Instance.setIsSevendayshappyHelpWin(false);
				if(SevenDaysHappyManagement.Instance.getHelpObj() != null)
				{
					SevenDaysHappyManagement.Instance.getHelpObj().SetActive(true);
				}
			}
			finishWindow ();
		} else if (gameObj.name == "ButtonGetWay") {
			UiManager.Instance.openWindow<PictureTipsWindow>();
		}
	}

	private void closeQualityEffect (GameObject obj)
	{
		if (obj.activeSelf)
			obj.SetActive (false);
	}
	
	private void openQualityEffect (GameObject obj)
	{
		if (!obj.activeSelf)
			obj.SetActive (true);
	}
	// jordenwu::展示卡片质量效果 蓝色 黄色 紫色
	public void UpdateQualityEF (Card showCard)
	{
		if (showCard == null) {
			qualityEffectPoint.SetActive(false);
			return;
		}
		bool isShow=showEffectByQuality(showCard.getQualityId ());
		if (isShow) {
			qualityEffectPoint.SetActive (true);
		}
		else{
			if(qualityEffectPoint!=null&&qualityEffectPoint.transform.childCount>0)
				Utils.RemoveAllChild (qualityEffectPoint.transform);
			qualityEffectPoint.SetActive(false);
		}
	}
	/** 显示卡片本身品质  */
	public bool showEffectByQuality (int qualityId)
	{
		if (qualityEffectPoint==null)
			return false;
		if (qualityId < QualityType.GOOD)
			return false;
		if(qualityEffectPoint.transform.childCount>0)
			Utils.RemoveAllChild (qualityEffectPoint.transform);
		EffectManager.Instance.CreateEffect (qualityEffectPoint.transform,"Effect/UiEffect/CardQualityEffect"+qualityId);
		return true;
	}
}
