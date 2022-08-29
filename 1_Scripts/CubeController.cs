using System.Collections;
using UnityEngine;

namespace Assessment
{
    public class CubeController : MonoBehaviour
    {
        [HideInInspector]
        public SpawnAreaController spawnAreaController;

        float remainingLifetime;
        Vector3 deltaRotation;

        Renderer cubeRenderer;
        MaterialPropertyBlock materialPropertyBlock;

        delegate void RetireCube();
        RetireCube retireCube;

        void Awake()
        {
            cubeRenderer = transform.GetComponent<Renderer>();
            materialPropertyBlock = new MaterialPropertyBlock();
        }

        void Start()
        {
            // Set up cube retire method
            if (spawnAreaController.useObjectPooling)
                retireCube = ReturnCubeToPool;
            else
                retireCube = DestroyCube;
        }

        void Update()
        {
            // Check remaining life
            if (remainingLifetime <= 0)
            {
                cubeRenderer.GetPropertyBlock(materialPropertyBlock);
                Color color = materialPropertyBlock.GetColor("_Color");
                float alpha = color.a;

                // Check if fully faded
                if (alpha <= 0)
                {
                    retireCube();
                    return;
                }

                // Update color with new decremented alpha
                alpha -= spawnAreaController.cubeFadeDelay * Time.deltaTime;
                materialPropertyBlock.SetColor("_Color", new Color(color.r, color.g, color.b, alpha));
                cubeRenderer.SetPropertyBlock(materialPropertyBlock);
            }

            // Rotate cube by deltaRotation
            transform.Rotate(deltaRotation * Time.deltaTime);

            // Decrement remainingLifetime
            remainingLifetime -= Time.deltaTime;
        }

        /// <summary>
        /// Method to return this cube back to the object pool
        /// </summary>
        void ReturnCubeToPool()
        {
            // Deactivate object
            gameObject.SetActive(false);

            // Register in pool
            spawnAreaController.objectPool.Add(gameObject);
        }

        /// <summary>
        /// Method to destroy this cube
        /// </summary>
        void DestroyCube()
        {
            // Destroy the cube game object
            Destroy(gameObject);
        }

        /// <summary>
        /// Method to set this cubes local position to a random point within <paramref name="radius"/> sphere
        /// </summary>
        /// <param name="radius"></param>
        public void SetRandomPosition(float radius)
        {
            Vector3 position = Random.insideUnitSphere * radius;
            transform.localPosition = position;
        }

        /// <summary>
        /// Method to set this cubes material color R, G and B to a random integer between: 
        /// <paramref name="rMin"/> to <paramref name="rMax"/>,
        /// <paramref name="gMin"/> to <paramref name="gMax"/> and
        /// <paramref name="bMin"/> to <paramref name="bMax"/> respectively
        /// </summary>
        /// <param name="rMin"></param>
        /// <param name="rMax"></param>
        /// <param name="gMin"></param>
        /// <param name="gMax"></param>
        /// <param name="bMin"></param>
        /// <param name="bMax"></param>
        public void SetRandomColor32(int rMin, int rMax, int gMin, int gMax, int bMin, int bMax)
        {
            byte r = (byte)Random.Range(rMin, rMax + 1);
            byte g = (byte)Random.Range(gMin, gMax + 1);
            byte b = (byte)Random.Range(bMin, bMax + 1);

            cubeRenderer.GetPropertyBlock(materialPropertyBlock);
            materialPropertyBlock.SetColor("_Color", new Color32(r, g, b, 255));
            cubeRenderer.SetPropertyBlock(materialPropertyBlock);
        }

        /// <summary>
        /// Method to set this cube size (scale) to random between <paramref name="minSize"/> and <paramref name="maxSize"/>
        /// </summary>
        /// <param name="minSize"></param>
        /// <param name="maxSize"></param>
        public void SetRandomSize(float minSize, float maxSize)
        {
            float scale = Random.Range(minSize, maxSize);

            transform.localScale = new Vector3(scale, scale, scale);
        }

        /// <summary>
        /// Method to set this cube delta rotation to random between <paramref name="minSpeed"/> and <paramref name="maxSpeed"/>
        /// </summary>
        /// <param name="minSpeed"></param>
        /// <param name="maxSpeed"></param>
        public void SetRandomDeltaRotation(float minSpeed, float maxSpeed)
        {
            // Calculate rotation direction
            int rotationDirection = 1;

            if (Random.value < 0.5f)
                rotationDirection = -1;

            // Calculate euler rotation
            float eulerRotationX = Random.Range(minSpeed, maxSpeed) * 360 * rotationDirection;
            float eulerRotationY = Random.Range(minSpeed, maxSpeed) * 360 * rotationDirection;
            float eulerRotationZ = Random.Range(minSpeed, maxSpeed) * 360 * rotationDirection;

            deltaRotation = new Vector3(eulerRotationX, eulerRotationY, eulerRotationZ);
        }

        /// <summary>
        /// Method to set this cubes lifetime to random (seconds) between <paramref name="minLifetime"/> and <paramref name="maxLifetime"/>
        /// </summary>
        /// <param name="minLifetime"></param>
        /// <param name="maxLifetime"></param>
        public void SetRandomLifetime(float minLifetime, float maxLifetime)
        {
            remainingLifetime = Random.Range(minLifetime, maxLifetime);
        }
    }
}