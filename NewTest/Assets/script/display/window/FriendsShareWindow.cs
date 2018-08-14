using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FriendsShareWindow : WindowBase {

	public GameObject shareButtonPrefab;
	public TapContentBase tapBase;//分页按钮
	public ContentFriendsShare contentShare;//分享容器
	public ContentFriendsShare contentPraise;//点赞容器
	public ButtonBase onekeyPraiseButton;
	public ButtonBase onekeyShareButton;
	public ButtonBase buttonMomo;   

	private int tapType = 0;//属于哪个标签。0我的分享，1朋友分享
	private bool isChange = false;//是否需要刷新
	private FriendsShare shares;

	protected override void begin ()
	{
		base.begin ();
		if(FriendsShareManagerment.Instance.getFriendsShare() == null)
			initShareInfo();
		MaskWindow.UnlockUI();
	}

	/// <summary>
	/// 初始化窗口
	/// </summary>
	/// <param name="_tap">属于哪个标签。0我的分享，1朋友分享</param>
	/// <param name="_isChange">是否需要刷新<c>true</c>需要</param>
	public void initWin(bool _isChange,int _tap)
	{
		shares = FriendsShareManagerment.Instance.getFriendsShare();
		tapType = _tap;
		isChange = _isChange;

		if(isChange)
			tapInitialize(tapType);
		else
		{
			if (isAwakeformHide == false) {
				tapInitialize(tapType);
			}
		}
	}

	public void initFriendsInfo()
	{
		shares = FriendsShareManagerment.Instance.getFriendsShare();
		if (isAwakeformHide == false) {
			tapInitialize(0);
		}
	}

	private void initShareInfo()
	{
		FriendsShareFPort fPort = FPortManager.Instance.getFPort ("FriendsShareFPort") as FriendsShareFPort;
		fPort.initShareInfo(initFriendsInfo);
	}

	//刷新面板后清除容器，有奖励则显示奖励
	private void shareBack(int sid,int num)
	{
		MaskWindow.UnlockUI();
		contentShare.reLoadUI(0);
		changeShareButton(false);
		if(num <=0)
			return;
		Prop prop = PropManagerment.Instance.createProp(sid);

		PrizeSample[] awards = new PrizeSample[1]{new PrizeSample(3,prop.sid,num)};
		UiManager.Instance.openDialogWindow<AllAwardViewWindow>((win)=>{
			win.Initialize(awards,LanguageConfigManager.Instance.getLanguage("s0177",prop.getName(),num.ToString()));
		});
		FriendsShareManagerment.Instance.setShareInfoNull();
	}    
	//一键点赞
	private void onekeyPraise()
	{

            if (FriendsShareManagerment.Instance.getPraiseInfo() == null)
            {
                UiManager.Instance.createMessageWindowByOneButton(LanguageConfigManager.Instance.getLanguage("PraiseNull"), null);
                return;
            }
            if (FriendsShareManagerment.Instance.getPraiseNumIsFull())
            {
                UiManager.Instance.createMessageWindowByOneButton(LanguageConfigManager.Instance.getLanguage("PraiseNoNum"), null);
                return;
            }
            FriendsShareFPort fPort = FPortManager.Instance.getFPort("FriendsShareFPort") as FriendsShareFPort;
            fPort.onekeyPraise(praiseBack);

	}

	//刷新面板后清除容器，有奖励则显示奖励
	private void praiseBack(int sid,int num,int useNum,int money)
	{
		MaskWindow.UnlockUI();
		onekeyPraiseButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("PraiseButton") + "(" + FriendsShareManagerment.Instance.getPraiseNum() + ")";
		initWin(true,1);
		contentPraise.reLoadUI(1);
		changePraiseButton(false);
		FriendsShareManagerment.Instance.setPraise(useNum);

		List<PrizeSample> awards = new List<PrizeSample>();
		string str = "";
		if(num >0) {
			awards.Add (new PrizeSample(3,sid,num));
			str = LanguageConfigManager.Instance.getLanguage("ShareMsg1",useNum.ToString(),money.ToString(),num.ToString());
		}
		else
		{
			int yu = FriendsShareManagerment.Instance.getPraiseNum() % 10;
			str = LanguageConfigManager.Instance.getLanguage("ShareMsg2",useNum.ToString(),money.ToString(),yu.ToString());
		}
		if(money >0) {
			awards.Add (new PrizeSample(1,0,money));
		}
		if(awards != null && awards.Count > 0) {
			UiManager.Instance.openDialogWindow<AllAwardViewWindow>((win)=>{
				win.Initialize(awards,str);
			});
		}
	}


	//标签切换方法
	public void tapInitialize(int index)
	{
		tapBase.changeTapPage(tapBase.tapButtonList[index]);
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		switch(gameObj.name)
		{
		case "close":
			finishWindow();
			break;

		case "buttonPraise":
			onekeyPraise();
			break;

		case "buttonMomo":
			UiManager.Instance.openDialogWindow<MomoShareWindow>();
			break;
		}
	}

	//切换页面
	public override void tapButtonEventBase (GameObject gameObj, bool enable)
	{
		base.tapButtonEventBase (gameObj, enable);
		
		if (gameObj.name == "tapMyShare" && enable == true) 
		{
			tapType = 0;
			if(FriendsShareManagerment.Instance.getShareInfo() != null) {
				contentShare.gameObject.SetActive(true);
				contentShare.reLoadUI(tapType);
			}
			else
				contentShare.gameObject.SetActive(false);
		}
		else if (gameObj.name == "tapFriendsShare" && enable == true) 
		{
			tapType = 1;
			if(FriendsShareManagerment.Instance.getPraiseNumIsFull())
			{
				changePraiseButton(false);
			}
			onekeyPraiseButton.textLabel.text = LanguageConfigManager.Instance.getLanguage("PraiseButton") + "(" + FriendsShareManagerment.Instance.getPraiseNum() + ")";
			if(FriendsShareManagerment.Instance.getPraiseInfo() != null) {
				contentPraise.gameObject.SetActive(true);
				contentPraise.reLoadUI(tapType);
			}
			else
				contentPraise.gameObject.SetActive(false);
		}

		MaskWindow.UnlockUI();

	}

	public void changeShareButton(bool bo)
	{
		if(!bo) {
			onekeyShareButton.disableButton(true);
			//onekeyShareButton.GetComponent<Collider>().enabled = false;
		}
		else
		{
			onekeyShareButton.spriteBg.spriteName = "button_big2";
			onekeyShareButton.GetComponent<Collider>().enabled = true;
		}
	}

	public void changePraiseButton(bool bo)
	{
		if(!bo) {
			onekeyPraiseButton.disableButton(true);
//			onekeyPraiseButton.GetComponent<Collider>().enabled = false;
		}
		else
		{
			onekeyPraiseButton.spriteBg.spriteName = "button_big2";
			onekeyPraiseButton.GetComponent<Collider>().enabled = true;
		}
	}

}
