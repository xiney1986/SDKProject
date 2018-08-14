using UnityEngine;
using System.Collections;

/// <summary>
/// 类说明：卡片属性升级内容
/// </summary>
public class CardAttrLevelUpContent : MonoBehaviour
{

    /* fields */
    /** 卡片属性升级条目列表 */
    public CardAttrLevelUpItem[] items;
	[HideInInspector] public bool isShowPrizeUpdate=false;
	[HideInInspector] public float waitShowPrizeTime=0f;


	void OnDisable ()
	{
		isShowPrizeUpdate = false;
		waitShowPrizeTime=0f;
	}

    /* methods */
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="role">卡片</param>
	public void Initialize(Card card, AttrAddInfo attrAddInfo)
    {
        updateAttrLevelUpItem(card, attrAddInfo);
    }
    /// <summary>
    /// 更新属性升级条目
    /// </summary>
    /// <param name="attrAddInfo"></param>
    public void updateAttrLevelUpItem(Card card, AttrAddInfo attrAddInfo)
    {
        if (card == null || attrAddInfo == null) return;
        updateActiveByAll(false);
        int index = 0;
		bool isUpLevel=false;
        if (attrAddInfo.HpExp > 0)
        {
            int oldHpAttr = CardManagerment.Instance.getCardAppendAttr(attrAddInfo.OldHpGrade, AttributeType.hp);
            int hpAttr = CardManagerment.Instance.getCardAppendAttr(attrAddInfo.HpGrade, AttributeType.hp);
			isUpLevel=items[index].updateAttributes(index,AttrChangeType.HP,attrAddInfo.oldHpExp,card.getHPExp(), oldHpAttr, hpAttr);
            index++;
        }
        if (attrAddInfo.AttExp > 0)
        {
            int oldAttAttr = CardManagerment.Instance.getCardAppendAttr(attrAddInfo.OldAttGrade, AttributeType.attack);
            int attAttr = CardManagerment.Instance.getCardAppendAttr(attrAddInfo.AttGrade, AttributeType.attack);
			isUpLevel=items[index].updateAttributes(index,AttrChangeType.ATTACK, attrAddInfo.oldAttExp,card.getATTExp(), oldAttAttr, attAttr);
            index++;
        }
        if (attrAddInfo.DefExp > 0)
        {
            int oldDefAttr = CardManagerment.Instance.getCardAppendAttr(attrAddInfo.OldDefGrade, AttributeType.defecse);
            int defAttr = CardManagerment.Instance.getCardAppendAttr(attrAddInfo.DefGrade, AttributeType.defecse);
			isUpLevel=items[index].updateAttributes(index,AttrChangeType.DEFENSE,attrAddInfo.oldDefExp, card.getDEFExp(), oldDefAttr, defAttr);
            index++;
        }
        if (attrAddInfo.MagExp > 0)
        {
            int oldMagAttr = CardManagerment.Instance.getCardAppendAttr(attrAddInfo.OldMagGrade, AttributeType.magic);
            int magAttr = CardManagerment.Instance.getCardAppendAttr(attrAddInfo.MagGrade, AttributeType.magic);
			isUpLevel=items[index].updateAttributes(index,AttrChangeType.MAGIC,attrAddInfo.oldMagExp, card.getMAGICExp(), oldMagAttr, magAttr);
            index++;
        }
        if (attrAddInfo.DexExp > 0)
        {
            int oldDexAttr = CardManagerment.Instance.getCardAppendAttr(attrAddInfo.OldDexGrade, AttributeType.agile);
            int dexAttr = CardManagerment.Instance.getCardAppendAttr(attrAddInfo.DexGrade, AttributeType.agile);
			isUpLevel=items[index].updateAttributes(index,AttrChangeType.AGILE,attrAddInfo.oldDexExp, card.getAGILEExp(), oldDexAttr, dexAttr);
            index++;
        }
		isShowPrizeUpdate = true;
		if(isUpLevel)waitShowPrizeTime = 1.5f;
    }
    /// <summary>
    /// 更新所有条目对象显示状态
    /// </summary>
    /// <param name="isActive"></param>
    public void updateActiveByAll(bool isActive)
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].gameObject.SetActive(isActive);
        }
    }

	void Update ()
	{
		if (isShowPrizeUpdate) 
		{
			bool b=checkShowRestorePrize();
			if(b)
			{
				StartCoroutine(showRestorePrize());
				isShowPrizeUpdate=false;
				waitShowPrizeTime=0;
			}
		}
	}

	bool checkShowRestorePrize()
	{
		foreach (CardAttrLevelUpItem item in items) {
			if(item.expBar==null) continue;
			if(item.expBar.endCall!=null) return false;
		}
		return true;
	}

	
	/// <summary>
	/// 显示返回奖励
	/// </summary>
	public IEnumerator showRestorePrize()
	{
		yield return new WaitForSeconds(waitShowPrizeTime);
		if (IntensifyCardManager.Instance.addOnRestorePrize!=null)
		{
			UiManager.Instance.openDialogWindow<AllAwardViewWindow>((win)=>{	
				win.Initialize(IntensifyCardManager.Instance.addOnRestorePrize,LanguageConfigManager.Instance.getLanguage("s0120"));
			});
			IntensifyCardManager.Instance.addOnRestorePrize=null;
		}
	}
}
