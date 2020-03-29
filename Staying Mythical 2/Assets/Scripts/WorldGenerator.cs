using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StayingMythical.Environment;
using StayingMythical.Reference;
using UnityEngine.AI;
public class WorldGenerator : MonoBehaviour
{
 
  
    [SerializeField] private Vector2Int gridSize = new Vector2Int(2, 2);
    private Vector2Int oldGridSize;
    [SerializeField]  GroundTile[,] GroundTiles;
  

    [ContextMenu("GenerateGrid")]
    public void GenerateGrid()
    {
        RemoveOldGrid();
        CreateNewGrid();
        CreateExplorers(10);
        CreateHills(1);
        CreateBase();
        CreatePlayer();
        CreateEnvironment(15,5);
        GenerateNavMesh();
       
        CreateRandomPositions(5);
        CreateCanvas();
        CreateWeather();
         
    }

    private void Awake()
    {
        GenerateGrid();
    }

  
    void RemoveOldGrid()
    {
        if(GroundTiles != null)
            
        for (int x = 0; x < oldGridSize.x; x++)
        {
            for (int y = 0; y < oldGridSize.y; y++)
            {
                if (GroundTiles[x, y] != null)
                {
                    GroundTiles[x, y].DestoyTile();
                       
                }

            }
        }


        foreach (var item in GetComponentsInChildren<Transform>())
        {
            if(item != transform)
            {
                Destroy(item.gameObject);
            }
        }
    }
    void CreateNewGrid()
    {

        GroundTiles = new GroundTile[gridSize.x, gridSize.y];


        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                GroundTiles[x, y] = new GroundTile(new Vector3(x * GroundTile.TileSize, 0, y * GroundTile.TileSize), transform);
                GroundTiles[x, y].EnvironmentName = x + "_" + y;
                

               
            }
        }
        oldGridSize = gridSize;
    }
    void CreateHills(int perGrid)
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int i = 0; i < perGrid; i++)
                {
                    GroundTiles[x, y].addObstacle(Environment.Obstacles.Hill);
                }
               
            }
        }
    }
    void CreateBase()
    {
        Vector2Int baseTile = GroundTile.randomTile(gridSize);
        GroundTiles[baseTile.x, baseTile.y].addObstacle(Environment.Obstacles.Base);
    }
    void CreatePlayer()
    {
        Vector2Int playerTile = GroundTile.randomTile(gridSize);
        GroundTiles[playerTile.x, playerTile.y].addObstacle(Environment.Obstacles.Yeti);
    }
    void CreateEnvironment(float treesPer, float rocksPer)
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int i = 0; i < treesPer; i++)
                {
                   
                    GroundTiles[x, y].addObstacle(Environment.Obstacles.Tree);
                }
                for (int i = 0; i < rocksPer; i++)
                {
                    GroundTiles[x, y].addObstacle(Environment.Obstacles.Rock);
                  
                }

            }
        }
    }  
    void CreateExplorers(float perGrid)
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int i = 0; i < perGrid; i++)
                {
                    bool playerHere = false;
                    foreach (var item in GroundTiles[x,y].Obtacles)
                    {
                        if(item is Yeti)
                        {
                            playerHere = true;
                            break;
                        }
                    }

                    if(!playerHere)
                    {
                        GroundTiles[x, y].addObstacle(Environment.Obstacles.Explorer);
                    }
                    else
                    {
                        //print("Found Player at (" + x + "," + y + ")");
                    }
                    
                  
                }

            }
        }

        gameObject.AddComponent<ExplorerManager>();
    }
    void CreateRandomPositions(float perGrid)
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int i = 0; i < perGrid; i++)
                {

                    GameObjects.ExplorerController.addGoToTarget(GroundTiles[x, y].addObstacle(Environment.Obstacles.TargetPos).EnvironmentTransform);
                }
             
               
            }
        }
    }
    void CreateWeather()
    {
        Vector3 stormPos = new Vector3(-50, 0, -20f);
        ParticleSystem SnowStorm = Instantiate(GameResources.SnowStorm,stormPos,Quaternion.identity,null).GetComponent<ParticleSystem>();
        ParticleSystem.ShapeModule SnowShapeModule = SnowStorm.shape;
        ParticleSystem.EmissionModule SnowEmissionModule = SnowStorm.emission;
        SnowShapeModule.length = 100 + gridSize.x/2;
        SnowEmissionModule.rateOverTime = 200 * (gridSize.x - 1);

        Instantiate(GameResources.WorldLight);
        
    }

    [ContextMenu("Gen Nav Mesh")]
    void GenerateNavMesh()
    {
        if(!GetComponent<NavMeshSurface>())
        {
            gameObject.AddComponent<NavMeshSurface>();
        }

        GetComponent<NavMeshSurface>().BuildNavMesh();
    }
    void CreateCanvas()
    {
        Instantiate(GameResources.StandardCanvas);
    }

}


