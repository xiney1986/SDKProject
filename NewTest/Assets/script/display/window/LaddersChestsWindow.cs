using UnityEngine;
using System.Collections;
/// <summary>
/// 天梯宝箱打开窗口
/// </summary>
public class LaddersChestsWindow : WindowBase {

	public ButtonBase btn_receive;

	public UILabel label_chestName;
	//public UILabel label_titleName;
	//public UISprite sprite_titleBg;

	public UIGrid gridContent;//动态创建的奖励父节点，需要排序
	public GameObject goodsViewPrefab;
	public PrizeSample[] prizes;
	public LaddersChestInfo data;

	//public GameObject root_titleDes;
	public GameObject prefab_des;

	/// <summary>
	/// 在开始时 根据玩家声望 更新
	/// </summary>
	public override void OnStart ()
	{
		prefab_des.SetActive(false);
		int prestige=UserManager.Instance.self.prestige;
		LaddersTitleSample sample=LaddersConfigManager.Instance.config_Title.M_getTitle(prestige);
		//updateDes(root_titleDes,sample.addDescriptions);
		//updateTitle(sample);
	}

	protected override void begin ()
	{
		base.begin ();
	    UiManager.Instance.laddersChestsWindow = this;
		MaskWindow.UnlockUI();
	}
	/// <summary>
	/// 更新宝箱里 称号的加成描述信息
	/// </summary>
	/// <param name="_parent">_parent.</param>
	/// <param name="_des">_des.</param>
	private void updateDes(GameObject _parent,string[] _des)
	{
		UIUtils.M_removeAllChildren(_parent);
		if(_des==null)
		{
			return;
		}
		GameObject itemDes;
		for(int i=0,length=_des.Length;i<length;i++)
		{
			if(i>5)
			{
				break;
			}
			itemDes=NGUITools.AddChild (_parent, prefab_des);
			itemDes.SetActive(true);
			itemDes.GetComponent<UILabel>().text=_des[i];
		}
		_parent.GetComponent<UIGrid>().Reposition();
	}
	/// <summary>
	/// 更新称号图标
	/// </summary>
	/// <param name="currentTitle">Current title.</param>
//	private void updateTitle(LaddersTitleSample currentTitle)
//	{
//		if(currentTitle!=null)
//		{
//			label_titleName.text=LaddersManagement.Instance.M_getCurrentPlayerTitle().name;
//		}else
//		{
//			label_titleName.text=Language("laddersTip_14");
//		}
//		
//		LaddersMedalSample currentMedal=LaddersManagement.Instance.M_getCurrentPlayerMedal();
//		if(currentMedal!=null)
//		{
//			sprite_titleBg.spriteName="medal_"+Mathf.Min(currentMedal.index+1,5);
//		}else
//		{
//			sprite_titleBg.spriteName="medal_0";
//		}
//		sprite_titleBg.gameObject.SetActive(true);
//	}
	/// <summary>
	/// 根据索引 更新宝箱名字
	/// </summary>
	private void updateChestName()
	{
		switch(data.index)
		{
		case 1:
			label_chestName.text=data.multiple>100?Language("laddersChest_02"):Language("laddersChest_01");
			break;
		case 2:
			label_chestName.text=data.multiple>100?Language("laddersChest_04"):Language("laddersChest_03");
			break;
		case 3:
			label_chestName.text=data.multiple>100?Language("laddersChest_06"):Language("laddersChest_05");
			break;
		}
	}
	/// <summary>
	/// 初始化 宝箱中的奖励
	/// </summary>
	/// <param name="chestInfo">Chest info.</param>
	/// <param name="_prizes">_prizes.</param>
	public void initAward(LaddersChestInfo chestInfo, PrizeSample[] _prizes)
	{
		this.prizes = _prizes;
		if (prizes != null && prizes.Length > 0) {
			for (int i = 0; i < prizes.Length; i++) {
				GameObject a = NGUITools.AddChild (gridContent.gameObject, goodsViewPrefab);
				a.name = StringKit.intToFixString (i + 1);
				GoodsView goodsButton = a.GetComponent<GoodsView> ();
				goodsButton.fatherWindow = this;
				goodsButton.onClickCallback = goodsButton.DefaultClickEvent;
				goodsButton.init(prizes[i]);
			}
			gridContent.repositionNow = true;
		}
		data=chestInfo;
		btn_receive.disableButton(!data.receiveEnble);
		updateChestName();
	}
	/// <summary>
	/// 该窗口的点击事件
	/// </summary>
	/// <param name="gameObj">Game object.</param>
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);

		if (gameObj.name == "btn_close") {
			finishWindow();
		}
		else if (gameObj.name == "btn_receive") {
			if (AllAwardViewManagerment.Instance.isFull(prizes)) {
				TextTipWindow.Show (Language("laddersTip_08"));
				MaskWindow.UnlockUI();
				return;
			} else {
				LaddersChestReceiveFPort fport=new LaddersChestReceiveFPort();
				fport.apply(data.index,(msg)=>{
					if(msg=="ok")
					{
						//获取本次奖励的所有东西，拿来做展示
						AwardManagerment.Instance.addFunc (AwardManagerment.AWARDS_LADDER, (awards)=>{
							EventDelegate.Add (OnHide,()=>{
								if (awards != null) {
                                    showHapply();
									UiManager.Instance.openDialogWindow<AllAwardViewWindow> ((win) => {
										win.Initialize (awards [0], Language ("activity06"));
										win.showComfireButton (true, Language ("ladderButton"));
									});

								}
								else {
									showHapply ();
								}
							});
							finishWindow ();
						});
					}
				});

			}
		}else if (gameObj.name == "btn_title") {
			finishWindow();
			//称号一览按钮
			UiManager.Instance.openWindow<LaddersTitleViewWindow> ();
		}
	}

	/// <summary>
	/// 领完奖励看看声望是否能升级
	/// </summary>
	private void showHapply ()
	{
		LaddersGetInfoFPort newFport = FPortManager.Instance.getFPort<LaddersGetInfoFPort> ();
		newFport.apply((hasApply)=>{
			if(fatherWindow!=null)
			{
				(fatherWindow as LaddersWindow).M_updateView();
				(fatherWindow as LaddersWindow).M_onReceiveChestBox();
			}
		});
	}
    public override void DoDisable() {
        base.DoDisable();
        UiManager.Instance.laddersChestsWindow = null;
    }
}
