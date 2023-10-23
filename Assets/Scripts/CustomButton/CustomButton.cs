using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/CustomButton")]
	public class CustomButton : Selectable, IPointerDownHandler, ISubmitHandler, IPointerUpHandler
	{
		[Serializable]
		public class ButtonOnDownEvent : UnityEvent {}

		[Serializable]
		public class ButtonOnUpEvent : UnityEvent {}

		// Event delegates triggered on click.
		[FormerlySerializedAs("OnDown")]
		[SerializeField]
		private ButtonOnDownEvent m_OnDown = new ButtonOnDownEvent();

		// Event delegates triggered on click.
		[FormerlySerializedAs("OnDown")]
		[SerializeField]
		private ButtonOnUpEvent m_OnUp = new ButtonOnUpEvent();

		protected CustomButton()
		{}

		public ButtonOnDownEvent onDown
		{
			get { return m_OnDown; }
			set { m_OnDown = value; }
		}

		public ButtonOnUpEvent onUp
		{
			get { return m_OnUp; }
			set { m_OnUp = value; }
		}

		private void PressDown()
		{
			if (!IsActive() || !IsInteractable())
				return;

			m_OnDown.Invoke();
		}

		private void PressUp()
		{
			if (!IsActive() || !IsInteractable())
				return;

			m_OnUp.Invoke();
		}

		// Trigger all registered callbacks.
		public virtual void OnPointerDown(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			PressDown();

		}

		// Trigger all registered callbacks.
		public virtual void OnPointerUp(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			PressUp();

		}

		public virtual void OnSubmit(BaseEventData eventData)
		{
			PressDown();
			PressUp();
			// if we get set disabled during the press
			// don't run the coroutine.
			if (!IsActive() || !IsInteractable())
				return;

			DoStateTransition(SelectionState.Pressed, false);
			StartCoroutine(OnFinishSubmit());
		}

		private IEnumerator OnFinishSubmit()
		{
			var fadeTime = colors.fadeDuration;
			var elapsedTime = 0f;

			while (elapsedTime < fadeTime)
			{
				elapsedTime += Time.unscaledDeltaTime;
				yield return null;
			}

			DoStateTransition(currentSelectionState, false);
		}

	}
}