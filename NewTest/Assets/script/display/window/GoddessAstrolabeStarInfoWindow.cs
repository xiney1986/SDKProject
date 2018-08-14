using UnityEngine;
using System.Collections;

public class GoddessAstrolabeStarInfoWindow : WindowBase {

	public UILabel[] needs;
	public GoodsView goodAward;
	public UILabel attrAward;
	public GameObject canOpen;//前置条件
	public ButtonBase buttonOpen;
	public UISprite starIcon;//星x
	public UISprite funcIcon;// 开启功能icon//

	private GoddessAstrolabeSample info;
	private GoddessAstrolabeWindow fawin;

	protected override void begin ()
	{
		base.begin ();
		GuideManager.Instance.guideEvent ();
		MaskWindow.UnlockUI ();
	}

	public void initUI(GoddessAstrolabeWindow _win,GoddessAstrolabeSample _info)
	{
		info = _info;
		fawin = _win;

		GoddessAstrolabeConditions[] conditions = info.conditions;

		//条件
		if(conditions != null) {
			for(int i=0;i<conditions.Length;i++) {
				needs[i].gameObject.SetActive (true);
				needs[i].text = getConStr(conditions[i]);
				if(conditions[i].type == PremiseType.STAR) {
					starIcon.gameObject.SetActive (true);
					starIcon.spriteName="star3";
					starIcon.transform.localPosition = new Vector3(needs[i].gameObject.transform.localPosition.x - 22,needs[i].gameObject.transform.localPosition.y);
				}
				if(conditions[i].type == PremiseType.RMB){
					starIcon.gameObject.SetActive (true);
					starIcon.spriteName="rmb";
					starIcon.transform.localPosition = new Vector3(needs[i].gameObject.transform.localPosition.x - 22,needs[i].gameObject.transform.localPosition.y);
				}
			}
		}

		//奖励
		if (info.awardType == GoddessAstrolabeManagerment.AWARD_ATTR || info.awardType == GoddessAstrolabeManagerment.AWARD_ADD) {
			attrAward.gameObject.SetActive (true);
			attrAward.text = info.awardDesc;
		} else if (info.awardType == GoddessAstrolabeManagerment.AWARD_AWARD) {
			if(info.award == null)
				return;
			goodAward.fatherWindow = this;
			switch (info.award.awardType) {
			case PrizeType.PRIZE_BEAST:
				goodAward.gameObject.SetActive (true);
				Card beast = CardManagerment.Instance.createCard (info.award.awardSid);
				goodAward.init(beast);
				break;
			case PrizeType.PRIZE_CARD:
				goodAward.gameObject.SetActive (true);
				Card card = CardManagerment.Instance.createCard (info.award.awardSid);
				goodAward.init(card);
				break;
			case PrizeType.PRIZE_EQUIPMENT:
				goodAward.gameObject.SetActive (true);
				Equip equip = EquipManagerment.Instance.createEquip (info.award.awardSid);
				goodAward.init(equip);
				break;
			case PrizeType.PRIZE_MONEY:
				goodAward.gameObject.SetActive (true);
				PrizeSample prizeMoney = new PrizeSample(PrizeType.PRIZE_MONEY,0,info.award.awardNum);
				goodAward.init(prizeMoney);
				break;
			case PrizeType.PRIZE_PROP:
				goodAward.gameObject.SetActive (true);
				Prop prop = PropManagerment.Instance.createProp (info.award.awardSid);
				goodAward.init(prop,info.award.awardNum);
				break;
			case PrizeType.PRIZE_RMB:
				goodAward.gameObject.SetActive (true);
				PrizeSample prizeRmb = new PrizeSample(PrizeType.PRIZE_RMB,0,info.award.awardNum);
				goodAward.init(prizeRmb);
				break;
			}
		}
		// 开启星屑商店//
		else if(info.awardType == GoddessAstrolabeManagerment.AWARD_OPENSHOP)
		{
			funcIcon.gameObject.SetActive (true);
		}

		if(info.isOpen) {
			buttonOpen.gameObject.SetActive (true);
			buttonOpen.disableButton(true);
			buttonOpen.textLabel.text = LanguageConfigManager.Instance.getLanguage("goddess11");
			return;
		}

		//显示按钮
		if(GoddessAstrolabeManagerment.Instance.getFatherStarIsOpen(info)){
			canOpen.SetActive (false);
			buttonOpen.gameObject.SetActive (true);
			buttonOpen.textLabel.text = LanguageConfigManager.Instance.getLanguage("goddess07");
			if(conditions != null) {
				int noNum = 0;
				for(int i=0;i<conditions.Length;i++) {
					if (isCanOpen(conditions[i]) == false) {
						noNum++;
					}
				}
				if(noNum > 0)
					buttonOpen.disableButton(true);
				else
					buttonOpen.disableButton(false);
			}
		} else {
			canOpen.SetActive (true);
			buttonOpen.gameObject.SetActive (false);
		}
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);

		if(gameObj.name == "buttonOpen")
		{
			GuideManager.Instance.doGuide();
			GuideManager.Instance.guideEvent ();
			EventDelegate.Add (OnHide,()=>{
				GoddessAstrolabeFPort fport = FPortManager.Instance.getFPort ("GoddessAstrolabeFPort") as GoddessAstrolabeFPort;
				fport.openStar (info.id,()=>{
					GoddessAstrolabeManagerment.Instance.openStarChange(info.id);
					GoddessAstrolabeManagerment.Instance.changeStarOpenById(info.id);
					fawin.updateUI(info.id);
				});
			});
			finishWindow();
		}
		else if(gameObj.name == "close")
		{
			finishWindow();
		}
	}

	//返回带颜色的需求描述
	private string getConStr(GoddessAstrolabeConditions gacinfo)
	{
		if (gacinfo.type == PremiseType.LEVEL) {
			if(UserManager.Instance.self.getUserLevel() >= StringKit.toInt(gacinfo.num))
				return Colors.GREEN + gacinfo.desc;
			else
				return Colors.RED + gacinfo.desc;
		} else if (gacinfo.type == PremiseType.VIP_LEVEL) {
			if(UserManager.Instance.self.getVipLevel() >= StringKit.toInt(gacinfo.num))
				return Colors.GREEN + gacinfo.desc;
			else
				return Colors.RED + gacinfo.desc;
		} else if (gacinfo.type == PremiseType.FRIENDS_NUM) {
			if(FriendsManagerment.Instance.getFriendAmount() >= StringKit.toInt(gacinfo.num))
				return Colors.GREEN + gacinfo.desc;
			else
				return Colors.RED + gacinfo.desc;
		} else if (gacinfo.type == PremiseType.STAR) {
			int num = GoddessAstrolabeManagerment.Instance.getStarScore();
			if(num >= StringKit.toInt(gacinfo.num))
				return Colors.GREEN + num + "/" + gacinfo.num;
			else
				return Colors.RED + num + "/" + gacinfo.num;
		}else if(gacinfo.type == PremiseType.RMB) {
			if(UserManager.Instance.self.getRMB() >= StringKit.toInt(gacinfo.num))
				return Colors.GREEN + gacinfo.desc;
			else
				return Colors.RED + gacinfo.desc;
		}
		return gacinfo.desc;
	}

	//判断是否达成条件
	private bool isCanOpen(GoddessAstrolabeConditions gacinfo)
	{
		if (gacinfo.type == PremiseType.LEVEL) {
			if(UserManager.Instance.self.getUserLevel() < StringKit.toInt(gacinfo.num))
				return false;
		} else if (gacinfo.type == PremiseType.VIP_LEVEL) {
			if(UserManager.Instance.self.getVipLevel() < StringKit.toInt(gacinfo.num))
				return false;
		} else if (gacinfo.type == PremiseType.FRIENDS_NUM) {
			if(FriendsManagerment.Instance.getFriendAmount() < StringKit.toInt(gacinfo.num))
				return false;
		} else if (gacinfo.type == PremiseType.STAR) {
			if(GoddessAstrolabeManagerment.Instance.getStarScore() < StringKit.toInt(gacinfo.num))
				return false;
		}else if(gacinfo.type == PremiseType.RMB) {
			if(UserManager.Instance.self.getRMB()< StringKit.toInt(gacinfo.num))
				return false;
		}

		return true;
	}
}
