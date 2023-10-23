using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GlobalVariables : MonoBehaviour {

	public GameObject[] hands;

	public static int player1HandIndex;
	public static int player2HandIndex;

	public static int matchMakingPoints;

	public static string applicationID;

	public static bool aiMatch; // Ako je true onda je protiv AIa, ako false onda je za 2 igraca

	// Promenljiva koja nam cuva informaciju da li trebamo da pustimo loading depart nakon sto je scena loadovana
	public static bool playLoadingDepart;

	// Promenljiva za remove ads
	public static bool removeAdsOwned;

	public List<int> lockedHandsIndices;

	// Ovde cemo cuvati objekat na koji je kliknuto prilikom odabira ruke kako bismo znali  koji item da otkljucamo nakon odgledanog videa
	public static GameObject objectForUnlocking;
	public static int unlockingObjectIndex;
	public static int unlockingObjectScrollRectIndex;

	// Idjevi za reklame
	public static int bannerID = 2;
	public static int homeInterstitialID = 3;
	public static int videoRewardID = 4;
	public static int exitInterstitialID = 5;

	public static GlobalVariables globalVariables;

	void Awake()
	{
		if (!PlayerPrefs.HasKey("MMP"))
		{
			PlayerPrefs.SetInt("MMP", 0);
			matchMakingPoints = 0;
		}
		else
		{
			matchMakingPoints = PlayerPrefs.GetInt("MMP");
		}

		// Ako ne postoji prefs koji cuva koje ruke su vec otkljucane kreiramo novi
		if (!PlayerPrefs.HasKey("UnlockedHands"))
		{
//			string lockedHands = "";
//
//			if (lockedHandsIndices.Count > 0)
//			{
//				for (int i = 0; i < lockedHandsIndices.Count; i++)
//				{
//					if (i < lockedHandsIndices.Count - 1)
//						lockedHands += lockedHandsIndices[i].ToString() + "#";
//					else
//						lockedHands += lockedHandsIndices[i].ToString();
//				}

				PlayerPrefs.SetString("UnlockedHands", "");
				PlayerPrefs.Save();
//			}
		}
		else
		{
//			lockedHandsIndices.Clear();
			string[] indices = PlayerPrefs.GetString("UnlockedHands").Split('#');

			for (int i = 0; i < indices.Length; i++)
			{
				int indeeex = -1;
				bool isNumber = int.TryParse(indices[i], out indeeex);
				if (isNumber && lockedHandsIndices.Contains(indeeex))
					lockedHandsIndices.Remove(int.Parse(indices[i]));
			}
		}

		#if UNITY_ANDROID || UNITY_EDITOR_WIN
		applicationID = "com.Slapping.Hands.Challenge.Friends";
		#endif

		aiMatch = false;

		removeAdsOwned = false;

		globalVariables = this;

		DontDestroyOnLoad(this);
	}
}
