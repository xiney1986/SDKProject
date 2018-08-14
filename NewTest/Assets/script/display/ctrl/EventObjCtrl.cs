using UnityEngine;
using System.Collections;

public class EventObjCtrl : MonoBase
{
	public barCtrl hpBar;
	public GameObject bossImagePanel;
	public Texture2D goldTex;
	public Texture2D sliverTex;

	void Start()
	{
		adjustRoation();
	}
	public void Show (MissionEventSample _event)
	{ 
		iTween.ValueTo (gameObject, iTween.Hash ("from", 0.001f, "to", 1f, "onupdate", "OnScale", "easetype", iTween.EaseType.easeOutElastic, "oncomplete", "ShowOver", "time", 0.6f));		
		if (_event.eventType == MissionEventType.BOSS) {
			changeImage (CardSampleManager.Instance.getRoleSampleBySid (_event.other).imageID);
			createBar();

		} else if (_event.eventType == MissionEventType.TREASURE||_event.eventType==MissionEventType.TOW_TREASURE) {
			changeTreasureType (_event.other);
		}
	}
	void createBar()
	{
		if (hpBar == null) {
			
			passObj obj = Create3Dobj ("Effect/Other/missionBossHpBar");
			obj.obj.transform.parent = UiManager.Instance.missionMainWindow.UIEffectRoot;
			obj.obj.transform.localScale = Vector3.one;
			obj.obj.name = "BOSS_HP";
			hpBar=obj.obj.GetComponent<barCtrl>();
		}
	}
	private void changeImage (int id)
	{
	///	bossImagePanel.renderer.material = new Material (bossImagePanel.renderer.material);
	//	bossImagePanel.renderer.material.mainTexture = null;
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + id, bossImagePanel);
	}
	
	private void changeTreasureType (int id)
	{
		if (id == TreasureType.TREASURE_GOLD)
	//  	bossImagePanel.renderer.material=new Material(	bossImagePanel.renderer.material);
			bossImagePanel.renderer.material.mainTexture = goldTex;
		else
			bossImagePanel.renderer.material.mainTexture = sliverTex;	
	 
	}
	
	public void setBossHP (float nowHp, float maxHp)
	{
		createBar();
		hpBar.updateValue (nowHp, maxHp);
	}
	
	void ShowOver ()
	{
		
	}

	void HideOver ()
	{
		Destroy (gameObject);
	}
	
	public void  destory ()
	{
		HideOver ();
	}
	
	void OnScale (float data)
	{
		
		transform.localScale = new Vector3 (data, data, data);
		
	}

	void Update ()
	{
		if(hpBar!=null && MissionManager.instance!=null && MissionManager.instance.backGroundCamera!=null)
		{
			Vector3 pos = MissionManager.instance.backGroundCamera.WorldToScreenPoint (transform.position);
			pos += new Vector3 (0, 220, 0);
			hpBar.transform.position = UiManager.Instance.gameCamera.ScreenToWorldPoint (pos);
		}
		if(Time.frameCount%10==0)
		{
			adjustRoation();
		}
	}
	private void adjustRoation()
	{
		if(MissionManager.instance!=null)
		{
			Vector3 lookPos = new Vector3 (MissionManager.instance.backGroundCamera.transform.position.x, transform.position.y,MissionManager.instance.backGroundCamera.transform.position.z);
			transform.LookAt(lookPos,Vector3.up);
		}
	}

	public void Hide ()
	{
		//iTween.ValueTo (gameObject, iTween.Hash ("from", 1f, "to", 0.001f, "onupdate", "OnScale", "easetype", iTween.EaseType.easeInBack, "oncomplete", "HideOver", "time", 0.4f));			
		HideOver ();
	}
		
	
}
