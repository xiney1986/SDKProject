using UnityEngine;
using System.Collections;

/*血脉节点模板*/
public class BloodPointSample : Sample {

    public int sid;//血脉节点sid
    public int[][] itemSid;//激活效果分段Sid 从绿色升级到蓝色开始
    public int maxLv = 0;//最高血脉等级
   // public int[] magicIds;//各阶段对应的卡片sid
    public BloodPointSample(string str) {
        parse(str);
    }
    public void parse(string str) {
        string[] strArr = str.Split('|');
        sid = StringKit.toInt(strArr[0]);
        maxLv = StringKit.toInt(strArr[1]);
        parseItemSid(strArr[2]);
       // parseMagicIds(strArr[3]);
    }

    //public void parseMagicIds(string str)
    //{
    //    string[] sids = str.Split('#');
    //    magicIds=new int[sids.Length];
    //    for (int i=0;i<sids.Length;i++)
    //    {
    //        magicIds[i] = StringKit.toInt(sids[i]);
    //    }
    //}
    private void parseItemSid(string str) {
        string[] strEffects = str.Split('#');
        itemSid=new int[strEffects.Length][];
        for (int i=0;i<strEffects.Length;i++)
        {
            string st = strEffects[i];
            string[] sts = st.Split(',');
            itemSid[i]=new int[sts.Length];
            for (int j=0;j<sts.Length;j++)
            {
                itemSid[i][j] = StringKit.toInt(sts[j]);
            }
        }
    }
    public override void copy(object destObj) {
        base.copy(destObj);
    }
}
