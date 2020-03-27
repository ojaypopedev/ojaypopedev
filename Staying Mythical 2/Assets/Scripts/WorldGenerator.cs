using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mythical;

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
        CreateEnvironment();

       
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
        GroundTiles[baseTile.x, baseTile.y].addObstacle(Environment.Obsctacles.Base);
    }

    void CreatePlayer()
    {
        Vector2Int playerTile = GroundTile.randomTile(gridSize);
        GroundTiles[playerTile.x, playerTile.y].addObstacle(Environment.Obsctacles.Yeti);
    }

    void CreateEnvironment()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int i = 0; i < 5; i++)
                {
                    GroundTiles[x, y].addObstacle(Environment.Obsctacles.Rock);
                    GroundTiles[x, y].addObstacle(Environment.Obsctacles.Tree);
                }

            }
        }
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

        private List<Obstacle> _obsctalces = new List<Obstacle>();
        public Obstacle[] Obtacles { get { return _obsctalces.ToArray(); } }
        public void addObstacle(Obsctacles type, Vector2 position)
        {
            switch (type)
            {
                case Obsctacles.Rock:
                    _obsctalces.Add(new Rock(position,this));
                    break;
                case Obsctacles.Tree:
                    _obsctalces.Add(new Tree(position,this));
                    break;
                case Obsctacles.Base:
                    _obsctalces.Add(new Base(position,this));
                    break;
                default:
                    break;
            }
            

        }
        public void addObstacle(Obsctacles type)
        {
            switch (type)
            {
                case Obsctacles.Rock:
                    _obsctalces.Add(new Rock(this));
                    break;
                case Obsctacles.Tree:
                    _obsctalces.Add(new Tree(this));
                    break;
                case Obsctacles.Base:
                    _obsctalces.Add(new Base(this));
                    break;
                case Obsctacles.Explorer:
                    _obsctalces.Add(new Explorer(this));
                    break;
                case Obsctacles.Yeti:
                    _obsctalces.Add(new Yeti(this));
                    break;
                default:
                    break;
            }

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
        public enum Obsctacles {Rock, Tree, Base, Explorer, Ground, Yeti};

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

        public Obsctacles obstacleType;
        public Obsctacles ObstacleType { get { return obstacleType; } }
        public void CreateObstacle()
        {
            GameObject _toSpawn;

            switch (obstacleType)
            {
                case Obsctacles.Rock:
                    _toSpawn = GameObjectReference.Rock;
                    break;
                case Obsctacles.Tree:
                    _toSpawn = GameObjectReference.Tree;
                    break;
                case Obsctacles.Base:
                    _toSpawn = GameObjectReference.Base;
                    break;
                case Obsctacles.Yeti:
                    _toSpawn = GameObjectReference.Player;
                    break;

                default:
                    _toSpawn = GameObjectReference.Rock;
                    break;
            }

            Debug.Log(GroundParent.EnvironmentTransform);
            EnvironmentObject = GameObject.Instantiate(_toSpawn, GroundParent.EnvironmentTransform.position + SpawnPos(), Quaternion.identity, GroundParent.EnvironmentTransform);
       

        }

        public static bool MinObstacleDistance(Obstacle toCheck, Vector3 position, Obstacle existingObstacle, float min)
        {
            if(toCheck.EnvironmentObject && existingObstacle.EnvironmentObject)
            {
                Vector3 sizeA = toCheck.EnvironmentObject.GetComponent<MeshFilter>().sharedMesh.bounds.extents;
                sizeA.y = 0;
                Vector3 sizeB = existingObstacle.EnvironmentObject.GetComponent<MeshFilter>().sharedMesh.bounds.extents;
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
            obstacleType = Obsctacles.Rock;
            ObstaclePosition = Pos;
            GroundParent = parent;
            CreateObstacle();
            
            
        }
        public Rock(GroundTile parent)
        {
            heightAboveGround = 1;
            obstacleType = Obsctacles.Rock;      
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
            obstacleType = Obsctacles.Base;
            ObstaclePosition = Pos;
            GroundParent = parent;
            CreateObstacle();
        }
        public Base(GroundTile parent)
        {
            
            heightAboveGround = 0f;
            obstacleType = Obsctacles.Base;
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
            obstacleType = Obsctacles.Tree;
            ObstaclePosition = Pos;
            GroundParent = parent;
            CreateObstacle();
        }
        public Tree(GroundTile parent)
        {
            heightAboveGround = 0.5f;
            obstacleType = Obsctacles.Tree;      
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

            obstacleType = Obsctacles.Tree;
            ObstaclePosition = Pos;
            GroundParent = parent;
            CreateObstacle();
        }
        public Explorer(GroundTile parent)
        {
            ObstaclePosition = new Vector2(Random.Range(0, GroundTile.TileSize), Random.Range(0, GroundTile.TileSize));

            obstacleType = Obsctacles.Tree;

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
            obstacleType = Obsctacles.Tree;
            ObstaclePosition = Pos;
            GroundParent = parent;
            CreateObstacle();
        }
        public Yeti(GroundTile parent)
        {
            heightAboveGround = 3;
            ObstaclePosition = new Vector2(Random.Range(0, GroundTile.TileSize), Random.Range(0, GroundTile.TileSize));
            obstacleType = Obsctacles.Yeti;
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
    }
}

