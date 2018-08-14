using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoleNameInput : UIInput
{

	public CallBack onClickFun;
	void OnClick()
	{
		if(onClickFun!=null)
		{
			onClickFun();
		}
	}
}

