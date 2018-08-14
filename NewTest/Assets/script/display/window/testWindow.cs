using UnityEngine;
using System.Collections;

public class testWindow : WindowBase
{


public void onClick (){

	
        UiManager.Instance.openWindow<testWindow>();
	}
	public void onClick3 (){
		
        
        UiManager.Instance.openWindow<testWindow>();
		
		
	}

	public void onClick2 (){
        
        UiManager.Instance.openWindow<testWindow>();

	}


}
