using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using PathCreation.Examples;
using DG.Tweening;

public class LineController : MonoBehaviour
{
    Vector3 worldPosition;
    LineRenderer line;
    bool isDragging;
    Transform firstObject, secondObject;
    [SerializeField] VertexEditing vertexEditing;
    [SerializeField] PathCreator pathCreator;
    [SerializeField] RoadMeshCreator roadMeshCreator;

    [SerializeField] float rubberDepth;
    [SerializeField] float rubberOffsetZ;
    [SerializeField] GameObject rubberMesh;
    [SerializeField] List<GameObject> rubberObjects = new List<GameObject>();
    GameManager gameManager;

    [SerializeField] public Transform[] wayPoints;
    private void OnEnable()
    {
        EventManager.GetInstance().OnUndo += Undo;
    }
    private void OnDisable()
    {
        if (EventManager.GetInstance() == null) return;
        EventManager.GetInstance().OnUndo -= Undo;

    }

    void Start()
    {
        line = GetComponent<LineRenderer>();
        firstObject = transform.GetChild(0);
        secondObject = transform.GetChild(1);
        gameManager = GameManager.GetInstance();
    }

    void Update()
    {

        if (gameManager.GameStep < gameManager.MaxGameStep)
            if (InputWrapper.Input.touchCount > 0)
            {
                line.enabled = true;
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = -Camera.main.transform.position.z - 0.2f;
                worldPosition = Camera.main.ScreenToWorldPoint(mousePos);
                if (InputWrapper.Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    line.SetPosition(0, worldPosition);
                    firstObject.position = worldPosition;
                }

                line.SetPosition(1, worldPosition);
                secondObject.position = worldPosition;
                isDragging = true;

            }
            else if (isDragging)
            {
                if (GetLength(firstObject.transform.position, secondObject.transform.position) > 1f)
                {
                    rubberMesh.SetActive(true);

                    float angle = Vector3.Angle(secondObject.transform.position - firstObject.transform.position, Vector3.right);
                    Vector3 firstPos = Vector3.zero;
                    Vector3 secondPos = Vector3.zero;

                    if (Mathf.Abs(angle - 90) > 10)
                    {
                        firstPos = vertexEditing.FindLeftSelectedVerticies();
                        secondPos = vertexEditing.FindRightSelectedVerticies();
                    }
                    else
                    {
                        firstPos = vertexEditing.FindUpSelectedVerticies();
                        secondPos = vertexEditing.FindDownSelectedVerticies();
                    }
                    Vector3 dir = secondPos - firstPos;
                    Vector3 newFirstPos = Vector3.zero, newSecondPos = Vector3.zero;
                    newFirstPos = firstPos - dir.normalized * 1.5f;
                    newSecondPos = secondPos + dir.normalized * 1.5f;

                    Vector3 amount = dir.normalized * 1.5f;

                    Vector3 doVec = newFirstPos;
                    float time = 0;
                    bool ifSeq = false;

                    BezierPath bezierPath = new BezierPath(wayPoints, true, PathSpace.xyz);
                    pathCreator.bezierPath = bezierPath;

                    DOTween.To(() => doVec, x => doVec = x, firstPos + dir.normalized * .2f, 0.5f).OnUpdate(() =>
                         {
                             time += Time.deltaTime;
                             if (time > .3f && !ifSeq)
                             {
                                 vertexEditing.ToSqueeze(20, CenterPos(firstObject.transform.position, secondObject.transform.position));
                                 ifSeq = true;
                             }


                             pathCreator.bezierPath.SetPoint(3, doVec + Vector3.forward * rubberOffsetZ);
                             pathCreator.bezierPath.SetPoint(2, doVec + Vector3.forward * rubberDepth + Vector3.forward * rubberOffsetZ);
                             pathCreator.bezierPath.SetPoint(4, doVec + Vector3.back * rubberDepth + Vector3.forward * rubberOffsetZ);
                             roadMeshCreator.PathUpdate();


                         }).OnComplete(() =>
                         {
                             GameObject rubber = Instantiate(rubberMesh);
                             MeshFilter filter = rubber.GetComponent<MeshFilter>();
                             Mesh mesh = new Mesh();
                             mesh.vertices = filter.mesh.vertices;
                             mesh.colors = filter.mesh.colors;
                             mesh.triangles = filter.mesh.triangles;
                             mesh.uv = filter.mesh.uv;
                             mesh.normals = filter.mesh.normals;
                             filter.mesh = mesh;


                             rubber.transform.SetParent(null);

                             rubberObjects.Add(rubber);
                             rubberMesh.SetActive(false);
                             gameManager.CheckFinal();
                         });
                    Vector3 doVec2 = newSecondPos;
                    DOTween.To(() => doVec2, x => doVec2 = x, secondPos - dir.normalized * .2f, 0.5f).OnUpdate(() =>
                    {
                        pathCreator.bezierPath.SetPoint(0, doVec2 + Vector3.forward * rubberOffsetZ);
                        pathCreator.bezierPath.SetPoint(1, doVec2 + Vector3.forward * rubberDepth + Vector3.forward * rubberOffsetZ);
                        pathCreator.bezierPath.SetPoint(5, doVec2 + Vector3.back * rubberDepth + Vector3.forward * rubberOffsetZ);
                        roadMeshCreator.PathUpdate();

                    });


                    MakeMove();
                    bool check = false;
                    print("aci: " + Mathf.Abs(angle - 90) + " firstObject.localPosition.x: " + firstObject.localPosition.x);

                    if (firstObject.localPosition.x > -.165f && firstObject.localPosition.x < .07f && (Mathf.Abs(angle - 180) < 10 || Mathf.Abs(angle) < 10))
                    {
                        EventManager.GetInstance().SuccessStep(gameManager.GameStep);
                        print("Succ 1");
                        check = true;
                    }
                    else if ((firstObject.localPosition.x < -.165f || firstObject.localPosition.x > .07f) && (Mathf.Abs(angle - 180) < 10 || Mathf.Abs(angle) < 10))
                    {
                        EventManager.GetInstance().FailStep(gameManager.GameStep);
                        print("Fail 1");
                        check = true;
                    }
                    if (firstObject.localPosition.y > -.12f && firstObject.localPosition.y < .12f && Mathf.Abs(angle - 90) < 10 && !check)
                    {
                        EventManager.GetInstance().SuccessStep(gameManager.GameStep);
                        print("Succ 2");
                        check = true;

                    }
                    else if ((firstObject.localPosition.y < -.12f || firstObject.localPosition.y > .12f) && Mathf.Abs(angle - 90) < 10 && !check)
                    {
                        EventManager.GetInstance().FailStep(gameManager.GameStep);
                        print("Fail 2");
                        check = true;

                    }
                }
                isDragging = false;

                line.enabled = false;

            }

    }

    Vector3 CenterPos(Vector3 pos1, Vector3 pos2)
    {
        return (pos1 + pos2) / 2;
    }

    float GetLength(Vector3 pos1, Vector3 pos2)
    {
        return Vector3.Distance(pos1, pos2);
    }
    void MakeMove()
    {
        EventManager.GetInstance().MakeMove();
    }
    void Undo()
    {
        if (gameManager.GameStep == 0) return;
        Destroy(rubberObjects[gameManager.GameStep - 1]);
        rubberObjects.RemoveAt(gameManager.GameStep - 1);

    }
}
