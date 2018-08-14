using UnityEngine;
using System.Collections;

public class PlayerMoveTest : MonoBehaviour {




	public MissionRoad missionRoad;
	private int testStep=1;
	public Animation anim;

	public GameObject camera;
	public GameObject player;

	private Quaternion TargetRotation;
	void Start () {
		/*
		Vector3 lookPos = camera.transform.position;
		TargetRotation = Quaternion.LookRotation (lookPos - transform.position, Vector3.up);
		*/
	}

	void Update () {
		//player.transform.rotation = Quaternion.Slerp (player.transform.rotation, TargetRotation, Time.deltaTime * 12f);
	}

	void OnGUI()
	{
		/*
		if(GUI.Button(new Rect(10,0,200,100),"Go Step()"))
		{
			if(testStep>missionRoad.totalStep)
			{
				Debug.LogError("#######################Arravie Desitination!!!");
				return;
			}
			missionRoad.M_goStep(testStep,()=>{
				Vector3 currentStepPositioin=missionRoad.M_getPosition(testStep);				
				playMove();
				iTween.MoveTo (gameObject, iTween.Hash ("position", currentStepPositioin, "oncomplete", "moveArrive", "easetype", "easeOutQuad", "time", 0.5f));				
				TargetRotation = Quaternion.LookRotation (currentStepPositioin - transform.position, Vector3.up);
				testStep++;
			});

		}*/

		if(GUI.Button(new Rect(10,0,200,100),"Forward"))
		{
			transform.LookAt(girls);
			transform.Translate(Vector3.forward*10);
		}
		if(GUI.Button(new Rect(250,0,200,100),"Back"))
		{
			transform.LookAt(girls);
			transform.Translate(Vector3.back*10);
		}

	}
	public Transform girls;

	void moveArrive()
	{
		Vector3 lookPos = camera.transform.position;
		TargetRotation = Quaternion.LookRotation (lookPos - transform.position, Vector3.up);

		int i=Random.Range(1,10);
		if(i%3==0)
		{
			playStand();
		}else if(i%4==0)
		{
			playIdle();
		}else
		{
			playIdle();
		}

		if(testStep==missionRoad.totalStep)
		{
			playHappy();;
		}
	}

	public void playMove ()
	{
		anim.Stop ();
		anim.Play ("run");


	}
	public void playFail ()
	{
		anim.Play ("fail");
	}

	public void playStand ()
	{
		
		anim.CrossFade ("stand");
	}
	
	public void playIdle ()
	{
		
		anim.CrossFade ("idle");
	}
	
	public void playHappy ()
	{
		anim.CrossFade ("happy");
	}
}
