using UnityEngine;
using System.Collections;
using System.Text;

/**
 * 血脉品质突破提示窗口
 * @author 
 * */
public class BigItemMessageWindow : WindowBase
{
	//按钮位置
	public UILabel oldEvoLevel;
    public UILabel newEvoLevel;
    public UILabel oldCombat;
    public UILabel newCombat;
	public UISprite oldQuality;
    public UISprite newQuality;
	public MessageHandle msg;
	CallBackMsg callback;
	
	public override void  OnAwake ()
	{
		msg = new MessageHandle ();
	}

	protected override void begin ()
	{
		base.begin ();
		
		//下面解决连续的对话框弹出无法回调问题
		GameManager.Instance.setMsgCallback (callback);
		MaskWindow.UnlockUI ();
	}

	public void initWindow (Card oldCard, CallBackMsg call)
	{
		callback = call;
        int value = 0;
        CardBaseAttribute oldCardAttr = CardManagerment.Instance.getCardAllWholeAttr(oldCard);
        BloodPointSample pointSample = BloodConfigManager.Instance.getBloodPointSampleBySid(CardSampleManager.Instance.getBloodSid(oldCard.sid));
        if (pointSample != null) {
            BloodItemSample itemSample = BloodItemConfigManager.Instance.getBolldItemSampleBySid(BloodConfigManager.Instance.getCurrentItemSid(pointSample, oldCard.cardBloodLevel));
            if (itemSample == null) return;
            bloodEffect[] effect = itemSample.effects;
            for (int i = 0; i < effect.Length; i++) {
                value = effect[i].perAllAttr;
            }
        }
        int[] costs = CommandConfigManager.Instance.getEvoCostByQuality(oldCard.getQualityId());
        oldEvoLevel.text = oldCard.getEvoLevel() + "";
        newEvoLevel.text = oldCard.getEvoLevel() + "[FF0000]-" + (oldCard.getEvoLevel() - oldCard.getEvoLevelForBlood(oldCard.getEvoTimes() == 0 ? costs[0] : costs[oldCard.getEvoTimes() - 1])) + "[-]";
        oldCombat.text = oldCard.getCardCombat() + "";
        newCombat.text = oldCard.getCardCombat() + "[358C35]+" + value + "%" + "[-]";
        oldQuality.spriteName = QualityManagerment.qualityIDToStringByBG(oldCard.getQualityId());
        newQuality.spriteName = QualityManagerment.qualityIDToStringByBG(oldCard.getQualityId() +1);
	}
	
	protected override void DoEnable ()
	{
		base.DoEnable ();
	}

    public override void buttonEventBase(GameObject gameObj) {
        base.buttonEventBase(gameObj);
        if (gameObj.name == "button_ok") {
            msg.msgEvent = msg_event.dialogOK;
            callback(msg);
            finishWindow();
        } else if (gameObj.name == "button_cancel") {
            finishWindow();
        }
    }
}
