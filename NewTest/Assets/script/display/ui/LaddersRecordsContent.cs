using UnityEngine;
using System.Collections;

public class LaddersRecordsContent : dynamicContent {

	/* gameobj fields */
	public GameObject laddersRecordsItem;
	LaddersRecordInfo[] records ;
	/** load */
	public void reLoad (LaddersRecordInfo[] records) {
		if (records == null)
			return;
		this.records = records;
		base.reLoad (records.Length);

	}

	public override void updateItem (GameObject item, int index) {
		LaddersRecordsItem lr = item.GetComponent<LaddersRecordsItem> ();
		lr.init(fatherWindow,records[index].description,records [index].index.ToString());
	}
	/** 初始化条目信息 */
	public override void initButton (int  i) {
		if (nodeList [i] == null){
			nodeList [i] = NGUITools.AddChild (gameObject, laddersRecordsItem);
		}
	}

}
