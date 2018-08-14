using UnityEngine;
using System.Collections;

public class EquipScrapView : MonoBehaviour
{

	[HideInInspector]
	public StoreWindow
		fawin;
	public GameObject goodsViewPrefab;
	public UILabel numLabel;
	public UILabel nameLabel;
	public ButtonExchangeReceive buttonExchange;
	private GameObject item;
	GoodsView button;
    private MagicWeapon magicW;//这个是展示的秘宝
    private ExchangeSample magicExSamle;//神器兑换模板
	
	public void init (Exchange ex)
	{
		ExchangeSample sample = ex.getExchangeSample ();
        this.magicExSamle = sample;
		//按钮显示判断
		int count = ExchangeManagerment.Instance.getCanExchangeNum (ex);
		if (count <= 0) {
			buttonExchange.disableButton (true);
		} else {
			buttonExchange.disableButton (false);
		}
		//数量9240c
		numLabel.text = "[92400c]" + LanguageConfigManager.Instance.getLanguage("store03")+"[-]" + EquipScrapManagerment.Instance.getNumString (sample);	
		//存信息到兑换按钮
		buttonExchange.fatherWindow = fawin;
		buttonExchange.updateButton (ex);
		//显示装备
        if (sample.exType == 8) {
            MagicWeapon nw = MagicWeaponManagerment.Instance.createMagicWeapon(sample.exchangeSid);
            magicW = nw;
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
        } else {
            Equip equip = EquipManagerment.Instance.createEquip(sample.exchangeSid);
            //名称
            nameLabel.text = QualityManagerment.getQualityColor(equip.getQualityId()) + equip.getName(); ;
            if (button == null) {
                item = NGUITools.AddChild(gameObject, goodsViewPrefab);
                item.name = "goodsView";
                button = item.GetComponent<GoodsView>();
            }
            if (button != null && equip != null) {
                button.init(equip);
                button.fatherWindow = fawin;
                button.rightBottomText.text = "";
            }
        }
		
	}
    /// <summary>
    /// 点击本身的回调（秘宝）冲仓库过去的
    /// </summary>
    void showMagicWeapon() {
        if (magicW != null)
            UiManager.Instance.openWindow<MagicWeaponStrengWindow>((win) => {
                win.init(magicW, MagicWeaponType.FORM_OTHER);
            });
        else MaskWindow.UnlockUI();
    }
}
