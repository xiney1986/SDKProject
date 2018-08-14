using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 获取签到信息通讯端口
/// </summary>
public class GetSignInInfoFport : BaseFPort {

	/** 回调函数 */
	private CallBack callback;
	private int sid;

	public void getSignInInfo (CallBack callback)
	{  
		this.callback = callback;
        ErlKVMessage message = new ErlKVMessage(FrontPort.GET_SIGNIN_INFO); 
		access (message);
	}
	public override void read (ErlKVMessage message) {
		base.read (message);
        ErlType type = message.getValue("msg") as ErlType;
        if (type == null) return;
        List<int> months = new List<int>();
        if (type is ErlArray) {
            ErlArray erlArray = message.getValue("msg") as ErlArray;
            if (erlArray.Value.Length == 0) {
                SignInManagerment.Instance.month = ServerTimeKit.getCurrentMonth();
                SignInManagerment.Instance.sign_inTimes = 0;
                SignInManagerment.Instance.stateList = new System.Collections.Generic.List<int>();
                if (callback != null) {
                    callback();
                    callback = null;
                }
                return;
            }
            for (int i = 0; i < erlArray.Value.Length; i++) { 
                ErlArray erlArrayTmp = erlArray.Value[i] as ErlArray;
                int month = StringKit.toInt((erlArrayTmp.Value[0].getValueString()));
                months.Add(month);
                if (month == ServerTimeKit.getCurrentMonth()) {
                    SignInManagerment.Instance.month = StringKit.toInt(erlArrayTmp.Value[0].getValueString());
                    SignInManagerment.Instance.sign_inTimes = StringKit.toInt(erlArrayTmp.Value[1].getValueString());
                    ErlArray erl = erlArrayTmp.Value[2] as ErlArray;
                    SignInManagerment.Instance.stateList.Clear();
                    for (int k = 0; k < erl.Value.Length; k++) {
                        SignInManagerment.Instance.stateList.Add(StringKit.toInt(erl.Value[k].getValueString()));
                    }
                    if (callback != null) {
                        callback();
                        callback = null;
                    }
                    return;
                }
            }
            if (!months.Contains(ServerTimeKit.getCurrentMonth())) { 
                SignInManagerment.Instance.month = ServerTimeKit.getCurrentMonth();
                SignInManagerment.Instance.sign_inTimes = 0;
                SignInManagerment.Instance.stateList = new System.Collections.Generic.List<int>();
                if (callback != null) {
                    callback();
                    callback = null;
                }
                return;
            }
            
        }
	}

}
