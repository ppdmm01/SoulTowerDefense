#if UNITY_EDITOR  
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ItemShape))]
public class SimpleItemShapeEditor : PropertyDrawer
{
    private const int MatrixLen = 4; //4x4 网格  
    private const float CellSize = 20f; //编辑器中每个格子的大小  

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        //绘制标题  
        Rect titleRect = new Rect(position.x, position.y, position.width, 20f);
        EditorGUI.LabelField(titleRect, label);

        //获取 shapeMatrix 形状矩阵属性  
        SerializedProperty matrixProp = property.FindPropertyRelative("shapeMatrix");

        //绘制 4x4 网格  
        Rect gridRect = new Rect(position.x, position.y + 25f, MatrixLen * CellSize, MatrixLen * CellSize);
        for (int x = 0; x < MatrixLen; x++)
        {
            for (int y = 0; y < MatrixLen; y++)
            {
                int index = x * MatrixLen + y; //计算一维数组索引  

                //这里GUI坐标系原点是左上角，而背包网格坐标原点是左下角，需要转换
                Rect cellRect = new Rect(
                    gridRect.x + x * CellSize,
                    gridRect.y + (MatrixLen - y - 1) * CellSize,
                    CellSize - 2f,
                    CellSize - 2f
                );

                // 获取当前格子的值  
                bool currentValue = matrixProp.GetArrayElementAtIndex(index).boolValue;

                // 绘制格子并允许点击切换状态  
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
        return 25f + MatrixLen * CellSize + 10f; // 总高度  
    }
}
#endif