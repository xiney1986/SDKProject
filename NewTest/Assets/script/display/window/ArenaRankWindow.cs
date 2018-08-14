using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArenaRankWindow : WindowBase 
{
    public TapContentBase tapContent;
    public RankContent content;
    public UILabel lblMyRank;
    public ButtonBase btnRefresh;
    public UILabel lblRefreshCD;
    
    int myRank;
    private static int nextRefreshTime;
    float refreshCD;
    bool startCD;
    int currTeam;
    int tapIndex;
    private bool dragSwitch;
    List<RankItemMoney>[] data = new List<RankItemMoney>[5];


	protected override void DoEnable ()
	{
		base.DoEnable ();
		UiManager.Instance.backGround.switchBackGround ("ChouJiang_BeiJing");
		//UiManager.Instance.backGroundWindow.switchToDark();
	}

    protected override void begin ()
    {

        if (!isAwakeformHide) { 
            int index = ArenaManager.instance.self.team - 1;
            tapContent.changeTapPage(tapContent.tapButtonList [index],true);

			refreshCD = (int)(nextRefreshTime - ServerTimeKit.getSecondTime());
            if(refreshCD > 0)
            {
                btnRefresh.disableButton(true);
                startCD = true;
            }
        }
		MaskWindow.UnlockUI ();
    }

    void Update()
    {
        if (startCD)
        {
            refreshCD -= Time.deltaTime;
            lblRefreshCD.text = ((int)refreshCD).ToString();
            if (refreshCD <= 0)
            {
                startCD = false;
                lblRefreshCD.text = "";
                btnRefresh.disableButton(false);
            } 
        }
    }

    void loadData(int team)
    {
        currTeam = team;
        int index = team - 1;
        if (data [index] != null)
        {
            OnLoadDataResault(data [index]);
        } else
        {
            FPortManager.Instance.getFPort<ArenaRankFPort>().access(OnLoadDataResault, team, 100);
        }
    }

    void OnLoadDataResault(List<RankItemMoney> list)
    {
        data [currTeam - 1] = list;
        myRank = 0;
        for (int i = 0; i < list.Count; i++)
        {
            if(list[i].uid == UserManager.Instance.self.uid)
            {
                myRank = i + 1;
                break;
            }
        }
        
        ArenaUserInfo self = ArenaManager.instance.self;
        if (currTeam == self.team)
        {
            lblMyRank.text = LanguageConfigManager.Instance.getLanguage("Arena45") + "  ";
            int rank = myRank > 0 ? myRank : self.rank;
            if (rank > 100)
                lblMyRank.text += LanguageConfigManager.Instance.getLanguage("Arena41", ArenaManager.instance.getTeamNameById(self.team));
            else
                lblMyRank.text += LanguageConfigManager.Instance.getLanguage("Arena42",ArenaManager.instance.getTeamNameById(self.team), rank.ToString());
        }
        content.init(RankManagerment.TYPE_MONEY,list,this);
    }
    
    public override void tapButtonEventBase (GameObject gameObj, bool enable)
    {
        if (!enable)
            return;
        int team = StringKit.toInt(gameObj.name);
        tapIndex = team - 1;
        loadData(team);
    }
    
    public override void buttonEventBase (GameObject gameObj)
    {
		base.buttonEventBase (gameObj);
        if (gameObj.name == "close")
        {
            finishWindow();
        } else if (gameObj.name == "buttonRefresh")
        {
            data = new List<RankItemMoney>[5];
            btnRefresh.disableButton(true);
            startCD = true;
            refreshCD = 30;
			nextRefreshTime = ServerTimeKit.getSecondTime() + (int)refreshCD;
            loadData(currTeam);
			MaskWindow.UnlockUI();
        }
    }
    
    /// <summary>
    /// 获取当前显示的排行榜中我的排名
    /// </summary>
    public int getMyRankWithShow()
    {
        return myRank;
    }

    public void OnDrag (Vector2 delta)
    {
        if (dragSwitch)
            return;
        int toIndex = tapIndex;
        if (delta.x < -40)
            toIndex++;
        else if (delta.x > 40)
            toIndex--;
        if (toIndex != tapIndex && toIndex >= 0 && toIndex < tapContent.tapButtonList.Length)
        {
            tapContent.changeTapPage(tapContent.tapButtonList[toIndex],true);
            
            dragSwitch = true;
            StartCoroutine(Utils.DelayRun(()=>{
                dragSwitch = false;
            },0.5f));
        }
    }
}
