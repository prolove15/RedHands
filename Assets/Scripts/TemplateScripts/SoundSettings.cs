using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/**
  * Scene:MainScene, GamePlayScene
  * Object:N/A
  * Description: Skripta koja zavisno od stanja PlayerPrefs-a podesava image-a u PopUpSettings meniju i isto tako sadrzi f-je koje registuju klikove i promenu image-a za zvuk i sound
  **/
public class SoundSettings : MonoBehaviour {

	// Use this for initialization
	void Start () {
			InitialiseSoundSettings();
	}

	/// <summary>
	/// F-ja koja zavisno od stanja PlayerPrefs-a podesava image za Sound i Music
	/// </summary>
	public void InitialiseSoundSettings()
	{
		if(PlayerPrefs.HasKey("SoundOn"))
		{
			SoundManager.musicOn = PlayerPrefs.GetInt("MusicOn");
			SoundManager.soundOn = PlayerPrefs.GetInt("SoundOn");
		}

		if(SoundManager.soundOn == 0)
			GameObject.Find("SoundOnOff").GetComponent<Image>().enabled = true;
		if(SoundManager.musicOn == 0)
			GameObject.Find("MusicOnOff").GetComponent<Image>().enabled = true;
	}

	/// <summary>
	/// F-ja koja se poziva clicom na button, i zavisno od stanja gasi/pali image u obliku kose crte koji ide preko image-a za sound
	/// </summary>
	public void SoundOnOff()
	{
		if(SoundManager.soundOn == 1)
		{
			SoundManager.soundOn = 0;
			GameObject.Find("SoundOnOff").GetComponent<Image>().enabled = true;
		}
		else
		{
			SoundManager.soundOn = 1;
			SoundManager.Instance.Play_ButtonClick();
			GameObject.Find("SoundOnOff").GetComponent<Image>().enabled = false;
		}
		PlayerPrefs.SetInt("SoundOn",SoundManager.soundOn);
		PlayerPrefs.SetInt("MusicOn",SoundManager.musicOn);
		PlayerPrefs.Save();
	}

	/// <summary>
	/// F-ja koja se poziva clicom na button, i zavisno od stanja gasi/pali image u obliku kose crte koji ide preko image-a za music
	/// </summary>
	public void MusicOnOff()
	{
		if(SoundManager.musicOn == 1)
		{
			SoundManager.Instance.Stop_MenuMusic();
			SoundManager.musicOn = 0;
			GameObject.Find("MusicOnOff").GetComponent<Image>().enabled = true;
		}
		else
		{
			SoundManager.musicOn = 1;
			SoundManager.Instance.Play_MenuMusic();
			GameObject.Find("MusicOnOff").GetComponent<Image>().enabled = false;
		}
		PlayerPrefs.SetInt("SoundOn",SoundManager.soundOn);
		PlayerPrefs.SetInt("MusicOn",SoundManager.musicOn);
		PlayerPrefs.Save();
	}
	
}
