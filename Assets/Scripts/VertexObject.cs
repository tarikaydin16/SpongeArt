using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexObject : MonoBehaviour
{
    // Start is called before the first frame update
    public int vertexNumber;
    public Vector3 vertexNormal;
    public List<Transform> firstnearVertices = new List<Transform>();
    public List<Transform> secondnearVertices = new List<Transform>();
    public List<Transform> thridnearVertices = new List<Transform>();

    VertexEditing vertexEditing;
    Color gizmosColor = Color.black;
    bool selected, findNearIsSucc;

    private void Start()
    {
        vertexEditing = transform.parent.gameObject.GetComponent<VertexEditing>();


    }
    private void Update()
    {
        vertexEditing.vertices[vertexNumber] = transform.position;
        FindNear();
    }
    void FindNear()
    {
        if (!selected || findNearIsSucc) return;

        for (int i = 0; i < vertexEditing.vertices.Length; i++)
        {
            if (Mathf.Abs(vertexNormal.y) == 1)
            {
                if (transform.position.y == vertexEditing.vertices[i].y && CompareFloats(Mathf.Abs(transform.position.x - vertexEditing.vertices[i].x), 0.03683151f * 2f) && transform.position.z == vertexEditing.vertices[i].z)
                {
                    if (!secondnearVertices.Contains(vertexEditing.transform.GetChild(i)))
                        secondnearVertices.Add(vertexEditing.transform.GetChild(i));

                }
                if (transform.position.y == vertexEditing.vertices[i].y && CompareFloats(Mathf.Abs(transform.position.x - vertexEditing.vertices[i].x), 0.03683151f) && transform.position.z == vertexEditing.vertices[i].z)
                {
                    if (!firstnearVertices.Contains(vertexEditing.transform.GetChild(i)))
                        firstnearVertices.Add(vertexEditing.transform.GetChild(i));

                }
                if (transform.position.y == vertexEditing.vertices[i].y && CompareFloats(Mathf.Abs(transform.position.x - vertexEditing.vertices[i].x), 0.03683151f * 3f) && transform.position.z == vertexEditing.vertices[i].z)
                {
                    if (!thridnearVertices.Contains(vertexEditing.transform.GetChild(i)))
                        thridnearVertices.Add(vertexEditing.transform.GetChild(i));

                }
            }
            if (Mathf.Abs(vertexNormal.x) == 1)
            {
                if (transform.position.x == vertexEditing.vertices[i].x && CompareFloats(Mathf.Abs(transform.position.y - vertexEditing.vertices[i].y), 0.03683151f * 2f) && transform.position.z == vertexEditing.vertices[i].z)
                {
                    if (!secondnearVertices.Contains(vertexEditing.transform.GetChild(i)))
                        secondnearVertices.Add(vertexEditing.transform.GetChild(i));

                }
                if (transform.position.x == vertexEditing.vertices[i].x && CompareFloats(Mathf.Abs(transform.position.y - vertexEditing.vertices[i].y), 0.03683151f) && transform.position.z == vertexEditing.vertices[i].z)
                {
                    if (!firstnearVertices.Contains(vertexEditing.transform.GetChild(i)))
                        firstnearVertices.Add(vertexEditing.transform.GetChild(i));

                }
                if (transform.position.x == vertexEditing.vertices[i].x && CompareFloats(Mathf.Abs(transform.position.y - vertexEditing.vertices[i].y), 0.03683151f * 3f) && transform.position.z == vertexEditing.vertices[i].z)
                {
                    if (!thridnearVertices.Contains(vertexEditing.transform.GetChild(i)))
                        thridnearVertices.Add(vertexEditing.transform.GetChild(i));

                }
            }
        }
        findNearIsSucc = true;
    }
    public void OnSelected()
    {
        selected = true;
        gizmosColor = Color.cyan;
    }
    public void OnUnselected()
    {
        selected = true;
        gizmosColor = Color.black;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmosColor;
        Gizmos.DrawSphere(transform.position, 0.02f);
    }
    bool CompareFloats(float num1, float num2)
    {
        if (Mathf.Abs(num1 - num2) < 0.01f)
        {
            return true;
        }
        else return false;
    }
}
