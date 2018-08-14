using System;
using UnityEngine;
using System.Collections;
 
public class carouselCtrl : MonoBehaviour
{         
	public GameObject[] prizes;
	public UITexture[] icons;
	public UISprite[] qualitys;
	public UITexture[] buttom;

    public UILabel [] cardName;
	public UISprite [] nameBgLeft;
	public UISprite [] nameBgRight;

	GameObject activeimage;
	GameObject moveImage;
	GameObject lastImage;
	public Transform pointLeft;
	public Transform pointRight;
	public Transform pointMiddle;
	public int unit = 386;//间隔单位
	private int maxCount = 10;//最大数量，到顶后重新来
	private int index = 0;
	private string type = "";
	private int[] ids ;
	
	public void setTextureInfo (int[] ids, string type)
	{ 
		if (ids == null || ids.Length < 1)
			throw new Exception (GetType () + "setTextureInfo ids is null or ids.Length <1");
		this.type = type; 
		this.ids = ids;
		
		maxCount = ids.Length;
		for (int i = 0; i < prizes.Length; i++) { 
			prizes [i].gameObject.SetActive(false);
		}
		 
		if (ids.Length < 3) { 
			for (int i = 0; i < ids.Length; i++) {
				changeImage (ids [i], i);
				prizes [i].gameObject.SetActive (true);
			}
		} else {
			for (int i = 0; i < prizes.Length; i++) { 
				changeImage (ids [i], i);
				prizes [i].gameObject.SetActive (true);
			}
			
			activeimage = prizes [1];
			moveImage = prizes [0];
			lastImage = prizes [2];
			index = 2;	
		} 
	}
	   
 
	// Update is called once per frame
	void Update ()
	{
		if (prizes == null || prizes.Length < 1)
			return;
		if (ids.Length == 1) {  
			prizes [0].transform.position = pointMiddle.transform.position;
		} else if (ids.Length == 2) {
			prizes [0].transform.position = pointLeft.transform.position;
			prizes [1].transform.position = pointRight.transform.position; 
		} else {
		
			for (int i=0; i<prizes.Length; i++) {
				prizes [i].transform.localPosition -= new Vector3 (Time.deltaTime * 80f, 0, 0);
			}
			//到达临界点
			if (moveImage.transform.localPosition.x <= -unit * 1.5f) {
			
				//第一页放最后页
				moveImage.transform.localPosition = lastImage.transform.localPosition + new Vector3 (unit, 0, 0);
				index ++;
				if (index == maxCount) {
					index = 0;
				}
			
				changeImage (ids [index], index);
			 
				//各种交换
				GameObject tmp = lastImage;
				lastImage = moveImage;
				moveImage = activeimage;
				activeimage = tmp;
			
			}
		}
		
	}
	
	//改变图片
	public void changeImage (int id, int index)
	{  
		if (type == "card") 
		{
			CardSample s = CardSampleManager.Instance.getRoleSampleBySid (id);
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + s.imageID, icons[index]);
			qualitys[index].spriteName = QualityManagerment.qualityIDToStringByBG (s.qualityId);
           

            //设置卡片底部的卡片名
            string colorName = QualityManagerment.getQualityColor(s.qualityId);
           // cardName[index].text = colorName + s.name + "[-]";
			cardName[index].text = s.name;
			nameBgLeft[index].transform.localPosition = new Vector3((float)(-50 - cardName[index].width / 2),nameBgLeft[index].transform.localPosition.y,nameBgLeft[index].transform.localPosition.z);
			nameBgRight[index].transform.localPosition = new Vector3((float)( 50 + cardName[index].width / 2),nameBgRight[index].transform.localPosition.y,nameBgRight[index].transform.localPosition.z);
            
		//	ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.BACKGROUNDPATH + "backGround_9",buttom[index]);
		}
		else if (type == "equip")
		{
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + EquipmentSampleManager.Instance.getEquipSampleBySid (id).iconId, icons[index]);
		}
		else if (type == "prop") 
		{
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.ICONIMAGEPATH + PropSampleManager.Instance.getPropSampleBySid (id).iconId, icons[index]);
		} else if (type == "beast")
		{
		    if (CommandConfigManager.Instance.getNvShenClothType() == 0)
                ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.CARDIMAGEPATH + CardSampleManager.Instance.getRoleSampleBySid(id).imageID + "c", icons[index]);
            else ResourcesManager.Instance.LoadAssetBundleTexture(ResourcesManager.CARDIMAGEPATH + CardSampleManager.Instance.getRoleSampleBySid(id).imageID, icons[index]);		
		}
	} 
}
