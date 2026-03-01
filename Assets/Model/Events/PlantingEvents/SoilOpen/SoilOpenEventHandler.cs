using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SoilOpenPlantingEvent : PlantEventHandler
{
    public override PlantingEvent EventName => PlantingEvent.SoilOpen;

    public int numberOfLevels = 5;
    public RectTransform sliderCanvas;
    public Slider slider;
    public Animator scissorsAnimator;
    public Animator soilOpenAnimator;
    public Transform soilBag;
    public AudioClip soilAppearanceClip;
    public List<AudioClip> scissorsClips;

    private bool isOpened;
    private float progress;
    private float levelStep;
    private int lastLevelArchive;

    private void Awake()
    {
        slider.onValueChanged.AddListener(OnSliderMove);
    }

    private void Start()
    {
        Clear();
        soilOpenAnimator.speed = 0;
        levelStep = 1.0f / numberOfLevels;
    }

    private void Update()
    {
        if (Status == HandlingStatus.Started)
        {
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                if (progress != 1)
                {
                    progress = 0;
                    slider.value = 0;
                    lastLevelArchive = -1;
                }
                else
                {
                    isOpened = true;
                }
            }
        }
    }

    protected override async Task PrepareHandlingAsync(CancellationToken token = default)
    {
        isOpened = false;
        progress = 0;
        slider.value = 0;
        lastLevelArchive = -1;
        sliderCanvas.gameObject.SetActive(false);
        soilOpenAnimator.gameObject.SetActive(true);
        soilOpenAnimator.SetFloat("progress", 0);
        SoundManager.PlaySound(soilAppearanceClip);
        await Task.Yield();
        await base.PrepareHandlingAsync();
        soilBag.localPosition = new Vector3(0, -12, 0);
        await MovementHelper.MoveObjectToBasePositionAsync(soilBag, 0.5f, true);
        sliderCanvas.gameObject.SetActive(true);
    }

    protected override async Task StartHandlingAsync(CancellationToken token = default)
    {
        _ = TutorialManager.Instance.SetSwipeAsync(slider.handleRect.gameObject, Vector2.right, 1f, false, token);
        while (!isOpened)
        {
            await Task.Yield();
        }

        sliderCanvas.gameObject.SetActive(false);
        await MovementHelper.MoveObjectAwayAsync(soilBag, Vector3.up, 0.5f, true);
    }

    public override void Clear()
    {
        isOpened = false;
        slider.value = 0;
        lastLevelArchive = 0;
        sliderCanvas.gameObject.SetActive(false);
        soilOpenAnimator.gameObject.SetActive(false);
        soilOpenAnimator.SetFloat("progress", 0);
    }

    public void OnSliderMove(float value)
    {
        if (value > progress)
        {
            progress = value;
            var currentLevelProcess =  value / levelStep;
            var animationTime = currentLevelProcess % 1;
            var currentLevel = (int)currentLevelProcess;
            if (currentLevel > lastLevelArchive)
            {
                lastLevelArchive = currentLevel;
                SoundManager.PlaySound(scissorsClips.GetRandom());
            }
            scissorsAnimator.SetFloat("progress", animationTime);
        }
        else
        {
            slider.value = progress;
        }

        soilOpenAnimator.SetFloat("progress", progress);
    }
}
