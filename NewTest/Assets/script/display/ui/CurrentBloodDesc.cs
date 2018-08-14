using UnityEngine;
using System.Collections;

public class CurrentBloodDesc : MonoBehaviour {

    public UILabel addAttrDesc;
    public UILabel needPropDesc1;
    public UILabel needPropDesc2;

    public void initDesc(string attr,string desc1,string desc2) {
        addAttrDesc.text = attr;
        needPropDesc1.text = desc1;
        needPropDesc2.text = desc2;
    }

    public void init(BloodPointSample bps,int lv)
    {
        int itemSid = BloodConfigManager.Instance.getCurrentItemSid(bps, lv);
        BloodItemSample bis = BloodItemConfigManager.Instance.getBolldItemSampleBySid(itemSid);
        bloodEffect[] beffects = bis.effects;
        string dec = "";
        for (int i = 0; i < beffects.Length; i++)
        {
            dec+=beffects[i].dec;
        }
        addAttrDesc.text = dec;
    }
}
