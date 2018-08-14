using UnityEngine;
using System.Collections;

public class InviteContent : MonoBase {


	public UITexture forwardTexture;
	public UIInput playerCode;
	public GameObject getButton;
	public GameObject noGetButton;

	public void initWindow(int _invtiteType,WindowBase window)
	{
		noGetButton.GetComponent<ButtonGetButton>().fatherWindow = window;
		getButton.SetActive (false);
		noGetButton.SetActive (false);
        if(CommandConfigManager.Instance.getNvShenClothType() == 0)
		    ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + "2052c", forwardTexture);
        else ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.CARDIMAGEPATH + "2052", forwardTexture);
		MaskWindow.UnlockUI ();
		isGet(_invtiteType);
	}
	
	public  void buttonEventBase ()
	{

		if (playerCode.label.text.Trim() == string.Empty)
		{
			MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("inviterror01"));
			return;
		}
		InvitewindowFPort fport = FPortManager.Instance.getFPort("InvitewindowFPort") as InvitewindowFPort;
		fport.access(playerCode.label.text,getSuccess,null,null);
		MaskWindow.UnlockUI ();
			/*(fatherWindow as InviteCodeWindow).invtiteCodeFport (playerCode.label.text);
			this.destoryWindow ();*/
	}
	void getSuccess()
	{
		AwardManagerment.Instance.addFunc (AwardManagerment.AWARDS_INVITE_EVENT, drawPrize); 	
		
	}

	private void drawPrize (Award[] award)
	{
		PrizeSample[] prizes = AllAwardViewManagerment.Instance.exchangeAwards(award);
		bool isOpen=HeroRoadManagerment.Instance.isOpenHeroRoad (prizes);
		if(isOpen)
			UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("HeroRoad_open"));
		UiManager.Instance.openDialogWindow<AllAwardViewWindow>((win)=>{	
			win.Initialize(prizes,LanguageConfigManager.Instance.getLanguage("s0120"));
		});
	}

	public void isGet(int _type)
	{
		if(_type == 0)
		{
			getButton.SetActive (false);
			noGetButton.SetActive (true);
		}
		if(_type == 1)
		{
			getButton.SetActive (true);
			noGetButton.SetActive (false);
		}
	}
}
