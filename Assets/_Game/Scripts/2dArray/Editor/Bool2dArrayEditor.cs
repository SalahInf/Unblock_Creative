using MatrixAlgebra;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Bool2dArray),false)]
public class Bool2dArrayEditor : PropertyDrawer
{
	protected Vector2Int cellSize = new Vector2Int(31, 31);

	protected Vector2 intSize = new Vector2(30, 30);
	protected Vector2 labelSize = new Vector2(100, 15);

	protected Color cellColorFalse = new Color (0.8f, 0.8f, 0.8f);
	protected Color cellColorTrue = new Color (0.3f, 0.3f, 0.3f);
	protected Color cellColorSelected = new Color(0.5f, 1, 0.5f, 1);
	protected Color ColorAplha1 = new Color(0, 0, 0, 0);


	protected int selectedNode = -1;

	protected int x;
	protected int y;

	protected Vector2 start;
	protected Vector2 size;

	protected SerializedProperty serArray;

	protected bool control = false;
	protected bool click = false;
	protected bool shift = false;

	protected KeyCode controlKeyCode;

	protected Bool2dArray valueArrayObject;

	protected static float dragslidder;

	protected Texture2D textureRectSelectet;
	//GetPropertyHeight specifies the height needed when drawing an Int2dArray
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
		if (!textureRectSelectet)
		{
			textureRectSelectet = new Texture2D(cellSize.x, cellSize.y, TextureFormat.RGBA32, false);
			textureRectSelectet.Rect(4, cellColorSelected, ColorAplha1);
			textureRectSelectet.alphaIsTransparency = true;
			textureRectSelectet.Apply();
		}

		if(valueArrayObject == null)
		{
			var targetObject = property.serializedObject.targetObject;
			var targetObjectClassType = targetObject.GetType();
			var field = targetObjectClassType.GetField(property.propertyPath);

			valueArrayObject = (Bool2dArray)field.GetValue(targetObject);
		}

		return 90 + property.FindPropertyRelative("y").intValue * 31;
	}

    //OnGUI specifies what is drawn
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
		EditorGUI.BeginProperty(position, label, property);

		EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		OnDrowGUI(position, property);

		EditorGUI.EndProperty();
	}

	public virtual void OnDrowGUI(Rect position, SerializedProperty property)
    {
		GUIStyle styleFalse = new GUIStyle(GUIStyle.none);
		styleFalse.alignment = TextAnchor.MiddleCenter;
		styleFalse.padding = new RectOffset(0, 0, 0, 0);

		GUIStyle styleError = new GUIStyle(EditorStyles.label);
		styleError.normal.textColor = Color.red;

		SerializedProperty serializedY = property.FindPropertyRelative("y");
		SerializedProperty serializedX = property.FindPropertyRelative("x");

		x = serializedX.intValue;
		y = serializedY.intValue;

		Rect rectL = new Rect(position.min + new Vector2(0, 20), labelSize);

		Rect rectXDec = new Rect(position.min + new Vector2(110, 20), intSize);
		Rect rectX = new Rect(position.min + new Vector2(140, 15), intSize);
		Rect rectXAdd = new Rect(position.min + new Vector2(170, 20), intSize);

		Rect rectYDec = new Rect(position.min + new Vector2(210, 20), intSize);
		Rect rectY = new Rect(position.min + new Vector2(240, 15), intSize);
		Rect rectYAdd = new Rect(position.min + new Vector2(270, 20), intSize);

		EditorGUI.LabelField(rectL, "Dimensions");

		int newY = EditorGUI.IntField(rectY, y, styleFalse);
		int newX = EditorGUI.IntField(rectX, x, styleFalse);

		if (GUI.Button(rectXDec, "-", EditorStyles.miniButtonMid))
		{
			newX--;
		}

		if (GUI.Button(rectXAdd, "+", EditorStyles.miniButtonMid))
		{
			newX++;
		}

		if (GUI.Button(rectYDec, "-", EditorStyles.miniButtonMid))
		{
			newY--;
		}

		if (GUI.Button(rectYAdd, "+", EditorStyles.miniButtonMid))
		{
			newY++;
		}

		serArray = property.FindPropertyRelative("m");

		newX = Mathf.Max(1, newX);
		newY = Mathf.Max(1, newY);

		if (x != newX || y != newY)
		{
			serializedX.intValue = newX;
			serializedY.intValue = newY;
			serArray.arraySize = newX * newY;

			for (int i = x * y; i < newX * newY; i++)
			{
				SerializedProperty intProp = serArray.GetArrayElementAtIndex(i);
				intProp.boolValue = false;
			}

			if (newX != x)
			{
				Bool2dArray tmpArray = new Bool2dArray(valueArrayObject);
                for (int i = 0; i < tmpArray.x && i < newX; i++)
                {
                    for (int j = 0; j < tmpArray.y && j < newY; j++)
                    {
						bool value = tmpArray[i,j];
						int index = j *newX + i;

						SerializedProperty intProp = serArray.GetArrayElementAtIndex(index);
						intProp.boolValue = value;
					}
                }
			}

			int oldX = x;
			int oldY = y;

			x = newX;
			y = newY;

			SerArrayResize(oldX , oldY);
		}

		float pad = position.xMax - (x + 2) * cellSize.x;
		if (pad < position.xMin) pad = position.xMin;

		start = new Vector2(pad, position.yMin + 40f);
		size = new Vector2(cellSize.x * x + 1, cellSize.y * y + 1);
		EditorGUI.DrawRect(new Rect(start, size), Color.black);

		start += Vector2.one;

		control = false;
		click = false;
		shift = false;


		if (Event.current.shift)
			shift = true;

		if (Event.current.control)
		{
			control = true;
		}

		if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
		{
			click = true;
		}

		if (Event.current.type == EventType.KeyDown || Event.current.type == EventType.KeyUp)
			controlKeyCode = Event.current.keyCode;

		int vTrueCount = 0;
		for (int j = 0; j < y; ++j)
		{
			for (int i = 0; i < x; ++i)
			{
				Vector2 offset = new Vector2(cellSize.x * i - dragslidder, size.y - cellSize.y * (j + 1));
				Rect rectV = new Rect(start + offset, intSize);

				int n = j * x + i;

				SerializedProperty intProp = serArray.GetArrayElementAtIndex(n);
				bool oldValue = intProp.boolValue;

				bool newIntValue = EditorGUI.Toggle(rectV, (control) ? false : oldValue, styleFalse);

				EditorGUI.DrawRect(rectV, (oldValue) ? cellColorTrue : cellColorFalse);

				if (selectedNode == n)
				{
					GUI.DrawTexture(rectV,textureRectSelectet);
				}
				


				ControlClickToggleBool(intProp, newIntValue, oldValue, n);

				if (intProp.boolValue)
					vTrueCount++;
			}
		}

        OnDrowGUIDragSLidderH(start + Vector2.up * (size.y + 2), position.xMax);

    }

    protected virtual void OnDrowGUIDragSLidderH(Vector2 start, float maxX)
    {
		if (size.x > maxX - 10)
		{
			Rect rect0 = new Rect(start, new Vector2(maxX + 35, 20));
			dragslidder = EditorGUI.Slider(rect0, dragslidder, 0, cellSize.x * x - maxX + 50);
		}
        else
        {
            dragslidder = 0;
        }
    }


	protected virtual void SerArrayResize(int oldX, int oldY)
    {
    }
    protected virtual void ControlClickToggleBool(SerializedProperty toogleBool, bool newV, bool oldV, int index = 0)
    {
		if (!control)
		{
			toogleBool.boolValue = newV;
		}
	}
}
