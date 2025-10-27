using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class CellSpring : MonoBehaviour
{
    public List<int> ColorIndex = new List<int>();
    public int springindex;
    public float hight = 0;
    public bool isCorner = false;



    public List<Vector3> cornersDir;
    public Dictionary<Vector3, CellSpring> adjustCells = new Dictionary<Vector3, CellSpring>();
    public Dictionary<Vector3, int> dirWhitColor = new Dictionary<Vector3, int>();
    public List<CellSpring> cellSprings = new List<CellSpring>();
    public CellData cellData;
    public bool isFull = false;

    public List<GameObject> activeWalls = new List<GameObject>();

    public BlockColor currentblockColor;
    public (int, int) IndexInGrid;
    public GameObject arrow;
    public void SetHight()
    {
        if (springindex >= 1 && springindex <= 5)
            hight = 1;
        else if (springindex > 5 && springindex <= 10)
            hight = 1.5f;
        else if (springindex > 10 && springindex <= 15)
            hight = 2;
    }

    public void SetAdjustSells(Vector3 dir)
    {
        cornersDir.Add(dir);
    }

    public void SetAdjust(Vector3 dir, CellSpring cell)
    {
        adjustCells.Add(dir, cell);
        cellSprings.Add(cell);
    }

    public void SetWallColor(GameObject wall)
    {
        if (!isCorner)
        {
            wall.transform.GetChild(0).GetComponent<MeshRenderer>().material = new Material(GameManager.Instance.colorsCELL[ColorIndex[0]]);
            dirWhitColor.Add(cornersDir[0], ColorIndex[0]);
            activeWalls.Add(wall);
        }
    }

    List<GameObject> lISTsPLITColor = new List<GameObject>();
    List<GameObject> lISTsPLITColor1 = new List<GameObject>();

    void SetColorsSplitsWalls()
    {
        lISTsPLITColor.Add(cellData.splitCornerUp1);
        lISTsPLITColor.Add(cellData.splitCornerRight1);
        lISTsPLITColor.Add(cellData.splitCornerLeft1);
        lISTsPLITColor.Add(cellData.splitCornerDown1);

    }
    void SetColorsSplitsWalls1()
    {
        lISTsPLITColor1.Add(cellData.splitCornerUp2);
        lISTsPLITColor1.Add(cellData.splitCornerRight2);
        lISTsPLITColor1.Add(cellData.splitCornerLeft2);
        lISTsPLITColor1.Add(cellData.splitCornerDown2);
    }

    public void SetWallColorCorner()
    {
        if (!isCorner)
            return;
        ColorIndex.Clear();
        for (int i = 0; i < cellSprings.Count; i++)
        {
            ColorIndex.Add(cellSprings[i].ColorIndex[0]);
        }
        SetColorsSplitsWalls();
        SetColorsSplitsWalls1();
        for (int i = 0; i < cornersDir.Count; i++)
        {
            int col = i == 0 ? 1 : 0;
            GameObject wall = null;
            GridSpowner gridSpowner = GridSpowner.instance;
            Vector3 dir = Vector3.zero;
            int indx = 0;
            if (cornersDir[i] == Vector3.up)
            {
                wall = cellData.upWall;
                dir = Vector3.up;              
                foreach (var item in lISTsPLITColor)
                {
                    item.GetComponentInChildren<MeshRenderer>().material = new Material(GameManager.
                      Instance.colorsCELL[cellSprings[1].ColorIndex[0]]);                   
                }
                foreach (var item in lISTsPLITColor1)
                {
                    item.GetComponentInChildren<MeshRenderer>().material = new Material(GameManager.
                      Instance.colorsCELL[cellSprings[0].ColorIndex[0]]);
                }
            }
            else if (cornersDir[i] == Vector3.right)
            {
                wall = cellData.rightWall;
                dir = Vector3.right;

                foreach (var item in lISTsPLITColor)
                {
                    item.GetComponentInChildren<MeshRenderer>().material = new Material(GameManager.
                      Instance.colorsCELL[cellSprings[1].ColorIndex[0]]);
                }
                foreach (var item in lISTsPLITColor1)
                {
                    item.GetComponentInChildren<MeshRenderer>().material = new Material(GameManager.
                      Instance.colorsCELL[cellSprings[0].ColorIndex[0]]);
                }
            }
            else if (cornersDir[i] == Vector3.left)
            {
                wall = cellData.leftWall;
                dir = Vector3.left;

                foreach (var item in lISTsPLITColor)
                {
                    item.GetComponentInChildren<MeshRenderer>().material = new Material(GameManager.
                      Instance.colorsCELL[cellSprings[1].ColorIndex[0]]);
                }
                foreach (var item in lISTsPLITColor1)
                {
                    item.GetComponentInChildren<MeshRenderer>().material = new Material(GameManager.
                      Instance.colorsCELL[cellSprings[0].ColorIndex[0]]);
                }
            }
            else if (cornersDir[i] == Vector3.down)
            {
                wall = cellData.downWall;
                dir = Vector3.down;
                indx = 0;
                foreach (var item in lISTsPLITColor)
                {
                    item.GetComponentInChildren<MeshRenderer>().material = new Material(GameManager.
                      Instance.colorsCELL[cellSprings[1].ColorIndex[0]]);
                }
                foreach (var item in lISTsPLITColor1)
                {
                    item.GetComponentInChildren<MeshRenderer>().material = new Material(GameManager.
                      Instance.colorsCELL[cellSprings[0].ColorIndex[0]]);
                }
            }
            activeWalls.Add(wall);
            wall.transform.GetChild(0).GetComponent<MeshRenderer>().material = new Material(GameManager.
                    Instance.colorsCELL[cellSprings[col].ColorIndex[0]]);

            dirWhitColor.Add(dir, cellSprings[col].ColorIndex[0]);
        }

    }
    public int GetWallByDir(Vector3 dir)
    {
        return dirWhitColor[dir];
    }
    public void Shake(float duration, float yStrength, float scaleStrength, int vibrato = 20, float randomness = 90f, bool fadeOut = true)
    {
        if (activeWalls == null || activeWalls.Count == 0) return;

        Vector3 posStrength = new Vector3(0f, Mathf.Abs(yStrength), 0f);
        Vector3 sclStrength = Vector3.one * Mathf.Abs(scaleStrength);

        foreach (var wall in activeWalls)
        {
            if (!wall) continue;

            // Don’t replay if already shaking
            if (DOTween.IsTweening(wall, true)) continue;

            // Capture exact starts (LOCAL to shake in place relative to parent)
            Vector3 startPos = wall.transform.localPosition;
            Vector3 startScl = wall.transform.localScale;

            // Kill any leftover tweens targeting this wall
            DOTween.Kill(wall, complete: false);

            // Build tweens: strictly localPosition Y-only + scale
            Tween posT = wall.transform.DOShakePosition(
                duration: duration,
                strength: posStrength,
                vibrato: vibrato,
                randomness: randomness,
                fadeOut: fadeOut,
                snapping: false
            );

            //Tween sclT = wall.transform.DOShakeScale(
            //    duration: duration,
            //    strength: sclStrength,
            //    vibrato: vibrato,
            //    randomness: randomness,
            //    fadeOut: fadeOut
            //);

            // Run both in parallel and ensure we restore transforms on finish or interrupt
            Sequence seq = DOTween.Sequence()
                .SetId(wall)           // so Kill(wall) targets this sequence
                .Join(posT)
                //.Join(sclT)
                .OnKill(() =>
                {
                    // Snap back if interrupted
                    if (wall)
                    {
                        wall.transform.localPosition = startPos;
                        wall.transform.localScale = startScl;
                    }
                })
                .OnComplete(() =>
                {
                    // Ensure exact final values
                    if (wall)
                    {
                        wall.transform.localPosition = startPos;
                        wall.transform.localScale = startScl;
                    }
                });
        }
    }

    public void BounceUpDown(CellSpring cell, float duration, float scaleDuration, float yOffset, float scaleStrength, Ease ease = Ease.OutQuad)
    {
        if (activeWalls == null || activeWalls.Count == 0) return;

        foreach (var wall in activeWalls)
        {
            if (!wall) continue;
            cell.activeWalls[0].transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
            // If already tweening, skip to avoid replaying
            if (DOTween.IsTweening(wall, true)) continue;

            // Capture exact starts (LOCAL so it moves relative to parent)
            Vector3 startPos = wall.transform.localPosition;
            Vector3 startScl = wall.transform.localScale;

            // Kill any leftover tweens targeting this wall
            DOTween.Kill(wall, complete: false);

            // Target position: move up by yOffset, then back
            Vector3 upPos = startPos + new Vector3(0f, Mathf.Abs(yOffset), 0f);

            // Build the sequence: up then down
            Sequence seq = DOTween.Sequence()
                .SetId(wall) // so Kill(wall) targets this sequence
                .Append(wall.transform.DOScaleY(scaleStrength, scaleDuration * 0.5f).SetEase(Ease.InQuad))
                .Append(wall.transform.DOLocalMove(upPos, duration * 0.5f).SetEase(ease))
                .Append(wall.transform.DOLocalMove(startPos, duration * 0.5f).SetEase(Ease.InQuad))
                .Append(wall.transform.DOScaleY(1, scaleDuration * 0.5f).SetEase(Ease.InQuad))
                .OnKill(() =>
                {
                    // Snap back if interrupted
                    if (wall)
                    {
                        wall.transform.localPosition = startPos;
                        wall.transform.localScale = startScl;
                        wall.transform.localScale = Vector3.one;
                        cell.activeWalls[0].transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);

                    }
                })
                .OnComplete(() =>
                {
                    // Ensure exact final values
                    if (wall)
                    {
                        wall.transform.localPosition = startPos;
                        wall.transform.localScale = startScl;
                        wall.transform.localScale = Vector3.one;
                        cell.activeWalls[0].transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
                    }
                });
        }
    }
}


[Serializable]
public struct CellData
{
    // Walls
    public GameObject upWall;
    public GameObject rightWall;
    public GameObject leftWall;
    public GameObject downWall;
    // Corners
    public GameObject cornerUp;
    public GameObject cornerRight;
    public GameObject cornerLeft;
    public GameObject cornerDown;
    // plitCorners
    public GameObject splitCornerUp1;
    public GameObject splitCornerUp2;
    public GameObject splitCornerRight1;
    public GameObject splitCornerRight2;
    public GameObject splitCornerLeft1;
    public GameObject splitCornerLeft2;
    public GameObject splitCornerDown1;
    public GameObject splitCornerDown2;


}
