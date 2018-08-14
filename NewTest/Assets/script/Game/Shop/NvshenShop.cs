﻿using UnityEngine;
using System.Collections;
/*
 * 女神商店
 * */
public class NvshenShop : MemorySpace 
{
    ArrayList showList;
    public NvshenShop() 
    { 
    }
    public ArrayList getShowList() {
        siftGoods();
        return showList;
    }
    //初始化所有
    public void initAllGoods(int[] ids) {
        if (ids == null || ids.Length < 1)
            return;
        for (int i = 0; i < ids.Length; i++) {
            Goods goods = new Goods(ids[i]);

            if (goods.isShow()) {
                space.Add(goods);
            }
        }
    }
    //过滤掉过期的和达到购买上限的商品
    void siftGoods() {
        showList = new ArrayList();
        for (int j = 0; j < space.Count; j++) {
            Goods goods = space[j] as Goods;
            if (goods.getGoodsMaxBuyCount() > 0 && goods.getNowBuyNum() >= goods.getGoodsMaxBuyCount())
                continue;
            //if (!FuBenManagerment.Instance.isCanShow(goods.sid)) continue;
            showList.Add(space[j]);
        }
        showList = SortManagerment.Instance.shopSort(showList, SortConditionManagerment.Instance.getConditionsByKey(SiftWindowType.SIFT_SHOP_WINDOW));
    }
    //移除商品
    public void removeGoods(Goods goods) {
        space.Remove(goods);
    }

    public void updateGoods(ErlArray arr) {
        ErlArray arr1;
        for (int i = 0; i < arr.Value.Length; i++) {
            arr1 = arr.Value[i] as ErlArray;
            Goods goods;
            for (int j = 0; j < space.Count; j++) {
                goods = space[j] as Goods;
                if (StringKit.toInt((arr1.Value[0] as ErlType).getValueString()) == goods.sid) {
                    goods.nowBuyNum = StringKit.toInt((arr1.Value[1] as ErlType).getValueString());
                    break;
                } else {

                }
            }
        }
    }
}
