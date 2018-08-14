using UnityEngine;
using System.Collections;

/** 建筑详细信息 */
public class GuildBuildDescItem : MonoBehaviour {

	/** 建筑图标 */
	public UISprite spriteIcon;
	/** 建筑升级标题名称 */
	public UILabel labelName;
	/** 建筑名称标题 */
	public UILabel labelNameTitle;
	/** 拥有活跃值 */
	public UILabel labelLiveness;
	/** 需求活跃值 */
	public UILabel labelCostLiveness;
	/** 需求大厅等级 */
	public UILabel labelNeedHellLv;
	/** 当前等级 */
	public UILabel labelOldLv;
	/** 提升后等级 */
	public UILabel labelNewLv;
	/** 提升箭头 */
	public GameObject objUpArrow;
	/** 解锁描点 */
	public GameObject objBuildShow;
	/** 修建按钮 */
	public ButtonBase buttonBuild;
	/** 提升后文字介绍 */
	public UILabel labelDesc;
	/** 当前人数 */
	public UILabel labelOldMember;
	/** 提升后人数 */
	public UILabel labelNewMember;
	/** 人数提升箭头 */
	public GameObject objMemberUpArrow;
	/** 大厅 */
	public GameObject objHell;

	/// <summary>
	/// 恢复初始状态
	/// </summary>
	public void clear () {
		labelNeedHellLv.transform.parent.gameObject.SetActive (false);
		spriteIcon.spriteName = "";
		labelName.text = "";
		labelNameTitle.text = "";
		labelLiveness.text = "";
		labelCostLiveness.text = "";
		labelNeedHellLv.text = "";
		labelOldLv.text = "";
		labelNewLv.text = "";
		objUpArrow.SetActive (false);
		Utils.DestoryChilds (objBuildShow);
		labelDesc.text = "";
		labelOldMember.text = "";
		labelNewMember.text = "";
		objMemberUpArrow.SetActive (false);
		objHell.SetActive (false);
		labelDesc.transform.parent.gameObject.SetActive (false);
	}
}
