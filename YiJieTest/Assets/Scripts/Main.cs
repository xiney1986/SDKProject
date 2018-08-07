using UnityEngine;
using System.Collections;
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
        SdkManager.INSTANCE.Logout();
    }
}
