using UnityEngine;
using System.Collections;

/**
 * 点击文本链接处理
 * @authro 陈世惟  
 * */
public class OpenWindowOnClick : MonoBehaviour {

	public const string TYPE_EQUIP = "equip";
	public const string TYPE_CARD = "card";
	public const string TYPE_REDIOCARD = "radioShowCard";
	public const string TYPE_REDIOEQUIP= "radioShowEquip";
	public const string TYPE_REDIOSTARSOUL= "radioShowStarSoul";

	void OnClick ()
	{
		UILabel lbl = GetComponent<UITextList>().textLabel;
		
		if (lbl != null)
		{
			string url = lbl.GetUrlAtPosition(UICamera.lastHit.point);
			if (!string.IsNullOrEmpty(url))
			{
				//Application.OpenURL(url);
//				Debug.LogError (url);
				string[] str = url.Split ('|');
				doSomething (str[0], str[1], str[2]);
			}
		}
	}

	void doSomething(string desc,string channelType,string uid)
	{
//		Debug.LogError ("desc==" + desc + "uid==" + uid);
		MaskWindow.LockUI();
		if (desc == "name") {
			getPlayerInfoFPort(uid);
		} else if (desc == TYPE_EQUIP) {
			ErlArray chatItem = ChatManagerment.Instance.getChatByUid (uid, channelType);
			if (chatItem == null) {
				MaskWindow.UnlockUI();
				return;
			}
			Equip eq = EquipManagerment.Instance.createEquip(chatItem);
			if (eq != null) {
				UiManager.Instance.openWindow <EquipAttrWindow>((win)=>{
					win.Initialize (eq, EquipAttrWindow.CARDSHOW,null);
				});
			}
		} else if (desc == TYPE_CARD) {
			ErlArray chatItem = ChatManagerment.Instance.getChatByUid(uid, channelType);
            string name = ChatManagerment.Instance.getChatByUidName(uid, channelType);
			if (chatItem == null) {
				MaskWindow.UnlockUI();
				return;
			}
			ServerCardMsg card = CardManagerment.Instance.createCardByChatServer(chatItem);
			if (card != null) {
				CardBookWindow.Show(card,CardBookWindow.CLICKCHATSHOW,name);
			}
		} else if (desc == TYPE_REDIOCARD) {
			Card card = CardManagerment.Instance.createCard(StringKit.toInt(uid));
			card.setLevel(1);
			if (card != null) {
				CardBookWindow.Show(card,CardBookWindow.OTHER,null);
			}
		} else if (desc == TYPE_REDIOEQUIP) {
			Equip eq = EquipManagerment.Instance.createEquip (StringKit.toInt(uid));
			if (eq != null) {
				UiManager.Instance.openWindow <EquipAttrWindow>((win)=>{
					win.Initialize (eq, EquipAttrWindow.CARDSHOW,null);
				});
			}
		}  else if (desc == TYPE_REDIOSTARSOUL) {
			StarSoul starSoul = StarSoulManager.Instance.createStarSoul (StringKit.toInt(uid));
			if (starSoul != null) {
				UiManager.Instance.openDialogWindow<StarSoulAttrWindow> ((win) => {
					win.Initialize (starSoul, StarSoulAttrWindow.AttrWindowType.None);
				});
			}
		} else {
			MaskWindow.UnlockUI();
		}
	}

	//发uid到后台获取人物数据
	void getPlayerInfoFPort(string _uid)
	{
		ChatGetPlayerInfoFPort fport = FPortManager.Instance.getFPort("ChatGetPlayerInfoFPort") as ChatGetPlayerInfoFPort;
		fport.access(_uid,null,null,PvpPlayerWindow.FROM_CHAT);
	}

}
