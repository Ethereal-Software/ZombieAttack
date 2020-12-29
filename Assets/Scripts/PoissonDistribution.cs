using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 
 Poisson distrubtion:

1. get random point
2. if has neighbor within radius then remove

Simplification(for computation)

1. create grid the diag = to distrib. radius
because
- will always be only point in block
- only need to check neighbors in 5x5 block
2. 
 
 
 */


[SelectionBase]
public class PoissonDistribution : MonoBehaviour
{
    public float radius;//min spacing inclusive
    public Vector2 gridSize;
    float cellSize;

    Vector2[,] grid;

    public GameObject instanceProto;
    List<GameObject> instances;

    public bool updatePoint;
    public bool populateInstances;
    public bool clear;

    public int retryCount = 10;

    public int seed = 0;

    public bool randomizeRotation = false;
    public float sizeVariation = 0;
    //[Vi]
    public int instanceCount;

    List<Vector3> points;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnDrawGizmosSelected()
    {
        if (clear) { 
            clear = false;
            ClearInstances();
        }

        if (updatePoint)
        {
            updatePoint = false;
            Generate();
            
        }        
        if (populateInstances)
        {
            populateInstances = false;
            PopulateInstances();
            
        }
    }
    void GenerateDistrubtion() {
        
    }

    void ClearInstances() {
        if (instances != null)
        {
            foreach (GameObject go in instances)
            {
                if (go == null) continue;
                DestroyImmediate(go);
            }
        }

        while (transform.childCount != 0)
        {
            foreach (Transform t in transform)
            {
                DestroyImmediate(t.gameObject);
            }
        }
    }


    bool CheckNeighbors(Vector2 pos, int xIndex, int yIndex) {
        int xStart = Mathf.Max(0, xIndex - 2);
        int yStart = Mathf.Max(0, yIndex - 2);        
        int xEnd = Mathf.Min(grid.GetLength(0)-1, xIndex + 2);
        int yEnd = Mathf.Min(grid.GetLength(1)-1, yIndex + 2);

        
        for (int x = xStart; x <= xEnd; x++) {
            for (int y = yStart; y <= yEnd; y++)
            {
                Vector2 candidate = grid[x, y];

                if (candidate == null) continue;//no point here yet

                float dist = Vector2.Distance(pos, candidate);
                if (dist < radius) {
                    return false;
                }

            }
        }
        return true;
    }

    private void Generate()
    {
        
        //{
        //    update = false;
        //}
        //else return;
        ClearInstances();
        instances = new List<GameObject>();
        points = new List<Vector3>();


        cellSize = Mathf.Sqrt(radius * radius / 2);

        int xSize = (int)(gridSize.x / cellSize);
        int ySize = (int)(gridSize.y / cellSize);
        grid = new Vector2[xSize, ySize];

        //Debug.Log("size:" + xSize + ", " + ySize +", cell size:"+ cellSize);

        float upperBound = cellSize - 0.000000001f;

        float xStart = 0f;
        float yStart = 0f;
        //crude (no check)
        Random.seed = seed;

        instanceCount = 0;

        for (int x = 0; x < xSize; x++)
        {
            yStart = 0f;
            for (int y = 0; y < ySize; y++)
            {

                Vector2 tp;
                int ct = 0;

                //find candidate

                bool gotPoint = false;
                while (true) {
                    Vector2 rand = new Vector2(Random.Range(0, upperBound), Random.Range(0, upperBound));
                    tp = new Vector2(xStart, yStart) + rand;

                    //Debug.Log("Neighbor check: " + CheckNeighbors(tp, x, y));
                    if (CheckNeighbors(tp, x, y))
                    {
                        gotPoint = true;
                        break;
                    }
                    //break;
                    if (ct > retryCount) {
                        //Debug.Log("More than 100 attempts to get point");
                        break;
                    }
                    ct++;

                }
                if (gotPoint)
                {//if can't find point, ignore this cell


                    grid[x, y] = tp;
                    //chack here
                    Vector3 posTp = new Vector3(tp.x, 0, tp.y);

                    //var inst = Instantiate(instanceProto, transform);
                    //inst.transform.localPosition = posTp;
                    //if (randomizeRotation)
                    //    inst.transform.localRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

                    //float randSize = Random.Range(1f, 1f + sizeVariation);
                    //inst.transform.localScale = new Vector3(randSize, randSize, randSize);

                    //instances.Add(inst);
                    instanceCount++;

                    points.Add(posTp);
                    //Gizmos.DrawSphere(posTp, 0.1f);
                }
                yStart += cellSize;
            }
            xStart += cellSize;
        }

    }

    public void PopulateInstances() {
        Random.seed = seed;
        foreach (Vector3 posTp in points) {
            var inst = Instantiate(instanceProto, transform);
            inst.transform.localPosition = posTp;
            if (randomizeRotation)
                inst.transform.localRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

            float randSize = Random.Range(1f, 1f + sizeVariation);
            inst.transform.localScale = new Vector3(randSize, randSize, randSize);

            instances.Add(inst);
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
