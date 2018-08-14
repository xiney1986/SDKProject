using UnityEngine;
using System.Collections.Generic;

public class PartialModelTextureLoop:MonoBehaviour
{


	public	int tileX = 1;
	public	int tileY = 1;
	public	int framerate = 16;
		private  List<Vector2> offsetArray ;
		private float secPerFrame ;

		public void Start ()
		{
				StartCoroutine (loop ());

		}

	System.Collections.IEnumerator   loop ()
		{
				int i;
				int j;
				offsetArray = new List<Vector2> ();
				secPerFrame = 1 / (float)framerate;
				renderer.material.SetTextureScale ("_MainTex", new Vector2 (1 / (float)tileX, 1 / (float)tileY));
				for (j = 0; j < tileX; j++) {
						for (i = 0; i < tileY; i++) {
								offsetArray.Add (new Vector2 (i / (float)tileX, (tileY - 1 - j) / (float)tileY));
						}
				}

				while (true) {
						foreach (Vector2 texOffset in offsetArray) {
								renderer.material.SetTextureOffset ("_MainTex", texOffset);
								yield  return new WaitForSeconds (secPerFrame);
						}
				}

		}

}

