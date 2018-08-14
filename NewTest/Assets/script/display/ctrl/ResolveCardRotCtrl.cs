using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResolveCardRotCtrl : MonoBehaviour
{
	float angel = 0;
	public GameObject[] pointList;
	public ResolveCardShowerCtrl[] castShowers;
	public GameObject luzi;
	public int size;
	float loopTime = 1f;
	public	void flashingBugFix()
	{
		loopTime=1f;
		foreach (ResolveCardShowerCtrl each in castShowers) {
			
			if(each.background.enabled==true){
				each.background.enabled=false;
				each.background.enabled=true;
			}
			if(each.cardImage.enabled==true){
				each.cardImage.enabled=false;
				each.cardImage.enabled=true;
			}
		}
		
	}
	
	//通过card来选中ResolveCardShowerCtrl
	public ResolveCardShowerCtrl selectShowerByCard (Card _card)
	{
		foreach (ResolveCardShowerCtrl each in castShowers) {
			if (each.card == _card) {
				return each;
			}
		}
		return null;
	}
	//祭品满没
	public bool isCasterFull ()
	{
		foreach (ResolveCardShowerCtrl each in castShowers) {
			if (each.card == null) {
				return false;
			}
		}
		return true;
	}
	
	//祭品台是否是空的
	public bool isCasterEmpty ()
	{
		foreach (ResolveCardShowerCtrl each in castShowers) {
			if (each.card != null) {
				return false;
			}
		}
		return true;
	}

	//清理 献祭台
	public void cleanCastShower ()
	{
		foreach (ResolveCardShowerCtrl each in castShowers) {
			each.cleanData ();
		}
	}	
	
	/** 刷新旋转控制台 */
	public void refreshShowerCtrl(List<Card> list){
		foreach (ResolveCardShowerCtrl each in castShowers) {
			if(each.card==null) continue;
			if(list!=null&&list.Contains(each.card)) continue;
			each.cleanData();
		}
	}
	/** 刷新旋转控制台 */
	public void refreshShowerCtrl(List<Equip> list){
		foreach (ResolveCardShowerCtrl each in castShowers) {
			if(each.equip==null) continue;
			if(list!=null&&list.Contains(each.equip)) continue;
			each.cleanData();
		}
	}
    public void refreshShowerCtrl(List<MagicWeapon> list) {
        foreach (ResolveCardShowerCtrl each in castShowers) {
            if (each.magicscrapt == null) continue;
            if (list != null && list.Contains(each.magicscrapt)) continue;
            each.cleanData();
        }
    }
	
	//是否为8个选中的献祭者 中的一个
	public ResolveCardShowerCtrl  isOneOfTheCaster (Card _card)
	{
		foreach (ResolveCardShowerCtrl each in castShowers) {
			if (each == null)
				continue;
			if (each.card == null)
				continue;
			if (each.card == _card)
				return each;
		}
		return null;
	}
	//是否为8个选中的献祭者 中的一个
	public ResolveCardShowerCtrl  isOneOfTheCaster (Equip _equip)
	{
		foreach (ResolveCardShowerCtrl each in castShowers) {
			if (each == null)
				continue;
			if (each.equip == null)
				continue;
			if (each.equip == _equip)
				return each;
		}
		return null;
	}
    public ResolveCardShowerCtrl isOneOfTheCaster(MagicWeapon magicScrapt) {
        foreach (ResolveCardShowerCtrl each in castShowers) {
            if (each == null)
                continue;
            if (each.magicscrapt == null)
                continue;
            if (each.magicscrapt == magicScrapt)
                return each;
        }
        return null;
    }
	//设置献祭者底座开关
	public void showCastShowerbase ()
	{
		foreach (ResolveCardShowerCtrl each in castShowers) {
			if (each.card == null && each.equip == null)
				each.cleanData ();
		}
	}
	
	public void hideCastShowerbase ()
	{
		foreach (ResolveCardShowerCtrl each in castShowers) {
			if(each.card == null && each.equip == null)
				each.cleanAll ();
		}
	}
	
	public ResolveCardShowerCtrl  selectOneEmptyCastShower ()
	{
        if (castShowers[4].card == null && castShowers[4].equip == null && castShowers[4].magicscrapt==null) {
			return castShowers [4];
		}
        if (castShowers[3].card == null && castShowers[3].equip == null && castShowers[3].magicscrapt == null) {
			return castShowers [3];
		}
        if (castShowers[2].card == null && castShowers[2].equip == null && castShowers[2].magicscrapt == null) {
			return castShowers [2];
		}
        if (castShowers[6].card == null && castShowers[6].equip == null && castShowers[6].magicscrapt == null) {
			return castShowers [6];
		}
        if (castShowers[1].card == null && castShowers[1].equip == null && castShowers[1].magicscrapt == null) {
			return castShowers [1];
		}
        if (castShowers[5].card == null && castShowers[5].equip == null && castShowers[5].magicscrapt == null) {
			return castShowers [5];
		}
        if (castShowers[7].card == null && castShowers[7].equip == null && castShowers[7].magicscrapt == null) {
			return castShowers [7];
		}
        if (castShowers[0].card == null && castShowers[0].equip == null && castShowers[0].magicscrapt == null) {
			return castShowers [0];
		}
		
		return null;
	}
	// Update is called once per frame
	void Update ()
	{
		
		loopTime -= Time.deltaTime;
		angel += 0.1f;
		transform.localRotation = Quaternion.AngleAxis (angel, Vector3.up);
		changeDepth(false);
		if (loopTime <= 0)
			loopTime = 0.5f;
	}
	
	public void changeDepth(bool immediately)
	{
		if(immediately)
			loopTime=0;
		for (int i=0; i<pointList.Length; i++) {
			castShowers [i].transform.position = pointList [i].transform.position;
			if (loopTime <= 0) {
				int offset=(int)(- castShowers [i].transform.localPosition.z);
				castShowers [i].changeDepth (offset);
				castShowers [i].changeColorByDepth(offset);
			}
		}
	}
}
