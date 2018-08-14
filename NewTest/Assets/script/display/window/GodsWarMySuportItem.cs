using UnityEngine;
using System.Collections;

public class GodsWarMySuportItem : MonoBase 
{
	/// <summary>
	/// 场次
	/// </summary>
	private string lblChangCi;
	/// <summary>
	/// 支持谁获胜
	/// </summary>
	private string lblName;
	/// <summary>
	/// 奖励
	/// </summary>
	private string lblPrize;
	/// <summary>
	/// 内容
	/// </summary>
	public UILabel lblContet;
	GodsWarMySuportInfo info;

	
	/// <summary>
	/// 初始化item
	/// </summary>
	public void initItem(GodsWarMySuportInfo info)
	{
		this.info = info;
		lblChangCi = getTypeByLocalId(info.localId);
		if(info.isWin == 1)
		{
			//lblName.text = LanguageConfigManager.Instance.getLanguage("godsWar_77",info.name);
			//lblPrize.text = LanguageConfigManager.Instance.getLanguage("godsWar_79",info.prizes.getPrizeNumByInt().ToString()+info.prizes.getPrizeName());
			lblContet.text = lblChangCi +" "+ LanguageConfigManager.Instance.getLanguage("godsWar_77",info.name) +" "+ LanguageConfigManager.Instance.getLanguage("godsWar_79",info.prizes.getPrizeNumByInt().ToString()+info.prizes.getPrizeName());
		}
		else if(info.isWin ==-1){
			lblName = LanguageConfigManager.Instance.getLanguage("godsWar_81",info.name);
			//int sid = GoodsSampleManager.Instance.getGoodsSampleBySid(GoodsSampleManager.Instance.getAllShopGoods(ShopType.GODSWAR_SHOP)[0]).costToolSid;
			//string name = PropManagerment.Instance.createProp(sid).getName(); //StorageManagerment.Instance.getProp(sid).getName();
            int a = GodsWarManagerment.Instance.getTypeByLocalId(info.localId);
            PrizeSample ps = GodsWarPrizeSampleManager.Instance.getSuportPrize()[a-1].item[0];
		   // PrizeSample pss = info.prizes;
			//lblPrize = LanguageConfigManager.Instance.getLanguage("godsWar_810",getPrePrizeNumByLocalID().ToString()+name);
            lblPrize = LanguageConfigManager.Instance.getLanguage("godsWar_810", info.freeState == 0 ? ps.num + ps.getPrizeName() : (StringKit.toInt(ps.num) * 3) + ps.getPrizeName());
			lblContet.text = lblChangCi +" "+lblName + " " +lblPrize;
		}
		else {
			lblContet.text = lblChangCi +" "+LanguageConfigManager.Instance.getLanguage("godsWar_78",info.name);
		} 
	}

	public string getTypeByLocalId(int localid)
	{
		int a = GodsWarManagerment.Instance.getTypeByLocalId(localid);
		if(a==1)
			return LanguageConfigManager.Instance.getLanguage("godsWar_55","8");
		
		if (a==2) 
			return LanguageConfigManager.Instance.getLanguage("godsWar_55","4");
		
		if (a==3) 
			//return LanguageConfigManager.Instance.getLanguage("godsWar_55","2");
            return LanguageConfigManager.Instance.getLanguage("godsWar_56");
		if(a==4)
			//return LanguageConfigManager.Instance.getLanguage("godsWar_56");
            return LanguageConfigManager.Instance.getLanguage("godswar_922l");
		if(a==5)
			return LanguageConfigManager.Instance.getLanguage("godsWar_92");
		return "";
	}
}
