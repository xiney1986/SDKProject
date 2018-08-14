using UnityEngine;
using System.Collections;

public class ContentMysticalShop : MonoBase {
	public ContentMysticalGoods contentGoods;
	/**身上有的钻石数量 */
	public UILabel number;
	/**身上有的金币数量 */
	public UILabel jinbNumber;
	/**身上有的紫水晶数量 */
	public UILabel zishuijNumber;
	/**身上有的橙水晶数量 */
	public UILabel cengshuijNumber;
	/**刷新按钮 */
	public ButtonBase buttonRush;
	public UISprite needTokenSp;
	public UILabel flagLabel;
	/**需要的钻石数量 */
	public UILabel needRMBNum;
	public UISprite needRMBSp;
	/**有的刷新令数量数量 */
	public UILabel haveTokenNum;
	private CallBackMsg callback;
	//时间刷新显示窗口
	public ButtonBase timeButton;
    private bool useRMBRush = false;
	public void init(WindowBase win){
		contentGoods.fatherWindow=win;
		buttonRush.fatherWindow=win;
		timeButton.fatherWindow=win;
		updateAllNumInfo();
		MysticalShopFPort fport = FPortManager.Instance.getFPort ("MysticalShopFPort") as MysticalShopFPort;		
		fport.access (getDataSuccess);
	}
	void getDataSuccess ()
	{
        useRMBRush = false;
		callback=doRush;
		updateInfo ();
		reloadShop ();
		MaskWindow.UnlockUI();
	}
	/// <summary>
	/// 刷新所有的需求数量
	/// </summary>
	public void updateAllNumInfo(){
		string[] strr= MysticalShopConfigManager.Instance.getUseData().Split(':');
		if(strr.Length==3&&strr[1]=="00"&&strr[2]=="00")timeButton.textLabel.text="[u]"+LanguageConfigManager.Instance.getLanguage("shop_zheng1")+strr[0]+LanguageConfigManager.Instance.getLanguage("shop_zheng");
		else timeButton.textLabel.text="[u]"+LanguageConfigManager.Instance.getLanguage("shop_zheng1")+MysticalShopConfigManager.Instance.getUseData();
	}

	public void updateInfo ()
	{
		number.text = UserManager.Instance.self.getRMB () + "";
		jinbNumber.text=UserManager.Instance.self.getMoney()+"";
		Prop prozi=StorageManagerment.Instance.getProp(71143);
		if(prozi==null){
			zishuijNumber.text="0";
		}else{
			zishuijNumber.text=prozi.getNum().ToString();
		}
		Prop procheng=StorageManagerment.Instance.getProp(71144);
		if(procheng==null){
			cengshuijNumber.text="0";
		}else{
			cengshuijNumber.text=procheng.getNum().ToString();
		}
		Prop pro=StorageManagerment.Instance.getProp(71131);
		if(pro==null){
            haveTokenNum.text = "[DC505A]0/" + MysticalShopConfigManager.Instance.getNeedTokenNum().ToString();
		}else{
            haveTokenNum.text = "[3A9663]" + pro.getNum().ToString() + "/" + MysticalShopConfigManager.Instance.getNeedTokenNum().ToString();
		}
//		flagLabel.transform.localPosition=haveTokenNum.transform.localPosition+new Vector3(haveTokenNum.width,0f,0f);
//		needRMBSp.transform.localPosition=flagLabel.transform.localPosition+new Vector3(flagLabel.width,0f,0f);
        if (UserManager.Instance.self.getRMB() >= MysticalShopConfigManager.Instance.getNeedRmbNum()) {
            needRMBNum.text = "[3A9663]X" + MysticalShopConfigManager.Instance.getNeedRmbNum().ToString();
        }else
        needRMBNum.text = "[DC505A]X" + MysticalShopConfigManager.Instance.getNeedRmbNum().ToString();
//		needRMBNum.transform.localPosition=needRMBSp.transform.localPosition+new Vector3(needRMBSp.width,0f,0f);
	}
	public void reloadShop ()
	{
		float y = contentGoods.transform.localPosition.y;
		ArrayList list = ShopManagerment.Instance.getAllMysticalGoods ();
		contentGoods.Initialize (list, ContentShopGoods.MYSTICAL_SHOP, updateInfo);
		contentGoods.reLoad (list.Count);
		StartCoroutine(Utils.DelayRunNextFrame(()=>{
			contentGoods.jumpToPos(y);})
		               );	
	}
	/** button点击事件 */
	public void buttonEventBase (GameObject gameObj) {
		if (gameObj.name == "buttonRush") {
			if(!StorageManagerment.Instance.checkProp(71131,MysticalShopConfigManager.Instance.getNeedTokenNum())){
				UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
					win.initWindow (2, LanguageConfigManager.Instance.getLanguage ("s0094"), LanguageConfigManager.Instance.getLanguage ("s0093"), LanguageConfigManager.Instance.getLanguage ("shop_useRMBbuyrush",MysticalShopConfigManager.Instance.getNeedRmbNum().ToString()), callback);
				});
			}else{
				beginRush();
			}
		}else if(gameObj.name=="timeButton"){
			UiManager.Instance.openDialogWindow<TimeShowWindow>();
		}
	}
	public void updateUI(){
		updateInfo ();
		reloadShop ();
	}
	public void doRush(MessageHandle msg){
		if (msg.msgEvent == msg_event.dialogCancel)
			return;
		if(UserManager.Instance.self.getRMB()<MysticalShopConfigManager.Instance.getNeedRmbNum()){
			UiManager.Instance.openDialogWindow<MessageLineWindow> ((win) => {
				win.Initialize (LanguageConfigManager.Instance.getLanguage 
				                ("shop_can_not_rush"));
			});
		}else{
            useRMBRush = true;
			beginRush();
		}
	}
	public void beginRush(){
		MysticalShopRushFPort fport = FPortManager.Instance.getFPort ("MysticalShopRushFPort") as MysticalShopRushFPort;		
		fport.rushGoods (getDataSuccess,useRMBRush);
	}
}