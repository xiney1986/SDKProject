using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 筛选排序管理器
 * 自动生成3D映射贴图预制体
 * */

public class RenderView : MonoBase {
	public const string MODELNAME = "RenderViewAni";
	/** 3d模型点 */
	public GameObject mount3dModel;
	/**只照射3D模型的摄像头 */
	public Camera MainCamera;
	private bool beginUpdate = false;//开始刷新摄像头状态
	private Transform parentTransform;
	private Transform childTransform;
    private FuBenCardCtrl fbCtrl;
	private int nodeHight;
	private int clipSize;
	private int type;
    private bool isShowFail;
	/** 创建出来的形象 */
	private GameObject gameObj;

	/// <summary>
	/// 根据模型路径生成模型
	/// ags0=模型路径 ags1 贴图
	public void init (string path, UITexture textue) {
		init (path, textue, -180f);
	}
    public void init(string path, UITexture textue,bool bo) {
        isShowFail = bo;
        init(path, textue, -180f);
    }
	/// <summary>
	/// 根据模型路径生成模型
	/// </summary>
	public void init (string path, UITexture textue, float size) {
		if (gameObj != null)
			Destroy (gameObj);
		RenderTexture rd = initRenderTexture (textue.width, textue.height, 0);
		rd.depth = 16;
		MainCamera.targetTexture = rd;
		ResourcesManager.Instance.LoadAssetBundleTexture (path, mount3dModel.transform, (obj) => {
			gameObj = obj as GameObject;
            fbCtrl = gameObj.GetComponentInChildren<FuBenCardCtrl>();
            if (isShowFail) fbCtrl.anim.Play("failLoop");
            else fbCtrl.anim.Play("stand");
			gameObj.name = MODELNAME;
			Transform temp = gameObj.transform;
			temp.localScale = new Vector3 (1f, 1f, 1f);
			temp.localPosition = Vector3.zero;
			temp.localRotation = new Quaternion (0, size, 0, 1);
			textue.mainTexture = rd;
		});
		MainCamera.aspect = 1;
	}

	public void setCamOrg(float size){
		MainCamera.orthographicSize = size;
	}
	/// <summary>
	/// 更新摄像头
	/// parentTransform == content
	/// childTransform == 001
	/// nodeHight = content nodeHight
	/// clipSize == panel Size x or y
	/// </summary>
	public void updateCam (Transform parentTransform, Transform childTransform, int nodeHight, int clipSize, int type) {
		this.parentTransform = parentTransform;
		this.childTransform = childTransform;
		this.nodeHight = nodeHight;
		this.clipSize = clipSize;
		this.type = type;
		beginUpdate = true;

	}

	public RenderTexture initRenderTexture(int width,int height,int depth) {
		RenderTexture rd;
		if (SystemInfo.SupportsRenderTextureFormat (RenderTextureFormat.ARGB4444)) {
			rd = new RenderTexture (width, height, depth, RenderTextureFormat.ARGB4444);
		}
		else {
			rd = new RenderTexture (width, height, depth, RenderTextureFormat.ARGB32);
		}
		return rd;
	}	
}