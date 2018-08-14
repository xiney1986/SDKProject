using UnityEngine;
using System.Collections;

//秘宝容器
public class MagicWeaponContent : dynamicContent {
    //秘宝列表
    //秘宝显示条预制体
    public GameObject magicWeaponPerfab;
    private ArrayList magicWeaponList;
    private MagicWeapon[] magicWeapons;
    private int typee;//进入容器的类型
    /** 排序对象 */
    MagicWeaponComp comp = new MagicWeaponComp();
    public void reLoad(ArrayList magics,int type) {
        magicWeaponList = magics;
        typee = type;
        magicWeapons=new MagicWeapon[magicWeaponList.Count];
        for(int i=0;i<magicWeaponList.Count;i++){
            magicWeapons[i]=magicWeaponList[i] as MagicWeapon;
        }
        //这里对秘宝进行排序
        if (type == MagicWeaponType.STRENG) {
            base.reLoad(magicWeaponList.Count);
            return;
        }
        SetKit.sort (magicWeapons, comp);
        base.reLoad(magicWeaponList.Count);
    }

    public override void updateItem(GameObject item, int index) {
        //base.updateItem (item, index);
        MagicWeaponItem button = item.GetComponent<MagicWeaponItem>();
        button.UpdateMagicWeapon(magicWeapons[index] as MagicWeapon, typee);
        button.fatherWindow = fatherWindow;
        button.iconButton.fatherWindow = fatherWindow;
        button.putonButton.fatherWindow = fatherWindow;
    }

    public override void initButton(int i) {
        if (nodeList[i] == null) {
            nodeList[i] = NGUITools.AddChild(gameObject, magicWeaponPerfab);
        }

        nodeList[i].name = StringKit.intToFixString(i + 1);
        MagicWeaponItem button = nodeList[i].GetComponent<MagicWeaponItem>();
        button.UpdateMagicWeapon(magicWeapons[i] as MagicWeapon, typee);

        //装备仓库中只能强化
       // button.UpdateEquip(magicWeapons[index] as MagicWeapon, 1);
        // button.intensifyButton.fatherWindow = fatherWindow;
    }
    /** 秘宝排序 */
    class MagicWeaponComp : Comparator {//1代表要交换

        public int compare(object o1, object o2) {
            //排序 先显示激活的（骑乘，非骑乘（品质 品质一样用战斗力））
            if (o1 == null)
                return 1;
            if (o2 == null)
                return -1;
            if (!(o1 is MagicWeapon) || !(o2 is MagicWeapon))
                return 0;
            MagicWeapon obj1 = (MagicWeapon)o1;
            MagicWeapon obj2 = (MagicWeapon)o2;
            if (obj1.state == 1 && obj2.state != 1) return -1;
            if (obj1.state != 1 && obj2.state == 1) return 1;
            if (obj1.getMagicWeaponQuality() < obj2.getMagicWeaponQuality()) return 1;
            if (obj1.getMagicWeaponQuality() > obj2.getMagicWeaponQuality()) return -1;
            return 0;
        }
    }

}
