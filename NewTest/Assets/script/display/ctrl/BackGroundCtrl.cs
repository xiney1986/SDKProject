using UnityEngine;
using System.Collections;

/// <summary>
/// 背景控制器(包含2D,3D)
/// </summary>
public class BackGroundCtrl : MonoBase {

	/** 动态背景类型 */
	public static int DYNAMIC_ORG_TYPE = 0, DYNAMIC_SYN_TYPE = 1;
	public static Vector3 normalSize = new Vector3 (-6.85f, 10.2f, 10.2f);
	public static Vector3 bigSize = new Vector3 (-10f, 10f, 10f);
	public static Vector3 gangSize = new Vector3 (10f, 10f, 10f);

	public GameObject ImagePanel;
	public Camera camera;
	/** 公会建筑定位点 */
	public Transform[] guildBuildPos;
	public float off;
	Vector3 tempVector3;
	Vector3 lastVector3=new Vector3();
	/** 上一次的同步ImagePanel的世界坐标 */
	Vector3 lastSynPosition=Vector3.zero;
	Vector3 orgPos = new Vector3 (0f, 0f, 1.3f);
	float lastClipOffsetX = 100000000;
	public float maxOffset;
	public float minOffset;
	string texturePath;
	bool  normalMode;
	bool isDark;
	UIPanel mainScrollViewPanel;
	WindowBase winBase;
	int dynamicType;

	void Awake () {
		DontDestroyOnLoad (this);
	}
	public void switchToDark () {
		if (isDark)
			return;
		isDark = true;
	}
	/** 重置上一次的同步ImagePanel的世界坐标 */
	public void resetLastSynPosition() {
		lastSynPosition=Vector3.zero;
	}
	/** 设置动态背景类型 */
	public void setDynamicType(int dynamicType) {
		this.dynamicType = dynamicType;
	}
	/** 设置最小最大偏移量 */
	public void setOffsetValue (float minOffset, float maxOffset) {
		this.minOffset = minOffset;
		this.maxOffset = maxOffset;
	}
	/** 切换动态主界面背景 */
	public void switchMainToDynamicBackground (UIPanel mainScrollViewPanel, string backPath) {
		dynamicType = DYNAMIC_ORG_TYPE;
		UiManager.Instance.backGround.setOffsetValue (-615f, 615f);
		isDark = false;
		normalMode = false;
		if (texturePath != backPath) {
			texturePath = backPath;	
			ResourcesManager.Instance.LoadAssetBundleTexture ("texture/backGround/" + texturePath, ImagePanel, (obj) => {
				ImagePanel.transform.localScale = bigSize;
				ImagePanel.transform.localPosition = orgPos;
			});
		}
		this.mainScrollViewPanel = mainScrollViewPanel;
		tempVector3 = new Vector3();
	}
	/** 切换动态背景--同步panel */
	public void switchSynToDynamicBackground (UIPanel mainScrollViewPanel, string backPath, Vector3 panelScale) {
		dynamicType = DYNAMIC_SYN_TYPE;
		isDark = false;
		normalMode = false;
		if (texturePath != backPath) {
			texturePath = backPath;	
			ResourcesManager.Instance.LoadAssetBundleTexture ("texture/backGround/" + texturePath, ImagePanel, (obj) => {
				ImagePanel.transform.localScale = panelScale;
				ImagePanel.transform.localPosition = orgPos;
			});
		}
		this.mainScrollViewPanel = mainScrollViewPanel;
		if(mainScrollViewPanel.parent!=null)
			winBase=mainScrollViewPanel.parent.gameObject.GetComponent<WindowBase> ();
		if (lastSynPosition != Vector3.zero)
			ImagePanel.transform.position=lastSynPosition;
		tempVector3 = Vector3.zero;
	}

	public void switchBackGround(string texturePath , CallBack callback){
		ImagePanel.SetActive (true);
		ImagePanel.renderer.material.color = Colors.BACKGROUND_LIGHT;
		if (this.texturePath == texturePath)
			return;
		mainScrollViewPanel = null;
		this.texturePath = texturePath;
		ResourcesManager.Instance.LoadAssetBundleTexture ("texture/backGround/" + texturePath, ImagePanel,(obj)=>{
			ImagePanel.transform.localPosition = orgPos;
			if (texturePath == "backGround_1") {
				ImagePanel.transform.localScale = bigSize;
			}
			else if(texturePath == "missionBG"){
				ImagePanel.transform.localScale = new Vector3 (6.8f, 20f, 20f);
				ImagePanel.transform.localPosition = new Vector3 (0, 4f, 1f);
				//SetMissionChooseBG();
            } else if (texturePath == "tower_bg") {//如果是爬塔副本背景
                ImagePanel.transform.localScale = new Vector3(-6.8f, 11f, 20f);
            }
              
			else {
				ImagePanel.transform.localScale = normalSize;
			}

			if(callback !=null){
				callback();
			}
		});
		isDark = false;
	}
	public void switchBackGround (string texturePath) {
		switchBackGround(texturePath,null);
	}
	/***/
	void Update () {
		UpdateDyncmicBackground ();
	}
	/** 更新动态背景 */
	public void UpdateDyncmicBackground () {
		if (normalMode) {
			ImagePanel.transform.localScale = normalSize;
			ImagePanel.transform.localPosition = orgPos;
			return;
		}
		if (mainScrollViewPanel == null)
			return;
		if (dynamicType == DYNAMIC_ORG_TYPE) { // 背景按照panelOffsetX比例移动
			float x = (mainScrollViewPanel.clipOffset.x-maxOffset)*0.1f;
			if (x < minOffset)
				x = minOffset;
			else if (x > maxOffset)
				x = maxOffset;
			if (x != lastClipOffsetX) {
				lastClipOffsetX = x;
				if (gameObject.activeInHierarchy) {
					tempVector3.x = -x * 0.003f;
					tempVector3.y = ImagePanel.transform.localPosition.y;
					tempVector3.z = ImagePanel.transform.localPosition.z;
					ImagePanel.transform.localPosition = tempVector3;
				}
			}
		}
		else if(dynamicType == DYNAMIC_SYN_TYPE) { // 背景与滚动视图panel同步移动

			if(winBase==null||winBase.animRoot==null)
				return;
			Transform rootTrans=winBase.animRoot.transform;
			if(rootTrans.localPosition.x==0 && rootTrans.localPosition.y==0){
				Transform trans=mainScrollViewPanel.transform;
				if(tempVector3==Vector3.zero)
					tempVector3=trans.position;
				if (gameObject.activeInHierarchy) {
					if(trans.position.x<minOffset)
						lastVector3.x=minOffset;
					else if(trans.position.x>maxOffset)
						lastVector3.x=maxOffset;
					else
						lastVector3.x=trans.position.x;
					lastVector3.y=trans.position.y;
					lastVector3.z=trans.position.z;
					// 滚动panel的世界坐标移动增量
					Vector3 v3 = 5f * (lastVector3-tempVector3);
					if(v3.x!=0) {
						ImagePanel.transform.Translate(-v3,Space.World);
						lastSynPosition=ImagePanel.transform.position;
						tempVector3=lastVector3;
					}
				}
			}
		}
	}
//	/** 设置副本选择背景 */
//	public void SetMissionChooseBG () {
//		switchBackGround ("missionBG");
//		ImagePanel.transform.localScale = new Vector3 (6.8f, 20f, 20f);
//		ImagePanel.transform.localPosition = new Vector3 (0, 4f, 1f);
//	}
	public void UpdateMissionChooseBG (float n) {
		ImagePanel.transform.localPosition = new Vector3 (0, 4 - n / 1200 < -5 ? -5 : 4 - n / 1200, 1f);
	}
    public void UpdateTowerChooseBG(float n) {
        ImagePanel.transform.localPosition = new Vector3(0, 0.995f - n / 1200 < -0.77 ? -0.76f : 0.995f - n / 1200, 1f);
    }
	public void ReturnFromMissionChooseWindow () {
		ImagePanel.transform.localScale = normalSize;
		ImagePanel.transform.localPosition = orgPos;
	}

	public void UpdateGuildBG (float n) {
		ImagePanel.transform.localPosition = new Vector3 (4 - n / 1200 < -5 ? -5 : 4 - n / 1200, 0, 1f);
	}

	/// <summary>
	/// 获得指定的公会建筑对应的UI摄像机世界坐标
	/// </summary>
	public Vector3 getBuildPosById (int id) {
		Vector3 tmpPos = Vector3.zero;
		switch (id)
		{
		case 0://骰子
			tmpPos = changeV3 (guildBuildPos[0].position);
			break;
		case 1://大厅
			tmpPos = changeV3 (guildBuildPos[1].position);
			break;
		case 2://学院
			tmpPos = changeV3 (guildBuildPos[2].position);
			break;
		case 3://商店
			tmpPos = changeV3 (guildBuildPos[3].position);
			break;
		case 4://祭坛
			tmpPos = changeV3 (guildBuildPos[4].position);
			break;
		case 5://领地
			tmpPos = changeV3 (guildBuildPos[5].position);
			break;
		}
		return tmpPos;
	}
	/// <summary>
	/// 从场景摄像机世界坐标转换为UI摄像机世界坐标
	/// </summary>
	private Vector3 changeV3 (Vector3 pos) {
		return UiManager.Instance.gameCamera.ScreenToWorldPoint (camera.WorldToScreenPoint (pos));
	}
}