using UnityEngine;
using System.Collections;

public class DoubleRMBNotice : Notice
{
	public DoubleRMBNotice (int sid) : base(sid)
	{ 
        
	}

	public override bool isValid ()
	{
		return DoubleRMBManagement.Instance.IsCanShow (sid);
	}
}
