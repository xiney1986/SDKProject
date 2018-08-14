using System;

/**
 * 道具实体
 * @author longlingquan
 * */
public class Prop:StorageProp
{
	public Prop ()
	{

	}

	public Prop (int sid, int num)
	{
		this.sid = sid;
		setNum (num);
	}

	/** 是否是卡片碎片 */
	public bool isCardScrap()
	{
		return this.getType() == PropType.PROP_TYPE_CARDSCRAP;
	}

	/** 是否是卡片碎片 */
	public bool isEquipScrap()
	{
		return this.getType() == PropType.PROP_TYPE_EQUIPSCRAP;
	}

    /// <summary>
    /// 是否是秘宝碎片
    /// </summary>
    /// <returns></returns>
    public bool isMagicScrap()
    {
        return getType() == PropType.PROP_MAGIC_SCRAP;
    }

   /// <summary>
    ///  /**是否是红色（2~5）星万能卡*/
    ///  //1~4星的红色万能卡可以合成高一星级的万能卡
   /// </summary>
   /// <returns></returns>
    public bool isCanExchageCard() {
        if (this.sid == 71196 || this.sid == 71197 || this.sid == 71198 || this.sid == 71199) {
            return true;
        }
        return false;
    }

    public bool isShenGeProp()
    {
        if (this.getType() == PropType.PROP_SHENGE_AGI || this.getType() == PropType.PROP_SHENGE_ATT ||
            this.getType() == PropType.PROP_SHENGE_DEF ||
            this.getType() == PropType.PROP_SHENGE_HP || this.getType() == PropType.PROP_SHENGE_MAG)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 是否是红色万能卡或红色万能卡碎片
    /// </summary>
    /// <returns></returns>
    public bool isRedOmnipotentCardOrScrap() {
        if (this.sid == 71196 || this.sid == 71197 || this.sid == 71198 || this.sid == 71199 ||
            this.sid == 71200 || this.sid == 71204 || this.sid == 71205 || this.sid == 71206
            || this.sid == 71207 || this.sid == 71208) {
            return true;
        }
        return false;
    }
	/** 是否是碎片 */
	public bool isScrap()
	{
		return this.getType() == PropType.PROP_TYPE_CARDSCRAP || this.getType() == PropType.PROP_TYPE_EQUIPSCRAP||this.getType()==PropType.PROP_MAGIC_SCRAP;
	}
	
	public int getSiftType ()
	{
		return PropSampleManager.Instance.getPropSampleBySid (sid).siftType;
	}
	//获得道具名称
	public string getName ()
	{
		return PropSampleManager.Instance.getPropSampleBySid (sid).name;
	}
	
	//获得道具描述
	public string getDescribe ()
	{
		return PropSampleManager.Instance.getPropSampleBySid (sid).describe.Replace ('~', '\n');
	}
	//获得图标id
	public int getIconId ()
	{
		return PropSampleManager.Instance.getPropSampleBySid (sid).iconId;
	}
	public PrizeSample[] getPrizeSampleLcok(){
		return PropSampleManager.Instance.getPropSampleBySid(sid).prizes;
	}
	public PrizeSample[] getNeedPropLcok(){
		return PropSampleManager.Instance.getPropSampleBySid(sid).needProps;
	}
	//获得品质
	public int getQualityId ()
	{
		return PropSampleManager.Instance.getPropSampleBySid (sid).qualityId;
	}
	//获得出售价格
	public int getPrice ()
	{
		return PropSampleManager.Instance.getPropSampleBySid (sid).price;
	}
	//获得道具类型
	public int getType ()
	{
		return PropSampleManager.Instance.getPropSampleBySid (sid).type;
	}
	//获得使用道具后的影响值
	public int getEffectValue ()
	{
		return PropSampleManager.Instance.getPropSampleBySid (sid).effectValue;
	}
	//获得道具是否可出售
	public bool getIsSell ()
	{
		return PropSampleManager.Instance.getPropSampleBySid (sid).isSell;
	}
	//获得道具限制使用等级
	public int getUseLv ()
	{
		return PropSampleManager.Instance.getPropSampleBySid (sid).useLv;
	}
    public int getUseCombat() {
        return PropSampleManager.Instance.getPropSampleBySid(sid).useLv;
    }
	
	//获得道具每日最大使用次数限制
	public int getMaxUseCount ()
	{
		return PropSampleManager.Instance.getPropSampleBySid (sid).maxUseCount;
	}

    //获得神格等级
    public int getShenGeLevel()
    {
        return PropSampleManager.Instance.getPropSampleBySid(sid).level;
    }

    //获得神格经验
    public int getShenGeExp()
    {
        return PropSampleManager.Instance.getPropSampleBySid(sid).expValue;
    }

    public int getNextShenGeSid()
    {
        return PropSampleManager.Instance.getPropSampleBySid(sid).nextLevelSid;
    }

    public override bool equal (StorageProp prop)
	{
		return this.sid == prop.sid;
	}

	public override void bytesRead (int j, ErlArray ea)
	{
		this.sid = StringKit.toInt (ea.Value [j++].getValueString ());
		setNum (StringKit.toInt (ea.Value [j++].getValueString ()));
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
	}

	public int GetOrderId(){
		return PropSampleManager.Instance.getPropSampleBySid (sid).orderId;
	}
} 

