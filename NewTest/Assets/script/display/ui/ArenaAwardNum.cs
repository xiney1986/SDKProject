using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArenaAwardNum : MonoBehaviour {

    public UISprite bg;
    public UILabel label;
    public int type;
	/** 积分奖励扩展--奖励 */
	public GoodsView prize;
	/** 我的积分 */
	public UILabel myIntegral;
	/** 可领取时的特效 */
	public GameObject effect;
	/// <summary>
	/// 刷新积分奖励相关UI
	/// </summary>
	public void  updateIntegralUI(){
		List<ArenaAwardInfo> integralAwardInfos = ArenaAwardManager.Instance.integralAwardInfos;
		if (integralAwardInfos == null || integralAwardInfos.Count ==0)
			return;
		myIntegral.text = "(" + ArenaAwardManager.Instance.getCurrentIntegralAwardInfo().sample.condition.ToString() + ")";
		prize.init (ArenaAwardManager.Instance.getCurrentIntegralAwardInfo().sample.prizes[0]);
		if (ArenaAwardManager.Instance.isCanGetIntegralAward ()) {
			effect.SetActive(true);
		} else {
			effect.SetActive(false);
		}
		
	}
	public void loadData()
    {
		//由于决赛和竞猜是一个领奖界面，要一起处理
        if (type == ArenaAwardWindow.TYPE_INTEGRAL)
            FPortManager.Instance.getFPort<ArenaGetAwardInfoIntegralFPort>().access(OnDataLoaded);
        else if (type == ArenaAwardWindow.TYPE_GUESS)
            FPortManager.Instance.getFPort<ArenaGetAwardInfoGuessFPort>().access(OnDataLoaded);
        else if(type == ArenaAwardWindow.TYPE_FINAL)
        {
            WindowBase win = UiManager.Instance.CurrentWindow;
            if(win is ArenaAwardWindow)
                win = win.GetFatherWindow();
            if(win is ArenaFinalWindow && ArenaManager.instance.state == ArenaManager.STATE_RESET && (win as ArenaFinalWindow).getMyRank() > 0)
            {
                FPortManager.Instance.getFPort<ArenaGetAwardInfoFinalFPort>().access(OnDataLoaded);
            }
        }
    }
	public void loadFinalAndGuess()
	{
		//由于决赛和竞猜是一个领奖界面，要一起处理
		if (type == ArenaAwardWindow.TYPE_GUESS)
			FPortManager.Instance.getFPort<ArenaGetAwardInfoGuessFPort>().access(OnDataLoaded);
		else if(type == ArenaAwardWindow.TYPE_FINAL)
		{
			WindowBase win = UiManager.Instance.CurrentWindow;
			if(win is ArenaAwardWindow)
				win = win.GetFatherWindow();
			if(win is ArenaFinalWindow && ArenaManager.instance.state == ArenaManager.STATE_RESET && (win as ArenaFinalWindow).getMyRank() > 0)
			{
				FPortManager.Instance.getFPort<ArenaGetAwardInfoFinalFPort>().access(OnDataLoaded);
			}
		}
	}
    
    public void OnDataLoaded(List<ArenaAwardInfo> list)
    {
		if (type == ArenaAwardWindow.TYPE_INTEGRAL) {
			updateIntegralUI();
			MaskWindow.UnlockUI();
		}
		WindowBase win = UiManager.Instance.CurrentWindow;
		if((win is ArenaAuditionsWindow || win is ArenaFinalWindow)&&win.gameObject.activeInHierarchy)
        	OnDataLoaded(list, false);
    }

    void OnDataLoaded(List<ArenaAwardInfo> list,bool received)
    {
		WindowBase win = UiManager.Instance.CurrentWindow;
		if ((win is ArenaAuditionsWindow || win is ArenaFinalWindow) && win.gameObject.activeInHierarchy) {
			if (type == ArenaAwardWindow.TYPE_FINAL) {
				SetShow (!received);
				return;
			}

			bool show = false;
			if (list != null && list.Count > 0) {
				foreach (ArenaAwardInfo info in list) {
					bool canReceived = false;
					if (info.sample.type == ArenaAwardWindow.TYPE_GUESS) {
						canReceived = info.condition > 0;
					} else {
						canReceived = info.condition >= info.sample.condition;
					}
					if (canReceived && !info.received) {
						show = true;
						break;
					}
				}
			}
			SetShow (show);
		}
    }

    public void SetShow(bool show)
    {
        bg.gameObject.SetActive(show);
        label.gameObject.SetActive(show);
    }
}
