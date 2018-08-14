using UnityEngine;
using System.Collections;

public class PropUseInChangeAlternateWindow : WindowBase
{
	public UITexture propImage;
	public UILabel propCount;
	public UILabel propDescript;
	public UILabel winName;
	public int showType;
	public const int ONEKEYREST = 0;//一键回复
	public const int ONEKEYREBIRTH = 1;//一键复活
	
	public const int SINGLEREST = 2;//普通的选单体回复
	public const int SINGLEREBIRTH = 3;
	public ButtonBase buttonUse;
	public ButtonBase buttonBuy;
	private string[] ids;//被使用道具卡片数组
	private CallBackStrtArray useCallBack;

	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj); 
		if (gameObj.name == "close") {	
			hideWindow ();
		} else if (gameObj.name == "buttonUse") {
			hideWindow ();
			useCallBack (ids);
		} else if (gameObj.name == "buttonBuy") {
			
		}
		
	}
	
	public void Initialize (int propId, int showtype, string[] ids, CallBackStrtArray useCallBack)
	{ 
		this.useCallBack = useCallBack;
		this.ids = ids;
		Prop prop = StorageManagerment.Instance.getProp (propId);
		if (prop == null) {
			prop = PropManagerment.Instance.createProp (propId);
		}
		 
		showType = showtype;
		updateProp (prop);
		propDescript.text = prop.getDescribe ();
		
		switch (showtype) {
		case ONEKEYREST:
			propDescript.text = LanguageConfigManager.Instance.getLanguage ("s0180", ids.Length.ToString ());
			break;
		case ONEKEYREBIRTH:
			propDescript.text = LanguageConfigManager.Instance.getLanguage ("s0181", ids.Length.ToString ());
			break;
		case SINGLEREST:	
			propDescript.text = LanguageConfigManager.Instance.getLanguage ("s0178");
			break;
		case SINGLEREBIRTH:	
			propDescript.text = LanguageConfigManager.Instance.getLanguage ("s0179");
			break;
		}


		if (prop.getNum() <= 0) {
			buttonUse.disableButton (true);
		} 
	}
	
	public void updateProp (Prop prop)
	{ 

		winName.text = prop.getName ();
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + prop.getIconId (), propImage);
		propCount.text = "x" + prop.getNum(); 
	}
	
	
}

