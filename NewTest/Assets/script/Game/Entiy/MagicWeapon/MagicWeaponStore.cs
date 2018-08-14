using UnityEngine;
using System;
using System.Collections;
 
/**
 * 秘宝仓库
 * @author liwei
 * */
public class MagicWeaponStore:Storage
{
    public MagicWeaponStore()
	{
	
	}

	/** 获得指定品质的秘宝 */
	public ArrayList getAllMagicWeaponByQuality (int qualityId)
	{
        ArrayList magicWeapons = getStorageProp();
		ArrayList temp = new ArrayList ();
        for (int i = 0; i < magicWeapons.Count; i++) {
            Equip c = magicWeapons[i] as Equip;
			if (c.getQualityId () == qualityId) {
				c.index = i;//重置索引
				temp.Add (c);
			}
		}
		return temp;
	}

    /// <summary>
    /// 解析后台数据 初始化秘宝仓库
    /// </summary>
    /// <param name="arr"></param>
    public override void parse(ErlArray arr) {
        ErlArray ea1 = arr.Value[1] as ErlArray;
        if (ea1.Value.Length <= 0) {
            init(StringKit.toInt(arr.Value[0].getValueString()), null);
        } else {
            ArrayList al = new ArrayList();
            MagicWeapon magicWeapon;
            for (int i = 0; i < ea1.Value.Length; i++) {
                magicWeapon = MagicWeaponManagerment.Instance.createMagicWeapon();
                magicWeapon.bytesRead(0, ea1.Value[i] as ErlArray);
                al.Add(magicWeapon);
            }
            init(StringKit.toInt(arr.Value[0].getValueString()), al);
        }
    }
} 

