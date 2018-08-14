using System;
using System.Collections.Generic;

/*
 * 剧情管理器
 * @author longlingquan
 * */
public class PlotManagerment
{
	private DialogueSample[] samples;
	private int index = -1;
	private int plotSid = 0;

	public PlotManagerment ()
	{ 

	}
	 
	public static PlotManagerment Instance 
	{
		get{return SingleManager.Instance.getObj("PlotManagerment") as PlotManagerment;}
	}
	
	public void over ()
	{
		samples = null;
		plotSid = 0;
		index = -1;
	}
	
	public void start (int sid)
	{
		this.plotSid = sid;
		initAllDialogues ();
	}
	
	//获得下一个对白
	public DialogueSample getNextDialogues ()
	{
		index++;
		if (index >= samples.Length) {
			over ();
			return null;
		}
		return samples [index];
	}
	
	//获得全部对白信息
	private void initAllDialogues ()
	{
		int[] sids = PlotConfigManager.Instance.getPlotSampleBySid (plotSid).sids;
		samples = new DialogueSample[sids.Length];
		for (int i=0; i<sids.Length; i++) {
			samples [i] = DialogueSampleManager.Instance.getDialogueSampleBySid (sids [i]);
		} 
	}
	
	public DialogueSample isSecond()
	{
		if(samples.Length>1)
		{
			return null;
		}
		return samples [index];
	}
	
	public int getSamplesLength()
	{
		return samples.Length;
	}
} 

