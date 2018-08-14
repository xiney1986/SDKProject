using UnityEngine;
using System.Collections;


public enum E_SystemSettings {
	AudioMusic,			//游戏背景音乐
	AudioSound,			//游戏音效
	FriendAutoReceive,	//自动接受好友申请
	NoticePveFull,		//通知,pve恢复满
	//NoticeShake,		//通知,好友摇一摇时间到
	NoticeFeast,		//通知,女神宴时间到
    MiaoShao,//开启秒杀功能
}


public class SystemSettingsManagerment {
	public static SystemSettingsManagerment Instance {
		get { return SingleManager.Instance.getObj ("SystemSettingsManagerment") as SystemSettingsManagerment; }
	}


	public bool[] Settings;


	public SystemSettingsManagerment () {

	}

	public void SaveData () {


	}

	public bool IsOpen ( E_SystemSettings index ) {
		return Settings != null && Settings[(int)index];
	}

	public bool IsOpen ( int index ) {
		return Settings[index];
	}

	public void UpdateSettings ( bool[] newSettings ) {
		Settings = newSettings;
	}
}
