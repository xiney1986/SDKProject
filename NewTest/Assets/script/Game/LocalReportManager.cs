using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//没用了？
public class LocalReportManager 
{
    public static LocalReportManager instance = new LocalReportManager();

    public ErlKVMessage CreatePoint1()
    {
        ErlKVMessage msg = new ErlKVMessage("msg");
        List<ErlType> list = new List<ErlType>();

        //队伍
        List<ErlType> teamList = new List<ErlType>();

        teamList.Add(createTeamInfo(2,16007,59,"1",46154,46154,"npc",15));
        teamList.Add(createTeamInfo(2,16006,56,"1",46154,46154,"npc",14));
        teamList.Add(createTeamInfo(2,16005,53,"1",46154,46154,"npc",13));
        teamList.Add(createTeamInfo(2,16004,50,"1",46154,46154,"npc",12));
        teamList.Add(createTeamInfo(2,16003,47,"1",46154,46154,"npc",11));

        
        teamList.Add(createTeamInfo(2,16012,44,"1",46154,46154,"npc",5));
        teamList.Add(createTeamInfo(2,16011,41,"1",46154,46154,"npc",4));
        teamList.Add(createTeamInfo(2,16010,38,"1",46154,46154,"npc",3));
        teamList.Add(createTeamInfo(2,16009,35,"1",46154,46154,"npc",2));
        teamList.Add(createTeamInfo(2,16008,32,"1",46154,46154,"npc",1));
        
        teamList.Add(createTeamInfo(1,16008,29,"281479271701964",46154,46154,"武凯复",15));
        teamList.Add(createTeamInfo(1,16019,26,"281479271701964",46632,46632,"武凯复",14));
        teamList.Add(createTeamInfo(1,2,23,"281479271701964",46632,46632,"武凯复",13));
        teamList.Add(createTeamInfo(1,16018,20,"281479271701964",46632,46632,"武凯复",12));
        teamList.Add(createTeamInfo(1,16012,17,"281479271701964",46632,46632,"武凯复",11));
        
        teamList.Add(createTeamInfo(1,16016,14,"281479271701964",46154,46154,"武凯复",5));
        teamList.Add(createTeamInfo(1,16017,11,"281479271701964",46632,46632,"武凯复",4));
        teamList.Add(createTeamInfo(1,16015,7,"281479271701964",46632,46632,"武凯复",3));
        teamList.Add(createTeamInfo(1,16013,4,"281479271701964",64103,64103,"武凯复",2));
        teamList.Add(createTeamInfo(1,16014,1,"281479271701964",46632,46632,"武凯复",1));

        ErlList arrTeam = new ErlList(teamList.ToArray());
        Array.Reverse(arrTeam.Value);
        list.Add(arrTeam);

        //开场buff
        list.Add(new ErlNullList());
        //回合
        list.Add(new ErlNullList());
        list.Add(new ErlNullList());

        List<ErlList> rihgtList = new List<ErlList>();

        List<ErlType> rounds = new List<ErlType>();
        /**
        rounds.Add(createAbiltyAttack(53,29544,54,new int[]{23}));
        rounds.Add(createAttrChange(29,1,-2769));
        rounds.Add(createBufferAdd(38,27018,70,new object[,]{{"deduck",0}},29));
        rounds.Add(createAttrChange(26,1,-2798));
        rounds.Add(createBufferAdd(38,27018,69,new object[,]{{"deduck",0}},29));
        rounds.Add(createAttrChange(26,1,-10003));
        rounds.Add(createAttrChange(20,1,-2798));
        rounds.Add(createBufferAdd(38,27018,68,new object[,]{{"deduck",0}},20));
        rounds.Add(createAttrChange(20,1,-10003));
        rounds.Add(createAttrChange(17,1,-2769));
        rounds.Add(createBufferAdd(38,27018,67,new object[,]{{"deduck",0}},17));
        rounds.Add(createAttrChange(17,1,-10939));
        rounds.Add(createAttrChange(14,1,-2798));
        rounds.Add(createBufferAdd(38,27018,66,new object[,]{{"deduck",0}},14));
        rounds.Add(createAttrChange(14,1,-10003));
        rounds.Add(createAttrChange(7,1,-3846));
        rounds.Add(createBufferAdd(38,27018,65,new object[,]{{"deduck",0}},7));
        rounds.Add(createAttrChange(7,1,-4956));
        */
        rounds.Add(createAbiltyAttack(38,29547,39,new int[]{7,14,17,20,23,26,29}));
        rounds.Add(createAttrChange(29,1,56723));
        rounds.Add(createAttrChange(26,1,56723));
        rounds.Add(createAttrChange(20,1,56723));
        rounds.Add(createAttrChange(17,1,56723));
        rounds.Add(createAttrChange(14,1,56723));
        rounds.Add(createAttrChange(7,1,56723));
        rounds.Add(createAttrChange(35,1,-16569));
        rounds.Add(createAbiltyAttack(17,29545,18,new int[]{35}));
        rounds.Add(createAttrChange(29,1,56723));
        rounds.Add(createAttrChange(26,1,56723));
        rounds.Add(createAttrChange(20,1,56723));
        rounds.Add(createAttrChange(17,1,56723));
        rounds.Add(createAttrChange(14,1,56723));
        rounds.Add(createAttrChange(7,1,56723));
        rounds.Add(createAttrChange(41,1,-16569));
        rounds.Add(createAbiltyAttack(29,29545,30,new int[]{41}));
        createAttrChange(rounds,"5:[59,1,-21046]},{5:[56,1,-19154]},{5:[53,1,-18296]},{5:[50,1,-19154]},{5:[47,1,-21046]},{5:[44,1,-21046]},{5:[41,1,-19154]},{5:[38,1,-19337]},{5:[35,1,-19154]},{5:[32,1,-21046]}");
        rounds.Add(createAbiltyAttack(7,29550,8,new int[]{32,35,38,41,44,47,50,53,56,59}));
        createAttrChange(rounds, "5:[29,1,-10846]},{5:[26,1,-9910]},{5:[20,1,-9910]},{5:[17,1,-10846]},{5:[14,1,-9910]},{5:[7,1,-4863]");
        rounds.Add(createBufferRemove(4, 28018, 63));
        rounds.Add(createAttrChange(4,1,-8954));
        rounds.Add(createAbiltyAttack(59,29542,60,new int[]{4,7,14,17,20,23,26,29}));
        createAttrChange(rounds, "5:[29,1,-10846]},{5:[26,1,-9910]},{5:[20,1,-9910]},{5:[17,1,-10846]},{5:[14,1,-9910]");
        rounds.Add(createBufferRemove(11, 28018, 64));
        createAttrChange(rounds, "5:[11,1,-8954]},{5:[7,1,-4863]},{5:[4,1,-8954]");
        rounds.Add(createAbiltyAttack(47,29542,48,new int[]{4,7,11,14,17,20,23,26,29}));
        createAttrChange(rounds, "5:[7,1,-5334]");
        rounds.Add(createDoubleAttack(56, 29543, 57, 7));
        createAttrChange(rounds, "5:[1,1,-10381");
        rounds.Add(createDoubleAttack(56, 29543, 57, 1));
        createAttrChange(rounds, "5:[11,1,-9425");
        rounds.Add(createDoubleAttack(56, 29543, 57, 11));
        createAttrChange(rounds, "5:[7,1,-5334");
        rounds.Add(createDoubleAttack(56, 29543, 57, 7));
        createAttrChange(rounds, "5:[1,1,-10381");
        rounds.Add(createDoubleAttack(56, 29543, 57, 1));
        rounds.Add(createAbiltyAttack(56,29543,57,new int[]{1}));
        createAttrChange(rounds, "5:[1,1,-10381");
        rounds.Add(createDoubleAttack(50, 29543, 51, 1));
        createAttrChange(rounds, "5:[1,1,-10381");
        rounds.Add(createDoubleAttack(50, 29543, 51, 1));
        createAttrChange(rounds, "5:[1,1,-10381");
        rounds.Add(createDoubleAttack(50, 29543, 51, 1));
        createAttrChange(rounds, "5:[4,1,-9425");
        rounds.Add(createDoubleAttack(50, 29543, 51, 4));
        createAttrChange(rounds, "5:[11,1,-9425");
        rounds.Add(createDoubleAttack(50, 29543, 51, 11));
        rounds.Add(createAbiltyAttack(50,29543,51,new int[]{11}));
        createAttrChange(rounds, "{5:[47,1,-9913]},{5:[50,1,-8021]},{5:[53,1,-7163]},{5:[56,1,-8021]},{5:[59,1,-9913]");
        rounds.Add(createAbiltyAttack(26,29552,27,new int[]{59,56,53,50,47}));
        createAttrChange(rounds, "5:[32,1,-9913]},{5:[35,1,-8021]},{5:[38,1,-8204]},{5:[41,1,-8021]},{5:[44,1,-9913]");
        rounds.Add(createAbiltyAttack(20,29551,21,new int[]{44,41,38,35,32}));
        rounds.Add(createBufferAdd(41,28018,64,new object[,]{{"magic",-2600}},11));
        createAttrChange(rounds, "5:[11,1,-27488");
        rounds.Add(createAbiltyAttack(41,29546,42,new int[]{11}));
        rounds.Add(createBufferAdd(35,28018,63,new object[,]{{"magic",-2600}},11));
        createAttrChange(rounds, "5:[4,1,-27488]");
        rounds.Add(createAbiltyAttack(35,29546,36,new int[]{4}));
        createAttrChange(rounds, "{5:[59,1,-9913]},{5:[56,1,-8021]},{5:[53,1,-7163]},{5:[50,1,-8021]},{5:[47,1,-9913]},{5:[44,1,-9913]},{5:[41,1,-8021]},{5:[38,1,-8204]},{5:[35,1,-8021]},{5:[32,1,-9913]");
        rounds.Add(createAbiltyAttack(14,29549,15,new int[]{32,35,38,41,44,47,50,53,56,59}));
        createAttrChange(rounds, "5:[59,1,-9913]},{5:[56,1,-8021]},{5:[53,1,-7163]},{5:[50,1,-8021]},{5:[47,1,-9913]},{5:[44,1,-9913]},{5:[41,1,-8021]},{5:[38,1,-8204]},{5:[35,1,-8021]},{5:[32,1,-9913]");
        rounds.Add(createAbiltyAttack(1,29549,2,new int[]{32,35,38,41,44,47,50,53,56,59}));

        ErlList roundsErlList = new ErlList(rounds.ToArray());
        Array.Reverse(roundsErlList.Value);
        rihgtList.Add(roundsErlList);

        ErlList rightErlList = new ErlList(rihgtList.ToArray());
        Array.Reverse(rightErlList.Value);
        list.Add(rightErlList);


        //win
        list.Add(createWinner(1));

        //return
        ErlList resultList = new ErlList(list.ToArray());
        Array.Reverse(resultList.Value);
        msg.addValue("report",resultList);
        return msg;
    }

    private ErlArray createTeamInfo(int camp,int sid,int id,string uid,int hp,int maxhp,string master,int embattle)
    {
        List<ErlType> list = new List<ErlType>();
        list.Add(new ErlInt(camp));
        list.Add(new ErlInt(sid));
        list.Add(new ErlInt(id));
        list.Add(new ErlString(uid));
        list.Add(new ErlInt(hp));
        list.Add(new ErlInt(maxhp));
        list.Add(new ErlString(master));
        list.Add(new ErlInt(embattle));

        ErlType t = new ErlInt(4);
        ErlArray arr = new ErlArray(list.ToArray());
        return new ErlArray(new ErlType[]{t,arr});
    }

    private ErlList createOpenBuff()
    {
        return null;
    }
    //构建攻击行为
    private ErlArray createAbiltyAttack(int userID,int skillSID,int skillID,int[] targets)
    {
        List<ErlType> list = new List<ErlType>();
        list.Add(new ErlInt(userID));
        list.Add(new ErlInt(skillSID));
        list.Add(new ErlInt(skillID));

        List<ErlType> list2 = new List<ErlType>();
        for (int i = 0; i < targets.Length; i++)
        {
            list2.Add(new ErlInt(targets[i]));
        }
        list.Add(new ErlArray(list2.ToArray()));
        
        return Round("2",new ErlArray(list.ToArray()));
    }

    //构建援护
    private ErlArray createIntervene(int userID,int skillSID,int skillID,int target,int trigger)
    {
        List<ErlType> list = new List<ErlType>();
        list.Add(new ErlInt(userID));
        list.Add(new ErlInt(skillSID));
        list.Add(new ErlInt(skillID));
        list.Add(new ErlInt(target));
        list.Add(new ErlInt(trigger));
        
        return Round("8",new ErlArray(list.ToArray()));
    }

    //构建改变属性
    private ErlArray createAttrChange(object str1,object str2,object str3)
    {
        List<ErlType> list = new List<ErlType>();
        list.Add(new ErlString(str1.ToString()));
        list.Add(new ErlString(str2.ToString()));
        list.Add(new ErlString(str3.ToString()));
        
        return Round("5",new ErlArray(list.ToArray()));
    }

    //构建反击
    private ErlArray createRebound(int a1,int a2,int a3,int a4)
    {
        List<ErlType> list = new List<ErlType>();
        list.Add(new ErlInt(a1));
        list.Add(new ErlInt(a2));
        list.Add(new ErlInt(a3));
        list.Add(new ErlInt(a4));
        
        return Round("9",new ErlArray(list.ToArray()));
    }
    //构建胜利者
    private ErlList createWinner(int a)
    {
        return new ErlList(new ErlType[]{new ErlArray(new ErlType[]{new ErlInt(1),new ErlInt(a)})});

    }
    //构建急救
    private ErlArray createFirstaid(int a1,int a2,int a3,int a4)
    {
        List<ErlType> list = new List<ErlType>();
        list.Add(new ErlInt(a1));
        list.Add(new ErlInt(a2));
        list.Add(new ErlInt(a3));
        list.Add(new ErlInt(a4));
        
        return Round("10",new ErlArray(list.ToArray()));
    }

    //构建合击参与者
    private ErlArray createParticipant(int[] a)
    {
        ErlType[] ts = new ErlType[a.Length];
        for (int i = 0; i < ts.Length; i++)
        {
            ts[i] = new ErlInt(a[i]);
        }
        return Round("13",new ErlArray(ts));
    }

    //构建合击 TOGERTHER_ATTACK
    private ErlArray createTogertherAttack(int a1, int a2)
    {
        List<ErlType> list = new List<ErlType>();
        list.Add(new ErlInt(a1));
        list.Add(new ErlInt(a2));
        
        return Round("12",new ErlArray(list.ToArray()));
    }
    //构建连击
    private ErlArray createDoubleAttack(int a1,int a2,int a3,int a4)
    {
        List<ErlType> list = new List<ErlType>();
        list.Add(new ErlInt(a1));
        list.Add(new ErlInt(a2));
        list.Add(new ErlInt(a3));
        list.Add(new ErlInt(a4));
        
        return Round("11",new ErlArray(list.ToArray()));
    }

    //构建添加buffer
    private ErlArray createBufferAdd(int a1,int a2,int a3,object[,] a4,int a5)
    {
        List<ErlType> list = new List<ErlType>();
        list.Add(new ErlInt(a1));
        list.Add(new ErlInt(a2));
        list.Add(new ErlInt(a3));

        if (a4 == null || a4.Length == 0)
            list.Add(new ErlNullList());
        else
        {
            ErlType[] ts = new ErlType[a4.GetLength(0)];
            for(int i = 0; i < a4.GetLength(0); i++)
            {
                ts[i] = new ErlArray(new ErlType[]{new ErlAtom(a4[i,0].ToString()),new ErlString(a4[i,1].ToString())});
            }
            list.Add(new ErlList(ts));
        }

        list.Add(new ErlInt(a5));

        return Round("6",new ErlArray(list.ToArray()));
    }

    //构建移除buffer
    private ErlArray createBufferRemove(int a1,int a2,int a3)
    {
        List<ErlType> list = new List<ErlType>();
        list.Add(new ErlInt(a1));
        list.Add(new ErlInt(a2));
        list.Add(new ErlInt(a3));

        return Round("14",new ErlArray(list.ToArray()));
    }
    //替换buffer
    private ErlArray createBufferReplace(int a1,int a2,int a3,int a4,int a5)
    {
        List<ErlType> list = new List<ErlType>();
        list.Add(new ErlInt(a1));
        list.Add(new ErlInt(a2));
        list.Add(new ErlInt(a3));
        list.Add(new ErlInt(a4));
        list.Add(new ErlInt(a5));
        
        return new ErlArray(list.ToArray());
        return Round("7",new ErlArray(list.ToArray()));
    }
    //buffer生效 BUFFER_ABILITY
    private ErlArray createBufferAbility(int a1,int a2,int a3)
    {
        List<ErlType> list = new List<ErlType>();
        list.Add(new ErlInt(a1));
        list.Add(new ErlInt(a2));
        list.Add(new ErlInt(a3));

        return Round("15",new ErlArray(list.ToArray()));
    }


    //npc出场
    private ErlArray createAddNPC()
    { 
        return Round("16",null);
    }
    
    //npc退场
    private ErlArray createDelNPC()
    {
        return Round("17",null);
    }
    
    //对话
    private ErlArray createTalk(int id)
    {
        return Round("18",new ErlInt(id));
    }
    //特效退出战斗
    private ErlArray createEffectExit(int id)
    {
        return Round("19",new ErlInt(id));
    }

    private ErlArray addCard(int a1,int a2,int a3,string a4,int a5,int a6,string a7,int a8)
    {
        List<ErlType> list = new List<ErlType>();
        list.Add(new ErlInt(a1));
        list.Add(new ErlInt(a2));
        list.Add(new ErlInt(a3));
        list.Add(new ErlString(a4));
        list.Add(new ErlInt(a5));
        list.Add(new ErlInt(a6));
        list.Add(new ErlString(a7));
        list.Add(new ErlInt(a8));

        return Round("4",new ErlArray(list.ToArray()));
    }

    private ErlArray Round(string command,ErlType type)
    {
        List<ErlType> list = new List<ErlType>();
        list.Add(new ErlString(command));
        if (type != null)
            list.Add(type);
        return new ErlArray(list.ToArray());
    }

    private void createAttrChange(List<ErlType> rounds,string str)
    {
        string[] ss = str.Split(new string[]{"},{"}, StringSplitOptions.RemoveEmptyEntries);
        for(int i = 0; i < ss.Length; i++)
        {
            string s = ss[i];
            int si = s.StartsWith("{") ? 4 : 3;
            s = s.Substring(si);
            s = s.Replace("{","");
            s = s.Replace("}","");
            s = s.Replace("[","");
            s = s.Replace("]","");

            string[] ns = s.Split(',');

            rounds.Add(createAttrChange(ns[0],ns[1],ns[2]));
        }
    }
}
