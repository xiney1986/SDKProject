using UnityEngine;
using System.Collections;

public class ArenaFinalPoint : MonoBehaviour {
    public ArenaFinalWindow window;
    public Transform roleLocation;
    public GameObject buttonReplay;
    public GameObject buttonGuess;
    public UILabel lblOpenTime;
    public UILabel lblRoleName;
    public GameObject findInfo;
    public GameObject lineLocaltion1;
    public GameObject lineLocaltion2;
    public GameObject lineLocaltion11;
    public GameObject lineLocaltion22;
    public GameObject guessNum;
	public UILabel chanName;
	public UILabel timecd;
	public GameObject nextTime;

    [HideInInspector] public ArenaFinalPoint parentPoint1;
    [HideInInspector] public ArenaFinalPoint parentPoint2;
    [HideInInspector] public ArenaFinalPoint nextPoint;
    [HideInInspector] public GameObject point3d;
    [HideInInspector] public GameObject role3d;

    [HideInInspector] public GameObject lineLeft;
    [HideInInspector] public GameObject lineRight;
    [HideInInspector] public GameObject lineConnect;
    public ArenaFinalInfo info;
    Vector3 roleOffset = new Vector3(0,0.08f,0);
    ArenaUserInfo user;
    float _updateInfoDelay;
	private CallBack callback;
    public void init(ArenaFinalInfo info)
    {
        this.info = info;
		window.map.addPoint (gameObject, info.finalState == ArenaManager.STATE_RESET,((obj)=>{
			point3d=obj;
		}));
        if (string.IsNullOrEmpty(info.userName))
        {
            lblRoleName.transform.parent.gameObject.SetActive(false);
        } else
        {
            lblRoleName.transform.parent.gameObject.SetActive(true);
            lblRoleName.text = info.userName;
            role3d = window.map.addRole(info.userIcon,gameObject);
        }
    }

    public void initParent(ArenaFinalPoint parentPoint1,ArenaFinalPoint parentPoint2)
    { 
        this.parentPoint1 = parentPoint1;
        this.parentPoint2 = parentPoint2;
        parentPoint1.nextPoint = this;
        parentPoint2.nextPoint = this;

        string startTime = TimeKit.getDateTime(ParentPoint.info.startTime).ToString("MM-dd HH:mm");
        string guessTime = TimeKit.getDateTime(ParentPoint.info.guessStartTime).ToString("MM-dd HH:mm");
        string guessTimeEnd = TimeKit.getDateTime(ParentPoint.info.guessEndTime).ToString("MM-dd HH:mm");
        ///lblRoleName.transform.parent.gameObject.SetActive(true);
        ///lblRoleName.text = info.userName+"\n"+startTime+"\n"+guessTime+"\n"+guessTimeEnd;
        
        parentPoint1.initLine(this);
        parentPoint2.initLine(this);

        bool active = false;
        int now = ServerTimeKit.getSecondTime();
        active = (ParentPoint.info.startTime > 0 && ParentPoint.info.startTime + 60 < now || ArenaManager.instance.state == ArenaManager.STATE_RESET);
        active &= parentPoint1.info.userId != null || parentPoint2.info.userId != null;
        lineLeft = window.map.addLine(point3d,transform.position, lineLocaltion1.transform.position,false,active);

        UpdateInfo();
    }

    public void initLine(ArenaFinalPoint next)
    {
        bool active = false;
        int now = ServerTimeKit.getSecondTime();
        if ((info.startTime > 0 && info.startTime + 60 < now || ArenaManager.instance.state == ArenaManager.STATE_RESET) && info.userId != null)
            active = true;
        lineRight = window.map.addLine(point3d,transform.position, lineLocaltion2.transform.position,false,active);
        lineConnect = window.map.addLine(point3d,lineLocaltion2.transform.position, next.lineLocaltion1.transform.position,true,active);
    }

    public void UpdateInfo()
    {
        lblOpenTime.transform.parent.gameObject.SetActive(false);
        buttonGuess.SetActive(false);
        buttonReplay.SetActive(false);
        info.state = 0;

        int now = ServerTimeKit.getSecondTime(); 
        if (ArenaManager.instance.state == ArenaManager.STATE_RESET || ParentPoint.info.startTime + 60 < now || !string.IsNullOrEmpty(info.userName))
        {
            if(parentPoint1.info.hasUser() || parentPoint2.info.hasUser())
            {
                info.state = ArenaFinalInfo.STATE_REPLAY;                
            }
			//只有2个人时 才显示战报按钮
			if(parentPoint1.info.hasUser() && parentPoint2.info.hasUser())
			{
				buttonReplay.SetActive(true);
			}

        }else if(ParentPoint.info.guessStartTime + 60 <= now && now <= ParentPoint.info.guessEndTime)
        {
            if(parentPoint1.info.hasUser() || parentPoint2.info.hasUser())
            {
                info.state = ArenaFinalInfo.STATE_GUESS;
                buttonGuess.SetActive(true);
				guessNum.SetActive(!parentPoint1.info.guessed&&!parentPoint2.info.guessed);
            }
        } else
        {
            info.state = ArenaFinalInfo.STATE_WAIT;
            lblOpenTime.transform.parent.gameObject.SetActive(true);
            System.DateTime dt = TimeKit.getDateTime(ParentPoint.info.startTime);
            System.DateTime dtNow = TimeKit.getDateTime(now);

            string str;
			if(dt.Year == dtNow.Year)
			{
	            if (dt.DayOfYear == dtNow.DayOfYear)
	                str = LanguageConfigManager.Instance.getLanguage("Arena18", dt.Hour + "");
	            else if (dt.DayOfYear - dtNow.DayOfYear == 1)
	                str = LanguageConfigManager.Instance.getLanguage("Arena19", dt.Hour + "");
				else if(dt.DayOfYear - dtNow.DayOfYear == 2)
					str = LanguageConfigManager.Instance.getLanguage("Arena20", dt.Hour + "");
				else 
					str = LanguageConfigManager.Instance.getLanguage("Arena20_1", dt.Hour + "");
			}
			else
			{
				int dtNowDays = System.DateTime.IsLeapYear(System.DateTime.Today.Year) ? 366 : 365;
				if(dtNowDays-dtNow.DayOfYear+dt.DayOfYear == 1)
				{
					str = LanguageConfigManager.Instance.getLanguage("Arena19", dt.Hour + "");
				}
				else if(dtNowDays-dtNow.DayOfYear+dt.DayOfYear == 2)
				{
					str = LanguageConfigManager.Instance.getLanguage("Arena20", dt.Hour + "");
				}
				else
					str = LanguageConfigManager.Instance.getLanguage("Arena20_1", dt.Hour + "");
			}
            lblOpenTime.text = str;
			//Debug.LogError("dt===="+dt+" dtNow======"+dtNow+" dt.dayofyear===="+dt.DayOfYear+" dt.dayofyear===="+dtNow.DayOfYear+" str===="+str);
        } 
    }


    Vector3 _tempV;
    public void Update()
    {
        _tempV = UiManager.Instance.gameCamera.WorldToViewportPoint(gameObject.transform.position);

        if (point3d != null)
        {
            _tempV.z = 20;
            point3d.transform.position = window.map.camera.ViewportToWorldPoint(_tempV);
        }

        if (role3d != null)
        {
            _tempV.z = 10;
            role3d.transform.position = window.map.camera.ViewportToWorldPoint(_tempV) + roleOffset;
        }

        if (info != null && info.state > 0 && parentPoint1 != null)
        {
            _updateInfoDelay += Time.deltaTime;
            if (_updateInfoDelay >= 1f)
            {
                _updateInfoDelay = 0;
                int now = ServerTimeKit.getSecondTime();
                if (info.state == ArenaFinalInfo.STATE_WAIT)
                {
                    if (ParentPoint.info.guessStartTime + 60 <= now && now <= ParentPoint.info.guessEndTime)
                    {
                        UpdateInfo();
                        window.updateGuessNumbers();
                    }
                } else if (info.state == ArenaFinalInfo.STATE_GUESS)
                {
                    if (now > ParentPoint.info.guessEndTime)
                    {
                        UpdateInfo();
                    }
                }
            }
        }
    }

    void OnButtonGuessClick()
    {
        UiManager.Instance.openDialogWindow<ArenaGuessWindow>((win)=>{
            win.init(this);
       EventDelegate.Add( win.onDestroy, () => {
                if(!window.IsDestoryed)
                {
                    window.updateGuessNumbers();
                }
            });
        });
    }

    void OnButtonReplayClick()
    {
		MaskWindow.LockUI();
        FPortManager.Instance.getFPort<ArenaReplayInfoFPort>().access((replayInfo)=>{
            UiManager.Instance.openDialogWindow<ArenaReplayWindow>((win)=>{
                win.init(replayInfo,ParentPoint.info,OnPlayReplay);
                window.map.gameObject.SetActive(false);
           EventDelegate.Add( win.onDestroy, () => {
                    if(!window.IsDestoryed)
                    {
                        window.map.gameObject.SetActive(true);
                    }
                });
            });
        },ParentPoint.info.finalState,ParentPoint.info.index);
    }

	void OnButtonFindInfo()
	{
		if(info.uid != null) {
			MaskWindow.LockUI();
			getPlayerInfoFPort(info.uid);
		}
	}

	private void getPlayerInfoFPort(string _uid)
	{
		ChatGetPlayerInfoFPort fport = FPortManager.Instance.getFPort("ChatGetPlayerInfoFPort") as ChatGetPlayerInfoFPort;
		//fport.access(_uid,null,null,PvpPlayerWindow.FROM_ARENA);
		fport.access(_uid,10,null,null,PvpPlayerWindow.FROM_ARENA);
	}

    void OnPlayReplay()
    {

    }

    public ArenaFinalPoint ParentPoint
    {
        get
        {
            if (parentPoint1.info.isWinner())
                return parentPoint1;
            else
                return parentPoint2;
        }
    }
}
