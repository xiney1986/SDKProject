using UnityEngine;
using System.Collections;

public class ButtonActivityPrizeView : ButtonBase
{
	
	public UITexture Image;
	public UISprite icon_back;
	public object item;
	Vector3 	bigPic = new Vector3 (276, 276, 0);
	Vector3 	smallPic = new Vector3 (160, 160, 0);

	public void cleanData ()
	{
		Image.gameObject.SetActive(false);
		icon_back.gameObject.SetActive(false);
		item = null;
		textLabel.text = "";
	}

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		
		if (item == null)
			return;
		
		if (item.GetType () == typeof(Equip)) {
			UiManager.Instance.openWindow <EquipAttrWindow>((win1)=>{
				win1.Initialize (item as Equip,EquipAttrWindow.OTHER,viewCallBack);
				fatherWindow.hideWindow();
			});
			return;
		}
		if (item.GetType () == typeof(Prop)) {
			UiManager.Instance.openDialogWindow<PropAttrWindow>((win)=>{
				win.Initialize (item as Prop);
			});
			return;
		}
		if (item.GetType () == typeof(Card)) {
			UiManager.Instance.openWindow<CardBookWindow>((win)=>{
				win.init (item as Card, CardBookWindow.OTHER,viewCallBack);
				fatherWindow.finishWindow();
			});
			return;
		}		
	}
	void viewCallBack()
	{
		UiManager.Instance.openWindow<ActivityChooseWindow> ();
		UiManager.Instance.openWindow<ActivityPrizeViewWindow> ();
	}
 
	
	//用于物品更新
	public void updateButton (PrizeSample sample)
	{
		gameObject.SetActive (true);
		if (sample.type == PrizeType.PRIZE_CARD) {
			Card showCard = CardManagerment.Instance.createCard (sample.pSid);
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + showCard.getImageID (), Image);
 
			Image.width=(int)bigPic.x;
			Image.height=(int)bigPic.y;
			icon_back.gameObject.SetActive(false);
			item = showCard;  
		} else if (sample.type == PrizeType.PRIZE_EQUIPMENT) {
			Equip showEquip = EquipManagerment.Instance.createEquip (sample.pSid);
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + showEquip.getIconId (), Image);
		 
			Image.width=(int)smallPic.x;
			Image.height=(int)smallPic.y;
				icon_back.gameObject.SetActive(true);
			item = showEquip;
			 
		} else if (sample.type == PrizeType.PRIZE_PROP) {
			Prop showProp = PropManagerment.Instance.createProp (sample.pSid);
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + showProp.getIconId (), Image);	
	 
			Image.width=(int)smallPic.x;
			Image.height=(int)smallPic.y;			
			icon_back.gameObject.SetActive(true);
				
			item = showProp; 
		} else if (sample.type == PrizeType.PRIZE_MONEY) {
			ResourcesManager.Instance.LoadAssetBundleTexture (constResourcesPath.GOLD_ICONPATH, Image);	
 
			Image.width=(int)smallPic.x;
			Image.height=(int)smallPic.y;			
				icon_back.gameObject.SetActive(true);
		} else if (sample.type == PrizeType.PRIZE_RMB) {
			ResourcesManager.Instance.LoadAssetBundleTexture (constResourcesPath.RMB_ICONPATH, Image);	
 
			Image.width=(int)smallPic.x;
			Image.height=(int)smallPic.y;	
				icon_back.gameObject.SetActive(true);
		}
		
		
	}
}
