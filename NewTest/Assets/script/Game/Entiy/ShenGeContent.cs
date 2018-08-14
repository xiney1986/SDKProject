using UnityEngine;
using System.Collections;

//神格容器
public class ShenGeContent : dynamicContent {
    //神格列表
    //神格显示条预制体
    public GameObject propPerfab;
    private ArrayList propList;
    private Prop[] props;
    private int type;
    private int indexx;

    public void reLoad(ArrayList tempList,int typee,int local) {
        propList = tempList;
        type = typee;
        indexx = local;
        props = new Prop[propList.Count];
        for (int i = 0; i < propList.Count; i++) {
            props[i] = propList[i] as Prop;
        }
        base.reLoad(propList.Count);
    }

    public override void updateItem(GameObject item, int index) {

        ButtonStoreProp button = item.GetComponent<ButtonStoreProp>();
        button.fatherWindow = fatherWindow;
        button.init(props[index], type, indexx);
    }

    public override void initButton(int i) {
        if (nodeList[i] == null) {
            nodeList[i] = NGUITools.AddChild(gameObject, propPerfab);
        }
        nodeList[i].name = StringKit.intToFixString(i + 1);
        ButtonStoreProp button = nodeList[i].GetComponent<ButtonStoreProp>();
        button.fatherWindow = fatherWindow;
        button.init(props[i], type, indexx);
    }

}
