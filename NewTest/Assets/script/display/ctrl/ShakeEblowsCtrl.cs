using UnityEngine;
using System.Collections;

/// <summary>
/// 投掷骰子控制
/// </summary>
public class ShakeEblowsCtrl : MonoBase
{
	public Animation[] anims;
	public void Init ()
	{
		foreach (Animation a in anims) {
			a.wrapMode = WrapMode.Once;
			a.playAutomatically = false;
			a.gameObject.SetActive(false);
		}
	}

//	public IEnumerator playAnim (int num)
	public void playAnim (int num)
	{
		int i = 1;
		foreach (Animation a in anims) {
			if (i <= num) {
				a.gameObject.SetActive (true);
				if (!a.isPlaying)
					a.Play ("Take 001");
			} else {
				a.gameObject.SetActive (false);
			}
//			yield return new WaitForSeconds(0.05f);
		}
	}

	public void stopAnim ()
	{
		int i = 0;
		foreach (Animation a in anims) {
			if (a.isPlaying)
				a.Stop ();
		}
	}
}
