using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mythical;
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
        CreateBase();
        CreatePlayer();
        CreateEnvironment(10,5);
        GenerateNavMesh();
        CreateExplorers(5);
        CreateRandomPositions(5);
        CreateCanvas();
            
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

                    StayingMythical.ExplorerController.addGoToTarget(GroundTiles[x, y].addObstacle(Environment.Obstacles.TargetPos).EnvironmentTransform);
                }
             
               
            }
        }
    }


    [ContextMenu("Gen Nav Mesh")]
    void GenerateNavMesh()
    {
        GetComponent<NavMeshSurface>().BuildNavMesh();
    }
    void CreateCanvas()
    {
        Instantiate(GameObjectReference.StandardCanvas);
    }

}


namespace Mythical
{


    using UnityEngine;

   
    [System.Serializable]
    public class GroundTile : Environment
    {

        public static Vector2Int randomTile(Vector2Int gridSize)
       {
           return new Vector2Int(Mathf.RoundToInt(Random.Range(0, gridSize.x)), Mathf.RoundToInt(Random.Range(0, gridSize.y)));

       }

        public static readonly float TileSize = 64;
        public static readonly float ObstacleMargin = 5;

        private List<Obstacle> _obstacles = new List<Obstacle>();
        public Obstacle[] Obtacles { get { return _obstacles.ToArray(); } }
     
        public void addObstacle(Obstacles type, Vector2 position)
        {
            switch (type)
            {
                case Obstacles.Rock:
                    _obstacles.Add(new Rock(position,this));
                    break;
                case Obstacles.Tree:
                    _obstacles.Add(new Tree(position,this));
                    break;
                case Obstacles.Base:
                    _obstacles.Add(new Base(position,this));
                    break;
                case Obstacles.TargetPos:
                    _obstacles.Add(new Target(position, this));
                    break;
                default:
                    Debug.Log(type.ToString() + " has not been added to addObstacle, specific pos");
                    break;
            }
            

        }
        public Obstacle addObstacle(Obstacles type)
        {
            Obstacle toReturn;

            switch (type)
            {
                case Obstacles.Rock:
                    toReturn  = (new Rock(this));
                    break;
                case Obstacles.Tree:
                    toReturn = (new Tree(this));
                    break;
                case Obstacles.Base:
                    toReturn = (new Base(this));
                    break;
                case Obstacles.Explorer:

                    toReturn = (new Explorer(this));
                   
                    break;
                case Obstacles.Yeti:
                    toReturn = (new Yeti(this));
                    break;
                case Obstacles.TargetPos:
                    toReturn =(new Target(this));
                    break;
                default:
                    toReturn = null;
                    Debug.Log(type.ToString() + " has not been added to addObstacle, random pos");
                    break;
            }

            _obstacles.Add(toReturn);
            return toReturn;
        }

        public GroundTile()
        {

        }

        public GroundTile(Vector3 position,Transform parent)
        {
           
            
            EnvironmentObject = Object.Instantiate((GameObject)Resources.Load("EnvironmentObjects/GroundTile"));
            EnvironmentTransform.parent = parent;
            EnvironmentTransform.position = position;

        }

        public GroundTile(Vector3 position)
        {

            EnvironmentObject = Object.Instantiate((GameObject)GameObjectReference.GroundTile);
            EnvironmentTransform.parent = null;
            EnvironmentTransform.position = position;

        }

        public void DestoyTile()
        {
            Object.DestroyImmediate(EnvironmentObject);
           
        }
    }

    [System.Serializable]
    public class Environment
    {
        private string name;
        public string EnvironmentName { get { return name; } set { name = EnvironmentName; EnvironmentObject.name = name; } }
        public enum Obstacles {Rock, Tree, Base, Explorer, Ground, Yeti, TargetPos};

        public GameObject EnvironmentObject;

        public Transform EnvironmentTransform { get { return EnvironmentObject.transform; } }

    }

    [System.Serializable]
    public class Obstacle : Environment
    {
        public GroundTile GroundParent;

        public Vector2 ObstaclePosition;

        public float heightAboveGround = 0;
        private Vector3 SpawnPos()
        {
            return new Vector3(ObstaclePosition.x, heightAboveGround, ObstaclePosition.y);
        }

        public Obstacles obstacleType;
        public Obstacles ObstacleType { get { return obstacleType; } }
        public void CreateObstacle()
        {
            GameObject _toSpawn;

            switch (obstacleType)
            {
                case Obstacles.Rock:
                    _toSpawn = GameObjectReference.Rock;
                    break;
                case Obstacles.Tree:
                    _toSpawn = GameObjectReference.Tree;
                    break;
                case Obstacles.Base:
                    _toSpawn = GameObjectReference.Base;
                    break;
                case Obstacles.Yeti:
                    _toSpawn = GameObjectReference.Player;
                    break;
                case Obstacles.Explorer:
                    _toSpawn = GameObjectReference.Explorer;                   
                    break;
                case Obstacles.TargetPos:
                    _toSpawn = GameObjectReference.Target;
                    break;

                default:
                    _toSpawn = GameObjectReference.Rock;
                    break;
            }

       
            EnvironmentObject = GameObject.Instantiate(_toSpawn, GroundParent.EnvironmentTransform.position + SpawnPos(), Quaternion.identity, GroundParent.EnvironmentTransform);
       

        }

        public static bool MinObstacleDistance(Obstacle toCheck, Vector3 position, Obstacle existingObstacle, float min)
        {
            if(toCheck.EnvironmentObject && existingObstacle.EnvironmentObject)
            {


                Vector3 sizeA;
                if (toCheck.EnvironmentObject.GetComponent<MeshFilter>())
                {
                   sizeA =  toCheck.EnvironmentObject.GetComponent<MeshFilter>().sharedMesh.bounds.extents;
                }
                else
                {
                    sizeA = Vector3.one * 2;
                }                 
                sizeA.y = 0;

                Vector3 sizeB;
                if(existingObstacle.EnvironmentObject.GetComponent<MeshFilter>())
                {
                    sizeB = existingObstacle.EnvironmentObject.GetComponent<MeshFilter>().sharedMesh.bounds.extents;
                }
                else
                {
                    sizeB = Vector3.one * 2;
                }
                sizeB.y = 0;

                return Vector3.Distance(existingObstacle.ObstaclePosition, position) > min + sizeA.magnitude + sizeB.magnitude;
            }
            else
            {
                return false;
            }
        }
       
        public void FindSuitablePosition(float size)
        {
            ObstaclePosition = new Vector2(Random.Range(0+GroundTile.ObstacleMargin, GroundTile.TileSize- GroundTile.ObstacleMargin), Random.Range(0+GroundTile.ObstacleMargin, GroundTile.TileSize- GroundTile.ObstacleMargin));
         
            bool passed = false;
            int iterations = 0;
            while (passed == false && iterations < 100)
            {
                if (iterations == 99) Debug.LogWarning("Failed to find pos");
                passed = true;
                iterations++;

                foreach (var item in GroundParent.Obtacles)
                {
                    if (!MinObstacleDistance(this, ObstaclePosition, item, size))
                    {
                        passed = false;
                        ObstaclePosition = new Vector2(Random.Range(0, GroundTile.TileSize), Random.Range(0, GroundTile.TileSize));
                    }

                }
            }

            EnvironmentTransform.position = GroundParent.EnvironmentTransform.position + SpawnPos();

        }

    }

    [System.Serializable]
    public class Rock: Obstacle
    {      
        public Rock(Vector2 Pos, GroundTile parent)
        {
            heightAboveGround = 1;
            obstacleType = Obstacles.Rock;
            ObstaclePosition = Pos;
            GroundParent = parent;
            CreateObstacle();
            
            
        }
        public Rock(GroundTile parent)
        {
            heightAboveGround = 1;
            obstacleType = Obstacles.Rock;      
            GroundParent = parent;
            CreateObstacle();
           
            FindSuitablePosition(3);
          
        }

    }

    [System.Serializable]
    public class Base : Obstacle
    {
        public Base(Vector2 Pos, GroundTile parent)
        {
            heightAboveGround = 0f;
            obstacleType = Obstacles.Base;
            ObstaclePosition = Pos;
            GroundParent = parent;
            CreateObstacle();
        }
        public Base(GroundTile parent)
        {
            
            heightAboveGround = 0f;
            obstacleType = Obstacles.Base;
            ObstaclePosition = new Vector2(Random.Range(0, GroundTile.TileSize), Random.Range(0, GroundTile.TileSize));
            GroundParent = parent;
            CreateObstacle();
            FindSuitablePosition(8);
        }

    }

    [System.Serializable]
    public class Tree:Obstacle
    {
        public Tree(Vector2 Pos, GroundTile parent)
        {
            
            heightAboveGround = 0.5f;
            obstacleType = Obstacles.Tree;
            ObstaclePosition = Pos;
            GroundParent = parent;
            CreateObstacle();
        }
        public Tree(GroundTile parent)
        {
            heightAboveGround = 0.5f;
            obstacleType = Obstacles.Tree;      
            GroundParent = parent;
            CreateObstacle();
            FindSuitablePosition(5);
           
        }


    }

    [System.Serializable]
    public class Explorer : Obstacle
    {
        public Explorer(Vector2 Pos, GroundTile parent)
        {

            obstacleType = Obstacles.Explorer;
            ObstaclePosition = Pos;
            GroundParent = parent;
            CreateObstacle();
        }
        public Explorer(GroundTile parent)
        {
            ObstaclePosition = new Vector2(Random.Range(0, GroundTile.TileSize), Random.Range(0, GroundTile.TileSize));
            obstacleType = Obstacles.Explorer;
            GroundParent = parent;
          
            CreateObstacle();
            FindSuitablePosition(3);
        }
    }

    public class Yeti : Obstacle
    {
        public Yeti(Vector2 Pos, GroundTile parent)
        {
            heightAboveGround = 3;
            obstacleType = Obstacles.Tree;
            ObstaclePosition = Pos;
            GroundParent = parent;
            CreateObstacle();
        }
        public Yeti(GroundTile parent)
        {
            heightAboveGround = 3;          
            obstacleType = Obstacles.Yeti;
            GroundParent = parent;
            CreateObstacle();
            FindSuitablePosition(5);
        }
    }

    public class Target : Obstacle
    {
        public Target(Vector2 Pos, GroundTile parent)
        {
            heightAboveGround = 0;
            obstacleType = Obstacles.TargetPos;
            ObstaclePosition = Pos;
            GroundParent = parent;
            CreateObstacle();
        }
        public Target(GroundTile parent)
        {
            heightAboveGround = 0;         
            obstacleType = Obstacles.TargetPos;
            GroundParent = parent;
            CreateObstacle();
            FindSuitablePosition(5);
        }
    }

    [System.Serializable]
    public static class GameObjectReference
    {

        //environment 
        public static GameObject GroundTile = (GameObject)Resources.Load("EnvironmentObjects/GroundTile");
        public static GameObject Rock = (GameObject)Resources.Load("EnvironmentObjects/Rock");
        public static GameObject Tree = (GameObject)Resources.Load("EnvironmentObjects/Tree");
        public static GameObject Base = (GameObject)Resources.Load("EnvironmentObjects/Base");
        public static GameObject Explorer = (GameObject)Resources.Load("EnvironmentObjects/Explorer");
        public static GameObject Stump = (GameObject)Resources.Load("EnvironmentObjects/Stump");
        public static GameObject Player = (GameObject)Resources.Load("EnvironmentObjects/Player");

        //inventory
        public static GameObject SnowBall = (GameObject)Resources.Load("Inventory/SnowBall");
        public static GameObject RockPiece = (GameObject)Resources.Load("Inventory/Rock");
        public static GameObject Logs = (GameObject)Resources.Load("Inventory/Logs");
        public static GameObject Fire = (GameObject)Resources.Load("Inventory/Fire");
        public static GameObject Trap = (GameObject)Resources.Load("Inventory/Trap");

        //particles
        public static GameObject SnowParticles = (GameObject)Resources.Load("Particles/SnowHit");

        //UI
        public static GameObject StandardCanvas = (GameObject)Resources.Load("UI/StandardCanvas");

        //Other
        public static GameObject Target = (GameObject)Resources.Load("Navigation/Target");
    }
}

