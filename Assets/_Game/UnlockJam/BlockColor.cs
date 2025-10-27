using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockColor : MonoBehaviour
{

    public CellSpring currentCell;

    public MeshRenderer meshRenderer;
    public MeshRenderer meshRenderer1;
    public int colorBlockIndex;

    public void InitColors(int blockColor)
    {
        switch (blockColor)
        {
            case 1: // no color
                colorBlockIndex = 0;
                return;
            case 2:// noir
                colorBlockIndex = 9;
                return;
            case 3://Yelow
                colorBlockIndex = 4;
                return;
            case 4://bleu
                colorBlockIndex = 2;
                return;
            case 5:// orange
                colorBlockIndex = 3;
                return;
            case 6:// red
                colorBlockIndex = 6;
                return;
            case 7://White
                colorBlockIndex = 1;
                return;
            case 8://Purple
                colorBlockIndex = 5;
                return;
            case 9://Cyan
                colorBlockIndex = 7;
                return;

        }

        colorBlockIndex = 0;
        return;
    }


    public void SetColor(int index, CellSpring cell)
    {
        //if (meshRenderer == null)
        //{
        //    meshRenderer = GetComponentInChildren<ski>();
        //    meshRenderer1 = GetComponentInChildren<MeshRenderer>();
        //}
        Material mat = new Material(GameManager.Instance.colorsWagon[index]);
        meshRenderer.material = mat;
        meshRenderer1.material = mat;
        cell.isFull = true;
        currentCell = cell;
        InitColors(index);
    }
}
