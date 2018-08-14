using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
 
/**技能模板管理器
  *负责技能模板信息的初始化 
  *@author longlingquan
  **/
public class SkillSampleManager:SampleConfigManager
{ 
	//单例
	private static SkillSampleManager _Instance;
	private static bool _singleton = true; 
	
	
	//特殊技能sid 前台唯一 -1 移除buff -2 胜利 -3连击 -4合击 -6添加替补 -7添加剧情npc  -8移除剧情npc   -9对话   -10播放特效退出战斗

	public const int SID_COMBO_ATTACK = -3;
    public const int SID_COMBO_ATTACKJ = -33;
	public const int SID_GROUP_ATTACK = -4;
	public const int SID_CHECK_BUFF = -5;
	public const int SID_ADD_CARD = -6;
    public const int SID_ADD_NPC = -7;
    public const int SID_DEL_NPC = -8;
    public const int SID_TALK = -9;
    public const int SID_EFFECT_EXIT = -10;
	
	public static SkillSampleManager Instance {
		get {
			
			if (_Instance == null) {
				_singleton = false;
				_Instance = new SkillSampleManager ();
				_singleton = true;
				return _Instance;
			} else
				return _Instance;
		}
		set { 
			_Instance = value;
		}
	}

	public SkillSampleManager ()
	{ 
		if (_singleton) {
			throw new Exception ("This is singleton!");
		}
		base.readConfig (ConfigGlobal.CONFIG_SKILL);
		initSpecialSkillSample ();
	} 
	 
	//获得技能模板对象
	public SkillSample getSkillSampleBySid (int sid)
	{
		if (!isSampleExist (sid))
			createSample (sid); 
		return samples [sid] as SkillSample;
	}   
	
	//解析模板数据
	public override void parseSample (int sid)
	{
		SkillSample sample = new SkillSample (); 
		string dataStr = getSampleDataBySid (sid); 
		sample.parse (sid, dataStr); 
		samples.Add (sid, sample);
	}
	
	//添加前台特殊技能 前台专有 sid唯一 手动添加
	private void initSpecialSkillSample ()
	{
		SkillSample sample = new SkillSample (); 
		sample.name = "buff check";
		sample.sid = SID_CHECK_BUFF;		
		sample.activeType = SkillActiveType.None;
		sample.type = SkillType.BuffCheck;
		sample.spellEffect = 25;
		sample.damageEffect = 22; 
		samples.Add (sample.sid, sample);	  
		 
		sample = new SkillSample ();
		sample.name = "combo  attack(follow)";
		sample.sid = SID_COMBO_ATTACK;		
		sample.damageEffect = 15;
		sample.activeType = SkillActiveType.SpellEnd;
		sample.type = SkillType.ComboAttack;
		sample.spellEffect = 25;
		samples.Add (sample.sid, sample);

        sample = new SkillSample();
        sample.name = "combo  attack(follow)";
        sample.sid = SID_COMBO_ATTACKJ;
        sample.damageEffect = 522;
        sample.activeType = SkillActiveType.SpellEnd;
        sample.type = SkillType.ComboAttack;
        sample.spellEffect = 25;
        samples.Add(sample.sid, sample);
		
		sample = new SkillSample ();
		sample.name = "(GroupAttack) follow attack";
		sample.sid = SID_GROUP_ATTACK;		
		sample.spellEffect = 25;
		sample.damageEffect = 22;
		sample.bulletEffect = 9;
		sample.activeType = SkillActiveType.Grouped;
		sample.type = SkillType.GroupAttack;
		samples.Add (sample.sid, sample);	 
		
		sample = new SkillSample ();
		sample.name = "add card";
		sample.sid = SID_ADD_CARD;		 
		sample.activeType = SkillActiveType.SkillUse;
		sample.type = SkillType.AddCard;
		samples.Add (sample.sid, sample);	
		
		sample = new SkillSample ();
		sample.name = "add npc";
		sample.sid = SID_ADD_NPC;		 
		sample.activeType = SkillActiveType.SkillUse;
		sample.type = SkillType.AddNPC;
		samples.Add (sample.sid, sample);	 
		
		sample = new SkillSample ();
		sample.name = "del npc";
		sample.sid = SID_DEL_NPC;		 
		sample.activeType = SkillActiveType.SkillUse;
		sample.type = SkillType.DelNPC;
		samples.Add (sample.sid, sample);	 

        sample = new SkillSample ();
        sample.name = "plot talk";
        sample.sid = SID_TALK;        
        sample.activeType = SkillActiveType.SkillUse;
        sample.type = SkillType.Talk;
        samples.Add (sample.sid, sample);   

        sample = new SkillSample ();
        sample.name = "effect exit";
        sample.sid = SID_EFFECT_EXIT;        
        sample.activeType = SkillActiveType.SkillUse;
        sample.type = SkillType.EffectExit;
        samples.Add (sample.sid, sample);   
	}
	 
} 

