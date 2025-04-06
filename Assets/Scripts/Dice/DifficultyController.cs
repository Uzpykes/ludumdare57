using System.Collections.Generic;
using UnityEngine;

public class DifficultyController : MonoBehaviour
{
    [SerializeField]
    private List<DifficultyConfig> difficultyConfigs = new();
    private bool isActive = false;

    private Dictionary<GameObject, GameObject> instanceLookup = new();
    private List<GameObject> activeInstances = new();
    [SerializeField]
    private StateData stateData;


    void Awake()
    {
        foreach (var config in difficultyConfigs)
        {
            if (!instanceLookup.ContainsKey(config.difficultyPrefab))
            {
                var instance = Instantiate(config.difficultyPrefab);
                instance.SetActive(false);
                instanceLookup[config.difficultyPrefab] = instance;
            }
        }
    }

    public void Enable(Die die)
    {
        isActive = true;
        var faceTransform = die.GetTopFace().facePlane;

        var depth = stateData.currentDepth;

        foreach (var config in difficultyConfigs)
        {
            if (config.affectedDepths.x <= depth && config.affectedDepths.y >= depth)
            {
                var instance = instanceLookup[config.difficultyPrefab];
                instance.transform.position = faceTransform.position;
                instance.transform.rotation = faceTransform.rotation;
                instance.SetActive(true);
                activeInstances.Add(instance);
            }
        }
    }

    public void Disable()
    {
        isActive = false;
        foreach (var i in activeInstances)
        {
            i.SetActive(false);
        }
        activeInstances.Clear();
    }
}

[System.Serializable]
public class DifficultyConfig
{
    public GameObject difficultyPrefab;
    public Vector2Int affectedDepths;
}
