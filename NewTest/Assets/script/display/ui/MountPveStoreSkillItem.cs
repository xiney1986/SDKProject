using UnityEngine;
using System.Collections;

/// <summary>
/// 坐骑生活技能--存储行动力
/// </summary>
public class MountPveStoreSkillItem : MonoBase {

	/* gameobj fields */
	/** 技能图标 */
	public UITexture icon;
	/** 技能为开启图标 */
	public UILabel limitLabel;
	/** 技能简介 */
	public UILabel skillInfo;
	/** 存储行动力经验条 */
	public barCtrl pveStoreBar;
	/** 存储行动力值 */
	public UILabel pveStoreValue;

	/* fields */

	/* methods */
	/** 更新UI */
	public void updateUI() {
		updateText();
		updateStorePve();
		updateIcon();
	}
	/** 更新文本描述 */
	private void updateText() {
		MountsConfigManager manager=MountsConfigManager.Instance;
		skillInfo.text=LanguageConfigManager.Instance.getLanguage("mount_pve_store_skillInfo",manager.getInitPveMax().ToString(),manager.getAddPveConut ().ToString());
	}
	/** 更新存储行动力 */
	private void updateStorePve () {
		MountsConfigManager config=MountsConfigManager.Instance;
		MountsManagerment manager=MountsManagerment.Instance;
		User user=UserManager.Instance.self;
		pveStoreBar.updateValue (user.getStorePvEPoint(),user.getStorePvEPointMax());
		pveStoreValue.text = user.getStorePvEPoint() + "/" + user.getStorePvEPointMax()+"("+LanguageConfigManager.Instance.getLanguage("s0572")+")";
	}
	/** 更新技能图标 */
	private void updateIcon() {
		MountsConfigManager config=MountsConfigManager.Instance;
		MountsManagerment manager=MountsManagerment.Instance;
		if(manager.getMountsLevel()<config.getAddPveOpenLv()) {
			icon.gameObject.SetActive(true);
			limitLabel.gameObject.SetActive(true);
			limitLabel.text=LanguageConfigManager.Instance.getLanguage("mount_open_lifeskill_desc",config.getAddPveOpenLv().ToString());
		} else {
			icon.gameObject.SetActive(true);
			limitLabel.gameObject.SetActive(false);
		}
	}
}
