using System;
 
/**装备模板 
  *@author longlingquan
  **/
public class EquipSample:Sample
{
	public EquipSample ()
	{
	}
	
	public string name = "";//装备名字
	public int iconId = 0;//图标编号
	public int levelId = 0;//等级指向
	public int maxLevel = 0;//最高等级 
	public int qualityId = 0;//品质编号
	
	
	public int partId = 0;//装备部件编号
	public int suitSid = 0;//套装sid
	
	public int baseLife = 0;//初始生命值
	public int baseAttack = 0;//初始攻击值
	public int baseDefecse = 0;//初始防御值
	public int baseMagic = 0;//初始魔力值
	public int baseAgile = 0;//初始敏捷值
	 
	public int developLife = 0;//生命成长值
	public int developAttack = 0;//攻击成长值
	public int developDefecse = 0;//防御成长值
	public int developMagic = 0;//魔力成长值
	public int developAgile = 0;//敏捷成长值
	
	public int eatenExp = 0;//被吃经验
	public int[] skillSids;//附带技能
	public int[] exclusive;//专属装备角色sid
	public int sell;//卖出价格
	public AttrChangeSample[] effects = null;//对描述参数提供数值(影响描述信息数值)
	public string desc;//描述信息
	
	public int equipJob = 0;//装备职业
	public PrizeSample[] resolveResults;//分解结果
	public int equipStarSid;  //装备升星sid
    public int redEquipSid;//升级为红色装备后的sid
    public PrizeSample[] upQualityCost;//装备升红消耗
	
	public override void parse (int sid, string str)
	{
		this.sid = sid;
		string[] strArr = str.Split ('|');
		//checkLength (strArr.Length, 28);
		
		//strArr[0] is sid  
		//strArr[1] name
		this.name = strArr [1];
		//strArr[2] iconId
		this.iconId = StringKit.toInt (strArr [2]); 
		//strArr[3] levelId
		this.levelId = StringKit.toInt (strArr [3]); 
		//strArr[4] maxLevel
		this.maxLevel = StringKit.toInt (strArr [4]);  
		//strArr[5] qualityId
		this.qualityId = StringKit.toInt (strArr [5]);   
		//strArr[6] partId
		this.partId = StringKit.toInt (strArr [6]);    
		//strArr[7] suitSid
		this.suitSid = StringKit.toInt (strArr [7]);     
		
		//strArr[8] baseLife
		this.baseLife = StringKit.toInt (strArr [8]);   
		//strArr[9] baseAttack  
		this.baseAttack = StringKit.toInt (strArr [9]);   
		//strArr[10] baseDefecse  
		this.baseDefecse = StringKit.toInt (strArr [10]);   
		//strArr[11] baseMagic     
		this.baseMagic = StringKit.toInt (strArr [11]);   
		//strArr[12] baseAgile
		this.baseAgile = StringKit.toInt (strArr [12]);   
		
		//strArr[13] developLife
		this.developLife = StringKit.toInt (strArr [13]);   
		//strArr[14] developAttack
		this.developAttack = StringKit.toInt (strArr [14]);   
		//strArr[15] developDefecse
		this.developDefecse = StringKit.toInt (strArr [15]);   
		//strArr[16] developMagic
		this.developMagic = StringKit.toInt (strArr [16]);   
		//strArr[17] 
		this.developAgile = StringKit.toInt (strArr [17]);   
		 
		//strArr[18] skillSids 
		parseSkills (strArr [18]); 
		//strArr[19] eatenExp
		this.eatenExp = StringKit.toInt (strArr [19]);   
		//strArr[20] exclusive
		parseExclusive (strArr [20]); 
		//strArr[21] sell
		this.sell = StringKit.toInt (strArr [21]);   
		//strArr[22] effects
		parseEffects (strArr [22]);
		//strArr[23] desc
		this.desc = strArr [23]; 
		//strArr[24] equipJob
		this.equipJob = StringKit.toInt (strArr [24]);
		//strArr[25] resolveResults
		parseResolve (strArr [25]);
		//stsrArr[26] equipStarSid
		this.equipStarSid = StringKit.toInt (strArr [26]);
	    if (strArr.Length > 27)
	        this.redEquipSid = StringKit.toInt(strArr[27]);
        if(strArr.Length >28)
            parseUpQualityCost(strArr[28]);

	}

    //解析升红消耗
    private void parseUpQualityCost(string str)
    {
        if (str == "0")
            return;
        string[] strs = str.Split('#');
        upQualityCost = new PrizeSample[strs.Length];
        for (int i = 0; i < strs.Length; i++)
        {
            upQualityCost[i] = new PrizeSample();
            string[] strss = strs[i].Split(',');
            upQualityCost[i].type = StringKit.toInt(strss[0]);
            upQualityCost[i].pSid = StringKit.toInt(strss[1]);
            upQualityCost[i].num = strss[2];
        }
    }

    //解析分解结果
	private void parseResolve (string str)
	{
		if (str == "0")
			return;
		string[] strArr = str.Split ('#');
		resolveResults = new PrizeSample[strArr.Length];
		for (int i = 0; i < strArr.Length; i++) {
			resolveResults[i]=new PrizeSample();
			string[] strs = strArr[i].Split(',');
			resolveResults[i].type = StringKit.toInt(strs[0]);
			resolveResults[i].pSid = StringKit.toInt(strs[1]);
			resolveResults[i].num = strs[2];
		}
		
	}
	//3,71001,10#5,0,10000
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
	
	//解析技能
	private void parseSkills (string str)
	{
		string[] strArr = str.Split (',');
		skillSids = new int[strArr.Length];
		for (int i =0; i<strArr.Length; i++) {
			skillSids [i] = StringKit.toInt (strArr [i]);			
		}
	}
	
	//解析专属id
	private void parseExclusive (string str)
	{
		string[] strArr = str.Split (',');
		exclusive = new int[strArr.Length];
		for (int i =0; i<strArr.Length; i++) {
			exclusive [i] = StringKit.toInt (strArr [i]);			
		}
	}

	public override void copy (object destObj)
	{
		base.copy (destObj);
		EquipSample dest = destObj as EquipSample;
		if (this.skillSids != null) {
			dest.skillSids = new int[this.skillSids.Length];
			for (int i = 0; i < this.skillSids.Length; i++)
				dest.skillSids [i] = this.skillSids [i];
		}
		if (this.exclusive != null) {
			dest.exclusive = new int[this.exclusive.Length];
			for (int i = 0; i < this.exclusive.Length; i++)
				dest.exclusive [i] = this.exclusive [i];
		}
		if (this.effects != null) {
			dest.effects = new AttrChangeSample[this.effects.Length];
			for (int i = 0; i < this.effects.Length; i++)
				dest.effects [i] = this.effects [i].Clone () as AttrChangeSample;
		}
		if (this.resolveResults != null) {
			dest.resolveResults = new PrizeSample[this.resolveResults.Length];
			for (int i = 0; i < this.resolveResults.Length; i++)
				dest.resolveResults [i] = this.resolveResults [i];
		}
	}
} 

