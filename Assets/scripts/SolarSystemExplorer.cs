using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystemExplorer : MonoBehaviour
{
    [System.Serializable]
    public struct PlanetData
    {
        public string name;
        public GameObject prefab;
        public float orbitRadius;
        public float orbitSpeed;
        public float selfRotationSpeed;
        public float sizeScale;
        public Color fallbackColor;
    }

    [Header("Sun Settings")]
    public GameObject sunPrefab;
    public float sunScale = 6f; 
    public Color sunFallbackColor = new Color(1f, 0.6f, 0f);

    [Header("Space Skybox Background")]
    [Tooltip("Seret material Skybox.mat ke sini untuk latar belakang angkasa 3D")]
    public Material spaceSkyboxMaterial;
    
    [Range(0.1f, 3f)]
    public float skyboxExposure = 0.6f; 

    public Color skyboxTint = Color.white;

    [Tooltip("Kecepatan berputarnya latar belakang angkasa (derajat per detik)")]
    public float skyboxRotationSpeed = 3f; 

    [Header("Planets Config")]
    public PlanetData[] planets;

    [Header("Orbit Visualization")]
    public Material orbitMaterial;
    public float orbitLineWidth = 0.06f;
    public int orbitSegments = 100; 

    private List<SpawnedPlanetInstance> spawnedInstances = new List<SpawnedPlanetInstance>();
    private float currentSkyboxRotation = 0f;

    private class SpawnedPlanetInstance
    {
        public Transform transform;
        public float orbitRadius;
        public float orbitSpeed;
        public float selfRotationSpeed;
        public float currentAngle;
    }

    void Start()
    {
        ConfigureSpaceEnvironment();
        SpawnSolarSystem();
    }

    void ConfigureSpaceEnvironment()
    {
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.15f, 0.15f, 0.2f); 

        if (spaceSkyboxMaterial != null)
        {
            if (spaceSkyboxMaterial.HasProperty("_Tint"))
            {
                spaceSkyboxMaterial.SetColor("_Tint", skyboxTint);
            }
            if (spaceSkyboxMaterial.HasProperty("_Exposure"))
            {
                spaceSkyboxMaterial.SetFloat("_Exposure", skyboxExposure);
            }

            RenderSettings.skybox = spaceSkyboxMaterial;
            DynamicGI.UpdateEnvironment();
        }
    }

    void SpawnSolarSystem()
    {
        // 1. Buat Bola Sempurna Standar untuk Matahari (Menghindari prefab URP yang rusak)
        GameObject sunObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sunObj.transform.SetParent(this.transform);
        sunObj.transform.localPosition = Vector3.zero;
        sunObj.transform.localRotation = Quaternion.identity;
        sunObj.name = "Sun";
        sunObj.transform.localScale = Vector3.one * sunScale;

        // Hilangkan collider bawaan agar tidak menghalangi raycast kamera
        Collider sunCollider = sunObj.GetComponent<Collider>();
        if (sunCollider != null)
        {
            Destroy(sunCollider);
        }

        // Terapkan material Standard dengan EMISSION (Self-Illumination) berwarna kuning-oranye membara
        Renderer rend = sunObj.GetComponent<Renderer>();
        if (rend != null)
        {
            Material sunMat = new Material(Shader.Find("Standard"));
            sunMat.color = new Color(1f, 0.4f, 0.02f, 1f); // Warna oranye matahari
            sunMat.SetColor("_EmissionColor", new Color(1.8f, 0.85f, 0.08f, 1f)); // Kuning-oranye menyala terang
            sunMat.EnableKeyword("_EMISSION");
            
            sunMat.SetFloat("_Glossiness", 0.1f);
            sunMat.SetFloat("_Metallic", 0.0f);
            rend.material = sunMat;
        }

        // 2. Tambahkan Point Light di pusat matahari
        GameObject lightObj = new GameObject("Sun_Light");
        lightObj.transform.SetParent(sunObj.transform);
        lightObj.transform.localPosition = Vector3.zero;
        Light pointLight = lightObj.AddComponent<Light>();
        pointLight.type = LightType.Point;
        pointLight.range = 250f;
        pointLight.intensity = 6f; 
        pointLight.shadows = LightShadows.Soft;

        // 3. Spawn each planet relative to this transform
        if (planets == null || planets.Length == 0) return;

        for (int i = 0; i < planets.Length; i++)
        {
            PlanetData data = planets[i];
            GameObject planetObj;

            if (data.prefab != null)
            {
                planetObj = Instantiate(data.prefab, this.transform);
            }
            else
            {
                planetObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                planetObj.transform.SetParent(this.transform);
                Renderer rendPlanet = planetObj.GetComponent<Renderer>();
                if (rendPlanet != null)
                {
                    rendPlanet.material = new Material(Shader.Find("Sprites/Default"));
                    rendPlanet.material.color = data.fallbackColor;
                }
            }

            planetObj.name = data.name;
            planetObj.transform.localScale = Vector3.one * data.sizeScale;

            // Buat instance pelacak pergerakan
            SpawnedPlanetInstance inst = new SpawnedPlanetInstance();
            inst.transform = planetObj.transform;
            inst.orbitRadius = data.orbitRadius;
            inst.orbitSpeed = data.orbitSpeed;
            inst.selfRotationSpeed = data.selfRotationSpeed;
            inst.currentAngle = Random.Range(0f, 360f);
            
            // Set posisi awal lokal
            float rad = inst.currentAngle * Mathf.Deg2Rad;
            inst.transform.localPosition = new Vector3(Mathf.Sin(rad) * inst.orbitRadius, 0, Mathf.Cos(rad) * inst.orbitRadius);

            spawnedInstances.Add(inst);

            // Buat visualisasi garis orbit
            CreateOrbitLine(data.orbitRadius);
        }
    }

    void CreateOrbitLine(float radius)
    {
        GameObject orbitObj = new GameObject("Orbit_Line_" + radius);
        orbitObj.transform.SetParent(this.transform);
        orbitObj.transform.localPosition = Vector3.zero;
        orbitObj.transform.localRotation = Quaternion.identity;

        LineRenderer line = orbitObj.AddComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.positionCount = orbitSegments + 1;
        line.startWidth = orbitLineWidth;
        line.endWidth = orbitLineWidth;

        if (orbitMaterial != null)
        {
            line.material = orbitMaterial;
        }
        else
        {
            Shader lineShader = Shader.Find("Hidden/Internal-Colored");
            if (lineShader != null)
            {
                line.material = new Material(lineShader);
            }
            else
            {
                line.material = new Material(Shader.Find("Sprites/Default"));
            }
            
            Color neonCyan = new Color(0.2f, 0.55f, 0.85f, 0.45f);
            line.startColor = neonCyan;
            line.endColor = neonCyan;
        }

        float angleStep = 360f / orbitSegments;
        for (int i = 0; i <= orbitSegments; i++)
        {
            float currentAngle = i * angleStep;
            float rad = currentAngle * Mathf.Deg2Rad;
            float x = Mathf.Sin(rad) * radius;
            float z = Mathf.Cos(rad) * radius;
            line.SetPosition(i, new Vector3(x, 0, z));
        }
    }

    void Update()
    {
        // 1. Gerakkan planet-planet di orbitnya
        for (int i = 0; i < spawnedInstances.Count; i++)
        {
            SpawnedPlanetInstance inst = spawnedInstances[i];
            if (inst.transform == null) continue;

            inst.currentAngle += inst.orbitSpeed * Time.deltaTime;
            if (inst.currentAngle >= 360f) inst.currentAngle -= 360f;

            float rad = inst.currentAngle * Mathf.Deg2Rad;
            float x = Mathf.Sin(rad) * inst.orbitRadius;
            float z = Mathf.Cos(rad) * inst.orbitRadius;
            inst.transform.localPosition = new Vector3(x, 0, z);

            inst.transform.Rotate(Vector3.up, inst.selfRotationSpeed * Time.deltaTime, Space.Self);
        }

        // 2. Putar latar belakang Bimasakti secara perlahan dan dinamis
        if (spaceSkyboxMaterial != null && spaceSkyboxMaterial.HasProperty("_Rotation"))
        {
            currentSkyboxRotation += skyboxRotationSpeed * Time.deltaTime;
            if (currentSkyboxRotation >= 360f) currentSkyboxRotation -= 360f;
            spaceSkyboxMaterial.SetFloat("_Rotation", currentSkyboxRotation);
        }
    }

    void Reset()
    {
        planets = new PlanetData[]
        {
            new PlanetData { name = "Mercury", orbitRadius = 9f, orbitSpeed = 30f, selfRotationSpeed = 30f, sizeScale = 0.8f, fallbackColor = Color.gray },
            new PlanetData { name = "Venus", orbitRadius = 14f, orbitSpeed = 24f, selfRotationSpeed = 25f, sizeScale = 1.5f, fallbackColor = new Color(0.9f, 0.7f, 0.5f) },
            new PlanetData { name = "Earth", orbitRadius = 19f, orbitSpeed = 18f, selfRotationSpeed = 35f, sizeScale = 1.6f, fallbackColor = Color.blue },
            new PlanetData { name = "Mars", orbitRadius = 24f, orbitSpeed = 14f, selfRotationSpeed = 30f, sizeScale = 1.2f, fallbackColor = Color.red },
            new PlanetData { name = "Jupiter", orbitRadius = 34f, orbitSpeed = 9f, selfRotationSpeed = 45f, sizeScale = 3.4f, fallbackColor = new Color(0.8f, 0.5f, 0.3f) },
            new PlanetData { name = "Saturn", orbitRadius = 45f, orbitSpeed = 6f, selfRotationSpeed = 40f, sizeScale = 2.8f, fallbackColor = new Color(0.9f, 0.8f, 0.6f) },
            new PlanetData { name = "Uranus", orbitRadius = 54f, orbitSpeed = 4f, selfRotationSpeed = 25f, sizeScale = 2.2f, fallbackColor = Color.cyan },
            new PlanetData { name = "Neptune", orbitRadius = 63f, orbitSpeed = 3f, selfRotationSpeed = 20f, sizeScale = 2.0f, fallbackColor = new Color(0.1f, 0.2f, 0.8f) }
        };
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (spaceSkyboxMaterial == null)
        {
            spaceSkyboxMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Planets of the Solar System 3D/Materials/Skybox.mat");
        }
        if (sunPrefab == null)
        {
            sunPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Planets of the Solar System 3D/Prefabs/Sun.prefab");
        }

        if (planets == null || planets.Length == 0 || planets[0].orbitRadius < 6f)
        {
            Reset();
        }

        for (int i = 0; i < planets.Length; i++)
        {
            if (planets[i].prefab == null)
            {
                string path = "Assets/Planets of the Solar System 3D/Prefabs/" + planets[i].name + ".prefab";
                GameObject loadedPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (loadedPrefab != null)
                {
                    planets[i].prefab = loadedPrefab;
                }
            }
        }
    }
#endif
}
