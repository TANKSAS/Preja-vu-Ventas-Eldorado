using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : Singleton<BackgroundTile>
{
    [Header("Grid Settings")]
    public Vector2Int gridSize;
    public List<GameObject> tilePrefabs;
    public float spacing = 1.0f;

    [Header("Line Settings")]
    public float lineWidth = 0.1f;
    public Color lineColor = Color.white;
    [Range(0, 1)] public float lineAlpha = 1f;

    [Header("Oscillation Settings")]
    public float baseAmplitude = 1.0f;
    public float amplitudeVariation = 0.2f;
    public float baseSpeed = 2.0f;
    public float speedVariation = 0.5f;

    private List<GameObject> tiles = new List<GameObject>();
    private List<Vector3> initialPositions = new List<Vector3>();
    private List<float> speeds = new List<float>();
    private List<float> amplitudes = new List<float>();
    private List<float> timeOffsets = new List<float>();
    public Texture2D texture;

    private void Start()
    {
        LayoutGrid();
    }

    private void FixedUpdate()
    {
        OscillateAndConnectTiles();
    }

    private void LayoutGrid()
    {
        ClearGrid();

        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                Vector3 position = new Vector3(x * spacing, 0, y * spacing) + transform.position;
                GameObject randomTilePrefab = tilePrefabs[Random.Range(0, tilePrefabs.Count)];
                GameObject tile = Instantiate(randomTilePrefab, position, Quaternion.identity, transform);
                tile.name = $"Tile {x},{y}";

                tiles.Add(tile);
                initialPositions.Add(tile.transform.position);
                speeds.Add(baseSpeed + Random.Range(-speedVariation, speedVariation));
                amplitudes.Add(baseAmplitude + Random.Range(-amplitudeVariation, amplitudeVariation));
                timeOffsets.Add(Random.Range(0f, Mathf.PI * 2f));

                LineRenderer lineRenderer = tile.AddComponent<LineRenderer>();
                lineRenderer.startWidth = lineWidth;
                lineRenderer.endWidth = lineWidth;
                lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                lineRenderer.useWorldSpace = true;
                lineRenderer.positionCount = 0;
            }
        }
    }

    private void OscillateAndConnectTiles()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            // Mueve cada tile en el eje Y con oscilación sin Lerp
            float newY = initialPositions[i].y + Mathf.Sin(Time.time * speeds[i] + timeOffsets[i]) * amplitudes[i];
            tiles[i].transform.position = new Vector3(initialPositions[i].x, newY, initialPositions[i].z);

            // Actualiza las conexiones
            Vector2Int coordinate = GetCoordinateFromTileName(tiles[i].name);
            ConnectToNeighbors(tiles[i], coordinate);
        }
    }

    private Vector2Int GetCoordinateFromTileName(string name)
    {
        string[] parts = name.Split(' ');
        string[] coords = parts[1].Split(',');
        int x = int.Parse(coords[0]);
        int y = int.Parse(coords[1]);
        return new Vector2Int(x, y);
    }

    private void ConnectToNeighbors(GameObject tile, Vector2Int coordinate)
    {
        List<Vector3> positions = new List<Vector3>();
        Vector3 tilePosition = tile.transform.position;

        Vector2Int[] neighbors = new Vector2Int[] {
            new Vector2Int(1, 0), new Vector2Int(-1, 0),
            new Vector2Int(0, 1), new Vector2Int(0, -1)
        };

        foreach (var offset in neighbors)
        {
            Vector2Int neighborCoord = coordinate + offset;
            if (IsWithinBounds(neighborCoord))
            {
                Vector3 neighborPosition = new Vector3(neighborCoord.x * spacing, 0, neighborCoord.y * spacing) + transform.position;
                positions.Add(tilePosition);
                positions.Add(neighborPosition);
            }
        }

        LineRenderer lineRenderer = tile.GetComponent<LineRenderer>();
        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());

        Color colorWithAlpha = new Color(lineColor.r, lineColor.g, lineColor.b, lineAlpha);
        lineRenderer.startColor = colorWithAlpha;
        lineRenderer.endColor = colorWithAlpha;
    }

    private bool IsWithinBounds(Vector2Int coord)
    {
        return coord.x >= 0 && coord.x < gridSize.x && coord.y >= 0 && coord.y < gridSize.y;
    }

    private void ClearGrid()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        tiles.Clear();
        initialPositions.Clear();
        speeds.Clear();
        amplitudes.Clear();
        timeOffsets.Clear();
    }
}
