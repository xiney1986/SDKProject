using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RankContent : MonoBehaviour
{
	/** 3列用 */
	public GameObject itemPrefab3;
	/** 4列用 */
	public GameObject itemPrefab4;
	private IList list;
	private int type;
	private WindowBase fatherWindow;
	private bool isRefresh = false;

	public void init (int type, IList list, WindowBase fatherWindow)
	{
		this.type = type;
		this.list = list;
		this.fatherWindow = fatherWindow;
//		waitLabel.gameObject.SetActive (true);
		gameObject.transform.GetComponent<UIProgressBar> ().value = 0f;
		gameObject.transform.GetComponent<UIScrollView> ().onDragFinished = () => {
			float p = gameObject.transform.GetComponent<UIProgressBar> ().value;
			if(p>0.6&&gameObject.transform.childCount < list.Count&&!isRefresh){
				isRefresh =	true;
				StartCoroutine ("nextRefresh");
			}
		};
//		if(gameObject.transform.childCount > list.Count)
			for (int i=0; i<gameObject.transform.childCount; i++) {
				Destroy (gameObject.transform.GetChild (i).gameObject);
			}
		if (list.Count > 0)
			StartCoroutine ("refresh");
		else
			noRank ();
//		transform.GetComponent<UIPanel> ().clipOffset = Vector2.zero;
//		transform.localPosition = Vector3.zero;
//		for (int i = 0; i < list.Count; i++) {
//			initButton (i);
//		}
//		transform.GetComponent<UIGrid> ().repositionNow = true;
	}

	private void noRank(){
		initButton (0);
		transform.GetComponent<UIPanel> ().clipOffset = Vector2.zero;
		transform.localPosition = Vector3.zero;
		transform.GetComponent<UIGrid> ().repositionNow = true;
		MaskWindow.UnlockUI ();
	}

	private IEnumerator nextRefresh(){
		int num = gameObject.transform.childCount;
		for (int i = num; i < Mathf.Min(num + 10,list.Count) ; i++) {
			yield return 0f;
			initButton (i);
		}
		transform.GetComponent<UIGrid> ().repositionNow = true;
		isRefresh = false;
		MaskWindow.UnlockUI ();
	}

	private IEnumerator refresh(){
		for (int i = 0; i < Mathf.Min(200,list.Count) ; i++) {
			yield return 0f;
			initButton (i);
		}
		transform.GetComponent<UIPanel> ().clipOffset = Vector2.zero;
		transform.localPosition = Vector3.zero;
		transform.GetComponent<UIGrid> ().repositionNow = true;
		MaskWindow.UnlockUI ();
	}

	public void initButton (int i)
	{
		GameObject obj = null;
		if (fatherWindow is RankWindow) {
			if ((fatherWindow as RankWindow).selectTabType == RankManagerment.TYPE_GUILD_FIGHT) {
				obj = Instantiate (itemPrefab4) as GameObject;				
			} else {
				obj = Instantiate (itemPrefab3) as GameObject;				
			}
		} else {
				obj = Instantiate (itemPrefab3) as GameObject;				
		}
		obj.transform.parent = transform;
		obj.transform.localPosition = new Vector3 (-800, 0, 0);
		obj.transform.localScale = Vector3.one;
		obj.name = StringKit.intToFixString (i + 1);
		RankItemView sc = obj.GetComponent<RankItemView> ();
		sc.setFatherWindow(this.fatherWindow);
		if (i >= list.Count)
			sc.init (null, type, i);
		else
			sc.init (list [i], type, i);
		if (!obj.activeSelf)
			obj.SetActive (true);
	}

	public void updateItem (GameObject item, int index)
	{
		if (list == null || index >= list.Count || list [index] == null)
			return;

		RankItemView sc = item.GetComponent<RankItemView> ();
		sc.init (list [index], type, index);
	}
}
