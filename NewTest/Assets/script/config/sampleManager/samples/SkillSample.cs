using System;
 
/**技能模板 
 * 
 * sid name iconId    levelId    effects(str[])  describe   type        activeType    buffId	      spellEffect    
 * sid 名字 图标编号  等级指向   影响效果	 描述	 技能类型	  触发时间类型  技能附带buff  施法特效 
 *  isNeedSpell	bulletPrefab  isAOE	
 * 是否需要吟唱	子弹特效路径  是否是AOE	
 * 
 * 技能数据结构 50001|麻痹攻击|5|1|paralysis,2,3,12,35#hp,1,1,1,1|%1几率使对方麻痹|5|2|3|4|1|/ss/dad|1
 *@author longlingquan
 **/
public class SkillSample:Sample
{
	public SkillSample ()
	{
		
	}
	  
	public string name = "";//技能名字
	public string iconId = "";//图标编号
	public int levelId = 0;//等级指向
	public AttrChangeSample[] effects = null;//技能产生属性影响效果(影响角色本身属性) 同时对技能描述参数提供数值(影响描述信息数值)
	public string describe = "";//技能描述 模板
	public int type = 0;//技能类型 枚举skillType.cs
	public int activeType = 0;//技能时间触发类型 枚举
	public int buffSid = 0;//技能附带buff sid
	public int spellEffect = 0;//技能施法特效
	public int  aroundEffect = 0;//吟唱后技能播放中人物身上的效果
	public bool isNeedSpell = false;//施法施法需要吟唱
	public int bulletEffect = 0;//子弹特效
//	public bool isAOE = false;//AOE子弹，没有发射过程，直接在目标点播放爆炸动画，完成后激活buff
	public int damageEffect = 0;//技能被击中效果 	
	public int maxLevel = 0;// 最大等级
	public int quality = 0;//技能品质
	public bool isBind; //是否绑定,不能替换
	public bool canHitBack; //是否可以击退敌人
	public int attackNum; //单次后台攻击前台的拆分数量
	public int showType;//展示类型:1主动技能，2天赋，3特性
    public int tempSid = 29548;
	
	override public void parse (int sid, string str)
	{  
		this.sid = sid;  
		string[] strArr = str.Split ('|');
		checkLength (strArr.Length, 19); 
		//strArr[0] is sid  
		//strArr[1] name
		this.name = strArr [1];

		//strArr[2] iconId
		this.iconId = strArr [2];
		//strArr[3] levelId
		this.levelId = StringKit.toInt (strArr [3]);
		//strArr[4] effects
		parseEffects (strArr [4]);
		//strArr[5] describe
		this.describe = strArr [5];
		//strArr[6] type
		this.type = StringKit.toInt (strArr [6]);
		//strArr[7] activeType
		this.activeType = StringKit.toInt (strArr [7]);
		//strArr[8] buffSid
		this.buffSid = StringKit.toInt (strArr [8]);
		//strArr[9] spellEffect
		this.spellEffect = StringKit.toInt (strArr [9]);
		//strArr[10] isNeedSpell
		parseIsNeedSpell (strArr [10]);
		//strArr[11] bulletEffect
		this.bulletEffect = StringKit.toInt (strArr [11]);
		//原来是是否aoe标志,换成环绕特效
		this.aroundEffect = StringKit.toInt (strArr [12]);
		//strArr[13] damageEffect
		this.damageEffect = StringKit.toInt (strArr [13]);
		//strArr[14] maxLevel
		this.maxLevel = StringKit.toInt (strArr [14]);
		this.quality = StringKit.toInt (strArr [15]);
		this.isBind = StringKit.toInt (strArr [16]) == 1;

		if (strArr [17] == "1")
			this.canHitBack = true;
		else
			this.canHitBack = false;

		this.attackNum = StringKit.toInt (strArr [18]);
		this.showType = StringKit.toInt (strArr [19]);

	}

	private void parseEffects (string str)
	{
		//表示空
		if (str == 0 + "")
			return;
		string[] strArr = str.Split ('#');  
		effects = new AttrChangeSample[strArr.Length]; 
		for (int i = 0; i<strArr.Length; i++) {
			AttrChangeSample attr = new AttrChangeSample ();
			attr.parse (strArr [i]);
			effects [i] = attr;
		}
	}
	  
	private void parseIsNeedSpell (string str)
	{
		if (str == "1")
			this.isNeedSpell = true; 
	}
	 
	public override void copy (object destObj)
	{
		base.copy (destObj);
		SkillSample dest = destObj as SkillSample;
		if (this.effects != null) {
			dest.effects = new AttrChangeSample[this.effects.Length];
			for (int i = 0; i < this.effects.Length; i++)
				dest.effects [i] = this.effects [i].Clone () as AttrChangeSample;
		}
	}
} 

