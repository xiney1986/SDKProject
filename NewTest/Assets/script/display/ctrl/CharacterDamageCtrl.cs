using UnityEngine;
using System.Collections;
using System.Collections.Generic;

struct damage
{
public 	int hitValue;
public 	int type;	

}

public class CharacterDamageCtrl : MonoBase
{
	damage[] damage;
	CharacterData character;
	
	public   void init (CharacterData _character)
	{
		character = _character;
		List<BuffCtrl> damageBuff = _character.characterCtrl.damageBuff;
		damage = new damage[damageBuff.Count];
		for (int i=0; i< damageBuff.Count; i++) {
			damage [i].hitValue = damageBuff [i].serverData.serverDamage;
			damage [i].type = damageBuff [i].serverData.sample.getDamageType();
 
		}	
		
		StartCoroutine (multipleDamage ());
	}

	IEnumerator multipleDamage ()
	{
		float offset=0;
		
		for (int i=0; i<damage.Length; i++) {
			EffectManager.Instance.CreateHitText (character.characterCtrl, damage [i].hitValue,damage [i].type, offset);
			offset += 0.03f;
			yield return new WaitForSeconds(0.1f);
		}
		
		Destroy (this);

	}
	
	
}
