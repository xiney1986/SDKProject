using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *诸神之战管理 
 * */
public class GodsWarManagerment
{
	#region  点位常量
		/** 16进8 */
		public  int[] STATE_16_8 = {1,2,3,4,7,8,9,10};
		/** 8进4 */
		public  int[] STATE_8_4 = {5,6,11,12};
		/** 4进2 */
		public  int[] STATE_4_2 = {13,14};
		/** 冠军 */
		public const int STATE_CHAMPION = 15;
		/** 神魔大战 */
		public const int STATE_FINAL = 16;
		public const int TYPE_BRONZE = 110;
		public const int TYPE_SILVER = 111;
		public const int TYPE_GOLD = 112;
    public const string STATE_SERVER_BUSY = "server_busy";
		public const string STATE_NOTOPEN = "not_open";
		public const string STATE_NOT_ZIGE = "not_zige";
		public const string STATE_NOT_ZIGE_GROUP = "not_zige_group";
		public const string STATE_NOT_ZIGE_TAOTAI = "not_zige_taotai";
		public const string STATE_NOT_ZIGE_FINAL = "not_zige_final";
		public const string STATE_HAVE_ZIGE = "have_zige";
		public const string STATE_HAVE_ZIGE_GROUP = "have_zige_group";
		public const string STATE_HAVE_ZIGE_TAOTAI = "have_zige_taotai";
		public const string STATE_HAVE_ZIGE_FINAL = "have_zige_final";
		public const string STATE_PREPARE = "prepare";
	

	#endregion

	#region 状态属性

		/** 我的竞技信息 */
		public GodsWarUserInfo self;
		/** 上次刷新对手时间 */
		public int lastUpdateEnemyTime;
		/** 已挑战次数 */
		public int challengeCount;
		/** 最大可挑战次数 */
		public int maxChallengeCount;
		/** 对手列表 */
		public List<GodsWarUserInfo> enemyList;
		/** 比赛状态,0开服竞技等待阶段,1分组休赛,2(32-16),3(16-8),4(8-4),5(4-2),6决赛*/
		public int state = 0;
		/** 状态结束时间 */
		public int stateEndTime;
		/** 淘汰赛用户信息列表 */
		public List<GodsWarFinalUserInfo> finalInfoList;
		/** 神魔大战点位列表 */
		public List<GodsWarFinalPoint> shenMoPointlist;
		/** 神魔大战用户信息 */
		public List<GodsWarFinalUserInfo> shenMoUserlist;
		/** 战报点位信息 */
		public List<GodsWarFinalPoint> godsWarFinalPoints;
		/** 对战玩家信息 */
		public List<GodsWarFinalUserInfo> pvpGodsWarFinalInfo;
		/** 单个基础玩家信息 */
		public GodsWarFinalUserInfo singlePlayer;
		/** 黄铜组玩家排名信息 */
		public List<GodsWarRankUserInfo> usersRankList_bronze;
		/** 白银组玩家排名信息 */
		public List<GodsWarRankUserInfo> usersRankList_silver;
		/** 黄金组玩家排名信息 */
		public List<GodsWarRankUserInfo> usersRankList_gold;
		/** 我的支持信息 */
		public List<GodsWarMySuportInfo> mySuportInfo;
		/** 小组玩家排名信息 */
		public List<GodsWarRankUserInfo> myGroupRanklist;


		/** 当前决赛场次 */
		public int finalRound;
		/** 当前正在挑战的对手uid */
		public string currentMassEnemyUid;

		/** 当前所在组(青铜，白银，黄金) */
		public int type = 2;
		/** 当前所在战场(圣域=1，魔域=2，神域=0) */
		public int tapIndex = 1;

		/** 已经领取的分数 */
		public List<int> currentScores;
		public int integralRank;


	#endregion

	#region  公用属性
		/// <summary>
		/// 诸神之战是否开启
		/// </summary>
		public bool isOpen = true;
		/// <summary>
		/// 祖神战开启状态(1:小组赛 2:淘汰赛 3:决赛 4:准备(休赛) 5:暂未开放)
		/// </summary>
		public int  StateInfo = -1;
		/// <summary>
		/// 是否有参与资格
		/// </summary>
		public bool godsWarZige = true;
		/// <summary>
		/// 拥有参与资格后，决赛的大组Id和域名
		/// </summary>
		public bool taoTaiZige = true;
		public int yu_ming = -1;
		public int big_id = -1;
		/// <summary>
		/// 我的排名
		/// </summary>
		public int myFinalRank;
	#endregion



		public static GodsWarManagerment Instance {
				get{ return SingleManager.Instance.getObj ("GodsWarManagerment") as GodsWarManagerment;}
		}


	#region  变量设置
		/// <summary>
		/// 设置是否开启
		/// </summary>
		public void setIsOpen (bool bo)
		{
				this.isOpen = bo;
		}

		/// <summary>
		/// 得到是否开启
		/// </summary>
		public bool GetIsOpen ()
		{
				return isOpen;
		}
		/// <summary>
		/// 获取给后台的组别类型
		/// </summary>
		public int getType ()
		{
				int _type;
				switch (type) {
		
				case 0:
						_type = TYPE_BRONZE;
						break;
				case 1:
						_type = TYPE_SILVER;
						break;
				case 2:
						_type = TYPE_GOLD;
						break;
				default:
						_type = TYPE_BRONZE;
						break;
				}
				return _type;
		}

		public void setTypeIndex ()
		{
				type = big_id;
				tapIndex = yu_ming;
		}

		/// <summary>
		/// 判断当前是否是服务器开服30天后
		/// </summary>
		public bool isOnlineDay30 ()
		{
				int onlineDay = (ServerTimeKit.getSecondTime () - ServerTimeKit.onlineTime) / 3600 / 24;
				if (GodsWarInfoConfigManager.Instance ().getSampleBySid (7001).num[0] >= onlineDay)
						return false;
				return true;
		}

		/// <summary>
		/// 设置对手信息
		/// </summary>
		public void setEnemyList (List<GodsWarUserInfo> list)
		{
				if (list == null)
						list = new List<GodsWarUserInfo> ();
            if(enemyList!=null)enemyList.Clear();
				enemyList = list;
		}

		/// <summary>
		/// 得到对手信息
		/// </summary>
		public List<GodsWarUserInfo> getEnemyList ()
		{
				return enemyList;
		}

		/// <summary>
		/// 获取当前可用挑战次数
		/// </summary>
		public int getChallengeCount ()
		{
				return maxChallengeCount - challengeCount;
		}

		public int getMaxCanBuyCount ()
		{
				int addNum = 0;
				int vipLevel = UserManager.Instance.self.vipLevel;
				Vip vip = VipManagerment.Instance.getVipbyLevel (vipLevel);
				if (vip != null) {
//			addNum = vip.privilege.godsWarCountBuyAdd;
				}
				return GodsWarInfoConfigManager.Instance ().getSampleBySid (1001).num[0] + addNum;
		}
		/// <summary>
		/// 根据localid获取当前赛事进度(1:淘汰赛8强 2：淘汰赛4强 3:淘汰赛2强 4:冠军赛)
		/// </summary>
		public int getTypeByLocalId (int localid)
		{
				for (int i = 0; i < STATE_16_8.Length; i++) {
						if (STATE_16_8 [i] == localid)
								return 1;
				}
				for (int i = 0; i < STATE_8_4.Length; i++) {
						if (STATE_8_4 [i] == localid) {
								return 2;
						}
				}
				for (int i = 0; i < STATE_4_2.Length; i++) {
						if (STATE_4_2 [i] == localid) {
								return 3;
						}
				}
				if (STATE_CHAMPION == localid)
						return 4;
				if (STATE_FINAL == localid)
						return 5;
				return 999;
		}

		public int getWeekOfDayState ()
		{
				System.DateTime serverDate = ServerTimeKit.getDateTime ();
				int day = TimeKit.getWeekCHA (serverDate.DayOfWeek);
	
				if (day == 6 || day == 7)
						state = 1;
				if (day == 1 || day == 2 || day == 3)
						state = 2;
				if (day == 4)
						state = 3;
				if (day == 5)
						state = 4;

//		if(GameManager.Instance.godsWarState!=-1)
//			return GameManager.Instance.godsWarState;
				return state;
		}

		public string getStateInfo ()
		{
				System.DateTime serverDate = ServerTimeKit.getDateTime ();
				int secondTime = ServerTimeKit.getSecondTime ();
				int day = TimeKit.getWeekCHA (serverDate.DayOfWeek);
				string time = "";
                if (getGodsWarStateInfo() == STATE_NOTOPEN || day == 6 || day == 7 ) {
						time = LanguageConfigManager.Instance.getLanguage ("godsWar_132") + getDateTime (ServerTimeKit.getSecondTime () + 86400 * (7 - day + 1));
						return time;
				}
                if (getGodsWarStateInfo() == STATE_SERVER_BUSY) return LanguageConfigManager.Instance.getLanguage("godsWar_13233");
				switch (day) {
				case 1:
						time = LanguageConfigManager.Instance.getLanguage ("godsWar_17") + getDateTime (ServerTimeKit.getSecondTime ()) + "-" + getDateTime (ServerTimeKit.getSecondTime () + 86400 * 2);
						break;
				case 2:
						time = LanguageConfigManager.Instance.getLanguage ("godsWar_17") + getDateTime (ServerTimeKit.getSecondTime () - 86400) + "-" + getDateTime (ServerTimeKit.getSecondTime () + 86400);
						break;
				case 3:
						time = LanguageConfigManager.Instance.getLanguage ("godsWar_17") + getDateTime (ServerTimeKit.getSecondTime () - 86400 * 2) + "-" + getDateTime (ServerTimeKit.getSecondTime ());
						break;
				case 4:
						time = LanguageConfigManager.Instance.getLanguage ("godsWar_19") + getDateTime (ServerTimeKit.getSecondTime ());
						break;
				case 5:
						time = LanguageConfigManager.Instance.getLanguage ("godsWar_20") + getDateTime (ServerTimeKit.getSecondTime ());
						break;
				default:
						break;
				}
				return time;
		}

		public string getMyRankInfo ()
		{
				string myrank = "";
				switch (myFinalRank) {
				case 1:
						myrank = LanguageConfigManager.Instance.getLanguage ("godsWar_117");
						break;
				case 2:
						myrank = LanguageConfigManager.Instance.getLanguage ("godsWar_118");
						break;
				case 3:
						myrank = LanguageConfigManager.Instance.getLanguage ("godsWar_119");
						break;
				case 4:
						myrank = LanguageConfigManager.Instance.getLanguage ("godsWar_120");
						break;
				case 5:
						myrank = LanguageConfigManager.Instance.getLanguage ("godsWar_121");
						break;
				case 6:
						myrank = LanguageConfigManager.Instance.getLanguage ("godsWar_122");
						break;
				default:
						myrank = LanguageConfigManager.Instance.getLanguage ("godsWar_123");
						break;
				}
				return myrank;
		}

		public string getMyGroupInfo ()
		{
				string myGroup = "";
				switch (type) {
				case 0:
						myGroup = LanguageConfigManager.Instance.getLanguage ("godsWar_70");
						break;
				case 1:
						myGroup = LanguageConfigManager.Instance.getLanguage ("godsWar_71");
						break;
				case 2:
						myGroup = LanguageConfigManager.Instance.getLanguage ("godsWar_72");
						break;
				default:
						break;
				}
				return myGroup;
		}

		/// <summary>
		/// 获得日期
		/// </summary>
		public string getDateTime (int secondTime)
		{
				return TimeKit.dateToFormat (secondTime, LanguageConfigManager.Instance.getLanguage ("notice04"));
		}
		/// <summary>
		/// 获取当前服务器为星期几
		/// </summary>
		/// <returns>The day of week.</returns>
		public int GetDayOfWeek ()
		{
				System.DateTime serverDate = ServerTimeKit.getDateTime ();
				int day = TimeKit.getWeekCHA (serverDate.DayOfWeek);
				return day;
		}
		/// <summary>
		/// 半决赛预告是否开启（淘汰赛）
		/// </summary>
		public bool isTaoTaiOpen ()
		{
				if (!GodsWarManagerment.Instance.getGodsWarStateInfo ().EndsWith ("_taotai"))
						return false;
				System.DateTime serverDate = ServerTimeKit.getDateTime ();
				List<godsWarTime> goodsTime = GodsWarInfoConfigManager.Instance ().getSampleBySid (6001).times;
				int min = GodsWarInfoConfigManager.Instance ().getSampleBySid (9001).num[0];
				if (goodsTime == null)
						return false;

				if (serverDate.Hour * 60 + serverDate.Minute - (goodsTime [goodsTime.Count - 1].hour * 60 + goodsTime [goodsTime.Count - 1].minute) <= min) {
						return true;
				} else 
						return false;
		}

		/// <summary>
		/// 决赛预告是否开启（神魔大战）
		/// </summary>
		public bool isFinalOpen ()
		{
            if (GodsWarManagerment.Instance.getGodsWarStateInfo() == "not_open")
						return false;
				System.DateTime serverDate = ServerTimeKit.getDateTime ();
		    if (serverDate.DayOfWeek!=DayOfWeek.Friday) return false;

				List<godsWarTime> goodsTime = GodsWarInfoConfigManager.Instance ().getSampleBySid (8001).times;
				int min = GodsWarInfoConfigManager.Instance ().getSampleBySid (9001).num[0];
				if (goodsTime == null)return false;
                if (goodsTime[goodsTime.Count - 1].hour * 60 + goodsTime[goodsTime.Count - 1].minute - serverDate.Hour * 60 - serverDate.Minute <= min && goodsTime[goodsTime.Count - 1].hour * 60 + goodsTime[goodsTime.Count - 1].minute - serverDate.Hour * 60 - serverDate.Minute>0) return true;
				return false;
		}

	#endregion
	
	
	
	#region  后台通信
		/// <summary>
		/// 获取诸神战当前所有的状态信息
		/// </summary>
		public void getGodsWarStateInfo (CallBack callback)
		{
				FPortManager.Instance.getFPort<GodsWarGetStateInfoFPort> ().access (callback);
		}
		/// <summary>
		/// 判断当前的状态
		/// </summary>
		public string getGodsWarStateInfo ()
		{
				string state;
				if (StateInfo == -1)
						return "";
		        if (StateInfo==6)
		        {//数据清理中
		             state = STATE_SERVER_BUSY;
		        }
				else if (StateInfo == 5) {//没有开启
						state = STATE_NOTOPEN;
				} else if (StateInfo == 4) {//准备中
						state = STATE_PREPARE;
				} else {//开启
						if (!godsWarZige) {//没有参赛资格
								state = STATE_NOT_ZIGE;
								if (StateInfo == 1) {//处于小组赛
										state = STATE_NOT_ZIGE_GROUP;
								} else if (StateInfo == 2) {//处于淘汰赛
										state = STATE_NOT_ZIGE_TAOTAI;
								} else if (StateInfo == 3) {//处于决赛
										state = STATE_NOT_ZIGE_FINAL;
								}
						} else {//有参赛资格
								state = STATE_HAVE_ZIGE;
								if (StateInfo == 1) {//处于小组赛
										state = STATE_HAVE_ZIGE_GROUP;
								} else if (StateInfo == 2) {//处于淘汰赛
										state = STATE_HAVE_ZIGE_TAOTAI;
								} else if (StateInfo == 3) {//处于决赛
										state = STATE_HAVE_ZIGE_FINAL;
								}
						}
				}
				return state;
		}

	#endregion



	#region  配置管理
	
	
	#endregion
}
