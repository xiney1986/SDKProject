using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct platfromID
{
    public const string MonoAndroid = "110";
    public const string MonoIOS = "139";
}

public class MonthCardContent : MonoBehaviour
{

    public GameObject go_rewardListContent;
    public ButtonBase btn_buy;
    public ButtonBase btn_receive;
    //public UILabel lab_cardInfo;
    public UILabel lab_timeInfo;
    public WindowBase fatherWindow;
    public UISprite sprite_next;
    public UISprite sprite_pre;
    public UIScrollView scrollView;
    public UILabel lab_receive;
    public GameObject go_goodsPrefab;
    public GameObject monthcardtip;//陌陌平台要屏蔽此提示


    void Awake()
    {
        btn_receive.disableButton(true);
        //		EventDelegate.Add(btn_buy.onClick,M_onClickBuy);
        btn_buy.onClickEvent = M_onClickBuy;
        //		EventDelegate.Add(btn_receive.onClick,M_onClickReceive);
        btn_receive.onClickEvent = M_onClickReceive;
        updateMonthCardInfo();
    }

    private void OnEnable()
    {
        updateMonthCardInfo();
        MonthCardSampleManager.Instance.getAllSampleIds();
        //M_creatDayReward();
    }

    //	public void refreshCardStatus()
    //	{
    //		updateMonthCardInfo ();
    //		NoticeMonthCardFPort sp=FPortManager.Instance.getFPort ("NoticeMonthCardFPort") as NoticeMonthCardFPort;
    //		sp.access_get(updateMonthCardInfo);
    //	}
    //更新月卡领取按钮
    private void updateReceiveButton()
    {
        MaskWindow.UnlockUI();
        int state = NoticeManagerment.Instance.getMonthCardRewardState();
        if (state == NoticeManagerment.MONTHCARD_STATE_VALID)
        {
            btn_receive.disableButton(false);
        }
        else
        {
            btn_receive.disableButton(true);
            if (state == NoticeManagerment.MONTHCARD_STATE_FINISHED)
            {
                lab_receive.text = LanguageConfigManager.Instance.getLanguage("recharge02");
            }
            else if (state == NoticeManagerment.MONTHCARD_STATE_FINISHED)
            {
                lab_receive.text = LanguageConfigManager.Instance.getLanguage("s0309");
            }
        }
    }
    //更新购买的月卡信息
    private void updateMonthCardInfo()
    {
        updateReceiveButton();
        int[] monthCardDueDate = NoticeManagerment.Instance.monthCardDueDate;
        if (monthCardDueDate == null || monthCardDueDate.Length == 0)
        {
            lab_timeInfo.text = LanguageConfigManager.Instance.getLanguage("monthCardNoBuy");
        }
        else
        {
            string info = LanguageConfigManager.Instance.getLanguage("monthCardTimeInfo", monthCardDueDate[0].ToString(), monthCardDueDate[1].ToString(), monthCardDueDate[2].ToString());
            lab_timeInfo.text = info;
        }
    }
    public void initContent(WindowBase win)
    {
        fatherWindow = win;
        btn_receive.fatherWindow = fatherWindow;
        btn_buy.fatherWindow = fatherWindow;
        MonthCardSampleManager.Instance.getAllSampleIds();
        M_creatDayReward();
    }
    private void M_creatDayReward()
    {
        List<PrizeSample> rewardList = MonthCardSampleManager.Instance.getDayReward();
        PrizeSample prize;
        GameObject newItem;
        GoodsView goodsButton;
        int i = 0, length = rewardList.Count;
        for (; i < length; i++)
        {
            prize = rewardList[i];
            newItem = NGUITools.AddChild(go_rewardListContent, go_goodsPrefab);
            newItem.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            goodsButton = newItem.GetComponent<GoodsView>();
            goodsButton.fatherWindow = fatherWindow;
            goodsButton.onClickCallback = goodsButton.DefaultClickEvent;

            if (prize == null)
            {
                return;
            }
            else
            {
                switch (prize.type)
                {
                    case PrizeType.PRIZE_BEAST:
                        goodsButton.gameObject.SetActive(true);
                        Card beast = CardManagerment.Instance.createCard(prize.pSid);
                        goodsButton.init(beast);
                        break;
                    case PrizeType.PRIZE_CARD:
                        goodsButton.gameObject.SetActive(true);
                        Card card = CardManagerment.Instance.createCard(prize.pSid);
                        goodsButton.init(card);
                        break;
                    case PrizeType.PRIZE_EQUIPMENT:
                        goodsButton.gameObject.SetActive(true);
                        Equip equip = EquipManagerment.Instance.createEquip(prize.pSid);
                        goodsButton.init(equip);
                        break;
                    case PrizeType.PRIZE_MONEY:
                        goodsButton.gameObject.SetActive(true);
                        goodsButton.init(prize);
                        break;
                    case PrizeType.PRIZE_PROP:
                        goodsButton.gameObject.SetActive(true);
                        Prop prop = PropManagerment.Instance.createProp(prize.pSid);
                        goodsButton.init(prop, prize.getPrizeNumByInt());
                        break;
                    case PrizeType.PRIZE_RMB:
                        goodsButton.gameObject.SetActive(true);
                        goodsButton.init(prize);
                        break;
                    case PrizeType.PRIZE_MAGIC_WEAPON:
                        goodsButton.gameObject.SetActive(true);
                        MagicWeapon magicweapon = MagicWeaponManagerment.Instance.createMagicWeapon(prize.pSid);
                        goodsButton.init(magicweapon);
                        break;
                }
            }
        }
        go_rewardListContent.GetComponent<UIGrid>().Reposition();
        sprite_pre.gameObject.SetActive(length > 4);
        sprite_next.gameObject.SetActive(length > 4);
    }
    private void M_onClickBuy(GameObject obj)
    {
        MaskWindow.LockUI();
        UiManager.Instance.openWindow<rechargeWindow>();
    }
    private void M_onClickReceive(GameObject obj)
    {
        MaskWindow.LockUI();
        NoticeMonthCardFPort sp = FPortManager.Instance.getFPort("NoticeMonthCardFPort") as NoticeMonthCardFPort;
        sp.access_receive(updateReceiveButton);
    }

}

