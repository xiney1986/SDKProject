//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Tween the object's color.
/// </summary>

[AddComponentMenu("NGUI/Tween/Tween Label Number")]
public class TweenLabelNumber : UITweener
{
	public int from ;
	public int to ;
	
	UILabel label;
	
	/// <summary>
	/// Current color.
	/// </summary>
	
	public int number
	{
		get
		{
			if(label.text == null || label.text == "")
				return 0;
			else
				return int.Parse(label.text);
		}
		set
		{
			label.text = value.ToString();
		}
	}
	
	/// <summary>
	/// Find all needed components.
	/// </summary>
	
	void Awake ()
	{
		label = GetComponent<UILabel>();
	}
	
	/// <summary>
	/// Interpolate and update the color.
	/// </summary>
	
	protected override void OnUpdate(float factor, bool isFinished) { number = (int)Mathf.Lerp(from, to, factor); }
	
	/// <summary>
	/// Start the tweening operation.
	/// </summary>
	
	static public TweenLabelNumber Begin (GameObject go, float duration, int number)
	{
		#if UNITY_EDITOR
		if (!Application.isPlaying) return null;
		#endif
		TweenLabelNumber comp = UITweener.Begin<TweenLabelNumber>(go, duration);
		comp.from = comp.number;
		comp.to = number;
		
		if (duration <= 0f)
		{
			comp.Sample(1f, true);
			comp.enabled = false;
		}
		return comp;
	}
}
