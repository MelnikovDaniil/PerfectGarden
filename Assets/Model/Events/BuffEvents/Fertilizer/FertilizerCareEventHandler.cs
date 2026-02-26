using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace Assets.Model.Events.CareEvents.Fertilizer
{
    public class FertilizerCareEventHandler : BuffEventHandler
    {
        public override BuffType EventName => BuffType.SpeedGro;

        public LevelEstimationText levelEstimationTextPrefab;
        public GlassBottle glassBottlePrefab;
        public Bucket bucketPrefab;

        [Header("Settings")]
        public MinMaxGradient fertilizerColorSet;
        public Vector2 iterationRange = new Vector2(2, 2);
        public Vector2 bucketOffset = new Vector2(2, 2);
        public Vector2 textOffset = new Vector2(-0.15f, 0.2f);

        public int spraysAmount = 4;
        public Vector2 sprayDistanceRange = new Vector2(1f, 2f);
        public Vector2 sprayOffset = new Vector2(0, 1);
        public HydrationSpray sprayPrefab;

        private GlassBottle glassBottleInstance;
        private Bucket bucketInstance;
        private LevelEstimationText levelEstimationTextInstance;

        private HydrationSpray sprayInstance;
        private int currentSpraysAmount;

        private LayerMask sprayLayerMask;

        private void Start()
        {
            glassBottleInstance = Instantiate(glassBottlePrefab);
            glassBottleInstance.gameObject.SetActive(false);
            bucketInstance = Instantiate(bucketPrefab);
            bucketInstance.gameObject.SetActive(false);
            levelEstimationTextInstance = Instantiate(levelEstimationTextPrefab);
            levelEstimationTextInstance.gameObject.SetActive(false);


            sprayLayerMask = LayerMask.GetMask("Spray");
            sprayInstance = Instantiate(sprayPrefab);
            sprayInstance.gameObject.SetActive(false);
            sprayInstance.gameObject.layer = LayerMask.NameToLayer("Spray");

            bucketInstance.OnWatering += glassBottleInstance.UpdateWaterLevel;
        }

        protected override async Task PrepareHandlingAsync(CancellationToken token = default)
        {
            // Get rid of Plant
            await MovementHelper.MoveObjectAwayAsync(Context.PotWithPlant.transform, Vector3.down, 1f, true);

            // Set bottle to beginning state
            glassBottleInstance.Clear();

            // Show glass
            glassBottleInstance.transform.parent = Context.PotWithPlant.transform.parent;
            glassBottleInstance.gameObject.SetActive(true);
            glassBottleInstance.transform.localPosition = Vector3.zero;
            glassBottleInstance.transform.localScale = Vector3.one;

            // Show object to tap
            bucketInstance.transform.parent = Context.PotWithPlant.transform.parent;
            bucketInstance.gameObject.SetActive(true);
            bucketInstance.transform.localPosition = bucketOffset;
            bucketInstance.transform.localRotation = Quaternion.identity;
            bucketInstance.transform.localScale = Vector3.one;


            // Define text
            levelEstimationTextInstance.transform.parent = Context.PotWithPlant.transform.parent;
            levelEstimationTextInstance.transform.localPosition = textOffset;
            levelEstimationTextInstance.transform.localRotation = Quaternion.identity;
            levelEstimationTextInstance.transform.localScale = Vector3.one;

            // Spray
            sprayInstance.transform.parent = Context.CarePlace;
            sprayInstance.transform.localScale = Vector3.one;

            currentSpraysAmount = spraysAmount;

            glassBottleInstance.Clear();
        }

        protected override async Task StartHandlingAsync(CancellationToken token = default)
        {
            var levelArchived = false;
            var estimationLevel = GlassBottleLevel.TryAgain;

            bucketInstance.SetUp();
            _ = TutorialManager.Instance.SetHoldAsync(bucketInstance.gameObject, 0.5f, false, token);
            bucketInstance.OnBucketUp = () => levelArchived = true;
            var iterationNumber = Random.Range(iterationRange.x, iterationRange.y);
            var previousColor = Color.white;
            for (var i = 0; i < iterationNumber; i++)
            {
                var randomColor = Color.white;
                do
                {
                    randomColor = fertilizerColorSet.Evaluate(Random.value, Random.value);

                }while (previousColor == randomColor);
                previousColor = randomColor;
                glassBottleInstance.GenerateTargetLevel(i + 1 == iterationNumber);
                glassBottleInstance.SetColor(randomColor);
                bucketInstance.SetColor(randomColor);
                do
                {
                    levelEstimationTextInstance.gameObject.SetActive(false);
                    glassBottleInstance.ResetLevel();
                    while (!levelArchived)
                    {
                        if (token.IsCancellationRequested)
                        {
                            await InterruptAsync();
                            return;
                        }
                        await Task.Yield();
                    }
                    levelArchived = false;
                    estimationLevel = glassBottleInstance.CheckWaterLevel();
                    levelEstimationTextInstance.gameObject.SetActive(true);
                    await levelEstimationTextInstance.ShowText(estimationLevel);
                } while (estimationLevel == GlassBottleLevel.TryAgain);
                glassBottleInstance.SaveLevel();
            }

            bucketInstance.Hide();

            _ = MovementHelper.MoveObjectAwayAsync(
                bucketInstance.transform, Vector3.right,
                1f,
                true);
            await glassBottleInstance.CloseCapAsync();

            _ = TutorialManager.Instance.SetShake(glassBottleInstance.gameObject, false, token);
            try
            {
                await glassBottleInstance.StartShakingAsync(token);
            }
            catch (TaskCanceledException)
            {
                await InterruptAsync();
                return;
            }

            await MovementHelper.MoveObjectToBasePositionAsync(glassBottleInstance.transform, 0.3f, true);
            await MovementHelper.MoveObjectAwayAsync(glassBottleInstance.transform, Vector3.up, 1f, true);
            await MovementHelper.MoveObjectToBasePositionAsync(Context.PotWithPlant.transform, 1f, true);

            glassBottleInstance.gameObject.SetActive(false);

            sprayInstance.MoveSprayRelatively(
                Context.PotWithPlant.plantRenderer.transform,
                sprayInstance.GenerateSprayPosition(sprayDistanceRange),
                sprayOffset);
            sprayInstance.gameObject.SetActive(true);
            _ = TutorialManager.Instance.SetTap(sprayInstance.gameObject, false, token);

            while (currentSpraysAmount > 0)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    var input = Input.mousePosition;
                    var mousePosition = Camera.main.ScreenToWorldPoint(input);
                    var camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(camRay, float.PositiveInfinity, sprayLayerMask))
                    {
                        currentSpraysAmount--;
                        await sprayInstance.MakeSprayAsync(Context.PotWithPlant.plantRenderer.transform, sprayDistanceRange, sprayOffset);
                    }
                }

                if (token.IsCancellationRequested)
                {
                    await InterruptAsync();
                    return;
                }
                await Task.Yield();
            }

            sprayInstance.gameObject.SetActive(false);
        }

        protected override async Task InterruptHandlingAsync()
        {
            Clear();
            await base.InterruptHandlingAsync();
        }

        public override void Clear()
        {
            // Hide bottle
            // Return Plant to base state
            bucketInstance.gameObject.SetActive(false);
            levelEstimationTextInstance.gameObject.SetActive(false);
            glassBottleInstance.gameObject.SetActive(false);
            glassBottleInstance.Clear();
            sprayInstance.gameObject.SetActive(false);

            StartCoroutine(MovementHelper.MoveObjectAwayRoutine(Context.PotWithPlant.transform,Vector3.up, 1f, true));
        }
    }
}