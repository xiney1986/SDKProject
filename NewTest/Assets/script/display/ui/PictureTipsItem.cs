using UnityEngine;
using System.Collections;

public class PictureTipsItem : MonoBase {
	/** 标题 */
	public UILabel title;
	/** 描述 */
	public UILabel des;
	/** 前往按钮 */
	public ButtonBase buttonGo;
	/** 连接模版 */
	private PictureTipsSample mSample;	
	private void Start () {
		buttonGo.onClickEvent = onClickBtn;
	}	

	private void onClickBtn ( GameObject go ) {
		if (UserManager.Instance.self.getUserLevel () < mSample.openLevel) {
			UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage("s0587"));

		}
		/** 如果是讨伐副本 */
		if (mSample.sid == 1) {
			int sid = -999;
			foreach(int i in PictureManagerment.Instance.currentSample.missionSids)
			{
				if(FuBenManagerment.Instance.isCompleteLastMission(i))
				{
					sid = i;
				}
			}
			if(sid == -999)
			{
				UiManager.Instance.createMessageLintWindow (LanguageConfigManager.Instance.getLanguage("s0588"));
				return;
			}
			else
			{
				WindowLinkManagerment.Instance.setTeamPrepareInfo(sid);
				WindowLinkManagerment.Instance.OpenWindow (mSample.windowLinkSid);
			}
		} else {
			WindowLinkManagerment.Instance.OpenWindow (mSample.windowLinkSid);
		}
	}

	public void setData (PictureTipsSample mSample ) {
		this.mSample = mSample;
		title.text =  mSample.title ;

		if (mSample.sid == 1) {
			string text = "";
			foreach(int sid in PictureManagerment.Instance.currentSample.missionSids)
			{
				Mission ms = MissionInfoManager.Instance .getMissionBySid (sid);
				text += ms.getMissionName() + ",";
			}
			des.text = mSample.des.Replace("%1",text);

		} else {
			des.text = mSample.des;
		}

		if (mSample.isCanClick) {
			buttonGo.gameObject.SetActive(true);
		} else {
			buttonGo.gameObject.SetActive(false);
		}
	}

	public void setFatherWindow(WindowBase fatherWindow )
	{
		buttonGo.setFatherWindow (fatherWindow);
	}
}
