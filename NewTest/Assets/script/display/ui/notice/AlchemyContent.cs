
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AlchemyContent:MonoBase
{
    public UILabel UI_MyRMB;
    public UILabel UI_MyGold;
    //public UILabel UI_RmbConsumeReduce;
    //public UISprite UI_DelLine;

	public GameObject effectPos;//暴击特效位置
	public UILabel moneyLabel;
	public UILabel rmbConsume; //本次炼金消耗
	public UILabel vipAdd;//vip暴击加成
	public ButtonAlchemy buyButton;
	public ButtonAlchemy buyButtonTen;//炼金10次按钮
	public GameObject alchemyEffect;
	//public UILabel vipReduce;//vip减免
	[HideInInspector]
	public WindowBase
		win;//活动窗口
	public GameObject bombEffect;
	public GameObject doubleEffect;
	private int oldGold;
	private int newGold;
	private bool needRefresh;
	private int step;
	private int lianjinSid;
	private int integral;
	private List<PrizeSample> showPrizes = new List<PrizeSample> ();
	public void initContent (WindowBase win)
	{
		this.win = win;
		buyButton.content = this;
		buyButton.fatherWindow = win;
		buyButtonTen.content=this;
		buyButtonTen.fatherWindow=win;
		int level = UserManager.Instance.self.getVipLevel ();
		if (level > 0) {
			//vipReduce.text = LanguageConfigManager.Instance.getLanguage ("AlchemyContent04", level.ToString (), vipRed (level).ToString ());
			if (VipManagerment.Instance.getVipbyLevel (UserManager.Instance.self.getVipLevel ()).privilege.alchemyAdd > 0) {
				string a = VipManagerment.Instance.getVipbyLevel (UserManager.Instance.self.getVipLevel ()).privilege.alchemyAdd * 0.0001f * 100 + "%";
				vipAdd.text = LanguageConfigManager.Instance.getLanguage ("AlchemyContent09", level.ToString (), a);
			} else {
				vipAdd.text = LanguageConfigManager.Instance.getLanguage("AlchemyContent10");
			}
		}
		else {
			//vipReduce.text = LanguageConfigManager.Instance.getLanguage ("AlchemyContent05", "1", vipRed (1).ToString ());
			vipAdd.text = LanguageConfigManager.Instance.getLanguage("AlchemyContent10");
		}
        UI_MyGold.text = UserManager.Instance.self.getMoney().ToString();
		UI_MyRMB.text = UserManager.Instance.self.getRMB().ToString();
		refreshInfo ();
	}

	public void refreshInfo ()
	{
		moneyLabel.text = "X" + NoticeManagerment.Instance.getAlchemyMoney ();
        rmbConsume.text = NoticeManagerment.Instance.getAlchemyConsume(0).ToString();
        //vipReduce.transform.localPosition = new Vector3(UI_RmbConsumeReduce.width + UI_RmbConsumeReduce.transform.localPosition.x + 20, 0, 0);

        UI_MyRMB.text = UserManager.Instance.self.getRMB().ToString();
		//TweenLabelNumber.Begin方法存在Bug暂时不用,实现方式改为：增加oldGodl,newGold,needfresh变量
		//增加refreshGold,setStep,Update方法.
		//UI_MyGold.text = UserManager.Instance.self.getMoney().ToString();
		//TweenLabelNumber.Begin(UI_MyGold.gameObject, 2 ,UserManager.Instance.self.getMoney()).method = UITweener.Method.EaseInOut;
		newGold = UserManager.Instance.self.getMoney();
		oldGold = newGold - NoticeManagerment.Instance.getAlchemyMoney ();
		needRefresh = true;
		showAward ();
	}

	private int vipRed (int level)
	{
		return level > 0 ? VipManagerment.Instance.getVipbyLevel (level).privilege.alchemyFactor : 0;
	}

	public int getConsume ()
	{
		return NoticeManagerment.Instance.getAlchemyConsume (vipRed (UserManager.Instance.self.getVipLevel ()));
//		int level = UserManager.Instance.self.getVipLevel ();
//		return level > 0 ? NoticeManagerment.Instance.getAlchemyConsume (VipManagerment.Instance.getVipbyLevel (level).privilege.alchemyFactor) :
//			NoticeManagerment.Instance.getAlchemyConsume (0);
	}
	//刷新金币
	private void refreshGold()
	{
		if(oldGold < newGold)
		{
			oldGold += step;
			UI_MyGold.text = oldGold.ToString();
		}
		if(oldGold == newGold) needRefresh = false;
	}
	//设置步长
	private void setStep()
	{
		step = (newGold - oldGold) / 10;
		if (step < 1)
			step = 1;
	}
	//刷新每帧调用
	void Update()
	{
		setStep();
		if(needRefresh)
			refreshGold();
	}
	private void showAward(){
		int index = 0;
		int inter = 0;
		NoticeSample noticeSample = null;
		showPrizes.Clear ();
		//List<PrizeSample> showPrizes = new List<PrizeSample> ();
		while (index < NoticeManagerment.Instance.noticeLimit.Count) {
			noticeSample = NoticeSampleManager.Instance.getNoticeSampleBySid(NoticeManagerment.Instance.noticeLimit[index].sid);
			if(noticeSample!=null && noticeSample.name.Contains(LanguageConfigManager.Instance.getLanguage("AlchemyContent_continueButton"))){
				lianjinSid = noticeSample.sid;
				inter = NoticeManagerment.Instance.noticeLimit[index].integral;
				RankAward rankAward = LucklyActivityAwardConfigManager.Instance.updateAwardDateByIntegral(lianjinSid,inter);
				if(rankAward != null)
					ListKit.AddRange(showPrizes,AllAwardViewManagerment.Instance.exchangeArrayToList(rankAward.prizes));
				NoticeManagerment.Instance.noticeLimit.RemoveAt(index);
				index--;
			}
			index++;
		}
		if(lianjinSid !=0)
		(FPortManager.Instance.getFPort ("LuckyXianshiFPort") as LuckyXianshiFPort).access (lianjinSid, setIntegralRank,4);
	}
	public void setIntegralRank(int _integral,int _rank){
		integral = _integral;
		if(showPrizes.Count > 0){
			UiManager.Instance.openDialogWindow<AllAwardViewWindow>((win)=>{
				win.Initialize(showPrizes,LanguageConfigManager.Instance.getLanguage("notice_xianshi_02",integral.ToString()));
			});
		}
	}
}

