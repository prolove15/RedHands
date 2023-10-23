using UnityEngine.UI;

namespace UnityEditor.UI
{
	[CustomEditor(typeof(CustomButton), true)]
	[CanEditMultipleObjects]
	public class CustomButtonEditor : SelectableEditor
	{
		SerializedProperty m_OnDownProperty;
		SerializedProperty m_OnUpProperty;

		protected override void OnEnable()
		{
			base.OnEnable();
			m_OnDownProperty = serializedObject.FindProperty("m_OnDown");
			m_OnUpProperty = serializedObject.FindProperty("m_OnUp");
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			EditorGUILayout.Space();

			serializedObject.Update();
			EditorGUILayout.PropertyField(m_OnDownProperty);
			EditorGUILayout.PropertyField(m_OnUpProperty);
			serializedObject.ApplyModifiedProperties();
		}
	}
}