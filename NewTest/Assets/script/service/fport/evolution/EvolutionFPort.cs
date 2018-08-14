using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 进化端口
 * @author 陈世惟
 * */
public class EvolutionFPort : BaseFPort {

	private const int TYPE_MJH = 1;//主角进化
	private const int TYPE_MTP = 2;//主角突破
	private const int TYPE_JH = 3;//卡片进化
	private int sendType;
	private string mainCardUid;
	private List<Card> foodCards;
	private CallBack callback;

	/** 主卡进化 */
	public void evolutionMainCard (int type,Card mcard,CallBack callback)
	{
		this.callback = callback;
		sendType = TYPE_MJH;
		mainCardUid = mcard.uid;
		ErlKVMessage message = new ErlKVMessage (FrontPort.EVOLUTION_MAIN);
		message.addValue ("card_uid", new ErlString (mainCardUid));
		message.addValue("type",new ErlInt(type));
		access (message);
	}


	/** 主卡突破 */
	public void surmountMainCard (Card mcard,CallBack callback)
	{
		this.callback = callback;
		sendType = TYPE_MTP;
		mainCardUid = mcard.uid;
		ErlKVMessage message = new ErlKVMessage (FrontPort.EVOLUTION_MAIN_TP);
		message.addValue ("card_uid", new ErlString (mainCardUid));
		access (message);
	}

	/** 普通卡进化,1普通卡,2万能卡 */
    public void evolutionCard(Card mcard, List<Card> foodCards, CallBack callback)
	{
        this.callback = callback;
		sendType = TYPE_JH;
		mainCardUid = mcard.uid;
        this.foodCards = foodCards;
		ErlKVMessage message = new ErlKVMessage (FrontPort.EVOLUTION_CARD);
		message.addValue ("main_uid", new ErlString (mainCardUid));
        if (!string.IsNullOrEmpty(getAllUidString())) {
            message.addValue("food_uid", new ErlString(getAllUidString()));
        }

		access (message);
	}


    /** 普通卡进化,1普通卡,2万能卡 */
    public void evolutionCard(Card mcard, Card foodCard, CallBack callback)
    {
        this.callback = callback;
        sendType = TYPE_JH;
        mainCardUid = mcard.uid;
        foodCards = new List<Card>();
        foodCards.Add(foodCard);
        ErlKVMessage message = new ErlKVMessage(FrontPort.EVOLUTION_CARD);
        message.addValue("main_uid", new ErlString(mainCardUid));
        message.addValue("food_uid", new ErlString(getAllUidString()));
        access(message);
    }

    /** 普通卡强化,1普通卡,2万能卡 */
    public void evolutionCard(Card mcard, CallBack callback)
    {
        this.callback = callback;
        sendType = TYPE_JH;
        mainCardUid = mcard.uid;
        ErlKVMessage message = new ErlKVMessage(FrontPort.EVOLUTION_CARD);
        message.addValue("main_uid", new ErlString(mainCardUid));
        access(message);
    }

    private string getAllUidString() {
        if (foodCards == null || foodCards.Count ==0)
            return "";
        string result = "";
        foreach (Card each in foodCards) {
            result += each.uid+"," ;
        }
        result = result.Remove(result.Length - 1);
        return result;
    }

	private void updateCardForSoul(){
        if (foodCards != null) {
            foreach (Card each in foodCards)
            {
                each.delStarSoulBoreByAll();
            }
        }

	}
	public override void read (ErlKVMessage message)
	{
		base.read (message);
		ErlType str = message.getValue ("msg") as ErlType;
		Card mainCard = StorageManagerment.Instance.getRole (mainCardUid);//取到仓库主卡引用
		switch(sendType)
		{
		case TYPE_MJH:

			if(str.getValueString() == "ok")//进化成功，后台推送卡片信息
			{
				updateCardForSoul();
				GuideManager.Instance.doGuide(); 
				if(callback!=null)
					callback();
			}
			else if(str.getValueString() == "change")//进化成功，手动增加卡片进化等级
			{
				updateCardForSoul();
				if(mainCard!=null)
					mainCard.evoOk(1);
				if(callback!=null)
					callback();
				mainCard = null;
			}
			else
			{
				UiManager.Instance.createMessageWindowByOneButton(LanguageConfigManager.Instance.getLanguage("Evo08"),null);
				UiManager.Instance.switchWindow<MainCardEvolutionWindow>();
			}
			break;

		case TYPE_MTP:
			if(str.getValueString() == "ok")
			{
				if(callback!=null)
					callback();
				mainCard = null;
			}
			else
			{
				UiManager.Instance.createMessageWindowByOneButton(LanguageConfigManager.Instance.getLanguage("Sur04"),null);
				UiManager.Instance.switchWindow<MainCardSurmountWindow>();
			}
			break;

		case TYPE_JH:
			if(str.getValueString() == "ok")//进化成功，后台推送卡片信息
			{
				updateCardForSoul();
				GuideManager.Instance.doGuide();
                if (callback != null)
                    callback();
			}
            else
			{
				UiManager.Instance.createMessageWindowByOneButton(LanguageConfigManager.Instance.getLanguage("Evo08"),null);
				IntensifyCardManager.Instance.clearData();
				IntensifyCardManager.Instance.intoIntensify(IntensifyCardManager.INTENSIFY_CARD_EVOLUTION);
			}
			break;
		}
	}
}
