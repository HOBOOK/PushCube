using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class StartManager : MonoBehaviour
{
    public static StartManager Instance;

    #region 맵 생성 변수
    public Transform MapTransform;
    public GameObject groundBlock;
    public List<GameObject> blockPrefabs = new List<GameObject>();
    public float nodeRadius;
    float nodeDiameter;
    int gridSizeX,  gridSizeY;
    Vector2 gridMapSize;
    Vector3 worldBottomLeft;
    MapNode[,] grid;
    Transform[,] blocks;
    int[,] MapData;
    #endregion

    #region
    public Image Fade;
    public Button StartButton;
    public Transform buttonsParentTransform;
    public Transform characterParentTransform;
    int hobookGamesClickCount;
    bool isGameReady = false;
    #endregion

    #region 캐릭터 생성 변수
    public Transform CharacterTransform;
    int charGridSizeX, charGridSizeY;
    MapNode[,] charGrid;
    Vector2 charGridMapSize;
    Vector3 charWorldBottomLeft;
    List<Transform> playerCharacters = new List<Transform>();
    Dictionary<int, Transform> playerHavingCharacterDictionary = new Dictionary<int, Transform>();
    #endregion

    private void Awake()
    {
        Instance = this;
        SoundManager.Initialize();
        nodeDiameter = nodeRadius * 2;
        RenderSettings.skybox.SetColor("_TintColor", new Color32(255, 250, 30, 255));
        if ((Screen.height / Screen.width)>=2)
        {
            Camera.main.orthographicSize = 8;
        }
        else
        {
            Camera.main.orthographicSize = 7;
        }
    }
    private void Start()
    {
        StartCoroutine("Starting");
    }

    private void Update()
    {
        SelectCharacter();
    }

    IEnumerator Starting()
    {
        isGameReady = false;
        StartCoroutine("Loading");
        LoadData();
        yield return new WaitForFixedUpdate();
        LocalizationManager.LoadLanguage(ConfigurationData.language);
        CreateMap();
        CreateCharGrid();
        SoundManager.PlayLoopSound(SoundManager.Sound.Loop0, true);
        hobookGamesClickCount = 0;
        AdMobManager.Instance.StartAdMob();
        IAPManager.Instance.InitStore();
        yield return new WaitForSeconds(.3f);
        isGameReady = true;
        GameSystemManager.Instance.isPlayerDataLoad = true;
        StartCoroutine("OnGameStartButton");
        AttendanceCheck();
        SoundManager.PlaySound(SoundManager.Sound.Ready);
        yield return new WaitForSeconds(.5f);
        CheckGetSpecialCharacter();
        yield return null;
    }
    IEnumerator Loading()
    {
        buttonsParentTransform.gameObject.SetActive(false);
        characterParentTransform.gameObject.SetActive(false);
        StartButton.GetComponent<Button>().enabled = false;
        Text text = StartButton.GetComponentInChildren<Text>();
        text.text = "";
        int cnt = 0;
        while(!isGameReady)
        {
            text.text = "Loading";
            for(int i = 0; i < cnt; i++)
            {
                text.text += ".";
            }
            for(int i = cnt; i < 3; i++)
            {
                text.text += " ";
            }
            cnt = cnt > 3 ? 0 : cnt + 1;
            yield return new WaitForSeconds(.1f);
        }
    }

    IEnumerator OnGameStartButton()
    {
        buttonsParentTransform.gameObject.SetActive(true);
        characterParentTransform.gameObject.SetActive(true);
        StartButton.GetComponent<Button>().enabled = true;
        StartButton.GetComponentInChildren<Text>().text = LocalizationManager.GetText("StartScene_GameStart");
        float time = 0f;
        while(time<1.0f)
        {
            foreach (Text child in buttonsParentTransform.GetComponentsInChildren<Text>())
                child.color = new Color(1, 1, 1, time);
            foreach (Image child in buttonsParentTransform.GetComponentsInChildren<Image>())
                child.color = new Color(child.color.r, child.color.g, child.color.b, time);
            time += 2.5f * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        foreach (Text child in buttonsParentTransform.GetComponentsInChildren<Text>())
            child.color = new Color(1, 1, 1, 1);
        foreach (Image child in buttonsParentTransform.GetComponentsInChildren<Image>())
            child.color = new Color(child.color.r, child.color.g, child.color.b, 1);

    }

    public void OnClickGameStart()
    {
        if(isGameReady)
        {
            StartCoroutine("GameStartAnimation");
        }
    }

    IEnumerator GameStartAnimation()
    {
        SoundManager.PlaySound(SoundManager.Sound.GameStart);
        Fade.gameObject.SetActive(true);
        Color startcolor = PushCubeColor.ThemeMainColor;
        float alpha = .0f;
        while(alpha<1)
        {
            Fade.color = new Color(startcolor.r, startcolor.g, startcolor.b, alpha);
            alpha += 3 * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        SceneManager.LoadSceneAsync(2);
    }

    void LoadData()
    {
        PlayerDataSystem.Load();
        ConfigurationDataSystem.Load();
        ItemManager.Instance.LoadItemData();
    }

    void AttendanceCheck()
    {
        if(PlayerData.currentStage>0)
        {
            DateTime nowTime = DateTime.Now;
            if (PlayerPrefs.HasKey("AttendanceDay"))
            {
                if (nowTime.Day != PlayerPrefs.GetInt("AttendanceDay"))
                {
                    Debug.Log(nowTime.Day + " 출석!");
                    PlayerPrefs.SetInt("AttendanceDay", nowTime.Day);
                    PlayerData.AddGoldCube(1);
                    GameSystemManager.ShowAlertMessage(new AlertMessage(1, LocalizationManager.GetText("Alert_AttendanceContinue"), GameAssetsManager.instance.SpriteAssets.Find(x => x.name == "GoldCube").sprite, null, null));
                }
            }
            else
            {
                Debug.Log(nowTime.Day + " 새로운 출석 기록!");
                PlayerPrefs.SetInt("AttendanceDay", nowTime.Day);
                PlayerData.AddGoldCube(2);
                GameSystemManager.ShowAlertMessage(new AlertMessage(1, LocalizationManager.GetText("Alert_Attendance"), GameAssetsManager.instance.SpriteAssets.Find(x => x.name == "GoldCube").sprite, null, null));
            }
        }
    }

    #region 타이틀 맵 생성
    public void CreateMap()
    {
        GetStartSceneMapData();
        gridMapSize.x = gridSizeX * nodeDiameter;
        gridMapSize.y = gridSizeY * nodeDiameter;
        StartCoroutine("CreateGrid");
    }

    // 그리드 생성
    IEnumerator CreateGrid()
    {
        int cubeCount = 0;
        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        grid = new MapNode[gridSizeX, gridSizeY];
        blocks = new Transform[gridSizeX, gridSizeY];
        worldBottomLeft = MapTransform.position - Vector3.right * gridMapSize.x / 2 - Vector3.forward * gridMapSize.y / 2;
        for(int x = 0; x < gridSizeX; x++)
        {
            for(int y = 0; y < gridSizeY; y++)
            {
                int type = MapData[x, y];
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                if( type !=0)
                {
                    if (cubeCount< PlayerData.currentStage)
                    {
                        GameObject block = Instantiate(blockPrefabs[2], MapTransform);
                        block.transform.position = worldPoint + new Vector3(0, 3, 0);
                        block.gameObject.SetActive(true);
                        StartCoroutine(CreateBlockAnimation(block.transform, worldPoint));
                        blocks[x, y] = block.transform;
                    }
                    else
                    {
                        GameObject block = Instantiate(blockPrefabs[1], MapTransform);
                        block.transform.position = worldPoint + new Vector3(0, 3, 0);
                        block.gameObject.SetActive(true);
                        StartCoroutine(CreateBlockAnimation(block.transform, worldPoint));
                        blocks[x, y] = block.transform;
                    }
                    cubeCount++;
                }
                grid[x, y] = new MapNode(type, x, y, worldPoint);
                yield return wait;
            }
        }
        yield return null;
    }

    IEnumerator CreateBlockAnimation(Transform t, Vector3 targetPos)
    {
        while (t.position != targetPos)
        {
            t.position = Vector3.MoveTowards(t.position, targetPos, 10 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
    // 맵 데이터 가져오는 함수
    void GetStartSceneMapData()
    {
        string[] data = MapDataManager.GetStartSceneMapData().Split(',');
        gridSizeX = data.GetLength(0);
        gridSizeY = data[0].Length;
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
    #endregion

    #region 캐릭터 맵 생성
    void CreateCharGrid()
    {
        playerCharacters = GameAssetsManager.instance.GetCharacterAssetsTransformList();
        charGridSizeX = 5;
        charGridSizeY = 5;
        charGridMapSize.x = charGridSizeX * nodeDiameter;
        charGridMapSize.y = charGridSizeY * nodeDiameter;
        charGrid = new MapNode[charGridSizeX, charGridSizeY];
        charWorldBottomLeft = CharacterTransform.position - Vector3.right * charGridMapSize.x / 2 - Vector3.forward * charGridMapSize.y / 2;

        List<int> havingCharacters = PlayerData.GetHavingCharacters();
        for (int x = 0; x < charGridSizeX; x++)
        {
            for (int y = 0; y < charGridSizeY; y++)
            {
                Vector3 worldPoint = charWorldBottomLeft + Vector3.right * (x * nodeDiameter*2 + nodeRadius) + Vector3.forward * (y * nodeDiameter*2 + nodeRadius);
                GameObject block = Instantiate(blockPrefabs[0], CharacterTransform);
                block.transform.position = worldPoint + new Vector3(0, -nodeDiameter / 2, 0);
                block.gameObject.SetActive(true);
                charGrid[x, y] = new MapNode(x*charGridSizeX+y, x, y, worldPoint);

                if(havingCharacters.Contains(x*charGridSizeX+y))
                {
                    GameObject character = Instantiate(playerCharacters[x * charGridSizeX + y].gameObject, CharacterTransform);
                    character.transform.position = worldPoint + new Vector3(0, .35f, 0);
                    character.SetActive(true);
                    playerHavingCharacterDictionary.Add(x * charGridSizeX + y, character.transform);
                }
            }
        }
        StartCoroutine("SelectCharacterJumping");
    }
    public void AddCharToGrid(int charNumber)
    {
        if(!playerHavingCharacterDictionary.ContainsKey(charNumber))
        {
            int x = charNumber / charGridSizeY;
            int y = charNumber % charGridSizeX;
            Vector3 worldPoint = charWorldBottomLeft + Vector3.right * (x * nodeDiameter * 2 + nodeRadius) + Vector3.forward * (y * nodeDiameter * 2 + nodeRadius);
            GameObject character = Instantiate(playerCharacters[charNumber].gameObject, CharacterTransform);
            character.transform.position = worldPoint + new Vector3(0, .35f, 0);
            character.SetActive(true);
            playerHavingCharacterDictionary.Add(charNumber, character.transform);
        }
    }
    IEnumerator SelectCharacterJumping()
    {
        Vector3 targetPos;
        while (gameObject.activeSelf)
        {
            Transform currentCharTransform = playerHavingCharacterDictionary[PlayerData.currentCharacter];
            targetPos = currentCharTransform.position;
            targetPos.y = 1;
            float time = .0f;
            while (time < 0.2f)
            {
                currentCharTransform.position = Vector3.Lerp(currentCharTransform.position, targetPos, 7 * Time.deltaTime);
                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            targetPos.y = .35f;
            while (currentCharTransform.position.y > targetPos.y)
            {
                currentCharTransform.position = Vector3.MoveTowards(currentCharTransform.position, targetPos, 3 * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
            currentCharTransform.position = targetPos;
            yield return new WaitForSeconds(.1f);
        }
        yield return null;
    }
    // 위치에 맞는 캐릭터 노드 가져오기
    public MapNode GetCharNodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + (charGridMapSize.x - nodeDiameter) / 2) / charGridMapSize.x*.5f;
        float percentY = (worldPosition.z + (charGridMapSize.y - nodeDiameter) / 2) / charGridMapSize.y*.5f;
        percentX = float.Parse(percentX.ToString("N2"));
        percentY = float.Parse(percentY.ToString("N2"));
        if (percentX >= 1 || percentY >= 1 || percentX < 0 || percentY < 0)
            return null;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((charGridSizeX) * percentX);
        int y = Mathf.RoundToInt((charGridSizeY) * percentY);
        return charGrid[x, y];
    }
    public void SelectCharacter()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                MapNode node = GetCharNodeFromWorldPoint(hit.transform.position - CharacterTransform.position);
                if (node != null)
                {
                    SoundManager.PlaySound(SoundManager.Sound.CharacterSelect);
                    PlayerData.currentCharacter = node.type;
                    PlayerDataSystem.Save();
                }

            }
        }
    }
    void CheckGetSpecialCharacter()
    {
        List<CharacterAssets> charList = GameAssetsManager.instance.CharacterAssets.FindAll(x=>ItemManager.Instance.IsSpecialCharacter(x.id)&&!PlayerData.IsHavingCharacter(x.id));
        foreach(var chr in charList)
        {
            switch(chr.id)
            {
                case 17:
                    if (PlayerData.currentStage >= 85)
                    {
                        PlayerData.AddCharacter(17);
                        GameSystemManager.ShowAlertMessage(new AlertMessage(1, string.Format("{0}",LocalizationManager.GetText("Alert_GetSpecialCharacter17")), null, GameAssetsManager.instance.CharacterAssets[17].asset, null));
                        AddCharToGrid(17);
                    }
                    break;
                case 18:
                    if (hobookGamesClickCount>= 10)
                    {
                        // GooglePlayManager.Instance.OnAddAchievement(6);
                        PlayerData.AddCharacter(18);
                        GameSystemManager.ShowAlertMessage(new AlertMessage(1, string.Format("{0}", LocalizationManager.GetText("Alert_GetSpecialCharacter18")), null, GameAssetsManager.instance.CharacterAssets[18].asset, null));
                        AddCharToGrid(18);
                    }
                    break;
                case 19:
                    if (PlayerData.totalPlayTime>=180) // 플레이 타임 3시간
                    {
                        PlayerData.AddCharacter(19);
                        GameSystemManager.ShowAlertMessage(new AlertMessage(1, string.Format("{0}", LocalizationManager.GetText("Alert_GetSpecialCharacter19")), null, GameAssetsManager.instance.CharacterAssets[19].asset, null));
                        AddCharToGrid(19);
                    }
                    break;
                case 20:
                    if (PlayerData.undoTryCount>=100&&PlayerData.retryCount>=100) //취소100번,다시하기100번
                    {
                        // GooglePlayManager.Instance.OnAddAchievement(5);
                        PlayerData.AddCharacter(20);
                        GameSystemManager.ShowAlertMessage(new AlertMessage(1, string.Format("{0}", LocalizationManager.GetText("Alert_GetSpecialCharacter20")), null, GameAssetsManager.instance.CharacterAssets[20].asset, null));
                        AddCharToGrid(20);
                    }
                    break;
                case 21:
                    if (PlayerData.goldCube>99)
                    {
                        PlayerData.AddCharacter(21);
                        GameSystemManager.ShowAlertMessage(new AlertMessage(1, string.Format("{0}", LocalizationManager.GetText("Alert_GetSpecialCharacter21")), null, GameAssetsManager.instance.CharacterAssets[21].asset, null));
                        AddCharToGrid(21);
                    }
                    break;
                case 22:
                    if(PlayerData.currentStage==100)
                    {
                        // GooglePlayManager.Instance.OnAddAchievement(1);
                        PlayerData.AddCharacter(22);
                        GameSystemManager.ShowAlertMessage(new AlertMessage(1, string.Format("{0}", LocalizationManager.GetText("Alert_GetSpecialCharacter22")), null, GameAssetsManager.instance.CharacterAssets[22].asset, null));
                        AddCharToGrid(22);
                    }
                    break;
                case 23:
                    if(PlayerData.currentStage==100)
                    {
                        int cnt = 0;
                        for(int i = 0; i < 100; i++)
                        {
                            if(PlayerData.clearStageInfoList[i + 1].totalMove== int.Parse(MapDataManager.GetMapClearCount(i + 1)))
                            {
                                cnt++;
                            }
                        }
                        if(cnt==100)
                        {
                            // GooglePlayManager.Instance.OnAddAchievement(4);
                            PlayerData.AddCharacter(23);
                            GameSystemManager.ShowAlertMessage(new AlertMessage(1, string.Format("{0}", LocalizationManager.GetText("Alert_GetSpecialCharacter23")), null, GameAssetsManager.instance.CharacterAssets[23].asset, null));
                            AddCharToGrid(23);
                        }
                    }
                    break;
                case 24:
                    if(charList.Count==1)
                    {
                        // GooglePlayManager.Instance.OnAddAchievement(3);
                        PlayerData.AddCharacter(24);
                        GameSystemManager.ShowAlertMessage(new AlertMessage(1, string.Format("{0}", LocalizationManager.GetText("Alert_GetSpecialCharacter24")), null, GameAssetsManager.instance.CharacterAssets[24].asset, null));
                        AddCharToGrid(24);
                    }
                    break;
            }
        }
    }
    public void OnClickHobookGames()
    {
        if(!PlayerData.IsHavingCharacter(18))
        {
            SoundManager.PlaySound(SoundManager.Sound.MenuClick);
            hobookGamesClickCount++;
            if (hobookGamesClickCount >= 10)
            {
                CheckGetSpecialCharacter();
                hobookGamesClickCount = 0;
            }
        }
    }
    #endregion
}
