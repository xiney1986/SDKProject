using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MineralBalanceFport : BaseFPort {

	CallBack callback;
	
	public void access (int local, CallBack callback) {   
		ErlKVMessage message = new ErlKVMessage (FrontPort.BALANCE_MINERAL);   
		message.addValue ("local", new ErlInt (local));
		this.callback = callback;
		access (message);
	}

	public override void read (ErlKVMessage message) { 
		ErlType data = message.getValue ("msg") as ErlType;
		if (data != null) {
            if (!(data is ErlArray))
            {
                string str = (data as ErlAtom).Value;
                if (str == "time_limit")
                {
                    MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("mining_time_limit"));

                }
                else if (str == "no_mineral")
                {
                    MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("mining_no_mineral"));
                }
			}
			else {
				ErlArray arr = data as ErlArray;
                List<PrizeSample> ps = new List<PrizeSample>();
				for(int i = 0;i< arr.Value.Length;i++){
					ErlArray prize = arr.Value[i] as ErlArray;
					string  strs="";
					if(prize.Value[0].getValueString() == "rmb"){
						strs = PrizeType.PRIZE_RMB +","+ "0"+","+prize.Value[1].getValueString();
					}else	if(prize.Value[0].getValueString() == "money"){
						strs = PrizeType.PRIZE_MONEY +","+ "0"+","+prize.Value[1].getValueString();
					}else if(prize.Value[0].getValueString() == "goods"){
                        strs = prize.Value[0].getValueString();
                        ErlArray values = prize.Value[1] as ErlArray;
                        strs += "," + values.Value[0].getValueString() + "," + values.Value[1].getValueString();
					}
                    ps.Add(new PrizeSample(strs, ','));
				}
                UiManager.Instance.createPrizeMessageLintWindow(ps.ToArray());

				if (this.callback != null) {
					callback ();
				}
			}
		}
	}
}
