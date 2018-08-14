using UnityEngine;
using System.Collections;

/// <summary>
/// 坐骑生活技能--共鸣
/// </summary>
public class MountCombinSkillItem :  MonoBase {

	/* gameobj fields */
	/** 技能图标 */
	public UITexture icon;
	/** 技能为开启图标 */
	public UILabel limitLabel;
	/** 技能描述 */
	public UILabel skillInfo;
	/** 当前拥有的坐骑数量 */
	public UILabel ownMountsNumber;
	/** 所有坐骑加成 */
	public UILabel mountAttrAddValue;
	
	/* fields */

	/* methods */
	/** 更新UI */
	public void updateUI() {
		updateText();
		updateIcon();
	}
	/** 初始化文本 */
	private void updateText() {
		MountsConfigManager config=MountsConfigManager.Instance;
		MountsManagerment manager= MountsManagerment.Instance;
		skillInfo.text=LanguageConfigManager.Instance.getLanguage("mount_combin_skillInfo",config.getAddAttrPer());
		ownMountsNumber.text=manager.getAllMountsCount()+LanguageConfigManager.Instance.getLanguage("ge");
		mountAttrAddValue.text=LanguageConfigManager.Instance.getLanguage("mount_addAttrValue",config.getAttrPerByString());
	}
	/** 更新技能图标 */
	private void updateIcon() {
		MountsConfigManager config=MountsConfigManager.Instance;
		MountsManagerment manager=MountsManagerment.Instance;
		if(manager.getMountsLevel()<config.getAttrPerOpenLv()) {
			icon.gameObject.SetActive(true);
			limitLabel.gameObject.SetActive(true);
			limitLabel.text=LanguageConfigManager.Instance.getLanguage("mount_open_lifeskill_desc",config.getAttrPerOpenLv().ToString());
		} else {
			icon.gameObject.SetActive(true);
			limitLabel.gameObject.SetActive(false);
		}
	}
}