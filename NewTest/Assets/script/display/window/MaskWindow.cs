using UnityEngine;
using System.Collections;

/**
 * 遮罩窗口
 * @author longlingquan
 * */
public class MaskWindow : MonoBehaviour
{
	public static MaskWindow instance;
	public GameObject maskUI;
	public GameObject maskNet;
	public GameObject prog;
	public GameObject serverReportWait;
	public UISprite usProg;
	public UISprite usProg2;
	float showProgCD;
	const float LOSTTIME=8f;//掉线时间
	float showTime;//显示小loading的持续时间(卡网络时间)
	bool uiShowProg;

	void Awake ()
	{
		instance = this;
		if(GameManager.Instance.maskDebug){
			maskUI.GetComponent<UISprite>().enabled=true;
			maskNet.GetComponent<UISprite>().enabled=true;
		}else{
			maskUI.GetComponent<UISprite>().enabled=false;
			maskNet.GetComponent<UISprite>().enabled=false;
		}
	}

	// Update is called once per frame
	void Update ()
	{
		if (showProgCD > 0) {
			showProgCD -= Time.deltaTime;
			if (showProgCD <= 0 && serverReportWait.activeSelf==false) {
				prog.SetActive (true);
			}
		}

		//太久直接掉线重连
		if (showTime > 0) {
			showTime -= Time.deltaTime;
			if (showTime <= 0) {
				NetUnlock ();
				GameManager.Instance.OnLostConnect (true);
			//	showTime = LOSTTIME;
			}
		}
	}
    public static void NetLockMaskShow() {
        if (instance == null)
            return;
        instance.prog.SetActive(true);
        instance.maskNet.SetActive(true);
    }
	public void setServerReportWait(bool onoff)
	{
		if(prog.activeSelf==true)
			return;
		serverReportWait.SetActive(onoff);
	}
	void FixedUpdate ()
	{
		usProg.transform.Rotate (0, 0, -4f);
		usProg2.transform.Rotate (0, 0, -4f);
	}

	public static void LockUI ()
	{
		LockUI (false);
	}

	//锁定UI,showProgress是否显示进度条
	public static void LockUI (bool showProgress)
	{
	//	Debug.LogError("##############LockUI");
		if (instance == null)
			return;

		instance.maskUI.SetActive (true);
		instance.uiShowProg = showProgress;
		if (showProgress) {
			instance.showProgCD = 0.00001f;
		}
	}

	public static void UnlockUI ()
	{
        //Debug.LogError("##############UnlockUI");
		UnlockUI (false);
	}

	//解锁UI,enforce是否同时解锁网络锁
	public static void UnlockUI (bool enforce)
	{
		if (instance == null || instance.maskUI==null || instance.maskNet==null)
			return;
		instance.maskUI.SetActive (false);
		instance.uiShowProg = false;
		if (enforce) {
			NetUnlock ();
		} else if (!instance.maskNet.activeSelf) {
			instance.showProgCD = 0;
			instance.prog.SetActive (false);
		}
	}

	public static void NetLock ()
	{
		if (instance == null)
			return;
		instance.maskNet.SetActive (true);
		instance.showProgCD = 0.3f;
		instance.showTime = LOSTTIME;
	}

	public static void NetUnlock ()
	{
		if (instance == null)
			return;
		instance.maskNet.SetActive (false);
		instance.showTime = 0;
		if (!instance.uiShowProg) {
			instance.showProgCD = 0;
			instance.prog.SetActive (false);
		}
	}
}
