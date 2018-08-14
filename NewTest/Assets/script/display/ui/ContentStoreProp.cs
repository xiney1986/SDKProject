using UnityEngine;
using System.Collections;

public class ContentStoreProp : dynamicContent
{
	ArrayList props;
	public GameObject propButtonPrefab;
	public  void Initialize (ArrayList _props)
	{
		props = _props;
		base.reLoad (props.Count);
		 
	}

	public void reLoad (ArrayList _props)
	{
		props = _props;
        GetShenGeInfoFPort fport = FPortManager.Instance.getFPort("GetShenGeInfoFPort") as GetShenGeInfoFPort;
        fport.access(null);
		 base.reLoad (props.Count);
	}
	
	public override void updateItem (GameObject item, int index)
	{
		//base.updateItem (item, index);
		ButtonStoreProp button = item.GetComponent<ButtonStoreProp> ();
		button.UpdateProp (props [index] as Prop);
	}

	public override void initButton (int  i)
	{
		if (nodeList [i] == null){
			nodeList [i] = NGUITools.AddChild (gameObject, propButtonPrefab);
		}
		nodeList [i].name = StringKit.intToFixString (i + 1);
		ButtonStoreProp button = nodeList [i].GetComponent<ButtonStoreProp> ();
		button.fatherWindow = fatherWindow;
		button.UpdateProp (props [i] as Prop);
		


		
	}
}

