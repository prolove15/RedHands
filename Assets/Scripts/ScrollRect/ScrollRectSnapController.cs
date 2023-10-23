using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

/// <summary>
/// Scene: 
/// Object: 
/// Description: Funkcija koja se dodeljuje scroll rectovima ukoliko zelimo da imamo mogucnost snapovanja na posebne elemente.
/// </summary>

public class ScrollRectSnapController : MonoBehaviour {

	// Snapping positions list
	public List<float> snapPositions;

	// List element size
	public float cellSizeX;
	public float cellSizeY;
	public float spacing;

	// Used for checking characters snap point
	private float currentElemCheckTemp;

	public Vector3 newLerpPosition;

	// Lerping variables
	private bool lerping;
	public float lerpingSpeed;

	// true if we are interacting with scroll rect
	private bool holdingRect;

	public float focusedElementScale;
	public float unfocusedElementsScale;

	public List<GameObject> listOfElements;

	public bool horizontalList;

	public GameObject backwardButton;
	public GameObject forwardButton;

	private bool buttonPressed;

	public int currentElement;

	public bool shadeUnfocusedElements;
	public float shadePercentage; // 0 - 1, 0 means black (full shaded)
	public Image[] shadeImages; // The must be added in array like characters

	public bool playerReady;

	// Animator koji sadrzi animacije kada je igrac spreman
	public Animator playerReadyAnimator;

	public GameObject crossPromotionPopup;

//	public GameObject nativeAd;

	// Index reklame u meniju
	public List<int> nativeAdIndices;
//	public List<bool> nativeAdsLoaded;

//	public bool nativeAdLoadedOnce;

//	public bool levelsLoaded;

	// Holder za tackice i niz
//	public GameObject dotsHolder;

	// Prefab za tackicu
//	public GameObject dotPrefab;

	// Promenljiva za proveru da li su tackice kreirane da ih ne bismo kreirali takodje i nakon refresha levela
//	private bool dotsCreated;

	// Promenljiva koja nam je flag da znamo kad smo setovali u lerpu tackice i ram za svetove
//	private bool dotsAndWorldFrameSet;

	// Promenljiva za velicinu skrolabilnog dela ispred i iza prvog elementa
	public int frontAndBackAdditionSize;

	// Privremena promenljiva za proveru da li je korisnik kliknuo na neki level
	public bool levelClicked;

	// Postavljamo kada postavimo poziciju native ada u scroll rectu
	public bool nativeAdPositionSet;

//	public GameObject soundButton;

	public int player;

	public GameObject element;

	public MenuManager menuManager;

	void Awake()
	{
//		if (GlobalVariables.isAmazon && horizontalList)
//			DestroyImmediate(transform.Find("NativeAdHolder").gameObject);
		
//		dotsCreated = false;
//		dotsAndWorldFrameSet = true;
		levelClicked = false;
		nativeAdPositionSet = false;

		nativeAdPositionSet = true;

		if (GlobalVariables.aiMatch)
		{
			nativeAdIndices = new List<int>(){1, 6};
//			nativeAdsLoaded = new List<bool>(){false, false};
		}
		else
			nativeAdIndices = new List<int>();

		playerReady = false;

		// Kreiramo sve elemente niza
		for (int i = 0; i < GlobalVariables.globalVariables.hands.Length; i++)
		{
			GameObject newElement = Instantiate(element, transform) as GameObject;

			GameObject h = Instantiate(GlobalVariables.globalVariables.hands[i], newElement.transform.Find("HandHolder/Mask/HandObject")) as GameObject;
			h.transform.SetAsFirstSibling();

			newElement.transform.Find("SelectElementButton").GetComponent<Button>().onClick.RemoveAllListeners();
			int tempIndex = i;
			newElement.transform.Find("SelectElementButton").GetComponent<Button>().onClick.AddListener(delegate { SelectElement(tempIndex); });

			// Setujemo rotaciju elementa na 0,0,0 jer za drugog igraca moraju da budu naopacke
			Quaternion rot = newElement.transform.localRotation;
			rot.eulerAngles = Vector3.zero;
			newElement.transform.localRotation = rot;

			// Iskljucujemo locked slicicu ako je item otkljucan
			if (!GlobalVariables.globalVariables.lockedHandsIndices.Contains(i))
				newElement.transform.Find("LockHolder").gameObject.SetActive(false);

			// Ako je index 1 i 6 pozicioniramo native adove gde treba
			if (nativeAdIndices.Count > 0)
			{
				if (i == nativeAdIndices[0] - 1 || i == nativeAdIndices[1] - 1)
				{
					transform.GetChild(0).SetAsLastSibling();
				}
			}
		}

		if (nativeAdIndices.Count > 0)
		{
			// Povecavam index drugog nativeada za 1 jer sam ubacio izmedju elemenata prvi native pa je index + 1
			nativeAdIndices[1] += 1;
		}

		SetScrollRect();
	}

	// Ovde cemo da stavimo da se aktivira samo prva tackica za nivo posto setujemo uvek nivo na prvi posle menjanja sveta
	void OnEnable()
	{
		if (horizontalList/* && dotsCreated*/)
		{
//			foreach (Transform d in dotsHolder.transform)
//				d.GetChild(0).gameObject.SetActive(false);
//
//			// Setujemo da je prva tacka puna
//			dotsHolder.transform.GetChild(currentElement).GetChild(0).gameObject.SetActive(true);

			// Ako je brzina scroll recta manja od 0.01 setujemo ga na najblicu tacku
			if (Mathf.Abs(transform.parent.GetComponent<ScrollRect>().velocity.x) <= 0.01f)
				SetLerpPositionToClosestSnapPoint();
		}
		else
		{
			// Ukljucujemo ram za trenutni element
//			for (int i = 0; i < listOfElements.Count; i++)
//			{
//				if (i != currentElement)
//					listOfElements[i].transform.GetChild(1).gameObject.SetActive(false);
//			}

			// FIXME ovo je zakomentarisano da se ne bi pojavljivao zeleni okvir oko selektovanog sveta
//			listOfElements[currentElement].transform.GetChild(1).gameObject.SetActive(true);

			// Ako je brzina scroll recta manja od 0.01 setujemo ga na najblicu tacku
			if (Mathf.Abs(transform.parent.GetComponent<ScrollRect>().velocity.y) <= 0.01f)
				SetLerpPositionToClosestSnapPoint();
		}
	}

	/// <summary>
	/// Funkcija za setovanje velicine sroll recta, kao i svih elemenata i snap pointa za svaki element.
	/// </summary>
	public void SetScrollRect()
	{
		// If we are coming from another scene and we want focus to be on some
		// other character than first one
		//		if (GlobalVariables.selectedCharacterIndex != -1)
		//			currentElement = GlobalVariables.selectedCharacterIndex;
		//		else
//		currentElement = 0;

		/* FIXME
		nativeAdLoadedOnce = false;
		*/

		lerping = false;
		buttonPressed = false;

//		levelsLoaded = true;

		// Set size of the cell
		if (GetComponent<GridLayoutGroup>().cellSize == Vector2.zero)
		{
			Vector2 cellSize = new Vector2(cellSizeX, cellSizeY);
			GetComponent<GridLayoutGroup>().cellSize = cellSize;
		}
		else
		{
			cellSizeX = GetComponent<GridLayoutGroup>().cellSize.x;
			cellSizeY = GetComponent<GridLayoutGroup>().cellSize.y;
		}

		if (horizontalList)
		{
			transform.parent.GetComponent<ScrollRect>().horizontal = true;
			transform.parent.GetComponent<ScrollRect>().vertical = false;

			// Check if layout spacing differes from zero vector
			if (GetComponent<GridLayoutGroup>().spacing == Vector2.zero)
			{
				Vector2 spacingVector = new Vector2(spacing, 0);
				GetComponent<GridLayoutGroup>().spacing = spacingVector;
			}
			else
			{
				if (GetComponent<GridLayoutGroup>().spacing.x != 0)
					spacing = GetComponent<GridLayoutGroup>().spacing.x;

				Vector2 spacingVector = new Vector2(spacing, 0);
			}

			GetComponent<GridLayoutGroup>().startAxis = GridLayoutGroup.Axis.Vertical;
			GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedRowCount;
			GetComponent<GridLayoutGroup>().constraintCount = 1;
			currentElemCheckTemp = (cellSizeX + spacing) / 2;
		}
		else
		{
			transform.parent.GetComponent<ScrollRect>().horizontal = false;
			transform.parent.GetComponent<ScrollRect>().vertical = true;

			if (GetComponent<GridLayoutGroup>().spacing == Vector2.zero)
			{
				Vector2 spacingVector = new Vector2(0, spacing);
				GetComponent<GridLayoutGroup>().spacing = spacingVector;
			}
			else
			{
				if (GetComponent<GridLayoutGroup>().spacing.y != 0)
					spacing = GetComponent<GridLayoutGroup>().spacing.y;

				Vector2 spacingVector = new Vector2(0, spacing);
			}

			GetComponent<GridLayoutGroup>().startAxis = GridLayoutGroup.Axis.Horizontal;
			GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedColumnCount;
			GetComponent<GridLayoutGroup>().constraintCount = 1;
			currentElemCheckTemp = (cellSizeY + spacing) / 2;
		}

		// Set size delta of parent scroll rect so elements wouldn't be jumpy
		transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(cellSizeX, cellSizeY);

		// Set size delta of parent scroll rect so elements wouldn't be jumpy
		// U odnosu kako budemo zeleli fokus otkomentarisati samo ono odozgo ako bude trebao fokus da bude na sredini
		//		transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(cellSizeX, cellSizeY);
		//		transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(GameObject.Find("Canvas").GetComponent<CanvasScaler>().referenceResolution.x - spacing / 2f, cellSizeY);
		//		transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(cellSizeX + spacing, transform.parent.GetComponent<RectTransform>().anchoredPosition.y);

		snapPositions.Clear();
		snapPositions = new List<float>();

		//		if (currentElement == 0)
		//			leftArrow.SetActive(false);
		//		else if (currentElement == 5)
		//			rightArrow.SetActive(false);

		// Get all characters and put then into list
		listOfElements.Clear();
		int elementsCounter = 0;
		foreach(Transform t in transform)
		{
			// Add only elements that are active in hierarchy
			if (t.gameObject.activeInHierarchy)
			{
				listOfElements.Add(t.gameObject);

//				if (!dotsCreated && horizontalList)
//				{
//					// Kreiramo tackicu u meniju za svaki objekat
//					GameObject d = Instantiate(dotPrefab, Vector3.zero, dotPrefab.transform.rotation) as GameObject;
//					d.transform.SetParent(dotsHolder.transform);
//					d.transform.localScale = Vector3.one;
//					d.transform.GetChild(0).gameObject.SetActive(false);
//				}

				elementsCounter++;
			}
		}

		// Setujemo da smo kreirali tackicu za svaki nivo i aktiviramo samo prvu
//		dotsCreated = true;
//		if (horizontalList)
//			dotsHolder.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);

		// Set transform rect position and size depending of number of characters and spacing
		if (horizontalList)
		{
			GetComponent<RectTransform>().sizeDelta = new Vector2(listOfElements.Count * cellSizeX + (listOfElements.Count - 1) * spacing + 2 * frontAndBackAdditionSize, cellSizeY);
			GetComponent<RectTransform>().anchoredPosition = new Vector2(GetComponent<RectTransform>().sizeDelta.x - 2 * spacing + frontAndBackAdditionSize, GetComponent<RectTransform>().anchoredPosition.y);

			float startSnapPosition = GetComponent<RectTransform>().sizeDelta.x / 2 - cellSizeX / 2;
			snapPositions.Add(startSnapPosition);

			// Set fist character to be of focused scale
			listOfElements[0].transform.localScale = new Vector3(focusedElementScale, focusedElementScale, 1);

			for (int i = 1; i < listOfElements.Count; i++)
			{
				startSnapPosition -= cellSizeX + spacing;
				snapPositions.Add(startSnapPosition);

				// Set scale for not focused elements to be scale
				listOfElements[i].transform.localScale = new Vector3(unfocusedElementsScale, unfocusedElementsScale, 1);
//
//				if (shadeUnfocusedElements)
//				{
//					// Set shadeImages to be shaded
//					Color c = shadeImages[i].color;
//					c.r = 1f * shadePercentage;
//					c.g = 1f * shadePercentage;
//					c.b = 1f * shadePercentage;
//					shadeImages[i].color = c;
//				}
			}

		}
		else
		{
			GetComponent<RectTransform>().sizeDelta = new Vector2(cellSizeX, listOfElements.Count * cellSizeY + (listOfElements.Count - 1) * spacing);
			GetComponent<RectTransform>().anchoredPosition = new Vector2(GetComponent<RectTransform>().anchoredPosition.x, -(GetComponent<RectTransform>().sizeDelta.y - 2 * spacing));

			float startSnapPosition = GetComponent<RectTransform>().sizeDelta.y / 2 - cellSizeY / 2;
			snapPositions.Add(startSnapPosition);

			// Set fist character to be of focused scale
			listOfElements[0].transform.localScale = new Vector3(focusedElementScale, focusedElementScale, 1);

			for (int i = 1; i < listOfElements.Count; i++)
			{
				startSnapPosition -= cellSizeY + spacing;
				snapPositions.Add(startSnapPosition);

				// Set scale for not focused elements to be scale
				listOfElements[i].transform.localScale = new Vector3(unfocusedElementsScale, unfocusedElementsScale, 1);
//
//				if (shadeUnfocusedElements)
//				{
//					// Set shadeImages to be shaded
//					Color c = shadeImages[i].color;
//					c.r = 1f * shadePercentage;
//					c.g = 1f * shadePercentage;
//					c.b = 1f * shadePercentage;
//					shadeImages[i].color = c;
//				}
			}
		}
	}

	/// <summary>
	/// Funkcija koja nam nalazi najblizu element za snapovanje i snapuje scroll rect na taj element.
	/// </summary>
	public void SetLerpPositionToClosestSnapPoint()
	{
		for (int i = 0; i < snapPositions.Count; i++)
		{
			if (horizontalList)
			{
				if (transform.localPosition.x > snapPositions[i] - currentElemCheckTemp - 1 && transform.localPosition.x <= snapPositions[i] + currentElemCheckTemp)
				{
					newLerpPosition = new Vector3(snapPositions[i], transform.localPosition.y, 0);
					lerping = true;

					currentElement = i;
					break;
				}
				else if (transform.localPosition.x <= snapPositions[i] && i == snapPositions.Count - 1) // Ako je poslednji element
				{
					newLerpPosition = new Vector3(snapPositions[i], transform.localPosition.y, 0);
					lerping = true;

					currentElement = i;
					break;
				}
			}
			else
			{
				if (transform.localPosition.y > snapPositions[i] - currentElemCheckTemp - 1 && transform.localPosition.y <= snapPositions[i] + currentElemCheckTemp)
				{
					newLerpPosition = new Vector3(0, snapPositions[i], 0);
					lerping = true;

//					if (!levelsLoaded)
//					{
//						levelsLoaded = true;
//					}
				
					// FIXME ovo je zakomentarisano da se ne bi pojavljivao zeleni okvir oko selektovanog sveta
//					if (LevelSelectManager.levelSelectManager.lastSelectedWorld != listOfElements.Count - i - 1)
//					{
//						if (listOfElements[LevelSelectManager.levelSelectManager.lastSelectedWorld].transform.GetChild(1).gameObject.activeInHierarchy)
//							listOfElements[LevelSelectManager.levelSelectManager.lastSelectedWorld].transform.GetChild(1).gameObject.SetActive(false);
//					}
					
					currentElement = listOfElements.Count - i - 1;

					// FIXME ovo je zakomentarisano da se ne bi pojavljivao zeleni okvir oko selektovanog sveta
//					if (LevelSelectManager.levelSelectManager.lastSelectedWorld != currentElement)
//					{
//						if (!listOfElements[currentElement].transform.GetChild(1).gameObject.activeInHierarchy)
//							listOfElements[currentElement].transform.GetChild(1).gameObject.SetActive(true);
//					}
					break;
				}
			}
		}
	}
		
	/// <summary>
	/// Funkcija koja nam pomaze da nadjemo koji je trenutni elementi iz scroll recta aktivan.
	/// </summary>
	void SetCurrentElement()
	{
		for (int i = 0; i < snapPositions.Count; i++)
		{
			if (horizontalList)
			{
				if (transform.localPosition.x > snapPositions[i] - currentElemCheckTemp - 1 && transform.localPosition.x <= snapPositions[i] + currentElemCheckTemp)
				{
					if (currentElement != i)
					{
						currentElement = i;
					}
					break;
				}
			}
			else
			{
				if (transform.localPosition.y > snapPositions[i] - currentElemCheckTemp - 1 && transform.localPosition.y <= snapPositions[i] + currentElemCheckTemp)
				{
					if (currentElement != listOfElements.Count - i - 1)
					{
//						listOfElements[currentElement].transform.GetChild(1).gameObject.SetActive(false);
						currentElement = listOfElements.Count - i - 1;
//						listOfElements[currentElement].transform.GetChild(1).gameObject.SetActive(true);
					}
					break;
				}
			}
		}
	}

	/// <summary>
	/// Svrha ove korutine je da se saceka malo pre nego sto opet moze da se stisne dugme ili se klikne na neki odredjeni element.
	/// </summary>
	IEnumerator ButtonPressed()
	{
		yield return new WaitForSeconds(0.4f);
		buttonPressed = false;
	}

	/// <summary>
	/// Funkcija koja nam pomera snap point za jedan unazad, kao i lerpuje rekt na tu poziciju.
	/// </summary>
	public void BackwardButtonPressed()
	{
		if (horizontalList)
		{
			if (currentElement > 0 && !buttonPressed)
			{
				// Button pressed
				buttonPressed = true;

				currentElement -= 1;
				newLerpPosition = new Vector3(snapPositions[currentElement], transform.localPosition.y, 0);
				lerping = true;

				// Ako je igrac bio spreman podesavamo da vise nije spreman
				if (playerReady)
				{
					playerReady = false;
					playerReadyAnimator.Play("PlayerNotReady", 0, 0);
				}

				SoundManager.Instance.Play_Sound(SoundManager.Instance.buttonClick);

				StartCoroutine(ButtonPressed());
			}
		}
		else
		{
			if (currentElement > 0 && !buttonPressed)
			{
				// Button pressed
				buttonPressed = true;
				
				currentElement -= 1;
				newLerpPosition = new Vector3(transform.localPosition.x, snapPositions[listOfElements.Count - currentElement - 1], 0);
				lerping = true;

				// FIXME ovo je zakomentarisano da se ne bi pojavljivao zeleni okvir oko selektovanog sveta
//				listOfElements[LevelSelectManager.levelSelectManager.lastSelectedWorld].transform.GetChild(1).gameObject.SetActive(false);

				StartCoroutine(ButtonPressed());

				// Pustamo zvuk za skrolovanje svetova
//				SoundManager.Instance.Play_MenuScroll();
			}
		}
	}

	/// <summary>
	/// Funkcija koja nam pomera snap point za jedan unapred, kao i lerpuje rekt na tu poziciju.
	/// </summary>
	public void ForwardButtonPressed()
	{
		if (horizontalList)
		{
			if (currentElement < snapPositions.Count - 1 && !buttonPressed)
			{
				// Button pressed
				buttonPressed = true;

				currentElement += 1;
				newLerpPosition = new Vector3(snapPositions[currentElement], transform.localPosition.y, 0);
				lerping = true;

				// Ako je igrac bio spreman podesavamo da vise nije spreman
				if (playerReady)
				{
					playerReady = false;
					playerReadyAnimator.Play("PlayerNotReady", 0, 0);
				}

				SoundManager.Instance.Play_Sound(SoundManager.Instance.buttonClick);

				StartCoroutine(ButtonPressed());
			}
		}
		else
		{
			if (currentElement < listOfElements.Count - 1 && !buttonPressed)
			{
				// Button pressed
				buttonPressed = true;

				currentElement += 1;
				newLerpPosition = new Vector3(transform.localPosition.x, snapPositions[listOfElements.Count - currentElement - 1], 0);
				lerping = true;

				// FIXME ovo je zakomentarisano da se ne bi pojavljivao zeleni okvir oko selektovanog sveta
//				listOfElements[LevelSelectManager.levelSelectManager.lastSelectedWorld].transform.GetChild(1).gameObject.SetActive(false);

				StartCoroutine(ButtonPressed());

				// Pustamo zvuk za skrolovanje svetova
//				SoundManager.Instance.Play_MenuScroll();
			}
		}
	}

	/// <summary>
	/// Funkcija koja nam enabluje dugme da mozemo opet da klikcemo na njega. Koristi se za backwarf i forward dugmice.
	/// </summary>
	public void SetButtonActive(GameObject button)
	{
		button.GetComponent<Button>().interactable = true;

	}

	/// <summary>
	/// Funkcija koja nam disabluje dugme da mozemo opet da klikcemo na njega. Koristi se za backwarf i forward dugmice.
	/// </summary>
	public void SetButtonInactive(GameObject button)
	{
		button.GetComponent<Button>().interactable = false;
	}

	/// <summary>
	/// Funkcija koju pozivako kada se klikne na neki od elemenata
	/// </summary>
	public void SelectElement(int index)
	{
        if (!GlobalVariables.globalVariables.lockedHandsIndices.Contains(index))
        {
            if (!GlobalVariables.aiMatch)
            {
                if (player == 1 && !playerReady)
                {
                    GlobalVariables.player1HandIndex = index;
                    playerReady = true;
                    playerReadyAnimator.Play("PlayerReady", 0, 0);
                    SoundManager.Instance.Play_Sound(SoundManager.Instance.handSelected);
                    
                    // Proveravamo da li je i drugi igrac spreman
                    if (GameObject.Find("ScrollRectPlayer2/ScrollObjectHolder").GetComponent<ScrollRectSnapController>().playerReady)
                    {
                        StartCoroutine("LoadLevelSceneAsynchronously");
                        //                              Application.LoadLevel("Level");
                    }
                }
                else if (player == 2 && !playerReady)
                {
                    GlobalVariables.player2HandIndex = index;
                    playerReady = true;
                    playerReadyAnimator.Play("PlayerReady", 0, 0);
                    SoundManager.Instance.Play_Sound(SoundManager.Instance.handSelected);
                    
                    // Proveravamo da li je i drugi igrac spreman
                    if (GameObject.Find("ScrollRectPlayer1/ScrollObjectHolder").GetComponent<ScrollRectSnapController>().playerReady)
                    {
                        StartCoroutine("LoadLevelSceneAsynchronously");
                        //                              Application.LoadLevel("Level");
                    }
                }
            }
            else
            {
                if (!playerReady)
                {
                    GlobalVariables.player1HandIndex = index;
                    playerReady = true;
                    playerReadyAnimator.Play("PlayerReady", 0, 0);
                    SoundManager.Instance.Play_Sound(SoundManager.Instance.handSelected);
                    
                    GlobalVariables.player2HandIndex = UnityEngine.Random.Range(0, GlobalVariables.globalVariables.hands.Length);
                    
                    StartCoroutine("LoadLevelSceneAsynchronously");
                    //                          Application.LoadLevel("Level");
                }
            }
        }
        else
        {
            listOfElements[currentElement].transform.Find("LockHolder/AnimationHolder").GetComponent<Animator>().Play("LockedHandClicked", 0, 0);
            
            GlobalVariables.objectForUnlocking = listOfElements[currentElement];
            GlobalVariables.unlockingObjectIndex = index;
            GlobalVariables.unlockingObjectScrollRectIndex = currentElement;
            
            SoundManager.Instance.Play_Sound(SoundManager.Instance.buttonClick);
            
            // Pustamo video ako je dostupan
            AdsManager.Instance.IsVideoRewardAvailable();
        }
	}

	void Update()
	{
		// If we are holding button than do not lerp
		if ((Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && !buttonPressed)
		{
			holdingRect = true;
			SetCurrentElement();
			newLerpPosition = transform.localPosition;
		}

//		if (Input.GetMouseButtonUp(0))
//		{
//			holdingRect = false;
//			levelsLoaded = false;
//			dotsAndWorldFrameSet = false;
//		}

		// If not lerping and velocityis small enough find closest snap point and lerp to it
		if (horizontalList)
		{
			if (!lerping && !holdingRect && Mathf.Abs(transform.parent.GetComponent<ScrollRect>().velocity.x) >= 0.1f && Mathf.Abs(transform.parent.GetComponent<ScrollRect>().velocity.x) < 100f)
			{
				SetLerpPositionToClosestSnapPoint();
			}
			else
			{
				SetCurrentElement();
			}
		}
		else
		{
			if (!lerping && !holdingRect && Mathf.Abs(transform.parent.GetComponent<ScrollRect>().velocity.y) >= 0.1f && Mathf.Abs(transform.parent.GetComponent<ScrollRect>().velocity.y) < 100f)
			{
				SetLerpPositionToClosestSnapPoint();
			}
//			else if (!lerping && !holdingRect && Mathf.Abs(transform.parent.GetComponent<ScrollRect>().velocity.y) < 0.1f)
//			{
//				SetCurrentElement();
//				SetLerpPositionToClosestSnapPoint();
//			}
//			else
//			{
//				SetCurrentElement();
//			}
		}
			
		// Set appropriate for elements in list according to distance from current snap point
		if (Mathf.Abs(transform.parent.GetComponent<ScrollRect>().velocity.x) >= 0f)
		{
			// Ako je igrac bio spreman podesavamo da vise nije spreman
			if (playerReady && Mathf.Abs(transform.parent.GetComponent<ScrollRect>().velocity.x) >= 100f)
			{
				playerReady = false;
				playerReadyAnimator.Play("PlayerNotReady", 0, 0);
			}
			
			if (horizontalList)
			{
				if(currentElement == 0)
				{
					float sb = Mathf.Abs(Mathf.Abs(snapPositions[currentElement] - transform.localPosition.x - currentElemCheckTemp * 2) * (focusedElementScale - unfocusedElementsScale) / Mathf.Abs(currentElemCheckTemp * 2) - focusedElementScale);
					float s = Mathf.Abs(Mathf.Abs(snapPositions[currentElement] - transform.localPosition.x) * (focusedElementScale - unfocusedElementsScale) / Mathf.Abs(currentElemCheckTemp * 2) - focusedElementScale);

					if (s <= unfocusedElementsScale)
						s = unfocusedElementsScale;

					if (sb <= unfocusedElementsScale)
						sb = unfocusedElementsScale;

					listOfElements[currentElement].transform.localScale = new Vector3(s, s, 1);

					listOfElements[currentElement + 1].transform.localScale = new Vector3(sb, sb, 1);
//
//					if (shadeUnfocusedElements)
//					{
//						// Set shadeImages to be shadedaccorgind to percentage
//						if (shadePercentage != unfocusedElementsScale)
//						{
//							sb = Mathf.Abs(Mathf.Abs(snapPositions[currentElement] - transform.localPosition.x - currentElemCheckTemp * 2) * (1f - shadePercentage) / Mathf.Abs(currentElemCheckTemp * 2) - 1f);
//							s = Mathf.Abs(Mathf.Abs(snapPositions[currentElement] - transform.localPosition.x) * (1f - shadePercentage) / Mathf.Abs(currentElemCheckTemp * 2) - 1f);
//						}
//
//						Color c = shadeImages[currentElement].color;
//						c.r = 1f * s;
//						c.g = 1f * s;
//						c.b = 1f * s;
//						shadeImages[currentElement].color = c;
//
//						c = shadeImages[currentElement + 1].color;
//						c.r = 1f * sb;
//						c.g = 1f * sb;
//						c.b = 1f * sb;
//						shadeImages[currentElement + 1].color = c;
//					}

//					if (!GlobalVariables.isAmazon)
//					{
//						if (LevelSelectManager.levelSelectManager.isInternetAvailable)
//						{
					/* FIXME
						if (nativeAdIndices.Count > 0)
						{
							if (nativeAd.GetComponent<FacebookNativeAd>().loadedOnce && nativeAd.transform.GetChild(0).gameObject.activeInHierarchy)
							{
								//FIXME zakomentarisano
//									nativeAdLoadedOnce = false;
								nativeAd.GetComponent<FacebookNativeAd>().CancelLoading();
								nativeAd.GetComponent<FacebookNativeAd>().HideNativeAd();
							}
						}
						*/
//						}
//					}
				}
				else if(currentElement == listOfElements.Count - 1)
				{
					float s = Mathf.Abs(Mathf.Abs(snapPositions[currentElement] - transform.localPosition.x) * (focusedElementScale - unfocusedElementsScale) / Mathf.Abs(currentElemCheckTemp * 2) - focusedElementScale);
					float sf = Mathf.Abs(Mathf.Abs(snapPositions[currentElement] - transform.localPosition.x + currentElemCheckTemp * 2) * (focusedElementScale - unfocusedElementsScale) / Mathf.Abs(currentElemCheckTemp * 2) - focusedElementScale);

					if (s <= unfocusedElementsScale)
						s = unfocusedElementsScale;

					if (sf <= unfocusedElementsScale)
						sf = unfocusedElementsScale;

					listOfElements[currentElement - 1].transform.localScale = new Vector3(sf, sf, 1);
					listOfElements[currentElement].transform.localScale = new Vector3(s, s, 1);
//
//					if (shadeUnfocusedElements)
//					{
//						// Set shadeImages to be shaded
//						if (shadePercentage != shadePercentage)
//						{
//							s = Mathf.Abs(Mathf.Abs(snapPositions[currentElement] - transform.localPosition.x) * (1f - shadePercentage) / Mathf.Abs(currentElemCheckTemp * 2) - 1f);
//							sf = Mathf.Abs(Mathf.Abs(snapPositions[currentElement] - transform.localPosition.x + currentElemCheckTemp * 2) * (1f - shadePercentage) / Mathf.Abs(currentElemCheckTemp * 2) - 1f);
//						}
//
//						Color c = shadeImages[currentElement].color;
//						c.r = 1f * s;
//						c.g = 1f * s;
//						c.b = 1f * s;
//						shadeImages[currentElement].color = c;
//
//						c = shadeImages[currentElement - 1].color;
//						c.r = 1f * sf;
//						c.g = 1f * sf;
//						c.b = 1f * sf;
//						shadeImages[currentElement - 1].color = c;
//					}
				}
				else
				{
					float sb = Mathf.Abs(Mathf.Abs(snapPositions[currentElement] - transform.localPosition.x - currentElemCheckTemp * 2) * (focusedElementScale - unfocusedElementsScale) / Mathf.Abs(currentElemCheckTemp * 2) - focusedElementScale);
	             	float s = Mathf.Abs(Mathf.Abs(snapPositions[currentElement] - transform.localPosition.x) * (focusedElementScale - unfocusedElementsScale) / Mathf.Abs(currentElemCheckTemp * 2) - focusedElementScale);
	                float sf = Mathf.Abs(Mathf.Abs(snapPositions[currentElement] - transform.localPosition.x + currentElemCheckTemp * 2) * (focusedElementScale - unfocusedElementsScale) / Mathf.Abs(currentElemCheckTemp * 2) - focusedElementScale);

					if (s <= unfocusedElementsScale)
						s = unfocusedElementsScale;
					
					if (sb <= unfocusedElementsScale)
						sb = unfocusedElementsScale;

					if (sf <= unfocusedElementsScale)
						sf = unfocusedElementsScale;

					listOfElements[currentElement - 1].transform.localScale = new Vector3(sf, sf, 1);
					listOfElements[currentElement].transform.localScale = new Vector3(s, s, 1);
					listOfElements[currentElement + 1].transform.localScale = new Vector3(sb, sb, 1);
//
//					if (shadeUnfocusedElements)
//					{
//						// Set shadeImages to be shaded
//						if (shadePercentage != unfocusedElementsScale)
//						{
//							sb = Mathf.Abs(Mathf.Abs(snapPositions[currentElement] - transform.localPosition.x - currentElemCheckTemp * 2) * (1f - shadePercentage) / Mathf.Abs(currentElemCheckTemp * 2) - 1f);
//							s = Mathf.Abs(Mathf.Abs(snapPositions[currentElement] - transform.localPosition.x) * (1f - shadePercentage) / Mathf.Abs(currentElemCheckTemp * 2) - focusedElementScale);
//							sf = Mathf.Abs(Mathf.Abs(snapPositions[currentElement] - transform.localPosition.x + currentElemCheckTemp * 2) * (1f - shadePercentage) / Mathf.Abs(currentElemCheckTemp * 2) - 1f);
//						}
//
//						Color c = shadeImages[currentElement].color;
//						c.r = 1f * s;
//						c.g = 1f * s;
//						c.b = 1f * s;
//						shadeImages[currentElement].color = c;
//
//						c = shadeImages[currentElement - 1].color;
//						c.r = 1f * sf;
//						c.g = 1f * sf;
//						c.b = 1f * sf;
//						shadeImages[currentElement - 1].color = c;
//
//						c = shadeImages[currentElement + 1].color;
//						c.r = 1f * sb;
//						c.g = 1f * sb;
//						c.b = 1f * sb;
//						shadeImages[currentElement + 1].color = c;
//					}

//					if (!GlobalVariables.isAmazon)
//					{
//						if (LevelSelectManager.levelSelectManager.isInternetAvailable)
//						{
					/* FIXME
						if (nativeAdIndices.Count > 0)
						{
							if (nativeAdIndices.Contains(currentElement) && !menuManager.popupOpened)
							{
								nativeAd = listOfElements[currentElement];
								if (!nativeAd.transform.GetChild(0).gameObject.activeInHierarchy && !nativeAd.GetComponent<FacebookNativeAd>().loadedOnce)
								{
//									nativeAd.GetComponent<FacebookNativeAd>().loadedOnce = true;
									
									// Pozivamo prikaz native ada
									nativeAd.GetComponent<FacebookNativeAd>().LoadAd();
								}
								else if (!nativeAd.transform.GetChild(0).gameObject.activeInHierarchy && nativeAd.GetComponent<FacebookNativeAd>().loadedOnce)
								{
									// Pozivamo prikaz native ada
									nativeAd.GetComponent<FacebookNativeAd>().ShowNativeAd();
								}
							}
							else
							{
								if (nativeAd.transform.GetChild(0).gameObject.activeInHierarchy)
								{
									// FIXME zakomentarisano
//									nativeAdLoadedOnce = false;
									// Sakrivamo native ad
									nativeAd.GetComponent<FacebookNativeAd>().CancelLoading();
									nativeAd.GetComponent<FacebookNativeAd>().HideNativeAd();
								}
							}
						}
					*/
//						}
//						else if (nativeAdLoadedOnce)
//						{
//							nativeAdLoadedOnce = false;
//
//							// Sakrivamo native ad
//							nativeAd.GetComponent<FacebookNativeAd>().CancelLoading();
//							nativeAd.GetComponent<FacebookNativeAd>().HideNativeAd();
//						}
//					}
				}
			}
//			else
//			{
//				if(currentElement == 0)
//				{
//					float sb = Mathf.Abs(Mathf.Abs(snapPositions[currentElement] + transform.localPosition.y - currentElemCheckTemp * 2) * (focusedElementScale - unfocusedElementsScale) / Mathf.Abs(currentElemCheckTemp * 2) - focusedElementScale);
//	             	float s = Mathf.Abs(Mathf.Abs(snapPositions[currentElement] + transform.localPosition.y) * (focusedElementScale - unfocusedElementsScale) / Mathf.Abs(currentElemCheckTemp * 2) - focusedElementScale);
//
//					if (s <= unfocusedElementsScale)
//						s = unfocusedElementsScale;
//					
//					if (sb <= unfocusedElementsScale)
//						sb = unfocusedElementsScale;
//
//					listOfElements[currentElement].transform.localScale = new Vector3(s, s, 1);
//					listOfElements[currentElement + 1].transform.localScale = new Vector3(sb, sb, 1);
//
//					if (shadeUnfocusedElements)
//					{
//						// Set shadeImages to be shaded
//						if (shadePercentage != unfocusedElementsScale)
//						{
//							sb = Mathf.Abs(Mathf.Abs(snapPositions[currentElement] + transform.localPosition.y - currentElemCheckTemp * 2) * (1f - shadePercentage) / Mathf.Abs(currentElemCheckTemp * 2) - 1f);
//							s = Mathf.Abs(Mathf.Abs(snapPositions[currentElement] + transform.localPosition.y) * (1f - shadePercentage) / Mathf.Abs(currentElemCheckTemp * 2) - 1f);
//						}
//
//						Color c = shadeImages[currentElement].color;
//						c.r = 1f * s;
//						c.g = 1f * s;
//						c.b = 1f * s;
//						shadeImages[currentElement].color = c;
//
//						c = shadeImages[currentElement + 1].color;
//						c.r = 1f * sb;
//						c.g = 1f * sb;
//						c.b = 1f * sb;
//						shadeImages[currentElement + 1].color = c;
//					}
//				}
//				else if(currentElement == listOfElements.Count - 1)
//				{
//					float s = Mathf.Abs(Mathf.Abs(snapPositions[currentElement] + transform.localPosition.y) * (focusedElementScale - unfocusedElementsScale) / Mathf.Abs(currentElemCheckTemp * 2) - focusedElementScale);
//	                float sf = Mathf.Abs(Mathf.Abs(snapPositions[currentElement] + transform.localPosition.y + currentElemCheckTemp * 2) * (focusedElementScale - unfocusedElementsScale) / Mathf.Abs(currentElemCheckTemp * 2) - focusedElementScale);
//
//					if (s <= unfocusedElementsScale)
//						s = unfocusedElementsScale;
//					
//					if (sf <= unfocusedElementsScale)
//						sf = unfocusedElementsScale;
//
//					listOfElements[currentElement - 1].transform.localScale = new Vector3(sf, sf, 1);
//					listOfElements[currentElement].transform.localScale = new Vector3(s, s, 1);
//
//					if (shadeUnfocusedElements)
//					{
//						// Set shadeImages to be shaded
//						if (shadePercentage != unfocusedElementsScale)
//						{
//							s = Mathf.Abs(Mathf.Abs(snapPositions[currentElement] + transform.localPosition.y) * (1f - shadePercentage) / Mathf.Abs(currentElemCheckTemp * 2) - 1f);
//							sf = Mathf.Abs(Mathf.Abs(snapPositions[currentElement] + transform.localPosition.y + currentElemCheckTemp * 2) * (1f - shadePercentage) / Mathf.Abs(currentElemCheckTemp * 2) - 1f);
//						}
//
//						Color c = shadeImages[currentElement].color;
//						c.r = 1f * s;
//						c.g = 1f * s;
//						c.b = 1f * s;
//						shadeImages[currentElement].color = c;
//
//						c = shadeImages[currentElement - 1].color;
//						c.r = 1f * sf;
//						c.g = 1f * sf;
//						c.b = 1f * sf;
//						shadeImages[currentElement - 1].color = c;
//					}
//				}
//				else
//				{
//					float sb = Mathf.Abs(Mathf.Abs(snapPositions[currentElement] + transform.localPosition.y - currentElemCheckTemp * 2) * (focusedElementScale - unfocusedElementsScale) / Mathf.Abs(currentElemCheckTemp * 2) - focusedElementScale);
//	         		float s = Mathf.Abs(Mathf.Abs(snapPositions[currentElement] + transform.localPosition.y) * (focusedElementScale - unfocusedElementsScale) / Mathf.Abs(currentElemCheckTemp * 2) - focusedElementScale);
//	           		float sf = Mathf.Abs(Mathf.Abs(snapPositions[currentElement] + transform.localPosition.y + currentElemCheckTemp * 2) * (focusedElementScale - unfocusedElementsScale) / Mathf.Abs(currentElemCheckTemp * 2) - focusedElementScale);
//
//					if (s <= unfocusedElementsScale)
//						s = unfocusedElementsScale;
//					
//					if (sb <= unfocusedElementsScale)
//						sb = unfocusedElementsScale;
//					
//					if (sf <= unfocusedElementsScale)
//						sf = unfocusedElementsScale;
//
//					listOfElements[currentElement - 1].transform.localScale = new Vector3(sf, sf, 1);
//					listOfElements[currentElement].transform.localScale = new Vector3(s, s, 1);
//					listOfElements[currentElement + 1].transform.localScale = new Vector3(sb, sb, 1);
//
//					if (shadeUnfocusedElements)
//					{
//						// Set shadeImages to be shaded
//						if (shadePercentage != unfocusedElementsScale)
//						{
//							sb = Mathf.Abs(Mathf.Abs(snapPositions[currentElement] + transform.localPosition.y - currentElemCheckTemp * 2) * (1f - shadePercentage) / Mathf.Abs(currentElemCheckTemp * 2) - 1f);
//							s = Mathf.Abs(Mathf.Abs(snapPositions[currentElement] + transform.localPosition.y) * (1f - shadePercentage) / Mathf.Abs(currentElemCheckTemp * 2) - 1f);
//							sf = Mathf.Abs(Mathf.Abs(snapPositions[currentElement] + transform.localPosition.y + currentElemCheckTemp * 2) * (1f - shadePercentage) / Mathf.Abs(currentElemCheckTemp * 2) - 1f);
//						}
//
//						Color c = shadeImages[currentElement].color;
//						c.r = 1f * s;
//						c.g = 1f * s;
//						c.b = 1f * s;
//						shadeImages[currentElement].color = c;
//
//						c = shadeImages[currentElement - 1].color;
//						c.r = 1f * sf;
//						c.g = 1f * sf;
//						c.b = 1f * sf;
//						shadeImages[currentElement - 1].color = c;
//
//						c = shadeImages[currentElement + 1].color;
//						c.r = 1f * sb;
//						c.g = 1f * sb;
//						c.b = 1f * sb;
//						shadeImages[currentElement + 1].color = c;
//					}
//				}
//			}
		}

		// If we let the mouse button and velocity small enough
		if (lerping)
		{
			transform.localPosition = Vector3.Lerp(transform.localPosition, newLerpPosition, lerpingSpeed);

			// Ako nisu postavljene tacke i ram, postavljamo ih
//			if (!dotsAndWorldFrameSet)
//			{
//				dotsAndWorldFrameSet = true;
//
//				if (!horizontalList)
//				{
//					// Ukljucujemo ram za trenutni element // FIXME ovo je zakomentarisano da se ne bi pojavljivao zeleni okvir oko selektovanog sveta
////					listOfElements[currentElement].transform.GetChild(1).gameObject.SetActive(true);
//
////					if (currentElement != LevelSelectManager.levelSelectManager.selectedWorld)
////						LevelSelectManager.levelSelectManager.WorldSelected(currentElement);
//				}
//				else
//				{
//					// Proveravamo da li je lista horizontalna i ako jeste aktiviramo tackicu koja je odgovarajuca za ovaj nivo
//					// Iskljucimo sve tackice i ukljucimo samo onu koja nam treba
//					for (int i = 0; i < dotsHolder.transform.childCount; i++)
//					{
//						dotsHolder.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
//					}
//
//					dotsHolder.transform.GetChild(currentElement).GetChild(0).gameObject.SetActive(true);
//				}
//			}

			if (Vector3.Distance(transform.localPosition, newLerpPosition) < 1f)
			{
				transform.localPosition = newLerpPosition;
				transform.parent.GetComponent<ScrollRect>().velocity = new Vector3(0, 0, 0);

//				if (!horizontalList)
//				{
//					
//
//					// Iskljucujemo ramove za ostale elemente
////					for (int i = 0; i < listOfElements.Count; i++)
////					{
////						if (i != currentElement)
////						{
////							listOfElements[i].transform.GetChild(1).gameObject.SetActive(false);
////						}
////					}
////
////					listOfElements[LevelSelectManager.levelSelectManager.lastSelectedWorld].transform.GetChild(1).gameObject.SetActive(false);
//
//
//				}

				lerping = false;

				levelClicked = false;

//				if (horizontalList)
//				{
//					// Proveravamo da li je lista horizontalna i ako jeste aktiviramo tackicu koja je odgovarajuca za ovaj nivo
//					// Iskljucimo sve tackice i ukljucimo samo onu koja nam treba
//					for (int i = 0; i < dotsHolder.transform.childCount; i++)
//					{
//						dotsHolder.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
//					}
//
//					dotsHolder.transform.GetChild(currentElement).GetChild(0).gameObject.SetActive(true);
//				}

//				for (int i = 0; i < listOfElements.Count; i++)
//				{
//					if (i != currentElement)
//					{
//						listOfElements[i].transform.localScale = new Vector3(unfocusedElementsScale, unfocusedElementsScale, 1);
//					}
//				}
			}
		}

		if (Input.GetMouseButtonUp(0))
		{
			holdingRect = false;
//			levelsLoaded = false;
//			dotsAndWorldFrameSet = false;

			if (horizontalList && !levelClicked)
			{
				if (!lerping && !holdingRect && Mathf.Abs(transform.parent.GetComponent<ScrollRect>().velocity.x) < 0.1f)
					SetLerpPositionToClosestSnapPoint();
			}
			else
			{
				if (!lerping && !holdingRect && Mathf.Abs(transform.parent.GetComponent<ScrollRect>().velocity.y) < 0.1f)
					SetLerpPositionToClosestSnapPoint();
			}
		}

		if (horizontalList)
		{
			// Updating arrow buttons
			if (transform.localPosition.x > snapPositions[snapPositions.Count - 1] + Mathf.Abs(spacing) / 2)
			{
				SetButtonActive(forwardButton);
			}
			else
			{
				SetButtonInactive(forwardButton);
			}

			if (transform.localPosition.x < snapPositions[0] - Mathf.Abs(spacing) / 2)
			{
				SetButtonActive(backwardButton);
			}
			else
			{
				SetButtonInactive(backwardButton);
			}
		}
		else
		{
			// Updating arrow buttons
			if (transform.localPosition.y > snapPositions[snapPositions.Count - 1] + Mathf.Abs(spacing) / 2)
			{
				SetButtonActive(backwardButton);
			}
			else
			{
				SetButtonInactive(backwardButton);
			}
			
			if (transform.localPosition.y < snapPositions[0] - Mathf.Abs(spacing) / 2)
			{
				SetButtonActive(forwardButton);
			}
			else
			{
				SetButtonInactive(forwardButton);
			}
		}
	}

	IEnumerator LoadLevelSceneAsynchronously()
	{	
		GameObject canvas = GameObject.Find("Canvas");
		canvas.transform.Find("ClickBlocker").gameObject.SetActive(true);

		yield return new WaitForSeconds(0.7f);	
		AsyncOperation async = SceneManager.LoadSceneAsync("Level");
		async.allowSceneActivation = false;

		GlobalVariables.playLoadingDepart = true;
		GameObject.Find("LoadingHolder/AnimationHolder").GetComponent<Animator>().Play("LoadingArrive", 0, 0);
		SoundManager.Instance.Play_Sound(SoundManager.Instance.loadingArrive);

		yield return new WaitForSeconds(1.5f);

		async.allowSceneActivation = true;
		yield return async;
	}

	void OnApplicationPause(bool paused)
	{
		if (!paused)
			SetLerpPositionToClosestSnapPoint();
	}
}
