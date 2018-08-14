using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

//服务器按钮
public class ServerButton : ButtonBase
{

    public Json_ServerInfo server;
    public UILabel platform;
    public UISprite state;
    /** 是否登录button */
    public bool isLoginButton = false;
    /** 是否聚焦背景 */
    public bool isFoucsBg = false;

    /** 更新按钮背景 */
    private void UpdateButtonBg()
    {
        if (fatherWindow is SelectServerWindow)
        {
            SelectServerWindow ssw = fatherWindow as SelectServerWindow;
            ssw.UpdateSelectBgPosition(this);
        }
    }

    public override void DoClickEvent()
    {
        //注释掉这句解锁，for YXZH-5319 by 2014.9.10  Int1.5
        //		MaskWindow.UnlockUI ();
        if (isFoucsBg)
        {
            UpdateButtonBg();
        }
        if (server == null)
            return;
        ServerManagerment.Instance.saveLastServer(server);
        ServerManagerment.Instance.setLatelyLoginServer(ServerManagerment.Instance.getServerInfo(server));
        if (fatherWindow is SelectServerWindow)
        {
            fatherWindow.finishWindow();
        }
        else
        {
            UiManager.Instance.openWindow<LoginWindow>();
        }
        PlayerPrefs.Save();
    }
    public override void DoDisable()
    {
        base.DoDisable();
        StopAllCoroutines();
    }
    public void initButton(Json_ServerInfo _server)
    {
        this.server = _server;
        platform.gameObject.SetActive(true);
        platform.text = server.name;

        if (server.is_new == "1")
        {
            textLabel.color = Colors.SERVER_NEW;
        }
        else if (server.is_hot == "1")
        {
            textLabel.color = Colors.SERVER_HOT;
        }
        else
        {
            textLabel.color = Colors.SERVER_NORMAL;
        }
    }
}
