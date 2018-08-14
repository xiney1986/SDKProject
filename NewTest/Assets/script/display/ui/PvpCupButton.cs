using UnityEngine;
using System.Collections;

public class PvpCupButton : ButtonBase
{
	public UILabel playersName;
	public UILabel playersLv;
	public UITexture playerIcon;
	public UISprite kuang;
	public UISprite line;
	private const int USER_EXP_SID = 1;//玩家经验索引
	private PvpOppInfo info;
	private const int WIN = 1;
	private const int LOSE = 0;
	private const string PLAYER = "img_4";
	private const string OPPSPRITE = "img_5";
	private const string COMMON = "img_7";
	private const string WINLINE = "pk_yellow";
	private const string COMMONLINE = "pk_blue";
	
	public PvpOppInfo getPvpOppInfo ()
	{
		return info;
	}
	
	public void initButton (PvpOppInfo info)
	{
		this.info = info;
		playersName.text = info.name;
		playersLv.text = "Lv." + EXPSampleManager.Instance.getLevel (USER_EXP_SID, info.exp, 0);
		ResourcesManager.Instance.LoadAssetBundleTexture (UserManager.Instance.getIconPath (info.headIcon), playerIcon);
		playerStateShow ();
	}
	
	private void playerStateShow ()
	{
		if (this.name == "1") {
			kuang.spriteName = PLAYER;	
		} else if (PvpInfoManagerment.Instance.isCupOpp (info)) {
			kuang.spriteName = OPPSPRITE;
		} else {
			kuang.spriteName = COMMON;
		}
		
		if (PvpInfoManagerment.Instance.getPvpInfo ().round == 1) {
			line.spriteName = COMMONLINE;
		} else {
			if (info.state == 0) {
				kuang.color = Color.gray;
				playersLv.color = Color.gray;
				playersName.color = Color.gray;
				playerIcon.color = Color.gray;
				line.depth = 3;
			} else {
				line.spriteName = WINLINE;
				line.depth = 4;
			}
		}
		this.gameObject.SetActive (true);
	}
	
	public override void DoClickEvent ()
	{
		base.DoClickEvent ();
		UiManager.Instance.openWindow<PvpPlayerWindow> (
			(win) => {
			win.initInfo (info, () => {});
			CardBookWindow.setChatPlayerUid(info.uid);
		}
		);
	}

}
