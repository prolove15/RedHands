using UnityEngine;
using System.Collections;

/**
  * Scene:All
  * Object:SoundManager
  * Description: Skripta zaduzena za zvuke u apliakciji, njihovo pustanje, gasenje itd...
  **/
public class SoundManager : MonoBehaviour {

	public static int musicOn = 1;
	public static int soundOn = 1;
	public static bool forceTurnOff = false;

	public AudioSource buttonClick;
	public AudioSource menuMusic;
	public AudioSource gameplayMusic;

	// Zvuci
	public AudioSource showPopup;
	public AudioSource hidePopup;
	public AudioSource handSelected;
	public AudioSource loadingArrive;
	public AudioSource loadingDepart;
	public AudioSource changeSides;
	public AudioSource dodge;
	public AudioSource slap;
	public AudioSource slapDouble;
	public AudioSource doubleSlapActivate;
	public AudioSource minusPoint;
	public AudioSource shieldActivate;
	public AudioSource shieldPop;
	public AudioSource trapActivate;
	public AudioSource chickeeen;
	public AudioSource winnerSound;
	public AudioSource loserSound;

	static SoundManager instance;

	public static SoundManager Instance
	{
		get
		{
			if(instance == null)
			{
				instance = GameObject.FindObjectOfType(typeof(SoundManager)) as SoundManager;
			}

			return instance;
		}
	}

	void Start () 
	{
		if(PlayerPrefs.HasKey("SoundOn"))
		{
			musicOn = PlayerPrefs.GetInt("MusicOn");
			soundOn = PlayerPrefs.GetInt("SoundOn");

			if (soundOn == 0 || musicOn == 0)
				MuteAllSounds();
		}
		else
		{
			soundOn = 1;
			musicOn = 1;
			PlayerPrefs.SetInt("SoundOn", 1);
			PlayerPrefs.SetInt("MusicOn", 1);
			PlayerPrefs.Save();
		}

		Screen.sleepTimeout = SleepTimeout.NeverSleep; 

		DontDestroyOnLoad(this.gameObject);
	}

	public void Play_ButtonClick()
	{
		if(buttonClick.clip != null && soundOn == 1)
			buttonClick.Play();
	}

	public void Play_MenuMusic()
	{
		if(menuMusic.clip != null && musicOn == 1)
			menuMusic.Play();
	}

	public void Stop_MenuMusic()
	{
		if(menuMusic.clip != null && musicOn == 1)
			menuMusic.Stop();
	}

	public void Play_GameplayMusic()
	{
		if(gameplayMusic.clip != null && musicOn == 1)
		{
			gameplayMusic.Play();
		}
	}

	public void Stop_GameplayMusic()
	{
		if(gameplayMusic.clip != null && musicOn == 1)
		{
			StartCoroutine(FadeOut(gameplayMusic, 0.005f));
		}
	}

	public void Play_Sound(AudioSource sound)
	{
		if (sound.clip != null && soundOn == 1)
		{
			sound.Play();
		}
	}

	/// <summary>
	/// Corutine-a koja za odredjeni AudioSource, kroz prosledjeno vreme, utisava AudioSource do 0, gasi taj AudioSource, a zatim vraca pocetni Volume na pocetan kako bi AudioSource mogao opet da se koristi
	/// </summary>
	/// <param name="sound">AudioSource koji treba smanjiti/param>
	/// <param name="time">Vreme za koje treba smanjiti Volume/param>
	IEnumerator FadeOut(AudioSource sound, float time)
	{
		float originalVolume = sound.volume;
		while(sound.volume != 0)
		{
			sound.volume = Mathf.MoveTowards(sound.volume, 0, time);
			yield return null;
		}
		sound.Stop();
		sound.volume = originalVolume;
	}

	/// <summary>
	/// F-ja koja Mute-uje sve zvuke koja su deca SoundManager-a
	/// </summary>
	public void MuteAllSounds()
	{
		foreach (Transform t in transform)
		{
			t.GetComponent<AudioSource>().mute = true;
		}
	}

	/// <summary>
	/// F-ja koja Unmute-uje sve zvuke koja su deca SoundManager-a
	/// </summary>
	public void UnmuteAllSounds()
	{
		foreach (Transform t in transform)
		{
			t.GetComponent<AudioSource>().mute = false;
		}
	}
	
}
