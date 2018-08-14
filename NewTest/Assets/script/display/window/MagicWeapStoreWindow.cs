using UnityEngine;
using System.Collections;

/// <summary>
/// 单独的秘宝仓库
/// </summary>
public class MagicWeapStoreWindow : WindowBase {

	/**field**/
	/** 星魂仓库 */
	public GameObject contentPrefab;
	/** 星魂仓库*/
    public MagicWeaponContent contentMagicWeapon;//秘宝容器
    ArrayList magicList;//秘宝列表
    private Card card;
    private int type;
	/** 激活window */
	protected override void begin () {
		base.begin ();
        if (isAwakeformHide) {
            init(card,type);
        }
		MaskWindow.UnlockUI ();
	}
    public override void OnNetResume() {
        base.OnNetResume();
        card = StorageManagerment.Instance.getRole(card.uid);
        init(card,type);
        MaskWindow.UnlockUI();
    }
    public void init(Card card,int typee) {//初始化秘宝
        this.card = card;
        this.type = typee;
        if (typee == MagicWeaponType.FROM_CARD_BOOK_NOT_M) {
            magicList = StorageManagerment.Instance.getAllMagicWeaponByType(card.getJob());//得到仓库里所有的秘宝
            contentMagicWeapon.cleanAll();//这里直接清理秘宝容器
            contentMagicWeapon.reLoad(magicList, type);
        } else if (typee == MagicWeaponType.FROM_CARD_BOOK_HAVE_M) {
            magicList = StorageManagerment.Instance.getAllMagicWeaponByType(card.getJob(), card.magicWeaponUID);
            contentMagicWeapon.cleanAll();//这里直接清理秘宝容器
            contentMagicWeapon.reLoad(magicList, type);
        }
         
    }
	/** 点击事件 */
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow ();
		}
	}
	public void updateUI()
	{

	}

}
