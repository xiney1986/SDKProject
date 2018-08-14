using UnityEngine;
using System.Collections;

/// <summary>
/// 节日礼花条目
/// </summary>
public class FestivalFireworksItemUI : MonoBase {
	/**  制造按钮 */
	public ButtonBase makeButton;
	/**  点燃按钮 */
	public ButtonBase fireButton;
	/** 烟花道具 */
	public GoodsView fireworks;
	/** 烟花名称 */
	public UILabel fireworksLabel;
	/** 条目sid(与兑换的sid相同) */
	private int itemSid;
	/** 烟花sid */
	private int fireworksSid;

	public GameObject[] effect;

	private WindowBase win;
	private int useCount;

	Prop propPrize;

	private FestivalFireworksSample fireworksSample;

	/// <summary>
	/// 初始化UI
	/// </summary>
	public void initItemUI(FestivalFireworksSample fireworksSample,WindowBase win)
	{
		this.win = win;
		this.fireworksSample = fireworksSample;
		this.itemSid = fireworksSample.noitceItemSid;
		this.fireworksSid = fireworksSample.prizes.pSid;
		this.fireworksLabel.text = QualityManagerment.getQualityColor(fireworksSample.prizes.getQuality ()) + fireworksSample.prizes.getPrizeName ();
		fireworks.init(fireworksSample.prizes);
		fireworks.rightBottomText.text = "x"+getPropsNum(fireworksSid);
		fireworks.fatherWindow = win;
		initButton();
	}

	/// <summary>
	/// 初始化按钮信息
	/// </summary>
	public void initButton()
	{
		this.makeButton.fatherWindow = win;
		this.fireButton.fatherWindow = win;
		this.makeButton.onClickEvent = buttonEventMake;
		this.fireButton.onClickEvent = buttonEventFire;
		//没有道具时不能点燃
		if(getPropsNum(fireworksSid)==0)
			fireButton.disableButton(true);

	}

	/// <summary>
	/// 点击事件
	/// </summary>
	private void buttonEventMake(GameObject obj)
	{
		UiManager.Instance.openDialogWindow<FireworksMakeWindow>((win)=>{
			win.init(fireworksSample,updateUI);
		});
	}
	/// <summary>
	/// 点击事件
	/// </summary>
	private void buttonEventFire(GameObject obj)
	{
		UiManager.Instance.openDialogWindow<FireworksFireWindow>((win)=>{
			win.init(fireworksSample,useProp);
		});
	}

	private void  useProp(int count,Prop prop)
	{
		useCount = count;
		propPrize = prop;
		if (!StorageManagerment.Instance.isPropStorageFull (prop)) {
			OpenGiftBagFport fport = FPortManager.Instance.getFPort ("OpenGiftBagFport") as OpenGiftBagFport;
			fport.access (count, prop, addAward);
		} else {
			UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, "user level limit", null);
			});
		}
	}
	
	private void sendInfoBack (Award[] award)
	{
		
		StartCoroutine(Utils.DelayRun(()=>{
			UiManager.Instance.openDialogWindow<AllAwardViewWindow>(win => {
				win.Initialize (award, LanguageConfigManager.Instance.getLanguage ("noticeActivityFF_13",fireworksSample.exchangeSample.describe));
				openHeroRoad(award);
				clearEffet();
			});
		},4f));
		playAnim();
	}
	
	private void addAward ()
	{
		AwardManagerment.Instance.addFunc (AwardManagerment.AWARDS_BOX, sendInfoBack);
		
	}
	private void openHeroRoad (Award[] award){
		PrizeSample[] prizes = AllAwardViewManagerment.Instance.exchangeAwards(award);
		bool isOpen=HeroRoadManagerment.Instance.isOpenHeroRoad (prizes);
		if (isOpen) {
			TextTipWindow.Show (LanguageConfigManager.Instance.getLanguage ("s0418"));
		}
	}
	/// <summary>
	/// 清理特效
	/// </summary>
	public void clearEffet()
	{
		if(effect==null)return;
		for(int i=0;i<effect.Length;i++)
		{
			effect[i].gameObject.SetActive(false);
		}
		MaskWindow.UnlockUI();
	}

	/// <summary>
	/// 播放特效
	/// </summary>
	public void playEffet()
	{
		if(effect==null)return;
		if(effect.Length == 3)
		{
			MaskWindow.LockUI();
			effect[0].gameObject.SetActive(true);
			StartCoroutine(Utils.DelayRun(()=>{
				effect[1].gameObject.SetActive(true);
			},1f));
			StartCoroutine(Utils.DelayRun(()=>{
				effect[2].gameObject.SetActive(true);
			},2f));
		}
	}
	/// <summary>
	/// 制造后更新界面
	/// </summary>
	public void updateUI(int sid,int num)
	{
		fireworks.rightBottomText.text = "x"+getPropsNum(fireworksSid);
		if(getPropsNum(fireworksSid)>0)
			fireButton.disableButton(false);
		else 
			fireButton.disableButton(true);
	}
	/// <summary>
	/// 点燃后播放动画
	/// </summary>
	public void playAnim()
	{
		playEffet();
		fireworks.rightBottomText.text = "x"+getPropsNum(fireworksSid);
		if(getPropsNum(fireworksSid)>0)
			fireButton.disableButton(false);
		else 
			fireButton.disableButton(true);
	}

	public void OnDisable ()
	{

	}
	/// <summary>
	/// 得到拥有道具的数量
	/// </summary>
	/// <returns>The properties number.</returns>
	public int getPropsNum(int sid)
	{
		Prop s;
		int num = 0;
		ArrayList list = StorageManagerment.Instance.getPropsBySid(sid);
		for(int j=0;j<list.Count;j++)
		{
			s = list[j] as Prop;
			num = s.getNum();
		}
		return num;
	}
}
