using UnityEngine;
using System.Collections;

public class NvShenStore : MonoBase {

    public ContentNvShenShopGoods contentGoods;
    public UILabel number;
    public void init(WindowBase win) {
        contentGoods.fatherWindow = win;
        InitShopFPort fport = FPortManager.Instance.getFPort("InitShopFPort") as InitShopFPort;
        fport.access(getDataSuccess);
    }
    public void getDataSuccess() {
        updateInfo();
        reLoadShop();
    }
    public void updateInfo() {
        Prop p = StorageManagerment.Instance.getProp(CommandConfigManager.Instance.getnvshenMoneySid());
        number.text =p==null?"0":p.getNum()+"";//拥有的女神币
    }
    public void reLoadShop() {
        float y = contentGoods.transform.localPosition.y;
        ArrayList list = ShopManagerment.Instance.getAllNvshenGoods();
        contentGoods.Initialize(list, updateInfo);
        contentGoods.reLoad(list.Count);
        StartCoroutine(Utils.DelayRunNextFrame(() => {contentGoods.jumpToPos(y);
        }));
    }
    public void updateUI() {
        reLoadShop();
        updateInfo();
    }
}
