using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Splines;

public class PositioningController : MonoBehaviour
{
    private Plane plane;
    [SerializeField]
    private SplineContainer splineContainer;
    private bool isActive = false;
    [SerializeField]
    private GameObject poolGhost;
    [SerializeField]
    private GameObject poolPrefab;

    private Die targetDie;

    private GameObject poolGhostInstance;
    private GameObject poolBallInstance;

    void Awake()
    {
        plane = new Plane(transform.up, transform.position);
        poolGhostInstance = Instantiate(poolGhost);
        poolBallInstance = Instantiate(poolPrefab);
        poolGhostInstance.SetActive(false);
        poolBallInstance.SetActive(false);
    }

    public void Enable(Die die)
    {
        targetDie = die;
        isActive = true;
        var faceTransform = die.GetTopFace().facePlane;

        transform.position = faceTransform.position;
        transform.rotation = faceTransform.rotation;
        plane.SetNormalAndPosition(faceTransform.up, faceTransform.position);
        poolGhostInstance.SetActive(true);

        InputSystem.actions["Fire"].performed += HandleSpawn;
    }


    void Update()
    {
        if (isActive)
        {
            HandlePlacement();
        }
    }

    private void HandlePlacement()
    {
        var r = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(r, out float enter))
        {
            var pos = r.GetPoint(enter);
            SplineUtility.GetNearestPoint(splineContainer[0], transform.InverseTransformPoint(pos), out float3 nearest, out float t, resolution: 6, iterations: 4);
            nearest = transform.TransformPoint(nearest);
            poolGhostInstance.transform.position = nearest;
#if UNITY_EDITOR
            Debug.DrawRay(nearest, Vector3.up, Color.red);
#endif
        }
    }

    private void HandleSpawn(InputAction.CallbackContext context)
    {
        if (isActive)
        {
            SpawnBall();
        }
    }

    private void SpawnBall()
    {
        isActive = false;
        poolGhostInstance.SetActive(false);
        poolBallInstance.transform.position = poolGhostInstance.transform.position + Vector3.up * 0.05f;
        poolBallInstance.SetActive(true);
        InputSystem.actions["Fire"].performed -= HandleSpawn;
    }

    private void OnDestroy()
    {
        InputSystem.actions["Fire"].performed -= HandleSpawn;
    }
}
