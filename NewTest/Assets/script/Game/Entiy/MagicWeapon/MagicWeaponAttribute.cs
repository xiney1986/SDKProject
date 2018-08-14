using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 秘宝属性类
/// </summary>
public class MagicWeaponAttribute {
        /*field */
    private int hp;//秘宝的血量
    private int attack;//秘宝的攻击
    private int defecse = 0;//防御值
    private int magic = 0;//魔力值
    private int agile = 0;//敏捷值

    public MagicWeaponAttribute(int sid) {//属性模板的sid
        
    }
    public MagicWeaponAttribute(int sid,int lv) {
        MagicWeaponAttributSample mwas = MagicWeaponAttributSampleManager.Instance.getMwAttrSampleBySid(sid);
        for (int i = 0; i < mwas.arrts.Length;i++ ) {
            if (mwas.arrts[i].StartsWith("hp")) hp += mwas.getAttributeByStrengLv(mwas.arrts[i],lv);//血量
            else if (mwas.arrts[i].StartsWith("attack")) attack += mwas.getAttributeByStrengLv(mwas.arrts[i], lv);//攻击
            else if (mwas.arrts[i].StartsWith("defense")) defecse += mwas.getAttributeByStrengLv(mwas.arrts[i], lv);//防御
            else if (mwas.arrts[i].StartsWith("magic")) magic += mwas.getAttributeByStrengLv(mwas.arrts[i], lv);//魔力
            else if (mwas.arrts[i].StartsWith("agile")) agile += mwas.getAttributeByStrengLv(mwas.arrts[i], lv);//敏捷

        }
        
    }
    public MagicWeaponAttribute() {

    }
    public int getMagicWeaponHp() {
        return hp;
    }
    public int getMagicWeaponAttack() {
        return attack;
    }
    public int getMagicWeaponDefecse() {
        return defecse;
    }
    public int getMagicWeaponMagic() {
        return magic;
    }
    public int getMagicWeaponAgile() {
        return agile;
    }
}

    
