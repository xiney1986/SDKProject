using UnityEngine;
using System.Collections;
/// <summary>
/// 移动的字体控制
/// </summary>
public class SpriteNumCtrl : MonoBehaviour
{

    public UILabel lableNum;
    private Vector3 newPostion;

    public IEnumerator flyNum(Vector3 oldPostion,long exp,int num)
    {
        //Debug.LogError(oldPostion);
       // newPostion = gameObject.transform.parent.InverseTransformPoint(oldPostion);
        //iTween.RotateTo(this.gameObject, new Vector3(0f, 0f, 0f), 1f);
        lableNum.text = exp +"  x  "+ num;
        iTween.MoveTo(gameObject, iTween.Hash("position", oldPostion,"islocal",true, "easetype", iTween.EaseType.linear, "time", 0.5f));
        yield return new WaitForSeconds(0.5f);
        DestroyImmediate(this.gameObject);
    }

}
