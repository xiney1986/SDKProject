using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonCompoundProp : ButtonBase
{
	public Prop prop;
    private Prop tempProp;
    int minNum = 0;
    int maxNum = 0;
	private int type;
	public void initButton (Prop prop)
	{
		this.prop = prop;
        switch (prop.sid) { 
            case 71196:
                tempProp = PropManagerment.Instance.createProp(71197);
                break;
            case 71197:
                tempProp = PropManagerment.Instance.createProp(71198);
                break;
            case 71198:
                tempProp = PropManagerment.Instance.createProp(71199);
                break;
            case 71199:
                tempProp = PropManagerment.Instance.createProp(71200);
                break;
        }
        if (StorageManagerment.Instance.getProp(prop.sid) != null)
            maxNum = (int)(StorageManagerment.Instance.getProp(prop.sid).getNum() / 2);
        minNum = 1;
        //if(StorageManagerment.Instance.getProp(tempProp.sid).getNum())
	}
	
	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
	    if (prop.isShenGeProp())
	    {
            UiManager.Instance.openDialogWindow<ShenGeGroupWindow>((win) => {
                win.Initialize(prop, 0, ShenGeManager.STORAGE);
            });
	        return;
	    }
	    if (maxNum == 0) {
            UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {//万能卡不足
                win.Initialize(LanguageConfigManager.Instance.getLanguage("prefabzc56", prop.getName()));
            });
            return;
        }
        UiManager.Instance.openDialogWindow<CompoundWindow>((win) => {
            win.init(tempProp as object, maxNum, minNum, 1, 1, compound);
        });
	}
    /// <summary>
    /// 合成窗口确定按钮的回调
    /// </summary>
    /// <param name="msg"></param>
    public void compound(MessageHandle msg) {
        if (msg.msgEvent == msg_event.dialogOK) {
            ExchangeFPort exf = FPortManager.Instance.getFPort("ExchangeFPort") as ExchangeFPort;
            List<Exchange> exchangeList = ExchangeManagerment.Instance.getCanExchangeWNCard();
            ExchangeSample sample;
            for (int i = 0; i < exchangeList.Count; i++) {
                sample = ExchangeSampleManager.Instance.getExchangeSampleBySid(exchangeList[i].sid);
                if (sample.exchangeSid == tempProp.sid) {
                    exf.exchange(exchangeList[i].sid, msg.msgNum, compoundOK);
                }
            }
        }
    }
    //合成成功的回调
    private void compoundOK(int sid ,int num) {
        UiManager.Instance.openDialogWindow<MessageLineWindow>((win) => {
            win.Initialize(LanguageConfigManager.Instance.getLanguage("prefabzc57"));
        });
        (fatherWindow as StoreWindow).updateContent();
    }
}
