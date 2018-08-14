using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiniConnectItem : MonoBehaviour
{


    public UILabel lblId;
    public UILabel lblState;

	ConnectManager cm;
	string server;
	int port;
    int missionSid; //下个副本
    int userId;

    public User user;

	public void Init (string server, int port,int userId)
	{
		this.server = server;
		this.port = port;
        this.userId = userId;

        lblId.text = userId.ToString();
	}

	void Start ()
	{
		cm = new ConnectManager ();
		cm.init (gameObject);
		cm.beginConnect (server, port, excute);
	}
	
	void Update ()
	{

	}

    private void log(string str)
    {
        lblState.text = str;
    }

	void excute()
	{
        //test();
        p1();
	}

	void p1()
    {
        log("开始登录");
		MiniLoginFPort p1 = new MiniLoginFPort ();
		p1.cm = this.cm;
		p1.login ("1", "1", gameObject.name, p2);
	}

    public static long st;
    void test()
    {
		st = ServerTimeKit.getMillisTime();
        MiniFport.access(cm, "/yxzh/test", (msg) => {
            //Debug.LogError(TimeKit.getMillisTime() - st);
            test(); 
        });
    }

	void p2(string str)
	{

        if (str == FPortGlobal.LOGIN_LOGIN_OK || str == FPortGlobal.LOGIN_RELOGIN_OK)
        {
			userNewFPort();
        }  else if (str == FPortGlobal.LOGIN_NO_ROLE)
        {
            MiniCreateRoleFPort p2 = new MiniCreateRoleFPort();
            p2.cm = this.cm;
			p2.access(gameObject.name, 2, 2, 2, userNewFPort);
        }
	}

	void userNewFPort(){
		MiniUserNewFPort p3 = new MiniUserNewFPort ();
		p3.cm = this.cm;
		p3.access (loginBack);
	}

	void p3()
	{
		MiniUserFPort p3 = new MiniUserFPort ();
		p3.cm = this.cm;
		p3.access (p4);
	}
	void p4()
	{
		MiniStorageFPort p4 = new MiniStorageFPort ();
		p4.cm = this.cm;
		p4.init (p5);
	}
	void p5()
	{
		MiniArmyGetFPort p5 = new MiniArmyGetFPort ();
		p5.cm = this.cm;
		p5.access (p6);
	}
	void p6()
	{
		MiniTotalLoginFPort p6 = new MiniTotalLoginFPort ();
		p6.cm = this.cm;
		p6.access (p7);
	}
	void p7()
	{
		MiniMailGetFPort p7 = new MiniMailGetFPort ();
		p7.cm = this.cm;
		p7.access (p9);
	}
	void p9()
	{
		MiniInitTaskFPort p9 = new MiniInitTaskFPort ();
		p9.cm = this.cm;
        p9.access (p11);
	}

	void p11()
	{
		MiniPvpGetInfoFPort p11 = new MiniPvpGetInfoFPort ();
		p11.cm = this.cm;
		p11.access (p12);
	}
	void p12()
	{
		MiniExchangeInfoFPort p12 = new MiniExchangeInfoFPort ();
		p12.cm = this.cm;
		p12.initInfo (p13);
	}
	void p13()
	{
		MiniInitLuckyDrawFPort p13 = new MiniInitLuckyDrawFPort ();
		p13.cm = this.cm;
		p13.init (p14);
	}
	void p14()
    {
		MiniFuBenInfoFPort p14 = new MiniFuBenInfoFPort ();
		p14.cm = this.cm;
		p14.info (p15,ChapterType.STORY);
	}

	void p15()
    {
		MiniFuBenInfoFPort p15 = new MiniFuBenInfoFPort ();
		p15.cm = this.cm;
        p15.info (p17,ChapterType.WAR);
	}
	void p16()
    {
		MiniGuideGetInfoFPort p16 = new MiniGuideGetInfoFPort ();
		p16.cm = this.cm;
		p16.getInfo (p17);
	}
	void p17()
	{
		MiniPyxFPort p17 = new MiniPyxFPort ();
		p17.cm = this.cm;
		p17.pyxInfo(p18);
	}
	void p18()
	{
		MiniBeastAddInfoFPort p18 = new MiniBeastAddInfoFPort ();
		p18.cm = this.cm;
		p18.getInfo(p19);
	}
	void p19()
	{
		MiniFriendsFPort p19 = new MiniFriendsFPort ();
		p19.cm = this.cm;
		p19.initFriendsInfo(p20);
	}
	void p20()
	{
		MiniGoddessAstrolabeFPort p20 = new MiniGoddessAstrolabeFPort ();
		p20.cm = this.cm;
		p20.getInfo(p21);
	}
	void p21()
	{
		MiniGetStarInfoFPort p21 = new MiniGetStarInfoFPort ();
		p21.cm = this.cm;
		p21.access(p22);
	}
	void p22()
	{
		MiniFriendsShareFPort p22 = new MiniFriendsShareFPort ();
		p22.cm = this.cm;
		p22.initShareInfo(p23);
	}
	void p23()
	{
		MiniHeroRoadFPort p23 = new MiniHeroRoadFPort ();
		p23.cm = this.cm;
        p23.getRoadActivation(p25);
	}
	void p24()
	{
		MiniArenaGetStateFPort p22 = new MiniArenaGetStateFPort ();
		p22.cm = this.cm;
		p22.access(p23);
	}
	void p25()
	{
		MiniGuildGetInfoFPort p25 = new MiniGuildGetInfoFPort ();
		p25.cm = this.cm;
		p25.access(p26);
	}
	void p26()
	{
		MiniGuildGetApplyListFPort p26 = new MiniGuildGetApplyListFPort ();
		p26.cm = this.cm;
		p26.access(p27);
	}
	void p27()
	{
		MiniDivineGetInfoFPort p27 = new MiniDivineGetInfoFPort ();
		p27.cm = this.cm;
		p27.access(p28);
	}
	void p28()
	{
		MiniNoticeGetTimeFPort p28 = new MiniNoticeGetTimeFPort ();
		p28.cm = this.cm;
		p28.access(p30);
	}

//	void p29()
//	{
//		StringKit.toInt ((getKVMsg (arr.Value [i++] as ErlArray).getValue ("msg") as ErlType).getValueString ());
//	}

	void p30()
	{
		MiniNoticetHeroEatInfoFPort p30 = new MiniNoticetHeroEatInfoFPort ();
		p30.cm = this.cm;
		p30.access(loginBack);
	}

	void loginBack ()
	{
        log("获取用户数据完成");

        missionSid = 41001;
        fuben();
	}

    void fuben()
    {
        //MiniFport.access(cm,new ErlKVMessage (FrontPort.FUBEN_GET_CURRENT),getCurrentFuben);
        //放弃副本
        MiniFport.access(cm, FrontPort.FUBEN_ABANDON, (msg2) => {
            log("进入副本");
            //进入副本
            MiniFuBenIntoFPort f = new MiniFuBenIntoFPort();
            f.cm = cm;
            f.intoFuben(missionSid++, 1, IntoFubenBack);
        });
    }

	void fuben(int sid)
	{
		//MiniFport.access(cm,new ErlKVMessage (FrontPort.FUBEN_GET_CURRENT),getCurrentFuben);
		//放弃副本
		MiniFport.access(cm, FrontPort.FUBEN_ABANDON, (msg2) => {
			log("进入副本");
			//进入副本
			MiniFuBenIntoFPort f = new MiniFuBenIntoFPort();
			f.cm = cm;
			f.intoFuben(sid, 1, IntoFubenBack);
		});
	}

    void getCurrentFuben(ErlKVMessage msg)
    {
        ErlType type = msg.getValue ("sid") as ErlType;
        int sid = StringKit.toInt (type.getValueString ());
        if (sid > 0)
        {
            //放弃副本
            MiniFport.access(cm, FrontPort.FUBEN_ABANDON, (msg2) => {
                log("进入副本");
                //进入副本
                MiniFuBenIntoFPort f = new MiniFuBenIntoFPort();
                f.cm = cm;
                f.intoFuben(missionSid = sid, 1, IntoFubenBack);
            });
        } else
        {
            log("进入副本");
            //进入副本
            MiniFuBenIntoFPort f = new MiniFuBenIntoFPort();
            f.cm = cm;
            f.intoFuben(missionSid++, 1, IntoFubenBack);
        }
    }
    
    void IntoFubenBack(int sid)
    {
        if (sid >= 0)
        {
            //if(missionSid == 41010)
              //  return;
			lblId.text = userId + " - " + sid;
            //missionSid = Random.Range(41032,41100);
            FubenStart();
        } else
        {
            log("进入副本失败:"+missionSid);
        }
    }
    
    int pointIndex;
    void FubenStart()
    {
        pointIndex = 0;
        FubenGotoNext();
    }

    void FubenGotoNext()
    {
        log("往下走");
        pointIndex++;
		MiniFport.access (cm, FrontPort.FUBEN_GET_CURRENT, (msg11) => {
			int curSid = StringKit.toInt((msg11.getValue("sid") as ErlType).getValueString());
			if(curSid == 0)
				fuben();
			else{
				MiniFport.access(cm, FrontPort.FUBEN_GOTO, (msg22) => {
					ErlType rt = msg22.getValue("msg") as ErlType;
					string str = rt != null ? rt.getValueString() : "";
					if(str == "no_fb")
					{
						fuben(missionSid);
					}
					else if(str == "not_goto" || str == "goto_point")
					{
						//FubenGetEventInfo();
						StartCoroutine(Utils.DelayRun(FubenGetEventInfo,Random.Range(5,8)));
					}
					else
					{
						fuben(missionSid - 1 > 41001?missionSid - 1:41001);
						Debug.LogError("走点返回异常:"+msg22.toJsonString());
					}
				});
			}
		});
    }

    void FubenDoEvent()
    {
        log("执行点位事件");
        MiniFport.access(cm, FrontPort.FUBEN_EXECUTE_EVENT, (msg) => { 
            FubenGotoNext();
        });
    }

    void FubenGetEventInfo()
    {
        log("获取点位信息");

        MiniFport.access(cm, FrontPort.FUBEN_GET_EVENT_INFO, (msg) => {
            FubenDoEvent();
        });

    }

    void OnApplicationQuit()
    {
        cm.closeAllConnects();
    }

}
