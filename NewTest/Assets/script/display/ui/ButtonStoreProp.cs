using UnityEngine;
using System.Collections;

public class ButtonStoreProp : ButtonBase
{
	
	public UILabel descript;
	public UILabel propName;
	public UILabel propNum;
	public UITexture itemIcon;
	public UISprite quality;
	public ButtonUseProp useButton;
    public ButtonCompoundProp compoundButton;//合成按钮
    public ButtonShenGeResult equipButton;//镶嵌替换按钮
	public Prop prop;
	private const string FIRSTOBJECT = "001";//第一个加载对象
	private const int STRLENGTH = 12;//字符串限制长度
    private int type;
    private Prop tempProp;
    private int localIndex;
	
	public void UpdateProp (Prop _prop)
	{
        compoundButton.gameObject.SetActive(false);
		if (_prop == null)
			return;
		prop = _prop;
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + prop.getIconId (), itemIcon);

		if (prop.getDescribe ().Length > STRLENGTH + 6) {
			descript.text = prop.getDescribe ().Substring (0, STRLENGTH + 4) + "...";
		}
		else {
			descript.text = prop.getDescribe ();
		}
		//propName.text =QualityManagerment.getQualityColor( prop.getQualityId ()) +prop.getName ();
        propName.text = prop.getName();
		propNum.text = " x " + prop.getNum();
		quality.spriteName = QualityManagerment.qualityIDToIconSpriteName (prop.getQualityId ());
        if (prop.isCanExchageCard()) {
            compoundButton.disableButton(false);
            compoundButton.gameObject.SetActive(true);
            compoundButton.fatherWindow = fatherWindow;
            compoundButton.initButton(prop);
        }
	    if (prop.isShenGeProp())
	    {
	        if (type == ShenGeManager.CHANGE)
	        {
	            equipButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("NvShenShenGe_011");
                equipButton.gameObject.SetActive(true);
                equipButton.fatherWindow = fatherWindow;
                equipButton.UpdateProp(prop, localIndex);
	        }
	        else if (type == ShenGeManager.EQUIP)
	        {
                equipButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("NvShenShenGe_010");
                equipButton.gameObject.SetActive(true);
                equipButton.fatherWindow = fatherWindow;
                equipButton.UpdateProp(prop,localIndex);
	        }
	        else
	        {
	            PropSample sample = PropSampleManager.Instance.getPropSampleBySid(prop.getNextShenGeSid());
	            if (sample != null)
	            {
	                compoundButton.gameObject.SetActive(true);
	                if (!ShenGeManager.Instance.checkCanGroup(prop, ShenGeManager.STORAGE))
	                {
	                    compoundButton.disableButton(true);
	                }
	                else
	                {
	                    compoundButton.disableButton(false);
	                    compoundButton.fatherWindow = fatherWindow;
	                    compoundButton.initButton(prop);
	                }
	            }
	        }
	    }
	    if (prop.getType () == PropType.PROP_TYPE_CHEST || prop.getType () == PropType.PROP_TYPE_LOCK_CHEST || prop.getType () == PropType.PROP_RENAME
            || prop.getType() == PropType.PROP_HAFE_MONTH || prop.getType() == PropType.PROP_COMBAT_CHEST) {
			useButton.gameObject.SetActive (true);
			useButton.fatherWindow = fatherWindow;
			useButton.initButton (prop);
		} else {
			useButton.gameObject.SetActive (false);
		}

//		if (prop.getType () != PropType.PROP_TYPE_CHEST&&prop.getType()!=PropType.PROP_TYPE_LOCK_CHEST) {
//			useButton.gameObject.SetActive (false);
//		} else {
//			useButton.gameObject.SetActive (true);
//			useButton.fatherWindow = fatherWindow;
//			useButton.initButton(prop);
//		}
	}

    public void init(Prop pp, int typee,int index)
    {
        type = typee;
        tempProp = pp;
        localIndex = index;
        UpdateProp(tempProp);
    }

    public override void DoClickEvent ()
	{
		base.DoClickEvent ();

		UiManager.Instance.openDialogWindow<PropAttrWindow>((win)=>{
			win.Initialize (prop);
		});

		//fatherWindow.hideWindow ();
	}
}
