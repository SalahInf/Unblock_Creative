#if UNITY_EDITOR
using UnityEngine;
#endif
namespace MatrixAlgebra
{
    [System.Serializable]
    public class Int2dArray : Array2D<int>
    {
#if UNITY_EDITOR
        public static Color[] celllsColor = new Color[]
        {
            new Color (1f, 1f, 0.8f),
            new Color (1f, 1f, 1f),
            new Color (0f, 0.345f, 0.878f),
            new Color (0.91f, 0.525f, 0),
            new Color (0.949f, 0.918f, 0f),
            new Color (.776f, 0f, .91f),
            new Color (1f, 0f, 0f),
            new Color (0,0.8f,0.8f),
            new Color (1f,1f,1f),
            new Color (0,0,0),

        };
        [System.NonSerialized] public bool update;
#endif

        public Int2dArray(int width, int height) : base(width, height)
        {
        }

        public Int2dArray(int width, int height, int[] elemnts) : base(width, height, elemnts)
        {
        }

        public Int2dArray(Int2dArray bool2DArray) : base(bool2DArray)
        {

        }
    }
}