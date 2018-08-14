using System;
 
/**
 * buff实体
 * @author longlingquan
 * */

public class BuffIconType
{
	public const int None = 1, //不显示buff
	Small_Icon = 2, //不显示buff小图标在血条下
	BodyEffect = 3;//buff图标显示在身上
}

public class BuffType
{
	public const int power = 1, //怒气槽
	damage = 2, //直接伤害
	durationDamage = 3, //持续伤害
	attr_change = 4, //属性改变	
	frozen = 5, //冰冻
	vertigo = 6, //眩晕
	silence = 7, //沉默 
	ImmuneFrozen=8, //免疫冰冻
	ImmuneVertigo=9, //免疫眩晕
	ImmunePoison=10, //免疫毒
	ImmuneSilence=11, //免沉默
	ImmuneAll=12, //全免疫
	swind=13, //魅惑
	chaos=14, //混乱
	shapeshifting=15, //变身
    jinghua=16;//净化
}
//伤害类型
public class BuffDamageType
{
	public const int none = 0,
	light = 1, //光
	dark = 2, //暗
	poison = 3, //毒
	fire = 4, //火
	ice = 5, //冰冻
	ray = 6,//雷
	beRebound=7,//被反击
	beIntervene=8;//被援护
}

public class Buff
{
	 int DamageType = BuffDamageType.none;
	
	public Buff (int sid)
	{
		this.sid = sid;
		DamageType=BuffSampleManager.Instance.getBuffSampleBySid (sid).damageType;
	}

	public int sid = 0;
	
	public string getName ()
	{
		return BuffSampleManager.Instance.getBuffSampleBySid (sid).name;
	}
 
	public int getDisplayType ()
	{
		return BuffSampleManager.Instance.getBuffSampleBySid (sid).displayType;
	}
	/** 获取buff特效路径 */
	public string getResPath ()
	{
		return BuffManagerment.Instance.getResPath (sid);
	}
	
	public int getType ()
	{
		return BuffSampleManager.Instance.getBuffSampleBySid (sid).type;
	}
	
	//持续buff激活时候的效果 
	public string getDurationEffect ()
	{
		return BuffManagerment.Instance.getDurationEffect (sid); 
	} 
	//变形ID
	public string getTransformID ()
	{
		return  BuffSampleManager.Instance.getBuffSampleBySid (sid).transformID;
	} 
	//持续buff激活时候的效果 
	public string getDurationEffectForBOSS ()
	{
		//检测有没对应boss技能
		string path= BuffManagerment.Instance.getDurationEffectForBoss (sid); 
		if(path!=null)
			return path;
		else
			return BuffManagerment.Instance.getDurationEffect (sid); 
	} 
	//buff的伤害类型
	public int getDamageType ()
	{
		return  DamageType;
	}
	//buff的伤害类型
	public void changeDamageType (int dtype)
	{
//		MonoBase.print("buff damge buff:"+dtype);
		DamageType=dtype;
	}

} 

