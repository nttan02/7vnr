using System.Linq;
using UnityEngine;

public class GamePlay : GameScreen
{
    private readonly GameManager gameManager;
    private GameObject obj;
    private MapDataWrapper mapsData;
    private GameObject currentMap;
    private Character character;
    private GameHub gameHub;
    private Enemy enemyFocus;

    public GamePlay(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    public override void Show()
    {
        if (obj == null)
        {
            obj = Object.Instantiate(gameManager.gameplayPrefab, gameManager.UIHolder);
            gameHub = obj.GetComponent<GameHub>();
            Initialize();
        }
        obj.SetActive(true);
    }

    public override void Hide()
    {
        obj.SetActive(false);
    }

    private void Initialize()
    {
        LoadMapData();
        LoadMap(mapsData?.maps[0].mapPrefab);
    }

    public void CreateCharacter(CharacterData data)
    {
        if (character == null)
        {
            GameObject playerObj = Object.Instantiate(gameManager.playerPrefab);
            character = playerObj.GetComponent<Character>();

            Subscribe(HandlePlayerInput);
            SubscribeMousePress(HandleMousePressed);
        }

        character.Initialize(data);
        gameHub.SetSkillButtons(character.skills);
        CameraFollowBounds.Instance.SetTarget(character.transform);
        CharacterInforUI.Show(character);
    }


    private void HandlePlayerInput(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.LeftArrow:
                character.Move(-1);
                break;
            case KeyCode.RightArrow:
                character.Move(1);
                break;
            case KeyCode.UpArrow:
                character.AddVelocityY(1);
                break;
            default:
                break;
        }
    }

    private void HandleMousePressed(Vector3 mousePos)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, 1 << LayerMask.NameToLayer("Enemy"));
        if (hit.collider != null && hit.collider.TryGetComponent<Enemy>(out Enemy hitEnemy))
        {
            this.enemyFocus = hitEnemy;
            EnemyInfoUI.Show(enemyFocus);
            var hpBar = gameManager.healthBar;
            if (hpBar == null || hpBar.gameObject == null)
            {
                return;
            }
            hpBar.SetTarget(enemyFocus);
            hpBar.gameObject.SetActive(true);
        }
    }


    private void LoadMapData()
    {
        TextAsset json = Resources.Load<TextAsset>("Data/Enemies/spawnPoints");
        if (json != null)
        {
            mapsData = JsonUtility.FromJson<MapDataWrapper>(json.text);
        }
        else
        {
            Debug.LogError("Không tìm thấy");
        }
    }

    public void LoadMap(string mapPrefabPath)
    {
        if (currentMap != null)
        {
            var hpBar = gameManager.healthBar;
            if (hpBar != null && hpBar.gameObject != null)
            {
                hpBar.transform.SetParent(null);
                hpBar.gameObject.SetActive(false);
            }
            EnemyManager.Instance.ClearAllEnemies();
            Object.Destroy(currentMap);
        }

        GameObject prefab = Resources.Load<GameObject>(mapPrefabPath);
        if (prefab == null)
        {
            Debug.LogError($"Không tìm thấy map prefab ở path: {mapPrefabPath}");
            return;
        }
        currentMap = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity, gameManager.mapRoot);
        MapJsonData mapData = mapsData.maps.FirstOrDefault(m => m.mapPrefab == mapPrefabPath);
        if (mapData != null)
        {
            LoadWayPoint(mapData);
            Debug.Log(EnemyManager.Instance);
            EnemyManager.Instance.LoadMapEnemies(mapData.enemies);
        }
    }

    public void SetPlayerPositionWhenEnterWaypoint(bool isLeft)
    {
        MapTransport mapTransport = currentMap.GetComponent<MapTransport>();
        if (mapTransport == null) return;

        if (isLeft)
        {
            character.Teleport(mapTransport.rightTransport.transform.position - new Vector3(5f, -5f, 0f));
        }
        else
        {
            character.Teleport(mapTransport.leftTransport.transform.position + new Vector3(5f, 5f, 0f));
        }

        CameraFollowBounds.Instance.SetEdgeParent(mapTransport.edgeParent.transform);
        CameraFollowBounds.Instance.SnapToTarget();
    }

    private void LoadWayPoint(MapJsonData mapData)
    {
        MapTransport mapTransport = currentMap.GetComponent<MapTransport>();
        CameraFollowBounds.Instance.SetEdgeParent(mapTransport.edgeParent.transform);
        mapTransport.SetData(this, mapData);
    }

}
