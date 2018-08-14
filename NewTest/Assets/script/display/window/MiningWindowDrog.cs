using UnityEngine;
using System.Collections;

public class MiningWindowDrog : MonoBehaviour {

	public MiningWindow faWin;
	public float offset;
	private Vector2 pressPos = Vector2.zero;
	private Vector2 upPos;

	void OnPress (bool pressed) {
		if (!pressed) {
			if (pressPos.x + offset < upPos.x) {
				faWin.RightDrag ();
			}
			if (pressPos.x - offset > upPos.x) {
				faWin.LeftDrag ();
			}
			pressPos = Vector2.zero;
			upPos = Vector2.zero;
		}
	}
	
	/// <summary>
	/// Drag the object along the plane.
	/// </summary>
	
	void OnDrag (Vector2 delta) {
		if (pressPos == Vector2.zero) {
			pressPos = delta;
		}
			
		upPos = delta;

	}
	
	void OnScroll (float delta) {

	}
}
