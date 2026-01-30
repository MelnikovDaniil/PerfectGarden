using UnityEngine;

[RequireComponent(typeof(Light))]
public class CandleFlicker : MonoBehaviour
{
    [Header("Основные настройки")]
    [Tooltip("Интенсивность базового света")]
    public float baseIntensity = 1.0f;
    [Tooltip("Скорость мерцания")]
    public float flickerSpeed = 10f;

    [Header("Мерцание")]
    [Tooltip("Минимальная интенсивность мерцания")]
    [Range(0f, 1f)] public float minIntensity = 0.8f;
    [Tooltip("Максимальная интенсивность мерцания")]
    [Range(1f, 2f)] public float maxIntensity = 1.2f;

    [Header("Случайность")]
    [Tooltip("Включить случайные всплески")]
    public bool enableRandomFlickers = true;
    [Tooltip("Вероятность случайного всплеска")]
    [Range(0f, 1f)] public float randomFlickerChance = 0.1f;
    [Tooltip("Интенсивность случайных всплесков")]
    public float randomFlickerIntensity = 2.0f;
    [Tooltip("Длительность случайных всплесков")]
    public float randomFlickerDuration = 0.1f;

    private Light pointLight;
    private float flickerTimer;
    private float randomFlickerTimer;
    private float originalIntensity;

    void Start()
    {
        pointLight = GetComponent<Light>();
        originalIntensity = pointLight.intensity;
        flickerTimer = Random.Range(0f, 100f); // Начальное случайное значение
    }

    void Update()
    {
        // Основное мерцание (синусоидальное с шумом)
        float noise = Mathf.PerlinNoise(flickerTimer, 0);
        float flickerIntensity = Mathf.Lerp(minIntensity, maxIntensity, noise);

        // Применяем основное мерцание
        pointLight.intensity = originalIntensity * flickerIntensity;

        // Случайные всплески (имитация порывов ветра)
        if (enableRandomFlickers)
        {
            HandleRandomFlickers();
        }

        // Увеличиваем таймер
        flickerTimer += Time.deltaTime * flickerSpeed;
    }

    void HandleRandomFlickers()
    {
        if (randomFlickerTimer > 0)
        {
            // Если активен случайный всплеск
            randomFlickerTimer -= Time.deltaTime;
            float flickerValue = randomFlickerTimer / randomFlickerDuration;
            pointLight.intensity *= Mathf.Lerp(1f, randomFlickerIntensity, flickerValue);
        }
        else
        {
            // Проверяем шанс нового всплеска
            if (Random.value < randomFlickerChance * Time.deltaTime)
            {
                randomFlickerTimer = randomFlickerDuration;
            }
        }
    }

    // Метод для сброса к исходной интенсивности
    public void ResetIntensity()
    {
        pointLight.intensity = originalIntensity;
    }

    // Метод для настройки параметров
    public void ConfigureFlicker(float newBaseIntensity, float newFlickerSpeed,
                                 float newMinIntensity, float newMaxIntensity)
    {
        baseIntensity = newBaseIntensity;
        flickerSpeed = newFlickerSpeed;
        minIntensity = newMinIntensity;
        maxIntensity = newMaxIntensity;
        pointLight.intensity = baseIntensity;
        originalIntensity = baseIntensity;
    }
}