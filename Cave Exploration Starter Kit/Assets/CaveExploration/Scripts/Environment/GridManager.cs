using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CaveExploration
{
    /// <summary>
    /// Singleton. Handles level generation. 
    /// </summary>
    [RequireComponent(typeof(TexturePack))]
    [RequireComponent(typeof(NodeClusterManager))]
    [RequireComponent(typeof(Utilities))]
    [RequireComponent(typeof(ObjectManager))]
    public class GridManager : MonoBehaviour
    {
        /// <summary>
        /// The size of the grid.
        /// </summary>
        public Rect GridSize;

        /// <summary>
        /// Disables background sprites renderers. Increases performance through reduced draw calls.  
        /// </summary>
        public bool DrawBackground = false;

        /// <summary>
        /// The number of transistion steps to perform when generating the level. The higher this number the more the the specified rules are applied.
        /// </summary>
        public int NumberOfTransistionSteps = 0;

        /// <summary>
        /// The chance for a floor tile to become a wall tile when first generating the level.
        /// </summary>
        [Range(0, 1)]
        public float
            ChanceToBecomeWall = 0.40f;

        /// <summary>
        /// If a tile has a number of neighbours higher than this then it too will be changed into a wall tile.
        /// </summary>
        [Range(0, 8)]
        public int
            FloorsToWallConversion = 4;

        /// <summary>
        /// If a wall tile has less than this number of neighbours that are wall tiles then it is converted into a background tile.
        /// </summary>
        [Range(0, 8)]
        public int
            WallsToFloorConversion = 3;

        /// <summary>
        /// The tile prefab.
        /// </summary>
        public GameObject Cell;

        /// <summary>
        /// Gets or sets the grid.
        /// </summary>
        /// <value>The grid.</value>
        public NodeList Grid { get; set; }

        /// <summary>
        /// Gets or sets the start node (where the player is spawned).
        /// </summary>
        /// <value>The start node.</value>
        public Node StartNode { get; set; }

        /// <summary>
        /// Gets or sets the end node (where the end object is spawned).
        /// </summary>
        /// <value>The end node.</value>
        public Node EndNode { get; set; }

        private static readonly string SCRIPT_NAME = typeof(GridManager).Name;

        private float minDistanceBetweenStartAndEnd;
        private NodeClusterManager nodeClusterManager;
        private List<GameObject> nodes = new List<GameObject>();
        private GameObject parent;
        private List<Node> emptyFloorNodes = new List<Node>();
        private bool IsConnectedEnvironment = false;

        private TexturePack texturePack;

        /// <summary>
        /// Gets the current texture pack.
        /// </summary>
        /// <value>The texture pack.</value>
        public TexturePack TexturePack
        {
            get
            {
                return texturePack;
            }
        }

        private static GridManager _instance;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static GridManager instance
        {
            get
            {
                return _instance;
            }
        }

        void Awake()
        {
            _instance = GetComponent<GridManager>();
        }

        /// <summary>
        /// Generates the environment with specified seed. If you pass same value for seed it will generate same environment.
        /// </summary>
        /// <param name="seed">Seed.</param>
        /// <param name="firstLevel">If set to <c>true</c> then player speech is started.</param>
        public void GenerateWithSeed(int seed, bool firstLevel)
        {
            if (parent == null)
            {
                parent = new GameObject("Cells");
            }

            if (!nodeClusterManager)
            {
                nodeClusterManager = GetComponent<NodeClusterManager>();
            }

            emptyFloorNodes.Clear();

            Random.InitState(seed);

            var texturePacks = GetEnabledTexturePacks();
            texturePack = texturePacks[Random.Range(0, texturePacks.Count)];
            Utilities.instance.SetTileSize(texturePack);

            Vector2 txtureSize = texturePack.GetSpriteSize();
            minDistanceBetweenStartAndEnd = ((GridSize.width * txtureSize.x) + (GridSize.height * txtureSize.y)) * 0.3f;

            float beginGeneratingTime = Time.realtimeSinceStartup;

            if (Utilities.instance.IsDebug)
                Debug.Log("Destroying old environment if present");

            DestroyEnvironment();

            if (Utilities.instance.IsDebug)
                Debug.Log("Generating environment");

            InitialiseEnvironment();

            for (int step = 0; step < NumberOfTransistionSteps; step++)
            {
                if (Utilities.instance.IsDebug)
                    Debug.Log("Performing transition step: " + (step + 1));
                PerformTransistionStep();
            }

            if (Utilities.instance.IsDebug)
                Debug.Log("Identifying clusters");
            // Identify clusters so they can be connected using Path manager 
            nodeClusterManager.IdentifyClusters(Grid, GridSize);

            if (IsConnectedEnvironment)
            {
                nodeClusterManager.ConnectClusters();

                // Need to re-identify main cavern to place enter and exit
                nodeClusterManager.IdentifyClusters(Grid, GridSize);
            }

            RemoveExtraneous();

            // Must be called before floor cells are cached.
            nodeClusterManager.CalculateMainCluster();

            CacheFloorCells();

            if (Utilities.instance.IsDebug)
                Debug.Log("Creating tiles");

            GenerateEnvironment();

            if (Utilities.instance.IsDebug)
                Debug.Log("Placing Entrance and Exit");

            PlaceEntranceAndExit();

            BuildExterior();

            if (firstLevel)
            {
                Events.instance.Raise(new LevelGeneratedSpeechRequired());
            }
            else
            {
                Events.instance.Raise(new LevelGeneratedEvent());
            }


            if (Utilities.instance.IsDebug)
                Debug.Log("Generated environment in " + (Time.realtimeSinceStartup - beginGeneratingTime) + " seconds");
        }

        /// <summary>
        /// Gets the floor node at a distance less than max distance from start node.
        /// </summary>
        /// <returns>The floor node.</returns>
        /// <param name="maxDistance">Maximum distance between start node and returned node.</param>
        /// <param name="startNode">Start node.</param>
        public Node GetFloorNodeMaxDistanceFromStartNode(float maxDistance, Node startNode)
        {
            var node = GetMaxDistanceNodeFromCluster(emptyFloorNodes, maxDistance, startNode);

            emptyFloorNodes.Remove(node);

            return node;

        }

        /// <summary>
        /// Gets the floor node at a distance greater than min distance from start node.
        /// </summary>
        /// <returns>The floor node.</returns>
        /// <param name="minDistance">Minimum distance between start node and returned node.</param>
        /// <param name="startNode">Start node.</param>
        public Node GetFloorNodeMinDistanceFromStartNode(float minDistance, Node startNode)
        {
            var node = GetMinDistanceNodeFromCluster(emptyFloorNodes, minDistance, startNode);
            emptyFloorNodes.Remove(node);

            return node;

        }

        /// <summary>
        /// Gets a random background node.
        /// </summary>
        /// <returns>The random background node.</returns>
        public Node GetRandomBackgroundNode()
        {
            List<Node> cluster = nodeClusterManager.MainCluster.Nodes;

            return cluster[(int)(Random.value * cluster.Count - 1)];

        }

        /// <summary>
        /// Returns a list of all background nodes.
        /// </summary>
        /// <returns>The background nodes.</returns>
        public List<Node> GetBackgroundNodes()
        {
            return nodeClusterManager.MainCluster.Nodes;
        }

        /// <summary>
        /// Gets a random floor node.
        /// </summary>
        /// <returns>The random floor node.</returns>
        public Node GetRandomFloorNode()
        {
            if (emptyFloorNodes.Count == 0)
                return null;

            var index = Random.Range(0, emptyFloorNodes.Count);

            var node = emptyFloorNodes[index];

            emptyFloorNodes.RemoveAt(index);

            return node;
        }

        /// <summary>
        /// Returns a count of the number of neighbours of the cell at the specified coord that are walls.
        /// </summary>
        /// <returns>The wall moore neighbours.</returns>
        /// <param name="coord">Coordinate.</param>
        public int CountWallMooreNeighbours(Vector2 coord)
        {
            int wallCount = 0;

            int x = (int)coord.x;
            int y = (int)coord.y;

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    // The middle point is the same as the passed Grid Coordinate, so skip it
                    if (i == 0 && j == 0)
                    {
                        continue;
                    }

                    Vector2 neighborCoordinate = new Vector2(x + i, y + j);

                    if (!Grid.IsValidCoordinate(neighborCoordinate))
                    {
                        wallCount += 1;
                    }
                    else if (Grid.GetNodeFromGridCoordinate(neighborCoordinate).NodeState != NodeType.Background)
                    {
                        wallCount += 1;
                    }
                }
            }
            return wallCount;
        }

        private List<TexturePack> GetEnabledTexturePacks()
        {
            var texturePacks = GetComponents<TexturePack>();

            List<TexturePack> returnPacks = new List<TexturePack>();

            foreach (var t in texturePacks)
            {
                if (t.Enabled)
                {
                    returnPacks.Add(t);
                }
            }

            if (returnPacks.Count == 0)
            {
                Debug.LogError("No enabled texturepacks found");
            }

            return returnPacks;

        }

        private void DestroyEnvironment()
        {
            ObjectManager.instance.RemoveObjects();
        }

        private void InitialiseEnvironment()
        {
            Grid = new NodeList(GridSize);

            for (int x = 0; x < GridSize.width; x++)
            {
                for (int y = 0; y < GridSize.height; y++)
                {


                    Vector2 coord = new Vector2(x, y);

                    NodeType cellType;

                    if (IsEdge(coord))
                    {
                        cellType = NodeType.Wall;
                    }
                    else
                    {
                        cellType = Random.value < ChanceToBecomeWall ? NodeType.Wall : NodeType.Background;
                    }

                    Grid.Add(new Node(coord, cellType));
                }
            }
        }

        private void PerformTransistionStep()
        {

            NodeList newGrid = new NodeList(GridSize);

            for (int x = 0; x < GridSize.width; x++)
            {
                for (int y = 0; y < GridSize.height; y++)
                {

                    Vector2 coord = new Vector2(x, y);

                    int neighbourCount = CountWallMooreNeighbours(coord);

                    Node oldCell = Grid.GetNodeFromGridCoordinate(coord);
                    Node newCell = new Node(coord);

                    if (oldCell.NodeState == NodeType.Wall)
                    {
                        newCell.NodeState = (neighbourCount < WallsToFloorConversion) ? NodeType.Background : NodeType.Wall;
                    }
                    else
                    {
                        newCell.NodeState = (neighbourCount > FloorsToWallConversion) ? NodeType.Wall : NodeType.Background;
                    }

                    newGrid.Add(newCell);
                }
            }

            Grid = newGrid;


        }

        private void RemoveExtraneous()
        {
            for (int x = 0; x < GridSize.width; x++)
            {
                for (int y = 0; y < GridSize.height; y++)
                {

                    var coord = new Vector2(x, y);
                    var node = Grid.GetNodeFromGridCoordinate(coord);

                    if (node.NodeState != NodeType.Wall)
                        continue;

                    if (IsExtraneousCell(node.Coordinates))
                    {
                        node.NodeState = NodeType.Background;
                    }
                }
            }

            RemoveLoneCells();
        }

        private void RemoveLoneCells()
        {
            for (int x = 0; x < GridSize.width; x++)
            {
                for (int y = 0; y < GridSize.height; y++)
                {

                    var coord = new Vector2(x, y);
                    var node = Grid.GetNodeFromGridCoordinate(coord);

                    if (node.NodeState != NodeType.Wall)
                        continue;

                    if (IsLoneCell(node.Coordinates))
                    {
                        node.NodeState = NodeType.Background;
                    }
                }
            }
        }

        private void CacheFloorCells()
        {
            var mainCluster = nodeClusterManager.MainCluster;

            foreach (var node in mainCluster.Nodes)
            {
                if (IsFloorCell(node.Coordinates))
                {
                    emptyFloorNodes.Add(node);
                }
            }
        }

        private bool IsFloorCell(Vector2 coord)
        {
            var currentCell = Grid.ContainsNodeTypeAtPosition(coord, NodeType.Background);
            var cellBelow = (Grid.ContainsNodeTypeAtPosition(new Vector2(coord.x, coord.y - 1), NodeType.Wall));
            var floorLeft = (Grid.ContainsNodeTypeAtPosition(new Vector2(coord.x - 1, coord.y), NodeType.Background));
            var floorRight = (Grid.ContainsNodeTypeAtPosition(new Vector2(coord.x + 1, coord.y), NodeType.Background));
            var floorAbove = (Grid.ContainsNodeTypeAtPosition(new Vector2(coord.x, coord.y + 1), NodeType.Background));

            return currentCell && cellBelow && floorLeft && floorRight && floorAbove;

        }

        private bool IsLoneCell(Vector2 coord)
        {
            var cellBelow = (Grid.ContainsNodeTypeAtPosition(new Vector2(coord.x, coord.y - 1), NodeType.Wall));
            var cellLeft = (Grid.ContainsNodeTypeAtPosition(new Vector2(coord.x - 1, coord.y), NodeType.Wall));
            var cellRight = (Grid.ContainsNodeTypeAtPosition(new Vector2(coord.x + 1, coord.y), NodeType.Wall));
            var cellAbove = (Grid.ContainsNodeTypeAtPosition(new Vector2(coord.x, coord.y + 1), NodeType.Wall));

            if (!cellLeft && !cellRight && !cellBelow && !cellAbove)
                return true;

            return false;
        }

        private bool IsExtraneousCell(Vector2 coord)
        {
            var cellBelow = (Grid.ContainsNodeTypeAtPosition(new Vector2(coord.x, coord.y - 1), NodeType.Wall));
            var cellLeft = (Grid.ContainsNodeTypeAtPosition(new Vector2(coord.x - 1, coord.y), NodeType.Wall));
            var cellRight = (Grid.ContainsNodeTypeAtPosition(new Vector2(coord.x + 1, coord.y), NodeType.Wall));
            var cellAbove = (Grid.ContainsNodeTypeAtPosition(new Vector2(coord.x, coord.y + 1), NodeType.Wall));

            var floorBelow = (Grid.ContainsNodeTypeAtPosition(new Vector2(coord.x, coord.y - 1), NodeType.Background));
            var floorLeft = (Grid.ContainsNodeTypeAtPosition(new Vector2(coord.x - 1, coord.y), NodeType.Background));
            var floorRight = (Grid.ContainsNodeTypeAtPosition(new Vector2(coord.x + 1, coord.y), NodeType.Background));
            var floorAbove = (Grid.ContainsNodeTypeAtPosition(new Vector2(coord.x, coord.y + 1), NodeType.Background));


            if (!cellLeft && !cellRight && !cellBelow && !cellAbove)
                return true;

            if (!cellLeft && !cellAbove && !cellRight)
                return true;

            if (!cellLeft && !cellAbove && !cellBelow)
                return true;

            if (!cellBelow && !cellAbove && !cellRight)
                return true;

            if (!cellBelow && !cellLeft && !cellRight)
                return true;

            if (floorLeft && floorRight && cellAbove && cellBelow)
                return true;

            if (floorAbove && floorBelow && cellRight && cellLeft)
                return true;


            return false;
        }

        private void GenerateEnvironment()
        {
            for (int x = 0; x < GridSize.width; x++)
            {
                for (int y = 0; y < GridSize.height; y++)
                {

                    var coord = new Vector2(x, y);
                    var node = Grid.GetNodeFromGridCoordinate(coord);

                    if (!DrawBackground && node.NodeState == NodeType.Background)
                    {
                        continue;
                    }


                    var cell = ObjectManager.instance.GetObject(Cell.name, Utilities.instance.GetNodePosition(node));

                    var cellCollider = GetCellCollider(cell);

                    int sortingOrder = 0;

                    int layerMask = LayerMask.NameToLayer("Default");

                    if (node.NodeState == NodeType.Background || node.NodeState == NodeType.Entry || node.NodeState == NodeType.Exit)
                    {
                        cellCollider.enabled = true;
                        cellCollider.isTrigger = true;

                    }
                    else
                    {
                        DefineWallType(node);

                        cellCollider.enabled = true;
                        cellCollider.isTrigger = false;
                        sortingOrder = 5;

                        layerMask = LayerMask.NameToLayer("Cave");
                    }

                    Utilities.instance.UpdateNodeSortingOrder(SCRIPT_NAME, cell, sortingOrder);
                    cell.layer = layerMask;

                    UpdateNodeSprite(cell, node.NodeState);
                    node.Position = cell.transform.position;
                    cell.transform.SetParent(parent.transform);
                    cell.SetActive(true);
                    cell.isStatic = true;

                    nodes.Add(cell);


                }
            }

        }

        private Collider2D GetCellCollider(GameObject cell)
        {
            var collider2D = cell.GetComponent<Collider2D>();

            if (!collider2D)
            {
                collider2D = cell.AddComponent<Collider2D>();
            }

            return collider2D;
        }

        private void DefineWallType(Node node)
        {
            var coord = node.Coordinates;
            var floorBelow = (Grid.ContainsNodeTypeAtPosition(new Vector2(coord.x, coord.y - 1), NodeType.Background));
            var floorLeft = (Grid.ContainsNodeTypeAtPosition(new Vector2(coord.x - 1, coord.y), NodeType.Background));
            var floorRight = (Grid.ContainsNodeTypeAtPosition(new Vector2(coord.x + 1, coord.y), NodeType.Background));
            var floorAbove = (Grid.ContainsNodeTypeAtPosition(new Vector2(coord.x, coord.y + 1), NodeType.Background));

            if (!floorBelow && !floorLeft && floorAbove && floorRight)
            {
                node.NodeState = NodeType.WallTopRight;
            }
            else if (!floorBelow && !floorLeft && floorAbove && !floorRight)
            {
                node.NodeState = NodeType.WallTopMiddle;
            }
            else if (!floorBelow && floorLeft && floorAbove && !floorRight)
            {
                node.NodeState = NodeType.WallTopLeft;
            }
            else if (floorBelow && !floorLeft && !floorAbove && floorRight)
            {
                node.NodeState = NodeType.WallBottomRight;
            }
            else if (floorBelow && !floorLeft && !floorAbove && !floorRight)
            {
                node.NodeState = NodeType.WallBottomMiddle;
            }
            else if (floorBelow && floorLeft && !floorAbove && !floorRight)
            {
                node.NodeState = NodeType.WallBottomLeft;
            }
            else if (!floorBelow && !floorAbove && floorLeft && !floorRight)
            {
                node.NodeState = NodeType.WallMiddleLeft;
            }
            else if (!floorBelow && !floorAbove && !floorLeft && floorRight)
            {
                node.NodeState = NodeType.WallMiddleRight;
            }
            else
            {
                node.NodeState = NodeType.WallMiddle;
            }

        }

        private void UpdateNodeSprite(GameObject node, NodeType nodeType)
        {
            SpriteRenderer spriteRenderer = Utilities.instance.GetChildComponent<SpriteRenderer>(SCRIPT_NAME, node.transform);

            if (spriteRenderer)
            {
                spriteRenderer.sprite = texturePack.GetSpriteFromCellType(nodeType);
            }

            /*
			if (!spriteRenderer.enabled) {
				spriteRenderer.enabled = true;
			}
			*/


        }

        private bool IsEdge(Vector2 coordinate)
        {
            return ((int)coordinate.x == 0 ||
            (int)coordinate.x == (int)GridSize.width - 1 ||
            (int)coordinate.y == 0 ||
            (int)coordinate.y == (int)GridSize.height - 1);
        }

        private void PlaceEntranceAndExit()
        {
            var entranceCell = GetRandomFloorNode();

            const int numOfTilesToRemove = 4;

            for (int i = -numOfTilesToRemove / 2; i <= numOfTilesToRemove / 2; i++)
            {

                if (i == 0)
                {
                    continue;
                }

                var coord = entranceCell.Coordinates + new Vector2(i, 0);
                var nodeToRemove = Grid.GetNodeFromGridCoordinate(coord);

                int index = emptyFloorNodes.IndexOf(nodeToRemove);

                if (index != -1)
                {
                    emptyFloorNodes.RemoveAt(index);
                }
            }

            Grid.GetNodeFromGridCoordinate(entranceCell.Coordinates).NodeState = NodeType.Entry;
            StartNode = entranceCell;

            Node exitCell = GetFloorNodeMinDistanceFromStartNode(minDistanceBetweenStartAndEnd, entranceCell);

            Grid.GetNodeFromGridCoordinate(exitCell.Coordinates).NodeState = NodeType.Exit;

            EndNode = exitCell;
        }

        private bool IsValidEntrance(Node node)
        {
            var coord = node.Coordinates;
            var floorAbove = (Grid.ContainsNodeTypeAtPosition(new Vector2(coord.x, coord.y + 1), NodeType.Background));
            var cellBelow = (Grid.ContainsNodeTypeAtPosition(new Vector2(coord.x, coord.y - 1), NodeType.Wall));

            return floorAbove && cellBelow;
        }

        private Node GetMinDistanceNodeFromCluster(List<Node> cluster, float minDistance, Node startNode)
        {
            cluster.Shuffle();

            const int attempts = 3;

            var startPos = Utilities.instance.GetNodePosition(startNode);

            Node retNode = null;

            int currentAttempts = 0;
            while (retNode == null)
            {

                foreach (var node in cluster)
                {
                    if (Vector2.Distance(startPos, Utilities.instance.GetNodePosition(node)) >= minDistance)
                    {
                        retNode = node;
                        break;
                    }
                }

                minDistance *= 0.8f;

                if (++currentAttempts > attempts)
                {
                    break;
                }
            }

            if (retNode == null)
            {
                Debug.LogWarning("No node found with a min distance of " + minDistance + " returning random node");

                return cluster[Random.Range(0, cluster.Count - 1)];
            }

            return retNode;

        }

        private Node GetMaxDistanceNodeFromCluster(List<Node> cluster, float maxDistance, Node startNode)
        {
            int mainClusterCount = cluster.Count - 1;

            float currentDistance = 0f;

            Node node = null;

            int count = 0;
            do
            {
                // Do not want an infinite loop, where you cannot find a node far enough away so we decrement the distance until a node is found.
                count++;
                if (count == 10)
                {
                    if (maxDistance < 40f)
                    {
                        maxDistance += (maxDistance * 0.1f);
                    }

                    count = 0;
                }

                node = cluster[(int)(Random.value * mainClusterCount)];

                int a = (int)(node.Coordinates.x - startNode.Coordinates.x);
                int b = (int)(node.Coordinates.y - startNode.Coordinates.y);
                currentDistance = Mathf.Sqrt(a * a + b * b);

            } while (currentDistance > maxDistance);

            return node;
        }

        private List<Node> GetTopRow()
        {
            var row = new List<Node>();

            for (int i = 0; i < GridSize.width; i++)
            {
                row.Add(Grid.GetNodeFromGridCoordinate(new Vector2(i, GridSize.height - 1)));
            }

            return row;
        }

        private List<Node> GetBottomRow()
        {
            var row = new List<Node>();

            for (int i = 0; i < GridSize.width; i++)
            {
                row.Add(Grid.GetNodeFromGridCoordinate(new Vector2(i, 0)));
            }

            return row;
        }

        private List<Node> GetLeftRow()
        {
            var row = new List<Node>();

            for (int i = 0; i < GridSize.height; i++)
            {
                row.Add(Grid.GetNodeFromGridCoordinate(new Vector2(0, i)));
            }

            return row;
        }

        private List<Node> GetRightRow()
        {
            var row = new List<Node>();

            for (int i = 0; i < GridSize.height; i++)
            {
                row.Add(Grid.GetNodeFromGridCoordinate(new Vector2(GridSize.width - 1, i)));
            }

            return row;
        }


        private void BuildTopExterior(Vector2 tileSize)
        {
            var topNodes = GetTopRow();

            // Top left.

            var topLeft = ObjectManager.instance.GetObject(Cell.name,
                              Utilities.instance.GetNodePosition(topNodes[0]) + (new Vector2(-tileSize.x, tileSize.y)));
            var topLeftRend = topLeft.GetComponent<SpriteRenderer>();
            topLeftRend.sortingOrder = 5;
            topLeftRend.sprite = texturePack.GetSpriteFromCellType(NodeType.WallTopLeft);
            topLeft.transform.SetParent(parent.transform);
            topLeft.isStatic = true;

        
            // Top right.
            var topRight = ObjectManager.instance.GetObject(Cell.name,
                               Utilities.instance.GetNodePosition(topNodes[topNodes.Count - 1]) + (new Vector2(tileSize.x, tileSize.y)));
            var topRightRend = topRight.GetComponent<SpriteRenderer>();
            topRightRend.sortingOrder = 5;
            topRightRend.sprite = texturePack.GetSpriteFromCellType(NodeType.WallTopRight);
            topRight.transform.SetParent(parent.transform);
            topRight.isStatic = true;

            const int wallHeight = 1;

            var topMidSprite = texturePack.GetSpriteFromCellType(NodeType.WallTopMiddle);
            var wallMidSprite = texturePack.GetSpriteFromCellType(NodeType.WallMiddle);

            for (int i = 0; i < topNodes.Count; i++)
            {

                for (int j = 0; j < wallHeight; j++)
                {
                    var exterior = ObjectManager.instance.GetObject(Cell.name,
                                       Utilities.instance.GetNodePosition(topNodes[i]) + (new Vector2(0, tileSize.y * (j + 1))));
                    var exteriorRend = exterior.GetComponent<SpriteRenderer>();
                    exteriorRend.sortingOrder = 5;
                    exterior.transform.SetParent(parent.transform);
                    exterior.isStatic = true;

                    if (j == wallHeight - 1)
                    {
                        exterior.GetComponent<SpriteRenderer>().sprite = topMidSprite;
                    }
                    else
                    {
                        exterior.GetComponent<SpriteRenderer>().sprite = wallMidSprite;
                    }
                }

            }
        }

        private void BuildBottomExterior(Vector2 tileSize)
        {
            var bottomNodes = GetBottomRow();

            // Bottom left.
            var bottomLeft = ObjectManager.instance.GetObject(Cell.name,
                                 Utilities.instance.GetNodePosition(bottomNodes[0]) + (new Vector2(-tileSize.x, -tileSize.y)));
            var btmLeftRend = bottomLeft.GetComponent<SpriteRenderer>();
            btmLeftRend.sortingOrder = 5;
            btmLeftRend.sprite = texturePack.GetSpriteFromCellType(NodeType.WallBottomLeft);
            bottomLeft.transform.SetParent(parent.transform);
            bottomLeft.isStatic = true;

            // Bottom right.
            var bottomRight = ObjectManager.instance.GetObject(Cell.name,
                                  Utilities.instance.GetNodePosition(bottomNodes[bottomNodes.Count - 1]) + (new Vector2(tileSize.x, -tileSize.y)));
            var btmRightRend = bottomRight.GetComponent<SpriteRenderer>();
            btmRightRend.sortingOrder = 5;
            btmRightRend.sprite = texturePack.GetSpriteFromCellType(NodeType.WallBottomRight);
            bottomRight.transform.SetParent(parent.transform);
            bottomRight.isStatic = true;

            var bottomMidSprite = texturePack.GetSpriteFromCellType(NodeType.WallBottomMiddle);
            for (int i = 0; i < bottomNodes.Count; i++)
            {
                var exterior = ObjectManager.instance.GetObject(Cell.name,
                                   Utilities.instance.GetNodePosition(bottomNodes[i]) + (new Vector2(0, -tileSize.y)));

                var exteriorRend = exterior.GetComponent<SpriteRenderer>();
                exteriorRend.sortingOrder = 5;
                exteriorRend.sprite = bottomMidSprite;
                exterior.transform.SetParent(parent.transform);
                exterior.isStatic = true;
            }
        }

        private void BuildLeftExterior(Vector2 tileSize)
        {
            var leftSprite = texturePack.GetSpriteFromCellType(NodeType.WallMiddleLeft);

            var leftRow = GetLeftRow();

            for (int i = 0; i < leftRow.Count; i++)
            {
                var exterior = ObjectManager.instance.GetObject(Cell.name,
                                   Utilities.instance.GetNodePosition(leftRow[i]) + (new Vector2(-tileSize.x, 0)));

                var exteriorRend = exterior.GetComponent<SpriteRenderer>();
                exteriorRend.sortingOrder = 5;
                exteriorRend.sprite = leftSprite;
                exterior.transform.SetParent(parent.transform);
                exterior.isStatic = true;
            }
        }

        private void BuildRightExterior(Vector2 tileSize)
        {
            var rightSprite = texturePack.GetSpriteFromCellType(NodeType.WallMiddleRight);

            var rightRow = GetRightRow();
            for (int i = 0; i < rightRow.Count; i++)
            {
                var exterior = ObjectManager.instance.GetObject(Cell.name,
                                   Utilities.instance.GetNodePosition(rightRow[i]) + (new Vector2(tileSize.x, 0)));


                var exteriorRend = exterior.GetComponent<SpriteRenderer>();
                exteriorRend.sortingOrder = 5;
                exteriorRend.sprite = rightSprite;
                exterior.transform.SetParent(parent.transform);
                exterior.isStatic = true;
            }
        }

        private void BuildExterior()
        {
            var tileSize = Utilities.instance.TileSize.Value;

            BuildTopExterior(tileSize);
            BuildBottomExterior(tileSize);
            BuildLeftExterior(tileSize);
            BuildRightExterior(tileSize);
        }

    }
}
