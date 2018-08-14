using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LimitCollectSample: Sample {
    /** sid */
    public int sid;
    /** 标题 */
    public string title;
    /** 需要的数量 */
    public int needNum;
    /** 条件 */
    public List<LimitCollectCondition> conditions;
    /** 奖励 */
    public PrizeSample prize;
    /** 是否领取过 */
    public bool isReceived;

    public override void parse(int sid, string str)
    {
        string[] strs = str.Split('|');
        this.sid = sid;
        title = strs[1];
        parseConditions(strs[2]);
        prize = parsePrize(strs[3]);

    }

    /** 解析条件  */
    private void parseConditions(string str) {
        conditions = new List<LimitCollectCondition>();
        string [] strs = str.Split('#');
        foreach (string s in strs) {
            conditions.Add(parseCondition(s));
        }
    }

    /** 解析条件 */
    LimitCollectCondition parseCondition(string str)
    {
        string[] strs = str.Split(',');
        LimitCollectCondition condition = new LimitCollectCondition();
        PrizeSample sample = new PrizeSample();
        sample.type = StringKit.toInt(strs[0]);
        sample.pSid = StringKit.toInt(strs[1]);
        sample.num = "0";
        condition.prize = sample;
        condition.need = StringKit.toInt(strs[2]);
        return condition;
    }

    /** 解析奖励 */
    PrizeSample parsePrize(string str)
    {
        string[] strs = str.Split(',');
        PrizeSample sample = new PrizeSample();
        sample.type = StringKit.toInt(strs[0]);
        sample.pSid = StringKit.toInt(strs[1]);
        sample.num = strs[2];
        return sample;
    }

    /// <summary>
    /// 设置物品收集数量
    /// </summary>
    /// <param name="sid">收集到的物品SID </param>
    /// <param name="num">收集到的物品数量</param>
    public void setCollected(int sid,int num) {
        foreach (LimitCollectCondition condition in conditions) {
            if (condition.prize.pSid == sid) {
                condition.collected = num;
                break;
            }
        }
    }
    public void clearCollected() {
        foreach (LimitCollectCondition condition in conditions) {
                condition.collected = 0;
            
        }
    }


    public bool isCanReceive() {
        if (isReceived)
        {
            return false;
        }
        else {
            foreach (LimitCollectCondition condition in conditions) {
                if (condition.collected < condition.need)
                    return false;
            }
            return true;
        }
    }
}

