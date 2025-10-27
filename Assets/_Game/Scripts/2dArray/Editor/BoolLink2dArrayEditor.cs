using MatrixAlgebra;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(BoolLink2dArray), false)]
public class BoolLink2dArrayEditor : Bool2dArrayEditor
{
    protected BoolLink2dArray boolLink2DArray;
    protected SerializedProperty linksArray;

    //OnGUI specifies what is drawn
    public override void OnDrowGUI(Rect position, SerializedProperty property)
    {
        linksArray = property.FindPropertyRelative("links");
        boolLink2DArray = (BoolLink2dArray)valueArrayObject;

        base.OnDrowGUI(position, property);

        Handles.color = Color.blue;

        for (int i = 0; i < boolLink2DArray.links.Length; ++i)
        {
            int index1 = boolLink2DArray.links[i].point1;
            int index2 = boolLink2DArray.links[i].point2;
            int index1i = index1 % x;
            int index1j = index1 / x;
            int index2i = index2 % x;
            int index2j = index2 / x;
            Vector2 offset1 = start + new Vector2((cellSize.x) * index1i - dragslidder + intSize.x / 2f, size.y - (cellSize.y) * (index1j + 0.9f));
            Vector2 offset2 = start + new Vector2((cellSize.x) * index2i - dragslidder + intSize.x / 2f, size.y - (cellSize.y) * (index2j + 0.9f));

            Vector2 offset = Quaternion.Euler(0, 0, 90) * (offset2 - offset1).normalized;

            {
                Handles.DrawLine(offset1 + offset * 2f, offset2 + offset * 2f);
                Handles.DrawLine(offset1 + offset * 1f, offset2 + offset * 1f);
                Handles.DrawLine(offset1, offset2);
                Handles.DrawLine(offset1 - offset * 1f, offset2 - offset * 1f);
                Handles.DrawLine(offset1 - offset * 2f, offset2 - offset * 2f);
            }
        }
    }

    protected override void ControlClickToggleBool(SerializedProperty toogleBool, bool newV, bool oldV, int index = 0)
    {
        if (!control)
        {
            toogleBool.boolValue = newV;
            if (!newV && oldV != newV)
            {
                ClearConnection(index);
                selectedNode = -1;
            }

            if (click)
                selectedNode = -1;
        }
        else if (click)
        {
            //if (oldV)
            //{
            //    if (newV)
            //    {
            //        if (selectedNode != -1)
            //        {
            //            if (selectedNode == index)
            //            {
            //                selectedNode = -1;
            //            }
            //            else
            //            {
            //                CreateConnection(selectedNode, index);
            //                selectedNode = -1;
            //            }
            //        }
            //        else
            //        {
            //            selectedNode = index;
            //        }
            //    }
            //}
            //else if (newV)
            //{
            //    selectedNode = -1;
            //}

            if (newV)
            {
                if (selectedNode != -1)
                {
                    if (selectedNode == index)
                    {
                        selectedNode = -1;
                    }
                    else
                    {
                        CreateConnection(selectedNode, index);
                    }
                }
                else
                {
                    selectedNode = index;
                }
            }

        }
    }
    protected virtual void CreateConnection(int index1, int index2)
    {
        if (boolLink2DArray.m[index1] && boolLink2DArray.m[index1])
        {
            selectedNode = -1;

            int countIndex = linksArray.arraySize;
            int newIndex1 = Mathf.Min(index1, index2);
            int newIndex2 = Mathf.Max(index1, index2);

            for (int i = 0; i < boolLink2DArray.links.Length; i++)
            {
                if (boolLink2DArray.links[i].point1 == newIndex1)
                {
                    if (boolLink2DArray.links[i].point2 == newIndex2)
                    {
                        linksArray.DeleteArrayElementAtIndex(i);
                        return;
                    }
                }
            }

            linksArray.InsertArrayElementAtIndex(countIndex);

            SerializedProperty link = linksArray.GetArrayElementAtIndex(countIndex);

            link.FindPropertyRelative("point1").intValue = newIndex1;
            link.FindPropertyRelative("point2").intValue = newIndex2;
        }

        selectedNode = index2;
    }
    protected virtual void ClearConnection(int index1)
    {
        for (int i = 0; i < linksArray.arraySize; i++)
        {
            SerializedProperty couLink = linksArray.GetArrayElementAtIndex(i);
            int in1 = couLink.FindPropertyRelative("point1").intValue;
            int in2 = couLink.FindPropertyRelative("point2").intValue;
            if (in1 == index1 || in2 == index1)
            {
                linksArray.DeleteArrayElementAtIndex(i);
                i--;
            }
        }
    }

    protected override void SerArrayResize(int oldX, int oldY)
    {
        int length = x * y;

        int offset = 0;

        //for (int i = 0; i < boolLink2DArray.links.Length; i++)
        //{
        //    int index1 = boolLink2DArray.links[i].point1;
        //    int index2 = boolLink2DArray.links[i].point2;

        //    int index1i = index1 % x;
        //    int index1j = index1 / x;
        //    int index2i = index2 % x;
        //    int index2j = index2 / x;

        //    SerializedProperty link = linksArray.GetArrayElementAtIndex(i);
        //    link.FindPropertyRelative("point1").intValue = index1j * x + index1i;
        //    link.FindPropertyRelative("point2").intValue = index2j * x + index2i;
        //}

        for (int i = 0; i < boolLink2DArray.links.Length; i++)
        {
            if (boolLink2DArray.links[i].point1 >= length || boolLink2DArray.links[i].point2 >= length)
            {
                linksArray.DeleteArrayElementAtIndex(i - offset);
                offset++;
            }
        }
        base.SerArrayResize(oldX, oldY);
    }
}
