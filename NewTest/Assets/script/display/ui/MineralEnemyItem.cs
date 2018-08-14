using UnityEngine;
using System.Collections;

public class MineralEnemyItem : MonoBehaviour {

	/* fields */
	public UITexture icon;
	public UILabel name;
	public UILabel timeDesc;
	public UILabel detailDesc;

	public ButtonBase fightButton;
	public ButtonBase replayButton;
	//父窗口
	private WindowBase win;
	
	/**method */
	/// <summary>
	/// 更新奖励条目
	/// </summary>
	/// <param name="tl">排行奖励</param>
	/// <param name="win">父窗口</param>
	public void updateItem (PillageEnemyInfo mi, int index, WindowBase win) {
		this.win = win;
		updateRank (mi);
		initButtonInfo (mi, index);
	}
	/// <summary>
	/// 更新排行条目
	/// </summary>
	/// <param name="tl">Tl.</param>
	private void updateRank (PillageEnemyInfo mi) {
		ResourcesManager.Instance.LoadAssetBundleTexture (UserManager.Instance.getIconPath (mi.HeadIconId), icon);
		name.text = mi.playerName;
		int type = MiningManagement.Instance.GetMiningSampleBySid (mi.sid).type;
        long time = ServerTimeKit.getSecondTime() - mi.time;
		if (type == (int)MiningTypePage.MiningGold) {
            timeDesc.text = time <= 3600 ? LanguageConfigManager.Instance.getLanguage("mining_info5") : LanguageConfigManager.Instance.getLanguage("mining_info1", TimeKit.timeTransformDHM(time));
			detailDesc.text = LanguageConfigManager.Instance.getLanguage("mining_info3",mi.count.ToString()); 
		}else {
            timeDesc.text = time <= 3600 ? LanguageConfigManager.Instance.getLanguage("mining_info6") : LanguageConfigManager.Instance.getLanguage("mining_info1", TimeKit.timeTransformDHM(time));
            detailDesc.text = LanguageConfigManager.Instance.getLanguage("mining_info4",mi.count.ToString()); 
		}
	}
	/// <summary>
	/// 初始化button信息
	/// </summary>
	/// <param name="tl">Tl.</param>
	public void initButtonInfo (PillageEnemyInfo mi, int index) {
		fightButton.setFatherWindow (win);
		fightButton.exFields = new Hashtable ();
		fightButton.exFields.Add ("roleUid", mi.RoleUid);
		fightButton.exFields.Add ("serverName", mi.serverName);

		replayButton.setFatherWindow(win);
		replayButton.exFields = new Hashtable();
		replayButton.exFields.Add("roleUid", mi.RoleUid);
		replayButton.exFields.Add ("serverName", mi.serverName);
        replayButton.exFields.Add("time",mi.time);

		if (mi.minerals.Count == 0) {
			fightButton.disableButton (true);
		}
		else {
			fightButton.disableButton (false);
		}
	}
}
