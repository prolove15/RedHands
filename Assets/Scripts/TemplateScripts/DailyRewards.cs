using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

/**
  * Scene:MainScene
  * Object:DailyRewards
  * Description: Skripta koja je zaduzena za DailyRewards, svakog novog dana daje korisniku nagradu, ako dolazi za redom svaki dan nagrada se povecava, cim se prekine niz vraca ga na pravi dan
  **/
public class DailyRewards : MonoBehaviour {

	public static int [] DailyRewardAmount = new int[]{0,100, 200, 300, 400, 500, 1000};
	int OneDayTime=60*60*24;
	public static int LevelReward;
	bool rewardCompleted = false;
	List<int> availableSixthReward=new List<int>();
	int sixDayCount, typeOfSixReward; // typeOfSixReward 0-stars, 1-blades, 2-bomb, 3-laser, 4-tesla
	Text moneyText;
	System.Globalization.DateTimeFormatInfo format;
	private  DateTime quitTime;
	string lastPlayDate,timeQuitString;
	string enterDay, enterMonth, enterYear, quitDay, quitMonth, quitYear;

	// Use this for initialization
	void Awake () {
		if(PlayerPrefs.HasKey("SixDayCount"))
		{
			sixDayCount=PlayerPrefs.GetInt("SixDayCount");
			if(sixDayCount<4)
			{
				sixDayCount++;
			}
		}
		else
		{
			sixDayCount=1;
		}
		moneyText = GameObject.Find("DailyReward/AnimationHolder/Body/CoinsHolder/AnimationHolder/Text").GetComponent<Text>();
		moneyText.text = "0"; // ovde upisujete vrednost koju cuvate za coine
		DateTime currentTime = DateTime.Now;

		enterDay = currentTime.Day.ToString();
		enterMonth = currentTime.Month.ToString();
		enterYear = currentTime.Year.ToString();

		if(PlayerPrefs.HasKey("LevelReward"))
		{
			LevelReward=PlayerPrefs.GetInt("LevelReward");
		}
		else
		{
			LevelReward=0;
		}

//		if(PlayerPrefs.HasKey("VremeQuit"))
//		{
//			lastPlayDate=PlayerPrefs.GetString("VremeQuit");
//			quitTime=DateTime.Parse(lastPlayDate);
//
//			quitDay=quitTime.Day.ToString();
//			quitMonth = quitTime.Month.ToString();
//			quitYear = quitTime.Year.ToString();
//			if((int.Parse(enterYear)-int.Parse(quitYear))<1)
//			{
//
//				if((int.Parse(enterMonth)-int.Parse(quitMonth))==0)
//				{
//
//					if((int.Parse(enterDay)-int.Parse(quitDay)) > 1)
//					{
//						LevelReward=1;
//						GameObject.Find("DailyReward").GetComponent<Animator>().Play("DailyRewardArrival");
//						SetActiveDay(LevelReward);
//						GameObject.Find("DayOne").GetComponent<Animator>().Play("DailyRewardDay");
//						PlayerPrefs.SetInt("LevelReward",LevelReward);
//						PlayerPrefs.Save();
//						ShowDailyReward(LevelReward);
//					}
//					else if ((int.Parse(enterDay)-int.Parse(quitDay)) > 0)
//					{
//
//
//						if(LevelReward<6)
//						{
//							GameObject.Find("DailyReward").GetComponent<Animator>().Play("DailyRewardArrival");
//							LevelReward++;
//							SetActiveDay(LevelReward);
//							GameObject.Find("Day"+LevelReward).GetComponent<Animator>().Play("DailyRewardDay");
//							PlayerPrefs.SetInt("LevelReward",LevelReward);
//							PlayerPrefs.Save();
//							ShowDailyReward(LevelReward);
//						}
//						else
//						{
//						
//							LevelReward=1;
//							GameObject.Find("DailyReward").GetComponent<Animator>().Play("DailyRewardArrival");
//							SetActiveDay(LevelReward);
//							GameObject.Find("Day"+LevelReward).GetComponent<Animator>().Play("DailyRewardDay");
//							PlayerPrefs.SetInt("LevelReward",LevelReward);
//							PlayerPrefs.Save();
//							ShowDailyReward(LevelReward);
//						}
//
//						//Ponisti notifikaciju za DailyReward i posalji novu i prikazi DailyRewards sa nivoom LevelReward na 24h
//					}
//					else
//					{
//					}
//				}
//				else
//				{
//
//					if(int.Parse(enterDay)==1)
//					{
//						if(int.Parse(quitMonth)==1 || int.Parse(quitMonth)==3 || int.Parse(quitMonth)==5 || int.Parse(quitMonth)==7 || int.Parse(quitMonth)==8 || int.Parse(quitMonth)==10 || int.Parse(quitMonth)==12)
//						{
//							if(int.Parse(quitDay)==31)
//							{
//
//								if(LevelReward<6)
//								{
//									GameObject.Find("DailyReward").GetComponent<Animator>().Play("DailyRewardArrival");
//									LevelReward++;
//									SetActiveDay(LevelReward);
//									GameObject.Find("Day"+LevelReward).GetComponent<Animator>().Play("DailyRewardDay");
//									PlayerPrefs.SetInt("LevelReward",LevelReward);
//									PlayerPrefs.Save();
//									ShowDailyReward(LevelReward);
//								}
//								else
//								{
//									LevelReward=1;
//									GameObject.Find("DailyReward").GetComponent<Animator>().Play("DailyRewardArrival");
//									SetActiveDay(LevelReward);
//									GameObject.Find("Day"+LevelReward).GetComponent<Animator>().Play("DailyRewardDay");
//									PlayerPrefs.SetInt("LevelReward",LevelReward);
//									PlayerPrefs.Save();
//									ShowDailyReward(LevelReward);
//								}
//
//								//Ponisti notifikaciju za DailyReward i posalji novu i prikazi DailyRewards sa nivoom LevelReward na 24h
//							}
//							else
//							{
//								LevelReward=1;
//								GameObject.Find("DailyReward").GetComponent<Animator>().Play("DailyRewardArrival");
//								SetActiveDay(LevelReward);
//								GameObject.Find("Day"+LevelReward).GetComponent<Animator>().Play("DailyRewardDay");
//								PlayerPrefs.SetInt("LevelReward",LevelReward);
//								PlayerPrefs.Save();
//								ShowDailyReward(LevelReward);
//
//								//Ponisti notifikaciju za DailyReward i posalji novu i prikazi DailyRewards sa nivoom LevelReward na 24h
//							}
//						}
//						else if(int.Parse(quitMonth)==4 || int.Parse(quitMonth)==6 || int.Parse(quitMonth)==9 || int.Parse(quitMonth)==11)
//						{
//							if(int.Parse(quitDay)==30)
//							{
//
//								if(LevelReward<6)
//								{
//									GameObject.Find("DailyReward").GetComponent<Animator>().Play("DailyRewardArrival");
//									LevelReward++;
//									SetActiveDay(LevelReward);
//									GameObject.Find("Day"+LevelReward).GetComponent<Animator>().Play("DailyRewardDay");
//									PlayerPrefs.SetInt("LevelReward",LevelReward);
//									PlayerPrefs.Save();
//									ShowDailyReward(LevelReward);
//								}
//								else
//								{
//									LevelReward=1;
//									GameObject.Find("DailyReward").GetComponent<Animator>().Play("DailyRewardArrival");
//									GameObject.Find("Day"+LevelReward).GetComponent<Animator>().Play("DailyRewardDay");
//									PlayerPrefs.SetInt("LevelReward",LevelReward);
//									PlayerPrefs.Save();
//									ShowDailyReward(LevelReward);
//								}
//
//								//Ponisti notifikaciju za DailyReward i posalji novu i prikazi DailyRewards sa nivoom LevelReward na 24h
//							}
//							else
//							{
//								LevelReward=1;
//								GameObject.Find("DailyReward").GetComponent<Animator>().Play("DailyRewardArrival");
//								SetActiveDay(LevelReward);
//								GameObject.Find("Day"+LevelReward).GetComponent<Animator>().Play("DailyRewardDay");
//								PlayerPrefs.SetInt("LevelReward",LevelReward);
//								PlayerPrefs.Save();
//								ShowDailyReward(LevelReward);
//								//Ponisti notifikaciju za DailyReward i posalji novu i prikazi DailyRewards sa nivoom LevelReward na 24h
//							}
//						}
//						else
//						{
//							if(int.Parse(quitDay)>27)
//							{
//
//								if(LevelReward<6)
//								{
//									GameObject.Find("DailyReward").GetComponent<Animator>().Play("DailyRewardArrival");
//									LevelReward++;
//									SetActiveDay(LevelReward);
//									GameObject.Find("Day"+LevelReward).GetComponent<Animator>().Play("DailyRewardDay");
//									PlayerPrefs.SetInt("LevelReward",LevelReward);
//									PlayerPrefs.Save();
//									ShowDailyReward(LevelReward);
//								}
//								else
//								{
//									LevelReward=1;
//									GameObject.Find("DailyReward").GetComponent<Animator>().Play("DailyRewardArrival");
//									SetActiveDay(LevelReward);
//									GameObject.Find("Day"+LevelReward).GetComponent<Animator>().Play("DailyRewardDay");
//									PlayerPrefs.SetInt("LevelReward",LevelReward);
//									PlayerPrefs.Save();
//									ShowDailyReward(LevelReward);
//								}
//
//								//Ponisti notifikaciju za DailyReward i posalji novu i prikazi DailyRewards sa nivoom LevelReward na 24h
//							}
//							else
//							{
//								LevelReward=1;
//								GameObject.Find("DailyReward").GetComponent<Animator>().Play("DailyRewardArrival");
//								SetActiveDay(LevelReward);
//								GameObject.Find("Day"+LevelReward).GetComponent<Animator>().Play("DailyRewardDay");
//								PlayerPrefs.SetInt("LevelReward",LevelReward);
//								PlayerPrefs.Save();
//								ShowDailyReward(LevelReward);
//								//Ponisti notifikaciju za DailyReward i posalji novu i prikazi DailyRewards sa nivoom LevelReward na 24h
//							}
//						}
//					}
//					else
//					{
//						LevelReward=1;
//						GameObject.Find("DailyReward").GetComponent<Animator>().Play("DailyRewardArrival");
//						SetActiveDay(LevelReward);
//						GameObject.Find("Day"+LevelReward).GetComponent<Animator>().Play("DailyRewardDay");
//						PlayerPrefs.SetInt("LevelReward",LevelReward);
//						PlayerPrefs.Save();
//						ShowDailyReward(LevelReward);
//						//Ponisti notifikaciju za DailyReward i posalji novu i prikazi DailyRewards sa nivoom LevelReward na 24h
//					}
//
//				}
//
//
//			}
//			else
//			{
//				if(int.Parse(quitDay)==31 && int.Parse(enterDay)==1)
//				{
//
//					if(LevelReward<6)
//					{
//						GameObject.Find("DailyReward").GetComponent<Animator>().Play("DailyRewardArrival");
//						LevelReward++;
//						SetActiveDay(LevelReward);
//						GameObject.Find("Day"+LevelReward).GetComponent<Animator>().Play("DailyRewardDay");
//						PlayerPrefs.SetInt("LevelReward",LevelReward);
//						PlayerPrefs.Save();
//						ShowDailyReward(LevelReward);
//					}
//					else
//					{
//						LevelReward=1;
//						GameObject.Find("DailyReward").GetComponent<Animator>().Play("DailyRewardArrival");
//						SetActiveDay(LevelReward);
//						GameObject.Find("Day"+LevelReward).GetComponent<Animator>().Play("DailyRewardDay");
//						PlayerPrefs.SetInt("LevelReward",LevelReward);
//						PlayerPrefs.Save();
//						ShowDailyReward(LevelReward);
//					}
//					
//					//Ponisti notifikaciju za DailyReward i posalji novu i prikazi DailyRewards sa nivoom LevelReward na 24h
//				}
//				else
//				{
//
//					LevelReward=1;
//					GameObject.Find("DailyReward").GetComponent<Animator>().Play("DailyRewardArrival");
//					SetActiveDay(LevelReward);
//					GameObject.Find("Day"+LevelReward).GetComponent<Animator>().Play("DailyRewardDay");
//					PlayerPrefs.SetInt("LevelReward",LevelReward);
//					PlayerPrefs.Save();
//					ShowDailyReward(LevelReward);
//					//Ponisti notifikaciju za DailyReward i posalji novu i prikazi DailyRewards sa nivoom LevelReward na 24h
//				}
//			}
//
//
//		}
//		else
//		{
////			Collect();
//			LevelReward=0;
//			PlayerPrefs.SetInt("LevelReward",LevelReward);
//			PlayerPrefs.Save();
//
//			//Pokreni Notifikaciju za DailyReward na 24h
//		}


	}
		
	void OnApplicationPause(bool pauseStatus) { //vraca false kad je aktivna app
		if(pauseStatus)
		{
			//izasao iz aplikacuje
			timeQuitString = DateTime.Now.ToString();
			PlayerPrefs.SetString("VremeQuit", timeQuitString);
			PlayerPrefs.Save();
		}
		else
		{
			//usao u aplikacuju
		}
	}

	void ShowDailyReward(int currentDayReward)
	{
//		GameObject.Find("SpotLight").GetComponent<Animation>().Play("DailyReward"+TrenutniDan);
		GameObject currentDay;
		currentDay = GameObject.Find("Day " + currentDayReward.ToString());

//		currentDay.transform.GetChild(0).GetComponent<Animator>().Play("CollectDailyRewardTab");
//		currentDay.transform.GetChild(0).Find("DailyRewardParticlesIdle").particleSystem.Play();
//		switch(TrenutniDan)
//		{
//		case 1:
//			GameObject.Find("SpotLight").GetComponent<Animation>().Play("DailyReward1");
//			break;
//
//		case 2:
//			GameObject.Find("SpotLight").GetComponent<Animation>().Play("DailyReward2");
//			break;
//
//		case 3:
//			GameObject.Find("SpotLight").GetComponent<Animation>().Play("DailyReward3");
//			break;
//
//		case 4:
//			GameObject.Find("SpotLight").GetComponent<Animation>().Play("DailyReward4");
//			break;
//
//		case 5:
//			GameObject.Find("SpotLight").GetComponent<Animation>().Play("DailyReward5");
//			break;
//
//		case 6:
//			GameObject.Find("SpotLight").GetComponent<Animation>().Play("DailyReward6");
//			break;
//		}
	}

	/// <summary>
	/// Coroutine-a koja prikazuje animaciju dobijanja nagrade, tacnije dodaje coine i ispisuje ih u Tect componentu
	/// </summary>
	public IEnumerator moneyCounter(int kolicina)
	{


		int current = int.Parse(moneyText.text);
		int suma = current + kolicina;
		int korak = (suma - current)/10;
		while(current != suma)
		{
			current += korak;
			moneyText.text = current.ToString();
			yield return new WaitForSeconds(0.07f);
		}
		timeQuitString = DateTime.Now.ToString();
		PlayerPrefs.SetString("VremeQuit", timeQuitString);
		PlayerPrefs.Save();
		yield return new WaitForSeconds(0.2f);
		GameObject.Find("DailyReward").GetComponent<Animator>().Play("DailyRewardDeparting");
	}

	/// <summary>
	/// F-ja koja podesava sliku za aktivnu nagradu po danu
	/// </summary>
	/// <param name="dayNumber">Redni broj nagrade/dana/param>
	void SetActiveDay(int dayNumber)
	{
		GameObject.Find("Day"+dayNumber+"/Image").GetComponent<Image>().color = new Color(255,255,255,1);
	}

	void OnApplicationQuit() {
		timeQuitString = DateTime.Now.ToString();
		PlayerPrefs.SetString("VremeQuit", timeQuitString);
		PlayerPrefs.Save();

		//Pokreni Notifikaciju za DailyReward na 24h
	}

	/// <summary>
	/// F-ja koja se poziva za uzimanje nagrade za dana 1-5
	/// </summary>
	public void TakeReward()
	{
		if(!rewardCompleted)
		{
			if(LevelReward!=6)
			{
				StartCoroutine("moneyCounter",DailyRewardAmount[LevelReward]);
			}
			rewardCompleted=true;
		}

	}

	/// <summary>
	/// F-ja koja se poziva ukoliko zelite da imate napredniju logiku za nagradjivanje korisnika prilikom 6-og dana posete
	/// </summary>
	public void TakeSixthReward()
	{
		if(!rewardCompleted)
		{
			rewardCompleted=true;

			// ovde vam ide logika ukoliko hocete da vam se cetvrta nagrada menja u vidu, da svaki sledeci put bude veca. U nastavku sledi zakomentarisani kod koji smo mi koristili, vi napisite sta vama odgovara
//			if(Shop.currentStateBlades==0 && Shop.currentStateBomb==0 && Shop.currentStateLaser==0 && Shop.currentStateTesla==0)
//			{
//				availableSixthReward.Add(0);
//			}
//			else
//			{
//				if(Shop.currentStateBlades>0)
//					availableSixthReward.Add(1);
//				if(Shop.currentStateBomb>0)
//					availableSixthReward.Add(2);
//				if(Shop.currentStateLaser>0)
//					availableSixthReward.Add(3);
//				if(Shop.currentStateTesla>0)
//					availableSixthReward.Add(4);
//			}
//			
//			
//			
//			int randomReward = UnityEngine.Random.Range(0, availableSixthReward.Count);
//			switch(availableSixthReward[randomReward])
//			{
//			case 0:  //-stars
//				GameObject.Find("PowerUp").GetComponent<Image>().sprite = GameObject.Find("ReferenceStars").GetComponent<Image>().sprite;
//				break;
//			case 1:  //-blades
//				GameObject.Find("PowerUp").GetComponent<Image>().sprite = GameObject.Find("ReferenceBlades").GetComponent<Image>().sprite;
//				Shop.bladesNumber+=sixDayCount;
//				break;
//			case 2:  //-bomb
//				GameObject.Find("PowerUp").GetComponent<Image>().sprite = GameObject.Find("ReferenceBomb").GetComponent<Image>().sprite;
//				Shop.bombNumber+=sixDayCount;
//				break;
//			case 3:  //-laser
//				GameObject.Find("PowerUp").GetComponent<Image>().sprite = GameObject.Find("ReferenceLaser").GetComponent<Image>().sprite;
//				Shop.laserNumber+=sixDayCount;
//				break;
//			case 4:  //-tesla
//				GameObject.Find("PowerUp").GetComponent<Image>().sprite = GameObject.Find("ReferenceTesla").GetComponent<Image>().sprite;
//				Shop.teslaNumber+=sixDayCount;
//				break;
//			}
//			
//			if(Shop.currentStateBlades==0 && Shop.currentStateBomb==0 && Shop.currentStateLaser==0 && Shop.currentStateTesla==0)
//			{
//				GameObject.Find("SixDayCountText").GetComponent<Text>().text = "1000x";
//				StartCoroutine("moneyCounter",DailyRewardAmount[LevelReward]);
//				Shop.stars += DailyRewardAmount[LevelReward];
//			}
//			else
//			{
//				switch(sixDayCount)
//				{
//				case 1:
//					GameObject.Find("SixDayCountText").GetComponent<Text>().text = "1x";
//					break;
//				case 2:
//					GameObject.Find("SixDayCountText").GetComponent<Text>().text = "2x";
//					break;
//				case 3:
//					GameObject.Find("SixDayCountText").GetComponent<Text>().text = "3x";
//					break;
//				case 4:
//					GameObject.Find("SixDayCountText").GetComponent<Text>().text = "4x";
//					break;
//				}
//				Invoke("HideAfterSixtDay",2f);
//			}
//			//ovde cuvajte u playerprefs coine
//			GameObject.Find("PowerUpCollect/AnimationHolder").GetComponent<Animation>().Play();

		}

	}

	/// <summary>
	/// F-ja koja zavisno od dana(1-5) ili 6 poziva odgovarajucu f-ju za preuzimanje nagrade
	/// </summary>
	public void Collect()
	{
		if(LevelReward<6)
		{
			TakeReward();
		}
		else
		{
			TakeSixthReward();
		}
	}

	/// <summary>
	/// F-ja koja po uzimanju 6-e nagrade poziva animaciju odlaska celog objekta
	/// </summary>
	void HideAfterSixtDay()
	{
		GameObject.Find("DailyReward").GetComponent<Animator>().Play("DailyRewardDeparting");
	}

}
