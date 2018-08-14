using System.Collections.Generic;
using UnityEngine;
using System.Collections;
/**
 * 神之位格管理器
 * */
public class ShenGeManager {

    /* 槽位的四个状态 */
    public const int HAVECANEQUIP = 1;//有可装备的神格
    public const int EQUIPED = 2;//已经装备
    public const int NOCANEQUIP = 3;//没有可装备的
    public const int LOCKED = 4;//未解锁

    //进入仓库的方式
    public const int CHANGE = 1;//替换
    public const int EQUIP = 2;//镶嵌
    
    //神格额外效果添加的附加效果类型
    public const int ADDATTACK = 1;//增加伤害
    public const int REDUECEDAMAGE = 2;//减免伤害

    //竞技场积分奖励领取方式
    public const int DOUBLEGET = 1;//双倍领取
    public const int THREEGET = 2;//三倍领取

    //神格合成是从哪进来的
    public const int STORAGE = 1;//仓库
    public const int SHENGEWINDOW = 2;//神格窗口

    public List<ShenGeCaoInfo> sidList;//已经镶嵌了的神格sid及槽位
    public List<ShenGeInfo> shengeList;//升级需要的神格列表 
    private static ShenGeManager instance;

    /* static methods */
    public static ShenGeManager Instance {
        get {
            if (instance == null)
                instance = new ShenGeManager();
            return instance;
        }
    }
    /// <summary>
    /// 判断该槽是否已经解锁了
    /// </summary>
    /// <returns></returns>
    public bool isLocked(int index) {
        if (StorageManagerment.Instance.getAllBeast().Count >= CommandConfigManager.Instance.getNumOfBeast()[index -1])
            return false;
        return true;
    }

    /// <summary>
    /// 检测是否可以升级 
    /// </summary>
    /// <param name="prop"></param>
    /// <returns></returns>
    public bool checkCanGroup(Prop prop,int type)
    {
        Prop tmpProp = prop;
        if (tmpProp == null) return false;
        PropSample sample = PropSampleManager.Instance.getPropSampleBySid(tmpProp.sid);
        PropSample nextSample = PropSampleManager.Instance.getPropSampleBySid(sample.nextLevelSid);
        if (nextSample == null) return false;
        int nextShenGeExp = nextSample.expValue;
        List<ShenGeInfo> list = getAllShenGeInStorage(tmpProp);

        //取得升级所需的神格列表
		if(shengeList == null)
        	shengeList = new List<ShenGeInfo>();
		else
		{
		    shengeList.Clear();
		}
        if (type == 1)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Prop temp = StorageManagerment.Instance.getProp(list[i].sid);
                if (temp.sid == tmpProp.sid)
                {
                    if (temp.getNum() == 1)
                        list.Remove(list[i]);
                    else list[i].num -= 1;
                }
            }
        }
        int sum = 0;
        for (int i = 0; i < list.Count; i++)
        {
            Prop temp = StorageManagerment.Instance.getProp(list[i].sid);
            //if (type == 1) {//仓库合成
            //    if (tmpProp.sid == temp.sid) {
            //        sum += (temp.getShenGeExp() * (temp.getNum() - 1));
            //    } else
            //        sum += (temp.getShenGeExp() * temp.getNum());
            //} else
            sum += (temp.getShenGeExp() * list[i].num);

            if (sum < (nextShenGeExp/2))
            {
                shengeList.Add(list[i]);
            } else if (sum == (nextShenGeExp/2))
            {
                shengeList.Add(list[i]);
                return true;
            }
            else
            {
                int moreNum = (sum - (nextShenGeExp/2))/temp.getShenGeExp();
                ShenGeInfo tmpInfo = new ShenGeInfo();
                tmpInfo.level = temp.getShenGeLevel();
                tmpInfo.sid = temp.sid;
                tmpInfo.num = list[i].num - moreNum;
                shengeList.Add(tmpInfo);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 把神格按高等级到低等级排列
    /// </summary>
    /// <param name="prop"></param>
    /// <returns></returns>
    public List<ShenGeInfo> getAllShenGeInStorage(Prop prop)
    {
        Prop tmpProp = prop;
        int shengeLevel = prop.getShenGeLevel();
        List<ShenGeInfo> shenGeList = new List<ShenGeInfo>();
        List<Prop> list = getTheSameTypeShenGe(tmpProp.getType());
        for (int i = shengeLevel; i > 0; i--)
        {
            for (int k = 0; k < list.Count; k++)
            {
                Prop temp = list[k];
                if (i == temp.getShenGeLevel())
                {
                    ShenGeInfo info = new ShenGeInfo();
                    info.sid = temp.sid;
                    info.level = i;
                    info.num = temp.getNum();
                    shenGeList.Add(info);
                }
            }
        }
        return shenGeList;
    }

    /// <summary>
    /// 是否有更高级的同类型的神格
    /// </summary>
    /// <param name="prop"></param>
    /// <returns></returns>
    public bool isHasBatterProp(Prop prop)
    {
        List<Prop> shenGes = getTheSameTypeShenGe(prop.getType());
        for (int i = 0; i < shenGes.Count; i++)
        {
            Prop tempProp = shenGes[i];
            if (prop.getShenGeLevel() < tempProp.getShenGeLevel())
                return true;
        }
        return false;
    }

    /// <summary>
    /// 是否有可以装备的神格
    /// </summary>
    /// <returns></returns>
    public bool isHasCanEquipShenGe()
    {
        ArrayList tempArrayList = getShowShenGeList(null);
        if (tempArrayList.Count > 0)
            return true;
        return false;
    }

    /// <summary>
    /// 获取仓库中所有的相同类型的神格
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public List<Prop> getTheSameTypeShenGe(int type)
    {
        ArrayList list = StorageManagerment.Instance.getAllProp();
        List<Prop> tmpList = new List<Prop>();
        for (int i = 0; i < list.Count; i++)
        {
            Prop tmProp = list[i] as Prop;
            if (tmProp.getType() == type)
            {
                tmpList.Add(tmProp);
            }
        }
        return tmpList;
    }

    /// <summary>
    /// 获取所有已装备的神格的类型
    /// </summary>
    /// <returns></returns>
    public ArrayList getAllEquipedTypes()
    {
        ArrayList list = new ArrayList();
        if (sidList == null) return list;
        for (int i = 0; i < sidList.Count; i++)
        {
            Prop prop = PropManagerment.Instance.createProp(sidList[i].sid);
            PropSample sample = PropSampleManager.Instance.getPropSampleBySid(sidList[i].sid);
            if (sample != null)
            {
                int type = prop.getType();
                list.Add(type);
            }
        }
        return list;
    }

    /// <summary>
    /// 获取在仓库中显示的神格
    /// </summary>
    /// <param name="choosedProp"></param>
    /// <returns></returns>
    public ArrayList getShowShenGeList(Prop choosedProp)
    {
        ArrayList typeList = getAllEquipedTypes();
        ArrayList allList = getAllShenGe();
        ArrayList cannotUseTypeList = getCannotUseShenGeTypes(typeList, choosedProp);
        for (int i = 0; i < cannotUseTypeList.Count; i++) {
            int tempType = (int)cannotUseTypeList[i];
            for (int j = 0; j < allList.Count; j++) {
                Prop tepProp = allList[j] as Prop;
                if (tempType == tepProp.getType()) {
                    allList.Remove(allList[j]);
                    j--;
                }
            }
        }
        //排序：由高等级到底等级至上而下排列
        for (int k = 0; k < allList.Count -1; k++)
        {
            for (int i = k + 1; i < allList.Count; i++)
            {
                Prop temp = allList[k] as Prop;
                Prop temProp = allList[i] as Prop;
                if (temp.getShenGeLevel() < temProp.getShenGeLevel())
                {
                    allList[k] = temProp;
                    allList[i] = temp;
                }
            }
        }
        return allList;
    }

    /// <summary>
    /// 获取所有镶嵌的神格信息
    /// </summary>
    /// <returns></returns>
    public List<ShenGeCaoInfo> getAllEquipedShenGeSid()
    {
        if(sidList == null)
            sidList = new List<ShenGeCaoInfo>();
        return sidList;
    }

    /// <summary>
    /// 获取所有的神格
    /// </summary>
    public ArrayList getAllShenGe() {
        ArrayList tempList = StorageManagerment.Instance.getAllProp();
        ArrayList propList = new ArrayList();
        for (int i = 0; i < tempList.Count; i++) {
            Prop tempProp = tempList[i] as Prop;
            if (tempProp.getType() == PropType.PROP_SHENGE_AGI || tempProp.getType() == PropType.PROP_SHENGE_ATT ||
                tempProp.getType() == PropType.PROP_SHENGE_DEF || tempProp.getType() == PropType.PROP_SHENGE_HP ||
                tempProp.getType() == PropType.PROP_SHENGE_MAG) {
                propList.Add(tempProp);
            }
        }
        return propList;
    }


    /// <summary>
    /// 获取不能使用的神格的类型
    /// </summary>
    /// <param name="types"></param>
    /// <returns></returns>
    public ArrayList getCannotUseShenGeTypes(ArrayList types,Prop choosedProp) {
        ArrayList tempList = new ArrayList();
        for (int i = 0; i < types.Count - 1; i++) {
            int type1 = (int)types[i];
            for (int j = i + 1; j < types.Count; j++) {
                int type2 = (int)types[j];
                if (type1 == type2) {
                    tempList.Add(type1);
                }
            }
        }
        if (choosedProp != null)
        {
            tempList.Remove(choosedProp.getType());
        }
        return tempList;
    }

    /// <summary>
    /// 计算上阵六个神格所带来的附加效果
    /// </summary>
    /// <returns></returns>
    public ShenGeExtraEffect CalculateExtrEffectValue() {
        List<ShenGeCaoInfo> shenGeCaoInfos = getAllEquipedShenGeSid();
        for (int i = 0; i < shenGeCaoInfos.Count; i++) {
            if (shenGeCaoInfos[i].sid == 0)
                shenGeCaoInfos.Remove(shenGeCaoInfos[i]);
        }
        if (shenGeCaoInfos.Count < 6)
            return null;

        Prop tmp = PropManagerment.Instance.createProp(shenGeCaoInfos[0].sid);
        PropSample sample = PropSampleManager.Instance.getPropSampleBySid(shenGeCaoInfos[0].sid);
        int minLevel = 0;
        if(sample != null)
            minLevel = tmp.getShenGeLevel();
        for (int i = 1; i < shenGeCaoInfos.Count; i++) {
            Prop tempProp = PropManagerment.Instance.createProp(shenGeCaoInfos[i].sid);
            PropSample samplee = PropSampleManager.Instance.getPropSampleBySid(shenGeCaoInfos[i].sid);
            if (samplee != null)
            {
                if (minLevel > tempProp.getShenGeLevel())
                    minLevel = tempProp.getShenGeLevel();
            }
        }
        List<ShenGeExtraEffect> extraList = CommandConfigManager.Instance.GetShenGeExtraEffectsList();
        if (minLevel < extraList[0].level)
            return null;
        if (minLevel >= extraList[extraList.Count - 1].level)
            return extraList[extraList.Count - 1];
        for (int i = 0; i < extraList.Count; i++) {
            int level = extraList[i].level;
            if (minLevel < level) {
                return extraList[i - 1];
            }
        }
        return null;
    }

    /// <summary>
    /// 计算神格威能
    /// </summary>
    /// <returns></returns>
    public ShenGePower CalculateShenGePower() {
        List<ShenGeCaoInfo> shenGeCaoInfos = getAllEquipedShenGeSid();
        for (int i = 0; i < shenGeCaoInfos.Count; i++) {
            if (shenGeCaoInfos[i].sid == 0)
                shenGeCaoInfos.Remove(shenGeCaoInfos[i]);
        }
        int sumLevel = 0;
        for (int i = 0; i < shenGeCaoInfos.Count; i++) {
            Prop tempProp = PropManagerment.Instance.createProp(shenGeCaoInfos[i].sid);
            PropSample sample = PropSampleManager.Instance.getPropSampleBySid(shenGeCaoInfos[i].sid);
            if (sample != null) sumLevel += tempProp.getShenGeLevel();
        }
        List<ShenGePower> shenGePowers = CommandConfigManager.Instance.GetsheGePowersList();
        if (sumLevel < shenGePowers[0].level)
            return null;
        if (sumLevel >= shenGePowers[shenGePowers.Count - 1].level)
            return shenGePowers[shenGePowers.Count - 1];
        for (int i = 0; i < shenGePowers.Count; i++) {
            int level = shenGePowers[i].level;
            if (sumLevel < level) {
                return shenGePowers[i - 1];
            }
        }
        return null;
    }
}

public class ShenGeInfo
{
    public int sid;
    public int num;
    public int level;
}

public class ShenGeCaoInfo
{
    public int index;
    public int sid;
}

public class ShenGeExtraEffect
{
    public int level;
    public List<AttrInfo> AttrInfos;
}

public class ShenGePower
{
    public int level;//神格总等级
    public List<AttrInfo> AttrInfos;

}

public class AttrInfo
{
    public int type;
    public int value;
}
