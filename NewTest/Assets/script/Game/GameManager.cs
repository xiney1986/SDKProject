using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

/** 
  * 游戏管理器
  * @author 李程
  * */

public class GameManager : MonoBase
{
    public const bool DEBUG = true; //是否为调试模式,用于控制打印,catch抛出异常便于定位

    public UIAtlas mainAtlas;
    public Font mainFont;
    //public GameObject titlePrefab;
    public ScreenManager screenManager;
    public static GameManager Instance;
    public Material defUItextureMat;
    //	public TextAsset text4000;
    public BeginGuide guide;
    public string json;
    public int gameSpeed = 1; //游戏加速，出包的时候请改回"1"
    public bool battleFast; //是否快速战斗,用于测试
    public bool skipGuide;//是否跳过新手
    public bool GM;//GM模拟登录
    public bool allowLoadFromRes;//false为资源包读取模式
    public bool maskDebug = false;
    public bool ignoreUpdate = false;//是否忽略外部更新
    public CallBack battleReportCallback;
    [HideInInspector]
    public bool isCanBeSecondSkill = false;
    [HideInInspector]
    public int playAnimationType = 0;//播放新手指引女神动画状态,0初始化,1动画,2动画后
    [HideInInspector]
    public bool isOpenExitWin = false;//是否打开了退出游戏提示
    float f = 0.5f;
    int newcount;
    int count;
    bool logOuting = false;//是否正在登出
    long lastUpdateTime;
    CallBackMsg MsgCallback;
    private bool startResumeConnect = false;
    private bool startShowAlert;
    [HideInInspector]
    public bool isFormMissionByGuide = false;//是否因为引导而从副本中去到主界面
    [HideInInspector]
    public bool isFirstLoginOpenBulletin = true;//是否第一次登陆弹出公告栏
                                                //main
    [HideInInspector]
    public Timer
        timer;
    [HideInInspector]
    public int
        loginDay;
    public bool disconnetTest = false;
    public LoginStateManager loginStateManagr;
    public bool skipStory = false; //跳过剧情
                                   /** 配置版本号 */
    public static string CONFIG_VERSION = "-1";
    [HideInInspector]
    public bool isShowStarSoulOneKeySelect = true;
    [HideInInspector]
    public int starSoulOneKeySelectValue = 1;
    public bool isReLogin = false;
    [HideInInspector]
    public CallBack miaoShaCallBack;//得到奖励以后的回调
    [HideInInspector]
    public bool isOpenMiaoSha;//得到奖励以后的回调
    public bool isShowTestServer;

    //	public int godsWarState = -1;

    /// <summary>
    /// 是否有版本号:true有
    /// </summary>
    public bool isHaveVersion()
    {
        return CONFIG_VERSION != "-1";
    }

    public void setMsgCallback(CallBackMsg callback)
    {
        MsgCallback = callback;
    }

    public IEnumerator DoMsgCallback(MessageHandle msg)
    {
        yield return new WaitForSeconds(0.1f);

        if (MsgCallback != null)
        {
            MsgCallback(msg);
            MsgCallback = null;
        }
    }

    public bool getStartResumeConnect()
    {
        return startResumeConnect;
    }

    void _push(int time, string text)
    {
        //		Debug.LogWarning (">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>add one notice at time:" + time + "         " + text);
    }

    public void intoBattleNoSwitchWindow()
    {
        LoadingWindow.isShowProgress = false;
        BattleDataErlang currentbattleData = BattleManager.battleData;
        if (currentbattleData.isLadders && !currentbattleData.isLaddersRecord && Instance.isCanBeSecondSkill &&
            PlayerPrefs.GetInt(UserManager.Instance.self.uid + "miaosha", 1) == 1)
        {
            MaskWindow.instance.setServerReportWait(false);
            bool isWin = currentbattleData.winnerID == TeamInfo.OWN_CAMP;
            LaddersWindow lw = UiManager.Instance.getWindow<LaddersWindow>();
            lw.fightBack = true;
            lw.fightWin = isWin;
            LaddersManagement.Instance.CurrentOppPlayer.isDefeated = true;
            LaddersManagement.Instance.M_updateChestStatus();
            LaddersManagement.Instance.currentChallengeTimes++;
            LaddersGetInfoFPort newFport = FPortManager.Instance.getFPort<LaddersGetInfoFPort>();
            newFport.apply((isUpdate) =>
            {
                lw.init();
            });
            // GameManager.Instance.battleReportCallback = null;
        }
        else if (currentbattleData.isArenaMass && Instance.isCanBeSecondSkill && PlayerPrefs.GetInt(UserManager.Instance.self.uid + "miaosha", 1) == 1)//海选
        {
            MaskWindow.instance.setServerReportWait(false);
            bool isWin = BattleManager.battleData.winnerID == TeamInfo.OWN_CAMP;
            BattleManager.battleData = null;
            ArenaAuditionsWindow win = UiManager.Instance.getWindow<ArenaAuditionsWindow>();
            if (win != null)
            {
                win.isWin = isWin ? 0 : 1;
            }
            win.init();
        }

        else
        {
            ScreenManager.Instance.loadScreen(3, null, () =>
            {
                UiManager.Instance.openWindow<BattleWindow>();
            });
        }


    }

    public void intoBattle()
    {
        LoadingWindow.isShowProgress = false;
        BattleDataErlang currentbattleData = BattleManager.battleData;

        //副本pve具体处理方式
        if (MissionInfoManager.Instance.mission != null &&
            MissionInfoManager.Instance.mission.getChapterType() == ChapterType.STORY &&
            isCanBeSecondSkill && currentbattleData.isPve &&
            PlayerPrefs.GetInt(UserManager.Instance.self.uid + "miaosha", 1) == 1 &&
            UserManager.Instance.self.getUserLevel() >= 15 &&
            UiManager.Instance.missionMainWindow.gameObject.activeSelf)
        {
            if (UiManager.Instance.missionMainWindow != null)
            {
                //if (!UiManager.Instance.missionMainWindow.gameObject.activeSelf)
                //{
                //    UiManager.Instance.BackToWindow<MissionMainWindow>(() => {
                //        UiManager.Instance.missionMainWindow.playMiaoShaEffect(true);

                //        StartCoroutine(Utils.DelayRun(() => {
                //            UiManager.Instance.missionMainWindow.playMiaoShaEffect(false);
                //            if (MissionInfoManager.Instance.mission.getChapterType() == ChapterType.STORY) {
                //                //飘星星
                //                FubenGetStarFPort userFPort =
                //                    FPortManager.Instance.getFPort("FubenGetStarFPort") as FubenGetStarFPort;
                //                userFPort.getStar(UserManager.Instance.self.updateStarSum,
                //                    MissionManager.instance.playStarEffect);
                //            }
                //            //飘奖励
                //            Award award = AwardManagerment.Instance.miaoShaAward == null
                //                ? null
                //                : Award.mergeAward(AwardManagerment.Instance.miaoShaAward);
                //            AwardManagerment.Instance.miaoShaAward = null;
                //            List<PrizeSample> tempStringList = getAwardMessageInfo(award);
                //            if (tempStringList != null) {
                //                PrizeSample[] pss = new PrizeSample[tempStringList.Count];
                //                for (int i = 0; i < tempStringList.Count; i++) {
                //                    pss[i] = tempStringList[i];
                //                }
                //                UiManager.Instance.openDialogWindow<PropMessageLineWindow>((win) => {
                //                    win.Initialize(pss);
                //                });
                //            }
                //            miaoShaCallBack();
                //            miaoShaCallBack = null;
                //        }, 2f));
                //    });
                //}
                //else
                //{
                UiManager.Instance.missionMainWindow.playMiaoShaEffect(true);

                StartCoroutine(Utils.DelayRun(() =>
                {
                    UiManager.Instance.missionMainWindow.playMiaoShaEffect(false);
                    if (MissionInfoManager.Instance.mission.getChapterType() == ChapterType.STORY)
                    {
                        //飘星星
                        FubenGetStarFPort userFPort =
                            FPortManager.Instance.getFPort("FubenGetStarFPort") as FubenGetStarFPort;
                        userFPort.getStar(UserManager.Instance.self.updateStarSum, MissionManager.instance.playStarEffect);
                    }
                    //飘奖励
                    Award award = AwardManagerment.Instance.miaoShaAward == null ? null : Award.mergeAward(AwardManagerment.Instance.miaoShaAward);
                    AwardManagerment.Instance.miaoShaAward = null;
                    List<PrizeSample> tempStringList = getAwardMessageInfo(award);
                    if (tempStringList != null)
                    {
                        UiManager.Instance.openDialogWindow<PropMessageLineWindow>((win) =>
                        {
                            win.init(tempStringList, false);
                        });
                    }
                    StartCoroutine(Utils.DelayRun(() =>
                    {
                        miaoShaCallBack();
                        miaoShaCallBack = null;
                    }, 2));
                }, 2f));
            }
            //}
        }
        //副本pvp具体处理方式
        else if (currentbattleData != null && currentbattleData.isPvP && Instance.isCanBeSecondSkill &&
            PlayerPrefs.GetInt(UserManager.Instance.self.uid + "miaosha", 1) == 1 &&
            UserManager.Instance.self.getUserLevel() >= 15)
        {
            PvpInfoManagerment.Instance.isMs = true;
            PvpInfoManagerment.Instance.result(true);
            //
        }
        //天梯秒杀具体处理方式
        else if (currentbattleData != null && currentbattleData.isLadders && !currentbattleData.isLaddersRecord && Instance.isCanBeSecondSkill &&
            PlayerPrefs.GetInt(UserManager.Instance.self.uid + "miaosha", 1) == 1)
        {
            MaskWindow.instance.setServerReportWait(false);
            bool isWin = currentbattleData.winnerID == TeamInfo.OWN_CAMP;
            LaddersWindow lw = UiManager.Instance.getWindow<LaddersWindow>();
            lw.fightBack = true;
            lw.fightWin = isWin;
            LaddersManagement.Instance.CurrentOppPlayer.isDefeated = true;
            LaddersManagement.Instance.M_updateChestStatus();
            LaddersManagement.Instance.currentChallengeTimes++;
            LaddersGetInfoFPort newFport = FPortManager.Instance.getFPort<LaddersGetInfoFPort>();
            newFport.apply((isUpdate) =>
            {
                UiManager.Instance.BackToWindow<LaddersWindow>().init();
            });

        }
        else if (currentbattleData.isArenaMass && Instance.isCanBeSecondSkill && PlayerPrefs.GetInt(UserManager.Instance.self.uid + "miaosha", 1) == 1)//海选
        {
            MaskWindow.instance.setServerReportWait(false);
            bool isWin = BattleManager.battleData.winnerID == TeamInfo.OWN_CAMP;
            BattleManager.battleData = null;
            ArenaAuditionsWindow win = UiManager.Instance.getWindow<ArenaAuditionsWindow>();
            if (win != null)
            {
                win.isWin = isWin ? 0 : 1;
            }
            UiManager.Instance.BackToWindow<ArenaAuditionsWindow>().initt();
        }
        else
        {
            LoadingWindow.isShowProgress = false;
            ScreenManager.Instance.loadScreen(3, null, () =>
            {
                UiManager.Instance.switchWindow<BattleWindow>();
            });
        }

    }

    IEnumerator showLineMessage(List<string> tempList)
    {
        for (int i = 0; i < tempList.Count; i++)
        {
            UiManager.Instance.openDialogWindow<MessageLineWindow>((win) =>
            {
                win.Initialize(tempList[i], true);
            });
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(0.5f);
        MaskWindow.UnlockUI();
    }
    public void intoBattleForGodsWar()
    {
        LoadingWindow.isShowProgress = false;
        BattleDataErlang currentbattleData = BattleManager.battleData;
        if (currentbattleData != null && currentbattleData.isGodsWarGroupFight && Instance.isCanBeSecondSkill &&
            PlayerPrefs.GetInt(UserManager.Instance.self.uid + "miaosha", 1) == 1 &&
            UserManager.Instance.self.getUserLevel() >= 15)
        {
            MaskWindow.instance.setServerReportWait(false);
            bool isWin = BattleManager.battleData.winnerID == TeamInfo.OWN_CAMP;
            BattleManager.battleData = null;
            GodsWarGroupStageWindow win = UiManager.Instance.getWindow<GodsWarGroupStageWindow>();
            if (win != null)
            {
                win.isWin = isWin ? 0 : 1;
            }
            if (win != null)
            {
                win.updateWin();
            }
        }
        else
        {
            LoadingWindow.isShowProgress = false;
            UiManager.Instance.openWindow<EmptyWindow>((win) =>
            {
                ScreenManager.Instance.loadScreen(3, null, () =>
                {
                    UiManager.Instance.switchWindow<BattleWindow>();
                });
            });
        }
    }

    public void pushNotice()
    {
        DateTime date = DateTime.Now;
        if (date.Hour > 0 && date.Hour < 12)
        {
            int distance = 0;
            DateTime d1;

            d1 = new DateTime(date.Year, date.Month, date.Day, 12, 0, 0);
            distance = (int)TimeKit.getTimeMillis(d1) - (int)TimeKit.getTimeMillis(date);
            _push(distance, LanguageConfigManager.Instance.getLanguage("IOS_Notice12"));

            d1 = new DateTime(date.Year, date.Month, date.Day, 18, 0, 0);
            distance = (int)TimeKit.getTimeMillis(d1) - (int)TimeKit.getTimeMillis(date);
            _push(distance, LanguageConfigManager.Instance.getLanguage("IOS_Notice18"));

        }
        else if (date.Hour > 12 && date.Hour < 18)
        {
            DateTime d1;
            int distance = 0;
            d1 = new DateTime(date.Year, date.Month, date.Day, 18, 0, 0);
            distance = (int)TimeKit.getTimeMillis(d1) - (int)TimeKit.getTimeMillis(date);
            _push(distance, LanguageConfigManager.Instance.getLanguage("IOS_Notice18"));

            d1 = date.AddDays(1);
            d1 = new DateTime(d1.Year, d1.Month, d1.Day, 12, 0, 0);
            distance = (int)TimeKit.getTimeMillis(d1) - (int)TimeKit.getTimeMillis(date);
            _push(distance, LanguageConfigManager.Instance.getLanguage("IOS_Notice12"));

        }
        else
        {
            DateTime d1;
            int distance = 0;

            d1 = date.AddDays(1);
            d1 = new DateTime(d1.Year, d1.Month, d1.Day, 12, 0, 0);
            distance = (int)TimeKit.getTimeMillis(d1) - (int)TimeKit.getTimeMillis(date);
            _push(distance, LanguageConfigManager.Instance.getLanguage("IOS_Notice12"));

            d1 = date.AddDays(1);
            d1 = new DateTime(d1.Year, d1.Month, d1.Day, 18, 0, 0);
            distance = (int)TimeKit.getTimeMillis(d1) - (int)TimeKit.getTimeMillis(date);
            _push(distance, LanguageConfigManager.Instance.getLanguage("IOS_Notice18"));

        }
    }

    void Awake()
    {
        Application.runInBackground = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = 60;
        //资源加载管理器初始化
        Instance = this;
        ResourcesManager.Instance.init();
        ScreenManager.Instance = GetComponent<ScreenManager>();
    }

    void Start()
    {
        GameStart();
    }

    public void GameStart()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = 60;
        //Dictionary<string, List<Json_ServerInfo>> dic = new Dictionary<string, List<Json_ServerInfo>>();
        //List<Json_ServerInfo> list = new List<Json_ServerInfo>();
        //Json_ServerInfo server = new Json_ServerInfo("1","2",3,"4");
        //list.Add(server);
        //dic.Add("servers", list);
        //Debug.Log(MiniJSON.Json.Serialize(dic));
        if (SdkManager.INSTANCE.IS_SDK)
        {
            LoadUrl();
        }
        else
        {
            if (GameObject.Find("NGUI_manager") == null)
            {
                loadBaseResource();
            }
        }
    }

    private void LoadUrl()
    {
        guide.text1.text = "获取服务器...";
        StartCoroutine(SdkManager.INSTANCE.LoadUrl("http://192.168.2.18:8080/", (result) =>
         {
             Debug.Log(result.state + "," + result.www.url);
             if (result.state == SdkManager.LoadStatus.OK)
             {
                 Dictionary<string, object> userInfoDic = (Dictionary<string, object>)MiniJSON.Json.Deserialize(result.www.text);
                 SdkManager.APP_VERSION = (string)userInfoDic["av"];
                 SdkManager.SERVERLIST_VERSION = (string)userInfoDic["sv"];
                 SdkManager.RESOURCES_VERSION = (string)userInfoDic["rv"];
                 SdkManager.CDN = (string)userInfoDic["cdn"];
                 LoadUpdate();
             }
             else
             {
                  guide.text1.text = "获取服务器失败，退出重进再试！";
             }
         }));
    }

    private int index = 0;
    private void LoadUpdate()
    {
        guide.text1.text = "检查更新...";
        StartCoroutine(SdkManager.INSTANCE.LoadUrl(SdkManager.CDN + "/" + SdkManager.RESOURCES_VERSION + "/update.json", (result) =>
        {
            Debug.Log(result.state + "," + result.www.error + "," + result.www.url);
            if (result.state == SdkManager.LoadStatus.OK)
            {
                Dictionary<string, object> dic = MiniJSON.Json.Deserialize(result.www.text) as Dictionary<string, object>;
                List<AssetItem> list = new List<AssetItem>(dic.Count);
                foreach (var VARIABLE in dic)
                {
                    string path = VARIABLE.Key;
                    ulong crc = ulong.Parse((string)VARIABLE.Value);
                    string newPath = Application.persistentDataPath + "/" + path;
                    if (!File.Exists(newPath))
                    {
                        Debug.Log(newPath + ",no Exist");
                        list.Add(new AssetItem(path, crc));
                    }
                    else
                    {
                        ulong tempCrc = Crc32Utils.GetCRC32Str(File.ReadAllBytes(newPath));
                        if (tempCrc != crc)
                        {
                            Debug.Log(newPath + ",crc is not same");
                            list.Add(new AssetItem(path, crc));
                        }
                    }
                }
                UpdateAsset(list);
            }
            else
            {
                guide.text1.text = "获取更新失败，退出重进再试！";
            }
        }));
    }

    private void UpdateAsset(List<AssetItem> list)
    {
        if (list.Count > 0)
        {
            guide.text1.text = "更新资源...";
            guide.progress.text = (index + 1) * 100f / list.Count + "%";
            StartCoroutine(SdkManager.INSTANCE.LoadUrl(SdkManager.CDN + "/" + SdkManager.RESOURCES_VERSION + "/" + list[index].p, (loadResult) =>
            {
                if (loadResult.state == SdkManager.LoadStatus.OK)
                {
                    ulong tempCrc = Crc32Utils.GetCRC32Str(loadResult.www.bytes);
                    if (tempCrc == list[index].c)
                    {
                        string newPath = Application.persistentDataPath + "/" + list[index].p;
                        Debug.Log(newPath);
                        File.WriteAllBytes(newPath, loadResult.www.bytes);
                        index++;
                        if (index >= list.Count)
                        {
                            LoadServerList();
                        }
                        else
                        {
                            UpdateAsset(list);
                        }
                    }
                    else
                    {
                        guide.text1.text = "获取更新失败，退出重进再试！";
                    }
                }
                else
                {
                    guide.text1.text = "获取更新失败，退出重进再试！";
                }
            }, 0));
        }
        else
        {
            LoadServerList();
        }
    }

    private void LoadServerList()
    {
        StartCoroutine(SdkManager.INSTANCE.LoadUrl(SdkManager.CDN + "/serverList.json?" + SdkManager.SERVERLIST_VERSION, (result) =>
        {
            Debug.Log(result.state + "," + result.www.url);
            if (result.state == SdkManager.LoadStatus.OK)
            {
                Debug.Log(result.www.text);
                Dictionary<string, object> dic = MiniJSON.Json.Deserialize(result.www.text) as Dictionary<string, object>;
                List<object> list = dic["servers"] as List<object>;
                List<Json_ServerInfo> serverList = new List<Json_ServerInfo>();
                foreach (object each in list)
                {
                    if (each != null)
                        serverList.Add(new Json_ServerInfo(each as Dictionary<string, object>));
                }
                SortServerList(serverList);
                ServerManagerment.Instance.setAllServer(serverList);
                if (GameObject.Find("NGUI_manager") == null)
                {
                    loadBaseResource();
                }
            }
            else if (result.state == SdkManager.LoadStatus.ERROR)
            {
                guide.text1.text = "获取线列表失败，退出重进再试！";
            }
            else if (result.state == SdkManager.LoadStatus.TIMEOUT)
            {
                guide.text1.text = "获取线列表失败，退出重进再试！";
            }
        }));
    }

    private void SortServerList(List<Json_ServerInfo> serverList)
    {
        if (serverList == null || serverList.Count < 1)
            return;
        List<Json_ServerInfo> newList = new List<Json_ServerInfo>();
        List<Json_ServerInfo> hotList = new List<Json_ServerInfo>();
        List<Json_ServerInfo> oldList = new List<Json_ServerInfo>();
        Json_ServerInfo server_info;
        //区分列表
        for (int i = 0; i < serverList.Count; i++)
        {
            server_info = serverList[i];
            if (server_info.is_new.Equals("1"))
                newList.Add(server_info);
            else if (server_info.is_hot.Equals("1"))
                hotList.Add(server_info);
            else
                oldList.Add(server_info);
        }
        SortSid(newList);
        SortSid(hotList);
        SortSid(oldList);
        serverList.Clear();
        ListKit.AddRange(serverList, newList);
        ListKit.AddRange(serverList, hotList);
        ListKit.AddRange(serverList, oldList);
    }
    //根据sid排序
    private void SortSid(List<Json_ServerInfo> list)
    {
        for (int j = 1; j < list.Count; j++)
        {//外循环每次把参与排序的最大数排在最后
            for (int i = 0; i < list.Count - j; i++)
            {  //内层循环负责对比相邻的两个数，并把最大的排在后面
                if (StringKit.toInt(list[i].sid) < StringKit.toInt(list[i + 1].sid))
                {  //如果前 一个数大于后一个数，则交换两个数
                    Json_ServerInfo temp = list[i];
                    list[i] = list[i + 1];
                    list[i + 1] = temp;
                }

            }
        }
    }

    private void loadBaseResource()
    {
        ServerManagerment.Instance.InitTestServers();
        string rootDir = PathKit.GetURLPath("", false, false);
        if (Directory.Exists(rootDir + "/Config") == false)
            Directory.CreateDirectory(rootDir + "/Config");
        if (ResourcesManager.Instance.allowLoadFromRes)
        {
            loadBaseResourceOk(null);
            return;
        }
        ResourcesManager.Instance.clean();
        guide.text1.text = "加载资源...";
        guide.progress.text = "0%";
        //缓冲初始资源
        string[] _list = ResourceConfigmanager.Instance.getLoginResource().ToArray();
        ResourcesManager.Instance.cacheData(_list, loadBaseResourceOk, "base");
    }
    void loadBaseResourceOk(List<ResourcesData> _list)
    {
        //init完成后呼叫ScreenReady();
        UiManager.Instance.init();
        AudioManager.Instance.init();
        DontDestroyOnLoad(gameObject);
        //初始化字体
        formatFontTexture();
        //pushNotice();
    }

    public void loadBaseExResource()
    {
        if (ResourcesManager.Instance.allowLoadFromRes)
        {
            loadBaseExResourceOK(null);
            return;
        }
        //缓冲初始资源
        ResourcesManager.Instance.UnloadAssetBundleBlock(ResourcesManager.Instance.baseDataList["Config"], false, "base");
        string[] _list = ResourceConfigmanager.Instance.getCacheResource().ToArray();
        ResourcesManager.Instance.cacheData(_list, effectCacheReady, "base");
    }

    void loadBaseExResourceOK(List<ResourcesData> _list)
    {
        loginStateManagr.isResourceCache = true;
    }

    public void loginBack(WindowBase fatherWindow)
    {
        //LoginWindow logwin=	UiManager.Instance.getWindow<LoginWindow>();
        UserManager.Instance.login();
        //切到空窗口后回调登录
        //		EventDelegate.Add (fatherWindow.OnHide, UserManager.Instance.login);
        //
        //		UiManager.Instance.switchWindow<EmptyWindow> ();
        //		
        //		UiManager.Instance.backGroundWindow.switchToDark ();

        //		UiManager.Instance.openDialogWindow<LoadingWindow> (
        //			(win) => {
        //			win.justLoading = true;
        //		}
        //		);

    }

    public void PrepareResourcesOK()
    {

        //临时保证config存在
        string rootDir = PathKit.GetURLPath("", false, false);
        if (Directory.Exists(rootDir + "/Config") == false)
            Directory.CreateDirectory(rootDir + "/Config");
        string[] _list1 = ResourceConfigmanager.Instance.getEffectResource().ToArray();

        if (ResourcesManager.Instance.allowLoadFromRes)
        {
            //baseCacheReady (null);
            return;
        }

        print("PrepareResourcesOK");
        ResourcesManager.Instance.clean();
        guide.showCacheText();
        guide.progress.text = "0%";
        //缓冲初始资源
        string[] _list = ResourceConfigmanager.Instance.getCacheResource().ToArray();
        ResourcesManager.Instance.cacheData(_list, effectCacheReady, "base");

    }

    void effectCacheReady(List<ResourcesData> lst)
    {

        string[] _list = ResourceConfigmanager.Instance.getEffectResource().ToArray();
        ResourcesManager.Instance.cacheData(_list, loadBaseExResourceOK, "effect");
        //TimeConfigManager.Instance.getLocalTimeInfo();
    }

    public void formatFontTexture()
    {

        if (ResourcesManager.Instance.allowLoadFromRes)
        {
            GameManager.Instance.mainFont = Resources.Load("fonts/font/font") as Font;
        }
        else
        {

            ResourcesData data = ResourcesManager.Instance.getResource("font");
            if (data != null)
                GameManager.Instance.mainFont = data.ResourcesBundle.Load("font") as Font;

        }

        //		GameManager.Instance .mainFont.characterInfo = null;
        //		GameManager.Instance.mainFont.RequestCharactersInTexture (text4000.text); 

    }

    public void logOut()
    {
        if (logOuting) return;


        logOuting = true;
        skipStory = false;
        UserManager.Instance.exit();
        backToLoginWindow();
    }

    public void ScreenReady()
    {
        UiManager.Instance.setScreenDPI();
        StartCoroutine(UiManager.Instance.setMask());

        LoadingWindow.isShowProgress = false;
        ScreenManager.Instance.loadScreen(1, null, openLoginWindow);

    }

    public void backToLoginWindow()
    {
        UiManager.Instance.clearWindowsName();
        ScreenManager.Instance.loadScreen(1, null, () =>
        {
            UiManager.Instance.openWindow<LoginWindow>((win) =>
            {
                logOuting = false;
            });

        });
    }




    void FixedUpdate()
    {
        TimerManager.Instance.update();
    }

    void Update()
    {
        if (Input.GetKeyDown("`"))
        {
            disconnetTest = true;
        }
        if (Input.GetKeyDown(KeyCode.Escape) && !isOpenExitWin)
        {
            if (UiManager.Instance.getWindow<SystemMessageWindow>() != null) UiManager.Instance.getWindow<SystemMessageWindow>().destoryWindow();
            SdkManager.INSTANCE.Exit();
        }
        //界面失去焦点超过10秒检查网络连接
        long now = TimeKit.getMillisTime();
        if (isReLogin)
        {
            if (lastUpdateTime > 0 && now - lastUpdateTime > 3000)
            {
                if (now - lastUpdateTime > 120000)
                {
                    MaskWindow.NetUnlock();
                    GameManager.Instance.OnLostConnect(true);
                }
                else
                {
                    MaskWindow.NetLockMaskShow();
                    OnApplicationResume();
                }
            }
        }
        lastUpdateTime = now;
    }


    void OnGUI()
    {

        if (Debug.isDebugBuild == true)
        {
            if (Time.time >= f)
            {
                f += 0.5f;
                newcount = Time.frameCount - count;
                count = Time.frameCount;
            }

            GUI.Label(new Rect(0, 0, 500, 50), (newcount * 2).ToString());
            if (UiManager.Instance != null)
                GUI.Label(new Rect(30, 0, 500, 50), ((int)(Input.mousePosition.x / UiManager.Instance.screenScaleX - 320)) + "," + ((int)(Input.mousePosition.y / UiManager.Instance.screenScaleY - 480)));


            if (!ResourcesManager.Instance.allowLoadFromRes)
            {
                GUI.Label(new Rect(0, 20, 500, 50), "AllTotalSize   " + ResourcesManager.Instance.AllTotalSize / 1024 + "MB");
                GUI.Label(new Rect(0, 40, 500, 50), "textureTotalSize   " + ResourcesManager.Instance.textureTotalSize / 1024 + "MB");
                GUI.Label(new Rect(0, 60, 500, 50), "otherTotalSize   " + ResourcesManager.Instance.otherTotalSize / 1024 + "MB");
                GUI.Label(new Rect(0, 80, 500, 50), "battleTotalSize   " + ResourcesManager.Instance.battleTotalSize / 1024 + "MB");
                GUI.Label(new Rect(0, 100, 500, 50), "uiTotalSize   " + ResourcesManager.Instance.uiTotalSize / 1024 + "MB");
                GUI.Label(new Rect(0, 120, 500, 50), "effectTotalSize   " + ResourcesManager.Instance.effectTotalSize / 1024 + "MB");
                GUI.Label(new Rect(0, 140, 500, 50), "baseTotalSize   " + ResourcesManager.Instance.baseTotalSize / 1024 + "MB");
            }


        }
    }

    public void netConnectInit()
    {
        ConnectManager.manager().init(gameObject);
        DataAccess.getInstance().defaultHandle = ServiceManager.Instance.severRadio;

    }

    IEnumerator delayOpenMainWindow()
    {
        yield return new WaitForSeconds(0.1f);
        UiManager.Instance.openWindow<MainWindow>();
    }
    /// <summary>
    /// 战斗失败退出强化界面
    /// </summary>
    public void outStrengItem()
    {
        if (UiManager.Instance.battleWindow == null)
            return;
        string chooseName = UiManager.Instance.battleWindow.ChooseStrengButton;
        UiManager.Instance.battleWindow.ChooseStrengButton = null;

        int type = 0;

        if (MissionInfoManager.Instance.mission != null)
            type = MissionInfoManager.Instance.mission.getChapterType();

        if (type == ChapterType.WAR ||
            type == ChapterType.PRACTICE)
        {
            if (MissionManager.instance != null)
            {
                MissionManager.instance.missionClean();
            }
            ArmyManager.Instance.unActiveArmy();
        }

        if (PvpInfoManagerment.Instance != null)
        {
            PvpInfoManagerment.Instance.clearDate();
        }

        MissionInfoManager.Instance.clearMission();
        UiManager.Instance.clearWindows(UiManager.Instance.getWindow<MainWindow>());
        UiManager.Instance.backGround.switchBackGround("ChouJiang_BeiJing");
        FuBenManagerment.Instance.isWarAttackBoss = false;
        FuBenManagerment.Instance.isWarActiveFuben = false;
        if (chooseName.StartsWith("strengRoleButton"))
        {
            string[] strs = chooseName.Split(':');
            IntensifyCardManager.Instance.intoIntensify(int.Parse(strs[1]));
        }
        else if (chooseName == "strengEquipButton")
        {
            UiManager.Instance.openWindow<IntensifyEquipWindow>();
        }
        else if (chooseName == "strengHallowsButton")
        {
            UiManager.Instance.openWindow<IntensifyHallowsWindow>((win) =>
            {
                win.inSideType = IntensifyHallowsWindow.TYPE_NEED;
            });
        }
    }

    //退出讨伐BOSS战斗
    public void outWarBattle()
    {
        UiManager.Instance.BackToWindow<TeamPrepareWindow>();
    }
    //退出活动副本战斗
    public void outActiveBattle()
    {
        UiManager.Instance.BackToWindow<ActivityChooseWindow>();
    }

    //退出工会战斗
    public void outGuildBattle()
    {
        UiManager.Instance.BackToWindow<GuildAltarWindow>();

    }

    /** 推出公会战战斗 */
    public void outGuildFightBattle()
    {
        UiManager.Instance.BackToWindow<GuildAreaWindow>();
    }

    public void outMiningWindow()
    {
        UiManager.Instance.BackToWindow<MiningWindow>();
    }
    public void outOneOnOneBossWindow()
    {
        UiManager.Instance.BackToWindow<OneOnOneBossWindow>();
    }
    public void outLastBattleWindow()
    {
        LastBattleInitFPort init = FPortManager.Instance.getFPort("LastBattleInitFPort") as LastBattleInitFPort;
        init.lastBattleInitAccess(() =>
        {
            UiManager.Instance.BackToWindow<LastBattleWindow>();
        });
        //UiManager.Instance.BackToWindow<LastBattleWindow>();
    }
    public void outGodsWarWindow()
    {

        if (BattleManager.battleData == null)
        {
            UiManager.Instance.openMainWindow();
            //异常
            Debug.LogWarning("GodsWar no battleData ?? why??!!!");
            return;
        }
        else
        {
            if (BattleManager.battleData.isGodsWarGroupFight)
            {
                bool isWin = BattleManager.battleData.winnerID == TeamInfo.OWN_CAMP;
                BattleManager.battleData = null;
                GodsWarGroupStageWindow win = UiManager.Instance.getWindow<GodsWarGroupStageWindow>();
                if (win != null)
                {
                    win.isWin = isWin ? 0 : 1;
                }
                if (UiManager.Instance.emptyWindow != null)
                {
                    UiManager.Instance.emptyWindow.finishWindow();
                }
                else
                {
                    UiManager.Instance.openMainWindow();
                }
            }
            else if (BattleManager.battleData.isGodsWarFinal)
            {
                bool isWin = BattleManager.battleData.winnerID == TeamInfo.OWN_CAMP;
                BattleManager.battleData = null;
                GodsWarFinalWindow win = UiManager.Instance.getWindow<GodsWarFinalWindow>();
                if (UiManager.Instance.emptyWindow != null)
                {
                    UiManager.Instance.emptyWindow.finishWindow();
                }
                else
                {
                    UiManager.Instance.openMainWindow();
                }
            }
        }
    }
    //退出战斗
    public void outSweepPvpBattle()
    {

        bool isWin = BattleManager.battleData.winnerID == TeamInfo.OWN_CAMP;

        if (isWin && PvpInfoManagerment.Instance.getCurrentRound() == 3 && PvpInfoManagerment.Instance.isCurrentRoundBattlePlayed)
        {
            //bugFix:必须在第三轮并且战斗结束后执行
            PvpInfoManagerment.Instance.clearDate();
        }
        else if (!isWin)
        {
            PvpInfoManagerment.Instance.clearDate();
            SweepManagement.Instance.clearPvpAward();//如果输了，就把连胜奖励清空，赢的情况，系统会自动覆盖奖励
        }
        UiManager.Instance.switchWindow<SweepAwardWindow>((winii) =>
        {
            winii.setDondShow(true);
        });
    }

    //退出到英雄之章界面
    public void outHeroRoadBattle()
    {
        //延时修正手机上返回不到英雄之章界面的bug
        StartCoroutine(Utils.DelayRun(() =>
        {
            UiManager.Instance.emptyWindow.finishWindow();
            HeroRoadManagerment.Instance.clean();
        }, 0.2f));

    }
    //退出竞技场
    public void outArenaBattle()
    {
        //竞技场
        if (BattleManager.battleData == null)
        {
            UiManager.Instance.openMainWindow();
            //异常
            Debug.LogWarning("Arean no battleData ?? why??!!!");
        }
        else
        {
            if (BattleManager.battleData.isArenaMass)
            {
                bool isWin = BattleManager.battleData.winnerID == TeamInfo.OWN_CAMP;
                BattleManager.battleData = null;
                ArenaAuditionsWindow win = UiManager.Instance.getWindow<ArenaAuditionsWindow>();
                if (win != null)
                {
                    win.isWin = isWin ? 0 : 1;
                }
                if (UiManager.Instance.emptyWindow != null)
                {
                    UiManager.Instance.emptyWindow.finishWindow();
                }
                else
                {
                    UiManager.Instance.openMainWindow();
                }
            }
            else if (BattleManager.battleData.isArenaFinal)
            {
                BattleManager.battleData = null;
                //				MissionInfoManager.Instance.clearMission (); 
                //				UiManager.Instance.clearWindows ();
                UiManager.Instance.switchWindow<ArenaFinalWindow>();
            }
        }
    }
    void backtoTowerWindow()
    {
        MissionInfoManager.Instance.clearMission();
        FuBenInfoFPort port = FPortManager.Instance.getFPort("FuBenInfoFPort") as FuBenInfoFPort;
        port.info(intoTowerFuben, ChapterType.TOWER_FUBEN);
    }
    void intoTowerFuben()
    {
        //添加过程记录
        if (FuBenManagerment.Instance.getTowerChapter() == null) return;
        FuBenManagerment.Instance.selectedChapterSid = FuBenManagerment.Instance.getTowerChapter().sid;//爬塔副本章节sid
        FuBenManagerment.Instance.selectedMapSid = 1;
        UiManager.Instance.BackToWindow<ClmbTowerChooseWindow>();
    }
    //退出副本 
    public void outMission()
    {
        MaskWindow.UnlockUI();
        //指定等级，指定副本并且最后一个副本进度符合要求才会检测
        //出副本的时候发现步骤还没到，就跳到指定步骤
        //韬哥需要严格控制新手期间的等级关系，不然这里无法产生作用。
        if (UserManager.Instance.self.getUserLevel() <= 5 &&
            MissionInfoManager.Instance != null &&
            MissionInfoManager.Instance.mission.getChapterType() == ChapterType.STORY &&
            !GameManager.Instance.skipGuide)
        {
            if (UserManager.Instance.self.getUserLevel() == 2 &&
                FuBenManagerment.Instance.getLastStoryMissionSid() == GuideGlobal.FIRST_MISSION_SID &&
                MissionInfoManager.Instance.mission.sid == GuideGlobal.FIRST_MISSION_SID)
            {
                if (GuideManager.Instance.isLessThanStep(7001000))
                {
                    GuideManager.Instance.setStep(7001000);
                }
                else if (GuideManager.Instance.isMoreThanStep(7001000))
                {
                    GuideManager.Instance.setStep(7001000);
                }
            }
            else if (UserManager.Instance.self.getUserLevel() == 3 &&
                     FuBenManagerment.Instance.getLastStoryMissionSid() == GuideGlobal.SECOND_MISSION_SID &&
                     MissionInfoManager.Instance.mission.sid == GuideGlobal.SECOND_MISSION_SID)
            {
                if (GuideManager.Instance.isLessThanStep(12001000))
                {
                    GuideManager.Instance.setStep(12001000);
                }
                else if (GuideManager.Instance.isMoreThanStep(12001000))
                {
                    GuideManager.Instance.setStep(12001000);
                }
            }
            else if (UserManager.Instance.self.getUserLevel() == 4 &&
                     FuBenManagerment.Instance.getLastStoryMissionSid() == GuideGlobal.THREE_MISSION_SID &&
                     MissionInfoManager.Instance.mission.sid == GuideGlobal.THREE_MISSION_SID)
            {
                if (GuideManager.Instance.isLessThanStep(16001000))
                {
                    GuideManager.Instance.setStep(16001000);
                }
                else if (GuideManager.Instance.isMoreThanStep(16001000))
                {
                    GuideManager.Instance.setStep(16001000);
                }
            }
            else if (UserManager.Instance.self.getUserLevel() == 5 &&
                     FuBenManagerment.Instance.getLastStoryMissionSid() == GuideGlobal.FOUR_MISSION_SID &&
                     MissionInfoManager.Instance.mission.sid == GuideGlobal.FOUR_MISSION_SID)
            {
                if (GuideManager.Instance.isLessThanStep(23001000))
                {
                    GuideManager.Instance.setStep(23001000);
                }
            }
        }
        int type = MissionInfoManager.Instance.mission.getChapterType();
        int sid = MissionInfoManager.Instance.mission.getChapterSid();
        if (type == ChapterType.TOWER_FUBEN)
        {//如果是爬塔副本中途退出就直接返回到主界面
            backtoTowerWindow();
            return;

        }
        //等级引导跳转
        if (!GuideManager.Instance.isGuideComplete())
        {
            MaskWindow.LockUI();
            UiManager.Instance.initNewPlayerGuideLayer();
            GuideManager.Instance.openGuideMask();
            StartCoroutine(delayOpenMainWindow());
            MissionInfoManager.Instance.clearMission();
            HeroRoadManagerment.Instance.clean();
            isFormMissionByGuide = true;
            return;
        }
        //强制引导跳转
        else if (GuideManager.Instance.isEqualStep(12001000) || GuideManager.Instance.isEqualStep(16001000)
            || GuideManager.Instance.isEqualStep(7001000) || GuideManager.Instance.isEqualStep(23001000))
        {
            MaskWindow.LockUI();
            UiManager.Instance.initNewPlayerGuideLayer();
            GuideManager.Instance.openGuideMask();
            StartCoroutine(delayOpenMainWindow());
            MissionInfoManager.Instance.clearMission();
            HeroRoadManagerment.Instance.clean();
            isFormMissionByGuide = true;
            return;
        }
        //友善指引出現了
        else if (GuideManager.Instance.isHaveNewFriendlyGuide() != 0)
        {
            MaskWindow.LockUI();
            UiManager.Instance.initNewPlayerGuideLayer();
            GuideManager.Instance.openGuideMask();
            StartCoroutine(delayOpenMainWindow());
            MissionInfoManager.Instance.clearMission();
            HeroRoadManagerment.Instance.clean();
            isFormMissionByGuide = true;
            return;
        }


        //讨伐
        if (type == ChapterType.WAR)
        {
            MissionInfoManager.Instance.clearMission();
            if (UiManager.Instance.getWindow<WarChooseWindow>() == null)
                UiManager.Instance.openMainWindow();
            else
                UiManager.Instance.BackToWindow<WarChooseWindow>();
            if (UiManager.Instance.missionMainWindow != null)
                UiManager.Instance.missionMainWindow.destoryWindow();
        }
        else if (type == ChapterType.HERO_ROAD)
        {
            //英雄之章副本退出和英雄之章战斗退出一样
            outHeroRoadBattle();
        }
        //修炼
        else if (type == ChapterType.PRACTICE)
        {
            MaskWindow.LockUI();
            StartCoroutine(delayOpenMainWindow());
            MissionInfoManager.Instance.clearMission();
            HeroRoadManagerment.Instance.clean();
        }
        //剧情副本
        //如果是最后一个关卡,直接开章节选择界面;
        else if (type == ChapterType.STORY)
        {
            int[] missionList = FuBenManagerment.Instance.getAllShowMissions(sid);
            if (MissionInfoManager.Instance.mission.sid == missionList[missionList.Length - 1] && MissionInfoManager.Instance.mission.isFirstComplete == true)
            {
                //杨大侠说第一章结束后要回主界面
                if (MissionInfoManager.Instance.mission.sid == 41010)
                {
                    MaskWindow.LockUI();
                    UiManager.Instance.initNewPlayerGuideLayer();
                    GuideManager.Instance.openGuideMask();
                    StartCoroutine(delayOpenMainWindow());
                    MissionInfoManager.Instance.clearMission();
                    HeroRoadManagerment.Instance.clean();
                    return;
                }
                //很特殊的操作,破坏了逻辑结构,一般不推荐这么搞
                if (UiManager.Instance.storyMissionWindow != null)
                    UiManager.Instance.storyMissionWindow.destoryWindow();
            }
            MissionInfoManager.Instance.clearMission();
            UiManager.Instance.emptyWindow.finishWindow();
        }
        //活动副本
        else if (type == ChapterType.ACTIVITY_CARD_EXP || type == ChapterType.ACTIVITY_EQUIP_EXP
            || type == ChapterType.ACTIVITY_MONEY || type == ChapterType.ACTIVITY_SKILL_EXP)
        {
            MissionInfoManager.Instance.clearMission();
            UiManager.Instance.emptyWindow.finishWindow();
        }
    }

    //保存副本 
    public void saveMission()
    {
        //保存副本直接回主界面不解释
        UiManager.Instance.openMainWindow();
        MissionInfoManager.Instance.clearMission();
        GuideManager.Instance.guideEvent();

    }

    public void openLoginWindow()
    {
        if (!ServerManagerment.Instance.isLogin)
        {
            //没登陆，显示登陆i画面
            UiManager.Instance.openWindow<LoginWindow>();
            GameManager.Instance.netConnectInit();
        }
        else
        {

            UiManager.Instance.openMainWindow();
        }
    }

    //网络连接丢失时调用,boo决定你是否可以重连
    public void OnLostConnect(bool bl)
    {
        if (MiniConnectManager.IsRobot || !ServerManagerment.Instance.lastServer.isRunning)
            return;
        if (bl)
        {
            //如果还没登录
            if (UserManager.Instance.self == null || !ServerManagerment.Instance.isLogin)
            {
                MaskWindow.UnlockUI(true);
                if (GuideManager.Instance.guideUI != null)
                {
                    GuideManager.Instance.guideUI.gameObject.SetActive(false);
                }
                if (LanguageConfigManager.Instance.getLanguage("isOpenClearConnectsTest") == "1")
                    ConnectManager.manager().closeAllConnects();
                SystemMessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("s0105"), (msg) =>
                {
                    UiManager.Instance.clearWindowsName("LoginWindow");
                    LoginWindow win = UiManager.Instance.getWindow<LoginWindow>();
                    if (win != null && !win.gameObject.activeSelf)
                    {
                        win.restoreWindow();
                    }
                });
                return;
            }
            if (GuideManager.Instance.isLessThanStep(50001000))
            {
                MaskWindow.UnlockUI(true);
                if (GuideManager.Instance.guideUI != null)
                {
                    GuideManager.Instance.guideUI.gameObject.SetActive(false);
                }
                if (LanguageConfigManager.Instance.getLanguage("isOpenClearConnectsTest") == "1")
                    ConnectManager.manager().closeAllConnects();
                SystemMessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("s0105"), (msg) =>
                {
                    logOut();
                });
                return;
            }
            //已经重连提示就返回
            if (startShowAlert)
                return;
            if (startResumeConnect)
            {
                //如果正在重连,那么说明重连失败
                startResumeConnect = false;
                startShowAlert = true;
                ConnectManager.manager().closeAllConnects();
                StartCoroutine(Utils.DelayRunNextFrame(() =>
                {
                    //弹框提示还重试不
                    SystemMessageWindow.ShowConfirm(LanguageConfigManager.Instance.getLanguage("GameManager_lostConnect"), (msg) =>
                    {
                        startResumeConnect = false;
                        startShowAlert = false;
                        if (msg.msgEvent == msg_event.dialogOK)
                        {
                            OnLostConnect(bl);
                        }
                        else
                        {
                            logOut();

                        }
                    });
                }));
            }
            else
            {
                //如果不在重连中,直接开始默认重连
                startResumeConnect = true;
                startShowAlert = false;
                ConnectManager.manager().closeAllConnects();
                MaskWindow.NetLock();
                //				Debug.LogWarning("OnLostConnect   begin: "+ServerManagerment.Instance.lastServer.domainName);
                ConnectManager.manager().beginConnect(ServerManagerment.Instance.lastServer.domainName, ServerManagerment.Instance.lastServer.port, resumeConnectOK);
            }
        }
        else
        {
            logOut();
        }
    }
    //重连成功后登录
    private void resumeConnectOK()
    {
        LoginFPort port = FPortManager.Instance.getFPort("LoginFPort") as LoginFPort;
        if (SdkManager.INSTANCE.IS_SDK)
        {
            port.login(SdkManager.URL, StringKit.toInt(ServerManagerment.Instance.lastServer.sid), resumeLoginBack);
        }
        else
        {
            port.login("1", "1", ServerManagerment.Instance.userName, resumeLoginBack);
        }


    }
    //登录成功后重新获取所有数据
    private void resumeLoginBack()
    {
        UiManager.Instance.destoryWindowByName("MessageWindow");
        UserManager.Instance.initUser();
    }

    public void ResumeConnectFinish()
    {
        MaskWindow.UnlockUI(true);
        startResumeConnect = false;
        if (UiManager.Instance != null)
        {
            UiManager.Instance.OnNetResume();
        }
        //这里控制新手引导期间重连的情况，一部分是强制引导，一部分是等级引导
        if (GuideManager.Instance.isMoreThanStep(50001000))
        {
            if (GuideManager.Instance.guideUI != null)
            {
                GuideManager.Instance.guideUI.closeGuide();
            }
        }
        else
        {
            if (!GuideManager.Instance.isGuideComplete() && GuideManager.Instance.guideUI != null)
            {
                GuideManager.Instance.guideUI.gameObject.SetActive(true);
                GuideManager.Instance.guideEvent();
            }
        }
    }

    private void OnApplicationResume()
    {
        //		if (ServerManagerment.Instance.lastServer != null && UserManager.Instance.self != null) {
        //			ConnectManager.manager ().ping ();
        //		}
    }

    private List<string> getAwardMessageInf0o(Award award)
    {
        List<string> tempStringList = new List<string>();
        if (award == null) return null;
        if (award.exps != null) //经验
        {
            for (int i = 0; i < award.exps.Count; i++)
            {
                Card targetCard = award.exps[i].cardLevelUpData.levelInfo.orgData as Card;
                tempStringList.Add(LanguageConfigManager.Instance.getLanguage("Award_exp_gave",
                    targetCard.getName(), award.exps[i].expGap + ""));
            }
        }
        if (award.expGap > 0)
            tempStringList.Add(LanguageConfigManager.Instance.getLanguage("Award_exp_gave1",
                UserManager.Instance.self.nickname, award.expGap + ""));
        if (award.moneyGap > 0)
            tempStringList.Add(LanguageConfigManager.Instance.getLanguage("Award_exp_gave2", award.moneyGap + ""));
        if (award.rmbGap > 0)
            tempStringList.Add(LanguageConfigManager.Instance.getLanguage("Award_exp_gave3", award.rmbGap + ""));
        if (award.honorGap > 0)
            tempStringList.Add(LanguageConfigManager.Instance.getLanguage("Award_exp_gave4", award.honorGap + ""));
        if (award.integralGap > 0)
            tempStringList.Add(LanguageConfigManager.Instance.getLanguage("Award_exp_gave5", award.integralGap + ""));
        if (award.meritGap > 0)
            tempStringList.Add(LanguageConfigManager.Instance.getLanguage("Award_exp_gave6", award.meritGap + ""));
        if (award.starGap > 0)
            tempStringList.Add(LanguageConfigManager.Instance.getLanguage("Award_exp_gave7", award.starGap + ""));
        if (award.godsWar_integralGap > 0)
            tempStringList.Add(LanguageConfigManager.Instance.getLanguage("Award_exp_gave8",
                award.godsWar_integralGap + ""));
        if (award.luckyStarGap > 0)
            tempStringList.Add(LanguageConfigManager.Instance.getLanguage("Award_exp_gave9",
                award.luckyStarGap + ""));
        if (award.props != null)
        {
            for (int i = 0; i < award.props.Count; i++)
            {
                tempStringList.Add(LanguageConfigManager.Instance.getLanguage("Award_exp_gave10",
                    PropSampleManager.Instance.getPropSampleBySid(award.props[i].sid).name, award.props[i].num + ""));
            }
        }
        if (award.equips != null)
        {
            for (int i = 0; i < award.equips.Count; i++)
            {
                tempStringList.Add(LanguageConfigManager.Instance.getLanguage("Award_exp_gave11",
                    EquipmentSampleManager.Instance.getEquipSampleBySid(award.equips[i].sid).name,
                    award.equips[i].num + ""));
            }
        }
        if (award.magicWeapons != null)
        {
            for (int i = 0; i < award.magicWeapons.Count; i++)
            {
                tempStringList.Add(LanguageConfigManager.Instance.getLanguage("Award_exp_gave12",
                    MagicWeaponSampleManager.Instance.getMagicWeaponSampleBySid(award.magicWeapons[i].sid).name,
                    award.magicWeapons[i].num + ""));
            }
        }
        if (award.starsouls != null)
        {
            for (int i = 0; i < award.starsouls.Count; i++)
            {
                tempStringList.Add(LanguageConfigManager.Instance.getLanguage("Award_exp_gave13",
                    StarSoulSampleManager.Instance.getStarSoulSampleBySid(award.starsouls[i].sid).name, "1"));
            }
        }
        if (award.cards != null)
        {
            for (int i = 0; i < award.cards.Count; i++)
            {
                tempStringList.Add(LanguageConfigManager.Instance.getLanguage("Award_exp_gave14",
                    CardSampleManager.Instance.getRoleSampleBySid(award.cards[i].sid).name, award.cards[i].num + ""));
            }
        }
        return tempStringList;
    }
    private List<PrizeSample> getAwardMessageInfo(Award award)
    {
        List<PrizeSample> temPrizeSamples = new List<PrizeSample>();
        if (award == null) return null;
        string careDec = "";
        string beastDec = "";
        // CharacterData tempGuardianForce =;//召唤兽
        int cardExp = 0;
        int beastExp = 0;
        if (award.exps != null)
        {
            if (BattleManager.battleData.playerTeamInfo.guardianForce != null)
            {
                for (int i = 0; i < award.exps.Count; i++)
                {
                    if (BattleManager.battleData.playerTeamInfo.guardianForce.uid == award.exps[i].id)
                    {
                        BeastEvolve tmp =
                            BeastEvolveManagerment.Instance.getBeastEvolveBySid(
                                BattleManager.battleData.playerTeamInfo.guardianForce.sid);
                        if (!tmp.getBeast().isMaxLevel())
                            beastExp = award.exps[i].expGap;
                    }
                    else if (cardExp == 0 && award.exps[i].expGap > 0)
                    {
                        cardExp = award.exps[i].expGap;
                    }
                }
            }
            else
            {
                for (int i = 0; i < award.exps.Count; i++)
                {
                    if (cardExp == 0 && award.exps[i].expGap > 0) cardExp = award.exps[i].expGap;
                }

            }
        }
        if (cardExp != 0)
        {


            float expAdd = 0;
            expAdd += GuildManagerment.Instance.getSkillAddExpPorCardPve() * 0.01f;
            if (UserManager.Instance.self.getVipLevel() > 0)
                expAdd +=
                    VipManagerment.Instance.getVipbyLevel(UserManager.Instance.self.getVipLevel()).privilege.expAdd *
                    0.0001f;
            if (ServerTimeKit.getSecondTime() < BackPrizeLoginInfo.Instance.endTimes) // 双倍经验期间//
            {
                careDec = LanguageConfigManager.Instance.getLanguage("Award_exp_gavee", cardExp + "",
                    2 + expAdd + "");
                //LanguageConfigManager.Instance.getLanguage("EXPADD") + (2 + expAdd);
            }
            else
            {
                if (expAdd == 0)
                {
                    careDec = LanguageConfigManager.Instance.getLanguage("Award_exp_gave", cardExp + "");
                }
                else
                {
                    careDec = LanguageConfigManager.Instance.getLanguage("Award_exp_gavee", cardExp + "",
                        1 + expAdd + "");
                }
            }
        }
        if (beastExp != 0)
        {
            float expA = 0;
            expA += GuildManagerment.Instance.getSkillAddExpPorBeastPve() * 0.01f;
            if (UserManager.Instance.self.getVipLevel() > 0)
                expA = VipManagerment.Instance.getVipbyLevel(UserManager.Instance.self.getVipLevel()).privilege.expAdd *
                    0.0001f;
            if (ServerTimeKit.getSecondTime() < BackPrizeLoginInfo.Instance.endTimes) // 双倍经验期间//
            {
                beastDec = LanguageConfigManager.Instance.getLanguage("Award_exp_gavee_nv1", beastExp + "",
                2 + expA + "");
                //LanguageConfigManager.Instance.getLanguage("EXPADD") + (2 + expAdd);
            }
            else
            {
                if (expA == 0)
                {
                    beastDec = LanguageConfigManager.Instance.getLanguage("Award_exp_gavee_nv", beastExp + "");
                }
                else
                {
                    beastDec = LanguageConfigManager.Instance.getLanguage("Award_exp_gavee_nv1", beastExp + "",
                        1 + expA + "");
                }
            }
        }
        if (careDec != "") temPrizeSamples.Add(new PrizeSample(-1, careDec));
        if (beastDec != "") temPrizeSamples.Add(new PrizeSample(-1, beastDec));
        if (award.moneyGap > 0) temPrizeSamples.Add(new PrizeSample(1, 0, award.moneyGap));
        if (award.rmbGap > 0) temPrizeSamples.Add(new PrizeSample(2, 0, award.rmbGap));
        if (award.honorGap > 0) temPrizeSamples.Add(new PrizeSample(10, 0, award.honorGap));
        // if (award.integralGap > 0)
        if (award.meritGap > 0) temPrizeSamples.Add(new PrizeSample(13, 0, award.meritGap));
        // if (award.starGap > 0) 
        // if (award.godsWar_integralGap > 0)
        // if (award.luckyStarGap > 0)
        if (award.props != null)
        {
            for (int i = 0; i < award.props.Count; i++)
            {
                temPrizeSamples.Add(new PrizeSample(3, award.props[i].sid, award.props[i].num));
            }
        }
        if (award.equips != null)
        {
            for (int i = 0; i < award.equips.Count; i++)
            {
                temPrizeSamples.Add(new PrizeSample(4, award.equips[i].sid, award.equips[i].num));
            }
        }
        if (award.magicWeapons != null)
        {
            for (int i = 0; i < award.magicWeapons.Count; i++)
            {
                temPrizeSamples.Add(new PrizeSample(21, award.magicWeapons[i].sid, award.magicWeapons[i].num));
            }
        }
        if (award.starsouls != null)
        {
            for (int i = 0; i < award.starsouls.Count; i++)
            {
                temPrizeSamples.Add(new PrizeSample(15, award.starsouls[i].sid, 1));
            }
        }
        if (award.cards != null)
        {
            for (int i = 0; i < award.cards.Count; i++)
            {
                temPrizeSamples.Add(new PrizeSample(5, award.cards[i].sid, award.cards[i].num));
            }
        }
        return temPrizeSamples;
    }

    struct AssetItem
    {
        public string p;
        public ulong c;

        public AssetItem(string p, ulong c)
        {
            this.p = p;
            this.c = c;
        }
    }
}