using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpCounter : MonoBehaviour {

	// Vreme koje je potrebno powerupu da se napuni
	public float powerUpTime;

	// Preostalo vreme za punjenje poverupa
	private float powerUpRemainingTime;

	// Koristicemo za pauzu i kada se menjaju powerupovi da bismo izracunali preostalo vreme
	private float powerUpStartFillTime;

	// Ako je powerup spreman onda je ovo true
	public bool powerUpReady;

	// Tip powerupa: 0 - double slap, 1 - narukvica, 2 - minus jedan poen, 3 - stit
	public int powerUpType; 

	// Igrac
	public PlayerControls player;
	public int playerIndex;

	// Resetovanje powerupa
	public void ResetPowerUp()
	{
		powerUpRemainingTime = powerUpTime;
		powerUpStartFillTime = Time.time;
		GetComponent<Image>().fillAmount = 0;
		powerUpReady = false;
	}

	// Podesavanja parametara nakon pauziranja
	public void SetPowerUpAfterPause()
	{
		if (!powerUpReady)
		{
			powerUpStartFillTime = Time.time;
		}
	}

	// Podesavanja paramatara kada se stisne pauza
	public void SetPowerUpOnPaused()
	{
		if (!powerUpReady)
		{
			powerUpRemainingTime = powerUpRemainingTime - Time.time + powerUpStartFillTime;
		}
	}

	void Start()
	{
		powerUpReady = true;

		if (playerIndex == 1)
		{
			player = GameplayManager.gameplayManager.player1;
		}
		else if (playerIndex == 2 && !GlobalVariables.aiMatch)
		{
			player = GameplayManager.gameplayManager.player2;
		}
	}

	void Update()
	{
		// Ako nije powerup spreman i ako je aktivna uloga u kojoj se nalazi powerup punimo isti
		if (!powerUpReady && GameplayManager.gameplayManager.canPause)
		{
			// Napad
			if (player.isAttacking && (powerUpType == 0 || powerUpType == 1))
			{
//				GetComponent<Image>().fillAmount = Time.time / (powerUpStartFillTime + powerUpRemainingTime);
				GetComponent<Image>().fillAmount = 1f - (powerUpStartFillTime + powerUpRemainingTime - Time.time) / powerUpTime;

				if (Time.time > powerUpStartFillTime + powerUpRemainingTime)
				{
					GetComponent<Image>().fillAmount = 0;
					powerUpReady = true;

					// Proveravamo koji je tip i podesavamo da igrac moze opet da odigra taj powerup
					if (powerUpType == 0)
					{
						player.doubleSlapHolder.transform.Find("AnimationHolder/ButtonHolder").GetComponent<CustomButton>().interactable = true;
						player.doubleSlapHolder.transform.Find("AnimationHolder/ButtonHolder").GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);

//						if (!player.nextHitDoubleSlap)
//						{
						ParticleSystem ps = player.transform.parent.Find("DoublePointsParticle").GetComponent<ParticleSystem>();
						var main = ps.main;
						main.loop = true;
						player.transform.parent.Find("DoublePointsParticle").gameObject.SetActive(false);
//						player.transform.parent.Find("DoublePointsParticleHit").gameObject.SetActive(false);
//						}
					}
					else if (powerUpType == 1)
					{
						player.trapHolder.transform.Find("AnimationHolder/ButtonHolder").GetComponent<CustomButton>().interactable = true;
						player.trapHolder.transform.Find("AnimationHolder/ButtonHolder").GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
					}
				}
			}

			// Odbrana
			if (!player.isAttacking && (powerUpType == 2 || powerUpType == 3))
			{
//				GetComponent<Image>().fillAmount = Time.time / (powerUpStartFillTime + powerUpRemainingTime);
				GetComponent<Image>().fillAmount = 1f - (powerUpStartFillTime + powerUpRemainingTime - Time.time) / powerUpTime;

				if (Time.time > powerUpStartFillTime + powerUpRemainingTime)
				{
					GetComponent<Image>().fillAmount = 0;
					powerUpReady = true;

					// Proveravamo koji je tip i podesavamo da igrac moze opet da odigra taj powerup
					if (powerUpType == 2)
					{
						player.minusOnePointHolder.transform.Find("AnimationHolder/ButtonHolder").GetComponent<CustomButton>().interactable = true;
						player.minusOnePointHolder.transform.Find("AnimationHolder/ButtonHolder").GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
					}
					else if (powerUpType == 3)
					{
						player.shieldGameObject.SetActive(false);
						player.shieldHolder.transform.Find("AnimationHolder/ButtonHolder").GetComponent<CustomButton>().interactable = true;
						player.shieldHolder.transform.Find("AnimationHolder/ButtonHolder").GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
					}
				}
			}
		}
	}
}
