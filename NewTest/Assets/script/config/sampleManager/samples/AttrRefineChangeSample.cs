using System;
/// <summary>
/// 精炼属性改变模板
/// </summary>
public class AttrRefineChangeSample : CloneObject
{
    //属性类型
    private string type = "";
    //属性值
    private int init = 0;
    //战斗力
    private int combat = 0;
    //类型,基础百分比,基础绝对值,成长百分比,成长绝对值
    public void parse(string str)
    {
        string[] strArr = str.Split(',');
        if (strArr.Length < 2)
            throw new Exception("skill effect error! str" + str);
        if (strArr.Length == 3)
        {
            this.type = strArr[0];
            this.init = StringKit.toInt(strArr[1]);
            this.combat = StringKit.toInt(strArr[2]);
        }
    }
    /// <summary>
    /// 针对精炼
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public int getAttrRefineValue(int level)
    {
        return init;
    }
    /// <summary>
    /// 获得战斗力
    /// </summary>
    /// <returns></returns>
    public int getAttrRefineCombatValue()
    {
        return this.combat;
    }
    //获得影响类型
    public string getAttrType()
    {
        return this.type;
    }

    public override void copy(object destObj)
    {
        base.copy(destObj);
        AttrRefineChangeSample dest = destObj as AttrRefineChangeSample;
    }
}