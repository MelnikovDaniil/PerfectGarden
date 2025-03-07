using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class GardernManager : MonoBehaviour
{
    public static GardernManager Instance;
    public static event Action OnGardenOpen;
    public static event Action OnGardenClose;
    public float plantCellScale = 0.3f;
    public int stateCheckIntervalSec = 60;

    public GameObject gardenLocation;
    public PotWithPlant potWithPlantPrefab;

    [Space]
    public GameObject plusTile;

    [NonSerialized] public List<PotWithPlant> growingPlants;

    private List<PotInfo> potTypes;
    private List<PlantInfo> plantTypes;

    private LayerMask selectionLayerMask;


    private void Awake()
    {
        Instance = this;
        growingPlants = new List<PotWithPlant>();
        selectionLayerMask = LayerMask.GetMask("Pot", "PlantPlace");

        potTypes = Resources.LoadAll<PotInfo>("Pots").ToList();
        plantTypes = Resources.LoadAll<PlantInfo>("Plants").ToList();
        LoadPlants();
    }

    private void Start()
    {
        CareManager.Instance.GenerateCare(growingPlants);
        PlantingManager.OnPlantingFinished += potWithPlant =>
        {
            growingPlants.Add(potWithPlant);
            gardenLocation.SetActive(true);
            PlacePlant(potWithPlant);
            OnGardenOpen?.Invoke();
        };

        CareManager.OnCareFinished += (plant) =>
        {
            gardenLocation.SetActive(true);
            if (plant.gameObject.activeSelf)
            {
                PlacePlant(plant);
            }
            else
            {
                // Replace cell wiss empty
                Instantiate(plusTile, plant.cell, plant.transform.rotation, gardenLocation.transform);
                growingPlants.Remove(plant);
                Destroy(plant.gameObject);
            }
            OnGardenOpen?.Invoke();
        };

        var _ = StateChecking();
    }

    private async void Update()
    {
        if (!CareManager.CareInProcess && Input.GetKeyUp(KeyCode.Mouse0))
        {
            var mousePos = Input.mousePosition;
            mousePos.z = 10;
            var ray = Camera.main.ScreenPointToRay(mousePos);
            //var tilePos = _plantingTileMap.WorldToCell(clickPos);
            var areaSelected = Physics.Raycast(ray, out var hit, float.PositiveInfinity, selectionLayerMask);
            if (areaSelected)
            {
                var potWithPlant = hit.collider.GetComponentInParent<PotWithPlant>();
                if (potWithPlant != null)
                {
                    await StartCaringAsync(potWithPlant);
                }
                else
                {
                    Debug.Log("Tilemap detected!!");
                    await StartPlantingAsync(hit.transform.position);
                }
            }
            else
            {
                Debug.Log("Tilemap not found =(");
            }
        }
    }

    public async Task StartPlantingAsync(Vector3 tilePostion)
    {
        gardenLocation.SetActive(false);
        await PlantingManager.Instance.StartPlanting(tilePostion);
        OnGardenClose.Invoke();
    }

    public async Task StartCaringAsync(PotWithPlant potWithPlant)
    {
        gardenLocation.SetActive(false);
        await CareManager.Instance.OpenCareMenu(potWithPlant);
        OnGardenClose.Invoke();
    }

    private void PlacePlant(PotWithPlant potWithPlant)
    {
        var potPosition = potWithPlant.cell - (new Vector3(0, potWithPlant.potBottomPostionY) * plantCellScale);
        var colliders = Physics.OverlapSphere(potWithPlant.cell, 0.1f, LayerMask.GetMask("PlantPlace"));
        if (colliders.Any())
        {
            Destroy(colliders.FirstOrDefault().gameObject);
        }
        potWithPlant.transform.position = potPosition;
        potWithPlant.transform.parent = gardenLocation.transform;
        potWithPlant.transform.localScale = Vector3.one * plantCellScale;
    }

    private void SavePlants()
    {
        var states = growingPlants.Select(plant =>
            new PlantStateInfo
            {
                plantId = Guid.NewGuid().ToString(),
                cellPosition = plant.cell,
                currentStage = plant.currentStage,
                lastCareTimeUtc = plant.lastStageChangeTime.Ticks,
                lastStatusUpdateTimeUtc = plant.lastCareAddedTime.Ticks,
                plantName = plant.plantInfo.name,
                potName = plant.potInfo.name,
                waitingCareEvents = plant.waitingCareEvents,
            }).ToList();

        PlantStateInfoMapper.SavePlantStates(states);
    }

    private void LoadPlants()
    {
        var states = PlantStateInfoMapper.GetAllPlantStates();
        foreach (var state in states)
        {
            var createdPotWithPlant = Instantiate(potWithPlantPrefab);

            createdPotWithPlant.potInfo = potTypes.First(x => x.name == state.potName);
            createdPotWithPlant.plantInfo = plantTypes.First(x => x.name == state.plantName);
            createdPotWithPlant.cell = state.cellPosition;
            createdPotWithPlant.waitingCareEvents = state.waitingCareEvents;
            createdPotWithPlant.lastStageChangeTime = new DateTime(state.lastCareTimeUtc);
            createdPotWithPlant.lastCareAddedTime = new DateTime(state.lastStatusUpdateTimeUtc);
            if (createdPotWithPlant.IsShouldBeRotted)
            {
                createdPotWithPlant.Rot();
            }
            else
            {
                createdPotWithPlant.SetStage(state.currentStage);
            }
            PlacePlant(createdPotWithPlant);
            growingPlants.Add(createdPotWithPlant);
        }
    }

    private async Task StateChecking()
    {
        while (true)
        {
            await Task.Delay(1000 * stateCheckIntervalSec);
            CareManager.Instance.GenerateCare(growingPlants);
            CareManager.Instance.UpdateMenu();
            OnGardenOpen?.Invoke();
        }
    }

    private void OnApplicationQuit()
    {
        SavePlants();
    }
}
