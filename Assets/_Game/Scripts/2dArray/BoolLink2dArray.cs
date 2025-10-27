using MatrixAlgebra;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoolLink2dArray : Bool2dArray
{
    public BoolLink2dArray.linkBool[] links;

    public BoolLink2dArray(int width, int height) : base(width, height) 
    {
        links = new linkBool[0];
    }

    public BoolLink2dArray(int width, int height, bool[] elemnts) : base(width, height, elemnts) 
    {
        links = new linkBool[0];
    }

    public BoolLink2dArray(BoolLink2dArray boolLink2D) : base(boolLink2D) 
    {
        links = new linkBool[boolLink2D.links.Length];
        Array.Copy(boolLink2D.links, links, links.Length);
    }

    public void ClearAllConnection(int index1i, int index1j)
    {
        ClearAllConnection(index1j * cols + index1i);
    }

    public void ClearAllConnection(int index1)
    {
        for (int i = 0; i < links.Length; i++)
        {
            int in1 = links[i].point1;
            int in2 = links[i].point2;
            if (in1 == index1 || in2 == index1)
            {
                RemoveConnection(in1, in2);
                i--;
            }
        }
    }

    public void ClearAndReConnection(int index1i, int index1j)
    {
        ClearAndReConnection(index1j * cols + index1i);
    }

    public void ClearAndReConnection(int index1)
    {
        int link1 = -1;
        int link2 = -1;
        for (int i = 0; i < links.Length; i++)
        {
            int in1 = links[i].point1;
            int in2 = links[i].point2;
            if (in1 == index1 || in2 == index1)
            {
                if (link1 == -1)
                {
                    link1 = i;
                }
                else if (link2 == -1)
                {
                    link2 = i;

                    Reconnect(link1, link2);

                    RemoveConnection(link1);
                    RemoveConnection(link2);
                    i -= 2;
                    link1 = -1;
                    link2 = -1;
                }
            }
        }

        if(link1 != -1)
        {
            RemoveConnection(link1);
        }

        void Reconnect(int indexLink1,int indexLink2)
        {
            int point11 = (links[indexLink1].point1 == index1)? links[indexLink1].point2: links[indexLink1].point1;
            int point22 = (links[indexLink2].point1 == index1) ? links[indexLink2].point2 : links[indexLink2].point1;
            AddConnection(point11, point22);
        }
    }

    public void AddConnection(int index1i, int index1j, int index2i, int index2j)
    {
        AddConnection(index1j * cols + index1i, index2j * cols + index2i);
    }

    public void AddConnection(int index1,int index2)
    {
		int countIndex = links.Length;
		int newIndex1 = Mathf.Min(index1, index2);
		int newIndex2 = Mathf.Max(index1, index2);

        if (newIndex1 == newIndex2 || CountianLink(newIndex1, newIndex2))
        {
            return;
        }

        Array.Resize<linkBool>(ref links, countIndex + 1);

        linkBool link = new linkBool();
        link.point1 = newIndex1;
		link.point2 = newIndex2;

        links[countIndex] = link;
    }

    public void RemoveConnection(int index1i, int index1j, int index2i, int index2j)
    {
        RemoveConnection(index1j * cols + index1i, index2j * cols + index2i);
    }

    public void RemoveConnection( int index1, int index2)
    {
        int newIndex1 = Mathf.Min(index1, index2);
        int newIndex2 = Mathf.Max(index1, index2);

        int indexOflink = indexOfLink(newIndex1, newIndex2);
        RemoveConnection(indexOflink);
    }

    public void RemoveConnection(int linkIndex)
    {
        if (linkIndex != -1)
        {
            links[linkIndex] = links[links.Length - 1];
            Array.Resize<linkBool>(ref links, links.Length - 1);
            return;
        }
    }

    public void MoveLinkFrom(int fromIndexI, int fromIndexJ, int toIndexI, int toIndexJ)
    {
        MoveLinkFrom( fromIndexJ * cols + fromIndexI, toIndexJ * cols + toIndexI);
    }

    public void MoveLinkFrom(int fromIndex, int toIndex)
    {
        if (fromIndex != toIndex)
        {
            for (int i = 0; i < links.Length; i++)
            {
                int newlink1 = -1;
                int newlink2 = -1;

                if (links[i].point1 == fromIndex)
                {
                    newlink1 = Mathf.Min(toIndex, links[i].point2);
                    newlink2 = Mathf.Max(toIndex, links[i].point2);
                }
                else if (links[i].point2 == fromIndex)
                {
                    newlink1 = Mathf.Min(toIndex, links[i].point1);
                    newlink2 = Mathf.Max(toIndex, links[i].point1);
                }

                if (newlink1 != -1)
                {
                    RemoveConnection(i);
                    i--;
                    if (newlink1 != newlink2)
                        AddConnection(newlink1, newlink2);
                }
            }
        }
    }

    public  bool CountianLink(int index1, int index2)
    {
        return indexOfLink(index1,index2) != -1;
    }

    public int indexOfLink(int index1, int index2)
    {
        int countIndex = links.Length;

        for (int i = 0; i < countIndex; i++)
        {
            int in1 = links[i].point1;
            int in2 = links[i].point2;
            if (in1 == index1 && in2 == index2)
            {
                return i;
            }
        }
        return -1;
    }

    [System.Serializable]
    public struct linkBool
    {
        public int point1;
        public int point2;
    }
}
