using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace StayingMythical
{
    namespace ExplorerTask
    {
        using Environment;
        using Reference;

        public class Task
        {
            public float reachedDistance = 3;

            public Transform location;
            public ExplorerController owner;

            bool complete = false;
            public bool Complete { get { return complete; } }
            public void CompleteTask()
            {
                currentState = TaskState.Complete;
                complete = true;
                owner.SetAnimation("Idle");
            }

            public enum TaskState { Travel, Action, Complete, Special }

            private TaskState currentState = TaskState.Travel;
            public TaskState taskState { get { return currentState; } }
            public void SetTaskState(TaskState state) { currentState = state; }
            public virtual void UpdateTask()
            {

                if (owner.agent)
                {
                    if (currentState == TaskState.Travel)
                    {
                        owner.agent.speed = 1.5f;
                        owner.agent.isStopped = false;
                        owner.agent.SetDestination(location.position);

                        if (Vector3.Distance(location.position, owner.transform.position) < reachedDistance)
                        {
                            currentState = TaskState.Action;
                        }

                    }

                    if (currentState == TaskState.Complete)
                    {
                        owner.agent.isStopped = true;

                        complete = true;
                    }

                    if (currentState == TaskState.Action)
                    {
                        owner.agent.isStopped = true;
                    }

                }

            }



        }
        public class GoTo : Task
        {
            public GoTo(Transform Target, ExplorerController owner)
            {
                this.owner = owner;
                location = Target;
            }


            public override void UpdateTask()
            {
                if (taskState == TaskState.Action)
                {
                    CompleteTask();

                }

                base.UpdateTask();

            }
        }
        public class Collect : Task
        {
            Interractable interractable;
            float processTime = 4;
            bool OnePass = false;

            public Collect(Interractable target, ExplorerController owner)
            {
                
                this.owner = owner;
                if(target is InterractableTree)
                {
                    reachedDistance = 3;
                }
                if(target is InterractableRock)
                {
                    reachedDistance = 5;
                }
              
                interractable = target;
                location = interractable.transform;
            }

            public override void UpdateTask()
            {

                if (interractable == null) CompleteTask();

              
                
                base.UpdateTask();

                if (taskState == TaskState.Action)
                {

                   

                    processTime -= Time.deltaTime;

                    if(!OnePass)
                    {
                        owner.transform.LookAt(location);
                        owner.SetAnimation("Collect");
                        OnePass = true;
                    }
                  

                    if(processTime <= 0)
                    {
                        SetTaskState(TaskState.Special);
                        interractable.Process(false, false);
                        owner.SetAnimation("Idle");
                        owner.SetAnimation("Carry");
                        OnePass = false;
                    }
                }

                if (taskState == TaskState.Special)
                {

                    if(!OnePass)
                    {
                        location = GameObjects.ExplorerBase.transform;
                        owner.agent.SetDestination(location.position);
                        owner.agent.speed = 1;
                        owner.agent.isStopped = false;
                        OnePass = true;
                    }
                                                        
                    if (Vector3.Distance(location.position, owner.transform.position) < 5)
                    {
                                                   
                        CompleteTask();
                    }

                }

              

            }

        }
        public class GoToBase : Task
        {
            public GoToBase(ExplorerController owner)
            {
                this.owner = owner;
                location = GameObjects.ExplorerBase.transform;
                reachedDistance = 5;
            }

            public override void UpdateTask()
            {
                base.UpdateTask();

                if(taskState == TaskState.Action)
                {
                    SetTaskState(TaskState.Complete);
                }
            }

        }
        public class Wait : Task
        {
            float waitTime;
            public Wait(ExplorerController owner, float time)
            {
                this.owner = owner;
                SetTaskState(TaskState.Action);
                waitTime = time;
            }

            public override void UpdateTask()
            {
                if(taskState == TaskState.Action)
                {
                    waitTime -= Time.deltaTime;

                    if(waitTime < 0)
                    {
                        CompleteTask();
                    }
                }
               
                
            }
        }

    }
    namespace Reference
    {
        public static class GameObjects
        {
            public static playerController player { get { return Object.FindObjectOfType<playerController>(); } }
            public static WorldGenerator WorldGenerator { get { return Object.FindObjectOfType<WorldGenerator>(); } }
            public static ExplorerManager ExplorerController { get { return Object.FindObjectOfType<ExplorerManager>(); } }
            public static ExplorerBase ExplorerBase { get { return Object.FindObjectOfType<ExplorerBase>(); } }
            public static InterractableRock[] Rocks { get { return Object.FindObjectsOfType<InterractableRock>(); } }
            public static InterractableTree[] Trees { get { return Object.FindObjectsOfType<InterractableTree>(); } }             
        }
        public static class GameResources
        {

            //environment 
            public static GameObject GroundTile = (GameObject)Resources.Load("EnvironmentObjects/GroundTile");
            public static GameObject Rock = (GameObject)Resources.Load("EnvironmentObjects/Rock");
            public static GameObject Tree = (GameObject)Resources.Load("EnvironmentObjects/Tree");
            public static GameObject Base = (GameObject)Resources.Load("EnvironmentObjects/Base");
            public static GameObject Explorer = (GameObject)Resources.Load("EnvironmentObjects/Explorer");
            public static GameObject Stump = (GameObject)Resources.Load("EnvironmentObjects/Stump");
            public static GameObject Player = (GameObject)Resources.Load("EnvironmentObjects/Player");
            public static GameObject SnowHill = (GameObject)Resources.Load("EnvironmentObjects/SnowHill");
            public static GameObject WorldLight = (GameObject)Resources.Load("EnvironmentObjects/WorldLight");

            //inventory
            public static GameObject SnowBall = (GameObject)Resources.Load("Inventory/SnowBall");
            public static GameObject RockPiece = (GameObject)Resources.Load("Inventory/Rock");
            public static GameObject Logs = (GameObject)Resources.Load("Inventory/Logs");
            public static GameObject Fire = (GameObject)Resources.Load("Inventory/Fire");
            public static GameObject Trap = (GameObject)Resources.Load("Inventory/Trap");

            //particles
            public static GameObject SnowParticles = (GameObject)Resources.Load("Particles/SnowHit");
            public static GameObject SnowStorm = (GameObject)Resources.Load("Particles/SnowStorm");

            //UI
            public static GameObject StandardCanvas = (GameObject)Resources.Load("UI/StandardCanvas");

            //Other
            public static GameObject Target = (GameObject)Resources.Load("Navigation/Target");
        }
    }
    namespace Environment
    {
        using Reference;

        using UnityEngine;
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

            public Obstacle addObstacle(Obstacles type)
            {
                Obstacle toReturn;

                switch (type)
                {
                    case Obstacles.Rock:
                        toReturn = (new Rock(this));
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
                        toReturn = (new Target(this));
                        break;
                    case Obstacles.Hill:
                        toReturn = (new SnowHill(this));
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

            public GroundTile(Vector3 position, Transform parent)
            {


                EnvironmentObject = Object.Instantiate((GameObject)Resources.Load("EnvironmentObjects/GroundTile"));
                EnvironmentTransform.parent = parent;
                EnvironmentTransform.position = position;

            }

            public GroundTile(Vector3 position)
            {

                EnvironmentObject = Object.Instantiate((GameObject)GameResources.GroundTile);
                EnvironmentTransform.parent = null;
                EnvironmentTransform.position = position;

            }

            public void DestoyTile()
            {
                Object.DestroyImmediate(EnvironmentObject);

            }
        }
        public class Environment
        {
            private string name;
            public string EnvironmentName { get { return name; } set { name = EnvironmentName; EnvironmentObject.name = name; } }
            public enum Obstacles { Rock, Tree, Base, Explorer, Ground, Yeti, TargetPos, Hill };

            public GameObject EnvironmentObject;

            public Transform EnvironmentTransform { get { return EnvironmentObject.transform; } }

        }
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
                        _toSpawn = GameResources.Rock;
                        break;
                    case Obstacles.Tree:
                        _toSpawn = GameResources.Tree;
                        break;
                    case Obstacles.Base:
                        _toSpawn = GameResources.Base;
                        break;
                    case Obstacles.Yeti:
                        _toSpawn = GameResources.Player;
                        break;
                    case Obstacles.Explorer:
                        _toSpawn = GameResources.Explorer;
                        break;
                    case Obstacles.TargetPos:
                        _toSpawn = GameResources.Target;
                        break;
                    case Obstacles.Hill:
                        _toSpawn = GameResources.SnowHill;
                        break;

                    default:
                        _toSpawn = GameResources.Rock;
                        break;
                }


                EnvironmentObject = GameObject.Instantiate(_toSpawn, GroundParent.EnvironmentTransform.position + SpawnPos(), Quaternion.identity, GroundParent.EnvironmentTransform);

            }

            public static bool MinObstacleDistance(Obstacle toCheck, Vector3 position, Obstacle existingObstacle, float min)
            {
                if (toCheck.EnvironmentObject && existingObstacle.EnvironmentObject)
                {


                    Vector3 sizeA;
                    if (toCheck.EnvironmentObject.GetComponent<MeshFilter>())
                    {
                        sizeA = toCheck.EnvironmentObject.GetComponent<MeshFilter>().sharedMesh.bounds.extents;
                    }
                    else
                    {
                        sizeA = Vector3.one * 2;
                    }
                    sizeA.y = 0;

                    Vector3 sizeB;
                    if (existingObstacle.EnvironmentObject.GetComponent<MeshFilter>())
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
                ObstaclePosition = new Vector2(Random.Range(0 + GroundTile.ObstacleMargin, GroundTile.TileSize - GroundTile.ObstacleMargin), Random.Range(0 + GroundTile.ObstacleMargin, GroundTile.TileSize - GroundTile.ObstacleMargin));

                bool passed = false;
                int iterations = 0;
                while (passed == false && iterations < 100)
                {
                    if (iterations == 99) Debug.LogWarning("Failed to find pos for " + obstacleType);
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
        public class Rock : Obstacle
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
        public class Tree : Obstacle
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
                FindSuitablePosition(5);
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
        public class SnowHill : Obstacle
        {
            public SnowHill(Vector2 Pos, GroundTile parent)
            {
                heightAboveGround = 0;
                obstacleType = Obstacles.Hill;
                ObstaclePosition = Pos;
                GroundParent = parent;
                CreateObstacle();
            }
            public SnowHill(GroundTile parent)
            {
                heightAboveGround = 0;
                obstacleType = Obstacles.Hill;
                GroundParent = parent;
                CreateObstacle();
                FindSuitablePosition(8);
            }
        }

    }
}
