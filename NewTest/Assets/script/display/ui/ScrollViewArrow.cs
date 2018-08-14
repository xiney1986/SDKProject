using UnityEngine;
using System.Collections;

/**
 * yxl
 * 控制箭头显示隐藏
 */
public class ScrollViewArrow : MonoBehaviour {
	
	public GameObject arrow1;
	public GameObject arrow2;
	public float offset;

	UIPanel panel;
	UIScrollView scrollView;
	float lastOffset = 10000000;
	float delayTime = 1;

	void Start () {
		panel = GetComponent<UIPanel> ();
		scrollView = GetComponent<UIScrollView> ();
	}

	void Update () {
		delayTime += Time.deltaTime;
		if (delayTime < 0.2f)
			return;
		delayTime = 0;

		Bounds b = scrollView.bounds;
		if (scrollView.movement == UIScrollView.Movement.Horizontal) {
			if(b.center.x > 0){
				if(lastOffset == panel.clipOffset.x)
					return;
				lastOffset = panel.clipOffset.x;
				if(arrow1 != null)
					arrow1.SetActive(panel.clipOffset.x > 0);
				if(arrow2 != null)
					arrow2.SetActive(panel.clipOffset.x + panel.finalClipRegion.z  - panel.clipSoftness.x*2 + offset + 1 < b.extents.x * 2);
			}else{
				if(arrow1 != null)
					arrow1.SetActive(false);
				if(arrow2 != null)
					arrow2.SetActive(false);
			}
		} else {
			if(b.center.y > 0){
				if(lastOffset != panel.clipOffset.y)
					return;
				lastOffset = panel.clipOffset.y;
				if(arrow1 != null)
					arrow1.SetActive(panel.clipOffset.y > 0);
				if(arrow2 != null)
					arrow2.SetActive(panel.clipOffset.y + panel.finalClipRegion.w  - panel.clipSoftness.y*2 + offset + 1 < b.extents.y * 2);
			}else{
				if(arrow1 != null)
					arrow1.SetActive(false);
				if(arrow2 != null)
					arrow2.SetActive(false);
			}
		}
	}
}
