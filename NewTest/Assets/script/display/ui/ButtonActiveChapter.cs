using UnityEngine;
using System.Collections;

public class ButtonActiveChapter : ButtonBase
{

	public	UILabel chapterNameLabel;
	public	UITexture chapterIcon;
	public	int sid;
	int index;

	void ScaleAnimComplete ()
	{
		GuideManager.Instance.guideEvent();
	}

	public void updateChapter (int sid)
	{
		chapterNameLabel.text = ChapterSampleManager.Instance.getChapterSampleBySid (sid).name;
//		ResourcesManager.Instance.LoadAssetBundleTexture( ResourcesManager.ACTIVITYCHAPTERICONPATH + 
//		                                                 "activity_"+MissionInfoManager.Instance.getChapterIconIdBySid(sid),chapterIcon);
		this.sid = sid;
	}
}
