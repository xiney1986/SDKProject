using UnityEngine;
using System.Collections;

public class PvpArmyContent : SampleDynamicContent
{ 
	public UISprite leftArrow;
	public UISprite rightArrow;
	private ButtonPvpInfo button;
	int pvpInfoIndex = 0;
	
	public  void updateButton (GameObject each)
	{
		int oppIndex=StringKit.toInt(each.name) - 1;
		PvpOppInfo[] oppinfos=PvpInfoManagerment.Instance.getPvpInfo().oppInfo;
		if(oppIndex<0||oppIndex>oppinfos.Length-1)
			return;
		PvpOppInfo opp = PvpInfoManagerment.Instance.getPvpInfo().oppInfo[oppIndex];
		pvpInfoIndex ++;
		FormationSample sample = FormationSampleManager.Instance.getFormationSampleBySid(opp.formation);

		button = each.GetComponent<ButtonPvpInfo> ();
		button.initInfo (opp,fatherWindow);
		loadFormationGB(button,sample.getLength(),each);
		CreateFormation(button,opp);
		
	}
	
	//加载阵型对象
	private void loadFormationGB (ButtonPvpInfo button,int formationLength, GameObject root)
	{
		passObj go=FormationManagerment.Instance.getPlayerInfoFormationObj(formationLength);
		go.obj.transform.parent = root.transform;
		go.obj .transform.localPosition = Vector3.zero;
		go.obj .transform.localScale = Vector3.one;
		
		if (go.obj != null) {
			button.formationRoot = go.obj;
			go.obj.transform.localPosition = new Vector3 (0, 235, 0);
		}
		
	}
	
	void CreateFormation (ButtonPvpInfo buttonInfo,PvpOppInfo info)
	{
		GameObject psObj;
		TeamPrepareCardCtrl button;
		PvpOppTeam infoTeam;

		for (int i = 0; i < info.opps.Length; i++) {
			psObj = NGUITools.AddChild(buttonInfo.formationRoot.gameObject,(fatherWindow as PvpMainWindow).cardPrefab);
			if (psObj == null) {
				print ("contentTeamPrepare no res!");
				return;
			} 
	
			psObj.AddComponent<UIDragScrollView>();
			button = psObj.GetComponent<TeamPrepareCardCtrl> ();
			infoTeam = info.opps[i];

			//找到对应的阵形点位
			Transform formationPoint = null;
			formationPoint = buttonInfo.formationRoot.transform .FindChild (FormationManagerment.Instance.getLoctionByIndex (info.formation, infoTeam.index).ToString());
			button.transform.position = formationPoint.position;
			button.updateButton (infoTeam);
			button.initInfo (info.uid, infoTeam.uid, null);
			button.fatherWindow = fatherWindow;
		 }
		
	}

	public  void updateActive (GameObject obj)
	{

		int pageNUm=StringKit.toInt(obj.name);
		PvpInfoManagerment.Instance.setOppIndex(pageNUm - 1);
		CardBookWindow.setChatPlayerUid(PvpInfoManagerment.Instance.getOpp ().uid);
		if (pageNUm == 1) {
			leftArrow.gameObject.SetActive (false);
			rightArrow.gameObject.SetActive (true);
		} 
		else if(pageNUm == 3)
		{
			leftArrow.gameObject.SetActive (true);
			rightArrow.gameObject.SetActive (false);
		}
		else {
			leftArrow.gameObject.SetActive (true);
			rightArrow.gameObject.SetActive (true);	
		}
		
	}
	
}
