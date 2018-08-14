using UnityEngine;
using System.Collections;

public class ButtonExchange : ButtonBase
{
	/**女神屏风上的加锁图片 */
	public UITexture luckImage;
	public UITexture Image;
	public UISprite icon_back;
	public object item; 
	public int type = 1;// 1召唤兽兑换模式,2兑换模式
	public const int BEASTSUMMON = 1;
	public const int EXCHANGE = 2;
	Vector3 bigPic = new Vector3 (158, 158, 0);
	Vector3 bigPicBg = new Vector3 (120, 150, 0);
	Vector3 smallPic = new Vector3 (52, 52, 0);
	Vector3 iconPic = new Vector3 (74, 74, 0);
	public UILabel haveValue;
	public UILabel needValue;
	 
	public void cleanData ()
	{
		Image.gameObject.SetActive (false);
		icon_back.gameObject.SetActive (false);
		item = null;
		textLabel.text = "";
	}

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
//		if(GuideManager.Instance.guideSid == GuideGlobal.SPECIALSID4)
//		{
//			GuideManager.Instance.doGuide(); 
//			GuideManager.Instance.closeGuideMask();
//		}
		if (item == null) {
			MaskWindow.UnlockUI ();
			return;
		} else if (item.GetType () == typeof(Equip)) {
			UiManager.Instance.openWindow<EquipAttrWindow> ((win) => {
				win.Initialize (item as Equip, EquipAttrWindow.OTHER, null);
			});
			return;
		} else if (item.GetType () == typeof(Prop)) {
//			if(GuideManager.Instance.guideSid == GuideGlobal.SPECIALSID16 || GuideManager.Instance.guideSid == GuideGlobal.SPECIALSID17)
//				GuideManager.Instance.doGuide(); 
			UiManager.Instance.openDialogWindow<PropAttrWindow> ((win) => {
				win.Initialize (item as Prop);
			});
			return;
		} else if (item.GetType () == typeof(Card)) {
			CardBookWindow.Show (item as Card, CardBookWindow.OTHER, null);
			/*指定窗口可能会导致活动热更的兑换无法点击，卡死
			if (fatherWindow.GetType () == typeof(BeastSummonWindow)) {
				CardBookWindow.Show (item as Card, CardBookWindow.OTHER, null);

			}
			if (fatherWindow.GetType () == typeof(ExChangeWindow)) {
				CardBookWindow.Show (item as Card, CardBookWindow.OTHER, null);
		
			}
			if (fatherWindow.GetType () == typeof(NoticeActivityExchangeContent)) {
				CardBookWindow.Show (item as Card, CardBookWindow.OTHER, null);
			}
			*/
			return;
		} else if(item.GetType() == typeof(int))
		{
			PrizeSample prize = new PrizeSample ();
			prize.type = (int)item;
			//暂时只处理了功勋的 其他类型需要显示自己加
			if(prize.type == PrizeType.PRIZE_MERIT)
				prize.num = UserManager.Instance.self.merit.ToString ();
			UiManager.Instance.openDialogWindow <PropAttrWindow> (
				(winProp) => {
				winProp.Initialize (prize);
			});
		}
		else {
			MaskWindow.UnlockUI();
		}
	}
	

	
	//用于目标兑换物品的按钮更新
	public void updateButton (ExchangeSample sample, int type)
	{
		cleanData();
		Image.gameObject.SetActive (true);
		this.type = type;

		
		
		if (sample.type == PrizeType.PRIZE_CARD) {
			Card showCard = CardManagerment.Instance.createCard (sample.exchangeSid);
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + showCard.getImageID (), Image);
			Image.width = (int)bigPic.x;
			Image.height = (int)bigPic.y;
			icon_back.gameObject.SetActive (true);
			icon_back.width = (int)bigPicBg .x ;
			icon_back.height = (int)bigPicBg .y;
			icon_back.spriteName = QualityManagerment.qualityIDToBackGround (showCard.getQualityId ());
			setNormalSprite(icon_back.spriteName);
			textLabel.text = "x" + sample.num;
			item = showCard;  
			
		} else if (sample.type == PrizeType.PRIZE_EQUIPMENT) {
			Equip showEquip = EquipManagerment.Instance.createEquip (sample.exchangeSid);
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + showEquip.getIconId (), Image);		
			Image.width = (int)smallPic .x;
			Image.height = (int)smallPic.y;
			icon_back.gameObject.SetActive (true);
			icon_back.width = (int)iconPic .x ;
			icon_back.height = (int)iconPic .y ;
			icon_back.spriteName = QualityManagerment.qualityIDToIconSpriteName (showEquip.getQualityId());
			setNormalSprite(icon_back.spriteName);
			textLabel.text = "x" + sample.num;
			item = showEquip;
			 
		} else if (sample.type == PrizeType.PRIZE_PROP) {
			Prop showProp = PropManagerment.Instance.createProp (sample.exchangeSid);
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + showProp.getIconId (), Image);		
			Image.width = (int)smallPic .x;
			Image.height = (int)smallPic.y;
			icon_back.gameObject.SetActive (true);
			icon_back.width = (int)iconPic .x ;
			icon_back.height = (int)iconPic .y ;
			icon_back.spriteName = QualityManagerment.qualityIDToIconSpriteName (showProp.getQualityId());
			setNormalSprite(icon_back.spriteName);
			textLabel.text = "x" + sample.num;
			item = showProp; 
		}
		
	}
			
	public void updateButton (ExchangeCondition condition, int type)
	{
		cleanData();
		Image.gameObject.SetActive (true);
		this.type = type;
		
		if (condition.costType == PrizeType.PRIZE_CARD) {
			Card showCard = CardManagerment.Instance.createCard (condition.costSid);
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + showCard.getImageID () , Image);
			
			if (type == EXCHANGE) {
				Image.width = (int)bigPic.x;
				Image.height = (int)bigPic.y;
			} else {
				Image.width = (int)bigPic  .x;
				Image.height = (int)bigPic.y;
			}
			icon_back.gameObject.SetActive (true);
			icon_back.width = (int)bigPicBg .x;
			icon_back.height = (int)bigPicBg .y ;
			icon_back.spriteName = QualityManagerment.qualityIDToBackGround (showCard.getQualityId ());
			setNormalSprite(icon_back.spriteName);
			ArrayList list = StorageManagerment.Instance.getNoUseRolesBySid (condition.costSid);
			
			if (list.Count >= condition.num)
				textLabel.color = Color.green;
			else
				textLabel.color = Color.red;
			
			textLabel.text = list.Count + "/" + condition.num;
			item = showCard;  
			
		} else if (condition.costType == PrizeType.PRIZE_EQUIPMENT) {
			Equip showEquip = EquipManagerment.Instance.createEquip (condition.costSid);
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + showEquip.getIconId (), Image);		
			Image.width = (int)smallPic .x;
			Image.height = (int)smallPic.y;
			icon_back.gameObject.SetActive (true);
			icon_back.width = (int)iconPic .x ;
			icon_back.height = (int)iconPic .y ;
			icon_back.spriteName = QualityManagerment.qualityIDToIconSpriteName (showEquip.getQualityId());
			setNormalSprite(icon_back.spriteName);
			ArrayList list = StorageManagerment.Instance.getEquipsBySid (condition.costSid);
			textLabel.text = list.Count + "/" + condition.num;
			
			if (list.Count >= condition.num)
				textLabel.color = Color.green;
			else
				textLabel.color = Color.red;
			item = showEquip;
			 

		} else if (condition.costType == PrizeType.PRIZE_PROP) {
			Prop showProp = PropManagerment.Instance.createProp (condition.costSid);
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + showProp.getIconId (), Image);	
			Image.width = (int)smallPic .x;
			Image.height = (int)smallPic.y;
			icon_back.gameObject.SetActive (true);
			icon_back.width = (int)iconPic .x ;
			icon_back.height = (int)iconPic .y ;
			icon_back.spriteName = QualityManagerment.qualityIDToIconSpriteName (showProp.getQualityId());
			setNormalSprite(icon_back.spriteName);
			ArrayList list = StorageManagerment.Instance.getPropsBySid (condition.costSid);
			
			int count = 0;
			foreach (Prop each in list) {
				count += each.getNum();
			}

			textLabel.text = count + "/" + condition.num;
			 
			if (count >= condition.num)
				textLabel.color = Color.green;
			else
				textLabel.color = Color.red;
				
			item = showProp; 
		} else if (condition.costType == PrizeType.PRIZE_MONEY) {
			ResourcesManager.Instance.LoadAssetBundleTexture (constResourcesPath.MONEY_ICONPATH, Image);	
			Image.width = (int)smallPic .x;
			Image.height = (int)smallPic.y;
			icon_back.width = (int)iconPic .x ;
			icon_back.height = (int)iconPic .y ;
			icon_back.gameObject.SetActive (true);
			icon_back.spriteName = QualityManagerment.qualityIDToIconSpriteName (5);
			setNormalSprite(icon_back.spriteName);
			int count = UserManager.Instance.self.getMoney ();
			textLabel.text = "x" + condition.num;
			 
			if (count >= condition.num)
				textLabel.color = Color.green;
			else
				textLabel.color = Color.red;
			
			item = PrizeType.PRIZE_MONEY; 
		}else if (condition.costType == PrizeType.PRIZE_RMB) {
			ResourcesManager.Instance.LoadAssetBundleTexture (constResourcesPath.RMB_ICONPATH, Image);	
			Image.width = (int)smallPic .x;
			Image.height = (int)smallPic.y;
			icon_back.width = (int)iconPic .x ;
			icon_back.height = (int)iconPic .y ;
			icon_back.gameObject.SetActive (true);
			int count = UserManager.Instance.self.getRMB ();
			textLabel.text = "x" + condition.num;
			
			if (count >= condition.num)
				textLabel.color = Color.green;
			else
				textLabel.color = Color.red;
			
			item = PrizeType.PRIZE_RMB; 
		}else if (condition.costType == PrizeType.PRIZE_MERIT) {
			ResourcesManager.Instance.LoadAssetBundleTexture (constResourcesPath.MERIT_ICONPATH, Image);	
			Image.width = (int)smallPic .x;
			Image.height = (int)smallPic.y;
			icon_back.width = (int)iconPic .x ;
			icon_back.height = (int)iconPic .y ;
			icon_back.gameObject.SetActive (true);
			icon_back.spriteName = QualityManagerment.qualityIDToIconSpriteName (5);
			setNormalSprite(icon_back.spriteName);
			int count = UserManager.Instance.self.merit;
			textLabel.text = "x" + condition.num;
			
			if (count >= condition.num)
				textLabel.color = Color.green;
			else
				textLabel.color = Color.red;
			
			item = PrizeType.PRIZE_MERIT; 
		}
	}
}
