using System.Collections;
using UnityEngine;
using Dreamteck.Splines;
using System;

public class SpringConntroller : MonoBehaviour
{
    SplineComputer spline;
    [SerializeField]
    SplineComputer _splinePrefab;
    [SerializeField]
    AnimationCurve _motionCurve;
    [SerializeField]
    AnimationCurve _motionCurve1;
    [SerializeField]
    MaterialsIndex[] _springMaterial;
    [SerializeField]
    MeshRenderer _meshRenderer;
    [HideInInspector]
    public Vector3 _startPos;
    [HideInInspector]
    public Vector3 _endPos;
    public int currentClorIndex = 0;

    public Detector detector;

    public float hightSpring = 0;

    public int springPointsindex = -1;

    public bool canMove = true;


    public Vector3 startpos1;
    public Vector3 startpos2;
    public Vector3 posMidel;

    Vector3 mtan1;
    Vector3 mtan2;

    Vector3 rtan1;
    Vector3 rtan2;

    Vector3 ltan1;
    Vector3 ltan2;


    Vector3 dirdetector;
    Vector3 scaledetector;
    Vector3 posdetector;

    public void SpownSlinky(Vector3 pos1, Vector3 pos2, float hight, int indexColor)
    {
        //heightIndex = hight;
        _startPos = pos1 + Vector3.up * 0.45f;
        _endPos = pos2 + Vector3.up * 0.45f;
        spline = _splinePrefab;
        spline.SetPointPosition(0, _startPos);
        spline.SetPointPosition(2, _endPos);

        Vector3 posMid = (_startPos + _endPos) * 0.5f;
        posMid.y = hight;
        spline.SetPointPosition(1, posMid);
        detector.transform.position = posMid;

        float distance = Vector3.Distance(_startPos, _endPos);
        Vector3 detectorScale = detector.transform.localScale;
        detector.transform.localScale = new Vector3(distance, detectorScale.y, detectorScale.z);

        Vector3 dirToStart = (_endPos - _startPos).normalized;
        detector.transform.right = dirToStart;
        Vector3 tan1 = (_startPos + posMid) * 0.5f;
        tan1.y = posMid.y;

        Vector3 tan2 = (_endPos + posMid) * 0.5f;
        tan2.y = posMid.y;

        spline.SetPointTangents(1, tan1, tan2);

        posMidel = posMid;
        dirdetector = dirToStart;
        scaledetector = detector.transform.localScale;
        posdetector = detector.transform.position;

        mtan1 = tan1;
        mtan2 = tan2;

        rtan1 = spline.GetPointTangent(0);
        rtan2 = spline.GetPointTangent2(0);

        ltan1 = spline.GetPointTangent(2);
        ltan2 = spline.GetPointTangent2(2);
        GetColor(indexColor);
    }

    void GetColor(int index)
    {
        if (index == 1 || index == 6 || index == 11 || index == 16)
        {
            // green
            Material mat = new Material(_springMaterial[0].mat);
            _meshRenderer.material = mat;
            currentClorIndex = 0;
        }
        else if (index == 2 || index == 7 || index == 12 || index == 17)
        {
            // bleue
            Material mat = new Material(_springMaterial[1].mat);
            _meshRenderer.material = mat;
            currentClorIndex = 1;
        }
        else if (index == 3 || index == 8 || index == 13 || index == 18)
        {
            // Orange
            Material mat = new Material(_springMaterial[2].mat);
            _meshRenderer.material = mat;
            currentClorIndex = 2;
        }
        else if (index == 4 || index == 9 || index == 14 || index == 19)
        {
            // yelow
            Material mat = new Material(_springMaterial[3].mat);
            _meshRenderer.material = mat;
            currentClorIndex = 3;
        }
        else if (index == 5 || index == 10 || index == 15 || index == 20)
        {
            // purple
            Material mat = new Material(_springMaterial[4].mat);
            _meshRenderer.material = mat;
            currentClorIndex = 4;
        }
    }

    public void StartMove(Vector3 startPos, Vector3 Target, float duration)
    {
        if (spline == null) return;
        StartCoroutine(MoveNode(startPos, Target, duration, spline));
    }

    IEnumerator MoveNode(Vector3 startPos, Vector3 endPos, float duration, SplineComputer spline)
    {
        float time = 0f;
        canMove = false;

        SplinePoint point0 = spline.GetPoint(0);
        SplinePoint point1 = spline.GetPoint(1);
        SplinePoint point2 = spline.GetPoint(2);
        spline.SetPointTangents(0, point0.position, point0.position);
        spline.SetPointTangents(2, point2.position, point2.position);
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            Vector3 pos1 = spline.GetPoint(0).position;
            float curveValue = _motionCurve.Evaluate(t);
            Vector3 newPos = Vector3.Lerp(startPos, endPos, t);
            newPos.y += curveValue;

            spline.SetPointPosition(2, newPos);
            UpdateMiddlePointAndTangents(spline, 1, pos1, newPos, 1.5f);

            yield return null;
        }

        time = 0f;
        startPos = spline.GetPoint(0).position + Vector3.up * 0.45f;
        endPos.y = 1f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            float curveValue = _motionCurve1.Evaluate(t);
            Vector3 newPos = Vector3.Lerp(startPos, endPos, t);
            newPos.y += curveValue;

            Vector3 pos2 = spline.GetPoint(2).position;
            spline.SetPointPosition(0, newPos);

            float middleY = Mathf.Lerp(1.5f, 1f, t);
            UpdateMiddlePointAndTangents(spline, 1, newPos, pos2, middleY);

            if (detector != null)
            {
                detector.transform.position = spline.GetPoint(1).position;
            }

            yield return null;
        }

        if (detector != null)
        {
            detector.transform.localScale = new Vector3(0.7f, 0.8f, 0.6f);
        }
    }

    void UpdateMiddlePointAndTangents(SplineComputer spline, int midIndex, Vector3 pos1, Vector3 pos2, float middleY)
    {
        Vector3 middle = (pos1 + pos2) * 0.5f;
        middle.y = middleY;
        spline.SetPointPosition(midIndex, middle);

        Vector3 tan1 = (pos1 + middle) * 0.5f;
        tan1.y = middle.y;
        Vector3 tan2 = (middle + pos2) * 0.5f;
        tan2.y = middle.y;

        spline.SetPointTangents(midIndex, tan1, tan2);
    }

    public Vector3 GetPointByIbdex(int index)
    {
        return spline.GetPoint(index).position;
    }

    public void ResetPosSpring(float duration)
    {
        StartCoroutine(ResetPos(duration));

    }

    IEnumerator ResetPos(float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            Vector3 pos1 = spline.GetPoint(0).position;
            Vector3 pos2 = spline.GetPoint(2).position;
            Vector3 pos3 = spline.GetPoint(1).position;

            float curveValue = _motionCurve.Evaluate(t);

            Vector3 newPos1 = Vector3.Lerp(pos1, _startPos, t);
            Vector3 newPos2 = Vector3.Lerp(pos2, _endPos, t);
            Vector3 newPos3 = Vector3.Lerp(pos3, posMidel, t);

            newPos1.y += curveValue;
            newPos2.y += curveValue;

            spline.SetPointPosition(0, newPos1);
            spline.SetPointPosition(1, newPos3);
            spline.SetPointPosition(2, newPos2);
           
            yield return null;
        }
        spline.SetPointTangents(0, rtan1, rtan2);
        spline.SetPointTangents(1, mtan1, mtan2);
        spline.SetPointTangents(2, ltan1, ltan2);
        detector.transform.position = posdetector;
        detector.transform.right = dirdetector;
        detector.transform.localScale = scaledetector;
        canMove = true;
    }
}

[Serializable]
public struct MaterialsIndex
{
    public string name;
    public Material mat;
    public int index;
}