using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;
using Random = UnityEngine.Random;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance;
    public static event Action<Order> OnOrderAdded;
    public static event Action<Order> OnOrderСomplete;
    public static event Action<Order> OnOrderPlaced;
    public static event Action OnAllOrdersComplete;
    public SpriteRenderer characterRenderer;
    public GameObject location;
    public Transform plantPlace;
    public int maxOrderNumber;
    public int orderGenerationInterval = 300;
    public List<OrderCharacterInfo> charactersInfos;

    [Space]
    [Header("Reward settings")]
    [Range(0f, 1f)]
    public float minRewardCostFromHighest = 0.08f;
    [Range(0f, 1f)]
    public float maxRewardCostFromHighest = 0.12f;


    [NonSerialized] private Order currentOrder;
    private readonly List<Order> orders = new List<Order>();
    private List<PlantInfo> plantTypes;
    private List<PotInfo> potTypes;
    private DateTime lastOrderGenerationTime;
    private CancellationTokenSource orderGenerationTokenSource;


    // Teplates for description
    private readonly string[] descriptionTamplates =
    {
        "Could you please take care of my {0}? It needs some attention while I'm away.",
        "I would be grateful if you could look after my {0}. Thank you!",
        "Hello! My {0} requires your care. Please give it the attention it needs.",
        "I kindly request your help with my {0}. It's very special to me!",
        "Would you mind caring for my {0}? I'd really appreciate your help.",
        "My dear {0} needs tending to. Could you assist me with this?",
        "I need someone to care for my {0}. Would you be able to help me out?",
        "Attention please: my {0} requires maintenance. Your assistance would be wonderful!",
        "Could I trouble you to watch over my {0}? It means a lot to me.",
        "I'd be so thankful if you could give my {0} some love and care today!"
    };

    private readonly string[] gratitudePhrases =
    {
        "Thanks for watering my plants!",
        "I appreciate you looking after my plant.",
        "Thank you for taking care of it!",
        "You're a plant-saving hero!",
        "Thanks for keeping it alive!",
        "I owe you for plant-sitting!",
        "Thank you for the TLC!",
        "You're the best plant parent!",
        "Thanks for nurturing my green friend!",
        "My plant thanks you too!"
    };

    private void Awake()
    {
        Instance = this;
        orderGenerationTokenSource = new CancellationTokenSource();
        OnAllOrdersComplete += () => characterRenderer.gameObject.SetActive(false);

        if (minRewardCostFromHighest > maxRewardCostFromHighest)
        {
            throw new ArgumentException("Min reward cost cannot be higher than max");
        }
        plantTypes = Resources.LoadAll<PlantInfo>("Plants").ToList();
        potTypes = Resources.LoadAll<PotInfo>("Pots").ToList();
    }

    private async void Start()
    {
        LoadOrders();
        await StartOrderGenerationTimerAsync(orderGenerationTokenSource.Token);
    }

    public async Task OpenAsync()
    {
        location.SetActive(true);
        if (currentOrder == null)
        {
            currentOrder = orders.GetRandomOrDefault();
        }

        if (currentOrder == null)
        {
            OnAllOrdersComplete?.Invoke();
        }
        else
        {
            await PlaceOrderAsync(currentOrder);
        }
    }

    public void Close()
    {
        location.SetActive(false);
    }

    private async Task StartOrderGenerationTimerAsync(CancellationToken token = default)
    {
        while (!token.IsCancellationRequested)
        {
            await CheckPendingOrdersAsync();
            await Task.Delay(orderGenerationInterval);
        }
    }

    private async Task CheckPendingOrdersAsync(CancellationToken token = default)
    {
        var timeSinceLastGeneration = DateTime.Now - lastOrderGenerationTime;
        var intervalsPassed = (int)(timeSinceLastGeneration.TotalSeconds / orderGenerationInterval);

        if (intervalsPassed > 0)
        {
            var ordersToAdd = Mathf.Min(intervalsPassed, maxOrderNumber - orders.Count);

            for (int i = 0; i < ordersToAdd; i++)
            {
                if (token.IsCancellationRequested) return;
                await AddOrderByTimerAsync(token);
            }

            lastOrderGenerationTime = DateTime.Now;
            SaveOrders();
        }
    }

    private async Task AddOrderByTimerAsync(CancellationToken token = default)
    {
        if (token.IsCancellationRequested && orders.Count >= maxOrderNumber) return;

        var newOrder = GenerateOrder();
        orders.Add(newOrder);
        OnOrderAdded.Invoke(newOrder);
        lastOrderGenerationTime = DateTime.Now;

        Debug.Log($"New order added by timer: {newOrder.Name}");

        SaveOrders();

        await Task.Yield();
    }

    private void GenerateOrders()
    {
        while (orders.Count < maxOrderNumber)
        {
            var newOrder = GenerateOrder();
            orders.Add(newOrder);
            OnOrderAdded.Invoke(newOrder);
        }
        lastOrderGenerationTime = DateTime.Now;
        SaveOrders();
    }

    private Order GenerateOrder()
    {
        var plantInfo = plantTypes.GetRandom();
        var potInfo = potTypes.GetRandom();
        var stage = Random.Range(0, plantInfo.growStages.Count - 1);

        var generatedPlant = Instantiate(potInfo.potPrefab);
        generatedPlant.plantInfo = plantInfo;
        generatedPlant.potInfo = potInfo;

        generatedPlant.SetStage(stage);
        generatedPlant.IsOrder = true;
        generatedPlant.gameObject.SetActive(false);

        CareManager.Instance.GenerateCare(new List<PotWithPlant> { generatedPlant });

        return new Order
        {
            Name = $"{plantInfo.name} care",
            Description = string.Format(descriptionTamplates.GetRandom(), plantInfo.name),
            CharacterId = Random.Range(0, charactersInfos.Count - 1),
            PotWithPlant = generatedPlant,
            Reward = CalculateCost(),
        };
    }

    public async Task StartOrderAsync()
    {
        Close();
        CareManager.OnCareFinished = async (plant) =>
        {
            if (plant.waitingCareEvents.Count == 0)
            {
                MoneyMapper.Money += currentOrder.Reward;
                RewardManager.Instance.GenerateCareLargeReward();
                Destroy(currentOrder.PotWithPlant.gameObject);
                orders.Remove(currentOrder);
                OnOrderСomplete.Invoke(currentOrder);

                SaveOrders();
                await SayThankYouAsync();
                currentOrder = null;
            }
            await OpenAsync();
        };

        await CareManager.Instance.OpenCareMenu(currentOrder.PotWithPlant);
    }

    private bool isThankYouDialogue = false;

    public async Task ShowOrderDescriptionAsync()
    {
        isThankYouDialogue = false;
        characterRenderer.sprite = charactersInfos[currentOrder.CharacterId].regular;
        await OrderManagerUI.Instance.ShowDialogueAsync(currentOrder.Description, isThankYouDialogue);
    }

    public async Task SayThankYouAsync()
    {
        location.SetActive(true);
        isThankYouDialogue = true;
        var thankYouPhrase = gratitudePhrases.GetRandom();
        characterRenderer.sprite = charactersInfos[currentOrder.CharacterId].fun;
        await OrderManagerUI.Instance.ShowDialogueAsync(thankYouPhrase, isThankYouDialogue);
    }

    public async Task PlaceOrderAsync(Order order)
    {
        foreach (Transform item in plantPlace)
        {
            item.gameObject.SetActive(false);
        }
        order.PotWithPlant.gameObject.SetActive(true);
        order.PotWithPlant.transform.parent = plantPlace;
        order.PotWithPlant.transform.localPosition = Vector3.zero;
        order.PotWithPlant.transform.localScale = Vector3.one;
        order.PotWithPlant.transform.localRotation = Quaternion.identity;

        currentOrder = order;
        await ShowOrderDescriptionAsync();
        OnOrderPlaced?.Invoke(currentOrder);
    }

    private int CalculateCost()
    {
        // Probably change to the most expencive seed cost instead most expensive sale
        var rewardPart = Random.Range(minRewardCostFromHighest, maxRewardCostFromHighest);
        var reward = (int)(MoneyMapper.GetHighestReward() * rewardPart);

        return reward == 0 ? Random.Range(1, 4) : reward;
    }

    private void LoadOrders()
    {
        lastOrderGenerationTime = OrderMapper.GetLastGenerationTime();
        var orderSaves = OrderMapper.GetAllOrders();
        foreach (var orderSave in orderSaves)
        {
            var potInfo = potTypes.First(x => x.name == orderSave.potName);
            var plantInfo = plantTypes.First(x => x.name == orderSave.plantName);
            var createdPotWithPlant = Instantiate(potInfo.potPrefab);
            createdPotWithPlant.plantInfo = plantInfo;
            createdPotWithPlant.potInfo = potInfo;
            createdPotWithPlant.waitingCareEvents = orderSave.waitingCareEvents;
            createdPotWithPlant.IsOrder = true;
            createdPotWithPlant.SetStage(orderSave.stage);
            CareManager.Instance.GenerateStates(createdPotWithPlant);
            createdPotWithPlant.gameObject.SetActive(false);

            var order = new Order
            {
                Name = orderSave.plantName,
                CharacterId = orderSave.characterId,
                Description = orderSave.description,
                PotWithPlant = createdPotWithPlant,
                Reward = orderSave.reward,
            };

            orders.Add(order);
            OnOrderAdded?.Invoke(order);
        }
    }

    private void SaveOrders()
    {
        OrderMapper.SaveLastGenerationTime(lastOrderGenerationTime);
        var saves = orders.Select(order =>
            new OrderSaveInfo
            {
                description = order.Description,
                orderName = order.Name,
                stage = order.PotWithPlant.currentStage,
                plantName = order.PotWithPlant.plantInfo.name,
                potName = order.PotWithPlant.potInfo.name,
                waitingCareEvents = order.PotWithPlant.waitingCareEvents,
                characterId = order.CharacterId,
                reward = order.Reward,
            }).ToList();

        OrderMapper.SaveOrders(saves);
    }

    private void OnApplicationQuit()
    {
        orderGenerationTokenSource.Cancel();
        SaveOrders();
    }
}
