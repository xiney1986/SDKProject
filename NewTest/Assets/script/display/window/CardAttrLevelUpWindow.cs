using UnityEngine;
using System.Collections;

/// <summary>
/// 类说明：卡片属性升级窗口
/// </summary>
public class CardAttrLevelUpWindow : WindowBase
{

    /* fields */
    /** 卡片属性内容 */
    public CardAttrContent cardAttrContent;
    /** 卡片属性升级内容 */
    public CardAttrLevelUpContent cardAttrUpContent;
    /** 点击继续按钮回调函数 */
    private CallBack continueCallback;
    /** 卡片 */
    Card card;

	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}

    /* methods */
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="card">主卡片</param>
    /// <param name="attrAddInfo">附加属性信息对象</param>
    /// <param name="closeCallback">关闭回调方法</param>
    public void Initialize(Card card, AttrAddInfo attrAddInfo, CallBack continueCallback)
    {
        this.card = card;
        this.continueCallback = continueCallback;
        cardAttrContent.Initialize(this, card);
        cardAttrContent.gameObject.SetActive(true);
        cardAttrUpContent.Initialize(card, attrAddInfo);
    }
    /// <summary>
    /// 按钮点击事件
    /// </summary>
    /// <param name="gameObj"></param>
    public override void buttonEventBase(GameObject gameObj)
    {
        base.buttonEventBase(gameObj);
        if (gameObj.name == "continue")
        {
            IntensifyCardManager.Instance.setMainCard(card);
			finishWindow();
        }
        if (gameObj.name == "close")
        {
            if (!GuideManager.Instance.isGuideComplete())
            {
                ArmyManager.Instance.cleanAllEditArmy();
                GuideManager.Instance.doGuide();
            }
			UiManager.Instance.openMainWindow();
        }
    }
}
