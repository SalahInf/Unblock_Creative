using MatrixAlgebra;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpowner : MonoBehaviour
{
    public static GridSpowner instance;
    public PPin2dGroundArr gridCells;
    public Int2dArray matrix;

    [Space(2)]
    [SerializeField] CellSpring cell;
    [Space]
    [SerializeField] SpringConntroller _springController;

    private int boardCount;
    public CellSpring[] cellSprings;

    public List<List<CellSpring>> springPoints;
    public int cols => matrix.cols;
    public int rows => matrix.rows;
    public List<SpringConntroller> activeSprings;

    List<GameObject> _spownedObjects = new List<GameObject>();
    public List<SpringConntroller> springConntrollers = new List<SpringConntroller>();
    private void Awake()
    {
        if (instance != null) Destroy(this);
        instance = this;

    }
    public void Init(int level)
    {
        matrix = Root.GameManager.levelList[level].matrix;
        boardCount = Root.GameManager.levelList[level].boardCount;
        cellSprings = new CellSpring[boardCount];
        foreach (var item in _spownedObjects)
        {
            if (item != null)
                Destroy(item);
        }
        _spownedObjects.Clear();
        activeSprings.Clear();
        springConntrollers.Clear();
        StartCoroutine(WaitForSpown());
        SetCorners();
        FindNoAdjacentCells();
        SpownWalls();
        FindAdjacentCells();
    }

    void SpownWalls()
    {
        int _index = 0;
        Vector3 _dir = Vector3.zero;


        int colosX = cols / 2;
        int rowsY = rows / 2;
        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                _index++;
                CellSpring _tmpCell = gridCells[i, j];
                _tmpCell.IndexInGrid = (i, j);
                if (_tmpCell.cornersDir.Count > 0)
                {
                    foreach (var obj in _tmpCell.cornersDir)
                    {
                        _dir += obj;

                        if (obj == Vector3.left)
                        {
                            _tmpCell.cellData.leftWall.SetActive(true);
                            _tmpCell.SetWallColor(_tmpCell.cellData.leftWall);
                            if (i == colosX|| j == rowsY)
                                _tmpCell.cellData.leftWall.transform.GetChild(1).gameObject.SetActive(true);
                        }

                        if (obj == Vector3.right)
                        {
                            _tmpCell.cellData.rightWall.SetActive(true);
                            _tmpCell.SetWallColor(_tmpCell.cellData.rightWall);
                            if (i == colosX || j == rowsY) _tmpCell.cellData.rightWall.transform.GetChild(1).gameObject.SetActive(true);
                        }
                        if (obj == Vector3.up)
                        {
                            _tmpCell.cellData.upWall.SetActive(true);
                            _tmpCell.SetWallColor(_tmpCell.cellData.upWall);
                            if (i == colosX || j == rowsY) _tmpCell.cellData.upWall.transform.GetChild(1).gameObject.SetActive(true);
                        }
                        if (obj == Vector3.down)
                        {
                            _tmpCell.cellData.downWall.SetActive(true);
                            _tmpCell.SetWallColor(_tmpCell.cellData.downWall);
                            if (i == colosX || j == rowsY) _tmpCell.cellData.downWall.transform.GetChild(1).gameObject.SetActive(true);
                        }
                    }

                    Vector3 posCell = _tmpCell.transform.position;

                    if (_tmpCell.isCorner)
                    {
                        if (posCell.x == -(cols - 1f) / 2f && posCell.z == -(rows - 1f) / 2f)
                        {
                            _tmpCell.cellData.cornerLeft.SetActive(true);
                        }
                        if (posCell.x == (cols - 1) - (cols - 1f) / 2f && posCell.z == -(rows - 1f) / 2f)
                        {
                            _tmpCell.cellData.cornerRight.SetActive(true);
                        }
                        if (posCell.z == (rows - 1) - (rows - 1f) / 2f && posCell.x == -(cols - 1f) / 2f)
                        {
                            _tmpCell.cellData.cornerUp.SetActive(true);
                        }
                        if (posCell.z == (rows - 1) - (rows - 1f) / 2f && posCell.x == (cols - 1) - (cols - 1f) / 2f)
                        {
                            _tmpCell.cellData.cornerDown.SetActive(true);
                        }
                    }
                }
            }
        }
    }
    void FindAdjacentCells()
    {
        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                var cell = gridCells[i, j];
                if (cell == null)
                    continue;

                // Left
                if (i > 0 && gridCells[i - 1, j] != null)
                {
                    cell.SetAdjust(Vector3.left, gridCells[i - 1, j]);
                }

                // Right
                if (i < cols - 1 && gridCells[i + 1, j] != null)
                {
                    cell.SetAdjust(Vector3.right, gridCells[i + 1, j]);
                }

                // Down
                if (j > 0 && gridCells[i, j - 1] != null)
                {
                    cell.SetAdjust(Vector3.down, gridCells[i, j - 1]);
                }

                // Up
                if (j < rows - 1 && gridCells[i, j + 1] != null)
                {
                    cell.SetAdjust(Vector3.up, gridCells[i, j + 1]);
                }
            }
        }

        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                gridCells[i, j].SetWallColorCorner();
            }
        }

    }
    void FindNoAdjacentCells()
    {
        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {

                if (i == 0 || gridCells[i - 1, j] == null)
                {
                    Vector3Int dir = Vector3Int.zero;
                    gridCells[i, j].SetAdjustSells(Vector3.left);
                }


                if (i == cols - 1 || gridCells[i + 1, j] == null)
                {
                    Vector3Int dir = Vector3Int.zero;
                    gridCells[i, j].SetAdjustSells(Vector3.right);
                }


                if (j == 0 || gridCells[i, j - 1] == null)
                {
                    Vector3Int dir = Vector3Int.zero;
                    gridCells[i, j].SetAdjustSells(Vector3.down);
                }


                if (j == rows - 1 || gridCells[i, j + 1] == null)
                {
                    Vector3Int dir = Vector3Int.zero;
                    gridCells[i, j].SetAdjustSells(Vector3.up);
                }
            }
        }
    }
    [SerializeField] BlockColor _block;
    void SpownBlocks(Vector3 pos, int colorindex, ref CellSpring cell)
    {
        BlockColor block = Instantiate(_block, pos /*+ Vector3.up * 0.3f*/, Quaternion.identity, transform);
        //block.transform.position = pos;
        block.SetColor(colorindex, cell);
        cell.currentblockColor = block;
    }
    IEnumerator WaitForSpown()
    {
        gridCells = new PPin2dGroundArr(cols, rows);
        springPoints = new List<List<CellSpring>>();
        activeSprings = new List<SpringConntroller>();

        for (int i = 0; i < gridCells.cols; i++)
        {
            for (int j = 0; j < gridCells.rows; j++)
            {
                CellSpring _tmpCell = Instantiate(cell, new Vector3(i - (cols - 1f) / 2f, 0, j - (rows - 1f) / 2f), Quaternion.identity, transform);
                gridCells[i, j] = _tmpCell;
                _spownedObjects.Add(_tmpCell.gameObject);
                int _index = matrix[i, j];
                int colorIndex = _index % 100;
                int spriteIndex = _index / 100;
                _tmpCell.ColorIndex.Add(colorIndex);
                _tmpCell.springindex = spriteIndex;
                if (spriteIndex > 0)
                    SpownBlocks(_tmpCell.transform.position, spriteIndex, ref _tmpCell);
                //if (spriteIndex != 0) CheckClells(_tmpCell, colorIndex, spriteIndex);
                //_tmpCell.SetHight();

            }
        }

        #region Spown Springs

        //for (int i = 0; i < springPoints.Count; i++)
        //{
        //    if (springPoints[i].Count > 1)
        //    {
        //        Vector3 pos1 = springPoints[i][0].transform.position;
        //        Vector3 pos2 = springPoints[i][1].transform.position;
        //        SpringConntroller springConntroller = Instantiate(_springController, (pos2 + pos2) / 2, Quaternion.identity, transform);
        //        springConntroller.startpos1 = pos1;
        //        springConntroller.startpos2 = pos2;
        //        _spownedObjects.Add(springConntroller.gameObject);
        //        springConntroller.SpownSlinky(pos1, pos2, springPoints[i][0].hight, springPoints[i][0].springindex);
        //        springConntroller.hightSpring = springPoints[i][0].hight;
        //        activeSprings.Add(springConntroller);
        //        springConntroller.springPointsindex = i;

        //    }
        //}

        #endregion

        #region Spown Board
        //for (int i = 0; i < cellSprings.Length; i++)
        //{
        //    CellSpring _tmpCell = Instantiate(cell, new Vector3((i - boardCount / 2f) + 0.5f, 0, (-rows / 1.5f) - 0.5f), Quaternion.identity, transform);
        //    _spownedObjects.Add(_tmpCell.gameObject);
        //    cellSprings[i] = _tmpCell;
        //}

        #endregion
        yield return null;
    }

    ////void CheckClells(CellSpring currentCell, int colorIndex, int spriteIndex)
    //{
    //    if (springPoints.Count <= 0)
    //    {
    //        springPoints.Add(new List<CellSpring>());
    //        springPoints[0].Add(currentCell);
    //        return;
    //    }

    //    foreach (var cell in springPoints)
    //    {
    //        if (cell.Count > 0 && cell.Count < 2)
    //        {
    //            if (cell[0].ColorIndex == colorIndex && cell[0].springindex == spriteIndex)
    //            {
    //                cell.Add(currentCell);
    //                return;
    //            }
    //        }
    //    }
    //    springPoints.Add(new List<CellSpring>());
    //    springPoints[springPoints.Count - 1].Add(currentCell);
    //}
    void SetCorners()
    {
        gridCells[0, 0].isCorner = true;
        gridCells[0, rows - 1].isCorner = true;
        gridCells[cols - 1, 0].isCorner = true;
        gridCells[cols - 1, rows - 1].isCorner = true;
    }
}


