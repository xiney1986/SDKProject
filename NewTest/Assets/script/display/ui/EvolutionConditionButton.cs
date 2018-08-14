using UnityEngine;
using System.Collections;

public class EvolutionConditionButton : ButtonBase {

	private string psname;
	private int psType;
	private long psNum;
	private int psSid;

	public UITexture Image;
	public UISprite bg;
	public UILabel moneyOwned;
	public override void begin ()
	{

		base.begin ();
	}

	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		if (psType == 3) {
			Prop prop = PropManagerment.Instance.createProp(psSid);
			UiManager.Instance.openDialogWindow<PropAttrWindow>((win)=>{
				win.Initialize (prop);
			});
		} else {
			MaskWindow.UnlockUI ();
		}
	}

	public void initButton(EvolutionCondition _ps)
	{
		psType = _ps.costType;
		psNum = _ps.num;
		psSid = _ps.costSid;
		initUI();
	}

	public void initButton(int _type,long num)
	{
		psType = _type;
		psNum = num;
		initUI();
	}

	public void initShowOld(EvolutionCondition _ps)
	{
		psType = _ps.costType;
		psNum = _ps.num;
		psSid = _ps.costSid;
		initShowOldUI();
	}

	private void initShowOldUI()
	{
		switch(psType)
		{
		case 1:
			ResourcesManager.Instance.LoadAssetBundleTexture (constResourcesPath.MONEY_ICONPATH, Image);
//			textLabel.text = ((psNum>UserManager.Instance.self.getMoney())?Colors.RED:Colors.GREEN) + psNum;
			textLabel.text =  psNum.ToString();
			
			if(fatherWindow is MainCardEvolutionWindow){
//			moneyOwned.text =((psNum>UserManager.Instance.self.getMoney())?Colors.RED:Colors.GREEN) + UserManager.Instance.self.getMoney();
				moneyOwned.text = UserManager.Instance.self.getMoney().ToString();
			}
			psname = LanguageConfigManager.Instance.getLanguage("money");
			break;
		case 2:
			ResourcesManager.Instance.LoadAssetBundleTexture (constResourcesPath.RMB_ICONPATH, Image);
//			textLabel.text = ((psNum>UserManager.Instance.self.getRMB())?Colors.RED:Colors.GREEN) + psNum;
			textLabel.text =  psNum.ToString();
			psname = LanguageConfigManager.Instance.getLanguage("rmb");
			break;
		case 3:
			Prop showProp = StorageManagerment.Instance.getProp(psSid);
			Prop prop = PropManagerment.Instance.createProp(psSid);
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + prop.getIconId(), Image);
			int num = showProp == null ? 0:showProp.getNum();
//			textLabel.text = ((psNum>num)?Colors.RED:Colors.GREEN) + psNum;
			textLabel.text =  psNum.ToString();
			psname = prop.getName();
			break;
		}

				
	}


	private void initUI()
	{
		switch(psType)
		{
		case 1:
			ResourcesManager.Instance.LoadAssetBundleTexture (constResourcesPath.MONEY_ICONPATH, Image);
			textLabel.text = ((psNum>UserManager.Instance.self.getMoney())?Colors.RED:Colors.GREEN) + psNum;
			//textLabel.text =  psNum.ToString();
			if(fatherWindow is MainCardEvolutionWindow){
				moneyOwned.text =((psNum>UserManager.Instance.self.getMoney())?Colors.RED:Colors.GREEN) + UserManager.Instance.self.getMoney();
				moneyOwned.text =  UserManager.Instance.self.getMoney().ToString();
			}
			psname = LanguageConfigManager.Instance.getLanguage("money");
			break;
		case 2:
			ResourcesManager.Instance.LoadAssetBundleTexture (constResourcesPath.RMB_ICONPATH, Image);
			textLabel.text = ((psNum>UserManager.Instance.self.getRMB())?Colors.RED:Colors.GREEN) + psNum;
			//textLabel.text =  psNum.ToString();
			psname = LanguageConfigManager.Instance.getLanguage("rmb");
			break;
		case 3:
			Prop showProp = StorageManagerment.Instance.getProp(psSid);
			Prop prop = PropManagerment.Instance.createProp(psSid);
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + prop.getIconId(), Image);
			int num = showProp == null ? 0:showProp.getNum();
			textLabel.text = ((psNum>num)?Colors.RED:Colors.GREEN) + num + "/" + psNum;
			//textLabel.text = num + "/" + psNum;
			psname = prop.getName();
			break;
		}

		if(bg != null){
			bg.spriteName=QualityManagerment.qualityIDToIconSpriteName(PropManagerment.Instance.createProp(psSid).getQualityId());
		}
	}

	public string getName()
	{
		return psname;
	}

	public bool isEnough()
	{
		switch(psType)
		{
		case 1:
			return psNum > UserManager.Instance.self.getMoney() ? false : true;
		case 2:
			return psNum > UserManager.Instance.self.getRMB() ? false : true;
		case 3:
			if(StorageManagerment.Instance.getProp(psSid)==null)
				return false;
			return psNum > StorageManagerment.Instance.getProp(psSid).getNum() ? false : true;
		default:
			return false;
		}
	}
}
