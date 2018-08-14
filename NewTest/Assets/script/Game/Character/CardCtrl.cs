using UnityEngine;
using System.Collections;

//卡片图像控制
// @author 李程
public class CardCtrl : MonoBehaviour
{
//	public UITexture tex;	
	public GameObject  cardPanel;
	CharacterData character;
	public  Color COLOR_NORMAL = new Color (0.1f, 0.1f, 0.1f, 1);
	public  Color COLOR_DARK = new Color (0, 0, 0, 1);
	public  Color COLOR_RED = new Color (0.1f, 0, 0, 1);


	// Use this for initialization
	public void init ()
	{
		if (character == null)
			character = transform.parent.GetComponent<CharacterCtrl> ().characterData;
		loadPicture (null);

	}
	/// <summary>
	/// 加载指定cardId的卡片图
	/// </summary>
	/// <param name="id">变身后的id,要取卡片默认图传null.</param>
	private void loadPicture (string id)
	{
		string iconId = "-1";
		if (string.IsNullOrEmpty (id)) {
			if (character.role.isBeast ()) {
				if(character.role.getQualityId()>1){
					iconId = character.role.getImageID () + "c";
				} else
				{
				    if (CommandConfigManager.Instance.getNvShenClothType() == 0)
				        iconId = character.role.getImageID() + "c";
				    else iconId = character.role.getImageID() + "";
				}
			} else {
				iconId = character.role.getImageID (character.EvoLevel).ToString ();
			}
		}
		else
			iconId = id;//id不为0 则是要变身
		cardPanel.renderer.material = new Material (cardPanel.renderer.material);
		if (iconId == "-1") {
			ResourcesManager.Instance.LoadAssetBundleTexture (UserManager.Instance.self.getImagePath (), cardPanel);
		} else {
			ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.CARDIMAGEPATH + iconId, cardPanel);
		}
		cardPanel.renderer.material.SetColor ("_color", COLOR_NORMAL);
	}
	//变形冷却
	public void changePicture (string id)
	{
        
		loadPicture (id);

	}

	//变形冷却
	public void transfromCoolDown ()
	{

		if (character.isInBattleField == false)
			return;

		iTween.ValueTo (gameObject, iTween.Hash ("from", 1f, "to", 0.1f, "onupdate", "colorChange", "oncomplete", "coolDownComplete", "time", 0.5f));
	}

	//女神攻击
	public void	GuardianForceMoveToBattle ()
	{
		if (character.isInBattleField == false)
			return;

		BattleManager.Instance.changeBackGroundColor (new Color(0.05f,0.05f,0.05f,1f));
		BattleManager.Instance.playerTeam.hideAllParter ();
		BattleManager.Instance.enemyTeam.hideAllParter ();

		iTween.MoveTo (character.characterCtrl.gameObject, iTween.Hash ("position", BattleManager.Instance.nvshenPoint.position, "oncomplete", "GuardianForceReady", "easetype", "easeInQuad", "time", 0.5f));
	}
	
	public void	goToHelp ()
	{
		if (character.isInBattleField == false)
			return;

		iTween.MoveTo (character.characterCtrl.gameObject, iTween.Hash ("position", character.characterCtrl.activeAction.Skill.position, "easetype", "easeInQuad", "time", 0.03f));
		//iTween.ValueTo (character.characterCtrl.gameObject, iTween.Hash ("from", 1f, "to", 0f, "onupdate", "", "oncomplete", "goHelpReady", "time", 0.8f  ));
	}


/// <summary>
/// 人物渐变消失
/// </summary>
	public void disAppear ()
	{
		if (!character.isInBattleField)
			return;
		//放这个表示此人已死	 
		iTween.ValueTo (gameObject, iTween.Hash ("from", 1f, "to", 0f, "onupdate", "alphaChange", "oncompletetarget", character.characterCtrl.gameObject, "oncomplete", "colorAnimComplete", "time", 0.3f));
		iTween.MoveTo (character.characterCtrl.gameObject, iTween.Hash ("position", character.orgPosition, "easetype", "easeOutQuad", "time", 0.2f));
	}

	public void	ChargeAOE ()
	{
		if (character.isInBattleField == false)
			return;

		//冲锋到对面队伍中心搞
		iTween.MoveTo (character.characterCtrl.gameObject, iTween.Hash ("position", character.characterCtrl.activeAction.Skill.targets [0].parentTeam.TeamHitPoint.position, "oncomplete", "beginMeleeAOE", "easetype", "easeInQuad", "time", 0.03f));
	}

	public void	jumpAOE ()
	{
		if (character.isInBattleField == false)
			return;

		//冲锋到对面队伍中心搞
		iTween.MoveTo (character.characterCtrl.gameObject, iTween.Hash ("position", character.characterCtrl.activeAction.Skill.targets [0].parentTeam.TeamHitPoint.position, "easetype", iTween.EaseType.easeOutQuad, "time", 0.6f));
	
		iTween.ValueTo (character.characterCtrl.gameObject, iTween.Hash ("from", character.orgScale.x + 0.5f, "to", character.orgScale.x + 6f, "easetype", iTween.EaseType.easeOutQuad, "onupdate", "SideChange", "time", 0.55f));
		iTween.ValueTo (character.characterCtrl.gameObject, iTween.Hash ("delay", 0.55f, "from", character.orgScale.x + 6f, "to", character.orgScale.x + 0.5f, "onupdate", "SideChange", "oncomplete", "jumpOver", "time", 0.05f));

		float size = BattleManager.Instance.BattleCamera.orthographicSize;
		iTween.ValueTo (BattleManager.Instance.gameObject, iTween.Hash ("from", size, "to", size + 1, "easetype", iTween.EaseType.easeOutQuad, "onupdate", "cameraSacle", "time", 0.55f));
		iTween.ValueTo (BattleManager.Instance.gameObject, iTween.Hash ("delay", 0.55f, "from", size + 1, "to", size, "onupdate", "cameraSacle", "time", 0.05f));

	}

	//最普通的攻击流程
	public void	goToAttack (bool isBlur)
	{

		if (character.isInBattleField == false)
			return;

		attackGrowUp ();
		float time = 0.08f;

		iTween.MoveTo (character.characterCtrl.gameObject, iTween.Hash ("position", character.characterCtrl.activeAction.Skill.position, "easetype", "easeInQuad", "oncomplete", "goAttackAnimComplete", "time", time));

		if (isBlur) {
			characterBlur (time);
		}
	}

	void characterBlur (float time)
	{
		float delay = 0;
		CharacterBlur blur = character.characterCtrl.gameObject.AddComponent<CharacterBlur> ();
		blur.init ();
		for (int i = 0; i < blur.blurs.Length; i++) {
			delay += time * 0.2f;
			iTween.MoveTo (blur.blurs [i].gameObject, iTween.Hash ("position", character.characterCtrl.activeAction.Skill.position, "easetype", "easeInQuad", "time", time, "delay", delay));
			
		}
		StartCoroutine (Utils.DelayRun (() => {
			blur.Close ();
		}, time + delay));
	}

	public void goToAttack ()
	{
		goToAttack (false);
	}

	public void meleeAOEgoBack (float time)
	{
		if (character.isInBattleField == false)
			return;
		//character.characterCtrl.SkillScaleDownComplete();
		iTween.MoveTo (character.characterCtrl.gameObject, iTween.Hash ("delay", time, "position", character.orgPosition, "easetype", iTween.EaseType.easeOutQuad, "oncomplete", "SkillScaleDownComplete", "time", 0.08f));
		iTween.ValueTo (character.characterCtrl.gameObject, iTween.Hash ("delay", time, "from", character.orgScale.x + 0.5f, "to", character.orgScale.x, "onupdate", "SideChange", "time", 0.2f));
	}

	public IEnumerator goBack ()
	{
		if (character.isInBattleField == false)
			yield break;

		yield return new WaitForSeconds (0.1f);
		iTween.MoveTo (character.characterCtrl.gameObject, iTween.Hash ("position", character.orgPosition, "easetype", "easeOutQuad", "oncomplete", "AttackAnimComplete", "time", 0.08f));
		 
	}

	public void	shake ()
	{
		shake (false);
	}

	public void	shake (bool hitBack)
	{
		if (character.isInBattleField == false)
			return;

		if (character.characterCtrl.state != enum_character_state.deaded && character.characterCtrl.state != enum_character_state.deading)
			cardPanel.renderer.material.SetColor ("_color", COLOR_RED);
 
		if (!hitBack) {
			iTween.ShakePosition (gameObject, iTween.Hash ("amount", new Vector3 (0.1f, 0.05f, 0.1f), "oncomplete", "shakeAnimComplete", "time", 0.2f));
		} else {

			Vector3 pos;
			if (character.camp == TeamInfo.OWN_CAMP)
				pos = character.characterCtrl.transform.position + new Vector3 (0, 0, Random.Range (-0.05f, -0.2f));
			else
				pos = character.characterCtrl.transform.position + new Vector3 (0, 0, Random.Range (0.05f, 0.2f));

			character.characterCtrl.transform.position = pos;
			iTween.ShakePosition (gameObject, iTween.Hash ("amount", new Vector3 (0.1f, 0.05f, 0.1f), "oncomplete", "shakeAnimComplete", "time", 0.2f));
			iTween.MoveTo (character.characterCtrl.gameObject, iTween.Hash ("delay", 0.4f, "position", character.orgPosition, "easetype", iTween.EaseType.easeOutExpo, "time", 0.3f));
		}
	}

	public void	longShake (float ti)
	{
		shakeAnimComplete ();
		if (character.characterCtrl.state != enum_character_state.deaded && character.characterCtrl.state != enum_character_state.deading)
		//	tex.color = Color.red;
			cardPanel.renderer.material.SetColor ("_color", COLOR_RED);
		iTween.ShakePosition (gameObject, iTween.Hash ("amount", new Vector3 (0.1f, 0.05f, 0.1f), "oncomplete", "shakeAnimComplete", "time", ti));
	}

	public void	longShakeComplete ()
	{
		//鞭尸的时候可不能把卡片变亮啊
		if (character.characterCtrl.getNowHp () > 0)
		//	tex.color = Color.white;
			cardPanel.renderer.material.SetColor ("_color", COLOR_NORMAL);
	}

	/// <summary>
	/// 死亡变灰
	/// </summary>
	public void gray ()
	{
		if (character.isInBattleField == false)
			return;
		//放这个表示此人已死	 
		iTween.ValueTo (gameObject, iTween.Hash ("from", 0.1f, "to", 0.03f, "onupdate", "colorChange", "oncomplete", "colorAnimComplete", "time", 0.3f));
		iTween.MoveTo (character.characterCtrl.gameObject, iTween.Hash ("position", character.orgPosition, "easetype", "easeOutQuad", "time", 0.2f));
		
	}
	
	public void	attackGrowUp ()
	{ 
		if (character.isInBattleField == false)
			return;

		iTween.ValueTo (character.characterCtrl.gameObject, iTween.Hash ("from", character.orgScale.x, "to", character.orgScale.x + 1f, "onupdate", "SideChange", "oncomplete", "AttackGrowUpComplete", "time", 0.1f));
	}
	//进场动画
	public void	MoveToBattleField ()
	{
		float _speed = Random.Range (0.5f, 1.5f);
		iTween.MoveTo (character.characterCtrl.gameObject, iTween.Hash ("position", character.orgPosition, "easetype", iTween.EaseType.easeOutCubic, "oncomplete", "onStandInBattleField", "time", _speed));
	}
	//出场动画
	public void MoveOutBattleField ()
	{

		float _speed = Random.Range (0.1f, 0.7f);
		Vector3 _pos = new Vector3 (0, 50, 50f);
		if (character.parentTeam.isGamePlayerTeam)
			_pos = new Vector3 (0, 50, -50f);

		iTween.MoveTo (character.characterCtrl.gameObject, iTween.Hash ("position", character.orgPosition + _pos, "easetype", iTween.EaseType.easeInCubic, "oncomplete", "onOutBattleField", "time", _speed));
	}
	//召唤兽进场展示
	public void	GuardianForceMoveToShow ()
	{
		if (character.isInBattleField == false)
			return;

		iTween.MoveTo (character.characterCtrl.gameObject, iTween.Hash ("position", character.parentTeam.TeamHitPoint.position, "easetype", iTween.EaseType.easeOutCubic, "time", 0.5f));
		iTween.MoveTo (character.characterCtrl.gameObject, iTween.Hash ("delay", 2f, "position", character.orgPosition, "easetype", iTween.EaseType.easeOutCubic, "time", 0.5f));

	}

	/// <summary>
	/// 人物变大+放施法特效+播放攻击效果
	/// </summary>
	/// <param name="time">多长时间后播放攻击效果</param>
	public void	SkillGrowUp (float time)
	{
		if (character.isInBattleField == false)
			return;

		iTween.MoveTo (character.characterCtrl.gameObject, iTween.Hash ("position", character.characterCtrl.transform.position + new Vector3 (0, 0.0f, 0.05f), "easetype", "easeOutQuad", "time", 0.2f));	
		if(!character.isGuardianForce)
			iTween.ValueTo (character.characterCtrl.gameObject, iTween.Hash ("from", character.orgScale.x, "to", character.orgScale.x + 0.5f, "onupdate", "SideChange", "easetype", "easeOutQuad", "time", 0.2f));
		iTween.ValueTo (character.characterCtrl.gameObject, iTween.Hash ("from", 0, "to", 1, "onupdate", "", "oncomplete", "SkillGrowUpComplete", "time", time));	
	}

    public void BackSkillGrowUp(float time)
    {
        iTween.MoveTo(character.characterCtrl.gameObject, iTween.Hash("position", character.characterCtrl.transform.position + new Vector3(0, 0.0f, 0.05f), "easetype", "easeOutQuad", "time", 0.2f));
        if (!character.isGuardianForce)
            iTween.ValueTo(character.characterCtrl.gameObject, iTween.Hash("from", character.orgScale.x, "to", character.orgScale.x + 0.5f, "onupdate", "SideChange", "easetype", "easeOutQuad", "time", 0.2f));
        iTween.ValueTo(character.characterCtrl.gameObject, iTween.Hash("from", 0, "to", 1, "onupdate", "", "oncomplete", "BackSkillGrowUpComplete", "time", time));
    }

	public void	groupCombieSKillScaleDown ()
	{
		if (character.isInBattleField == false)
			return;
		iTween.ValueTo (character.characterCtrl.gameObject, iTween.Hash ("from", character.orgScale.x + 0.5f, "to", character.orgScale.x, "onupdate", "SideChange", "oncomplete", "SkillScaleDownComplete", "easetype", "easeOutQuad", "time", 0.2f));
		iTween.MoveTo (character.characterCtrl.gameObject, iTween.Hash ("position", character.orgPosition, "easetype", "easeOutQuad", "time", 0.2f));	
	}

	public void	SKillScaleDown ()
	{

		if (character.isInBattleField == false)
			return;

		float _time2 = 0.2f;//缩放时间
		if (character.isGuardianForce) {
			//召唤兽退场后要回调
			character.characterCtrl.GuardianForceOut ();
		}else{
			//召唤兽不会缩放卡片
		iTween.ValueTo (character.characterCtrl.gameObject, iTween.Hash ("from", character.orgScale.x + 0.5f, "to", character.orgScale.x, "onupdate", "SideChange", "easetype", "easeOutQuad", "time", _time2));
		}
		//召唤兽返回
		iTween.MoveTo (character.characterCtrl.gameObject, iTween.Hash ("position", character.orgPosition, "easetype", "easeOutQuad", "time", _time2));	
		
	}

	public void moveToAttackPoint ()
	{
		if (character.isInBattleField == false)
			return;

		iTween.MoveTo (character.characterCtrl.gameObject, iTween.Hash ("position", character.characterCtrl.activeAction.Skill.position, "easetype", "easeInQuad", "oncomplete", "moveToAttackPointComplete", "time", 0.1f));
	}
	
	public void attackScaleDown ()
	{
		if (character.isInBattleField == false)
			return;

		iTween.ValueTo (character.characterCtrl.gameObject, iTween.Hash ("delay", 0.08f, "from", character.orgScale.x + 0.5f, "to", character.orgScale.x, "onupdate", "SideChange", "oncomplete", "AttackScaleDownComplete", "time", 0.2f));
			
	}
	
	void colorChange (float _a)
	{
		cardPanel.renderer.material.SetColor ("_color", new Color (_a, _a, _a, 1));
	}

	void alphaChange (float _a)
	{
		cardPanel.renderer.material.SetColor ("_color", new Color (COLOR_NORMAL.r, COLOR_NORMAL.g, COLOR_NORMAL.b, _a));
		
	}

	//抖完扣血，目前可认为只要抖，就是被击
	public 	void shakeAnimComplete ()
	{

		transform.localPosition = Vector3.zero;
		character.characterCtrl.isHurting = false;

		 
		character.characterCtrl.changeHP ();
	 
		//提取需要的值另开线程播放动画
		CharacterDamageCtrl damageCtrl = character.characterCtrl.gameObject.AddComponent<CharacterDamageCtrl> ();
		damageCtrl.init (character);
		
		//鞭尸的时候可不能把卡片变亮啊
		if (character.characterCtrl.getNowHp () > 0)
		//	tex.color = Color.white;
			cardPanel.renderer.material.SetColor ("_color", COLOR_NORMAL);


		//已经播放了的伤害移除;
		character.characterCtrl.damageBuff.Clear ();
		
	} 
}
