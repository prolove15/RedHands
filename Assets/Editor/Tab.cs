using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor;
using UnityEngine.EventSystems;
public static class Tab {

	[MenuItem("GameObject/UI/WebelinxButton", false, 0)]
	static void Init() {

		Canvas[] canvases = (Canvas[])Resources.FindObjectsOfTypeAll<Canvas>();
		if(GameObject.Find("Canvas") != null && canvases.Length>0)
		{
			Debug.Log("1 "+canvases.Length);
			var go = new GameObject("WebelinxButton", typeof(Image),typeof(CustomButton));
			go.transform.SetParent(canvases[0].transform);
			go.transform.localScale = Vector3.one;
			go.transform.localPosition = Vector3.zero;
		}
		else
		{
			Debug.Log("2 "+canvases.Length);
			var canvas = new GameObject("Canvas", typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));
			var go = new GameObject("WebelinxButton", typeof(Image),typeof(CustomButton));
			canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
			canvas.GetComponent<Canvas>().worldCamera = Camera.main;
			canvas.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			canvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1080,1920); //FIX za landscape ide obrnuto
			canvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 0.5f;
			go.transform.SetParent(canvas.transform);
			go.transform.localScale = Vector3.one;
			go.transform.localPosition = Vector3.zero;

		}

		if(GameObject.Find("EventSystem")==null)
		{
			var eventsystem = new GameObject("EventSystem", typeof(EventSystem),typeof(StandaloneInputModule));
		}
		else
		{
			if(GameObject.Find("EventSystem").GetComponent<EventSystem>())
			{
				if(!GameObject.Find("EventSystem").GetComponent<StandaloneInputModule>())
				{
					GameObject.Find("EventSystem").AddComponent(typeof(StandaloneInputModule));
				}
			}
			else 
			{
				GameObject.Find("EventSystem").AddComponent(typeof(EventSystem));
				GameObject.Find("EventSystem").AddComponent(typeof(StandaloneInputModule));
			}
		}
	}
}
