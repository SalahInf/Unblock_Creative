using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;
using UnityEngine;


public class Controller : MonoBehaviour
{
    [SerializeField]
    LayerMask _layer;
    [SerializeField]
    GameObject _effectVanish;
    Camera _mainCam => Camera.main;
    RaycastHit mouseHit;
    int _countBoard = 0;
    List<SpringConntroller> _springConntrollers => GridSpowner.instance.springConntrollers;

    bool isAnimating = false;
    private void Update()
    {
        //if (Root.GameManager.gameStart)
        //    Fetch();


        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    RestPosLastSpring();
        //}
    }
    void Fetch()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        if (!Physics.Raycast(_mainCam.ScreenPointToRay(Input.mousePosition), out mouseHit, 100f, _layer))
            return;

        GridSpowner spowner = GridSpowner.instance;

        if (_countBoard >= spowner.cellSprings.Length)
        {
            if (Root.GameManager.gameStart && !isAnimating)
            {
                _countBoard = 0;
                Root.GameManager.LoseGame();
                Debug.Log("Lose");
            }
            return;
        }

        SpringConntroller sp = mouseHit.collider.GetComponentInParent<SpringConntroller>();
        if (sp == null || !sp.canMove)
            return;

        foreach (var detector in sp.detector.springController)
        {
            if (detector.hightSpring > sp.hightSpring)
            {
                sp.transform.DOShakeRotation(0.2f, 3, 3, 90);
                return;
            }
        }

        Vector3 postarget = spowner.cellSprings[_countBoard].transform.position + Vector3.up * 0.45f;
        sp.StartMove(sp._endPos, postarget, 0.25f);

        spowner.springConntrollers.Add(sp);
        spowner.activeSprings.Remove(sp);
        _countBoard++;

        StartCoroutine(CheckList(spowner));
    }
    List<SpringConntroller> CheckListIndex(GridSpowner grid)
    {
        Dictionary<int, List<SpringConntroller>> grouped = new Dictionary<int, List<SpringConntroller>>();

        foreach (var controller in grid.springConntrollers)
        {
            int index = controller.currentClorIndex;

            if (!grouped.ContainsKey(index))
                grouped[index] = new List<SpringConntroller>();

            grouped[index].Add(controller);

            if (grouped[index].Count == 3)
                return grouped[index];
        }


        return null;
    }
    IEnumerator CheckList(GridSpowner grid)
    {
        while (isAnimating)
            yield return null;

        var matchedList = CheckListIndex(grid);
        if (matchedList == null)
            yield break;

        isAnimating = true;
        _countBoard -= 3;

        Vector3 matchTargetPos = matchedList[0].GetPointByIbdex(0);

        for (int i = 1; i < matchedList.Count; i++)
        {
            var match = matchedList[i];
            Vector3 start = match.GetPointByIbdex(0);
            match.StartMove(start, matchTargetPos, 0.12f);
            yield return new WaitForSeconds(0.35f);
        }
        int index = 0;
        foreach (var match in matchedList)
        {
            index++;
            if (index == 3)
            {
                Vector3 effectPos = match.GetPointByIbdex(0);
                GameObject g = Instantiate(_effectVanish, effectPos, Quaternion.identity);
                Destroy(g, 0.5f);
            }

            _springConntrollers.Remove(match);
            Destroy(match.gameObject);
        }

        if (GridSpowner.instance.activeSprings.Count <= 0 && Root.GameManager.gameStart)
        {
            Debug.Log("win");
            Root.GameManager.GameWin();
        }
        yield return new WaitForSeconds(0.15f);
        if (_springConntrollers.Count > 0)
        {
            var reordered = _springConntrollers.OrderBy(obj => obj.currentClorIndex).ToList();
            GridSpowner.instance.springConntrollers = reordered;

            for (int i = 0; i < _springConntrollers.Count; i++)
            {
                Vector3 targetPos = grid.cellSprings[i].transform.position + Vector3.up * 0.45f;
                Vector3 currentPos = _springConntrollers[i].GetPointByIbdex(0);

                _springConntrollers[i].StartMove(currentPos, targetPos, 0.12f);
                yield return new WaitForSeconds(0.35f);
            }
        }

        isAnimating = false;
        yield return null;
    }
    public void RestPosLastSpring()
    {
        if (_countBoard <= 0)
            return;

        SpringConntroller sp = _springConntrollers[^1];
        sp.ResetPosSpring(0.5f);
        GridSpowner.instance.springConntrollers.Remove(sp);
        GridSpowner.instance.activeSprings.Add(sp);
        _countBoard--;

    }
}
