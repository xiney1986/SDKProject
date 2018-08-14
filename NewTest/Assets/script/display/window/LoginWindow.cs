using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class LoginWindow : WindowBase
{
    class LoginData
    {
        public string name;
        public string serverIP;
    }

    public GameObject inputContent;
    public ButtonBase buttonServer;
    //	public GameObject loginWindow;
    public UIInput NameInput;
    public UIInput serverIPInput;
    public GameObject selectContent;
    public UISprite selectBackground;
    public GameObject[] nameButtons;
    public UILabel[] nameLabels;
    public UIToggle savePass;
    public UILabel version;
    public string androidWebURL;
    public string iosWebURL;
    Json_ServerInfo latelyServer = null;
    LinkedList<LoginData> loginDataList;
    public GameObject monoObject;
    public UILabel loginLabel;
    public bool firstInto = true;
    public ButtonBase loginButton;

    public override void OnAwake()
    {
        base.OnAwake();
        loginButton.setFatherWindow(this);
        GameManager.Instance.isReLogin = false;
        SdkManager.createGoodList();
        GameManager.Instance.loadBaseExResource();
        SdkManager.INSTANCE.Login();
        if (SdkManager.INSTANCE.IS_SDK)
        {
            inputContent.SetActive(false);
        }
        else
        {
            inputContent.SetActive(true);
        }
        //List<Notice> list = NoticeManagerment.Instance.getValidNoticeList(2);
        //for (int i = 0; i < list.Count; i++)
        //{
        //    if (list[i] is NewRechargeNotice)
        //    {
        //        NewRechargeNotice rechargeNotice = list[i] as NewRechargeNotice;
        //    }
        //}
        //Debug.Log("count="+list.Count);
        //StringBuilder sb = new StringBuilder();
        //for (int i = 0; i < list.Count; i++)
        //{
        //    sb.Append(list[i]);
        //    sb.AppendLine();
        //}
        //File.WriteAllText("C:/Users/QWQ/Desktop/1.txt", sb.ToString());
    }

    protected override void begin()
    {
        base.begin();
        MaskWindow.UnlockUI(true);
        loadLoginInfo();
        if (GuideManager.Instance.guideUI != null)
        {
            GuideManager.Instance.hideGuideUI();
        }
        latelyServer = ServerManagerment.Instance.getLocalSaveServer();
        if (latelyServer != null)
        {
            buttonServer.textLabel.text = latelyServer.name;
        }
    }

    public override void DoDisable()
    {
        base.DoDisable();
    }

    protected override void DoEnable()
    {
        UiManager.Instance.backGround.switchBackGround("loginBackGround");
    }

    public override void buttonEventBase(GameObject gameObj)
    {
        if (gameObj.name == "buttonLogin")
        {
            ClickLogin();
        }
        else if (gameObj.name == "buttonSelect")
        {
            if (loginDataList.Count > 0)
                tweenScaleSelect(!selectContent.activeInHierarchy);
            MaskWindow.UnlockUI();
        }
        else if (gameObj.name == "select_bg")
        {
            tweenScaleSelect(false);
        }
        else if (gameObj.name == "serverButton")
        {
            if (ServerManagerment.Instance.getAllServer().Count == 0)
            {
                SystemMessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("loginError1"));
                return;
            }
            saveLoginInfo();
            UiManager.Instance.openWindow<SelectServerWindow>();
        }
        else
        {
            for (int i = 0; i < nameButtons.Length; i++)
            {
                if (gameObj == nameButtons[i])
                {
                    tweenScaleSelect(false);
                    NameInput.value = nameLabels[i].text;
                    serverIPInput.value = getIPWithName(nameLabels[i].text);
                    break;
                }
            }
        }

    }

    private void ClickLogin()
    {
        Json_ServerInfo server = ServerManagerment.Instance.lastServer;
        if (server != null && server.is_open == "1")
        {
            MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("serverState09"));
            return;
        }
        if (SdkManager.INSTANCE.IS_SDK)
        {
            Debug.Log(server.payUrl);
            wantToIntoGame(server);
        }
        else
        {
            pcLogin(server);
        }
        ServerManagerment.Instance.setLatelyLoginServer(ServerManagerment.Instance.getServerInfo(server));
    }

    private void pcLogin(Json_ServerInfo server)
    {
        bool isload = setMsg(server);
        if (isload == false)
        {
            return;
        }
        //如果有自定义服务器ip,那么当前选中的服务器ip换掉
        string ip = serverIPInput.value;
        if (ip != "" && !SdkManager.INSTANCE.IS_SDK)
        {
            if (ServerManagerment.Instance.lastServer != null && ServerManagerment.Instance.lastServer.ipEndPoint != null)
                ServerManagerment.Instance.lastServer.ipEndPoint = null;
            string[] str = ip.Split(':');
            if (str != null && str.Length == 2)
            {
                server.domainName = str[0];
                server.port = StringKit.toInt(str[1]);
            }
        }
        if (server != null)
        {
            MaskWindow.NetLock();
            ConnectManager.manager().beginConnect(server.domainName, server.port, PcConnectOK);
        }
    }

    private void PcConnectOK()
    {
        LoginFPort port = FPortManager.Instance.getFPort("LoginFPort") as LoginFPort;
        if (GameManager.Instance.GM)
        {
            port.loginGM(ServerManagerment.Instance.userName, 1, () =>
            {
                GameManager.Instance.loginBack(fatherWindow);
            }
            );
        }
        else
        {
            port.login("1", "1", ServerManagerment.Instance.userName, () =>
            {
                GameManager.Instance.loginBack(fatherWindow);
            });
        }
    }

    void wantToIntoGame(Json_ServerInfo server)
    {
        if (server.is_open == "1")
        {
            MaskWindow.LockUI();
            UiManager.Instance.openDialogWindow<MessageLineWindow>((win) =>
            {
                win.dialogCloseUnlockUI = false;
                win.Initialize(LanguageConfigManager.Instance.getLanguage("serverState09"), false);
            });
            return;
        }
        else
        {
            MaskWindow.LockUI();
            connetGameServer(server);
        }
    }


    public void connetGameServer(Json_ServerInfo server)
    {
        MaskWindow.NetLock();
        ConnectManager.manager().beginConnect(server.domainName, server.port, ()=> {
            connectGameServerOK(server);
        });
    }

    void connectGameServerOK(Json_ServerInfo server)
    {
        LoginFPort port = FPortManager.Instance.getFPort("LoginFPort") as LoginFPort;
        Debug.Log(SdkManager.URL);
        port.login(SdkManager.URL, StringKit.toInt(server.sid), () =>
        {
            GameManager.Instance.loginBack(UiManager.Instance.getWindow<LoginWindow>());
        });
    }

    private void tweenScaleSelect(bool isScale)
    {
        if (isScale)
        {
            selectContent.SetActive(isScale);
            selectContent.GetComponent<TweenScale>().from = new Vector3(1, 0, 1);
            selectContent.GetComponent<TweenScale>().to = new Vector3(1, 1, 1);
            TweenScale.Begin<TweenScale>(selectContent, 0.35f);

        }
        else
        {
            StartCoroutine(closeTweenScale(0.4f, isScale));
        }
        MaskWindow.UnlockUI();
    }

    private IEnumerator closeTweenScale(float time, bool isscale)
    {
        selectContent.GetComponent<TweenScale>().from = new Vector3(1, 1, 1);
        selectContent.GetComponent<TweenScale>().to = new Vector3(1, 0, 1);
        TweenScale.Begin<TweenScale>(selectContent, (time - 0.1f));
        yield return new WaitForSeconds(time);
        selectContent.SetActive(isscale);
    }

    public bool setMsg(Json_ServerInfo server)
    {
        if (NameInput.value == "")
        {
            MessageWindow.ShowAlert(LanguageConfigManager.Instance.getLanguage("loginwindow004"));
            return false;
        }
        ServerManagerment.Instance.saveLastServer(server);
        saveLoginInfo();
        if (GuideManager.Instance.guideUI != null)
        {
            GuideManager.Instance.hideGuideUI();
        }
        return true;
    }

    void loadLoginInfo()
    {
        NameInput.defaultText = Language("loginWindow_nameInputLabel");
        loginDataList = new LinkedList<LoginData>();
        for (int i = 0; i < nameButtons.Length; i++)
        {
            string name = PlayerPrefs.GetString(PlayerPrefsComm.LOGIN_NAME + i);
            string ip = PlayerPrefs.GetString(PlayerPrefsComm.LOGIN_IP + i);
            if (!string.IsNullOrEmpty(name))
            {
                LoginData data = new LoginData();
                data.name = name;
                data.serverIP = ip;
                loginDataList.AddLast(data);

                if (i == 0)
                {
                    NameInput.value = name;
                    serverIPInput.value = ip;
                }
            }
            else
            {
                break;
            }
        }

        if (loginDataList.Count > 0)
        {
            selectBackground.height = 20 + loginDataList.Count * 45;

            LinkedListNode<LoginData> node = loginDataList.First;
            for (int i = 0; i < nameButtons.Length; i++)
            {
                if (i < loginDataList.Count)
                {
                    nameLabels[i].text = node.Value.name;
                    nameButtons[i].SetActive(true);
                    node = node.Next;
                }
                else
                {
                    nameButtons[i].SetActive(false);
                }

            }
        }
    }

    public override void OnNetResume()
    {
        base.OnNetResume();
        UserManager.Instance.gotoMainWindow();
    }
    //保存登录信息
    void saveLoginInfo()
    {

        ServerManagerment.Instance.userName = NameInput.value;
        ServerManagerment.Instance.passWord = "123";


        string name = NameInput.value;
        string ip = serverIPInput.value;
        LoginData newData = new LoginData();
        newData.name = name;
        newData.serverIP = ip;
        //添加到数据列表的第一位
        loginDataList.AddFirst(newData);

        int index = 0;
        LinkedListNode<LoginData> node = loginDataList.First;
        while (node != null)
        {
            LoginData ld = node.Value;
            if (node == loginDataList.First || ld.name != name)
            {
                PlayerPrefs.SetString(PlayerPrefsComm.LOGIN_NAME + index, ld.name);
                if (node == loginDataList.First && !savePass.value)
                {
                    PlayerPrefs.DeleteKey(PlayerPrefsComm.LOGIN_IP + index);
                }
                else
                {
                    PlayerPrefs.SetString(PlayerPrefsComm.LOGIN_IP + index, ld.serverIP);
                }
                index++;
            }
            node = node.Next;
        }
    }

    string getIPWithName(string name)
    {
        LinkedListNode<LoginData> node = loginDataList.First;
        while (node != null)
        {
            if (node.Value.name == name)
                return node.Value.serverIP;
            node = node.Next;
        }
        return null;
    }
}
