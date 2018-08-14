using UnityEngine;
using System.Collections;

public class AudioPlayer : MonoBehaviour {
	
	public int id;
	public float delay;
	
	// Use this for initialization
	void Start () {
		if(id <= 0)
		{
			return;
		}
		if(delay > 0)
		{
			StartCoroutine(Utils.DelayRun(()=>
			{
				AudioManager.Instance.PlayAudio(id);
			},delay));
		}
		else
		{
				AudioManager.Instance.PlayAudio(id);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
