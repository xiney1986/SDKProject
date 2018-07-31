using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Main : MonoBehaviour
{

    void Awake()
    {
        Button loginButton = transform.Find("LoginButton").GetComponent<Button>();
        loginButton.onClick.AddListener(ClickLogin);
    }

    private void ClickLogin()
    {

    }
}
