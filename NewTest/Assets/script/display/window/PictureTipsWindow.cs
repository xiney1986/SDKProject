using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PictureTipsWindow : WindowBase {
	/** 跳转项预制体 */
	public PictureTipsItem tipsItem;
	public UIGrid content;
	/** 官方活动提示标签 */
	public UILabel officeTips;

	protected override void begin()
	{
		base.begin();
		if (!isAwakeformHide) {
			updateData();
		}
		MaskWindow.UnlockUI();
	}
	
	
	public override void buttonEventBase(GameObject gameObj)
	{
		base.buttonEventBase(gameObj);
		switch (gameObj.name)
		{
		case "close":
			finishWindow();
			break;
		}	
	}
	
	private void updateData()
	{
		int lv = UserManager.Instance.self.getUserLevel();
		List<PictureTipsSample> allPicTipsSamples = PictureTipsSampleConfigManager.Instance.allPictureTips;
		int offset = 0;
		for (int i = 0; i < allPicTipsSamples.Count; i++)
		{
			PictureTipsSample sample = allPicTipsSamples[i];
			/** 条目开启标值 */
			bool DontHaveCount = PictureManagerment.Instance.currentSample.missionSids.Count == 0;
			bool isOn_0 = PictureManagerment.Instance.currentSample.isON[0]== 0;
			bool isOn_1 = PictureManagerment.Instance.currentSample.isON[1]== 0;
			bool isOn_2 = PictureManagerment.Instance.currentSample.isON[2]== 0;
			bool isOn_3 = PictureManagerment.Instance.currentSample.isON[3]== 0;

			/** 所有条目均没有，则显示提示标签 */
			if(DontHaveCount&&isOn_0&&isOn_1&&isOn_2&&isOn_3)
			{
				officeTips.gameObject.SetActive(true);
			}
			/** 如果类型为讨伐且没有配关卡ID,则跳过不显示该项 */
			if(sample.sid == 1 && DontHaveCount){
				offset ++;
				continue;
			}
			/** 如果对应类型开关没有开启，则跳过不显示该项 */
			if(sample.sid == 2 && isOn_0){
				offset++;
				continue;
			}if(sample.sid == 3 && isOn_1){
				offset++;
				continue;
			}if(sample.sid == 4 && isOn_2){
				offset++;
				continue;
			}if(sample.sid == 5 && isOn_3){
				offset++;
				continue;
			}

			int index = i - offset;
			PictureTipsItem pictureTipsItem = GameObject.Instantiate(tipsItem) as PictureTipsItem;
			pictureTipsItem.setFatherWindow(this);
			pictureTipsItem.setData(sample);
			Transform t = pictureTipsItem.transform;
			t.parent = content.transform;
			t.localPosition = Vector3.zero;
			t.localRotation = Quaternion.identity;
			t.localScale = Vector3.one;
			content.Reposition();
//			iTween.MoveTo(t.gameObject, iTween.Hash("isLocal", true, "position", new Vector3(0, -index * content.cellHeight, 0), "time", 1f));
//			yield return new WaitForSeconds(0.1f);
		}
	}

}
