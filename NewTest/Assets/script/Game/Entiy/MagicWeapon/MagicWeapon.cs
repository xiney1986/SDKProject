using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 秘宝实体类
/// </summary>
public class MagicWeapon : StorageProp {
    /**秘宝的强化等级 */
    public int strengLv=0;
    /**秘宝的阶位等级 */
    private int phaseLv = 0;
    private string name="";
    private int iconID = 0;
    private bool isNew = false;
    /**秘宝的使用状态 */
    public int state = 0;//不在使用中
    public MagicWeaponAttribute magicWeaponAttrbutes;//秘宝的属性
    public int[] skillSids;//附带技能这里只保存技能的Sid
    public List<int> activationSkill;//秘宝激活的技能
    public int addSuccess = 0;//额外成功率

    public MagicWeapon() {
        isU = true;//唯一物品 写死
    }
    //带参数构造方法
    /// <summary>
    /// 
    /// </summary>
    /// <param name="uid">唯一id</param>
    /// <param name="sid">模板id</param>
    /// <param name="strengLv">强化等级</param>
    /// <param name="phaseLv">阶位等级</param>
    /// <param name="state">状态（是不是在使用）</param>
    /// 技能 基础属性等都不从后台拿 直接根据段位和强化等级拿模板的数据
    public MagicWeapon(string uid, int sid, int strengLv, int phaseLv,int state) {
        this.uid = uid;
        this.sid = sid;
        this.strengLv = strengLv;
        this.isU = true;//写死的玩意
        this.phaseLv = phaseLv;
        this.state = state;
        this.isNew = false;
        updateAttruibte();
        updateSkill();
        updateCommon();
    }
    /// <summary>
    /// 后台序列化过来 对创建的秘宝附件属性
    /// </summary>
    /// <param name="j"></param>
    /// <param name="ea"></param>
    public override void bytesRead(int j, ErlArray ea) {
        this.uid = ea.Value[j++].getValueString();
        this.sid = StringKit.toInt(ea.Value[j++].getValueString());
        this.state = StringKit.toInt(ea.Value[j++].getValueString());
        this.strengLv = StringKit.toInt(ea.Value[j++].getValueString());
        this.phaseLv = StringKit.toInt(ea.Value[j++].getValueString());
        this.addSuccess = StringKit.toInt(ea.Value[j++].getValueString());
        //this.isNew = StringKit.toInt(ea.Value[j++].getValueString())==1?true:false;
        updateAttruibte();
        updateSkill();
        updateCommon();
    }
    private void updateCommon() {
        MagicWeaponSample ms = MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(sid);
        this.name = ms.name;
        this.iconID = ms.iconId;
    }
    //检测装备状态 处于对应状态返回true
    public bool checkState(int _state) {
        if (state == _state)
            return true;
        else
            return (state & _state) > 0;
    }
	
    /// <summary>
    /// 更新秘宝的技能 根据进阶等级
    /// </summary>
    public void updateSkill() {
        MagicWeaponSample ms = MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(sid);
        skillSids = ms.skillSids;
        if (phaseLv == 0) activationSkill = null;
        else {
            activationSkill = new List<int>();
            for (int i = 0; i < phaseLv; i++) {
                activationSkill.Add(skillSids[i]);
            }
        }
    }
    /// <summary>
    /// 得到秘宝的成功率
    /// </summary>
    public int getSuccess() {//基础成功率+额外的成功率
        int baseNum= MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(sid).baseSuccess[phaseLv>=getMaxPhaseLv()?getMaxPhaseLv()-1:phaseLv];
        return baseNum + (100 - baseNum) * addSuccess / 100;
    }
    public int getLuckNumber() {
        return addSuccess;
    }
    /// <summary>
    /// 更新这个石头的具体属性 目前是根据属性SID来的
    /// </summary>
    private void updateAttruibte() {
        MagicWeaponSample ms = MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(sid);
        magicWeaponAttrbutes = new MagicWeaponAttribute(ms.attributSid, strengLv);//初始化模板属性

    }
    /// <summary>
    /// 得到秘宝的品质
    /// </summary>
    /// <returns></returns>
    public int getMagicWeaponQuality() {
        return MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(sid).qualityId;
    }
    /// <summary>
    /// 得到神器的返回奖励
    /// </summary>
    /// <returns></returns>
    public PrizeSample[] getReSourcePrizes() {
        List<PrizeSample> temp = new List<PrizeSample>();
        int quaIndex = this.getLvType();
        if (phaseLv >= 1) {
            string[][] props = CommandConfigManager.Instance.getMagicWeaponPhaseProp();//需要的锻造物品
            string[] selectPropList = props[quaIndex - 2];
            for (int i = 0; i < phaseLv;i++) {
                string Propsid =selectPropList[i];
                int num = 0;
                if (i == 0||i==1) {
                    num=StringKit.toInt(Propsid.Split(',')[1]);
                }else if(i==2){
                     num=(int)(0.9*StringKit.toInt(Propsid.Split(',')[1]));
                }
                else if(i==3){
                     num=(int)(0.85*StringKit.toInt(Propsid.Split(',')[1]));
                }
                else if(i==4){
                    num = (int)(0.8 * StringKit.toInt(Propsid.Split(',')[1]));
                }
				else 
				{
					num = (int)(0.8 * StringKit.toInt(Propsid.Split(',')[1]));
				}
                temp.Add(new PrizeSample(3,StringKit.toInt(Propsid.Split(',')[0]),num));
            }
        }
        if(strengLv>=1){
            string[][] strProps = CommandConfigManager.Instance.getMagicWeaponStrengProp();//需要的强化物品
            string[] strSelectPropList = strProps[quaIndex - 2];
            List<string> tempSid = new List<string>();//保存遍历过得奖品SID
            //for (int j = 0; j < strengLv;j++) {
            //    string strSid = strSelectPropList[j];
            //    string[] sp = strSid.Split(',');
            //    temp.Add(new PrizeSample(3,StringKit.toInt(sp[0]),StringKit.toInt(sp[1])));
            //}
            for (int m = 0; m < strengLv;m++ ) {
                string[] strSid = strSelectPropList[m].Split(',');
                if (tempSid.Contains(strSid[0])) continue;
                tempSid.Add(strSid[0]);
                int numm = 0;
                for (int n = 0; n < strengLv;n++ ) {
                    string[] tempps = strSelectPropList[n].Split(',');
                    if(strSid[0]==tempps[0]){
                        numm += StringKit.toInt(tempps[1]);
                    }
                }
                temp.Add(new PrizeSample(3,StringKit.toInt(strSid[0]),(int)(numm*0.8)));
            }
        }
        PrizeSample[] src=MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(sid).resolveResults;
        for (int m = 0; m < src.Length;m++ ) {
            temp.Add(src[m]);
        }
        PrizeSample[] pss = new PrizeSample[temp.Count];
        for (int n = 0; n < temp.Count;n++ ) {
            pss[n] = temp[n];
        }
        return pss;
    }
    /// <summary>
    /// 得到秘宝的强化等级
    /// </summary>
    /// <returns></returns>
    public int getPhaseLv() {
        return phaseLv;
    }
    //获得装备属性
    public AttrChange[] getAttrChanges() {
        List<AttrChange> list = new List<AttrChange>();
        int hp = magicWeaponAttrbutes.getMagicWeaponHp();
        if (hp != 0) {
            AttrChange attrHp = new AttrChange(AttrChangeType.HP, hp);
            list.Add(attrHp);
        }

        int attack = magicWeaponAttrbutes.getMagicWeaponAttack();
        if (attack != 0) {
            AttrChange attrAtt = new AttrChange(AttrChangeType.ATTACK, attack);
            list.Add(attrAtt);
        }

        int agi = magicWeaponAttrbutes.getMagicWeaponAgile();
        if (agi != 0) {
            AttrChange attrAgi = new AttrChange(AttrChangeType.AGILE, agi);
            list.Add(attrAgi);
        }

        int mag = magicWeaponAttrbutes.getMagicWeaponMagic();
        if (mag != 0) {
            AttrChange attrMag = new AttrChange(AttrChangeType.MAGIC, mag);
            list.Add(attrMag);
        }

        int def = magicWeaponAttrbutes.getMagicWeaponDefecse();
        if (def != 0) {
            AttrChange attrDef = new AttrChange(AttrChangeType.DEFENSE, def);
            list.Add(attrDef);
        }
        return list.ToArray();
    }
    //获得下一级装备属性
    public AttrChange[] getNextAttrChanges() {
        List<AttrChange> list = new List<AttrChange>();
        MagicWeaponSample ms = MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(sid);
        MagicWeaponAttribute nextAttrbutes = new MagicWeaponAttribute(ms.attributSid, strengLv+1);//初始化模板属性
        int hp = nextAttrbutes.getMagicWeaponHp();
        if (hp != 0) {
            AttrChange attrHp = new AttrChange(AttrChangeType.HP, hp-magicWeaponAttrbutes.getMagicWeaponHp());
            list.Add(attrHp);
        }

        int attack = nextAttrbutes.getMagicWeaponAttack();
        if (attack != 0) {
            AttrChange attrAtt = new AttrChange(AttrChangeType.ATTACK, attack - magicWeaponAttrbutes.getMagicWeaponAttack());
            list.Add(attrAtt);
        }

        int agi = nextAttrbutes.getMagicWeaponAgile();
        if (agi != 0) {
            AttrChange attrAgi = new AttrChange(AttrChangeType.AGILE, agi - magicWeaponAttrbutes.getMagicWeaponAgile());
            list.Add(attrAgi);
        }

        int mag = nextAttrbutes.getMagicWeaponMagic();
        if (mag != 0) {
            AttrChange attrMag = new AttrChange(AttrChangeType.MAGIC, mag - magicWeaponAttrbutes.getMagicWeaponMagic());
            list.Add(attrMag);
        }

        int def = nextAttrbutes.getMagicWeaponDefecse();
        if (def != 0) {
            AttrChange attrDef = new AttrChange(AttrChangeType.DEFENSE, def - magicWeaponAttrbutes.getMagicWeaponDefecse());
            list.Add(attrDef);
        }
        return list.ToArray();
    }
    /// <summary>
    /// 得到秘宝的名字
    /// </summary>
    /// <returns></returns>
    public string getName() {
        return name;
    }
    public int getIconId() {
        return iconID;
    }
    public int getStrengLv() {
        return strengLv;
    }
    public bool getIsNew() {
        return isNew;
    }
    public int getMaxStrengLv() {
        return MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(sid).maxLevel;
    }
    public int getMaxPhaseLv() {
        return MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(sid).maxphaseLv;
    }
    public int getMgType() {
        return MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(sid).MagicWeaponType;
    }
    public int getNeedStrengLv(int lv) {
        return MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(sid).needStrengLv[lv];
    }
    /// <summary>
    /// 得到神器的战力
    /// </summary>
    /// <returns></returns>
    public int getPowerCombat() {
        return 0;
    }
    public int getLvType() {
        return MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(sid).lvType;
    }


    


}
