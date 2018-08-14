using System;
 
/**角色卡片模板   
 * 详见配置说明文件
 *@author longlingquan
 **/
public class CardSample:Sample
{
	public CardSample ()
	{
		
	}

	public string name = "";//角色名字
	public int imageID = 0;//图标编号
	public int iconID = 0;//奖励卡片对应的图标iD
	public int levelId = 0;//等级指向
	public int qualityId = 0;//品质编号
	public int maxLevel = 0;//最高等级
	public int starLevel = 0;//还可以进化星级
	public int evoStarLevel = 0;//已进化星级
	public int level = 0;//当前等级 
	public int baseLife = 0;//初始生命值
	public int baseAttack = 0;//初始攻击值
	public int baseDefecse = 0;//初始防御值
	public int baseMagic = 0;//初始魔力值
	public int baseAgile = 0;//初始敏捷值
    public int sFlagLevel=0;//卡片星级（紫色2级，橙色3级，红色4级）
    public int bloodPointSid;//节点sid
    public int qualityLevel = 0;

	public int developLife = 0;//生命成长值
	public int developAttack = 0;//攻击成长值
	public int developDefecse = 0;//防御成长值
	public int developMagic = 0;//魔力成长值
	public int developAgile = 0;//敏捷成长值
	
	public string deadWords = "";//死亡遗言
	public int effectId = 0;//角色特效id
	public string[] features ;//特性 召唤兽特性 数组长度为2 特性名称 特性描述
	public int cardType = 0;//1card 2 beast 3isBoss
	public int[] skillsNum;//[5,5,5] 开场buff  主动技能  被动技能 
	public int eatenExp = 0;//被吃经验
	public int sell = 0;//卖出价格
	public int job;//职业

	public int evolveSid = 0;//进化后卡片sid //更改为英雄之章类型
	public int evolveToolSid = 0;//进化万能卡sid
	public int evolvePrice = 0;//进化金额 只能是游戏币
	
	//默认技能配置
	public int[] buffSkills;
	public int[] mainSkills;
	public int[] attrSkills;
	
	//默认属性附加等级 血量 攻击 防御 魔法 敏捷
	public int[] attrLevel;
	PrizeSample[] resolveResults;
    public int[] sameCardSids;

	//暂时写这里
	public int count = 1;

	override public void parse (int sid, string str)
	{ 
		this.sid = sid; 
		string[] strArr = str.Split ('|');
		//checkLength (strArr.Length, 30);
		
		//strArr[0] is sid  
		//strArr[1] name
		this.name = strArr [1];
		//strArr[2] iconId
		this.imageID = StringKit.toInt (strArr [2]); 
		//strArr[2] iconId
		this.iconID = StringKit.toInt (strArr [3]); 

		//strArr[3] levelId
		this.levelId = StringKit.toInt (strArr [4]); 
		//strArr[4] qualityId
		this.qualityId = StringKit.toInt (strArr [5]); 
		//strArr[5] maxLevel
		this.maxLevel = StringKit.toInt (strArr [6]); 
		//strArr[6] starLevel
		this.starLevel = StringKit.toInt (strArr [7]); 

		//strArr[7] baseLife
		this.baseLife = StringKit.toInt (strArr [8]); 
		//strArr[8] baseAttack
		this.baseAttack = StringKit.toInt (strArr [9]); 
		//strArr[9] baseDefecse
		this.baseDefecse = StringKit.toInt (strArr [10]); 
		//strArr[10] baseMagic
		this.baseMagic = StringKit.toInt (strArr [11]); 
		//strArr[11] baseAgile
		this.baseAgile = StringKit.toInt (strArr [12]); 

		//strArr[12] developLife
		this.developLife = StringKit.toInt (strArr [13]); 
		//strArr[13] developAttack
		this.developAttack = StringKit.toInt (strArr [14]); 
		//strArr[14] developDefecse
		this.developDefecse = StringKit.toInt (strArr [15]); 
		//strArr[15] developMagic
		this.developMagic = StringKit.toInt (strArr [16]); 
		//strArr[16] developAgile
		this.developAgile = StringKit.toInt (strArr [17]);  
		
		//strArr[17] deadWords
		this.deadWords = strArr [18];  
		//strArr[18] effectId
		this.effectId = StringKit.toInt (strArr [19]);  
		//strArr[19] features
		parseFeatures (strArr [20]);		
		//strArr[20] cardType
		this.cardType = StringKit.toInt (strArr [21]);  
		//strArr[21] skillsNum
		parseSkillsNum (strArr [22]);
		//strArr[22] eatenExp
		this.eatenExp = StringKit.toInt (strArr [23]);  
		//strArr[23] sell
		this.sell = StringKit.toInt (strArr [24]);  
		//strArr[24] job
		this.job = StringKit.toInt (strArr [25]);  
		//strArr[25]  evolveSid
		this.evolveSid = StringKit.toInt (strArr [26]);

		parseSkills (strArr [27]);
	
		parseResolve (strArr [28]);
		parseAttrLevel (strArr [29]);

		this.evoStarLevel = StringKit.toInt (strArr [30]);
	    this.bloodPointSid = StringKit.toInt(strArr[31]);
        this.sFlagLevel = StringKit.toInt(strArr[32]);
	    if (strArr.Length > 33) parseSameSid(strArr[33]);
	}

    private void parseSameSid(string str)
    {
        string[] st = str.Split(',');
        sameCardSids=new int[st.Length];
        for (int i=0;i<st.Length;i++)
        {
            sameCardSids[i] = StringKit.toInt(st[i]);
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

	/// <summary>
	/// 使用前先确认是否需要克隆,克隆使用(getClonePrizeSample)
	/// </summary>
	/// <returns>The prize sample.</returns>
	public PrizeSample[] getPrizeSample() {
		return resolveResults;
	}
	/// <summary>
	/// 克隆过后的奖励模板
	/// </summary>
	/// <returns>The clone prize sample.</returns>
	public PrizeSample[] getClonePrizeSample() {
		if (resolveResults == null)
			return null;
		PrizeSample[] temp = new PrizeSample[resolveResults.Length];
		PrizeSample sample;
		for (int i = 0; i < temp.Length; i++) {
			sample = (PrizeSample)resolveResults [i].Clone ();
			temp[i] = sample;
		}
		return temp;
	}

	private void parseFeatures (string str)
	{
		string[] strArr = str.Split ('#');
		features = strArr; 
	}
	
	//解析进化信息
	private void parseEvolve (string str)
	{
		string[] strArr = str.Split (',');
		//strArr[0] evolve 
		this.evolveSid = StringKit.toInt (strArr [0]);  
		//strArr[1] evolveToolSid
		this.evolveToolSid = StringKit.toInt (strArr [1]);  
		//strArr[2] evolveToolSid
		this.evolvePrice = StringKit.toInt (strArr [2]);  
	}
	
	//解析技能数量
	private void parseSkillsNum (string str)
	{
		string[] strArr = str.Split (',');
		skillsNum = new int[strArr.Length];
		for (int i =0; i<strArr.Length; i++) {
			skillsNum [i] = StringKit.toInt (strArr [i]);			
		}
	}
	
	//解析附加属性等级
	private void parseAttrLevel (string str)
	{
		string[] strArr = str.Split (',');
		int max = strArr.Length;
		attrLevel = new int[max];
		for (int i = 0; i < max; i++) {
			attrLevel [i] = StringKit.toInt (strArr [i]);
		}
	}
	
	//解析默认技能配置
	private void parseSkills (string str)
	{
		string[] strArr = str.Split ('#');
		string[] skArr1 = strArr [0].Split (',');
		string[] skArr2 = strArr [1].Split (',');
		string[] skArr3 = strArr [2].Split (',');
		
		buffSkills = new int[skArr1.Length];
		for (int i = 0; i < skArr1.Length; i++) {
			if (skArr1 [i] == "0") {
				buffSkills = null;
				break;
			}
			buffSkills [i] = StringKit.toInt (skArr1 [i]);
		}
		
		mainSkills = new int[skArr2.Length];
		for (int i = 0; i < skArr2.Length; i++) {
			if (skArr2 [i] == "0") {
				mainSkills = null;
				break;
			}
			mainSkills [i] = StringKit.toInt (skArr2 [i]);
		}
		
		attrSkills = new int[skArr3.Length];
		for (int i = 0; i < skArr3.Length; i++) {
			if (skArr3 [i] == "0") {
				attrSkills = null;
				break;
			}
			attrSkills [i] = StringKit.toInt (skArr3 [i]);
		}
	}


	public override void copy (object destObj)
	{
		base.copy (destObj);
		CardSample dest = destObj as CardSample;
		if (this.features != null) {
			dest.features = new string[this.features.Length];
			for (int i = 0; i < dest.features.Length; i++)
				dest.features [i] = this.features [i];
		}
		if (this.skillsNum != null) {
			dest.skillsNum = new int[this.skillsNum.Length];
			for (int i = 0; i < dest.skillsNum.Length; i++)
				dest.skillsNum [i] = this.skillsNum [i];
		}
		if (this.buffSkills != null) {
			dest.buffSkills = new int[this.buffSkills.Length];
			for (int i = 0; i < dest.buffSkills.Length; i++)
				dest.buffSkills [i] = this.buffSkills [i];
		}
		if (this.mainSkills != null) {
			dest.mainSkills = new int[this.mainSkills.Length];
			for (int i = 0; i < dest.mainSkills.Length; i++)
				dest.mainSkills [i] = this.mainSkills [i];
		}
		if (this.attrSkills != null) {
			dest.attrSkills = new int[this.attrSkills.Length];
			for (int i = 0; i < dest.attrSkills.Length; i++)
				dest.attrSkills [i] = this.attrSkills [i];
		}
		if (this.attrLevel != null) {
			dest.attrLevel = new int[this.attrLevel.Length];
			for (int i = 0; i < dest.attrLevel.Length; i++)
				dest.attrLevel [i] = this.attrLevel [i];
		}
		if (this.resolveResults != null) {
			dest.resolveResults = new PrizeSample[this.resolveResults.Length];
			for (int i = 0; i < dest.resolveResults.Length; i++)
				dest.resolveResults [i] = this.resolveResults [i];
		}
	}
} 

