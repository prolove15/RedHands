using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAI : MonoBehaviour {

	public int difficulty;

	public int escapes;

	void Awake()
	{
		difficulty = Random.Range(0, 3);
		escapes = 0;
	}

	public void Attack()
	{
		StopCoroutine("AttackCoroutine");
		StartCoroutine("AttackCoroutine");
	}

	IEnumerator AttackCoroutine()
	{
		float waitTime = 0;
		switch (difficulty)
		{
		case 0:
			if (GetComponent<PlayerControls>().timerProgressBar.fillAmount * GameplayManager.attackTimer > 2f)
				waitTime = Random.Range(1f, 2f);
			else
				waitTime = Random.Range(0, GetComponent<PlayerControls>().timerProgressBar.fillAmount * GameplayManager.attackTimer);

			yield return new WaitForSeconds(waitTime);
			GetComponent<PlayerControls>().MakeMove();
			break;
		case 1:
			if (GetComponent<PlayerControls>().timerProgressBar.fillAmount * GameplayManager.attackTimer > 3f)
				waitTime = Random.Range(1f, 3f);
			else
				waitTime = Random.Range(0, GetComponent<PlayerControls>().timerProgressBar.fillAmount * GameplayManager.attackTimer);
			
			yield return new WaitForSeconds(waitTime);
			GetComponent<PlayerControls>().MakeMove();
			break;
		case 2:
			if (GetComponent<PlayerControls>().timerProgressBar.fillAmount * GameplayManager.attackTimer > 3f)
				waitTime = Random.Range(1f, 4f);
			else
				waitTime = Random.Range(0, GetComponent<PlayerControls>().timerProgressBar.fillAmount * GameplayManager.attackTimer);

			yield return new WaitForSeconds(waitTime);
			GetComponent<PlayerControls>().MakeMove();
			break;
		default:
			break;
		}
	}

	public void Retreat()
	{
		switch (difficulty)
		{
		case 0:
			if (escapes < 2)
			{
				int r = Random.Range(0, 3);

				if (r != 2 && r != 1)
				{
					escapes++;
					GetComponent<PlayerControls>().MakeMove();
				}
			}
			else
			{
				escapes = 0;
			}
			break;
		case 1:
			if (escapes < 3)
			{
				int r = Random.Range(0, 3);

				if (r != 2 && r != 1)
				{
					escapes++;
					GetComponent<PlayerControls>().MakeMove();
				}
			}
			else
			{
				escapes = 0;
			}
			break;
		case 2:
			if (escapes < 4)
			{
				int r = Random.Range(0, 8);

				if (r != 2 && r != 1)
				{
					escapes++;
					GetComponent<PlayerControls>().MakeMove();
				}
			}
			else
			{
				escapes = 0;
			}
			break;
		default:
			break;
		}
	}
}
