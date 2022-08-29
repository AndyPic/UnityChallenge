using System.Collections.Generic;
using UnityEngine;

namespace Assessment
{
    public class SpawnAreaController : MonoBehaviour
    {
        // Vars
        #region
        [Header("Spawn Area")]

        [SerializeField, Min(5), Tooltip("The radius of the spawn area.")]
        float radius = 40;

        [SerializeField, Min(0), Tooltip("The minimum speed with which the cube can rotate on any axis (Rotations per second)")]
        float areaMinRotationsPerSec = 0;

        [SerializeField, Min(0), Tooltip("The maximum speed with which the cube can rotate on any axis (Rotations per second)")]
        float areaMaxRotationsPerSec = 0.2f;


        [Header("Cubes")]

        [SerializeField, Tooltip("The cube prefab")]
        GameObject cubePrefab;

        [SerializeField, Min(1), Tooltip("The number of cubes to spawn")]
        int numberOfCubes = 1500;

        [Min(0), Tooltip("The time (seconds) it takes for a cube to fade away after it's lifetime expires")]
        public float cubeFadeDelay = 0.5f;

        [Header("Cube Rotation")]

        [SerializeField, Min(0), Tooltip("The minimum speed with which the cube can rotate on any axis (Rotations per second)")]
        float cubeMinRotationsPerSec = 0;

        [SerializeField, Min(0), Tooltip("The maximum speed with which the cube can rotate on any axis (Rotations per second)")]
        float cubeMaxRotationsPerSec = 1;

        [Header("Cube Lifetime")]

        [SerializeField, Min(1), Tooltip("The minimum lifetime of a cube (Seconds)")]
        float cubeMinLifetime = 4;

        [SerializeField, Min(1), Tooltip("The maximum lifetime of a cube (Seconds)")]
        float cubeMaxLifetime = 20;

        [Header("Cube Color")]

        [SerializeField, Tooltip("The maximum value of Red (RGB) for the cubes color")]
        [Range(0, 255)]
        int cubeMinRed = 0;

        [SerializeField, Tooltip("The maximum value of Red (RGB) for the cubes color")]
        [Range(0, 255)]
        int cubeMaxRed = 255;

        [SerializeField, Tooltip("The maximum value of Green (RGB) for the cubes color")]
        [Range(0, 255)]
        int cubeMinGreen = 0;

        [SerializeField, Tooltip("The maximum value of Green (RGB) for the cubes color")]
        [Range(0, 255)]
        int cubeMaxGreen = 255;

        [SerializeField, Tooltip("The maximum value of Blue (RGB) for the cubes color")]
        [Range(0, 255)]
        int cubeMinBlue = 0;

        [SerializeField, Tooltip("The maximum value of Blue (RGB) for the cubes color")]
        [Range(0, 255)]
        int cubeMaxBlue = 255;

        [Header("Cube Size")]

        [SerializeField, Min(0.1f), Tooltip("The minimum lifetime of a cube (Seconds)")]
        float cubeMinSize = 0.5f;

        [SerializeField, Min(0.1f), Tooltip("The maximum lifetime of a cube (Seconds)")]
        float cubeMaxSize = 2;

        [Header("Misc Settings")]

#if UNITY_EDITOR
        [SerializeField]
        bool drawSphereRadius = true;
#endif

        [Tooltip("Whether to pool cubes or instantiate new ones")]
        public bool useObjectPooling = true;

        [HideInInspector]
        public List<GameObject> objectPool;

        const int NumPoolBuffer = 10;

        Vector3 deltaRotation;

        delegate void AddNewCube();
        AddNewCube addNewCube;
        #endregion

        void Awake()
        {
            // Calculate the delta rotation on start
            CalculateDeltaRotation();
        }

        void Start()
        {
            SetUpCubeSpawnMethod();

            // Spawn in this.numToSpawn + this.NumPoolBuffer cubes on start
            for (int i = 0; i < numberOfCubes + NumPoolBuffer; i++)
            {
                SpawnNewCube();
            }
        }

        void Update()
        {
            // Rotate spawn area
            transform.Rotate(deltaRotation * Time.deltaTime);

            // Check if need to spawn more cubes
            float numToAdd = numberOfCubes - (transform.childCount - objectPool.Count);
            for (int i = 0; i < numToAdd; i++)
            {
                addNewCube();
            }
        }

        /// <summary>
        /// Method to set the cube spawn method as specified by this.useObjectPooling
        /// </summary>
        void SetUpCubeSpawnMethod()
        {
            if (useObjectPooling)
                addNewCube = GetCubeFromPool;
            else
                addNewCube = SpawnNewCube;
        }

        /// <summary>
        /// Method to calculate the spawn area delta rotation (per second) as a euler angle.
        /// </summary>
        void CalculateDeltaRotation()
        {
            // Calculate rotation direction
            int rotationDirection = 1;

            if (Random.value < 0.5f)
                rotationDirection = -1;

            // Calculate euler rotation
            float eulerRotationX = Random.Range(areaMinRotationsPerSec, areaMaxRotationsPerSec) * 360 * rotationDirection;
            float eulerRotationY = Random.Range(areaMinRotationsPerSec, areaMaxRotationsPerSec) * 360 * rotationDirection;
            float eulerRotationZ = Random.Range(areaMinRotationsPerSec, areaMaxRotationsPerSec) * 360 * rotationDirection;

            deltaRotation = new Vector3(eulerRotationX, eulerRotationY, eulerRotationZ);
        }

        /// <summary>
        /// Method to spawn (instantiate) a new cube with parameter constrained random properties
        /// </summary>
        void SpawnNewCube()
        {
            // Instantiate a new cube
            GameObject newCube = Instantiate(cubePrefab, transform);

            // Set the cubes parameters
            CubeController cubeController = newCube.GetComponent<CubeController>();
            SetCubeToRandom(cubeController);
            cubeController.spawnAreaController = this;
        }

        /// <summary>
        /// Method to get a cube from the pool and re-randomise its properties.<br></br>
        /// If no cube is available in the pool, a new one will be instantiated (via SpawnNewCube)
        /// </summary>
        void GetCubeFromPool()
        {
            // Guard clause if object pool was empty
            if (objectPool.Count == 0)
            {
                SpawnNewCube();
                return;
            }

            // Get the first cube from the pool
            GameObject toActivate = objectPool[0];
            CubeController cubeController = toActivate.GetComponent<CubeController>();

            // Randomise cube
            SetCubeToRandom(cubeController);

            // Re-activate the cube, and remove from pool
            toActivate.SetActive(true);
            objectPool.Remove(toActivate);
        }

        /// <summary>
        /// Method to randomise the given <paramref name="cubeController"/> properties.
        /// </summary>
        /// <param name="cubeController"></param>
        void SetCubeToRandom(CubeController cubeController)
        {
            cubeController.SetRandomPosition(radius);
            cubeController.SetRandomColor32(cubeMinRed, cubeMaxRed, cubeMinGreen, cubeMaxGreen, cubeMinBlue, cubeMaxBlue);
            cubeController.SetRandomSize(cubeMinSize, cubeMaxSize);
            cubeController.SetRandomLifetime(cubeMinLifetime, cubeMaxLifetime);
            cubeController.SetRandomDeltaRotation(cubeMinRotationsPerSec, cubeMaxRotationsPerSec);
        }

#if UNITY_EDITOR

        void OnValidate()
        {
            // Update spawn method
            SetUpCubeSpawnMethod();
        }

        void OnDrawGizmosSelected()
        {
            // Guard clause if don't want to draw radius
            if (!drawSphereRadius)
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

#endif

    }
}