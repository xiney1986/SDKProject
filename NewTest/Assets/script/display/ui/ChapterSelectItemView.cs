using UnityEngine;
using System.Collections;

public class ChapterSelectItemView : ButtonBase
{
	public UILabel lab_name;
	public UILabel lab_info;
	public UILabel lab_starInfo;
	public ChapterSample data;
	public UITexture bgTexture;
	public GameObject effectParfab;
	public UILabel awardNum;

	public void init(ChapterSample cs)
	{
		data=cs;
//		lab_name.text=cs.name;
//		lab_info.text=cs.describe;
		lab_starInfo.text = FuBenManagerment.Instance.getMyMissionStarNum(cs.sid) + "/" + FuBenManagerment.Instance.getAllMissionStarNum(cs.sid);
		ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.CHAPTERDESCIMAGEPATH + ChapterSampleManager.Instance.getChapterSampleBySid (cs.sid).thumbIcon + "_chapterImage",bgTexture);
//		ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.BACKGROUNDPATH + "backGround_3",bgTexture);
		if (this.name == "001") {
			effectParfab.SetActive (true);
		}
		int num = FuBenManagerment.Instance.checkAwardCanGetNum (cs.sid);
		if (num == 0) {
			awardNum.transform.parent.gameObject.SetActive (false);
		} else {
			awardNum.transform.parent.gameObject.SetActive (true);
			awardNum.text = num.ToString();
		}

	}
}

