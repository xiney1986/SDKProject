using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// 女神选择窗口 
/// </summary>
public class GoddessSelectWindow : WindowBase {
	/**filed */
	/**拥有的女神列表 */
	private List<Card> list;
	/**父类窗口 */
	private GoddessUnitWindow winn;
	/**提示组建 */
	public GameObject point;
	public GoddessSelectItem[] items;
	/**method */

	protected override void begin ()
	{
		base.begin ();
		updateBeastList();
		if(list==null||list.Count<1){
			point.SetActive(true);
			for(int i=0;i<items.Length;i++){
				items[i].init(winn,null,items[i].gameObject.name);
			}
		}else{
			//content.reLoad(list,winn);
			updateUI();
			point.SetActive(false);
		}
		MaskWindow.UnlockUI();
	}
	public void init(GoddessUnitWindow win){
		winn=win;
	}
	/// <summary>
	/// 更新现有的女神列表（并且剔除满级的和比玩家等级高的） 
	/// </summary>
	private void updateBeastList(){
		ArrayList beastList=StorageManagerment.Instance.getAllBeast();
		if(beastList!=null){
			for (int k = 0; k < beastList.Count; k++) {
				Card ca=beastList[k] as Card;
				if(ca.getLevel()<StorageManagerment.Instance.getRole(UserManager.Instance.self.mainCardUid).getLevel()&&ca.getLevel()<ca.getMaxLevel()){
					if(list==null)list=new List<Card> ();
					list.Add(beastList[k] as Card);
				}
			}
		}
	}
	/** button点击事件 */
	public override void buttonEventBase (GameObject gameObj) {
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			finishWindow ();
		} else if(gameObj.name=="goddessButton"){
			UiManager.Instance.openDialogWindow<GoddessSelectWindow>();
		}
	}
	void updateUI(){
		List<string> namelist=new List<string>();
		for(int j=0;j<list.Count;j++){
			namelist.Add(list[j].getImageID().ToString());
		}
		for(int i=0;i<items.Length;i++){
			if(namelist.Contains(items[i].gameObject.name)){
				for(int m=0;m<list.Count;m++){
					if(list[m].getImageID().ToString()==items[i].gameObject.name){
						items[i].init(winn,list[m],"");
						break;
					}
				}
			}else{
                items[i].init(winn, null, items[i].gameObject.name);
			}
		}
	}
}
