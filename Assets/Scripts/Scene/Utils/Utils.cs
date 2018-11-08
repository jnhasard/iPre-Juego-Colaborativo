using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Utils
{

    #region Drawing

    public void DrawDistance(Vector2 from, Vector2 to, Color color, MonoBehaviour mono)
    {
        DrawDistance(from, to, color, mono.gameObject);
    }

    public void DrawCircle(Vector2 origin, float radius, Vector2? offset, Color color, MonoBehaviour mono)
    {
        DrawCircle(origin, radius, offset, color, mono.gameObject);
    }

    public void DrawBox(Vector2 origin, Vector2 size, Vector2? offset, Color color, MonoBehaviour mono)
    {
        DrawBox(origin, size, offset, color, mono.gameObject);
    }

    public void DrawCircle(Vector2 origin, float radius, Vector2? offset, Color color, GameObject GO)
    {
        origin = (offset != null) ? origin + (Vector2)offset : origin;

        float ThetaScale = 0.01f;
        float theta = 0f;

        float Size = (1 / ThetaScale) + 1;
        Vector3[] positions = new Vector3[(int)Size];

        for (int i = 0; i < Size; i++)
        {
            theta += (2.0f * Mathf.PI * ThetaScale);

            float x = radius * Mathf.Cos(theta);
            float y = radius * Mathf.Sin(theta);

            x += origin.x;
            y += origin.y;

            positions[i] = FrontVector(x, y);
        }

        LineRenderer lineRenderer = GetLineRenderer((int)Size, color, GO);
        lineRenderer.SetPositions(positions);
    }

    public void DrawBox(Vector2 origin, Vector2 size, Vector2? offset, Color color, GameObject GO)
    {
        origin = (offset != null) ? origin + (Vector2)offset : origin;

        float baseDistance = size.x / 2;
        float verticalDistance = size.y / 2;

        Vector2 A = FrontVector(origin.x - baseDistance, origin.y - verticalDistance);
        Vector2 B = FrontVector(origin.x - baseDistance, origin.y + verticalDistance);
        Vector2 C = FrontVector(origin.x + baseDistance, origin.y + verticalDistance);
        Vector2 D = FrontVector(origin.x + baseDistance, origin.y - verticalDistance);

        LineRenderer lineRenderer = GetLineRenderer(5, color, GO);
        Vector3[] points = new Vector3[] { A, B, C, D, A };
        lineRenderer.SetPositions(points);

        DrawText(origin, GO.name, GO);

    }

    public void DrawDistance(Vector2 from, Vector2 to, Color color, GameObject GO)
    {
        LineRenderer lineRenderer = GetLineRenderer(2, color, GO);

        Vector3[] positions = new Vector3[2] { FrontVector(from), FrontVector(to) };

        lineRenderer.SetPositions(positions);
    }

    public LineRenderer GetLineRenderer(int numPositions, Color color, GameObject GO)
    {
        LineRenderer lineRenderer = null;

        if (GO.activeInHierarchy)
        {
            lineRenderer = GO.GetComponent<LineRenderer>();

            if (!lineRenderer)
            {
                lineRenderer = GO.AddComponent<LineRenderer>();
            }

        }
        else
        {
            LevelManager levelManager = GameObject.FindObjectOfType<LevelManager>();

            lineRenderer = levelManager.GetComponent<LineRenderer>();

            if (!lineRenderer)
            {
                lineRenderer = levelManager.gameObject.AddComponent<LineRenderer>();
            }
        }

        //lineRenderer.material = new Material(Shader.Find("Particles/Additive"));

        lineRenderer.startWidth = .015f;
        lineRenderer.endWidth = .015f;
        lineRenderer.sortingOrder = 10;
        lineRenderer.sortingLayerName = "Player";

        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        lineRenderer.numPositions = numPositions;

        return lineRenderer;

    }

    public Vector3 FrontVector(Vector2 vector)
    {
        return new Vector3(vector.x, vector.y, 100);
    }

    public Vector3 FrontVector(float x, float y)
    {
        return new Vector3(x, y, 10);
    }

    public void DrawText(Vector2 origin, string text, GameObject GO)
    {
        if (!GO.GetComponent<Text>())
        {
            GO.AddComponent<Text>();
        }

        GO.GetComponent<Text>().text = text;
    }


    #endregion

    #region Common

    public void DrawColliders(GameObject[] gameObjects, bool onlyTriggers, MonoBehaviour mono)
    {
        List<Collider2D> colliders = new List<Collider2D>();

        for (int i = 0; i < gameObjects.Length; i++)
        {
            Collider2D collider = gameObjects[i].GetComponent<Collider2D>();

            if (collider)
            {
                colliders.Add(collider);
            }
        }

        DrawColliders(colliders.ToArray(), onlyTriggers, mono);
    }

    public void DrawColliders(Collider2D[] colliders, bool onlyTriggers, MonoBehaviour mono)
    {
        foreach (Collider2D collider in colliders)
        {
            if (collider.isTrigger)
            {
                if (collider.GetType().Equals(new CircleCollider2D().GetType()))
                {
                    CircleCollider2D circleCollider = (CircleCollider2D)collider;
                    DrawCircle(collider.transform.position, circleCollider.radius, circleCollider.offset, Color.cyan, mono);
                }

                else if (collider.GetType().Equals(new BoxCollider2D().GetType()))
                {
                    BoxCollider2D boxCollider = (BoxCollider2D)collider;
                    DrawBox(collider.transform.position, boxCollider.size, boxCollider.offset, Color.cyan, mono);
                }
            }
        }
    }

    #endregion
}
