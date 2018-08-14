using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PropMessageLineWindow : WindowBase {
	List<PrizeSample> prizeList;
	public PropLineCtrl sample;
	public Transform content;
	int nextLine = 0;//下个横幅位置
	bool started = false;
	bool isUnLockUi=true;
	float lift = 1.5f;
	protected override void begin ()
	{
		base.begin ();
		if(isUnLockUi)
			MaskWindow.UnlockUI ();
	}
	int index = 0;
	public void readOne ()
	{
		lift = 1.5f;
		if (prizeList == null)
			return;
		if(index >= 5){
			MaskWindow.LockUI();
			return;
		}


		if (prizeList.Count == 0 ) {
			started = false;
			return;
		}

		foreach(PrizeSample tmp in prizeList){
			if (started == false) 
				started = true;
			GameObject prizeLine = Instantiate(sample.gameObject) as GameObject;
			prizeLine.transform.parent = content;
			prizeLine.transform.localPosition = sample.transform.localPosition;
			prizeLine.transform.localScale = Vector3.one;
            prizeLine.GetComponent<PropLineCtrl>().Initialize(this, nextLine, tmp);
			prizeLine.SetActive (true);
			prizeList.Remove (tmp);
			nextLine -= 120;

			iTween.MoveTo(content.gameObject,iTween.Hash("islocal",true,"position",new Vector3(0f,content.localPosition.y+60,0f),"EaseType",iTween.EaseType.linear,"time",0.2f));

			if (nextLine <= -500) {
				nextLine = 0;

			}
			index++;
			return;	
		}
	}

	public void RemovedPrize(){
		index--;
	}

	void Update(){
		lift -= Time.deltaTime;
		if(lift < 0){
			finishWindow();
		}
	}

	public void  Initialize (PrizeSample prize)
	{
		Initialize (prize,true);
	}

    public void Initialize(PrizeSample[] prizes) {
        foreach (PrizeSample ps in prizes) {
            Initialize(ps, false);
        }
    }
	public void  Initialize (PrizeSample prize,bool isUnLockUi)
	{
		this.isUnLockUi = isUnLockUi;
			dialogCloseUnlockUI=true;
		if(prizeList == null){
			prizeList = new List<PrizeSample>();
		}
		prizeList.Add(prize);
		if (started == false)

			readOne ();
	}

    public void Initialize(List<PrizeSample> prizeListt,bool  UnlockUi )
    {
        this.isUnLockUi = UnlockUi;
			dialogCloseUnlockUI=true;
		if(prizeList == null){
			prizeList = new List<PrizeSample>();
		}
        prizeList = prizeListt;
		if (started == false)

			readOne ();
    }
    public void init(List<PrizeSample> prizeListt, bool UnlockUi) {
        this.isUnLockUi = UnlockUi;
        dialogCloseUnlockUI = UnlockUi;
        if (prizeList == null) {
            prizeList = new List<PrizeSample>();
        }
        prizeList = prizeListt;
        if (started == false)

            readOne();
    }
}
