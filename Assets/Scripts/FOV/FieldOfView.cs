using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float viewAngle = 90f;
    public float viewDistance = 8f;
    public int rayCount = 60;
    public LayerMask obstacleMask;

    private Mesh viewMesh;
    private Vector3 origin;
    private float startingAngle;

    void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        GetComponent<MeshFilter>().mesh = viewMesh;
    }

    void LateUpdate()
    {
        DrawFieldOfView();
    }

    public void SetOrigin(Vector3 newOrigin)
    {
        origin = newOrigin;
    }

    public void SetAimDirection(Vector3 direction)
    {
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        startingAngle = angle + (viewAngle / 2f); 
    }

    void DrawFieldOfView()
    {
        float angleStep = viewAngle / rayCount;
        Vector3[] vertices = new Vector3[rayCount + 1];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = origin;
        int vertexIndex = 1;
        int triangleIndex = 0;

        for (int i = 0; i < rayCount; i++)
        {
            float angle = startingAngle - angleStep * i;
            Vector3 dir = GetDirectionFromAngle(angle);
            Vector3 vertex;

            RaycastHit2D hit = Physics2D.Raycast(origin, dir, viewDistance, obstacleMask);
            vertex = (hit.collider != null) ? hit.point : origin + dir * viewDistance;

            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex++] = 0;
                triangles[triangleIndex++] = vertexIndex - 1;
                triangles[triangleIndex++] = vertexIndex;
            }

            vertexIndex++;
        }

        triangles[triangleIndex++] = 0;
        triangles[triangleIndex++] = rayCount;
        triangles[triangleIndex++] = 1;

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    Vector3 GetDirectionFromAngle(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad));
    }
}