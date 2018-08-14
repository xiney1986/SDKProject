using UnityEngine;
using System.Collections.Generic;

/**
 * 公会建设窗口
 * @author 汤琦
 * */
public class GuildBuildWindow : WindowBase {

	/** 道具预制体 */
	public GameObject goodsItem;
	/** 建筑预制体 */
	public GameObject buildItem;
	/** 建筑容器 */
	public GuildBuildContent content;
	/** 建筑详细信息 */
	public GuildBuildDescItem descItem;
	/** 当前浏览的建筑样本 */
	private GuildBuildSample buildSample;
	private GuildManagerment instance;
	/** 建筑等级 */
	private int buildLevel = -1;

	protected override void begin () {
		base.begin ();
		instance = GuildManagerment.Instance;
		changeButton ();
		content.reLoad ();
		if(buildSample != null){
			updateInfo(buildSample);
		}
		MaskWindow.UnlockUI ();
	}

	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow ();
		}
		else if (gameObj.name == "buttonBuild") {
			if (buildSample == null || !isManager ()) {
				MaskWindow.UnlockUI ();
				return;
			}
			buildLevel = instance.getBuildLevel(buildSample.sid.ToString());

			if(GuildManagerment.Instance.isUpGuildBuild(buildSample.sid,buildLevel))
			{
				UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
					win.dialogCloseUnlockUI=false;
					if(GuildManagerment.Instance.getBuildLevel(buildSample.sid.ToString ()) <= 0)
					{
						win.initWindow(2,LanguageConfigManager.Instance.getLanguage("s0094"),LanguageConfigManager.Instance.getLanguage("s0093"),
						               LanguageConfigManager.Instance.getLanguage("Guild_77",buildSample.buildName),building);
					}
					else
					{
						win.initWindow(2,LanguageConfigManager.Instance.getLanguage("s0094"),LanguageConfigManager.Instance.getLanguage("s0093"),
						               LanguageConfigManager.Instance.getLanguage("Guild_76",buildSample.buildName),building);
					}
				});
			}
		}
	}

	public override void OnNetResume () {
		base.OnNetResume ();
		instance = GuildManagerment.Instance;
		changeButton ();
		content.reLoad ();
	}

	/// <summary>
	/// 根据职位确定修建按钮是否显示
	/// </summary>
	private void changeButton () {
		if (isManager ()) {
			descItem.buttonBuild.gameObject.SetActive (true);
		}
		else {
			descItem.buttonBuild.gameObject.SetActive (false);
		}
	}

	/// <summary>
	/// 是否是管理者
	/// </summary>
	private bool isManager () {
		return GuildManagerment.Instance.getGuildJob () == GuildJobType.JOB_PRESIDENT || GuildManagerment.Instance.getGuildJob () == GuildJobType.JOB_VICE_PRESIDENT;
	}

	private void building(MessageHandle msg)
	{
		if(msg.buttonID == MessageHandle.BUTTON_LEFT || buildSample == null){
			MaskWindow.UnlockUI();
			return;
		}

		buildLevel = instance.getBuildLevel(buildSample.sid.ToString());

		//建筑存在就升级没有就创建
		if(buildLevel > 0)
		{
			GuildUpgradeBuildFPort fport = FPortManager.Instance.getFPort("GuildUpgradeBuildFPort") as GuildUpgradeBuildFPort;
			fport.access(buildSample.sid.ToString (),buildSample.costs[buildLevel],()=>{
				GuildManagerment.Instance.updateGuildInfo (()=>{
					content.updateAllItems ();
					updateInfo (buildSample);
				});
			});
		}
		else
		{
			GuildCreateBuildFPort fport = FPortManager.Instance.getFPort("GuildCreateBuildFPort") as GuildCreateBuildFPort;
			fport.access(buildSample.sid.ToString (),buildSample.costs[buildLevel],()=>{
				GuildManagerment.Instance.updateGuildInfo (()=>{
					content.updateAllItems ();
					updateInfo (buildSample);
				});
			});
		}
	}

public void init(GuildBuildSample _buildSample){
	buildSample = _buildSample;
	}
	/// <summary>
	/// 更新建筑升级信息
	/// </summary>
	public void updateInfo (GuildBuildSample _buildSample) {
		
		if (_buildSample == null || (this.buildSample == _buildSample && buildLevel == instance.getBuildLevel(buildSample.sid.ToString()))) {
			return;
		}
		descItem.clear ();
		this.buildSample = _buildSample;
		Guild guild = instance.getGuild();
		buildLevel = instance.getBuildLevel(buildSample.sid.ToString());
		bool isMaxLv = buildLevel >= buildSample.levelMax;

		if (buildLevel > 0) {
			descItem.buttonBuild.textLabel.text = Language ("Guild_34");
		} else {
			descItem.buttonBuild.textLabel.text = Language ("Guild_35");
		}

		//标题部分
		descItem.spriteIcon.spriteName = instance.getBuildIcon (buildSample.sid);
		descItem.labelNameTitle.text = Language("s0303") + buildSample.buildName + ":";
		descItem.labelOldLv.text = "Lv." + buildLevel;
		descItem.labelNewLv.text = isMaxLv ? "" : ("Lv." + (buildLevel + 1));
		descItem.objUpArrow.SetActive (!isMaxLv);
		descItem.labelLiveness.text = guild.livenessing.ToString ();
		descItem.labelCostLiveness.text = isMaxLv ? "" : buildSample.costs [buildLevel].ToString ();

		//提升部分
		//是公会大厅就显示成员提升信息
		if (buildSample.sid == 1) {
			descItem.labelNeedHellLv.transform.parent.gameObject.SetActive (false);
			descItem.objHell.SetActive (true);
			descItem.labelOldMember.text = guild.membershipMax.ToString ();
			descItem.labelNewMember.text = isMaxLv ? "" : (guild.membershipMax + 2).ToString ();
			descItem.objMemberUpArrow.SetActive (!isMaxLv);
		} else {
			descItem.labelNeedHellLv.transform.parent.gameObject.SetActive (true);
			descItem.labelNeedHellLv.text = isMaxLv ? Language ("s0070") : ("Lv." + buildSample.hallLevel [buildLevel]);
			descItem.objHell.SetActive (false);
		}

		//升级描述
		if (isMaxLv) {
			descItem.labelDesc.transform.parent.gameObject.SetActive (true);
			descItem.labelDesc.text = Language ("s0070");
			descItem.buttonBuild.disableButton (true);
		} else {
			descItem.buttonBuild.disableButton (false);
			if (buildSample.sid != 3) {
				descItem.labelDesc.transform.parent.gameObject.SetActive (true);
				descItem.labelDesc.text = buildSample.getDesc (buildLevel).Replace ("~","\n");
			} else {
				descItem.labelDesc.transform.parent.gameObject.SetActive (false);

				if (buildSample.goods != null) {

					List<GuildGood> goods = buildSample.goods;

					List<Goods> tmpGoods = new List<Goods> ();
					for (int i = 0; i < goods.Count; i++) {
						if (goods[i].level == buildLevel + 1) {
							tmpGoods.Add (new Goods(goods[i].sid));
						}
					}
					GoodsView goodsView;
					if (tmpGoods.Count > 0) {
						for (int i = 0; i < tmpGoods.Count; i++) {
							goodsView = NGUITools.AddChild (descItem.objBuildShow, goodsItem).GetComponent<GoodsView> ();
							goodsView.fatherWindow = this;
							goodsView.init (tmpGoods[i].getGoodsType (),tmpGoods[i].getGoodsSid (),0);
						}
						descItem.objBuildShow.GetComponent<UIGrid> ().repositionNow = true;
						descItem.objBuildShow.GetComponent<UIPanel>().clipOffset = new Vector2(0,10.0f);
						descItem.objBuildShow.gameObject.transform.localPosition = new Vector3(0,-100f,0);
					}
				}

			}
		}
		
		descItem.labelName.text = Language("guildBuildUp_" + buildSample.sid);
	}
}
