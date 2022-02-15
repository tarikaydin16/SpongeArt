using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class VertexEditing : MonoBehaviour
{

    public Vector3[] vertices;
    public Vector3[] normals;
    public List<VertexObject> vertexObjectList = new List<VertexObject>();
    public List<VertexObject> selectedVertices = new List<VertexObject>();

    public List<Vector3> startVertices = new List<Vector3>(); // step 1
    public List<Vector3> oneStepVertices = new List<Vector3>();// step 2
    public List<Vector3> twoStepVertices = new List<Vector3>();// step 3
    public List<Vector3> threeStepVertices = new List<Vector3>();// step 4


    public Transform[] raycastTransforms;
    public float[] zTestLevels;

    public AnimationCurve analogIntensityCurve;
    public int smoothPoint;
    MeshFilter mf;
    MeshRenderer mr;
    Camera _camera;
    float d = 0.2f;
    [SerializeField]
    float checkDistance;
    [SerializeField] Transform lastikTransform;

    GameManager gameManager;
    private void OnEnable()
    {
        EventManager.GetInstance().OnGameStart += OnGameStarted;
        EventManager.GetInstance().OnPrepareLevel += PrepareLevel;
        EventManager.GetInstance().OnUndo += Undo;
        EventManager.GetInstance().OnMakeMove += MakeMove;
    }

    private void OnDisable()
    {
        if (EventManager.GetInstance() == null) return;
        EventManager.GetInstance().OnGameStart -= OnGameStarted;
        EventManager.GetInstance().OnPrepareLevel -= PrepareLevel;
        EventManager.GetInstance().OnUndo -= Undo;
        EventManager.GetInstance().OnMakeMove -= MakeMove;

    }


    void Start()
    {
        mf = GetComponent<MeshFilter>();
        mr = GetComponent<MeshRenderer>();
        vertices = mf.mesh.vertices;
        normals = mf.mesh.normals;

        for (int i = 0; i < vertices.Length; i++)
        {
            GameObject vertexObject = new GameObject(i.ToString());

            vertexObject.transform.SetParent(transform);
            vertexObject.transform.position = vertices[i];
            VertexObject _vertexObject = vertexObject.AddComponent<VertexObject>();
            _vertexObject.vertexNormal = normals[i];
            vertexObjectList.Add(_vertexObject);
            _vertexObject.vertexNumber = i;

            startVertices.Add(vertices[i]);

        }


        gameManager = GameManager.GetInstance();

        _camera = Camera.main;



    }

    private void Update()
    {

        mf.mesh.vertices = vertices;
        mf.mesh.RecalculateNormals();
        for (int i = 0; i < selectedVertices.Count; i++)
        {

            selectedVertices[i].OnUnselected();
        }
        selectedVertices.Clear();



        for (int i = 0; i < vertexObjectList.Count; i++)
        {
            float dst = DistanceLineSegmentPoint(raycastTransforms[0].transform.position, raycastTransforms[1].transform.position, vertexObjectList[i].transform.position);
            if (dst < checkDistance)//&& dst > -0.02f
            {

                if (!selectedVertices.Contains(vertexObjectList[i]))
                    selectedVertices.Add(vertexObjectList[i]);
                vertexObjectList[i].OnSelected();


            }

        }

    }
    public void ToSqueeze(int sequezeControl, Vector3 centerPos)
    {
        for (int a = 0; a < sequezeControl; a++)
        {


            for (int i = 0; i < selectedVertices.Count; i++)
            {
                Vector3 dir = selectedVertices[i].transform.position - centerPos;
                dir.z = 0;
                //if (dir.magnitude < 0.1f) dir = Vector3.zero;
                //-normals[selectedVertices[i].vertexNumber]
                selectedVertices[i].transform.position = new Vector3(selectedVertices[i].transform.position.x, selectedVertices[i].transform.position.y, -0.08f);

                selectedVertices[i].transform.position += -dir.normalized * Time.deltaTime * analogIntensityCurve.Evaluate(1f) * d * 2;
                for (int j = 0; j < selectedVertices[i].firstnearVertices.Count; j++)
                {
                    selectedVertices[i].firstnearVertices[j].transform.position += -dir.normalized * Time.deltaTime * analogIntensityCurve.Evaluate(.7f) * d;

                }
                for (int j = 0; j < selectedVertices[i].secondnearVertices.Count; j++)
                {
                    selectedVertices[i].secondnearVertices[j].transform.position += -dir.normalized * Time.deltaTime * analogIntensityCurve.Evaluate(.4f) * d;

                }
                for (int j = 0; j < selectedVertices[i].thridnearVertices.Count; j++)
                {
                    selectedVertices[i].thridnearVertices[j].transform.position += -dir.normalized * Time.deltaTime * analogIntensityCurve.Evaluate(.3f) * d;

                }
            }


        }




    }

    private void PrepareLevel()
    {
    }

    private void OnGameStarted()
    {
    }

    private void Undo()
    {
        //if (gameManager.GameStep == 0) return;
        if (gameManager.GameStep == 0)
        {
            for (int i = 0; i < vertexObjectList.Count; i++)
            {
                vertexObjectList[i].transform.position = startVertices[i];
            }
        }
        if (gameManager.GameStep == 1)
        {
            for (int i = 0; i < vertexObjectList.Count; i++)
            {
                vertexObjectList[i].transform.position = oneStepVertices[i];
            }
            oneStepVertices.Clear();
        }
        if (gameManager.GameStep == 2)
        {
            for (int i = 0; i < vertexObjectList.Count; i++)
            {
                vertexObjectList[i].transform.position = twoStepVertices[i];
            }
            twoStepVertices.Clear();

        }
        if (gameManager.GameStep == 3)
        {
            for (int i = 0; i < vertexObjectList.Count; i++)
            {
                vertexObjectList[i].transform.position = threeStepVertices[i];
            }
            threeStepVertices.Clear();

        }
    }
    void MakeMove()
    {

        //if (gameManager.GameStep == 0)
        //{
        //    for (int i = 0; i < vertexObjectList.Count; i++)
        //    {
        //        oneStepVertices.Add(vertexObjectList[i].transform.position);
        //    }
        //}
        if (gameManager.GameStep == 1)
        {
            for (int i = 0; i < vertexObjectList.Count; i++)
            {
                oneStepVertices.Add(vertexObjectList[i].transform.position);
            }
        }
        if (gameManager.GameStep == 2)
        {
            for (int i = 0; i < vertexObjectList.Count; i++)
            {
                twoStepVertices.Add(vertexObjectList[i].transform.position);
            }
        }
        if (gameManager.GameStep == 3)
        {
            for (int i = 0; i < vertexObjectList.Count; i++)
            {
                threeStepVertices.Add(vertexObjectList[i].transform.position);
            }
        }


        //print(FindLeftSelectedVerticies());
        //print(FindRightSelectedVerticies());
        //print(FindUpSelectedVerticies());
    }
    float DistanceLineSegmentPoint(Vector3 a, Vector3 b, Vector3 p)
    {

        p = new Vector3(p.x, p.y, a.z);
        if (a == b)
            return Vector3.Distance(a, p);

        Vector3 ba = b - a;
        Vector3 pa = a - p;
        return (pa - ba * (Vector3.Dot(pa, ba) / Vector3.Dot(ba, ba))).magnitude;
    }

    public Vector3 FindLeftSelectedVerticies()
    {
        float mostSmallX = 99f;
        Vector3 pos = Vector3.zero;
        VertexObject obj = null;
        for (int i = 0; i < selectedVertices.Count; i++)
        {
            if (mostSmallX >= selectedVertices[i].transform.position.x && selectedVertices[i].transform.position.z < 0)
            {
                mostSmallX = selectedVertices[i].transform.position.x;
                pos = selectedVertices[i].transform.position;
                obj = selectedVertices[i];

            }

        }
        //print("FindLeftSelectedVerticies " + obj.vertexNumber);
        return pos;
    }

    public Vector3 FindRightSelectedVerticies()
    {
        float mostSmallX = -99f;
        Vector3 pos = Vector3.zero;
        VertexObject obj = null;


        for (int i = 0; i < selectedVertices.Count; i++)
        {
            if (mostSmallX <= selectedVertices[i].transform.position.x && selectedVertices[i].transform.position.z < 0)
            {
                mostSmallX = selectedVertices[i].transform.position.x;
                pos = selectedVertices[i].transform.position;
                obj = selectedVertices[i];
            }

        }
        // print("FindRightSelectedVerticies " + obj.vertexNumber);

        return pos;
    }
    public Vector3 FindUpSelectedVerticies()
    {
        float mostSmallY = -99f;
        Vector3 pos = Vector3.zero;
        VertexObject obj = null;


        for (int i = 0; i < selectedVertices.Count; i++)
        {
            if (mostSmallY <= selectedVertices[i].transform.position.y && selectedVertices[i].transform.position.z < 0)
            {
                mostSmallY = selectedVertices[i].transform.position.y;
                pos = selectedVertices[i].transform.position;
                obj = selectedVertices[i];

            }

        }
        // print("FindRightSelectedVerticies " + obj.vertexNumber);

        return pos;
    }
    public Vector3 FindDownSelectedVerticies()
    {
        float mostSmallY = 99f;
        Vector3 pos = Vector3.zero;
        VertexObject obj = null;


        for (int i = 0; i < selectedVertices.Count; i++)
        {
            if (mostSmallY >= selectedVertices[i].transform.position.y && selectedVertices[i].transform.position.z < 0)
            {
                mostSmallY = selectedVertices[i].transform.position.y;
                pos = selectedVertices[i].transform.position;
                obj = selectedVertices[i];

            }

        }
        // print("FindRightSelectedVerticies " + obj.vertexNumber);

        return pos;
    }
}

