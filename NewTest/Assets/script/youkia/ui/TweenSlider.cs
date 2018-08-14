//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Tween the object's color.
/// </summary>

[AddComponentMenu("NGUI/Tween/Tween Slider")]
public class TweenSlider : UITweener
{
	public float from ;
	public float to ;
	
	UISlider slider;
	
	/// <summary>
	/// Current color.
	/// </summary>
	
	public float number
	{
		get
		{
			return slider.value;
		}
		set
		{
			slider.value = value;
		}
	}
	
	/// <summary>
	/// Find all needed components.
	/// </summary>
	
	void Awake ()
	{
		slider = GetComponent<UISlider>();
	}
	
	/// <summary>
	/// Interpolate and update the color.
	/// </summary>
	
	protected override void OnUpdate(float factor, bool isFinished) { number = Mathf.Lerp(from, to, factor); }
	
	/// <summary>
	/// Start the tweening operation.
	/// </summary>
	
	static public TweenSlider Begin (GameObject go, float duration, float number)
	{
		#if UNITY_EDITOR
		if (!Application.isPlaying) return null;
		#endif
		TweenSlider comp = UITweener.Begin<TweenSlider>(go, duration);
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
