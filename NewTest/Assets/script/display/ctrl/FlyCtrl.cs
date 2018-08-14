using UnityEngine;
using System.Collections;

/// <summary>
/// 纹理图飞行控制类
/// </summary>
public class FlyCtrl : MonoBehaviour {

	/** 飞行对象 */
	public GameObject itemObjPoint;
	/** 动画结束回调 */
	public CallBack<GameObject> overCallBack;
	/** 飞向的目标对象 */
	GameObject targetGameObj;

	/// <summary>
	/// 初始化纹理图飞行控制器
	/// </summary>
	/// <param name="itemObj">飞起来的对象</param>
	/// <param name="moveToPos">飞行缓冲目标点</param>
	/// <param name="position">飞向的目标点</param>
	public void Initialize (GameObject itemObj,Vector3 moveToPos,GameObject targetGameObj) {
		this.targetGameObj = targetGameObj;
		GameObject obj = NGUITools.AddChild(itemObjPoint,itemObj) as GameObject;
		obj.SetActive (true);
		transform.localScale = Vector3.one;
		Vector3 position = targetGameObj.transform.position;
		iTween.MoveTo (gameObject, iTween.Hash ("position", transform.position + moveToPos, "easetype", iTween.EaseType.easeInOutCubic, "time", 0.3f));	
		iTween.ScaleTo (gameObject, iTween.Hash ("scale", new Vector3 (1.4f, 1.4f, 1.4f), "easetype", iTween.EaseType.easeInOutCubic, "time", 0.3f));		
		Vector3 pos = position; 
		iTween.MoveTo (gameObject, iTween.Hash ("delay", 0.2f, "position", pos, "easetype", "easeInQuad", "time", 0.2f));
		iTween.ScaleTo (gameObject, iTween.Hash ("delay", 0.2f, "scale", new Vector3 (0.2f, 0.2f, 0.2f), "easetype", "easeInQuad", "oncomplete", "over", "time", 0.2f));	
	}
	/** 飞行动画结束 */
	void over () {
		if (overCallBack != null)
			overCallBack (targetGameObj);
		gameObject.SetActive (false);
		GameObject.Destroy (gameObject);
	}
}