using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class contentTeamPrepare : ContentBase
{
	TeamPreparePageCtrl ctrl ;
	TeamPrepareWindow window ;
	public GameObject roleViewPrefab;
	int armyIndex = 1;
	 Army army;

	public const float OFFSET_Y = -0.2f;

	public override void CreateButton (int index, GameObject page, int buttonIndex)
	{
		
	}
	
	public override void initAllButton (GameObject each)
	{
		base.initAllButton (each);
		//window = fatherWindow as TeamPrepareWindow;
		army = ArmyManager.Instance.getArmy (armyIndex);
		armyIndex++;
		if (army == null)
			return;
		loadFormationGB (army.getLength (), each);
		ctrl = each.GetComponent<TeamPreparePageCtrl> ();
		
		Card beast = StorageManagerment.Instance.getBeast (army.beastid);
		if (beast != null) {
			ctrl.GuardianForce.gameObject.SetActive (true);
			//显示召唤兽信息
			ctrl.GuardianForce.gameObject.SetActive (true);
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + beast.getImageID () + "s", ctrl.GuardianForce);
			ctrl.beastLevel.text = "Lv." + StorageManagerment.Instance.getBeast (army.beastid).getLevel ().ToString ();
		} else {
			ctrl.GuardianForce.gameObject.SetActive (false);
			ctrl.beastLevel.text = "";
			ctrl.beastLevelBg.SetActive(false);
		}

		//计算战斗力
	

		//ctrl.combat.text = LanguageConfigManager.Instance.getLanguage ("s0368") + ArmyManager.Instance.getTeamCombat(armyIndex - 1);
		//遍历队伍，创建卡片图
		CreateCard ();

		
	}

	private int subSet (string str)
	{
		int result = 0;
		result = int.Parse (str.Substring (str.Length - 1));
		return result;
	}

	void CreateCard ()
	{ 
		List<Card> cards = new List<Card>();
		for (int i=0; i<5; i++) { 
			if (army.players [i] != null && army.players [i] != "0") {
				GameObject obj = NGUITools.AddChild(ctrl.formationRoot,roleViewPrefab);
				obj.transform.localScale = new Vector3(0.7f,0.7f,1);
				RoleView view = obj.GetComponent<RoleView>();
				Card c = StorageManagerment.Instance.getRole (army.players [i]);
				if (c != null) {
					view.hideInBattle = true;
					view.init(c,window,null);
					cards.Add(c);

					//找到对应的阵形点位
					Transform formationPoint = null;
					formationPoint = ctrl.formationRoot.transform .FindChild (army.getLoctionByIndex (i).ToString ());
					
					if (formationPoint == null) {
						Debug.LogError ("can't find the formatPoint in " + army.getLoctionByIndex (i));
						continue;
					}
					view.transform.localPosition = new Vector3(formationPoint.localPosition.x,formationPoint.localPosition.y * 0.5f,0);
					createAlternate (i, formationPoint); 
				}
				
			} else if (army.players [i] != null && army.players [i] == "0") {
			
				Transform formationPoint = null;
				int loc = army.getLoctionByIndex (i);
				if (loc != -1) {
					formationPoint = ctrl.formationRoot.transform .FindChild (loc.ToString ());
					createAlternate (i, formationPoint);
				}
			}
		}
		int combat = CombatManager.Instance.getTeam_MainCombat(army);
		ctrl.combat.text = combat.ToString ();
	}

	void createAlternate (int i, Transform parterTran)
	{
		//看看有没替补
		if (army.alternate [i] != null && army.alternate [i] != "0") {
			GameObject obj = NGUITools.AddChild(ctrl.substituteRoot,roleViewPrefab);
			obj.transform.localScale = new Vector3(0.7f,0.7f,1);
			RoleView view = obj.GetComponent<RoleView>();
			Card sub = StorageManagerment.Instance.getRole (army.alternate [i]);
			view.hideInBattle = true;
			view.init(sub,window,null);
			//根据对应的上场队员确定该替补的位置,为了让他们在显示在一条直线上
			view.transform.localPosition = new Vector3 (parterTran.localPosition.x, OFFSET_Y, 0);
		}
	}
	//加载阵型对象
	private void loadFormationGB (int formationLength, GameObject root)
	{
 			
		
		GameObject obj = FormationManagerment.Instance.loadFormationPrefab (formationLength, root,true);
		
		if (obj != null) {
			root.GetComponent<TeamPreparePageCtrl> ().formationRoot = obj;
			obj .transform.localPosition = new Vector3 (0, 304, 0);
			obj.transform.localScale = new Vector3(0.95f,0.95f,1);
		}
		
	}
}
