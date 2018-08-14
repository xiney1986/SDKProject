using System;
using System.Collections;
using System.Collections.Generic;

/**
 * 道具仓库
 * @author longlingquan
 * */
public class PropStorage:Storage
{
	public PropStorage ()
	{
		
	}
	/** 检查能否添加一个sid的道具，true能 */
	public bool isAddProp (int sid){
		if(checkSize(1))
			return true;
		return getPropBySid(sid) != null;
	}
	/** 检查能否添加一组sid的道具，true能 */
	public bool isAddProp (int[] sids){
		int l=sids.Length;
		if(checkSize(l))
			return true;
		int num=0;
		for(int i=0; i<l; i++){
			if(getPropBySid(sids[i]) == null)
				num++;
		}
		return checkSize(num);
	}
	/** 检查能否添加一个道具，true能 */
	public bool isAddProp (Prop prop){
		return checkAddProp(prop);
	}
	/** 检查能否添加一组道具，true能 */
	public bool isAddProp (Prop[] props){
		int l=props.Length;
		if(checkSize(l))
			return true;
		int num=0;
		for(int i=0; i<l; i++){
			if(getPropBySid(props[i].sid) == null)
				num++;
		}
		return checkSize(num);
	}

	/** 获得非碎片道具 */
	public ArrayList getAllPropExcludeScrap ()
	{
		ArrayList props = getStorageProp ();
		ArrayList temp = new ArrayList ();
		for (int i = 0; i < props.Count; i++) {
			Prop c = props [i] as Prop;
			if (c.getType() == PropType.PROP_TYPE_CARDSCRAP || c.getType() == PropType.PROP_TYPE_EQUIPSCRAP||c.getType()==PropType.PROP_MAGIC_SCRAP) {
				continue;
			} else {
				c.index = i;//重置索引
				temp.Add (c);
			}
		}
		return temp;
	}

	/** 获得卡片碎片道具 */
	public ArrayList getAllPropByCardScrap ()
	{
		ArrayList props = getStorageProp ();
		ArrayList temp = new ArrayList ();
		for (int i = 0; i < props.Count; i++) {
			Prop c = props [i] as Prop;
			if (c.getType() == PropType.PROP_TYPE_CARDSCRAP) {
				c.index = i;//重置索引
				temp.Add (c);
			}
		}
		temp.Sort(new PropComp());
		return temp;
	}

	/** 获得装备碎片道具 */
	public ArrayList getAllPropByEquipScrap ()
	{
		ArrayList props = getStorageProp ();
		ArrayList temp = new ArrayList ();
		for (int i = 0; i < props.Count; i++) {
			Prop c = props [i] as Prop;
			if (c.getType() == PropType.PROP_TYPE_EQUIPSCRAP) {
				c.index = i;//重置索引
				temp.Add (c);
			}
		}
		temp.Sort(new PropComp());
		return temp;
	}
    /// <summary>
    /// 获得秘宝碎片道具
    /// </summary>
    /// <returns></returns>
    public ArrayList getAllPropByMagicScrap() {
        ArrayList props = getStorageProp();
        ArrayList temp = new ArrayList();
        for (int i = 0; i < props.Count;i++ ) {
            Prop c = props[i] as Prop;
            if (c.getType() == PropType.PROP_MAGIC_SCRAP) {
                c.index = i;
                temp.Add(c);
            }
        }
        temp.Sort(new PropComp());
        return temp;
    }

	//检查道具使用不足
//	public bool checkProp(Prop prop,int num)
//	{
//		return  prop.getNum() >= num ? true : false;
//	}
	//初始化解析数据
	public override void parse (ErlArray arr)
	{
		ErlArray ea1 = arr.Value [1] as ErlArray;
		if (ea1.Value.Length <= 0) {
			init (StringKit.toInt (arr.Value [0].getValueString ()),null);
		} else {
			ArrayList al = new ArrayList ();
			Prop prop;
			for (int i=0; i < ea1.Value.Length; i++) {
				prop=PropManagerment.Instance.createProp();
				prop.bytesRead(0,ea1.Value [i] as ErlArray);
				al.Add(prop);
			}
			init (StringKit.toInt (arr.Value [0].getValueString ()), al);
		}
	}

	//装备根据品质排序
	public class PropComp : IComparer
	{
		public int Compare(object o1,object o2)
		{
			if(o1==null) return 1;
			if(o2==null) return -1;
			Prop prop1 = o1 as Prop;
			Prop prop2 = o2 as Prop;
			if(prop1==null || prop2==null) return 0;
			if(prop1.getQualityId()==prop2.getQualityId())
			{
				if(prop1.getSiftType()>prop2.getSiftType())
					return -1;
				if(prop1.getSiftType()<prop2.getSiftType())
					return 1;
				return 0;
			}
			else
			{
				if(prop1.getQualityId()>prop2.getQualityId())
					return -1;
				if(prop1.getQualityId()<prop2.getQualityId())
					return 1;
				return 0;
			}
		}
	}
} 

