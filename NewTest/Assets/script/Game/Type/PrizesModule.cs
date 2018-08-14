using UnityEngine;
using System.Collections;

//奖品组件 
public class PrizesModule: ButtonBase
{  
	public UITexture itemImage;//卡片显示的主图
	public Vector2 propImageSide = new Vector2 (160, 160);
	public Vector2 equipImageSide = new Vector2 (160, 160);
	public Vector2 cardImageSide = new Vector2 (192, 192);
	public UISprite backgroud;//道具，装备等显示的背景框
	public Vector2 propBackgroudSide = new Vector2 (160, 160);
	public Vector2 equipBackgroudSide = new Vector2 (160, 160);
	public Vector2 cardBackgroudSide = new Vector2 (192, 192);
	public UILabel num;//数量
	public	PrizeSample prize;
	private CallBack back;
    public GameObject starPrefab;
	 
	public void initPrize (PrizeSample _prize, CallBack _back, WindowBase win)
	{
		fatherWindow = win;
		this.back = _back;
		updateButton (_prize);
	}
	 
	public override void DoClickEvent ()
	{
		if (prize != null) {
			clickButton (prize);
		}
	}

	public void cleanData ()
	{
		if (itemImage != null)
			itemImage.gameObject.SetActive (false);

		if (backgroud != null)
			backgroud.gameObject.SetActive (false);
 
		if (num != null)
			num.text = "";
		back = null;
		fatherWindow = null;
	}
	
	private void clickButton (PrizeSample prize)
	{
		switch (prize.type) {
		case PrizeType.PRIZE_BEAST:
			Card beast = CardManagerment.Instance.createCard (prize.pSid);
			CardBookWindow.Show(beast, CardBookWindow.OTHER, null);
			break;
		case PrizeType.PRIZE_CARD:
			Card card = CardManagerment.Instance.createCard (prize.pSid);
			CardBookWindow.Show(card, CardBookWindow.OTHER, null);
			break;
		case PrizeType.PRIZE_EQUIPMENT:
			Equip equip = EquipManagerment.Instance.createEquip (prize.pSid);	
			UiManager.Instance.openWindow <EquipAttrWindow>((win)=>{
				win.Initialize (equip, EquipAttrWindow.OTHER, back);
			});
			break;
		case PrizeType.PRIZE_MONEY:
			//暂时处理，游戏币也需要显示详情
			MaskWindow.UnlockUI();
			break;
		case PrizeType.PRIZE_PROP:
			Prop prop = PropManagerment.Instance.createProp (prize.pSid);
			UiManager.Instance.openDialogWindow<PropAttrWindow>((win)=>{
				win.Initialize (prop);
			});
			break;
		case PrizeType.PRIZE_RMB:
			//暂时处理，软妹币也需要显示详情
			MaskWindow.UnlockUI();
			break;
		}
		//	back = null;
	}
 
	//设置创建按钮信息
	private void updateButton (PrizeSample _prize)
	{
		if (_prize == null) {
			return;
		} else {
			prize = _prize;
 
			if (backgroud != null) {
				backgroud.gameObject.SetActive (false);
			}

			if (itemImage != null) {
				itemImage.gameObject.SetActive (false);
			}
			if (num != null) {
				num.gameObject.SetActive (true);
				num.text = "X " + prize.num.ToString ();
				num.transform.localPosition=new Vector3(num.transform.localPosition.x,-66,num.transform.localPosition.z);
			}
			switch (prize.type) {
			//召唤兽
			case PrizeType.PRIZE_BEAST:
				Card beast = CardManagerment.Instance.createCard (prize.pSid);
                if (starPrefab != null) {
                    for (int i = 0; i < starPrefab.transform.childCount; i++) {
                        starPrefab.transform.GetChild(i).gameObject.SetActive(false);
                    }
                    for (int i = 0; i < CardSampleManager.Instance.getStarLevel(beast.sid); i++) {
                        starPrefab.transform.GetChild(i).gameObject.SetActive(true);
                    }
                }
				if (itemImage != null) {
                    if(CommandConfigManager.Instance.getNvShenClothType() == 0)
					    ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + beast.getImageID ()+ "c", itemImage);
                    else ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.CARDIMAGEPATH + beast.getImageID(), itemImage);
					itemImage.width = (int)cardImageSide.x;
					itemImage.height = (int)cardImageSide.y;
				}
				if (backgroud != null) {
					backgroud.gameObject.SetActive (true);
					backgroud.spriteName = QualityManagerment.qualityIDToBackGround (beast.getQualityId ());
					backgroud.width = (int)cardBackgroudSide.x;
					backgroud.height = (int)cardBackgroudSide.y;
				}
				break;
			//卡片
			case PrizeType.PRIZE_CARD:
				Card card = CardManagerment.Instance.createCard (prize.pSid);
                if (starPrefab != null) {
                    for (int i = 0; i < starPrefab.transform.childCount; i++) {
                        starPrefab.transform.GetChild(i).gameObject.SetActive(false);
                    }
                    for (int i = 0; i < CardSampleManager.Instance.getStarLevel(card.sid); i++) {
                        starPrefab.transform.GetChild(i).gameObject.SetActive(true);
                    }
                }
                if (itemImage != null) {
                    ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.CARDIMAGEPATH + card.getImageID(), itemImage);
                    itemImage.width = (int)cardImageSide.x;
                    itemImage.height = (int)cardImageSide.y;
                }
				if (backgroud != null) {
					backgroud.gameObject.SetActive (true);
					backgroud.spriteName = QualityManagerment.qualityIDToBackGround (card.getQualityId ());
					backgroud.width = (int)cardBackgroudSide.x;
					backgroud.height = (int)cardBackgroudSide.y;
				}
				if (num != null) {
					num.transform.localPosition=new Vector3(num.transform.localPosition.x,-92,num.transform.localPosition.z);
				}
				break;
			//装备
			case PrizeType.PRIZE_EQUIPMENT:
				Equip equip = EquipManagerment.Instance.createEquip (prize.pSid);
				if (itemImage != null) {
					ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + equip.getIconId (), itemImage);
					itemImage.width = (int)equipImageSide.x;
					itemImage.height = (int)equipImageSide.y;
				}
				if (backgroud != null) {
					backgroud.gameObject.SetActive (true);
					backgroud.spriteName = QualityManagerment.qualityIDToIconSpriteName  (equip.getQualityId ());
					backgroud.width = (int)equipBackgroudSide.x;
					backgroud.height = (int)equipBackgroudSide.y;
				}
				break;
			case PrizeType.PRIZE_MONEY:
				if (itemImage != null) {
					backgroud.gameObject.SetActive (true);
					backgroud.spriteName = QualityManagerment.qualityIDToIconSpriteName  (5);
					backgroud.width = (int)equipBackgroudSide.x;
					backgroud.height = (int)equipBackgroudSide.y;
					ResourcesManager.Instance.LoadAssetBundleTexture (prize.getIconPath (), itemImage);
					itemImage.width = (int)propImageSide.x;
					itemImage.height = (int)propImageSide.y;
				}
				break;
			//道具
			case PrizeType.PRIZE_PROP:
				Prop prop = PropManagerment.Instance.createProp (prize.pSid);
				if (itemImage != null) {
					ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + prop.getIconId (), itemImage);
					itemImage.width = (int)propImageSide.x;
					itemImage.height = (int)propImageSide.y;
				}
				if (backgroud != null) {
					backgroud.gameObject.SetActive (true);
					backgroud.spriteName =  QualityManagerment .qualityIDToIconSpriteName (prop.getQualityId ());
					backgroud.width = (int)propBackgroudSide.x;
					backgroud.height = (int)propBackgroudSide.y;
				}
				break;
			case PrizeType.PRIZE_RMB:
				if (itemImage != null) {
					backgroud.gameObject.SetActive (true);
					backgroud.spriteName = QualityManagerment.qualityIDToIconSpriteName  (5);
					backgroud.width = (int)equipBackgroudSide.x;
					backgroud.height = (int)equipBackgroudSide.y;
					ResourcesManager.Instance.LoadAssetBundleTexture (prize.getIconPath (), itemImage);
					itemImage.width = (int)propImageSide.x;
					itemImage.height = (int)propImageSide.y;
				}
				break;
			case PrizeType.PRIZE_EXP:
				if (itemImage != null) {
					ResourcesManager.Instance.LoadAssetBundleTexture (prize.getIconPath (), itemImage);
					itemImage.width = (int)propImageSide.x;
					itemImage.height = (int)propImageSide.y;
				}
				break;
			}
		}
	}
}
