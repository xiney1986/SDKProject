using UnityEngine;
using System.Collections;

public class MagicWeaponScrapView : MonoBehaviour
{

	[HideInInspector]
	public MagicWeaponStoreWindow fawin;
	public GameObject goodsViewPrefab;
	public UILabel numLabel;
	public UILabel nameLabel;
	public ButtonExchangeReceive buttonExchange;
    public ButtonBase buttonDecompostion;//分解按键
	private GameObject item;
    public UISprite typeIcon;//神器类型图标
    public UISprite typeQuality;//神器类型背景
	GoodsView button;
    private MagicWeapon magicW;//这个是展示的秘宝
    private ExchangeSample magicExSamle;//神器兑换模板
    private Prop thisProp=null;//这个物品是什么？
    private int havePropNum=0;//仓库里有多少个这样的物品
    private PrizeSample ps;//组成奖励
    private Exchange exchange;
	
	public void init (Exchange ex)
	{
        exchange = ex;
		ExchangeSample sample = ex.getExchangeSample ();
        this.magicExSamle = sample;
        updateUI();
		
	}
    void updateUI() {
		fawin.updateCanGetMagicWeaponCount();
        numLabel.text = "[6A4D36]" + LanguageConfigManager.Instance.getLanguage("store03")+"[-]" + MagicWeaponScrapManagerment.Instance.getNumString(magicExSamle);
        thisProp = MagicWeaponScrapManagerment.Instance.getNeedProp(magicExSamle);
        havePropNum = StorageManagerment.Instance.getProp(thisProp.sid) == null ? 0 : StorageManagerment.Instance.getProp(thisProp.sid).getNum();
        if (havePropNum == 0) buttonDecompostion.disableButton(true);
        //按钮显示判断
        int count = ExchangeManagerment.Instance.getCanExchangeNum(exchange);
        if (count <= 0) {
            buttonExchange.disableButton(true);
        } else {
            buttonExchange.disableButton(false);
        }
        buttonExchange.fatherWindow = fawin;
        buttonExchange.updateButton(exchange);
        //初始化分解按键
        buttonDecompostion.fatherWindow = fawin;
        buttonDecompostion.onClickEvent = beginDecomposion;
        //显示装备
        if (magicExSamle.exType == 8) {
            MagicWeapon nw = MagicWeaponManagerment.Instance.createMagicWeapon(magicExSamle.exchangeSid);
            magicW = nw;
            //更新神器职业背景颜色，用以区分品质
            typeQuality.spriteName = QualityManagerment.qualityIconBgToBackGround(magicW.getMagicWeaponQuality());
            //显示职业
            typeIcon.gameObject.SetActive(true);
            typeQuality.gameObject.SetActive(true);
            if (magicW.getMgType() == JobType.POWER) {//力
                typeIcon.spriteName = "roleType_2";
            } else if (magicW.getMgType() == JobType.MAGIC) {//魔
                typeIcon.spriteName = "roleType_5";
            } else if (magicW.getMgType() == JobType.AGILE) {//敏
                typeIcon.spriteName = "roleType_3";
            } else if (magicW.getMgType() == JobType.POISON) {//毒
                typeIcon.spriteName = "roleType_4";
            } else if (magicW.getMgType() == JobType.COUNTER_ATTACK) {//反 
                typeIcon.spriteName = "roleType_1";
            } else if (magicW.getMgType() == JobType.ASSIST) {//辅
                typeIcon.spriteName = "roleType_6";
            } else {
                typeIcon.gameObject.SetActive(false);
                typeQuality.gameObject.SetActive(false);
            }
            nameLabel.text = QualityManagerment.getQualityColor(nw.getMagicWeaponQuality()) + nw.getName();
            if (button == null) {
                item = NGUITools.AddChild(gameObject, goodsViewPrefab);
                item.name = "goodsView";
                button = item.GetComponent<GoodsView>();
                button.onClickCallback = showMagicWeapon;
            }
            if (button != null && nw != null) {
                button.init(nw);
                button.fatherWindow = fawin;
                button.rightBottomText.text = "";
            }
        }
    }
    /// <summary>
    /// 点击分解按钮干的事情
    /// </summary>
    /// <param name="obj"></param>
    void beginDecomposion(GameObject obj) {
        //能进这里 就证明有物品可以分解
        Prop needProp = MagicWeaponScrapManagerment.Instance.getNeedProp(magicExSamle);//这里拿到了需要的物品也就是神器碎片
        //打开特别的分解窗口
        UiManager.Instance.openDialogWindow<MagicWeapdecompositionWindow>((win) => {
            win.init(needProp, havePropNum, 1, sureDeccompostion);
            win.dialogCloseUnlockUI = false;
        });
       

    }
    /// <summary>
    /// 点击本身的回调（秘宝）冲仓库过去的
    /// </summary>
    void showMagicWeapon() {
        if (magicW != null)
            UiManager.Instance.openWindow<MagicWeaponStrengWindow>((win) => {
                win.init(magicW,magicExSamle, MagicWeaponType.FORM_OTHER);
            });
        else MaskWindow.UnlockUI();
    }
    public void sureDeccompostion(MessageHandle msg) {
        if (msg.msgEvent == msg_event.dialogOK) {
            int num = msg.msgNum;
            //PropSampleManager.Instance.createSample((msg.msgInfo as Prop).sid).prizes[0];
            ps = PropSampleManager.Instance.getPropSampleBySid((msg.msgInfo as Prop).sid).prizes[0];
            ps.num = num + "";
            //组建分解字符串
            string str = "card|equipment|artifact|goods," + (msg.msgInfo as Prop).sid.ToString() + "," + num.ToString();
            ResolveGoodsFPort port = FPortManager.Instance.getFPort("ResolveGoodsFPort") as ResolveGoodsFPort;
            fawin.ps = ps;
            port.resolveGoods(str, null, fawin.resolveBack);
        } else {
            
        }

    }
}
