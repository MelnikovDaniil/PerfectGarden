using Assets.Scripts.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class GardernManager : MonoBehaviour
{
    public static GardernManager Instance;
    public static event Action OnGardenOpen;
    public static event Action OnGardenClose;
    public float plantCellScale = 0.3f;

    public GameObject gardenLocation;
    public PotWithPlant potWithPlantPrefab;

    [Space]
    public GameObject plusTile;

    [NonSerialized] public List<PotWithPlant> growingPlants;

    private List<PotInfo> potTypes;
    private List<PlantInfo> plantTypes;
    private bool isOpen;

    private LayerMask selectionLayerMask;


    private void Awake()
    {
        Instance = this;
        growingPlants = new List<PotWithPlant>();
        selectionLayerMask = LayerMask.GetMask("Pot", "PlantPlace");

        potTypes = Resources.LoadAll<PotInfo>("Pots").ToList();
        plantTypes = Resources.LoadAll<PlantInfo>("Plants").ToList();
    }

    private async void Start()
    {
        isOpen = true;
        LoadPlants();
        CareManager.Instance.GenerateCare(growingPlants);
        PlantingManager.OnPlantingCancel += Open;
        PlantingManager.OnPlantingFinished += potWithPlant =>
        {
            growingPlants.Add(potWithPlant);
            Open();
            PlacePlant(potWithPlant);
        };

        CareManager.OnCareFinished = (plant) =>
        {
            // Error - Null reference
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
            Open();
        };

        if (!GuideMapper.IsGuideComplete(GuideStep.PlantPlaceSelection))
        {
            var plane = GameObject.FindGameObjectWithTag("plantPlace");
            await TutorialManager.Instance.SetTap(plane, true);
            GuideMapper.Complete(GuideStep.PlantPlaceSelection);
        }
    }

    private async void Update()
    {
        if (gardenLocation.activeSelf && !CareManager.CareInProcess && Input.GetKeyUp(KeyCode.Mouse0))
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
                    var plantPlace = potWithPlant.transform.Find("PlaceBox");
                    plantPlace.gameObject.SetActive(false);
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
    
    public void Open()
    {
        gardenLocation.SetActive(true);
        isOpen = true;
        OnGardenOpen?.Invoke();
    }

    public void Close()
    {
        gardenLocation.SetActive(false);
        isOpen = false;
        OnGardenClose?.Invoke();
    }

    public void UpdateMenu()
    {
        if (isOpen)
        {
            OnGardenOpen?.Invoke();
        }
    }

    public async Task StartPlantingAsync(Vector3 tilePostion)
    {
        Close();
        await PlantingManager.Instance.StartPlanting(tilePostion);
    }

    public async Task StartCaringAsync(PotWithPlant potWithPlant)
    {
        Close();
        await CareManager.Instance.OpenCareMenu(potWithPlant);
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
        potWithPlant.transform.localRotation = Quaternion.identity;
        potWithPlant.transform.parent = gardenLocation.transform;
        potWithPlant.transform.localScale = Vector3.one * plantCellScale;

        var plantPlace = potWithPlant.transform.Find("PlaceBox");
        
        if (plantPlace != null)
        {
            plantPlace.gameObject.SetActive(true);
        }
        else
        {
            CreatePlaceBox(potWithPlant.gameObject);
        }
    }

    public void CreatePlaceBox(GameObject targetObject)
    {
        var clickBox = new GameObject("PlaceBox")
        {
            layer = LayerMask.NameToLayer("PlantPlace"),
        };
        clickBox.transform.parent = targetObject.transform;
        clickBox.transform.localPosition = Vector3.zero;
        clickBox.transform.localRotation = Quaternion.identity;
        clickBox.transform.localScale = Vector3.one;

        var boxCollider = clickBox.AddComponent<BoxCollider>();
        var totalBounds = CalculateTotalBounds(targetObject);
        if (totalBounds.size != Vector3.zero)
        {
            var localCenter = targetObject.transform.InverseTransformPoint(totalBounds.center);
            var localSize = targetObject.transform.InverseTransformVector(totalBounds.size);

            //localSize = Vector3.Scale(localSize, sizeMultiplier) + padding;

            boxCollider.center = localCenter;
            boxCollider.size = localSize * 0.7f;

            Debug.Log($"Composite collider created for {targetObject.name}: center={localCenter}, size={localSize}");
        }
        else
        {
            Debug.LogWarning($"Could not calculate valid bounds for {targetObject.name}");
        }
    }


    private Bounds CalculateTotalBounds(GameObject targetObject)
    {
        var renderers = targetObject.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            Debug.LogWarning($"No renderers found in {targetObject.name} hierarchy!");
            return new Bounds(targetObject.transform.position, Vector3.zero);
        }

        var totalBounds = renderers[0].bounds;

        for (var i = 1; i < renderers.Length; i++)
        {
            totalBounds.Encapsulate(renderers[i].bounds);
        }

        return totalBounds;
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
                buffs = plant.GetAllBuffStates()
                    .Select(x => x.GetSaveInfo())
                    .ToList()
            }).ToList();

        PlantStateInfoMapper.SavePlantStates(states);
    }

    private void LoadPlants()
    {
        var states = PlantStateInfoMapper.GetAllPlantStates();
        foreach (var state in states)
        {
            var potInfo = potTypes.First(x => x.name == state.potName);
            var createdPotWithPlant = Instantiate(potInfo.potPrefab);

            createdPotWithPlant.potInfo = potInfo;
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
            BuffManager.Instance.ApplyBuffs(createdPotWithPlant, state.buffs);
            PlacePlant(createdPotWithPlant);
            growingPlants.Add(createdPotWithPlant);
        }
    }

    private void OnApplicationQuit()
    {
        SavePlants();
    }
}
