using UnityEngine;
using System.Collections;

/// <summary>
/// 坐骑属性条目
/// </summary>
public class AngelItem : MonoBase {
	/** 3d模型点 */
	public GameObject angel3dModel;
	/** 坐骑模型阴影图 */
	public GameObject angelModelShadows;
	/** 父窗口 */
	WindowBase fatherWindow;

	/*  fields */
	/** 守护天使 */
	AngelSample angel;

	/* methods */
	/***/
	public void init(WindowBase fatherWindow,AngelSample _angel) {
		this.fatherWindow = fatherWindow;
		this.angel = _angel;
		UpdateUI();
	}
	/** 更新UI */
	public void UpdateUI() {
		update3DModel();
	}

	/** 更新3D模型 */
	private void update3DModel() {
		if(angel3dModel.transform.childCount>0)
			Utils.RemoveAllChild(angel3dModel.transform);
		if(angel==null) {
			angelModelShadows.SetActive(true);
		} else {
			angelModelShadows.SetActive(false);
			createMountsModel((obj)=>{
				if(obj!=null) {
					FuBenCardCtrl angelAnimCtrl=obj.transform.GetChild (0).GetComponent<FuBenCardCtrl> ();
					Utils.SetLayer (obj, UiManager.Instance.gameCamera.gameObject.layer);
					if (angelAnimCtrl != null)
						angelAnimCtrl.playStand();
				}
			});
		}
	}
	/** 创建坐骑模型 */
	private void createMountsModel (CallBack<GameObject> callback) {
		ResourcesManager.Instance.LoadAssetBundleTexture ("angel/"+angel.modelID,angel3dModel.transform,(obj)=>{
			GameObject gameObj=obj as GameObject;
			Transform temp=gameObj.transform;
			temp.localPosition = Vector3.zero;
			temp.localRotation = new Quaternion (0, 0, 0, 1);
			if (callback != null) {
				callback(gameObj);
			}
		});
	}
	//***/
	public void removeAngelModel(){
		if(angel3dModel.transform.childCount>0)
			Utils.RemoveAllChild(angel3dModel.transform);
	}
}