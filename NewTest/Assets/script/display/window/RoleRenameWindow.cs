using UnityEngine;
using System.Collections;

public class RoleRenameWindow : WindowBase
{		
	/** 用于判断是否为男性角色 */
	private bool[] gender = new bool[]{false,false,true,false,true,false};//角色性别 false 为女 true 为男
	/** 是否为男性角色 */
	private bool isMan;
	/** 输入 */
	public UIInput input;
	/** 记录改名卡的模版 */
	public PropSample sample;
	protected override void begin ()
	{
		base.begin ();
		Card mainCard = StorageManagerment.Instance.getRole(UserManager.Instance.self.mainCardUid);
		isMan = gender [mainCard.sid -1];
		MaskWindow.UnlockUI ();
	}
	private void roleRename (string name)
	{
		UserRenameFport fport = FPortManager.Instance.getFPort ("UserRenameFport") as UserRenameFport;
		fport.access (sample.sid, name, renameCallBack);
			
	}
	private void renameCallBack (string newName)
	{
		MaskWindow.UnlockUI ();
		UserManager.Instance.self.nickname = newName;
		this.finishWindow ();
		StoreWindow store = UiManager.Instance.getWindow<StoreWindow> ();
		if (store != null) {
			store.updateContent();
		}

	}
		
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "buttonSure") {
			if (Utils.EncodeToValid (input.value)) {
				UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage ("roleNameWindow_Validname"));
				return;
			} else if (getLength (input.value) <= 0) {
				UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage ("s0438"));
				return;
			}
			if (getLength (input.value) > 12) {
				UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage ("s0278"));
				return;
			}
			if (ShieldManagerment.Instance.isContainShield (input.value)) {
				UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage ("s0279"));
				return;
			}
			roleRename (input.value);
		} else if (gameObj.name == "buttonCancel") {
			this.finishWindow ();
		}
		else if (gameObj.name == "randomButton") {
			string newName = RandomNameManagerment.Instance.getRandomName (isMan);
			input.value = newName;
			input.UpdateLabel ();
			MaskWindow.UnlockUI ();
		}
	}


	//获得字符串字节长度
	private int getLength (string str)
	{
		if (string.IsNullOrEmpty (str))
			return 0;
		
		int len = 0;
		foreach (char each in str.ToCharArray()) {
			if ((int)each >= 19968 && (int)each <= 40959) {
				len += 2;
			} else
				len += 1;
		}
		return len;
		
	}
		

}
