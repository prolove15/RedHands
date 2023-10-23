using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerControls : MonoBehaviour {

	public bool isAttacking;
	public bool canAttackOrDefend;

	public string playerName;
	public PlayerControls enemy;

	public int playerScore;
	public Text playerScoreText;

	// Ako je bio hit u ovom koraku onda postavljamo ovo na true
	public bool wasHit;

	public bool madeMove;

	public int chickenCounter;
	public Animator[] chickenAnimators;

	public Animator playerControlesAnimator;

	// Powerupovi
	public bool doubleSlap;
	public bool trap;
	public bool minusOnePoint;
	public bool shieldActive;

	public GameObject shieldGameObject;

	public bool nextHitDoubleSlap;

	public GameObject doubleSlapHolder;
	public GameObject trapHolder;
	public GameObject minusOnePointHolder;
	public GameObject shieldHolder;

	public GameObject attackPowerUps;
	public GameObject defencePowerUps;

	public GameObject chickenHolder;
	public GameObject timerHolder;
	public float timerStartTime;
	public float timerRemainigTime;
	public Image timerProgressBar;

	// Ako je igrac zarobljen ovo postaje true
	public bool trapped;

	public Image injuryImage;

	void Start()
	{
		playerControlesAnimator = transform.parent.GetComponent<Animator>();

		wasHit = false;
		madeMove = false;

		playerScore = 0;
		chickenCounter = 0;
		playerScoreText.text = playerScore.ToString();

		// TODO za sada omogucavamo sva tri powerupa
		doubleSlap = true;
		trap = true;
		minusOnePoint = true;
		nextHitDoubleSlap = false;
		trapped = false;

		shieldGameObject = transform.Find("ShieldHolder").gameObject;

		// Setujemo ime igraca FIXME za sad uzimam ime parent objekta
		playerName = transform.parent.parent.name;

		// Pronalazimo drugog igraca
		if (transform.parent.parent.name == "Player1")
		{
			enemy = GameObject.Find("Player2/AnimationHolder/Hand").GetComponent<PlayerControls>();

			// Dodajemo listener objektu koji hvata kontrole
			GameObject.Find("ClickableAreaPlayer1").GetComponent<CustomButton>().onDown.AddListener(MakeMove);
		}
		else
		{
			enemy = GameObject.Find("Player1/AnimationHolder/Hand").GetComponent<PlayerControls>();

			// Ako je drugi igrac onda listener za klik dodajemo samo ako je igra za dva igraca
			// U suprotnom dodajemo AI skriptu
			if (!GlobalVariables.aiMatch)
				GameObject.Find("ClickableAreaPlayer2").GetComponent<CustomButton>().onDown.AddListener(MakeMove);
			else
			{
				// Dodajemo AI skriptu i gasimo sve powerupove
				gameObject.AddComponent<PlayerAI>();
				DisablePowerUpsForAI();
			}
		}

		// Dodajemo listenere za powerupove
		doubleSlapHolder.transform.Find("AnimationHolder/ButtonHolder").GetComponent<CustomButton>().onDown.AddListener(DoubleSlap);
		trapHolder.transform.Find("AnimationHolder/ButtonHolder").GetComponent<CustomButton>().onDown.AddListener(Trap);
		minusOnePointHolder.transform.Find("AnimationHolder/ButtonHolder").GetComponent<CustomButton>().onDown.AddListener(MinusOnePoint);
		shieldHolder.transform.Find("AnimationHolder/ButtonHolder").GetComponent<CustomButton>().onDown.AddListener(Shield);

		// Pronalazimo progress bar za timer
		timerProgressBar = timerHolder.transform.Find("BarImage").GetComponent<Image>();

		// Dodajemo canvas komponentu
		gameObject.AddComponent<Canvas>();

		if (isAttacking)
		{
			GetComponent<Canvas>().overrideSorting = true;
			GetComponent<Canvas>().sortingOrder = 2;

			ResetTimer();
		}
		else
		{
			GetComponent<Canvas>().overrideSorting = true;
			GetComponent<Canvas>().sortingOrder = 0;
		}
	}

	public void MakeMove()
	{
		if (isAttacking && canAttackOrDefend && !madeMove)
		{
			playerControlesAnimator.Play("PlayerAttack", 0, 0);

			StartCoroutine("AttackCoroutine");
		}

		if (!isAttacking && canAttackOrDefend && !madeMove)
		{
			playerControlesAnimator.Play("PlayerRetreat", 0, 0);
			StartCoroutine("RetreatCoroutine");
		}

		if (!isAttacking && trapped)
		{
			// Pustamo animaciju za trapped
			playerControlesAnimator.Play("PlayerTrapped", 0, 0);
		}
	}

	// Ovu korutinu pozivamo nakon napada, pa ako protivnik nije pomerio ruku
	//  za neki vremenski interval onda mu onemogucavamo da je pomeri
	IEnumerator AttackCoroutine()
	{
		madeMove = true;

		// Prvo zabranjujemo da se klikce non stop na udarac
		canAttackOrDefend = false;

		// Ako je single player match onda pozivamo fju koja odlucuje da li ce igrac da pogodi ili promasi AI igraca
		if (GlobalVariables.aiMatch)
		{
			if (transform.parent.parent.name == "Player1")
			{
				enemy.GetComponent<PlayerAI>().Retreat();
			}
		}

		// Cekamo vremenski interval zatim onemogucavamo protivniku pomeranje
		yield return new WaitForSeconds(0.2f);

		enemy.canAttackOrDefend = false;

		if (!enemy.madeMove)
		{
			wasHit = true;
			StartCoroutine("PlayerHit");
		}
		else
		{
			if (nextHitDoubleSlap)
			{
				nextHitDoubleSlap = false;
//				ParticleSystem ps = transform.parent.Find("DoublePointsParticle").GetComponent<ParticleSystem>();
//				var main = ps.main;
//				main.loop = false;
				transform.parent.Find("DoublePointsParticle").GetComponent<ParticleSystem>().Stop(true);
			}
		}

		// Cekamo opet pa ako je bio miss menjamo igracima uloge
		yield return new WaitForSeconds(0.75f);

		if (isAttacking && !wasHit)
		{
			GameplayManager.gameplayManager.ChangeSides();
		}
	}

	IEnumerator PlayerHit()
	{
		// Dodajemo poene igracu
		if (!enemy.shieldActive)
		{
			if (nextHitDoubleSlap)
			{
				SoundManager.Instance.Play_Sound(SoundManager.Instance.slapDouble);
				nextHitDoubleSlap = false;
//				transform.parent.Find("DoublePointsParticle").gameObject.SetActive(false);
//				ParticleSystem ps = transform.parent.Find("DoublePointsParticle").GetComponent<ParticleSystem>();
//				var main = ps.main;
//				main.loop = false;
//				transform.parent.Find("DoublePointsParticleHit").gameObject.SetActive(true);

				transform.parent.Find("DoublePointsParticle").GetComponent<ParticleSystem>().Stop(true);
				playerScore += 2;
			}
			else
			{
				SoundManager.Instance.Play_Sound(SoundManager.Instance.slap);
				playerScore++;
			}

			playerScoreText.text = playerScore.ToString();

			// Pustamo animaciju da se zatresu ruke
			GameplayManager.gameplayManager.changeSidesAnimator.Play("HandsShakeAnimation", 0, 0);

			transform.parent.parent.Find("HitParticle").GetComponent<ParticleSystem>().Play(true);
		}
		else
		{
			SoundManager.Instance.Play_Sound(SoundManager.Instance.shieldPop);
			enemy.shieldActive = false;
			enemy.shieldGameObject.transform.GetChild(0).GetComponent<Animator>().Play("ShieldPop", 0, 0);

			if (nextHitDoubleSlap)
			{
				nextHitDoubleSlap = false;
				transform.parent.Find("DoublePointsParticle").GetComponent<ParticleSystem>().Stop(true);
			}
		}

		// Proveravamo score i na osnovu toga povecavamo transparenciju slike za crvenilo
		if (playerScore >= 8)
		{
			Color pom = enemy.injuryImage.color;
			pom.a = 1f;
			enemy.injuryImage.color = pom;
		}
		else if (playerScore >= 6)
		{
			Color pom = enemy.injuryImage.color;
			pom.a = 0.75f;
			enemy.injuryImage.color = pom;
		}
		else if (playerScore >= 4)
		{
			Color pom = enemy.injuryImage.color;
			pom.a = 0.5f;
			enemy.injuryImage.color = pom;
		}
		else if (playerScore >= 2)
		{
			if (enemy.injuryImage == null)
			{
				// Nalazimo image za crvenilo
				enemy.injuryImage = enemy.transform.Find("AnimationHolder/HandInjury").GetComponent<Image>();
			}

			Color pom = enemy.injuryImage.color;
			pom.a = 0.25f;
			enemy.injuryImage.color = pom;
		}
		else if (playerScore == 1 && enemy.injuryImage == null)
		{
			// Nalazimo image za crvenilo
			enemy.injuryImage = enemy.transform.Find("AnimationHolder/HandInjury").GetComponent<Image>();
		}
		
		// Sacekamo malo i onda pustamo animaciju za vracanje ruke
		yield return new WaitForSeconds(0.2f);
		playerControlesAnimator.Play("PlayerHit", 0, 0);

		// Cekamo vremenski interval zatim omogucavamo igracima da nastave igru
		yield return new WaitForSeconds(0.55f);

		if (playerScore >= 10)
		{
			enemy.canAttackOrDefend = false;
			canAttackOrDefend = false;

			// TODO ovo sad je zeznuto jer treba samo da doda poene ako se pobedi,
			// a posalje pruku playeru 2 ako izbugi da se kod njega pozove fja koja ce da oduzme poene
			// Vise kad se namesti pravi multiplayer
			SetMMPoints(20);
			enemy.SetMMPoints(-20);
			GameplayManager.gameplayManager.GameFinished(playerName);
		}
		else
		{
			// Resetujemo chicken counter
			if (enemy.chickenCounter > 0)
			{
				for (int i = 0; i < enemy.chickenCounter; i++)
				{
					enemy.chickenAnimators[i].Play("ChickenReset", 0, 0);
				}

				enemy.chickenCounter = 0;

				// A ako je bas jednak 3 znaci da trebamo da uklonimo okove
				if (enemy.chickenCounter == 3)
				{
					if (playerName == "Player1")
						GameplayManager.gameplayManager.chainsHolder.transform.GetChild(0).GetComponent<Animator>().Play("Player2Unchained", 0, 0);
					else
						GameplayManager.gameplayManager.chainsHolder.transform.GetChild(0).GetComponent<Animator>().Play("Player1Unchained", 0, 0);
				}
			}

			if (enemy.trapped)
			{
				if (playerName == "Player1")
				{
					GameplayManager.gameplayManager.chainsHolder.transform.GetChild(0).GetComponent<Animator>().Play("Player2Unchained", 0, 0);
				}
				else
				{
					GameplayManager.gameplayManager.chainsHolder.transform.GetChild(0).GetComponent<Animator>().Play("Player1Unchained", 0, 0);
				}

				enemy.trapped = false;
			}

			enemy.canAttackOrDefend = true;
			canAttackOrDefend = true;
			wasHit = false;
			enemy.madeMove = false;
			madeMove = false;

			// Ako je single player igra pozivamo opet korutinu za napad
			if (GlobalVariables.aiMatch)
			{
				if (GetComponent<PlayerAI>() != null)
					GetComponent<PlayerAI>().Attack();
			}

			// Posto je bio Hit resetujemo vreme
			ResetTimer();
		}
	}

	public void SetMMPoints(int amount)
	{
		GlobalVariables.matchMakingPoints += amount;
		PlayerPrefs.SetInt("MMP", amount);
		PlayerPrefs.Save();
	}

	IEnumerator RetreatCoroutine()
	{
		madeMove = true;
		canAttackOrDefend = false;

		SoundManager.Instance.Play_Sound(SoundManager.Instance.dodge);

		// Prvo sacekamo interval trajanja animacije
		yield return new WaitForSeconds(0.2f);

		enemy.canAttackOrDefend = false;

		if (!enemy.wasHit && enemy.madeMove)
		{
			yield return new WaitForSeconds(0.75f);

			// Pustamo animaciju vracanja u originalni polozaj
//			playerControlesAnimator.Play("PlayerRetreatToIdle", 0, 0);

			enemy.canAttackOrDefend = true;
			canAttackOrDefend = true;
			madeMove = false;
		}
		else if (!enemy.wasHit && !enemy.madeMove && chickenCounter < 3)
		{
			yield return new WaitForSeconds(0.2f);

			chickenCounter++;

			chickenAnimators[chickenCounter - 1].Play("IsChicken", 0, 0);

			// Pustamo animaciju vracanja u originalni polozaj
			playerControlesAnimator.Play("PlayerRetreatToIdle", 0, 0);

			enemy.canAttackOrDefend = true;

			if (chickenCounter < 3)
			{
				canAttackOrDefend = true;
				SoundManager.Instance.Play_Sound(SoundManager.Instance.chickeeen);
			}
			else
			{
				SoundManager.Instance.Play_Sound(SoundManager.Instance.chickeeen);

				if (playerName == "Player1")
				{
					GameplayManager.gameplayManager.chainsHolder.transform.GetChild(0).GetComponent<Animator>().Play("Player1Chained", 0, 0);
				}
				else
				{
					GameplayManager.gameplayManager.chainsHolder.transform.GetChild(0).GetComponent<Animator>().Play("Player2Chained", 0, 0);
				}

				trapped = true;
			}

			madeMove = false;

			if (GlobalVariables.aiMatch)
			{
				if (enemy.GetComponent<PlayerAI>() != null)
				{
					enemy.GetComponent<PlayerAI>().StopAllCoroutines();
					enemy.GetComponent<PlayerAI>().Attack();
				}
			}
		}
	}

	public void DoubleSlap()
	{
		if (isAttacking && GameplayManager.gameplayManager.canPause)
		{
			SoundManager.Instance.Play_Sound(SoundManager.Instance.doubleSlapActivate);
			doubleSlap = false;
			nextHitDoubleSlap = true;
			doubleSlapHolder.transform.Find("AnimationHolder/ButtonHolder").GetComponent<CustomButton>().interactable = false;
			doubleSlapHolder.transform.Find("AnimationHolder/ButtonHolder").GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.6f);
			transform.parent.Find("DoublePointsParticle").gameObject.SetActive(true);
			doubleSlapHolder.transform.Find("AnimationHolder/ButtonHolder/ButtonFillImage").GetComponent<PowerUpCounter>().ResetPowerUp();
		}
	}

	public void Trap()
	{
		if (!enemy.trapped && isAttacking && canAttackOrDefend && chickenCounter < 3 && GameplayManager.gameplayManager.canPause)
		{
			SoundManager.Instance.Play_Sound(SoundManager.Instance.trapActivate);
			enemy.canAttackOrDefend = false;
			enemy.trapped = true;
			trapHolder.transform.Find("AnimationHolder/ButtonHolder").GetComponent<CustomButton>().interactable = false;
			trapHolder.transform.Find("AnimationHolder/ButtonHolder").GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.6f);
			trapHolder.transform.Find("AnimationHolder/ButtonHolder/ButtonFillImage").GetComponent<PowerUpCounter>().ResetPowerUp();

			if (playerName == "Player1")
				GameplayManager.gameplayManager.chainsHolder.transform.GetChild(0).GetComponent<Animator>().Play("Player2Chained", 0, 0);
			else
				GameplayManager.gameplayManager.chainsHolder.transform.GetChild(0).GetComponent<Animator>().Play("Player1Chained", 0, 0);
		}
	}

	public void MinusOnePoint()
	{
		if (GameplayManager.gameplayManager.canPause)
		{
			SoundManager.Instance.Play_Sound(SoundManager.Instance.minusPoint);
			enemy.playerScore--;
			enemy.playerScoreText.text = enemy.playerScore.ToString();
			minusOnePointHolder.transform.Find("AnimationHolder/ButtonHolder").GetComponent<CustomButton>().interactable = false;
			minusOnePointHolder.transform.Find("AnimationHolder/ButtonHolder").GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.6f);
			minusOnePointHolder.transform.Find("AnimationHolder/ButtonHolder/ButtonFillImage").GetComponent<PowerUpCounter>().ResetPowerUp();

			// Pustamo animaciju za minus 1 poen
			enemy.playerScoreText.transform.parent.GetComponent<Animator>().Play("MinusPoint", 0, 0);
		}
	}

	public void Shield()
	{
		if (GameplayManager.gameplayManager.canPause)
		{
			SoundManager.Instance.Play_Sound(SoundManager.Instance.shieldActivate);
			shieldActive = true;
			shieldGameObject.SetActive(true);
			shieldHolder.transform.Find("AnimationHolder/ButtonHolder").GetComponent<CustomButton>().interactable = false;
			shieldHolder.transform.Find("AnimationHolder/ButtonHolder").GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.6f);
			shieldHolder.transform.Find("AnimationHolder/ButtonHolder/ButtonFillImage").GetComponent<PowerUpCounter>().ResetPowerUp();
		}
	}

	public void DisablePowerUpsForAI()
	{
		doubleSlapHolder.transform.Find("AnimationHolder/ButtonHolder").GetComponent<CustomButton>().interactable = false;
		doubleSlapHolder.transform.Find("AnimationHolder/ButtonHolder").GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.6f);
		trapHolder.transform.Find("AnimationHolder/ButtonHolder").GetComponent<CustomButton>().interactable = false;
		trapHolder.transform.Find("AnimationHolder/ButtonHolder").GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.6f);
		minusOnePointHolder.transform.Find("AnimationHolder/ButtonHolder").GetComponent<CustomButton>().interactable = false;
		minusOnePointHolder.transform.Find("AnimationHolder/ButtonHolder").GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.6f);
		shieldHolder.transform.Find("AnimationHolder/ButtonHolder").GetComponent<CustomButton>().interactable = false;
		shieldHolder.transform.Find("AnimationHolder/ButtonHolder").GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.6f);
	}

	// Podesavanja vremena za udarac nakon pauziranja
	public void SetTimerAfterPause()
	{
		if (isAttacking)
		{
			timerStartTime = Time.time;
		}
	}

	// Podesavanja vremena za udarac kada se stisne pauza
	public void SetTimerOnPaused()
	{
		if (isAttacking)
		{
			timerRemainigTime = timerRemainigTime - Time.time + timerStartTime;
		}
	}

	public void ResetTimer()
	{
		timerStartTime = Time.time;
		timerRemainigTime = GameplayManager.attackTimer;
		timerProgressBar.fillAmount = 1f;
	}

	void Update()
	{
		// Ako igrac napada i ako moze da udari tada mu odbrojavamo za udarac
		if (isAttacking && canAttackOrDefend)
		{
			timerProgressBar.fillAmount = (timerStartTime + timerRemainigTime - Time.time) / GameplayManager.attackTimer;

			// Ako je vreme za udarac isteklo menjamo strane
			if (timerProgressBar.fillAmount <= 0)
				GameplayManager.gameplayManager.ChangeSides();
		}
	}
}
