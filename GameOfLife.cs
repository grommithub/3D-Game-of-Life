using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cell
{
    public bool isAlive, isGoingToBeAlive;
    public GameObject cube;
}
public class GameOfLife : MonoBehaviour
{
    Vector3 temp = new Vector3();

    static int gridSize = 20;
    public Cell[] cells = new Cell[gridSize * gridSize * gridSize];

    [Range(0f, 1f)]
    public float probabliltyOfAlive = 0.5f;

    [Range(0.05f, 3.0f)]
    public float waitTime = 0.2f;

    [Range(1.0f, 100.0f)]
    public float cameraSpeed = 50f;

    public int tooFew = 3, tooMany = 6, comeToLife = 5;



    // Start is called before the first frame update
    void Start()
    {
        for (int z = 0; z < gridSize; z++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    int num = GetIndex(x, y, z);
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.name = $"Cube { num }";
                    cube.transform.position = new Vector3(x, y, z);
                    cube.GetComponent<MeshRenderer>().material.color = new Color((float)x / gridSize, (float)y / gridSize, (float)z / gridSize);
                    cells[num].cube = cube;
                    cube.isStatic = true;
                }
            }
        }

        Reset();

        StartCoroutine(RunGameOfLife());
        Reset();
    }
    void Reset()
    {
        foreach (var c in cells)
        {
            c.isAlive = Random.Range(0f, 1f) < probabliltyOfAlive;
            c.isGoingToBeAlive = c.isAlive;
            c.cube.SetActive(c.isAlive);
        }
    }

    void NextGeneration()
    {
        foreach (var c in cells)
        {
            c.isAlive = c.isGoingToBeAlive;
            c.cube.SetActive(c.isAlive);
            c.isGoingToBeAlive = false;
        }
        for (int z = 0; z < gridSize; z++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    Cell c = cells[GetIndex(x, y, z)];
                    int neighbours = GetNeighbours(x, y, z);
                    if (c.isAlive)
                    {
                        if (neighbours < tooFew || neighbours > tooMany)
                        {
                            c.isGoingToBeAlive = false;
                        }
                        else
                        {
                            c.isGoingToBeAlive = true;
                        }
                    }
                    else
                    {
                        if (neighbours == comeToLife)
                        {
                            c.isGoingToBeAlive = true;
                        }
                    }
                }
            }
        }
    }

    int GetIndex(int x, int y, int z)
    {
        return x + (y * gridSize) + z * (gridSize * gridSize);
    }
    bool OutOfBounds(int x, int y, int z)
    {
        if (x >= gridSize || x < 0 || y >= gridSize || y < 0 || z >= gridSize || z < 0 || GetIndex(x, y, z) > cells.Length - 1 || GetIndex(x, y, z) < 0)
        {
            return true;
        }
        return false;
    }

    int GetNeighbours(int x, int y, int z)
    {
        int total = 0;
        for (int z2 = z - 1; z2 <= z + 1; z2++)
        {
            for (int y2 = y - 1; y2 <= y + 1; y2++)
            {
                for (int x2 = x - 1; x2 <= x + 1; x2++)
                {
                    if ((x == x2 && y == y2 && z == z2) || OutOfBounds(x2, y2, z2)) continue;
                    if (cells[GetIndex(x2, y2, z2)].isAlive) total++;
                }
            }
        }
        return total;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Reset();
        }

        transform.position = Quaternion.AngleAxis(Time.deltaTime * cameraSpeed, Vector3.up) * transform.position;
        transform.LookAt(cells[cells.Length / 2].cube.transform.position);
    }
    public IEnumerator RunGameOfLife()
    {
        int generation = 0;
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            NextGeneration();
            generation++;
        }
    }
}
