using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// 升级血脉端口
/// </summary>
public class BloodLineFPort : BaseFPort {
    private string TYPE_RMB = "rmb";//奖励类型 人民币
    private string TYPE_CARD = "card";//奖励类型 卡片 角色
    private string TYPE_PROP = "goods";//一般道具
    private string TYPE_EQUIP = "equipment";//装备
    private string TYPE_MONEY = "money";//金币
    private string KEY_MAGICWEAPON = "artifact";//神器

	private CallBack callback;
    public BloodLineFPort() {
	}
	public void access (string uid,CallBack callback)
	{
		this.callback = callback;
        ErlKVMessage message = new ErlKVMessage(FrontPort.BLOOD_LINE);
        message.addValue("id", new ErlString(uid));
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		parseMsg (message);
	}
	//解析ErlKVMessgae
	public void parseMsg (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;
	    string msgg = type.getValueString();
	    if (msgg == "limit")
	    {
	        MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("sli002l"));
	        callback = null;
        } else if (msgg == "no_bloodline") {
            MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("sliboold01"));
             callback = null;
        } else if (msgg == "uid_error") {
            MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("sliboold02"));
             callback = null;
        } else if (msgg == "ok")
        {//血脉成功啦
            callback();
        }
        ErlArray arr = message.getValue("award") as ErlArray;
        if (arr == null ||arr.Value.Length == 0) return;
        List<PrizeSample> prize = new List<PrizeSample>();
        ErlArray arr1;
        try {
            for (int i = 0; i < arr.Value.Length; i++) {
                arr1 = arr.Value[i] as ErlArray;
                prize.Add(parse(arr1));
            }
        } catch (System.Exception ex) {
            UnityEngine.Debug.LogError("ex:" + message.toJsonString());
        }
        BloodManagement.Instance.prizes = prize.ToArray();
	}
    /// <summary>
    /// 解析奖励
    /// </summary>
    /// <param name="array"></param>
    /// <returns></returns>
    private PrizeSample parse(ErlArray array) {
        string type = (array.Value[0]).getValueString();
        string  strs="";
        if (type == TYPE_RMB) {
            strs = PrizeType.PRIZE_RMB +","+ "0"+","+array.Value[1].getValueString();
        } else if (type == TYPE_MONEY) {
            strs = PrizeType.PRIZE_MONEY + "," + "0" + "," + array.Value[1].getValueString();
        } else if (type == TYPE_CARD) {
            strs = PrizeType.PRIZE_CARD + "," + array.Value[1].getValueString() + "," + array.Value[2].getValueString();
        } else if (type == TYPE_PROP) {
            strs = PrizeType.PRIZE_PROP + "," + array.Value[1].getValueString() + "," + array.Value[2].getValueString();
        } else if (type == TYPE_EQUIP) {
            strs = PrizeType.PRIZE_EQUIPMENT + "," + array.Value[1].getValueString() + "," + array.Value[2].getValueString();
        } else if (type == KEY_MAGICWEAPON) {
            strs = PrizeType.PRIZE_MAGIC_WEAPON + "," + array.Value[1].getValueString() + "," + array.Value[2].getValueString();
        } else if (type == TempPropType.STARSOUL) {
            strs = PrizeType.PRIZE_STARSOUL + "," + array.Value[1].getValueString() + "," + array.Value[2].getValueString();
        }
        return  new PrizeSample(strs, ',');
    }

}
