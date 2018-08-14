using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 筛选容器内容编辑
 * @author 汤琦
 * */
public class ContentPrizeEdit : ContentBase
{
	public ButtonBase confirmButton;
	public ButtonBase detailButton;
	public Transform point1;//按钮坐标 左
	public Transform point2;//按钮坐标 右
	public Transform point3;//按钮坐标 中
	public UISprite leftShow;
	public UISprite rightShow;
	public BoxCollider silde;
	public UILabel title;
	public UILabel number;
	private LuckyDrawResults ldResult;
	public int currentIndex;

	public void initPrize (LuckyDrawResults result)
	{
		ldResult = result;
	}
	
	public override void CreateButton (int index, GameObject page, int buttonIndex)
	{
		base.CreateButton (index, page, buttonIndex);
		if (index == -1)
			return;
		
		ButtonPrizeChoose button = page.GetComponent<ButtonPrizeChoose> ();
		
		setCreatButton (button, ldResult.getSinglePrizesByQuality(ldResult.getSinglePrizes ()) [index]);
	}

	public override void updateActive (GameObject obj, int pageNUm)
	{
		updateContent (activeGameObj, pageNUm);
		currentIndex = pageNUm;
		rightShow.gameObject.SetActive (true);
		leftShow.gameObject.SetActive (true);
		if (ldResult.getSinglePrizesByQuality(ldResult.getSinglePrizes ()).Count == 1) { 
			rightShow.gameObject.SetActive (false);
			leftShow.gameObject.SetActive (false);
			silde.enabled = false;
		} else if (pageNUm == 1)
			leftShow.gameObject.SetActive (false);
		else if (pageNUm == ldResult.getSinglePrizesByQuality(ldResult.getSinglePrizes ()).Count)
			rightShow.gameObject.SetActive (false);
		setShowInfo (ldResult.getSinglePrizesByQuality(ldResult.getSinglePrizes ()) [pageNUm - 1]);
//		title.text = titleShow (ldResult.getSinglePrizesByQuality(ldResult.getSinglePrizes ()) [pageNUm - 1].sourceType);
		
	}
	//设置创建按钮信息
	private void setCreatButton (ButtonPrizeChoose button, SinglePrize prize)
	{
		switch (prize.type) {
		case LuckyDrawPrize.TYPE_CARD:
			Card card = StorageManagerment.Instance.getRole (prize.uid);
			if (card == null) {
				card = CardManagerment.Instance.createCard (prize.sid);
			}
			//ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.BACKGROUNDPATH + "backGround_9" , button.buttom);
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + card.getImageID () , button.prizeImage);
			button.quality.spriteName = QualityManagerment.qualityIDToStringByBG (card.getQualityId ());
			//add 
			button.nameLabel.text=getQualityColorName(card.getQualityId(),card.getName());
			break;
		case LuckyDrawPrize.TYPE_EQUIP:
			Equip equip = StorageManagerment.Instance.getEquip (prize.uid);
			if (equip == null) {
				equip = EquipManagerment.Instance.createEquip ("", prize.sid, 0, 0,0);
			}
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + equip.getIconId (), button.prizeImage);
//			button.quality.spriteName = QualityManagerment.qualityIDToStringByBG (equip.getQualityId ());
			//add 
			button.nameLabel.text=equip.getName();
			break;
		default:
			Prop prop = StorageManagerment.Instance.getProp (prize.sid);
			if (prop == null) {
				prop = PropManagerment.Instance.createProp (prize.sid, 1);
			}
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + prop.getIconId (), button.prizeImage);
//			button.quality.spriteName = QualityManagerment.qualityIDToStringByBG (prop.getQualityId ());
			button.nameLabel.text=prop.getName();
			break;
		}
	}
	private string getQualityColorName(int qualityID,string name)
	{
		return QualityManagerment.getQualityColor(qualityID) +name+"[-]";
	}
	//设置展示信息
	private void setShowInfo (SinglePrize prize)
	{
		switch (prize.type) {
		case LuckyDrawPrize.TYPE_CARD:
			confirmButton.transform.position = point2.transform.position;
			detailButton.transform.position = point1.transform.position;
			detailButton.gameObject.SetActive (true);
			Card card = StorageManagerment.Instance.getRole (prize.uid);
			if (card == null) {
				card = CardManagerment.Instance.createCard (prize.sid);
			}
			number.text = numberShow (prize.type, card.sid);
			break;
		case LuckyDrawPrize.TYPE_EQUIP:
			confirmButton.transform.position = point2.transform.position;
			detailButton.transform.position = point1.transform.position;
			detailButton.gameObject.SetActive (true);
			Equip equip = StorageManagerment.Instance.getEquip (prize.uid);
			if (equip == null) {
				equip = EquipManagerment.Instance.createEquip ("", prize.sid, 0, 0,0);
			}
			number.text = numberShow (prize.type, equip.sid);
			break;
		default:
			confirmButton.transform.position = point3.transform.position;
			detailButton.gameObject.SetActive (false);
			Prop prop = StorageManagerment.Instance.getProp (prize.sid);
			if (prop == null) {
				prop = PropManagerment.Instance.createProp (prize.sid, 1);
			}
			number.text = numberShow (prize.type, prop.sid);
			break;
		}
	}
	
	private string titleShow (string type)
	{
		string str = string.Empty;
		switch (type) {
		case LuckyDrawPrize.PRIZE:
			str = LanguageConfigManager.Instance.getLanguage ("s0027");
			break;
		case LuckyDrawPrize.GIFT:
			str = LanguageConfigManager.Instance.getLanguage ("s0028");
			break;
		}
		return str;
	}

	private string numberShow (string type, int sid)
	{

		string str = string.Empty;
		switch (type) {
		case LuckyDrawPrize.TYPE_CARD:
			str = LanguageConfigManager.Instance.getLanguage ("s0029") + ":" + StorageManagerment.Instance.getAllRole ().Count + "/" + StorageManagerment.Instance.getRoleStorageMaxSpace ();
			break;
		case LuckyDrawPrize.TYPE_TOOL:
			str = PropSampleManager.Instance.getPropSampleBySid (sid).name + ":" + StorageManagerment.Instance.getProp (sid).getNum();
			break;
		case LuckyDrawPrize.TYPE_MONEY:
			str = LanguageConfigManager.Instance.getLanguage ("s0030") + ":" + UserManager.Instance.self.getMoney ();
			break;
		case LuckyDrawPrize.TYPE_RMB:
			str = LanguageConfigManager.Instance.getLanguage ("s0031") + ":" + UserManager.Instance.self.getRMB ();
			break;
		case LuckyDrawPrize.TYPE_EQUIP:
			str = EquipmentSampleManager.Instance.getEquipSampleBySid (sid).name;
			break;
		}
		return str;
	}
	 
}
//奖励类型
public class SinglePrize
{
	public string type;
	public string sourceType;
	public int num;
	public int sid;
	public string uid;
}
