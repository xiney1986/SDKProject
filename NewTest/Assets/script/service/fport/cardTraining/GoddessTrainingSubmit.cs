using UnityEngine;
using System.Collections;
/// <summary>
/// 神女挂机确定端口 
/// </summary>
public class GoddessTrainingSubmit : BaseFPort {
	CallBack<int> callback;
	private int selecttime=0;
	public void access(CallBack<int> callback, int isRmb, string beast_id, int locationIndex, int timeIndex)
	{
//	参数:
//			isrmb		是否使用钻石训练
//				可选项: 1(钻石训练) | 0(普通训练)
//				beast_id	训练的女神uid
//				location	卡槽ID
//				可选项: 1--目前女神训练只有一个卡槽
//				timeindex	时间选择
//				可选项: 1(4小时) | 2(8小时) | 3(12小时)
//				返回:
//				not_exist	卡牌不存在
//				condition_limit 训练条件不足
//				locked		卡槽未解锁
//				rmb_limit	钻笔不足
//				cd_limit	卡槽CD中
//				卡槽CD结束时间	训练成功
//				女神训练初始化
//		
		timeIndex=timeIndex+1;
		this.callback = callback;
		ErlKVMessage message = new ErlKVMessage(FrontPort.CARDTRAINING_SUBMIT);
		message.addValue("isrmb", new ErlInt(isRmb));
		message.addValue("cardid", new ErlString(beast_id));
		message.addValue("location", new ErlInt(1));
		message.addValue("type",new ErlInt(2));
		message.addValue("timeindex", new ErlInt(timeIndex));
		access(message);
	}
	public override void read(ErlKVMessage message)
	{
		if (message.Value[1] is ErlAtom)
		{
			string str = (message.Value[1] as ErlAtom).getValueString();
			switch (str)
			{
			case "not_exist":
				MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("s0479"));
				break;
			case "condition_limit":
				MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("s0480"));
				break;
			case "locked":
				MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("s0481"));
				break;
			case "rmb_limit":
				MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("s0482"));
				break;
			case "cd_limit":
				MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("s0483"));
				break;
			}
		}
		else
		{
			int time = StringKit.toInt((message.Value[1] as ErlInt).getValueString()); //StringKit.toInt((message.Value[1] as ErlInt).getValueString());
			callback(time);
		}
		
		
	}

}