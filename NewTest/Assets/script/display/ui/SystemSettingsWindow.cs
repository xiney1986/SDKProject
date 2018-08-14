using System.Configuration;
using UnityEngine;
using System.Collections;

/**
 * 系统设置
 * @authro 陈世惟  
 * */
public class SystemSettingsWindow : WindowBase {

	public GameObject exitButton;
	public GameObject cancelButton;
	public Transform switchContainer;

	private SysSet[] sysSets;
	private bool[] mSettings;

	public override void OnStart () {
		base.OnStart ();
		#region ios
		if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXEditor) {
			cancelButton.transform.localPosition = new Vector3 (0, -140, 0);
			exitButton.SetActive (false);
		}
		#endregion
		mSettings = new bool[SystemSettingsManagerment.Instance.Settings.Length];
		SystemSettingsManagerment.Instance.Settings.CopyTo(mSettings, 0);
		sysSets = switchContainer.GetComponentsInChildren<SysSet> ();
		System.Array.Sort<SysSet> (sysSets, ( a, b ) => {
			return int.Parse (a.name.Substring (0, 1)) < int.Parse (b.name.Substring (0, 1)) ? -1 : 1;
		});
		updateUI (true);
	}


	protected override void begin () {
		base.begin ();
		MaskWindow.UnlockUI ();
	}


	private void saveData () {
		string changeSettings = "";
		for (int i = 0; i < mSettings.Length-1; i++) {
			if (mSettings[i] != SystemSettingsManagerment.Instance.Settings[i]) {
				changeSettings += "|" + (i + 1).ToString () + ":" + (mSettings[i] ? 1 : 0);
			}
        }
        PlayerPrefs.SetInt(UserManager.Instance.self.uid + "miaosha", mSettings[mSettings.Length - 1] ? 1 : 0);
        SystemSettingsManagerment.Instance.UpdateSettings(mSettings);
		if (changeSettings.Length <= 0) return;
		changeSettings = changeSettings.Substring (1);
		FPortManager.Instance.getFPort<SystemSettingsFport> ().Submit (() => {
			SystemSettingsManagerment.Instance.UpdateSettings (mSettings);
		}, changeSettings);
	}


	private void updateUI ( bool isUpdateSettingsItem )
	{
	    int index = 0;
		for (int i = 0; i < sysSets.Length; i++) {
		    if (i == sysSets.Length - 1)
		    {
		        sysSets[i].choose.gameObject.SetActive(mSettings[mSettings.Length - 1]);
		        index = mSettings.Length - 1;
		    }
		    else
		    {
		        sysSets[i].choose.gameObject.SetActive(mSettings[i]);
		        index = i;
		    }
		    if (isUpdateSettingsItem)
                updateSettingsItem(index);
		}
	}


	private void updateSettingsItem ( int index ) {
		this.GetType ().GetMethod ("sysSet_" + index, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke (this, null);
	}


	public override void buttonEventBase ( GameObject gameObj ) {
		base.buttonEventBase (gameObj);
		switch (gameObj.name) {
			case "close":
				saveData ();
				finishWindow ();
				break;
			case "exit":
				saveData ();
                SdkManager.INSTANCE.Exit();
		        break;
			case "cancelled":
		        DoubleRMBManagement.Instance.isEnd = false;
				saveData ();
                GameManager.Instance.isFirstLoginOpenBulletin = true;//解决注销后重新登录没有公告弹出
				SdkManager.INSTANCE.Logout ();

				GameManager.Instance.logOut();

				MaskWindow.UnlockUI ();
				break;
		}
		MaskWindow.UnlockUI ();

		if (gameObj.transform.parent == switchContainer) {
			int index = int.Parse (gameObj.name.Substring (0, 1));
			mSettings[index] = !mSettings[index];
			updateSettingsItem (index);
		}
		updateUI (false);
	}



	//=============具体每个设置实现,只做实现,不用写选择框状态等...新加入设置也只需要在这里加入新的实现代码==========================
	/// <summary>
	/// 声音设置
	/// </summary>
	private void sysSet_0 () {
		AudioManager.Instance.IsMusicOpen = mSettings[(int)E_SystemSettings.AudioMusic];
	}
	/// <summary>
	/// 音效设置
	/// </summary>
	private void sysSet_1 () {
		AudioManager.Instance.IsAudioOpen = mSettings[(int)E_SystemSettings.AudioSound];
	}

    private void sysSet_5()
    {
        GameManager.Instance.isOpenMiaoSha = mSettings[(int) E_SystemSettings.MiaoShao ];
    }
	/// <summary>
	/// 好友自动接受设置
	/// </summary>
	private void sysSet_2 () {
	}
	/// <summary>
	/// notice pve恢复满
	/// </summary>
	private void sysSet_3 () {
		//GameManager.Instance.pushNotice ();
	}
	/// <summary>
	/// notice 女神摇一摇时间到
	/// </summary>
	private void sysSet_4 () {
		//GameManager.Instance.pushNotice ();
	}
	/// <summary>
	/// notice 女神宴会
	/// </summary>
	//private void sysSet_5 () {
		//GameManager.Instance.pushNotice ();
	//}

}
