using MatrixAlgebra;
using UnityEngine;


[CreateAssetMenu(fileName = "GridData", menuName = "ScriptableObjects/GridData", order = 1)]
public class Level : ScriptableObject
{
    public Int2dArray matrix;

    [Space(10)]
    public int boardCount;
}
