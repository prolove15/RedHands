using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour {

	public PlayerControls player1;
	public PlayerControls player2;
	RectTransform player1Holder;
	RectTransform player2Holder;

	public float transitionSpeed;
	public float lerpSpeed;

	public Animator changeSidesAnimator;

	public MenuManager menuManager;
	public GameObject pausePopup;
	public GameObject pausePopup2Players;
	public bool canPause;
	public GameObject gameFinishedPopup;
	public GameObject gameFinishedPopupOnePlayer;

	public GameObject attackRoleHolder;
	public GameObject dodgeRoleHolder;

	// Button fill objekti za powerupove
	public PowerUpCounter[] powerUpControlers;

	// Vreme za koje igrac mora da odradi udarac
	public static float attackTimer;

	// Holder za okove
	public GameObject chainsHolder;

	public static GameplayManager gameplayManager;

	void Awake()
	{
		// Ako je loading depart ukljucen pustamo loading depart animaciju
		if (GlobalVariables.playLoadingDepart)
		{
			GameObject.Find("LoadingHolder/AnimationHolder").GetComponent<Animator>().Play("LoadingDepart", 0, 0);
			SoundManager.Instance.Play_Sound(SoundManager.Instance.loadingDepart);
			GlobalVariables.playLoadingDepart = false;
		}

		attackTimer = 5f;

		// Prvo pronalazimo holdere za ruke
		player1Holder = GameObject.Find("Player1").GetComponent<RectTransform>();
		player2Holder = GameObject.Find("Player2").GetComponent<RectTransform>();

		// Pronalazimo sve powerup controler objekte
		powerUpControlers = FindObjectsOfType(typeof(PowerUpCounter)) as PowerUpCounter[];

		// Zatim kreiramo odabrane igrace
		GameObject p1 = Instantiate(GlobalVariables.globalVariables.hands[GlobalVariables.player1HandIndex], player1Holder.transform.Find("AnimationHolder"));
		p1.transform.localScale = Vector3.one;
		p1.transform.localPosition = Vector3.zero;
		p1.name = "Hand";
		p1.AddComponent<PlayerControls>();
		p1.GetComponent<PlayerControls>().playerScoreText = GameObject.Find("Player1Score").GetComponent<Text>();
		p1.GetComponent<PlayerControls>().chickenAnimators = new Animator[3];
		p1.GetComponent<PlayerControls>().chickenAnimators[0] = GameObject.Find("TimeAndChickenHolderPlayer1/ChickenPlayerHolder/Chicken1/AnimationHolder").GetComponent<Animator>();
		p1.GetComponent<PlayerControls>().chickenAnimators[1] = GameObject.Find("TimeAndChickenHolderPlayer1/ChickenPlayerHolder/Chicken2/AnimationHolder").GetComponent<Animator>();
		p1.GetComponent<PlayerControls>().chickenAnimators[2] = GameObject.Find("TimeAndChickenHolderPlayer1/ChickenPlayerHolder/Chicken3/AnimationHolder").GetComponent<Animator>();
		p1.GetComponent<PlayerControls>().doubleSlapHolder = GameObject.Find("PowerUpsPlayer1Holder/Attack/DoubleSlap");
		p1.GetComponent<PlayerControls>().trapHolder = GameObject.Find("PowerUpsPlayer1Holder/Attack/Trap");
		p1.GetComponent<PlayerControls>().minusOnePointHolder = GameObject.Find("PowerUpsPlayer1Holder/Defence/MinusOnePoint");
		p1.GetComponent<PlayerControls>().shieldHolder = GameObject.Find("PowerUpsPlayer1Holder/Defence/Shield");
		p1.GetComponent<PlayerControls>().defencePowerUps = p1.GetComponent<PlayerControls>().shieldHolder.transform.parent.gameObject;
		p1.GetComponent<PlayerControls>().attackPowerUps = p1.GetComponent<PlayerControls>().doubleSlapHolder.transform.parent.gameObject;
		p1.GetComponent<PlayerControls>().timerHolder = GameObject.Find("TimeAndChickenHolderPlayer1/TimeHolder");
		p1.GetComponent<PlayerControls>().chickenHolder = GameObject.Find("TimeAndChickenHolderPlayer1/ChickenPlayerHolder");

		GameObject p2 = Instantiate(GlobalVariables.globalVariables.hands[GlobalVariables.player2HandIndex], player2Holder.transform.Find("AnimationHolder"));
		p2.transform.localScale = Vector3.one;
		p2.transform.localPosition = Vector3.zero;

		Quaternion rot = p2.transform.localRotation;
		rot.eulerAngles = Vector3.zero;
		p2.transform.localRotation = rot;

		p2.name = "Hand";
		p2.AddComponent<PlayerControls>();
		p2.GetComponent<PlayerControls>().playerScoreText = GameObject.Find("Player2Score").GetComponent<Text>();
		p2.GetComponent<PlayerControls>().chickenAnimators = new Animator[3];
		p2.GetComponent<PlayerControls>().chickenAnimators[2] = GameObject.Find("TimeAndChickenHolderPlayer2/ChickenPlayerHolder/Chicken1/AnimationHolder").GetComponent<Animator>();
		p2.GetComponent<PlayerControls>().chickenAnimators[1] = GameObject.Find("TimeAndChickenHolderPlayer2/ChickenPlayerHolder/Chicken2/AnimationHolder").GetComponent<Animator>();
		p2.GetComponent<PlayerControls>().chickenAnimators[0] = GameObject.Find("TimeAndChickenHolderPlayer2/ChickenPlayerHolder/Chicken3/AnimationHolder").GetComponent<Animator>();
		p2.GetComponent<PlayerControls>().doubleSlapHolder = GameObject.Find("PowerUpsPlayer2Holder/Attack/DoubleSlap");
		p2.GetComponent<PlayerControls>().trapHolder = GameObject.Find("PowerUpsPlayer2Holder/Attack/Trap");
		p2.GetComponent<PlayerControls>().minusOnePointHolder = GameObject.Find("PowerUpsPlayer2Holder/Defence/MinusOnePoint");
		p2.GetComponent<PlayerControls>().shieldHolder = GameObject.Find("PowerUpsPlayer2Holder/Defence/Shield");
		p2.GetComponent<PlayerControls>().defencePowerUps = p2.GetComponent<PlayerControls>().shieldHolder.transform.parent.gameObject;
		p2.GetComponent<PlayerControls>().attackPowerUps = p2.GetComponent<PlayerControls>().doubleSlapHolder.transform.parent.gameObject;
		p2.GetComponent<PlayerControls>().timerHolder = GameObject.Find("TimeAndChickenHolderPlayer2/TimeHolder");
		p2.GetComponent<PlayerControls>().chickenHolder = GameObject.Find("TimeAndChickenHolderPlayer2/ChickenPlayerHolder");

		// TODO Ovde u odnosu na nesto setujemo ko prvi put napada a ko ne, za sada prvi player uvek napada prvi
		p1.GetComponent<PlayerControls>().canAttackOrDefend = true;
		p2.GetComponent<PlayerControls>().canAttackOrDefend = true;
		p1.GetComponent<PlayerControls>().isAttacking = true;
		p2.GetComponent<PlayerControls>().isAttacking = false;


		player1 = GameObject.Find("Player1/AnimationHolder/Hand").GetComponent<PlayerControls>();
		player2 = GameObject.Find("Player2/AnimationHolder/Hand").GetComponent<PlayerControls>();

		changeSidesAnimator = GameObject.Find("HandsHolder").GetComponent<Animator>();

		canPause = true;

		// Setujemo interfejs za uloge
		if (player2.GetComponent<PlayerControls>().isAttacking)
		{
			p1.GetComponent<PlayerControls>().attackPowerUps.SetActive(false);
			p2.GetComponent<PlayerControls>().defencePowerUps.SetActive(false);
			p1.GetComponent<PlayerControls>().timerHolder.SetActive(false);
			p2.GetComponent<PlayerControls>().chickenHolder.SetActive(false);

			attackRoleHolder.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 660f);
			attackRoleHolder.transform.Rotate(new Vector3(0, 0, 180f));
			dodgeRoleHolder.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -660f);
			dodgeRoleHolder.transform.Rotate(new Vector3(0, 0, 180f));

			p2.GetComponent<PlayerControls>().SetTimerAfterPause();
		}
		else
		{

			p1.GetComponent<PlayerControls>().defencePowerUps.SetActive(false);
			p2.GetComponent<PlayerControls>().attackPowerUps.SetActive(false);
			p1.GetComponent<PlayerControls>().chickenHolder.SetActive(false);
			p2.GetComponent<PlayerControls>().timerHolder.SetActive(false);

			p1.GetComponent<PlayerControls>().SetTimerAfterPause();
		}

		// Ako je single player partija setujemo da je level finished screen u stvari drugi popup
		if (GlobalVariables.aiMatch)
			gameFinishedPopup = gameFinishedPopupOnePlayer;

		gameplayManager = this;
	}

	public void ChangeSides()
	{
		StartCoroutine("ResetFieldCoroutine");
	}

	IEnumerator ResetFieldCoroutine()
	{
		// Zabranjujemo stiskanje pauze dok se menjaju strane
		canPause = false;

		player1.canAttackOrDefend = false;
		player2.canAttackOrDefend = false;

		// Ako je ai match stopiramo korutine i za ai skriptu
		if (GlobalVariables.aiMatch)
			player2.GetComponent<PlayerAI>().StopAllCoroutines();

		player1.StopAllCoroutines();
		player2.StopAllCoroutines();

		SoundManager.Instance.Play_Sound(SoundManager.Instance.changeSides);

		if (player1.chickenCounter == 3 || player1.trapped)
		{
			GameplayManager.gameplayManager.chainsHolder.transform.GetChild(0).GetComponent<Animator>().Play("Player1Unchained", 0, 0);
		}
		else if (player2.chickenCounter == 3 || player2.trapped)
		{
			GameplayManager.gameplayManager.chainsHolder.transform.GetChild(0).GetComponent<Animator>().Play("Player2Unchained", 0, 0);
		}

		changeSidesAnimator.Play("ChangeSides", 0, 0);

		if (player1.isAttacking)
		{
			// Swapujemo powerupove
			player1.attackPowerUps.transform.parent.GetComponent<Animator>().Play("AttackToDefence", 0, 0);
			player2.attackPowerUps.transform.parent.GetComponent<Animator>().Play("DefenceToAttack", 0, 0);
			player1.chickenHolder.transform.parent.GetComponent<Animator>().Play("AttackToDefence", 0, 0);
			player2.chickenHolder.transform.parent.GetComponent<Animator>().Play("DefenceToAttack", 0, 0);

			// Menjamo role da pokazuju da player 2 napada
			attackRoleHolder.transform.parent.GetComponent<Animator>().Play("Player2Attack", 0, 0);

			// Odpauziramo odgovarajuce powerupove u odnosu na to koji igrac napada
			powerUpControlers[0].SetPowerUpOnPaused();
			powerUpControlers[6].SetPowerUpOnPaused();
			powerUpControlers[1].SetPowerUpOnPaused();
			powerUpControlers[3].SetPowerUpOnPaused();

			if (player1.nextHitDoubleSlap)
			{
				player1.nextHitDoubleSlap = false;
				player1.transform.parent.Find("DoublePointsParticle").GetComponent<ParticleSystem>().Stop(true);
			}

			if (player2.shieldActive)
			{
				player2.shieldActive = false;
				player2.shieldGameObject.transform.GetChild(0).GetComponent<Animator>().Play("ShieldPop", 0, 0);
			}
		}
		else
		{
			// Swapujemo powerupove
			player2.attackPowerUps.transform.parent.GetComponent<Animator>().Play("AttackToDefence", 0, 0);
			player1.attackPowerUps.transform.parent.GetComponent<Animator>().Play("DefenceToAttack", 0, 0);
			player2.chickenHolder.transform.parent.GetComponent<Animator>().Play("AttackToDefence", 0, 0);
			player1.chickenHolder.transform.parent.GetComponent<Animator>().Play("DefenceToAttack", 0, 0);

			powerUpControlers[2].SetPowerUpOnPaused();
			powerUpControlers[4].SetPowerUpOnPaused();
			powerUpControlers[5].SetPowerUpOnPaused();
			powerUpControlers[7].SetPowerUpOnPaused();

			// Menjamo role da pokazuju da player 1 napada
			attackRoleHolder.transform.parent.GetComponent<Animator>().Play("Player1Attack", 0, 0);

			if (player2.nextHitDoubleSlap)
			{
				player2.nextHitDoubleSlap = false;
				player2.transform.parent.Find("DoublePointsParticle").GetComponent<ParticleSystem>().Stop(true);
			}

			if (player1.shieldActive)
			{
				player1.shieldActive = false;
				player1.shieldGameObject.transform.GetChild(0).GetComponent<Animator>().Play("ShieldPop", 0, 0);
			}
		}

		yield return new WaitForSeconds(0.6f);

		player1.playerControlesAnimator.Play("PlayerIdle");
		player2.playerControlesAnimator.Play("PlayerIdle");

		if (player1.isAttacking)
		{
			player1.timerProgressBar.fillAmount = 1f;
		}
		else
		{
			player2.timerProgressBar.fillAmount = 1f;
		}

		player1.shieldGameObject.SetActive(false);
		player2.shieldGameObject.SetActive(false);

		yield return new WaitForSeconds(0.5f);

		if (player1.isAttacking)
		{
			player1.isAttacking = false;
			player2.isAttacking = true;

			if (player2.chickenCounter > 0)
			{
				for (int i = 0; i < player2.chickenCounter; i++)
				{
					player2.chickenAnimators[i].Play("ChickenReset", 0, 0);
				}
			}

			player2Holder.SetAsLastSibling();

			// Ako je single player i ako napada drugi igrac pozivamo korutinu za napad
			if (GlobalVariables.aiMatch)
			{
				player2.GetComponent<PlayerAI>().Attack();
			}

			// Odpauziramo odgovarajuce powerupove u odnosu na to koji igrac napada
			powerUpControlers[2].SetPowerUpAfterPause();
			powerUpControlers[4].SetPowerUpAfterPause();
			powerUpControlers[5].SetPowerUpAfterPause();
			powerUpControlers[7].SetPowerUpAfterPause();

			// Setujemo canvas komponenti order in layer
			player1.GetComponent<Canvas>().sortingOrder = 0;
			player2.GetComponent<Canvas>().sortingOrder = 2;

			player2.ResetTimer();
		}
		else
		{
			player1.isAttacking = true;
			player2.isAttacking = false;

			if (player1.chickenCounter > 0)
			{
				for (int i = 0; i < player1.chickenCounter; i++)
				{
					player1.chickenAnimators[i].Play("ChickenReset", 0, 0);
				}
			}

			player1Holder.SetAsLastSibling();

			// Odpauziramo odgovarajuce powerupove u odnosu na to koji igrac napada
			powerUpControlers[0].SetPowerUpAfterPause();
			powerUpControlers[6].SetPowerUpAfterPause();
			powerUpControlers[1].SetPowerUpAfterPause();
			powerUpControlers[3].SetPowerUpAfterPause();

			// Setujemo canvas komponenti order in layer
			player2.GetComponent<Canvas>().sortingOrder = 0;
			player1.GetComponent<Canvas>().sortingOrder = 2;

			player1.ResetTimer();
		}

		player1.wasHit = false;
		player2.wasHit = false;
		player1.canAttackOrDefend = true;
		player2.canAttackOrDefend = true;
		player1.madeMove = false;
		player2.madeMove= false;
		player1.trapped = false;
		player2.trapped = false;
		player1.chickenCounter = 0;
		player2.chickenCounter = 0;

		// Omogucavamo stiskanje pauze dok se menjaju strane
		canPause = true;
	}

	public void PauseGame()
	{
		if (canPause && !player1.madeMove && !player2.madeMove)
		{
			player1.canAttackOrDefend = false;
			player2.canAttackOrDefend = false;

			if (GlobalVariables.aiMatch)
			{
				menuManager.ShowPopUpMenu(pausePopup);
			}
			else
			{
				menuManager.ShowPopUpMenu(pausePopup2Players);
//				pauseNativeAd2Players.LoadAd();
			}
			
			canPause = false;

			// Pauziramo sve powerup controlere
			for (int i = 0; i < powerUpControlers.Length; i++)
			{
				powerUpControlers[i].SetPowerUpOnPaused();
			}

			// Ako je single player i ako napada drugi igrac stopiramo korutinu za napad
			if (GlobalVariables.aiMatch && player2.isAttacking && !player2.madeMove)
			{
				player2.GetComponent<PlayerAI>().StopCoroutine("AttackCoroutine");
			}

			if (player1.isAttacking)
				player1.SetTimerOnPaused();
			else
				player2.SetTimerOnPaused();
		}
	}

	public void ContinueGame()
	{
		StartCoroutine("ContinueGameCoroutine");

		// Ako je single player i ako napada drugi igrac pustamo korutinu za napad
		if (GlobalVariables.aiMatch && player2.isAttacking && !player2.madeMove)
		{
			player2.GetComponent<PlayerAI>().Attack();
		}
	}

	IEnumerator ContinueGameCoroutine()
	{
		if (GlobalVariables.aiMatch)
		{
			menuManager.ClosePopUpMenu(pausePopup);
		}
		else
		{
			menuManager.ClosePopUpMenu(pausePopup2Players);
			// Izbaceni nativi jer je zarotiran menu za 90 stepeni
//			pauseNativeAd2Players.GetComponent<FacebookNativeAd>().CancelLoading();
//			pauseNativeAd2Players.GetComponent<FacebookNativeAd>().HideNativeAd();
		}

		yield return new WaitForSeconds(0.7f);
		player1.canAttackOrDefend = true;
		player2.canAttackOrDefend = true;
		canPause = true;

		// Odpauziramo sve powerup controlere
		for (int i = 0; i < powerUpControlers.Length; i++)
		{
			powerUpControlers[i].SetPowerUpAfterPause();
		}

		if (player1.isAttacking)
			player1.SetTimerAfterPause();
		else
			player2.SetTimerAfterPause();
		
	}

	public void ReturnToMainMenu()
	{
		// Pustamo home interstitial reklamu
		AdsManager.Instance.ShowInterstitial();

//		SceneManager.LoadScene("MainScene");
		StartCoroutine(LoadSceneAsynchronously("MainScene"));
	}

	public void RestartGame()
	{
//		SceneManager.LoadScene("Level");
		StartCoroutine(LoadSceneAsynchronously("Level"));
	}

	IEnumerator LoadSceneAsynchronously(string sceneName)
	{	
//		GameObject canvas = GameObject.Find("Canvas");
//		canvas.transform.Find("ClickBlocker").gameObject.SetActive(true);

		AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
		async.allowSceneActivation = false;

		GlobalVariables.playLoadingDepart = true;
		GameObject.Find("LoadingHolder/AnimationHolder").GetComponent<Animator>().Play("LoadingArrive", 0, 0);
		SoundManager.Instance.Play_Sound(SoundManager.Instance.loadingArrive);

		yield return new WaitForSeconds(1.5f);

		async.allowSceneActivation = true;
		yield return async;
	}

	public void GameFinished(string playerName)
	{
		if (!GlobalVariables.aiMatch)
			SoundManager.Instance.Play_Sound(SoundManager.Instance.winnerSound);
		
		menuManager.ShowPopUpMenu(gameFinishedPopup);
//		gameFinishedPopup.transform.Find("AnimationHolder/Body/HeaderHolder/TextHeader").GetComponent<Text>().text = playerName + " WON!";
		if (playerName == "Player1")
		{
			gameFinishedPopup.transform.Find("AnimationHolder").GetComponent<Animator>().Play("Player1Win", 0, 0);

			if (GlobalVariables.aiMatch)
				SoundManager.Instance.Play_Sound(SoundManager.Instance.winnerSound);
		}
		else
		{
			gameFinishedPopup.transform.Find("AnimationHolder").GetComponent<Animator>().Play("Player2Win", 0, 0);

			if (GlobalVariables.aiMatch)
				SoundManager.Instance.Play_Sound(SoundManager.Instance.loserSound);
		}
	}
}
