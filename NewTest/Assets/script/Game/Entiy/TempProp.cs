using System;

/**
 * 临时道具
 * @author zhoujie
 * */
public class TempProp:StorageProp
{
	public TempProp ()
	{

	}

	public int time; // 时间
	public string tempUid; //临时道具uid
	public string type; // 类型

	//获得名字
	public string getName ()
	{
		if (type == TempManagerment.card) {
			return CardSampleManager.Instance.getRoleSampleBySid (sid).name;
		} else if (type == TempManagerment.equipment) {
			return EquipmentSampleManager.Instance.getEquipSampleBySid (sid).name;
		} else if (type == TempManagerment.goods) {
			return PropSampleManager.Instance.getPropSampleBySid (sid).name;
		} else if (type == TempManagerment.beast) {
			return CardSampleManager.Instance.getRoleSampleBySid (sid).name;
		} else if (type == TempPropType.STARSOUL) {
			return StarSoulSampleManager.Instance.getStarSoulSampleBySid (sid).name;
		} else {
			return "";
		}
	}
	
	//获得图标编号
	public int getIconId ()
	{
		if (type == TempManagerment.card) {
			return CardSampleManager.Instance.getRoleSampleBySid (sid).imageID;
		} else if (type == TempManagerment.equipment) {
			return EquipmentSampleManager.Instance.getEquipSampleBySid (sid).iconId;
		} else if (type == TempManagerment.goods) {
			return PropSampleManager.Instance.getPropSampleBySid (sid).iconId;
		} else if (type == TempManagerment.beast) {
			return CardSampleManager.Instance.getRoleSampleBySid (sid).imageID;
		} else {
			return 0;
		}
	}
	
	//获得品质
	public int getQualityId ()
	{
		if (type == TempManagerment.card) {
			return CardSampleManager.Instance.getRoleSampleBySid (sid).qualityId;
		} else if (type == TempManagerment.equipment) {
			return EquipmentSampleManager.Instance.getEquipSampleBySid (sid).qualityId;
		} else if (type == TempManagerment.beast) {
			return CardSampleManager.Instance.getRoleSampleBySid (sid).qualityId;
		} else {
			return 0;
		}
	}
	
	//获得星级
	public int getStarLevel ()
	{
		if (type == TempManagerment.card) {
			return CardSampleManager.Instance.getRoleSampleBySid (sid).starLevel;
		} else if (type == TempManagerment.beast) {
			return CardSampleManager.Instance.getRoleSampleBySid (sid).starLevel;
		} else {
			return 0;
		}
	}
	/** 检查过期，true过期 */
	public bool checkOverdue ()
	{
		return this.time - ServerTimeKit.getSecondTime () <= 0;
	}

	public override bool equal (StorageProp prop)
	{
		return this.tempUid == (prop as TempProp).tempUid;
	}

	public override void bytesRead (int j, ErlArray ea)
	{
		this.tempUid = ea.Value [j++].getValueString ();
		this.uid = ea.Value [j++].getValueString ();
		this.time = StringKit.toInt (ea.Value [j++].getValueString ());
		this.sid = StringKit.toInt (ea.Value [j++].getValueString ());
		this.type = (ea.Value [j++] as ErlAtom).Value; 
		if (this.type != TempManagerment.goods)
			this.isU = true;
		setNum (StringKit.toInt (ea.Value [j++].getValueString ()));
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
	}
}

