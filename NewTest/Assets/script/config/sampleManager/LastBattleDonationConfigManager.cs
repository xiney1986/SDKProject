using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LastBattleDonationConfigManager : SampleConfigManager
{
//	public List<LastBattleDonationSample> donationLow;// 低库//
//	public List<LastBattleDonationSample> donationMiddle;// 中库//
//	public List<LastBattleDonationSample> donationHigh;// 高库//
//	public List<LastBattleDonationSample> donationDimond;// 钻石库//

	public List<LastBattleDonationSample> totalDonation;

	public LastBattleDonationConfigManager ()
	{
		base.readConfig (ConfigGlobal.CONFIG_LASTBATTLEDONATION);
	}

	public static LastBattleDonationConfigManager Instance
	{		
		get{return SingleManager.Instance.getObj("LastBattleDonationConfigManager") as LastBattleDonationConfigManager;}
	}

	public override void parseConfig (string str)
	{
		LastBattleDonationSample sample = new LastBattleDonationSample();
		string[] strArr = str.Split ('|');
		sample.id = StringKit.toInt(strArr [0]);
		sample.donation = parseDonation(strArr[1]);
		sample.scores = StringKit.toInt(strArr [2]);
		sample.process = StringKit.toInt(strArr [3]);
		sample.junGong = StringKit.toInt(strArr [4]);
		sample.nvShenBlessLV = StringKit.toInt(strArr [5]);
		sample.donationType = StringKit.toInt(strArr [6]);

		//addListByType(sample);
		if(totalDonation == null)
			totalDonation = new List<LastBattleDonationSample>();
		totalDonation.Add(sample);
	}

	PrizeSample parseDonation(string str)
	{
		PrizeSample donation = new PrizeSample();;
		string[] strArr = str.Split (',');
		donation.type = StringKit.toInt(strArr[0]);
		donation.pSid = StringKit.toInt(strArr[1]);
		donation.num = strArr[2];
		return donation;
	}

//	public void addListByType(LastBattleDonationSample sample)
//	{
//		if(sample.libraryType == LastBattleDonation.DONATION_LOW)
//		{
//			if(donationLow == null)
//				donationLow = new List<LastBattleDonationSample>();
//			donationLow.Add(sample);
//		}
//		else if(sample.libraryType == LastBattleDonation.DONATION_MID)
//		{
//			if(donationMiddle == null)
//				donationMiddle = new List<LastBattleDonationSample>();
//			donationMiddle.Add(sample);
//		}
//		else if(sample.libraryType == LastBattleDonation.DONATION_HI)
//		{
//			if(donationHigh == null)
//				donationHigh = new List<LastBattleDonationSample>();
//			donationHigh.Add(sample);
//		}
//		else if(sample.libraryType == LastBattleDonation.DONATION_DIAMOND)
//		{
//			if(donationDimond == null)
//				donationDimond = new List<LastBattleDonationSample>();
//			donationDimond.Add(sample);
//		}
//	}
}

public class LastBattleDonation
{
	public const int DONATION_LOW = 1;// 底裤//
	public const int DONATION_MID = 2;// 中裤//
	public const int DONATION_HI = 3;// 高裤//
	public const int DONATION_DIAMOND = 4;// 钻石裤//
}
