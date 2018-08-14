using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResolveCardContentNew : MonoBase {
	public ResolveCardRotCtrl sacrificeRotCtrl;//旋转控制器
	private ResolveWindow win;
	private float time; 
	public const float SWITCHTIME = 3f;
	public bool playEnd = false;
	
	void Update () {
		time -= Time.deltaTime;
		if (time <= 0) {
			time = SWITCHTIME;
		}
	}
	public void initInfo (ResolveWindow win) {
		this.win = win;
	}
	public void updateCtrl (List<Card> _list) {
		List<Card> list = _list;
		sacrificeRotCtrl.refreshShowerCtrl (list);
		if (list == null)
			return;
		for (int i = 0; i <sacrificeRotCtrl.castShowers.Length; i++) {
			if (sacrificeRotCtrl.castShowers [i].card == null)
				continue;
			for (int ii = 0; ii < list.Count; ii++) {
				if (list [ii].uid == sacrificeRotCtrl.castShowers [i].card.uid) {
					continue;
				}
			}
			//遍历完都没,说明选择的时候已经移除了此人
			sacrificeRotCtrl.castShowers [i].cleanAll ();
		}
		for (int i = 0; i < list.Count && i < 8; i++) {
			if (sacrificeRotCtrl.isOneOfTheCaster (list [i]))
				continue;
			sacrificeRotCtrl.selectOneEmptyCastShower ().updateShower (list [i]);
		}
	}
    public void updateCtrl(List<MagicWeapon> _list) {
        List<MagicWeapon> list = _list;
        sacrificeRotCtrl.refreshShowerCtrl(list);
        if (list == null) return;
        for (int i = 0; i < sacrificeRotCtrl.castShowers.Length; i++) {
            if (sacrificeRotCtrl.castShowers[i].magicscrapt == null)
                continue;
            for (int ii = 0; ii < list.Count; ii++) {
                if (list[ii].uid == sacrificeRotCtrl.castShowers[i].magicscrapt.uid) {
                    continue;
                }
            }
            //遍历完都没,说明选择的时候已经移除了此人
            sacrificeRotCtrl.castShowers[i].cleanAll();
        }
        for (int i = 0; i < list.Count && i < 8; i++) {
            if (sacrificeRotCtrl.isOneOfTheCaster(list[i]))
                continue;
            sacrificeRotCtrl.selectOneEmptyCastShower().updateShower(list[i]);
        }
    }
	public void updateCtrl (List<Equip> _list) {
		List<Equip> list = _list;
		sacrificeRotCtrl.refreshShowerCtrl (list);
		if (list == null)
			return;
		for (int i = 0; i <sacrificeRotCtrl.castShowers.Length; i++) {
			if (sacrificeRotCtrl.castShowers [i].equip == null)
				continue;
			for (int ii = 0; ii < list.Count; ii++) {
				if (list [ii].uid == sacrificeRotCtrl.castShowers [i].equip.uid) {
					continue;
				}
			}
			//遍历完都没,说明选择的时候已经移除了此人
			sacrificeRotCtrl.castShowers [i].cleanAll ();
		}
		for (int i = 0; i < list.Count && i < 8; i++) {
			if (sacrificeRotCtrl.isOneOfTheCaster (list [i]))
				continue;
			sacrificeRotCtrl.selectOneEmptyCastShower ().updateShower (list [i]);
		}
	}
	public void oneKey () {
		if (IntensifyCardManager.IsOpenOneKeyWnd) {
			UiManager.Instance.openDialogWindow<OneKeyWindow> ();
		}
		else
			OneKeyChoose ();
	}
	
	public void OneKeyChoose () {

	}
	IEnumerator showEffect () {
		int count = 0;
		foreach (ResolveCardShowerCtrl each in sacrificeRotCtrl.castShowers) {
			if (each.card != null || each.equip != null || each.magicscrapt!= null) {
				yield return new WaitForSeconds (Random.Range (0.04f, 0.2f));
				each.cleanData ();
				count += 1;
				EffectManager.Instance.CreateEffectCtrlByCache (transform, "Effect/UiEffect/Reinforced_SyntheticONE", (obj,ctrl) => {
					ctrl.transform.position = each.transform.position;
				});
			}
		}
		yield return new WaitForSeconds (0.2f);
		EffectManager.Instance.CreateEffectCtrlByCache (transform, "Effect/UiEffect/Reinforced_SyntheticTwo", (obj,bigCtrl) => {
			bigCtrl.transform.position = new Vector3 (sacrificeRotCtrl.luzi.transform.position.x - 0.05f, sacrificeRotCtrl.luzi.transform.position.y + 0.13f, sacrificeRotCtrl.luzi.transform.position.z);
			//sacrificeRotCtrl.luzi.transform.position;
		});
		for (int i=0; i<count; i++) {
			yield return new WaitForSeconds (Random.Range (0.04f, 0.1f));
			EffectManager.Instance.CreateEffectCtrlByCache (transform, "Effect/UiEffect/Reinforced_SyntheticThree", (obj,ctrl) => {
				ctrl.transform.position = new Vector3 (sacrificeRotCtrl.luzi.transform.position.x - 0.05f, sacrificeRotCtrl.luzi.transform.position.y + 0.13f, sacrificeRotCtrl.luzi.transform.position.z);
				iTween.MoveTo (ctrl.gameObject, iTween.Hash ("position", sacrificeRotCtrl.luzi.transform.position, "easetype", iTween.EaseType.easeOutCubic, "time", 0.3f));
			});
		}	
		yield return new WaitForSeconds (2f);
		sacrificeRotCtrl.hideCastShowerbase ();
		playEnd = true;
	}
	public void playEffect () {
		StartCoroutine (showEffect ());
	}
}
