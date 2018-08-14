using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

public class Main : MonoBehaviour
{

    void Awake()
    {
        Button loginButton = transform.Find("LoginButton").GetComponent<Button>();
        loginButton.onClick.AddListener(ClickLogin);
        Button logoutButton = transform.Find("LogoutButton").GetComponent<Button>();
        logoutButton.onClick.AddListener(ClickLogout);
    }

    private void ClickLogin()
    {
        SdkManager.INSTANCE.Login();
    }

    private void ClickLogout()
    {

    }

    struct LoadResult
    {
        public int state;
        public WWW www;

        public LoadResult(int state, WWW www)
        {
            this.state = state;
            this.www = www;
        }
    }

    class LoadStatus
    {
        public static int OK = 0;
        public static int ERROR = 1;
        public static int TIMEOUT = 2;
    }
}
