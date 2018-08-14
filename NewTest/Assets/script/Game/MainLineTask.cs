using UnityEngine;
using System.Collections;


public class MainLineTask : dynamicContent
{
	Task[] tasks;
	 
	public void Initialize (Task[] _tasks)
	{
		tasks = _tasks;
		base.reLoad (tasks.Length); 
	}
	public void reLoad(Task[] _tasks)
	{
		if(_tasks == null)
			return;
		tasks = _tasks;
		base.reLoad(tasks.Length);
	}
	public override void updateItem (GameObject item, int index)
	{
		//base.updateItem (item, index);
		
		ButtonTask button = item.GetComponent<ButtonTask> ();
		button.updateTask (tasks [index]); 
	}

	public override void initButton (int  i)
	{
		if (nodeList [i] == null){
			nodeList [i] = NGUITools.AddChild (gameObject, (fatherWindow as TaskWindow).taskitem);
		}

		nodeList [i].name = StringKit. intToFixString (i + 1);
		ButtonTask button = nodeList [i].GetComponent<ButtonTask> ();
		button.fatherWindow = fatherWindow; 
		button.initialize (tasks [i]);
	}

	public override void jumpToPage (int index) {
		base.jumpToPage (index);
		if(GuideManager.Instance.isEqualStep(30001000)){
			GuideManager.Instance.doGuide ();
			GuideManager.Instance.guideEvent ();
		}
	}
}
