using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class MeshCutter : MonoBehaviour
{
    public LayerMask cuttableLayer;
    public InputAction cutAction;
    private List<int> triangles = new List<int>();
    private List<int> deletedTriangles = new List<int>();
    private GameObject hittingObject;

    private bool isCutting;
    private Vector3 bladeStart;
    private Vector3 bladeEnd;

    private void OnEnable()
    {
        cutAction.Enable();
        cutAction.started += OnCutStarted;
        cutAction.canceled += OnCutCanceled;
    }

    private void OnDisable()
    {
        cutAction.Disable();
        cutAction.started -= OnCutStarted;
        cutAction.canceled -= OnCutCanceled;
    }

    private void OnCutStarted(InputAction.CallbackContext context)
    {
        Debug.Log("Start cutting");
        isCutting = true;
        PerformCut();
    }

    private void OnCutCanceled(InputAction.CallbackContext context)
    {
        Debug.Log("Stop cutting");
        isCutting = false;
    }

    private void Update()
    {
        bladeStart = transform.position - transform.up * transform.localScale.y;
        bladeEnd = transform.position + transform.up * transform.localScale.y;

        if (isCutting)
        {
            
        }
        
        Debug.DrawLine(bladeStart, bladeEnd, Color.blue);
    }

    private void PerformCut()
    {
        if(Physics.Raycast(bladeStart, (bladeEnd - bladeStart).normalized, out RaycastHit hit, Vector3.Distance(bladeStart, bladeEnd), cuttableLayer) && hit.collider.gameObject.tag == "Cuttable")
        {
            Debug.Log("Hit: " + hit.collider.gameObject.name);
            //CreateCircleOnMesh(hit.collider.gameObject, hit, 0.01f, 3);
            hittingObject = hit.collider.gameObject;
            DeleteTriangle(hit.collider.gameObject, hit);
        }
        

        if(Physics.Raycast(bladeEnd, (bladeStart - bladeEnd).normalized, out RaycastHit hitReverse, Vector3.Distance(bladeStart, bladeEnd), cuttableLayer) && hitReverse.collider.gameObject.tag == "Cuttable")
        {
            Debug.Log("Reverse hit: " + hitReverse.collider.gameObject.name);
            //CreateCircleOnMesh(hitReverse.collider.gameObject, hitReverse, 0.01f, 3);
            DeleteTriangle(hitReverse.collider.gameObject, hitReverse);
        }
        
    }

    private void DeleteTriangle(GameObject target, RaycastHit hit)
    {
        MeshFilter meshFilter = target.GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;

        triangles = mesh.triangles.ToList();
        var vertices = mesh.vertices.ToList();

        Debug.Log("Object name: " + target.name +
                "\nTriangle count: " + (triangles.Count / 3) +
                "\nTouched triangle: " + hit.triangleIndex);

        // Üçgenin başlangıç indeksini bul
        int startIndex = hit.triangleIndex * 3;

        // Üçgeni kaldırmak için üç eleman çıkar
        if (startIndex < triangles.Count - 2)
        {
            for (int i = 0; i < 3; i++)
            {
                deletedTriangles.Add(triangles[startIndex + i]);
            }

            triangles.RemoveRange(startIndex, 3);
        }else
        {
            Debug.LogError("Invalid triangle index");
            return;
        }
        
        FillTheBlank();

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // MeshCollider'ı güncelle
        MeshCollider meshCollider = target.GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = mesh;
        }
    }

    private void FillTheBlank()
    {
        if(deletedTriangles.Count == 6)
        {
            for (int i = 0; i < deletedTriangles.Count / 2; i++)
            {
                int[] addedTriangles= new int[3];

                if(i != (deletedTriangles.Count / 2) - 1)
                {
                    triangles.Add(deletedTriangles[i + 1]);
                    addedTriangles[0] = deletedTriangles[i + 1];
                }else
                {
                    triangles.Add(deletedTriangles[0]);
                    addedTriangles[0] = deletedTriangles[0];
                }

                triangles.Add(deletedTriangles[deletedTriangles.Count - 1 - i]);
                addedTriangles[1] = deletedTriangles[deletedTriangles.Count - 1 - i];

                triangles.Add(deletedTriangles[i]);
                addedTriangles[2] = deletedTriangles[i];
                
                RemoveDuplicateTriangles(addedTriangles);
            }


            for (int i = 0; i < deletedTriangles.Count / 2; i++)
            {
                int[] addedTriangles= new int[3];

                triangles.Add(deletedTriangles[i]);
                addedTriangles[0] = deletedTriangles[i];

                triangles.Add(deletedTriangles[deletedTriangles.Count - 1 - i]);
                addedTriangles[1] = deletedTriangles[deletedTriangles.Count - 1 - i];

                if(i == 0)
                {
                    triangles.Add(deletedTriangles[deletedTriangles.Count - 3 - i]);
                    addedTriangles[2] = deletedTriangles[deletedTriangles.Count - 3 - i];
                }else
                {
                    triangles.Add(deletedTriangles[deletedTriangles.Count - i]);
                    addedTriangles[2] = deletedTriangles[deletedTriangles.Count - i];
                }

                RemoveDuplicateTriangles(addedTriangles);
            }

            deletedTriangles.Clear();
        }
    }

    private void RemoveDuplicateTriangles(int[] addedTriangles)
    {
        Array.Sort(addedTriangles);
        string triKey = string.Join(",", addedTriangles);

        int repeatCounter = 0;
        int[] repeatIndexes = new int[2];
        for (int j = 0; j < triangles.Count; j += 3)
        {
            int[] triVerticesToCompare = new int[] { triangles[j], triangles[j + 1], triangles[j + 2] };
            Array.Sort(triVerticesToCompare);
            string triKeyToCompare = string.Join(",", triVerticesToCompare);

            if (triKey == triKeyToCompare)
            {
                repeatIndexes[repeatCounter] = j;
                repeatCounter++;
            }
        }

        if(repeatCounter == 2)
        {
            for (int k = 0; k < 2; k++)
            {
                if(k == 1)
                {
                    triangles.RemoveRange(repeatIndexes[k] - 3, 3);
                    }else
                {
                    triangles.RemoveRange(repeatIndexes[k], 3);
                }
            }
        }
    }

    void CreateCircleOnMesh(GameObject target, RaycastHit hit, float radius, int segments)
    {
        MeshFilter meshFilter = target.GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;

        ProBuilderMesh proBuilderMesh = ProBuilderMesh.Create();
        //proBuilderMesh.CreateShapeFromPolygon;

        Vector3[] vertices = mesh.vertices;
        List<int> triangles = mesh.triangles.ToList();
        Vector3[] normals = mesh.normals;

        List<Vector3> newVertices = new List<Vector3>(vertices);
        List<int> newTriangles = new List<int>();

        // Merkezi noktayı mevcut vertex'lerin yakınlarına getir
        Vector3 center = hit.point;
        center = target.transform.InverseTransformPoint(center);

        // Yeni yuvarlak vertex'leri ekle
        int centerIndex = newVertices.Count;
        newVertices.Add(center);

        for (int i = 0; i < segments; i++)
        {
            float angle = 2 * MathF.PI * i / segments;
            Vector3 offset = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
            
            if(Physics.Raycast(bladeStart + offset, (bladeEnd - bladeStart + offset).normalized, out RaycastHit circleHit, Vector3.Distance(bladeStart, bladeEnd), cuttableLayer) && hit.collider.gameObject.tag == "Cuttable")
            {
                Vector3 newVertex = target.transform.InverseTransformPoint(circleHit.point);
                newVertices.Add(newVertex);
            }
        }

        for (int i = 0; i < segments; i++)
        {
            int nextIndex = (i + 1) % segments;

            triangles.Add(centerIndex + 1 + nextIndex);
            triangles.Add(centerIndex + 1 + i);
            triangles.Add(centerIndex);
        }

        // Mesh'i güncelle
        mesh.vertices = newVertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }

    int ReturnClosestPoint(Vector3 pivot, Vector3 v1, Vector3 v2, Vector3 v3, int i)
    {
        float dis1 = Vector3.Distance(pivot, v1);
        float dis2 = Vector3.Distance(pivot, v2);
        float dis3 = Vector3.Distance(pivot, v3);

        if(dis1 <= dis2 && dis1 <= dis3)
        {
            return triangles[i];
        }else if (dis2 <= dis1 && dis2 <= dis3)
        {
            return triangles[i + 1];
        }else
        {
            return triangles[i + 2];
        }
    }

    private void OnDrawGizmos()
    {
        if(hittingObject != null && false)
        {
            MeshFilter meshFilter = hittingObject.GetComponent<MeshFilter>();
            Mesh mesh = meshFilter.mesh;

            Gizmos.color = Color.red;

            for (int i = 0; i < mesh.vertices.Count(); i++)
            {
                Gizmos.DrawWireSphere(hittingObject.transform.TransformPoint(mesh.vertices[i]), 0.03f);
            }
        }
    }
}   
