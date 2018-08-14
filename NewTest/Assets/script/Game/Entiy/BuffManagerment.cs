using System;
 
/**
 * buff管理器
 * @author longlingquan
 * */
public class BuffManagerment
{

	public BuffManagerment ()
	{ 

	}
	 
	public static BuffManagerment Instance {
		get{return SingleManager.Instance.getObj("BuffManagerment") as BuffManagerment;}
	}
	
	public Buff createBuff (int sid)
	{
		return new Buff (sid);
	}
	
	//得到buff特效
	public string getResPath(int sid)
	{
		int id = BuffSampleManager.Instance.getBuffSampleBySid (sid).resPath;
		return EffectConfigManager.Instance.getEffectPerfab (id); 
	}
	
	//持续buff激活时候的效果 
	public string getDurationEffect (int sid)
	{
		int id = BuffSampleManager.Instance.getBuffSampleBySid (sid).damageEffect;
		return EffectConfigManager.Instance.getEffectPerfab (id);
	}

	//持续buff激活时候的效果 
	public string getDurationEffectForBoss (int sid)
	{
		int id = BuffSampleManager.Instance.getBuffSampleBySid (sid).damageEffect;
		return EffectConfigManager.Instance.getEffectPerfab (id+20000);
	}
    public string getSkillIconPath(int sid) { 
        BuffSample sample = BuffSampleManager.Instance.getBuffSampleBySid(sid);
        return ResourcesManager.SKILLIMAGEPATH + sample.skillIcon;
    }

} 

