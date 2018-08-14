using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ContentChooseBar : dynamicContent {

    public List<PrizeSample> prizeList;
    public void init(WindowBase win) 
    {
        this.fatherWindow = win;
    }
    public void Initialize(Mission mis) {
        prizeList = new List<PrizeSample>();
        PrizeSample[] prizes = MissionSampleManager.Instance.getMissionSampleBySid(mis.sid).prizes;
        foreach (PrizeSample prize in prizes) {
            prizeList.Add(prize);
        }
        reLoad(prizeList.Count);

    }

    public override void initButton(int i) {
        if (nodeList[i] == null) {
            nodeList[i] = NGUITools.AddChild(this.gameObject, (fatherWindow as ActivityChooseWindow).goodsPrefab);
        }

        nodeList[i].name = StringKit.intToFixString(i + 1);
        GoodsView button = nodeList[i].GetComponent<GoodsView>();
        button.fatherWindow = fatherWindow;
        button.init(prizeList[i]);
    }
}
