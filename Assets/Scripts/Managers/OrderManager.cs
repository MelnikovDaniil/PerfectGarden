using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance;
    public static event Action<Order> OnOrderAdded;
    public static event Action<Order> OnOrderСomplete;
    public static event Action<Order> OnOrderPlaced;
    public static event Action OnAllOrdersComplete;
    public PotWithPlant potWithPlantPrefab;
    public GameObject location;
    public Transform plantPlace;
    public int maxOrderNumber;
    public int orderGenerationInterval = 300;

    [NonSerialized] private Order currentOrder;
    private readonly List<Order> orders = new List<Order>();
    private List<PlantInfo> plants;
    private List<PotInfo> pots;
    private DateTime lastOrderGenerationTime;


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

    private void Awake()
    {
        Instance = this;
        plants = Resources.LoadAll<PlantInfo>("Plants").ToList();
        pots = Resources.LoadAll<PotInfo>("Pots").ToList();
        //GenerateOrders();
        LoadOrders();
    }
    private async void Start()
    {
        await StartOrderGenerationTimerAsync();
    }

    public void Open()
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
            PlaceOrder(currentOrder);
        }
    }

    public void Close()
    {
        location.SetActive(false);
    }

    private async Task StartOrderGenerationTimerAsync()
    {
        while (true)
        {
            await CheckPendingOrdersAsync();
            await Task.Delay(orderGenerationInterval);
        }
    }

    private async Task CheckPendingOrdersAsync()
    {
        var timeSinceLastGeneration = DateTime.Now - lastOrderGenerationTime;
        var intervalsPassed = (int)(timeSinceLastGeneration.TotalSeconds / orderGenerationInterval);

        if (intervalsPassed > 0)
        {
            var ordersToAdd = Mathf.Min(intervalsPassed, maxOrderNumber - orders.Count);

            for (int i = 0; i < ordersToAdd; i++)
            {
                await AddOrderByTimer();
            }

            lastOrderGenerationTime = DateTime.Now;
            SaveOrders();
        }
    }

    private async Task AddOrderByTimer()
    {
        if (orders.Count >= maxOrderNumber) return;

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
        var plantInfo = plants.GetRandom();
        var potInfo = pots.GetRandom();
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
                //MoneyMapper.Money += currentOrder.Reward;
                RewardManager.Instance.GenerateCareLargeReward();
                Destroy(currentOrder.PotWithPlant.gameObject);
                orders.Remove(currentOrder);
                OnOrderСomplete.Invoke(currentOrder);
                currentOrder = null;

                SaveOrders();
            }
            Open();
        };

        await CareManager.Instance.OpenCareMenu(currentOrder.PotWithPlant);
    }

    public void PlaceOrder(Order order)
    {
        // Place selected Order on table
        // Setup button
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
        OnOrderPlaced?.Invoke(currentOrder);
    }

    private int CalculateCost()
    {
        // Add balance calculation of the cost depending on the plant seed cost
        // Hardcoder for now
        return 100;
    }

    private void LoadOrders()
    {
        if (PlayerPrefs.HasKey("LastOrderGenerationTime"))
        {
            var ticks = long.Parse(PlayerPrefs.GetString("LastOrderGenerationTime"));
            lastOrderGenerationTime = new DateTime(ticks);
        }
        else
        {
            lastOrderGenerationTime = DateTime.Now;
        }
        // Loading orders and generate new for the time that user was apsent
    }

    private void SaveOrders()
    {
        PlayerPrefs.SetString("LastOrderGenerationTime", lastOrderGenerationTime.Ticks.ToString());
        // Save existing orders in they state
        PlayerPrefs.Save();
    }
}
