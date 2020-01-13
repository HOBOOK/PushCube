using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;

/// <summary>
/// 0:범위밖 1:빈공간 2:목표지점 3:벽 4:박스 5:목표지점 위 박스 6:플레이어
/// </summary>
public class MapGrid : MonoBehaviour
{
    public Material normalCubeMat, goalCubeMat;
    public Transform MapTransform;
    public GameObject groundBlock;
    public List<GameObject> blockPrefabs = new List<GameObject>();
    public Transform Player;

    public float nodeRadius;

    float nodeDiameter;
    int gridSizeX,  gridSizeY;
    Vector2 gridMapSize;
    Vector3 worldBottomLeft;
    MapNode[,] grid;
    Transform[,] blocks;

    int[,] MapData;
    int needClearNodeCount;
    int currentClearNodeCount;

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
    }

    void CreatePlayerObject()
    {
        if(Player!=null)
        {
            Destroy(Player.gameObject);
        }
        GameObject obj = Instantiate(GameAssetsManager.instance.GetCurrentCharacterAsset().gameObject);
        obj.gameObject.SetActive(false);
        Player = obj.transform;
        Controller.instance.Target = Player.transform;
    }

    public void CreateMap(int stage)
    {
        CreatePlayerObject();
        GetMapData(stage);
        gridMapSize.x = gridSizeX * nodeDiameter;
        gridMapSize.y = gridSizeY * nodeDiameter;
        Player.gameObject.SetActive(false);
        StartCoroutine("CreateGrid");
    }

    // 그리드 생성
    IEnumerator CreateGrid()
    {
        yield return StartCoroutine("ClearGrid");
        needClearNodeCount = 0;
        currentClearNodeCount = 0;
        grid = new MapNode[gridSizeX, gridSizeY];
        blocks = new Transform[gridSizeX, gridSizeY];
        worldBottomLeft = MapTransform.position - Vector3.right * gridMapSize.x / 2 - Vector3.forward * gridMapSize.y / 2;
        for(int x = 0; x < gridSizeX; x++)
        {
            for(int y = 0; y < gridSizeY; y++)
            {
                int type = MapData[x, y];
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);

                if (type == 6)//플레이어
                {
                    Player.transform.position = worldPoint+new Vector3(0,0.2f,0);
                    type = 1;
                }
                else if(type == 7)//골인지점 위 플레이어
                {
                    Player.transform.position = worldPoint + new Vector3(0, 0.2f, 0);
                    type = 2;
                }

                if (type==2||type==5) // 골인 지점
                {
                    if(type==5)
                    {
                        GameObject block = Instantiate(blockPrefabs[2], MapTransform);
                        block.transform.position = worldPoint + new Vector3(0, 3, 0);
                        SetCubeMat(block.transform, true);
                        block.gameObject.SetActive(true);
                        StartCoroutine(CreateBlockAnimation(block.transform, worldPoint));
                        blocks[x, y] = block.transform;
                        currentClearNodeCount++;
                    }
                    needClearNodeCount++;
                    GameObject gBlock = Instantiate(blockPrefabs[0], MapTransform);
                    gBlock.transform.position = worldPoint + new Vector3(0, -nodeDiameter/2, 0);
                    gBlock.gameObject.SetActive(true);
                }
                else if (type == 0) // 범위 밖 지점
                {

                }
                else // 1:빈공간 3:벽 4:푸쉬큐브
                {
                    if (type == 3) 
                    {
                        GameObject block = Instantiate(blockPrefabs[1], MapTransform);
                        block.transform.position = worldPoint + new Vector3(0, 3, 0);
                        block.gameObject.SetActive(true);
                        StartCoroutine(CreateBlockAnimation(block.transform, worldPoint));
                        blocks[x, y] = block.transform;
                    }
                    else if(type == 4)
                    {
                        // 골드 큐브 or 일반 큐브 생성
                        GameObject block = IsCreateGoldCube() ? Instantiate(blockPrefabs[3], MapTransform) : Instantiate(blockPrefabs[2], MapTransform);
                        block.transform.position = worldPoint + new Vector3(0, 3, 0);
                        SetCubeMat(block.transform, false);
                        block.gameObject.SetActive(true);
                        StartCoroutine(CreateBlockAnimation(block.transform, worldPoint));
                        blocks[x, y] = block.transform;
                    }
                    GameObject gBlock = Instantiate(groundBlock, MapTransform);
                    gBlock.transform.position = worldPoint + new Vector3(0, -nodeDiameter / 2, 0);
                    gBlock.gameObject.SetActive(true);
                }
                grid[x, y] = new MapNode(type, x, y, worldPoint);

            }
            yield return new WaitForEndOfFrame();
        }
        yield return null;
        StartCoroutine("CreatePlayer");
    }

    IEnumerator CreatePlayer()
    {
        Player.gameObject.SetActive(true);
        Vector3 refScale = Player.transform.localScale;
        float scale = .0f;
        while (scale < refScale.x)
        {
            scale += 2 * Time.deltaTime;
            Player.localScale = new Vector3(scale, scale, scale);
            yield return new WaitForEndOfFrame();
        }
        Player.localScale = refScale;
        Player.GetComponentInChildren<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        Camera.main.GetComponent<CameraFollow>().SetFollowTarget(Player);
    }
    IEnumerator CreateBlockAnimation(Transform t, Vector3 targetPos)
    {
        while (t.position != targetPos)
        {
            t.position = Vector3.MoveTowards(t.position, targetPos, 10 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator ClearGrid()
    {
        List<Transform> blocks = new List<Transform>();
        for(int i = 0; i < MapTransform.childCount; i++)
        {
            blocks.Add(MapTransform.GetChild(i));
        }
        if(blocks!=null&&blocks.Count>0)
        {
            float time = .0f;
            while (time < 1.0f)
            {
                foreach (var t in blocks)
                {
                    t.Translate(Vector3.down * 0.1f);
                    t.localScale = new Vector3(Mathf.Clamp(t.localScale.x-time,0,1), Mathf.Clamp(t.localScale.y-time,0,1), Mathf.Clamp(t.localScale.z-time,0,1));

                }
                time += 5*Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            foreach (var t in blocks)
            {
                Destroy(t.gameObject);
            }
        }
        yield return null;
    }

    // 블럭이동
    public void MoveBlock(Vector3 fromPos, Vector3 toPos)
    {
        fromPos.y = 0;
        toPos.y = 0;
        MapNode FromNode = GetNodeFromWorldPoint(fromPos);
        MapNode ToNode = GetNodeFromWorldPoint(toPos);
        MapNode newToNode = new MapNode(4, ToNode.x, ToNode.y, ToNode.worldPosition);
        MapNode newFromNode = new MapNode(FromNode.type == 5 ? 2 : 1, FromNode.x, FromNode.y, FromNode.worldPosition);

        SetNodeFromWorldPoint(newToNode);
        SetNodeFromWorldPoint(newFromNode);

        Transform trans = GetBlockFromWorldPoint(fromPos);
        SetCubeMat(trans, false);
        StartCoroutine(Moving(trans, toPos));
        SetBlockFromWorldPoint(toPos, trans);
        SetBlockFromWorldPoint(fromPos, null);
    }

    // 목표지점으로 블록이동
    public void GoalBlock(Vector3 fromPos, Vector3 toPos)
    {
        fromPos.y = 0;
        toPos.y = 0;
        MapNode FromNode = GetNodeFromWorldPoint(fromPos);
        MapNode ToNode = GetNodeFromWorldPoint(toPos);
        MapNode newToNode = new MapNode(5, ToNode.x, ToNode.y, ToNode.worldPosition);
        MapNode newFromNode = new MapNode(FromNode.type==5?2:1, FromNode.x, FromNode.y, FromNode.worldPosition);
        SetNodeFromWorldPoint(newToNode);
        SetNodeFromWorldPoint(newFromNode);

        Transform trans = GetBlockFromWorldPoint(fromPos);
        SetCubeMat(trans, true);
        StartCoroutine(Goaling(trans, toPos));
        SetBlockFromWorldPoint(toPos, trans);
        SetBlockFromWorldPoint(fromPos, null);
    }

    IEnumerator Moving(Transform target, Vector3 targetPos)
    {
        while (target.position != targetPos)
        {
            target.position = Vector3.MoveTowards(target.position, targetPos, 5* Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
    IEnumerator Goaling(Transform target, Vector3 targetPos)
    {
        while (target.position != targetPos)
        {
            target.position = Vector3.MoveTowards(target.position, targetPos, 10 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        float scale = target.localScale.x * 2;
        while(scale>1)
        {
            target.localScale = new Vector3(scale, scale, scale);
            scale -= (10 - scale) * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        target.localScale = Vector3.one;
        //target.gameObject.SetActive(false);
    }


    // 맵 데이터 가져오는 함수
    void GetMapData(int stage)
    {
        string[] data = MapDataManager.GetMapData(stage).Split(',');
        gridSizeX = data.GetLength(0);
        gridSizeY = data[0].Length;
        Debug.Log(gridSizeX + "x" + gridSizeY + " 맵 불러옴");
        int[,] mapData = new int[gridSizeX, gridSizeY];
        for(int x=0; x < gridSizeY; x++)
        {
            for(int y = 0; y < gridSizeX; y++)
            {
                mapData[x, y] = data[x][y]-'0';
            }
        }
        MapData = mapData;
    }

    int GetSolveCount()
    {
        return 0;
    }

    // 위치에 맞는 노드 가져오기
    public MapNode GetNodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + (gridMapSize.x-nodeDiameter) / 2) / gridMapSize.x;
        float percentY = (worldPosition.z + (gridMapSize.y- nodeDiameter) / 2) / gridMapSize.y;

        if (percentX >= 1 || percentY >= 1 || percentX < 0 || percentY < 0)
            return null;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX) * percentX);
        int y = Mathf.RoundToInt((gridSizeY) * percentY);
        return grid[x, y];
    }
    public void SetNodeFromWorldPoint(MapNode node)
    {
        grid[node.x, node.y] = node;
    }

    // 위치에 맞는 블럭 가져오기
    public Transform GetBlockFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + (gridMapSize.x - nodeDiameter) / 2) / gridMapSize.x;
        float percentY = (worldPosition.z + (gridMapSize.y - nodeDiameter) / 2) / gridMapSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX) * percentX);
        int y = Mathf.RoundToInt((gridSizeY) * percentY);
        return blocks[x, y];
    }
    public void SetBlockFromWorldPoint(Vector3 worldPosition, Transform t)
    {
        float percentX = (worldPosition.x + (gridMapSize.x - nodeDiameter) / 2) / gridMapSize.x;
        float percentY = (worldPosition.z + (gridMapSize.y - nodeDiameter) / 2) / gridMapSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX) * percentX);
        int y = Mathf.RoundToInt((gridSizeY) * percentY);
        blocks[x, y] = t;
    }

    public bool IsClearable()
    {
        currentClearNodeCount = 0;
        for(int i = 0; i < grid.GetLength(0); i++)
        {
            for(int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i, j].type == 5)
                    currentClearNodeCount++;
            }
        }
        if (currentClearNodeCount == needClearNodeCount)
            return true;
        return false;
    }

    void SetCubeMat(Transform transform, bool isGoal)
    {
        if(!transform.name.Contains("Gold"))
        {
            foreach (var mr in transform.GetComponentsInChildren<MeshRenderer>())
            {
                if (isGoal)
                {
                    mr.material = goalCubeMat;
                }
                else
                {
                    mr.material = normalCubeMat;
                }
            }
        }
    }

    bool IsCreateGoldCube()
    {
        int random = UnityEngine.Random.Range(1, 2);
        if(GameManager.instance.GoldCubeCount > random)
        {
            GameManager.instance.ResetGoldCubeCount();
            return true;
        }
        return false;
    }
}
