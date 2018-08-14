using System;
using System.Collections;
using System.Collections.Generic;
 
/**秘宝模板 
  *@author longlingquan
  **/
public class MagicWeaponSample:Sample
{
    public MagicWeaponSample()
	{
	}
	
	public string name = "";//秘宝名字
	public int iconId = 0;//图标编号
	public int maxLevel = 0;//最高强化等级
    public int maxphaseLv = 0;//最高阶位等级
	public int qualityId = 0;//品质编号
	
    //为了简化模板长度 没有的直接不填写
    public int attributSid;//这个保存SID
    public int[] skillSids;//附带技能
	public int sell;//卖出价格
    public AttrChangeSample[] effects = null;//对描述参数提供数值(影响描述信息数值)
	public string desc;//描述信息
	public PrizeSample[] resolveResults;//分解结果
    public int MagicWeaponType = 0;//默认的通用类型：1:力、2:魔、3:敏、4:毒、5:反
    public int[] baseSuccess;//
    public int[] needStrengLv;//锻造需求的强化等级
    public int lvType = 0;//区分神器的品质
    public int starLevel;//神器星级
	
	public override void parse (int sid, string str)
	{
		this.sid = sid;
		string[] strArr = str.Split ('|');
		//strArr[0] is sid  
		//strArr[1] name
		this.name = strArr [1];
		//strArr[2] iconId
		this.iconId = StringKit.toInt (strArr [2]);
        //strArr[3] maxLv
        this.maxLevel = StringKit.toInt(strArr[3]);
        this.maxphaseLv = StringKit.toInt(strArr[4]);
        this.qualityId = StringKit.toInt(strArr[5]);
        //开始解析模板属性
        this.attributSid = StringKit.toInt(strArr[6]);
		//strArr[6] skillSids 
		parseSkills (strArr [7]); 
		//strArr[7] sell
		this.sell = StringKit.toInt (strArr [8]);   
		//strArr[8] effects
		parseEffects (strArr [9]);
		//strArr[9] desc
		this.desc = strArr [10]; 
		//strArr[10] resolveResults
		parseResolve (strArr [11]);
        this.MagicWeaponType = StringKit.toInt(strArr[12]);
        parseSuccess(strArr[13]); // StringKit.toInt(strArr[13]);
        parseStrengLv(strArr[14]);
        this.lvType = StringKit.toInt(strArr[15]);
        if (strArr.Length > 16)
        this.starLevel = StringKit.toInt(strArr[16]);
	}
    private void parseSuccess(string str) {
        string[] st = str.Split(',');
        baseSuccess = new int[st.Length];
        for (int i = 0; i < st.Length;i++ ) {
            baseSuccess[i] = StringKit.toInt(st[i]);
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
    private void parseStrengLv(string str) {
        string[] st = str.Split(',');
        needStrengLv = new int[st.Length];
        for (int i = 0; i < st.Length;i++ ) {
            needStrengLv[i] = StringKit.toInt(st[i]);
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
        string[] strArr = str.Split(',');
        skillSids = new int[strArr.Length];
        for (int i = 0; i < strArr.Length; i++) {
            skillSids[i] = StringKit.toInt(strArr[i]);
        }
    }
	

	public override void copy (object destObj)
	{
		base.copy (destObj);
        MagicWeaponSample dest = destObj as MagicWeaponSample;
        if (this.skillSids != null) {
            dest.skillSids = new int[this.skillSids.Length];
            for (int i = 0; i < this.skillSids.Length; i++)
                dest.skillSids[i] = this.skillSids[i];
        }
		if (this.effects != null) {
			dest.effects = new AttrChangeSample[this.effects.Length];
			for (int i = 0; i < this.effects.Length; i++)
				dest.effects [i] = this.effects [i].Clone () as AttrChangeSample;
		}
        if (this.resolveResults != null) {
            dest.resolveResults = new PrizeSample[this.resolveResults.Length];
            for (int i = 0; i < this.resolveResults.Length; i++)
                dest.resolveResults[i] = this.resolveResults[i];
        }
	}
} 

