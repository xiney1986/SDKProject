using System;
using System.Collections.Generic;
using quicksdk;
using UnityEngine;

/**
 * 用户
 * @author zhanghaishan
 * */
public class User : SampleUser
{
    /** 战斗播放速度倍数 */
    public static float[] BATTLE_PLAY_MULTIPLE = new float[] { 1.0f, 1.5f, 2.0f };
    /** 策划要求的速度倍数显示与真实速度不相同 */
    public static int[] SHOW_BATTLE_PLAY_MULTIPLE = new int[] { 1, 2, 3 };
    public string uid = "";//uid

    private int money = 0;//持有游戏币
    private int rmb = 0;//持有rmb
    public string mainCardUid = "";//主角卡id

    public int merit; //功勋
    public int prestige;//声望
    public int maxLevel = 0; //最高等级
                             /** 行军值 */
    public int guildFightPower = 0;
    /** 行军值的最大值 */
    public int guildFightPowerMax = 0;

    private int pve = 0;//Pve 行动力
    private int pve_max = 0;//Pve 行动力满值
    private long PveFirstTime = 0;//后台提供的第一次回复1点的时间位置
    private long PvpFirstTime = 0;//后台提供的第一次回复1点的时间位置
    private long ChvFirstTime = 0;//后台提供的第一次回复1点的时间位置
    private long StorePveFirstTime = 0;//后台提供的第一次回复1点的时间位置

    /** 存储行动力 */
    private int storePve = 0;
    /** 存储行动力满值 */
    private int storePveMax;

    private int pvp = 0;//Pvp 行动力
    private int pvp_max = 0;//Pvp 行动力满值

    private int chv = 0;//英雄之章次数
    private int chv_max = 0;//英雄之章次数满值

    public int honorLevel = 1;//荣誉等级
    public int honor = 0;//荣誉

    public int winNum = 0;//Pvp当前连胜次数
    public int winNumDay = 0;//Pvp本日最高连胜次数
    public int winRankDay = 0;//Pvp连胜次数排名

    public string guildId = "0";//公会id
    public string guildName = "";//公会名称
    public int firendsNum = 0;//好友上限

    public int titleId = 0;//当前称号
    public int arenaScore = 0;//竞技场积分
    public int activeScore = 0;//活动积分

    public int starSum = 0;//星星总数
    public int battleGetStarNum = 0;//本次战斗后得到的星星数
    public int lastStarSum = 0;//演播用的星星,非实际星星数.

    //public int lastFubenSid = 0;//最后一次副本章节挑战id 不一定是最新的副本进度

    private Timer timer;//时间器 用于倒计时
    int[] icons = new int[] { 404, 405, 401, 408, 407, 402 };
    public int star;//我的星座
    public int divineFortune; //占卜运势
    public bool canDivine; //是否可以占卜
    public int practiceHightPoint; //修炼副本的最高记录
                                   /** 战斗播放速度倍数-0=1倍,1=1.5被,2=2倍 */
    int battlePlayVelocity;
    /** 起始讨伐倒计时时间 */
    private int startWarCDTime = 0;
    /** 是否可以倒计时 */
    private bool isWarCDTime = false;


    public User()
    {

    }
    /// <summary>
    /// 增加运势
    /// </summary>
    public void addDivineFortune(int num)
    {
        divineFortune += num;
    }

    public bool isUpdateStar()
    {
        if (starSum == 0 || starSum != lastStarSum)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void initStarNum(int num)
    {
        starSum = num;
        lastStarSum = num;
    }

    public void updateStarSum(int num)
    {
        battleGetStarNum = num - starSum;
        starSum = num;
    }

    public void addStar(int num)
    {
        lastStarSum += num;
    }

    public void setLastStarSum(int num)
    {
        lastStarSum = num;
    }
    public int getlastStarSum()
    {
        return lastStarSum;
    }

    public bool addBattlePlayVelocity(int maxUpLimit)
    {
        int velocity = this.battlePlayVelocity;
        velocity++;
        bool isAdd = true;
        if (velocity > maxUpLimit)
        {
            isAdd = false;
            velocity = 0;
        }
        setBattlePlayVelocity(velocity);
        return isAdd;
    }

    public void setBattlePlayVelocity(int battlePlayVelocity)
    {
        this.battlePlayVelocity = battlePlayVelocity;
        if (this.battlePlayVelocity > BATTLE_PLAY_MULTIPLE.Length - 1)
            this.battlePlayVelocity = BATTLE_PLAY_MULTIPLE.Length - 1;
        PlayerPrefs.SetInt(uid + PlayerPrefsComm.BATTLE_PLAY_VELOCITY, this.battlePlayVelocity);
    }

    public float getBattlePlayVelocity()
    {
        return BATTLE_PLAY_MULTIPLE[battlePlayVelocity];
    }

    public int getShowBattlePlayVelocity()
    {
        return SHOW_BATTLE_PLAY_MULTIPLE[battlePlayVelocity];
    }

    public void resetCurrentStarNum()
    {
        battleGetStarNum = 0;
    }
    //副本战斗后获得的星星数
    public int getBattleStarNum()
    {
        return battleGetStarNum;

    }

    public int getActiveScore()
    {
        return this.activeScore;
    }

    public int getMoney()
    {
        return this.money;
    }

    public int getRMB()
    {
        return this.rmb;
    }

    public int getMerit()
    {
        return this.merit;
    }

    //获得pvp点数
    public int getPvPPoint()
    {
        return pvp;
    }

    public bool isPvpMax()
    {
        if (pvp >= pvp_max)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /** 公会战行军值是否已满 */
    public bool isGuildFightPowerMax()
    {
        return guildFightPower >= guildFightPowerMax ? true : false;
    }


    //获得chv点数
    public int getChvPoint()
    {
        updateChv();
        return chv;
    }

    public bool isChvMax()
    {
        if (chv >= chv_max)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool isPveMax()
    {
        if (pve >= pve_max)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 坐骑行动力是否满
    /// </summary>
    public bool isStorePveMax()
    {
        if (storePve >= storePveMax)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// 获得总pve点数(pve+存储pve)
    /// </summary>
    public int getTotalPvEPoint()
    {
        return getPvEPoint() + getStorePvEPoint();
    }
    //获得pve点数
    public int getPvEPoint()
    {
        updatePve();
        return pve;
    }

    /// <summary>
    /// 获得坐骑pve点数
    /// </summary>
    public int getStorePvEPoint()
    {
        updateStorePve();
        return storePve;
    }

    //设置体力
    public void setPvPPoint(int pvp)
    {

        updatePvp();
        this.pvp = pvp;
    }
    //添加体力(返回真实添加的值)
    public int addPvPPoint(int pvp)
    {
        int addpvp = pvp;
        this.pvp += pvp;
        if (this.pvp > pvp_max)
        {
            addpvp = pvp - (this.pvp - pvp_max);
            this.pvp = pvp_max;
        }
        if (addpvp < 0) addpvp = 0;
        return addpvp;
    }
    //设置chv
    public void setChvPoint(int chv)
    {
        this.chv = chv;
    }
    //添加chv(返回真实添加的值)
    public int addChvPoint(int chv)
    {
        int addchv = chv;
        this.chv += chv;
        if (this.chv > chv_max)
        {
            addchv = chv - (this.chv - chv_max);
            this.chv = chv_max;
        }
        if (addchv < 0) addchv = 0;
        return addchv;
    }

    /// <summary>
    /// 添加存储行动力(返回真实添加的值)
    /// </summary>
    public int addStorePvEPoint(int pve)
    {
        int addpve = pve;
        this.storePve += pve;
        if (this.storePve > storePveMax)
        {
            addpve = pve - (this.storePve - storePveMax);
            this.storePve = storePveMax;
        }
        if (addpve < 0) addpve = 0;
        return addpve;
    }

    /// <summary>
    /// 添加行动力(返回真实添加的值)--特别注意,是否需要添加存储行动力
    /// </summary>
    /// <param name="pve">Pve.</param>
    public int _addPvEPoint(int pve)
    {
        int addpve = pve;
        this.pve += pve;
        if (this.pve > pve_max)
        {
            addpve = pve - (this.pve - pve_max);
            this.pve = pve_max;
        }
        if (addpve < 0) addpve = 0;
        //		GameManager.Instance.pushNotice ();
        return addpve;
    }
    /// <summary>
    /// 添加行动力(返回真实添加的值)--特别注意,是否需要添加存储行动力
    /// </summary>
    /// <param name="pve">Pve.</param>
    public int addPvEPoint(int pve)
    {
        int totalPveMax = CommonConfigSampleManager.Instance.getSampleBySid<PvePowerMaxSample>(CommonConfigSampleManager.PvePowerMax_SID).pvePowerMax;//总的存储行动力上限，默认为600
        int addpve = pve;
        this.pve += pve;
        if (this.pve > totalPveMax)//pve_max)
        {
            addpve = pve - (this.pve - totalPveMax); //pve_max);
            this.pve = totalPveMax;//pve_max;
        }
        if (addpve < 0) addpve = 0;
        //		GameManager.Instance.pushNotice ();
        return addpve;
    }
    //设置行动力
    public void setPvEPoint(int pve)
    {
        this.pve = pve;
        //		GameManager.Instance.pushNotice ();
    }
    /// <summary>
    /// 添加行动力,如果行动力满，则继续添加存储行动力
    /// </summary>
    /// <param name="pve">Pve.</param>
    public int addTotalPvEPoint(int _pve)
    {
        if (_pve <= 0)
            return 0;
        int addValue = 0;
        // pvp差多少满
        //int reNum=pve_max-this.pve;
        int reNum = CommandConfigManager.Instance.maxPvePoint - this.pve;
        // 额外多出的pvp存储值
        int addStorePve = _pve - reNum;
        if (addStorePve > 0)
        {
            int addPvp = addPvEPoint(reNum);
            int addStorePvp = addStorePvEPoint(addStorePve);
            addValue = addPvp + addStorePvp;
        }
        else
        {
            int addPvp = addPvEPoint(_pve);
            addValue = addPvp;
        }
        return addValue;
    }
    /** Pvp 行动力是否满 */
    public bool isPvpFull()
    {
        if (this.pvp >= this.pvp_max)
            return true;
        return false;
    }

    /** 英雄之章次数是否满 */
    public bool isChvFull()
    {
        if (this.chv >= this.chv_max)
            return true;
        return false;
    }

    /** Pve 行动力是否满 */
    public bool isPveFull()
    {
        if (this.pve >= this.pve_max)
            return true;
        return false;
    }

    /** 存储Pve 行动力是否满 */
    public bool isStorePveFull()
    {
        if (this.storePve >= this.storePveMax)
            return true;
        return false;
    }

    public void expendPvPPoint(int pvp)
    {
        this.pvp -= pvp;
    }

    public void expendChvPoint(int chv)
    {
        this.chv -= chv;
    }
    /// <summary>
    /// 设置坐骑行动力
    /// </summary>
    public void setStorePvEPoint(int pve)
    {
        this.storePve = pve;
    }

    //获得体力最大值
    public int getPvPPointMax()
    {
        return this.pvp_max;
    }

    //获得chv最大值
    public int getChvPointMax()
    {
        return this.chv_max;
    }

    //获得行动力最大值
    public int getPvEPointMax()
    {
        return this.pve_max;
    }

    //设置体力最大值
    public void setPvPPointMax(int pvp_m)
    {
        this.pvp_max = pvp_m;
    }

    //设置体力最大值
    public void setChvPointMax(int chv_m)
    {
        this.chv_max = chv_m;
    }

    /// <summary>
    /// 获得坐骑行动力最大值
    /// </summary>
    public int getStorePvEPointMax()
    {
        return this.storePveMax;
    }

    /// <summary>
    /// 设置坐骑行动力最大值
    /// </summary>
    public void setStorePvEPointMax(int pve_m)
    {
        this.storePveMax = pve_m;
    }

    /// <summary>
    /// 更新坐骑行动力最大值
    /// </summary>
    public void updateStorePvEPointMax(int pve_m)
    {
        this.storePveMax += pve_m;
    }

    //设置行动力最大值
    public void setPvEPointMax(int pve_m)
    {
        this.pve_max = pve_m;
    }
    //更新行动力最大值
    public void updatePvEPointMax(int pve_m)
    {
        this.pve_max += pve_m;
    }

    //得到服务器的pvp第一个回复点时间
    public void setPvpFirstTime(long pvp_s)
    {
        PvpFirstTime = ServerTimeKit.getMillisTime() + pvp_s;
    }

    /// <summary>
    /// 得到服务器的pve存储第一个回复点时间
    /// </summary>
    public void setStorePveFirstTime(long pve_s)
    {
        StorePveFirstTime = ServerTimeKit.getMillisTime() + pve_s;
    }

    //得到服务器的pve第一个回复点时间
    public void setPveFirstTime(long pve_s)
    {
        PveFirstTime = ServerTimeKit.getMillisTime() + pve_s;
    }

    //得到服务器的chv第一个回复点时间
    public void setChvFirstTime(long chv_s)
    {
        if (chv_s < 0)
            chv_s = 0;
        ChvFirstTime = ServerTimeKit.getMillisTime() + chv_s;
    }

    //获得行动力恢复满需要多少时间
    public int getPveFullTime()
    {
        if (isPveFull()) return 0;
        int pveFullTime = (int)(updatePve());
        pveFullTime += (pve_max - (pve + 1)) * UserManager.PVE_SPEED;
        return pveFullTime;
    }

    //获得回复下一点行动力需要的时间
    public long updatePve()
    {
        if (isPveFull())
        {
            setPveFirstTime(UserManager.PVE_SPEED);
            return 0;
        }
        else
        {
            long t = ServerTimeKit.getMillisTime() - (long)PveFirstTime;
            if (t > 0)
            {
                //大于说明游戏暂停导致时间超过.进行修正
                int los = (int)(t / UserManager.PVE_SPEED) + 1;
                PveFirstTime += los * UserManager.PVE_SPEED;
                _addPvEPoint(los);
                t = ServerTimeKit.getMillisTime() - (long)PveFirstTime;
            }
            return (long)Mathf.Abs(t);
        }
    }
    //获得回复下一点行动力需要的时间
    public long updatePvp()
    {
        if (isPvpFull())
        {
            setPvpFirstTime(UserManager.PVP_SPEED);
            return 0;
        }
        else
        {
            long t = ServerTimeKit.getMillisTime() - (long)PvpFirstTime;
            if (t > 0)
            {
                //大于说明游戏暂停导致时间超过.进行修正
                int los = (int)(t / UserManager.PVP_SPEED) + 1;
                addPvPPoint(los);
                PvpFirstTime += los * UserManager.PVP_SPEED;
                t = ServerTimeKit.getMillisTime() - (long)PvpFirstTime;
            }
            return (long)Mathf.Abs(t);
        }
    }
    //获得回复下一点英雄之章需要的时间
    public long updateChv()
    {
        if (isChvFull())
        {
            setChvFirstTime(UserManager.CHV_SPEED);
            return 0;
        }
        else
        {
            long t = ServerTimeKit.getMillisTime() - (long)ChvFirstTime;
            if (t > 0)
            {
                //大于说明游戏暂停导致时间超过.进行修正
                int los = (int)(t / UserManager.CHV_SPEED) + 1;
                addChvPoint(los);
                ChvFirstTime += los * UserManager.CHV_SPEED;
                t = ServerTimeKit.getMillisTime() - (long)ChvFirstTime;
            }
            return (long)Mathf.Abs(t);
        }
    }

    /// <summary>
    /// 获得恢复下一点坐骑行动力需要的时间
    /// </summary>
    /// <returns>The chv.</returns>
    public long updateStorePve()
    {
        int speed = MountsConfigManager.Instance.getPveSpeed();
        if (!isPveFull() || isStorePveFull())
        {
            setStorePveFirstTime(speed);
            return 0;
        }
        else
        {
            long t = ServerTimeKit.getMillisTime() - (long)StorePveFirstTime;
            if (t > 0)
            {
                //大于说明游戏暂停导致时间超过.进行修正
                int los = (int)(t / speed) + 1;
                addStorePvEPoint(los);
                StorePveFirstTime += los * speed;
                t = ServerTimeKit.getMillisTime() - (long)StorePveFirstTime;
            }
            return (long)Mathf.Abs(t);
        }
    }

    //得到头像路径
    public string getIconPath()
    {
        return UserManager.Instance.getIconPath(style);
    }
    //得到肖像路径
    public string getImagePath()
    {
        return UserManager.Instance.getImagePath(style);
    }
    //得到肖像路径
    public string getModelPath()
    {
        return UserManager.Instance.getModelPath(style);
    }


    //花费pve,pvp点数
    public bool costPoint(int num, int type)
    {
        if (costCheck(num, type))
        {
            if (type == MissionEventCostType.COST_PVE)
            {
                if (storePve > 0)
                {
                    int reNum = storePve - num;
                    if (reNum > 0)
                    {
                        storePve = reNum;
                    }
                    else
                    {
                        storePve = 0;
                        setPvEPoint(pve - Mathf.Abs(reNum));
                    }
                }
                else
                {
                    setPvEPoint(pve - num);
                }
            }
            else if (type == MissionEventCostType.COST_PVP)
            {
                pvp = pvp - num;
            }
            else if (type == MissionEventCostType.COST_CHV)
            {
                chv = chv - num;
            }
            return true;
        }
        return false;
    }
    //改变钱
    public void updateMoney(int money)
    {
        this.money = money;
    }

    //改变人民币
    public void updateRMB(int rmb)
    {
        this.rmb = rmb;
    }

    //检查是否能够消耗
    public bool costCheck(int num, int type)
    {
        if (type == MissionEventCostType.COST_PVE)
        {
            if (getTotalPvEPoint() < num)
                return false;
            else
                return true;
        }
        else if (type == MissionEventCostType.COST_PVP)
        {
            if (getPvPPoint() < num)
                return false;
            else
                return true;
        }
        else if (type == MissionEventCostType.COST_CHV)
        {
            if (getChvPoint() < num)
                return false;
            else
                return true;
        }
        else
        {
            return false;
        }
    }

    //开始倒计时
    public void startCountdown()
    {
        timer = TimerManager.Instance.getTimer(UserManager.TIMER_DELAY);
        timer.addOnTimer(onTimer);
        timer.start();
    }

    /// <summary>
    /// 设置各种时间监控数据
    /// </summary>
    private void onTimer()
    {
        updatePve();
        updatePvp();
        updateChv();
        updateStorePve();
        if (isWarCDTime)
        {
            startWarCDTime--;
            if (startWarCDTime <= 0)
            {
                isWarCDTime = false;
                startWarCDTime = 0;
            }
        }
    }
    /// <summary>
    /// 讨伐战败后设置计算冷却时间打开
    /// </summary>
    public void startWarCD()
    {
        if (startWarCDTime == 0)
        {
            startWarCDTime = 10;
            isWarCDTime = true;
        }
    }
    /// <summary>
    /// 获得讨伐冷却时间
    /// </summary>
    public int getWarCDTime()
    {
        return Math.Max(startWarCDTime, 0);
    }

    //更新荣誉
    public void updateHonor(int costValue)
    {
        honorLevel++;
        honor -= costValue;
    }

    public void updateVipLevel(long vipExp)
    {
        int oldLevel = vipLevel;//未加经验前的等级
        updateVipExp(vipExp);
        int newLevel = vipLevel;
        //vip升级,处理特权
        if (newLevel > oldLevel)
            VipManagerment.Instance.updateLevel(oldLevel, newLevel);
    }

    public void updateVipRewardLevel(int[] vipAwardSids)
    {
        VipManagerment.Instance.setAwardSids(vipAwardSids);
    }
    //最后一次领取的升级奖励Sid
    public void updateLevelupRewardLastSid(int lastLevelupRewardSid)
    {
        LevelupRewardManagerment.Instance.lastRewardSid = lastLevelupRewardSid;
    }

    public void updateLadderRank(int _ladderRank)
    {
        LaddersManagement.Instance.currentPlayerRank = _ladderRank;
    }

    public Dictionary<string, string> ToDic(int state = 0)
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("roleId", uid);//当前登录的玩家角色ID，必须为数字
        dic.Add("roleName", nickname);//当前登录的玩家角色名，不能为空，不能为null
        dic.Add("roleLevel", (getUserLevel() == 0 ? 1 : getUserLevel()).ToString());//当前登录的玩家角色等级，必须为数字，且不能为0，若无，传入1
        dic.Add("zoneId", ServerManagerment.Instance.lastServer.sid);//当前登录的游戏区服ID，必须为数字，且不能为0，若无，传入1
        dic.Add("zoneName", ServerManagerment.Instance.lastServer.Namec);//当前登录的游戏区服名称，不能为空，不能为null
        dic.Add("balance", rmb.ToString());   //用户游戏币余额，必须为数字，若无，传入0
        dic.Add("vip", (getVipLevel() + 1).ToString());            //当前用户VIP等级，必须为数字，若无，传入1
        dic.Add("partyName", string.IsNullOrEmpty(guildName) ? "无帮派" : guildName);//当前角色所属帮派，不能为空，不能为null，若无，传入“无帮派”
        dic.Add("roleCTime", ServerTimeKit.getSecondTime().ToString());    //单位为秒，创建角色的时间
        dic.Add("roleLevelMTime", state == 1 ? ServerTimeKit.getSecondTime().ToString() : "-1");  //单位为秒，角色等级变化时间,如果没有就传-1
        return dic;
    }

    public GameRoleInfo ToRoleInfo()
    {
        GameRoleInfo gameRoleInfo = new GameRoleInfo();

        gameRoleInfo.gameRoleBalance = rmb.ToString();
        gameRoleInfo.gameRoleID = uid;
        gameRoleInfo.gameRoleLevel = getUserLevel().ToString();
        gameRoleInfo.gameRoleName = nickname;
        gameRoleInfo.partyName = string.IsNullOrEmpty(guildName) ? "" : guildName;
        gameRoleInfo.serverID = ServerManagerment.Instance.lastServer.sid;
        gameRoleInfo.serverName = ServerManagerment.Instance.lastServer.Namec;
        gameRoleInfo.vipLevel = getVipLevel().ToString();
        gameRoleInfo.roleCreateTime = ServerTimeKit.getSecondTime().ToString();//UC与1881渠道必传，值为10位数时间戳

        gameRoleInfo.gameRoleGender = "";//360渠道参数
        gameRoleInfo.gameRolePower = "";//360渠道参数，设置角色战力，必须为整型字符串
        gameRoleInfo.partyId = "";//360渠道参数，设置帮派id，必须为整型字符串

        gameRoleInfo.professionId = "";//360渠道参数，设置角色职业id，必须为整型字符串
        gameRoleInfo.profession = "";//360渠道参数，设置角色职业名称
        gameRoleInfo.partyRoleId = "";//360渠道参数，设置角色在帮派中的id
        gameRoleInfo.partyRoleName = ""; //360渠道参数，设置角色在帮派中的名称
        gameRoleInfo.friendlist = "";//360渠道参数，设置好友关系列表，格式请参考：http://open.quicksdk.net/help/detail/aid/190
        return gameRoleInfo;
    }

    public override void copy(object destObj)
    {
        base.copy(destObj);
    }
}

