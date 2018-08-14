using UnityEngine;
using System.Collections;

/// <summary>
/// 开启钥匙宝箱窗口
/// </summary>
public class TreasureChestWindow : WindowBase {
	/*filed */
	/**宝箱icon */
	public UITexture icon;
	/**宝箱的名字 */
	public UILabel name;
	/**宝箱的说明 */
	public UILabel dec;
	public UILabel dec2;
	/**左边开启按钮 */
	public ButtonBase rightButton;
	/**右边开启按钮*/
	public ButtonBase leftButton;
	/**选择的宝箱 */
	private Prop selectProp;
	/**需要的钥匙 */
	public GoodsView needP;
	/**需要的钥匙数量 */
	private int needNum;
	/**需要的钥匙SId */
	private int lockSid;
	/**需要的钥匙描述 */
	public UILabel needDec;
	/**拥有宝箱数量 */
	private int openNum;
	/**需要的商品sid 把钥匙当成商品 */
	private int luckGoodsSid;
	/**实际开了多少个宝箱 */
	private int openNumTrue;
	/**奖品预览容器 */
	public TreasureShowContent content;
	/**method */
	/** begin */
	protected override void begin () {
		base.begin ();
		if(isAwakeformHide){
			if(selectProp!=null){
				updateUI();
			}
		}
		MaskWindow.UnlockUI ();
		
	}
	/// <summary>
	/// 断线重连
	/// </summary>
	public override void OnNetResume () {
		base.OnNetResume ();
		if(selectProp!=null)updateUI();
	}
	/**初始化入口 */
	public void init(int sid){
		selectProp=StorageManagerment.Instance.getProp(sid);
		if(selectProp!=null){
			updateUI();
		}
	}
	/// <summary>
	/// 更新页面上UI
	/// </summary>
	public void updateUI(){
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + selectProp.getIconId (), icon);
		name.text=QualityManagerment.getQualityColor( selectProp.getQualityId ()) +selectProp.getName ();;
		string[] desc = selectProp.getDescribe().Split('\n');
		dec.text=desc[0];
		dec2.text = desc[1];
		openNum=selectProp.getNum()<=10?selectProp.getNum():10;

		if(openNum == 0){
			rightButton.disableButton(true);
			leftButton.disableButton(true);
		}else{
			rightButton.disableButton(false);
			leftButton.disableButton(false);
		}
		content.reload(selectProp.getPrizeSampleLcok(),this);
		PrizeSample[] needProp=selectProp.getNeedPropLcok();
		if(needProp!=null&&needProp.Length>=1){
			GoodsSample p=GoodsSampleManager.Instance.getGoodsSampleBySid (needProp[0].pSid);
			Prop prop=PropManagerment.Instance.createProp(p.goodsSID);
			Prop storageProp = StorageManagerment.Instance.getProp(prop.sid);
			if(storageProp != null)
			{
				prop.setNum(storageProp.getNum());
			}
			else
			{
				prop.setNum(0);
			}

			needP.init(prop);
			needP.setCountActive(true);
			lockSid=p.goodsSID;
			needNum=needProp[0].getPrizeNumByInt ();
			luckGoodsSid=needProp[0].pSid;
 			needDec.text=QualityManagerment.getQualityColor( prop.getQualityId ()) +prop.getName ()+"X"+needNum.ToString();
		}
		int num=10;
		if(selectProp.getNum()<num)num=selectProp.getNum();
		rightButton.textLabel.text=LanguageConfigManager.Instance.getLanguage("s03l2",num.ToString());
	}
	/** button点击事件 */
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow ();
		} else if(gameObj.name=="buttonRight"){//左边按键
			int num=getNeedBuyNum();
			if(checkStore()){
				MaskWindow.UnlockUI();
				return;
			}
			if(num>0){
				UiManager.Instance.openDialogWindow<BuyFortreasureChestWindow>((win)=>{
					int Rmb=UserManager.Instance.self.getRMB();
					Goods luckGodds=new Goods(luckGoodsSid);
					int maxNum=Rmb%luckGodds.getCostPrice()<=num?10:Rmb%luckGodds.getCostPrice();
					win.init(luckGodds,num,1,num,1,luckGodds.getCostType(),buy);
				});
			}else{
				OpenGiftBagFport fport = FPortManager.Instance.getFPort ("OpenGiftBagFport") as OpenGiftBagFport;
				openNumTrue=selectProp.getNum()<=10?selectProp.getNum():10;
				fport.access (openNumTrue, selectProp, addAward);
			}
		}else if(gameObj.name=="buttonLeft"){
			if(checkStore()){
				MaskWindow.UnlockUI();
				return;
			}
			Prop luck=StorageManagerment.Instance.getProp(lockSid);//拿钥匙
			if(luck==null){
				UiManager.Instance.openDialogWindow<BuyFortreasureChestWindow>((win)=>{
					int Rmb=UserManager.Instance.self.getRMB();
					Goods luckGodds=new Goods(luckGoodsSid);
					win.init(luckGodds,1,1,1,1,luckGodds.getCostType(),buy);
				});
			}else{
				OpenGiftBagFport fport = FPortManager.Instance.getFPort ("OpenGiftBagFport") as OpenGiftBagFport;
				openNumTrue=1;
				fport.access (1, selectProp, addAward);
			}
		}
	}
	public void buy (MessageHandle msg)
	{
		if (msg.msgEvent == msg_event.dialogCancel)
			return;
		OpenGiftBagFport fport = FPortManager.Instance.getFPort ("OpenGiftBagFport") as OpenGiftBagFport;
		Prop luck=StorageManagerment.Instance.getProp(lockSid);
		int haveNum=0;
		if(luck!=null){
			haveNum=luck.getNum();
		}
		openNumTrue=msg.msgNum+haveNum;
		fport.access (openNumTrue, selectProp, addAward);
	}
	private int getNeedBuyNum(){
		Prop luck=StorageManagerment.Instance.getProp(lockSid);
		int num=0;//一共需要买多少个钥匙
		if(luck==null){
			num=openNum*needNum;
		}else{
			if(luck.getNum()-openNum*needNum>=0)num=0;
			else num=openNum*needNum-luck.getNum();
		}
		return num;
	}
	private void addAward ()
	{
		AwardManagerment.Instance.addFunc (AwardManagerment.AWARDS_BOX, sendInfoBack);
		
	}
	private void sendInfoBack (Award[] award)
	{
		UiManager.Instance.openDialogWindow<AllAwardViewWindow>(win => {
			win.Initialize (award, updateWindow,LanguageConfigManager.Instance.getLanguage ("s0206", selectProp.getName (),openNumTrue.ToString()));
			//检查是否有可开启的英雄之章
			PrizeSample[] prizes = AllAwardViewManagerment.Instance.exchangeAwards(award);
			bool isOpen=HeroRoadManagerment.Instance.isOpenHeroRoad (prizes);
			if (isOpen) {
				TextTipWindow.Show (LanguageConfigManager.Instance.getLanguage ("s0418"));
			}
		});
	}
	/// <summary>
	/// 关闭奖励窗口后回调 
	/// </summary>
	public void updateWindow(){
		if(selectProp.getNum()>0){
			updateUI();
		}else{
			finishWindow();	
		}
	}
	private bool checkStore(){
		if (StorageManagerment.Instance.getAllTemp ().Count > 0)
		{
			UiManager.Instance.openDialogWindow<MessageLineWindow> ((win) => {
				win.Initialize (LanguageConfigManager.Instance.getLanguage ("storeFull_temp_tip"));
			});
			return true;
		}
		return false;
	}
}
