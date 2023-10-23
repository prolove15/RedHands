using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

	/**
  * Scene:All
  * Object:Canvas
  * Description: Skripta zaduzena za hendlovanje(prikaz i sklanjanje svih Menu-ja,njihovo paljenje i gasenje, itd...)
  **/
public class MenuManager : MonoBehaviour 
{
	
	public Menu currentMenu;
	Menu currentPopUpMenu;
//	[HideInInspector]
//	public Animator openObject;
	public GameObject[] disabledObjects;
	GameObject ratePopUp, crossPromotionInterstitial;
	public GameObject soundButton;
	public bool popupOpened;

	public GameObject videoNotAvailablePopup;
	
	void Start () 
	{
		if(SceneManager.GetActiveScene().name == "MainScene")
		{
			crossPromotionInterstitial = GameObject.Find("PopUps/PopUpInterstitial");
			ratePopUp = GameObject.Find("PopUps/PopUpRate");

			if (SoundManager.soundOn == 1)
			{
				SoundManager.Instance.Play_MenuMusic();

				if (SoundManager.Instance.gameplayMusic.isPlaying)
					SoundManager.Instance.gameplayMusic.Stop();
			}
			else
			{
				if (soundButton != null)
					soundButton.transform.Find("SoundOffImage").gameObject.SetActive(true);
			}

			// Ako je loading depart ukljucen pustamo loading depart animaciju
			if (GlobalVariables.playLoadingDepart)
			{
				GameObject.Find("LoadingHolder/AnimationHolder").GetComponent<Animator>().Play("LoadingDepart", 0, 0);
				SoundManager.Instance.Play_Sound(SoundManager.Instance.loadingDepart);
				GlobalVariables.playLoadingDepart = false;
			}

		}
		else if (SceneManager.GetActiveScene().name == "HandSelectionScene")
		{
			if (GlobalVariables.aiMatch)
			{
				Debug.Log(disabledObjects.Length);
				if (disabledObjects.Length > 0) {
					for(int i=0;i<disabledObjects.Length;i++)
					{
						Debug.Log(disabledObjects[i].name);
						if (disabledObjects[i].name == "HandSelectionPanel1Player")
						{
							GameObject g = disabledObjects[i];
							disabledObjects[i] = currentMenu.gameObject;
							currentMenu = g.GetComponent<Menu>();
							break;
						}
					}
				}
			}
			else
			{
				if (disabledObjects.Length > 0) {
					for(int i=0;i<disabledObjects.Length;i++)
					{
						Debug.Log(disabledObjects[i].name);
						if (disabledObjects[i].name == "HandSelectionPanel2Players")
						{
							GameObject g = disabledObjects[i];
							disabledObjects[i] = currentMenu.gameObject;
							currentMenu = g.GetComponent<Menu>();
							break;
						}
					}
				}
			}
		}
		else if (SceneManager.GetActiveScene().name == "Level")
		{
			if (SoundManager.soundOn == 1)
			{
				SoundManager.Instance.Play_GameplayMusic();

				if (SoundManager.Instance.menuMusic.isPlaying)
					SoundManager.Instance.menuMusic.Stop();
			}
			else
			{
				if (soundButton != null)
					soundButton.transform.Find("SoundOffImage").gameObject.SetActive(true);
			}
		}

		if (disabledObjects!=null) {
			for(int i=0;i<disabledObjects.Length;i++)
			{
				disabledObjects[i].SetActive(false);
			}

			ShowMenu(currentMenu.gameObject);
		}
		
		if(Application.loadedLevelName!= "MapScene")
			ShowMenu(currentMenu.gameObject);	
		
		if(Application.loadedLevelName=="MainScene")
		{
			
			
			if(PlayerPrefs.HasKey("alreadyRated"))
			{
				Rate.alreadyRated = PlayerPrefs.GetInt("alreadyRated");
			}
			else
			{
				Rate.alreadyRated = 0;
			}
			
			if(Rate.alreadyRated==0)
			{
				Rate.appStartedNumber = PlayerPrefs.GetInt("appStartedNumber");
				Debug.Log("appStartedNumber "+Rate.appStartedNumber);
				
				if(Rate.appStartedNumber>=6)
				{
					Rate.appStartedNumber=0;
					PlayerPrefs.SetInt("appStartedNumber",Rate.appStartedNumber);
					PlayerPrefs.Save();
					ShowPopUpMenu(ratePopUp);
					
				}
			}
		}
	}
	
	/// <summary>
	/// Funkcija koja pali(aktivira) objekat
	/// </summary>
	/// /// <param name="gameObject">Game object koji se prosledjuje i koji treba da se upali</param>
	public void EnableObject(GameObject gameObject)
	{
		
		if (gameObject != null) 
		{
			if (!gameObject.activeSelf) 
			{
				gameObject.SetActive (true);
			}
		}
	}

	/// <summary>
	/// Funkcija koja gasi objekat
	/// </summary>
	/// /// <param name="gameObject">Game object koji se prosledjuje i koji treba da se ugasi</param>
	public void DisableObject(GameObject gameObject)
	{
		Debug.Log("Disable Object");
		if (gameObject != null) 
		{
			if (gameObject.activeSelf) 
			{
				gameObject.SetActive (false);
			}
		}
	}
	
	/// <summary>
	/// F-ja koji poziva ucitavanje Scene
	/// </summary>
	/// <param name="levelName">Level name.</param>
	public void LoadScene(string levelName )
	{
		if (levelName != "") {
			try {
				Application.LoadLevel (levelName);
			} catch (System.Exception e) {
				Debug.Log ("Can't load scene: " + e.Message);
			}
		} else {
			Debug.Log ("Can't load scene: Level name to set");
		}
	}
	
	/// <summary>
	/// F-ja koji poziva asihrono ucitavanje Scene
	/// </summary>
	/// <param name="levelName">Level name.</param>
	public void LoadSceneAsync(string levelName )
	{
		if (levelName != "") {
			try {
				Application.LoadLevelAsync (levelName);
			} catch (System.Exception e) {
				Debug.Log ("Can't load scene: " + e.Message);
			}
		} else {
			Debug.Log ("Can't load scene: Level name to set");
		}
	}

	/// <summary>
	/// Funkcija za prikaz Menu-ja koji je pozvan kao Menu
	/// </summary>
	/// /// <param name="menu">Game object koji se prosledjuje i treba da se skloni, mora imati na sebi skriptu Menu.</param>
	public void ShowMenu(GameObject menu)
	{
		if (currentMenu != null)
		{
			currentMenu.IsOpen = false;
			currentMenu.gameObject.SetActive(false);
		}
		
		currentMenu = menu.GetComponent<Menu> ();
		menu.gameObject.SetActive (true);
		currentMenu.IsOpen = true;
	}

	/// <summary>
	/// Funkcija za zatvaranje Menu-ja koji je pozvan kao Meni
	/// </summary>
	/// /// <param name="menu">Game object koji se prosledjuje za prikaz, mora imati na sebi skriptu Menu.</param>
	public void CloseMenu(GameObject menu)
	{
		if (menu != null) 
		{
			menu.GetComponent<Menu> ().IsOpen = false;
			menu.SetActive (false);
		}
	}

	/// <summary>
	/// Funkcija za prikaz Menu-ja koji je pozvan kao PopUp-a
	/// </summary>
	/// /// <param name="menu">Game object koji se prosledjuje za prikaz, mora imati na sebi skriptu Menu.</param>
	public void ShowPopUpMenu(GameObject menu)
	{
		SoundManager.Instance.Play_Sound(SoundManager.Instance.showPopup);
		menu.gameObject.SetActive (true);
		currentPopUpMenu = menu.GetComponent<Menu> ();
		currentPopUpMenu.IsOpen = true;
		popupOpened = true;
	}

	/// <summary>
	/// Funkcija za zatvaranje Menu-ja koji je pozvan kao PopUp-a, poziva inace coroutine-u, ima delay zbog animacije odlaska Menu-ja
	/// </summary>
	/// /// <param name="menu">Game object koji se prosledjuje i treba da se skloni, mora imati na sebi skriptu Menu.</param>
	public void ClosePopUpMenu(GameObject menu)
	{
		StartCoroutine("HidePopUp",menu);
	}

	/// <summary>
	/// Couorutine-a za zatvaranje Menu-ja koji je pozvan kao PopUp-a
	/// </summary>
	/// /// <param name="menu">Game object koji se prosledjuje, mora imati na sebi skriptu Menu.</param>
	IEnumerator HidePopUp(GameObject menu)
	{
		SoundManager.Instance.Play_Sound(SoundManager.Instance.hidePopup);
		menu.GetComponent<Menu> ().IsOpen = false;
		yield return new WaitForSeconds(1.2f);

		popupOpened = false;
		menu.SetActive (false);
	}

	/// <summary>
	/// Funkcija za prikaz poruke preko Log-a, prilikom klika na dugme
	/// </summary>
	/// /// <param name="message">poruka koju treba prikazati.</param>
	public void ShowMessage(string message)
	{
		Debug.Log(message);
	}

	/// <summary>
	/// Funkcija koja podesava naslov dialoga kao i poruku u dialogu i ova f-ja se poziva iz skripte
	/// </summary>
	/// <param name="messageTitleText">naslov koji treba prikazati.</param>
	/// <param name="messageText">custom poruka koju treba prikazati.</param>
	public void ShowPopUpMessage(string messageTitleText, string messageText)
	{
		transform.Find("PopUps/PopUpMessage/AnimationHolder/Body/HeaderHolder/TextHeader").GetComponent<Text>().text=messageTitleText;
		transform.Find("PopUps/PopUpMessage/AnimationHolder/Body/ContentHolder/TextBG/TextMessage").GetComponent<Text>().text=messageText;
		ShowPopUpMenu(transform.Find("PopUps/PopUpMessage").gameObject);

	}

	/// <summary>
	/// Funkcija koja podesava naslov CustomMessage-a, i ova f-ja se poziva preko button-a zajedno za f-jom ShowPopUpMessageCustomMessageText u redosledu: 1-ShowPopUpMessageTitleText 2-ShowPopUpMessageCustomMessageText
	/// </summary>
	/// <param name="messageTitleText">naslov koji treba prikazati.</param>
	public void ShowPopUpMessageTitleText(string messageTitleText)
	{
		transform.Find("PopUps/PopUpMessage/AnimationHolder/Body/HeaderHolder/TextHeader").GetComponent<Text>().text=messageTitleText;
	}

	/// <summary>
	/// Funkcija koja podesava poruku CustomMessage, i poziva meni u vidu pop-upa, ova f-ja se poziva preko button-a zajedno za f-jom ShowPopUpMessageTitleText u redosledu: 1-ShowPopUpMessageTitleText 2-ShowPopUpMessageCustomMessageText
	/// </summary>
	/// <param name="messageText">custom poruka koju treba prikazati.</param>
	public void ShowPopUpMessageCustomMessageText(string messageText)
	{
		transform.Find("PopUps/PopUpMessage/AnimationHolder/Body/ContentHolder/TextBG/TextMessage").GetComponent<Text>().text=messageText;		
		ShowPopUpMenu(transform.Find("PopUps/PopUpMessage").gameObject);
	}

	/// <summary>
	/// Funkcija koja podesava naslov dialoga kao i poruku u dialogu i ova f-ja se poziva iz skripte
	/// </summary>
	/// <param name="dialogTitleText">naslov koji treba prikazati.</param>
	/// <param name="dialogMessageText">custom poruka koju treba prikazati.</param>
	public void ShowPopUpDialog(string dialogTitleText, string dialogMessageText)
	{
		transform.Find("PopUps/PopUpMessage/AnimationHolder/Body/HeaderHolder/TextHeader").GetComponent<Text>().text=dialogTitleText;
		transform.Find("PopUps/PopUpMessage/AnimationHolder/Body/ContentHolder/TextBG/TextMessage").GetComponent<Text>().text=dialogMessageText;
		ShowPopUpMenu(transform.Find("PopUps/PopUpMessage").gameObject);
	}

	/// <summary>
	/// Funkcija koja podesava naslov dialoga, ova f-ja se poziva preko button-a zajedno za f-jom ShowPopUpDialogCustomMessageText u redosledu: 1-ShowPopUpDialogTitleText 2-ShowPopUpDialogCustomMessageText
	/// </summary>
	/// <param name="dialogTitleText">naslov koji treba prikazati.</param>
	public void ShowPopUpDialogTitleText(string dialogTitleText)
	{
		transform.Find("PopUps/PopUpMessage/AnimationHolder/Body/HeaderHolder/TextHeader").GetComponent<Text>().text=dialogTitleText;
	}

	/// <summary>
	/// Funkcija koja podesava poruku dialoga i poziva meni u vidu pop-upa, ova f-ja se poziva preko button-a zajedno za f-jom ShowPopUpDialogTitleText u redosledu: 1-ShowPopUpDialogTitleText 2-ShowPopUpDialogCustomMessageText
	/// </summary>
	/// <param name="dialogMessageText">custom poruka koju treba prikazati.</param>
	public void ShowPopUpDialogCustomMessageText(string dialogMessageText)
	{
		transform.Find("PopUps/PopUpMessage/AnimationHolder/Body/ContentHolder/TextBG/TextMessage").GetComponent<Text>().text=dialogMessageText;		
		ShowPopUpMenu(transform.Find("PopUps/PopUpMessage").gameObject);
	}

	public void StartLoading()
	{
		transform.Find("LoadingMenu").gameObject.SetActive(true);
		transform.Find("LoadingMenu/AnimationHolder").GetComponent<Animator>().Play("Arriving");
	}

	// Main Scene
	public void TwoPlayersPlayButtonClicked()
	{
		GlobalVariables.aiMatch = false;

		// FIXME za sada samo loadujem scenu za biranje ruku
		SceneManager.LoadScene("HandSelectionScene");
	}

	public void SinglePlayerPlayButtonClicked()
	{
		GlobalVariables.aiMatch = true;

		// FIXME za sada samo loadujem scenu za biranje ruku
		SceneManager.LoadScene("HandSelectionScene");
	}

	// Hand Selection scene
	public void BackToMainMenu()
	{
		// Pustamo home interstitial reklamu
		AdsManager.Instance.ShowInterstitial();

		// FIXME za sada samo loadujem main menu
		SceneManager.LoadScene("MainScene");
	}

	public void PlayButtonClickSound()
	{
		SoundManager.Instance.Play_ButtonClick();
	}

	public void ToggleSound()
	{
		if (SoundManager.soundOn == 1)
		{
			SoundManager.Instance.MuteAllSounds();
			soundButton.transform.Find("SoundOffImage").gameObject.SetActive(true);
			SoundManager.soundOn = 0;
			PlayerPrefs.SetInt("SoundOn", 0);
			PlayerPrefs.Save();
		}
		else
		{
			SoundManager.Instance.UnmuteAllSounds();
			soundButton.transform.Find("SoundOffImage").gameObject.SetActive(false);
			SoundManager.soundOn = 1;

			if (!SoundManager.Instance.menuMusic.isPlaying && SceneManager.GetActiveScene().name == "MainScene")
				SoundManager.Instance.Play_MenuMusic();
			else if (!SoundManager.Instance.gameplayMusic.isPlaying && SceneManager.GetActiveScene().name == "Level")
				SoundManager.Instance.Play_GameplayMusic();

			PlayerPrefs.SetInt("SoundOn", 1);
			PlayerPrefs.Save();
		}
	}

	public void ShopVideoNotAvailablePopup()
	{
		ShowPopUpMenu(videoNotAvailablePopup);
	}

    public void OpenPrivacyPolicyLink()
    {
        Application.OpenURL(AdsManager.Instance.privacyPolicyLink);
    }

}
