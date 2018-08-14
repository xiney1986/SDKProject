using UnityEngine;
using System.Collections;

/**
 * 临时仓库项按钮
 * @author 汤琦
 * */
public class ButtonTempResult : ButtonBase
{
	public TempProp temp;
	public MailWindow win;
	
	public void UpdateTemp (TempProp _temp)
	{
		win = fatherWindow as MailWindow;
		this.temp = _temp;
	}
	
	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		if (gameObject.name == "delete") {//删除
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0093"), LanguageConfigManager.Instance.getLanguage ("s0040"), LanguageConfigManager.Instance.getLanguage ("s0110"), sendDeleteFPort);
			});
			return;
		} else if (gameObject.name == "extract") {//提取
			if (isFull ()) {
				UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
					win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0040"), null, LanguageConfigManager.Instance.getLanguage ("TempGetFull"), null);
				});
			} else {
				sendExtractFPort ();
			}
		}
	}
	//删除或者提取通信成功后的回调
	private void extractResultBack ()
	{
		StorageManagerment.Instance.getTempStorage ().reducePropByIndex (temp.index, 1);
		win.Initialize (0);
		win.Initialize (1);
	}

	private void deleteBack ()
	{
		StorageManagerment.Instance.getTempStorage ().reducePropByIndex (temp.index, 1);
		win.Initialize (0);
		win.Initialize (1);
	}
	//删除通信
	private void sendDeleteFPort (MessageHandle msg)
	{
		if (msg.buttonID == MessageHandle.BUTTON_RIGHT)
			return;
		TempProp tp = StorageManagerment.Instance.getTempPropByUid (temp.tempUid);
		TempStorageDeleteFPort fport = FPortManager.Instance.getFPort ("TempStorageDeleteFPort") as TempStorageDeleteFPort;
		fport.access (tp.index + 1, deleteBack);
	}
	//提取通信
	private void sendExtractFPort ()
	{
		TempProp tp = StorageManagerment.Instance.getTempPropByUid (temp.tempUid);
		if (tp == null){
			MaskWindow.UnlockUI();
			return;
		}
			
		TempStorageExtractFPort fport = FPortManager.Instance.getFPort ("TempStorageExtractFPort") as TempStorageExtractFPort;
		fport.access (tp.index + 1, extractBack);
	}

	private void extractBack ()
	{
		//这里暂时注释掉，抽奖的时候哪怕超出仓库范围，依然直接激活了英雄之章
//		if (temp.type == TempManagerment.card) {
//			Card c = CardManagerment.Instance.createCard (temp.sid);
//			if (HeroRoadManagerment.Instance.activeHeroRoadIfNeed (c))
//				TextTipWindow.Show(LanguageConfigManager.Instance.getLanguage("s0418"));
//		}

		UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
			win.dialogCloseUnlockUI=false;
			win.Initialize(LanguageConfigManager.Instance.getLanguage("s0114"));
		});
		extractResultBack ();
	}
	//验证相应仓库是否满
	private bool isFull ()
	{
		switch (temp.type) {
		case TempPropType.BEAST:
			if ((StorageManagerment.Instance.getAllBeast ().Count + temp.getNum ()) > StorageManagerment.Instance.getBeastStorageMaxSpace ()) {

				return true;
			} else {
				return false;
			}
		case TempPropType.CARD:
			if ((StorageManagerment.Instance.getAllRole ().Count + temp.getNum ()) > StorageManagerment.Instance.getRoleStorageMaxSpace ()) {
				return true;
			} else {
				return false;
			}
		case TempPropType.EQUIPMENT:
			if ((StorageManagerment.Instance.getAllEquip ().Count + temp.getNum ()) > StorageManagerment.Instance.getEquipStorageMaxSpace ()) {
				return true;
			} else {
				return false;
			}
		case TempPropType.GOODS:
			if ((StorageManagerment.Instance.getAllProp ().Count + temp.getNum ()) > StorageManagerment.Instance.getPropStorageMaxSpace ()) {
				return true;
			} else {
				return false;
			}
        case TempPropType.MAGICWEAPON:
            if ((StorageManagerment.Instance.getAllMagicWeapon().Count + temp.getNum()) >
                StorageManagerment.Instance.getMagicWeaponStorageMaxSpace()) {
                return true;
            } else return false;
                
		default:
			return false;
		}
	}
}
