using UnityEngine;
using System.Collections;

public class ContentSkillEquip : ContentBase
{
	
	public GameObject leftArrow;
	public GameObject rightArrow;
	public UILabel label;//是否开放文字描述
	private bool isOpen = true;
	
	public void closeLimit()
	{
		this.isOpen = false;
	} 
	
	public override void updateActive (GameObject obj, int pageNUm)
	{
		base.updateActive (obj, pageNUm);

		if (pageNUm == 1) {
			leftArrow.SetActive (false);
			rightArrow.SetActive (true);
		}
		else if (pageNUm == 2) {
//			if(GuideManager.Instance.guideSid == GuideGlobal.SPECIALSID12)
//			{
//				GetComponent<UIScrollView>().enabled = false;
//				GuideManager.Instance.doGuide(); 
//				GuideManager.Instance.guideEvent();
//			}
//			if(GuideManager.Instance.guideSid == GuideGlobal.SPECIALSID11 || GuideManager.Instance.guideSid == GuideGlobal.SPECIALSID48)
//			{
//				GetComponent<UIScrollView>().enabled = false;
//			}
		}
		else {
			if(isOpen && UserManager.Instance.self.getUserLevel() < GuideGlobal.LIMITLEVEL1)
			{
				label.gameObject.SetActive(true);
			}
			else
			{
				label.gameObject.SetActive(false);
			}
			
			leftArrow.SetActive (true);
			rightArrow.SetActive (false);		
		}
		
	}
	
}
