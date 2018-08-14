using UnityEngine;
using System.Collections;

/// <summary>
/// 秘宝属性对应模板
/// </summary>
public class MagicWeaponAttributSample : Sample {

	public MagicWeaponAttributSample(){
	}
    public MagicWeaponAttributSample(string str) {

	}
	/**filed */
	/**模板sid */
	public int sampleSid;
    /**属性的类型 */
    public string arrType="";//这个字段不用 只是为了配置的时候便与理解
    //具体强化等级对应的值
    public string[] arrts;


	/**method*/
	public override void parse (int sid, string str)
	{
		string[] strArr = str.Split ('|');
        this.sid = sid;
        parseArr(strArr[1]);
	}
    private void parseArr(string str) {
        string[] st = str.Split('#');
        arrts=new string[st.Length];
        for(int i=0;i<st.Length;i++){
            arrts[i]=st[i];
        }

    }
    public int getAttributeByStrengLv(string attr,int lv) {//根据秘宝的强化等级拿到对应数据的具体数值
        string[] att = attr.Split(',');
        return StringKit.toInt(att.Length <= lv + 1 ? att[att.Length-1] : att[lv + 1]);
    }
}
