#if UNITY_EDITOR  
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ItemShape))]
public class SimpleItemShapeEditor : PropertyDrawer
{
    private const int MatrixLen = 4; //4x4 ����  
    private const float CellSize = 20f; //�༭����ÿ�����ӵĴ�С  

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        //���Ʊ���  
        Rect titleRect = new Rect(position.x, position.y, position.width, 20f);
        EditorGUI.LabelField(titleRect, label);

        //��ȡ shapeMatrix ��״��������  
        SerializedProperty matrixProp = property.FindPropertyRelative("shapeMatrix");

        //���� 4x4 ����  
        Rect gridRect = new Rect(position.x, position.y + 25f, MatrixLen * CellSize, MatrixLen * CellSize);
        for (int x = 0; x < MatrixLen; x++)
        {
            for (int y = 0; y < MatrixLen; y++)
            {
                int index = x * MatrixLen + y; //����һά��������  

                //����GUI����ϵԭ�������Ͻǣ���������������ԭ�������½ǣ���Ҫת��
                Rect cellRect = new Rect(
                    gridRect.x + x * CellSize,
                    gridRect.y + (MatrixLen - y - 1) * CellSize,
                    CellSize - 2f,
                    CellSize - 2f
                );

                // ��ȡ��ǰ���ӵ�ֵ  
                bool currentValue = matrixProp.GetArrayElementAtIndex(index).boolValue;

                // ���Ƹ��Ӳ��������л�״̬  
                bool newValue = GUI.Toggle(cellRect, currentValue, GUIContent.none, "Button");
                if (newValue != currentValue)
                {
                    matrixProp.GetArrayElementAtIndex(index).boolValue = newValue;
                    matrixProp.serializedObject.ApplyModifiedProperties();
                }
            }
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 25f + MatrixLen * CellSize + 10f; // �ܸ߶�  
    }
}
#endif