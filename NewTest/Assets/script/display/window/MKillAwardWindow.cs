using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 秒杀战斗结算窗口
 * @author 汤琦
 * */
public class MKillAwardWindow : WindowBase
{
    /**卡片经验增加label */
    public UILabel cardExpLabel;
    /**女神经验增加label */
    public UILabel beastExpLabel;
    /**奖励容器挂接数组 */
    public GameObject awardPoints;
    /**奖励预制体 */
    public GameObject goodsVaViewPerfab;
    private Award award;
    private List<PrizeSample> selectPrizeList;
    private string careDec="";//卡片经验描述
    private string beastDec = "";//女神经验描述
    public CallBack cl;
	protected override void DoEnable ()
	{
		base.DoEnable ();
		
	}

    private void setDecForInfo(Award award)
    {
        if (award == null) return;
        //CharacterData tempGuardianForce = BattleManager.battleData.playerTeamInfo.guardianForce;//召唤兽
        int cardExp = 0;
        int beastExp = 0;
        string beastExpDesc = "";
        if (award.exps != null)
        {
            if (BattleManager.battleData.playerTeamInfo.guardianForce != null)
            {
                for (int i = 0; i < award.exps.Count; i++)
                {
                    if (BattleManager.battleData.playerTeamInfo.guardianForce.uid == award.exps[i].id)
                    {
                        BeastEvolve tmp =
                            BeastEvolveManagerment.Instance.getBeastEvolveBySid(
                                BattleManager.battleData.playerTeamInfo.guardianForce.sid);
                        if (!tmp.getBeast().isMaxLevel())
                            beastExp = award.exps[i].expGap;
                        else if (tmp.getBeast().isMaxLevel() && tmp.getBeast().getLevel() != 125)
                            beastExpDesc = LanguageConfigManager.Instance.getLanguage("Evo19");
                        else if (tmp.getBeast().isMaxLevel() && tmp.getBeast().getLevel() == 125)
                            beastExpDesc = LanguageConfigManager.Instance.getLanguage("Evo20");
                    }
                    else if (cardExp == 0&&award.exps[i].expGap>0)
                    {
                        cardExp = award.exps[i].expGap;
                    }
                }
            }
            else
            {
                for (int i=0;i<award.exps.Count;i++)
                {
                    if (cardExp == 0 && award.exps[i].expGap > 0) cardExp = award.exps[i].expGap;
                }
                //cardExp = award.exps[0].expGap;
            }
        }
        if (cardExp != 0)
        {


            float expAdd = 0;
            expAdd += GuildManagerment.Instance.getSkillAddExpPorCardPve()*0.01f;
            if (UserManager.Instance.self.getVipLevel() > 0)
                expAdd +=
                    VipManagerment.Instance.getVipbyLevel(UserManager.Instance.self.getVipLevel()).privilege.expAdd*
                    0.0001f;
            if (ServerTimeKit.getSecondTime() < BackPrizeLoginInfo.Instance.endTimes) // 双倍经验期间//
                {
                careDec = LanguageConfigManager.Instance.getLanguage("Award_exp_gavee", cardExp + "",
                    2 + expAdd + "");
                //LanguageConfigManager.Instance.getLanguage("EXPADD") + (2 + expAdd);
            } else {
                if (expAdd == 0) {
                    careDec = LanguageConfigManager.Instance.getLanguage("Award_exp_gave", cardExp + "");
                } else {
                    careDec = LanguageConfigManager.Instance.getLanguage("Award_exp_gavee", cardExp + "",
                        1 + expAdd + "");
                }
            }
        }
        if (beastExp != 0)
        {
            float expA = 0;
            expA += GuildManagerment.Instance.getSkillAddExpPorBeastPve()*0.01f;
            if (UserManager.Instance.self.getVipLevel() > 0)
                expA = VipManagerment.Instance.getVipbyLevel(UserManager.Instance.self.getVipLevel()).privilege.expAdd*
                       0.0001f;
            if (ServerTimeKit.getSecondTime() < BackPrizeLoginInfo.Instance.endTimes) // 双倍经验期间//
            {
                beastDec = LanguageConfigManager.Instance.getLanguage("Award_exp_gavee_nv1", beastExp + "",
                    2 + expA + "");
                //LanguageConfigManager.Instance.getLanguage("EXPADD") + (2 + expAdd);
            }
            else
            {
                if (expA == 0)
                {
                    beastDec = LanguageConfigManager.Instance.getLanguage("Award_exp_gavee_nv", beastExp + "");
                }
                else
                {
                    beastDec = LanguageConfigManager.Instance.getLanguage("Award_exp_gavee_nv1", beastExp + "",
                        1 + expA + "");
                }
            }
        }
        else
        {
            if (BattleManager.battleData.playerTeamInfo.guardianForce == null)
                beastDec = "";
            else
                beastDec = LanguageConfigManager.Instance.getLanguage("Award_exp_gavee_nvShen", beastExpDesc);
        }
    }
    private void getAwardMessageInfo(Award award) {
        List<PrizeSample> temPrizeSamples = new List<PrizeSample>();
        if (award == null) return;
        //if (award.exps != null) //经验
        //{
        //    for (int i = 0; i < award.exps.Count; i++) {
        //        Card targetCard = award.exps[i].cardLevelUpData.levelInfo.orgData as Card;
        //        tempStringList.Add(LanguageConfigManager.Instance.getLanguage("Award_exp_gave",
        //            targetCard.getName(), award.exps[i].expGap + ""));
        //    }
        //}
        //if (award.expGap > 0)
        //    tempStringList.Add(LanguageConfigManager.Instance.getLanguage("Award_exp_gave1",
        //        UserManager.Instance.self.nickname, award.expGap + ""));
        if (award.moneyGap > 0)temPrizeSamples.Add(new PrizeSample(1,0,award.moneyGap));
        if (award.rmbGap > 0)temPrizeSamples.Add(new PrizeSample(2,0,award.rmbGap));
        if (award.honorGap > 0)temPrizeSamples.Add(new PrizeSample(10,0,award.honorGap));
       // if (award.integralGap > 0)
        if (award.meritGap > 0)temPrizeSamples.Add(new PrizeSample(13,0,award.meritGap));
       // if (award.starGap > 0) 
       // if (award.godsWar_integralGap > 0)
       // if (award.luckyStarGap > 0)
        if (award.props != null) {
            for (int i = 0; i < award.props.Count; i++) {
                temPrizeSamples.Add(new PrizeSample(3,award.props[i].sid,award.props[i].num));
            }
        }
        if (award.equips != null) {
            for (int i = 0; i < award.equips.Count; i++) {
                temPrizeSamples.Add(new PrizeSample(4,award.equips[i].sid,award.equips[i].num));
            }
        }
        if (award.magicWeapons != null) {
            for (int i = 0; i < award.magicWeapons.Count; i++) {
                temPrizeSamples.Add(new PrizeSample(21,award.magicWeapons[i].sid,award.magicWeapons[i].num));
            }
        }
        if (award.starsouls != null) {
            for (int i = 0; i < award.starsouls.Count; i++) {
                temPrizeSamples.Add(new PrizeSample(15,award.starsouls[i].sid,1));
            }
        }
        if (award.cards != null) {
            for (int i = 0; i < award.cards.Count; i++) {
                temPrizeSamples.Add(new PrizeSample(5,award.cards[i].sid,award.cards[i].num));
            }
        }
        selectPrizeList = temPrizeSamples;
    }
	protected override void begin ()
	{
	    getAwardMessageInfo(award);
	    setDecForInfo(award);
	    updateInfo();
		MaskWindow.UnlockUI ();
	}

    public void updateInfo()
    {
        cardExpLabel.text = careDec;
        beastExpLabel.text = beastDec;
        if (selectPrizeList == null) return;
        for (int i=0;i<selectPrizeList.Count;i++)
        {
            PrizeSample prize = selectPrizeList[i];
            if (prize == null) continue;
            GameObject obj = NGUITools.AddChild(awardPoints, goodsVaViewPerfab);
            GoodsView goodsButton = obj.GetComponent<GoodsView>();
            goodsButton.fatherWindow = fatherWindow;
            goodsButton.onClickCallback = goodsButton.DefaultClickEvent;
            switch (prize.type) {
                case PrizeType.PRIZE_BEAST:
                    goodsButton.gameObject.SetActive(true);
                    Card beast = CardManagerment.Instance.createCard(prize.pSid);
                    goodsButton.init(beast, true);
                    break;
                case PrizeType.PRIZE_CARD:
                    goodsButton.gameObject.SetActive(true);
                    Card card = CardManagerment.Instance.createCard(prize.pSid);
                    goodsButton.init(card, true);
                    break;
                case PrizeType.PRIZE_EQUIPMENT:
                    goodsButton.gameObject.SetActive(true);
                    Equip equip = EquipManagerment.Instance.createEquip(prize.pSid);
                    goodsButton.init(equip, true);
                    break;
                case PrizeType.PRIZE_MONEY:
                    goodsButton.gameObject.SetActive(true);
                    PrizeSample prizeMoney = new PrizeSample(PrizeType.PRIZE_MONEY, 0, prize.num);
                    goodsButton.init(prizeMoney, true);
                    break;
                case PrizeType.PRIZE_PROP:
                    goodsButton.gameObject.SetActive(true);
                    Prop prop = PropManagerment.Instance.createProp(prize.pSid);
                    goodsButton.init(prop, prize.getPrizeNumByInt(), true);
                    break;
                case PrizeType.PRIZE_RMB:
                    goodsButton.gameObject.SetActive(true);
                    PrizeSample prizeRmb = new PrizeSample(PrizeType.PRIZE_RMB, 0, prize.num);
                    goodsButton.init(prizeRmb, true);
                    break;
            }
        }
        awardPoints.GetComponent<UIGrid>().Reposition();
    }
    public void init(Award wards)
    {
        //this.cl = callBack;
        this.award = wards;
    }
	public override void DoDisable ()
	{
		
	}
	  
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
	    if (gameObj.name == "enter")
	    {
	        finishWindow();
            MaskWindow.UnlockUI();
            //PvpInfoManagerment.Instance.result(true);
            //进入pvpResultWindow
	    }
		

	}
	


}
