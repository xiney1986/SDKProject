using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DayTopButton:ButtonBase
{
	public UITexture icon;
	public UITexture selelct;
	public GameObject tip;
	public UILabel tipLabel;
	public UISprite timeLimit;
	[HideInInspector]
	public float
		local_x;

	[HideInInspector]
	public SevenDaysHappyDetail detail;

	private SevenDaysHappySample sample;

	public void setSevenDaysHappySample(SevenDaysHappySample _sample)
	{
		sample = _sample;
	}
	public SevenDaysHappySample getSevenDaysHappySample()
	{
		return sample;
	}

	// 更新小角标//
	public void updateTime ()
	{
		if(sample != null)
		{
			if(SevenDaysHappyManagement.Instance.dayIDAndCount[sample.dayId] > 0 && sample.dayId <= SevenDaysHappyManagement.Instance.getDayIndex())
			{
				tip.gameObject.SetActive(true);
				tipLabel.text = SevenDaysHappyManagement.Instance.dayIDAndCount[sample.dayId].ToString();
			}
			else
			{
				tip.gameObject.SetActive(false);
			}
		}
	}
}