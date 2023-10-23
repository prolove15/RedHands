using UnityEngine;
using System.Collections;
using UnityEngine.UI;
/// <summary>
/// Scene: NA
/// Objects: itemHolder(s) 
/// Description: Used on all list holders that require dynamic content fitting.
///				 Works on single column/row lists.  
/// </summary>
public class DynamicContentSize : MonoBehaviour {
	/// <summary>
	/// The is vertical flag. Set true for column layout based lists (vertical).Otherwise set to false.
	/// </summary>
	public bool isVertical = true;
	/// <summary>
	/// The item spacing.
	/// </summary>
	public float itemSpacing=10f;
	/// <summary>
	/// The size of the item.
	/// </summary>
	public float itemSize;
	/// <summary>
	/// Sets the size of holder and size and order of childrenchildern.
	/// Call this function after all children has been assigned to holder.
	/// </summary>
	public void SetSizeAndChildern()
	{
		StartCoroutine (WaitAndDoWork ());
	}

	IEnumerator WaitAndDoWork ()
	{
		//transform's children gets updated only at the end of current frame
		//wait till the end of frame   to process shildren 
		yield return new WaitForEndOfFrame ();

		//if transform has more than 0 children
		if (transform.childCount > 0) {
			//is this vertical list
			if (isVertical) {
				gameObject.GetComponent<RectTransform> ().sizeDelta = new Vector2 (gameObject.GetComponent<RectTransform> ().sizeDelta.x, 1f);
				//gameObject.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0f, 0f);
				int childCount = gameObject.transform.childCount;
				gameObject.transform.GetChild (0).GetComponent<RectTransform> ().localScale = Vector3.one;
				itemSize = gameObject.transform.GetChild (0).GetComponent<RectTransform> ().sizeDelta.y;
				Vector2 newSize = new Vector2 ();
				newSize.x = gameObject.GetComponent<RectTransform> ().sizeDelta.x;
				newSize.y = (childCount + 1) * itemSpacing + childCount * itemSize;
				gameObject.GetComponent<RectTransform> ().sizeDelta = newSize;
				float startPositionY = newSize.y / 2 - itemSpacing - itemSize / 2;
				
				for (int i=0; i<gameObject.transform.childCount; i++) {//for every child
					gameObject.transform.GetChild (i).GetComponent<RectTransform> ().localScale = Vector3.one;
					gameObject.transform.GetChild (i).GetComponent<RectTransform> ().sizeDelta = new Vector2 (newSize.x, itemSize);
					gameObject.transform.GetChild (i).transform.localPosition = new Vector3 (0, startPositionY, 0);
					startPositionY -= itemSpacing + itemSize;
					
				}

			}//if this is horizontal list 
			else {
				gameObject.GetComponent<RectTransform> ().sizeDelta = new Vector2 (1f, gameObject.GetComponent<RectTransform> ().sizeDelta.y);
				int childCount = gameObject.transform.childCount;
				gameObject.transform.GetChild (0).GetComponent<RectTransform> ().localScale = Vector3.one;
				itemSize = gameObject.transform.GetChild (0).GetComponent<RectTransform> ().sizeDelta.x;
				Vector2 newSize = new Vector2 ();
				newSize.y = gameObject.GetComponent<RectTransform> ().sizeDelta.y;
				newSize.x = (childCount + 1) * itemSpacing + childCount * itemSize;
				gameObject.GetComponent<RectTransform> ().sizeDelta = newSize;
				float startPositionX = newSize.x / 2 - itemSpacing - itemSize / 2;
				
				for (int i=0; i<gameObject.transform.childCount; i++) {//for every child
					gameObject.transform.GetChild (i).GetComponent<RectTransform> ().localScale = Vector3.one;
					gameObject.transform.GetChild (i).GetComponent<RectTransform> ().sizeDelta = new Vector2 (itemSize, newSize.y);
					gameObject.transform.GetChild (i).transform.localPosition = new Vector3 (startPositionX, 0, 0);
					startPositionX -= itemSpacing + itemSize;
					
				}
				
			}
			//set object to be centered in parent holder
			//gameObject.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0f, 0f);
			
		}
	}
}
