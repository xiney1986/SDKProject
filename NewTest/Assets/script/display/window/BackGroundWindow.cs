using UnityEngine;
using System.Collections;

public class BackGroundWindow : WindowBase
{
	public UITexture backGroundMain1;
	UIPanel mainScrollViewPanel;
	Vector3 tempVector3 = new Vector3 ();
	float lastClipOffsetX = 100000000;
	string texturePath;
	bool isDark = true;//黑屏
	bool normalMode = true;


	public void hideAllBackGround (bool removeTexture)
	{
		animRoot.gameObject.SetActive (false);
		texturePath = "";
		isDark = false;
		if (removeTexture)
			backGroundMain1.mainTexture = null;
	}

	public void restoreBackGround ()
	{
		animRoot.gameObject.SetActive (true);
	}
	/// <summary>
	/// 切换背景
	/// </summary>
	public void switchBackGround (string texturePath)
	{
		animRoot.gameObject.SetActive (true);
		backGroundMain1.color = Colors.BACKGROUND_LIGHT;
		if (this.texturePath == texturePath)
			return;
		normalMode = true;
		mainScrollViewPanel=null;
		this.texturePath = texturePath;
		openNormalBackground (texturePath);

	}

	public void switchToDark ()
	{
		if (!isDark) {
			//UiManager.Instance.gameCamera.clearFlags=CameraClearFlags.SolidColor;
			animRoot.gameObject.SetActive (true);
			backGroundMain1.color = Colors.BACKGROUND_DARK;
		}
	}

	public void switchToLight ()
	{
		texturePath = "";
		if (isDark) {
			backGroundMain1.color = Colors.BACKGROUND_LIGHT;
		} 
	}

	public void switchToDynamicBackground ()
	{
		backGroundMain1.color = Colors.BACKGROUND_LIGHT;
		normalMode = false;
		openDynamicBackground ();
	}

	public void openNormalBackground (string texturePath)
	{

		ResourcesManager.Instance.LoadAssetBundleTexture ("texture/backGround/" + texturePath, backGroundMain1);
		backGroundMain1.width = 640;
		backGroundMain1.height = 960;
		backGroundMain1.transform.localPosition = new Vector3 (0, 0, backGroundMain1.transform.localPosition.z);
		isDark = false;
	}
 
	void openDynamicBackground ()
	{
		string fixPath = "texture/backGround/backGround_1";
		if (texturePath != fixPath) {
			ResourcesManager.Instance.LoadAssetBundleTexture ("texture/backGround/backGround_1", backGroundMain1,(obj)=>{

				backGroundMain1.uvRect = new Rect (0, -0.001f, 1, 1);
				backGroundMain1.width = 1024;
				backGroundMain1.height = 960;

			});
			texturePath = fixPath;

		}
		if (UiManager.Instance.mainWindow != null)
			mainScrollViewPanel = UiManager.Instance.mainWindow.launcher.GetComponent<UIPanel> ();
	}

	public override void DoDisable ()
	{
		base.DoDisable ();
	//	UiManager.Instance.backGround = null;
	}

	void Update ()
	{
		UpdateDyncmicBackground ();
	}

	public void UpdateDyncmicBackground ()
	{
		if (normalMode) {
			backGroundMain1.width = 640;
			backGroundMain1.height = 960;
			return;
		}


		if (mainScrollViewPanel != null) {
			float x = mainScrollViewPanel.clipOffset.x - 615;
			if (x < -615)
				x = -615;
			else if (x > 615)
				x = 615;
			if (x != lastClipOffsetX) {
				lastClipOffsetX = x;

				if (backGroundMain1.gameObject.activeInHierarchy) {
					tempVector3.x = -x * 0.3f;
					tempVector3.y = backGroundMain1.transform.localPosition.y;
					tempVector3.z = backGroundMain1.transform.localPosition.z;
					backGroundMain1.transform.localPosition = tempVector3;
				}
			}
			
		} else {


		}
	}

}
