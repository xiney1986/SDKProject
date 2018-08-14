using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//获取恶魔挑战信息
public class GetBossAttackFPort : BaseFPort {
    private CallBack callback;

    public void access(int sid,CallBack callback) {
        this.callback = callback;
        ErlKVMessage message = new ErlKVMessage(FrontPort.GET_ONEONONE_INFO);
        message.addValue("sid", new ErlInt(sid));
        access(message);
    }

    public override void read(ErlKVMessage message) {
        ErlType type = message.getValue("msg") as ErlType;
        if (type == null) return;
        if (type is ErlArray) {
            ErlArray erlArray = message.getValue("msg") as ErlArray;
            if (erlArray.Value.Length == 0)
                return;
            int fightedTimes = StringKit.toInt(erlArray.Value[0].getValueString());

            AttackBossOneOnOneManager.Instance.canChallengeTimes = CommandConfigManager.Instance.getTimesOfDay() - fightedTimes;

            AttackBossOneOnOneManager.Instance.bossSid = StringKit.toInt(erlArray.Value[1].getValueString());
            string beastUid = erlArray.Value[2].getValueString();
            if (beastUid != "0") {
                AttackBossOneOnOneManager.Instance.choosedBeast = StorageManagerment.Instance.getBeast(beastUid);
            }
            AttackBossOneOnOneManager.Instance.totalDamage = erlArray.Value[3].getValueString();

            ErlArray cardInfoErl = erlArray.Value[4] as ErlArray;
            List<string> list = new List<string>();
            for (int i = 0; i < cardInfoErl.Value.Length; i++)
            {
                ErlArray array = cardInfoErl.Value[i] as ErlArray;
                if (array.Value[1].getValueString() == "2")
                    list.Add(array.Value[0].getValueString());
            }
            AttackBossOneOnOneManager.Instance.usedCardUid = list;

            ErlArray fightInfoErl = erlArray.Value[5] as ErlArray;
            AttackBossOneOnOneManager.Instance.buyTimes = StringKit.toInt(erlArray.Value[6].getValueString());
            List<FightInfo> fightInfoList = new List<FightInfo>();
            for (int k = 0; k < fightInfoErl.Value.Length; k++) {
                FightInfo fight = new FightInfo();
                ErlArray tempErl = fightInfoErl.Value[k] as ErlArray;
                if (tempErl.Value.Length == 0) continue;
                fight.cardSid = StringKit.toInt(tempErl.Value[0].getValueString());
                fight.beastSid = StringKit.toInt(tempErl.Value[1].getValueString());
                fight.bossSid = StringKit.toInt(tempErl.Value[2].getValueString());
                fight.damage = tempErl.Value[3].getValueString();
                fightInfoList.Add(fight);
            }
            AttackBossOneOnOneManager.Instance.fightInfo = fightInfoList;
            if (callback != null) {
                callback();
                callback = null;
            }
        } else {
			if (callback != null) {
				callback();
				callback = null;
			}
            //MaskWindow.UnlockUI();
        }
    }
}
