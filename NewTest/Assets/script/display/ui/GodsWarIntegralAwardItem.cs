using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 诸神战奖励Item
/// </summary>
public class GodsWarIntegralAwardItem : MonoBase
{
	public UILabel description;
    public ButtonBase receiveButton;
	public ButtonBase receiveDoubleButton;
	private GodsWarPrizeSample data;
    private WindowBase fwin;
    public GoodsView[] awardViews;
	/// <summary>
	/// 是否领取
	/// </summary>
	public UISprite recived;
	/// <summary>
	/// 领取的倍数
	/// </summary>
	private int count=1;
	public List<int> scores;
	CallBack callback;


    /// <summary>
    /// 领取积分按钮点击
    /// </summary>
    private void onReceiveButtonClick(GameObject obj)
    {
		receiveButton.gameObject.SetActive(false);
		receiveDoubleButton.gameObject.SetActive(false);
		recived.gameObject.SetActive(true);
        count = 1;
		FPortManager.Instance.getFPort<GodsWarIntegralAwardFport>().access(updateUI,data.integral,1);
    }
	/// <summary>
	/// 双倍领取积分按钮点击
	/// </summary>
	private void onReceiveDoubleButtonClick(GameObject obj)
	{
		
		if(checkRMB())
			UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
			win.dialogCloseUnlockUI = false;
			win.initWindow(2,LanguageConfigManager.Instance.getLanguage("s0094"),LanguageConfigManager.Instance.getLanguage("s0093"),
				               LanguageConfigManager.Instance.getLanguage("godsWar_52",data.costRMB.ToString()),(msg)=>{
				if (msg.buttonID == MessageHandle.BUTTON_LEFT) 
				{
					MaskWindow.UnlockUI();
					return;
				}
				else if (msg.buttonID == MessageHandle.BUTTON_RIGHT){
						receiveButton.gameObject.SetActive(false);
						receiveDoubleButton.gameObject.SetActive(false);
						recived.gameObject.SetActive(true);
                        count = 2;
						FPortManager.Instance.getFPort<GodsWarIntegralAwardFport>().access(updateUI,data.integral,2);
					}
			});
		});
		else
        UiManager.Instance.openDialogWindow<MessageWindow>((win) => {
            win.initWindow(2, LanguageConfigManager.Instance.getLanguage("s0094"), LanguageConfigManager.Instance.getLanguage("s0324"),
                           LanguageConfigManager.Instance.getLanguage("s0158"), (msg) => {
                               if (msg.buttonID == MessageHandle.BUTTON_RIGHT) {
                                   fwin.finishWindow();
                                   UiManager.Instance.openWindow<rechargeWindow>();
                               } else {
                                   MaskWindow.UnlockUI();
                               }
                           });
        });
	}

	/// <summary>
	/// 初始化条目内容
	/// </summary>
	public void initItem(GodsWarPrizeSample sample,WindowBase win,CallBack  callback)
	{
		this.data = sample;
		this.callback = callback;
		this.fwin = win;
		initPrize();
		initButtons();
	}
	/// <summary>
	/// 初始化buttons
	/// </summary>
	public void initButtons()
	{
		setButtonActive();
		receiveButton.fatherWindow = fwin;
		receiveDoubleButton.fatherWindow = fwin;
		receiveButton.onClickEvent = onReceiveButtonClick;
		receiveDoubleButton.onClickEvent = onReceiveDoubleButtonClick;
	}
	/// <summary>
	/// 设置条件
	/// </summary>
	public void setButtonActive()
	{
		scores = GodsWarManagerment.Instance.currentScores;
		if(scores!=null)
		{
			for (int i = 0; i < scores.Count; i++) {
				if(data.integral==scores[i]){
					receiveButton.gameObject.SetActive(false);
					receiveDoubleButton.gameObject.SetActive(false);
					recived.gameObject.SetActive(true);
				}
			}
		}else{
			scores = new List<int>();
		}
		if(!checkIntegral())
		{
			receiveButton.disableButton(true);
			receiveDoubleButton.disableButton(true);
			
		}
	}

	/// <summary>
	/// 更新UI
	/// </summary>
	private void updateUI()
	{
		List<int> currentScores = GodsWarManagerment.Instance.currentScores;
		if(currentScores==null)
			currentScores = new List<int>();
		if(!currentScores.Contains(data.integral))
			currentScores.Add(data.integral);
		GodsWarManagerment.Instance.currentScores = currentScores;
		//UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage("godsWar_51",data.des,count.ToString()));
        //UiManager
        List<PrizeSample> itemm=new List<PrizeSample>();
	    for (int i=0;i<data.item.Count;i++)
	    {
	        if (count == 2)
	        {
                itemm.Add(data.item[i]);
                itemm.Add(data.item[i]);
	        }
	        else
	        {
	            itemm.Add(data.item[i]);
	        }
	    }
        UiManager.Instance.openDialogWindow<PropMessageLineWindow>((win) =>
        {
            win.Initialize(itemm, true);
        });
		receiveButton.gameObject.SetActive(false);
		receiveDoubleButton.gameObject.SetActive(false);
		recived.gameObject.SetActive(true);

		if(callback!=null)
			callback();
	}
	/// <summary>
	/// 检测钻石是否充足
	/// </summary>
	public bool checkRMB()
	{
		if(data.costRMB<=UserManager.Instance.self.getRMB())
			return true;
		return false;
	}
	/// <summary>
	/// 检测积分是否足够
	/// </summary>
	public bool checkIntegral()
	{
		if(GodsWarManagerment.Instance.self.todayIntegral< data.integral)
			return false;
		return true;
	}
	/// <summary>
	/// 初始化奖励
	/// </summary>
	public void initPrize()
	{
		description.text = LanguageConfigManager.Instance.getLanguage("godsWar_44",data.des,data.integral.ToString());
		for(int i=0;i<awardViews.Length;i++)
		{
			if(i<data.item.Count)
			{
				awardViews[i].init(data.item[i]);
				awardViews[i].fatherWindow = fwin;
			}
			else
				awardViews[i].gameObject.SetActive(false);
		}
	}
}

