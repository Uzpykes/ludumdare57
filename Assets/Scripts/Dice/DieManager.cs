using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DieManager : MonoBehaviour
{
    public static DieManager Instance;

    public Die die;

    private Die instance;

    [SerializeField]
    private DieEyeLookup mapping;

    private Dictionary<DiceFaceSlot, ABaseEye> eyeMapping;
    [SerializeField]

    private InventoryManager inventoryManager;

    private DiceConfigurationTree tree;
    private DiceConfiguration current;
    [SerializeField]
    private MainCameraController cameraController;
    [SerializeField]
    private PositioningController positioningController;
    [SerializeField]
    private StateData stateData;

    private int depth = -1;
    private bool dieHasSettled = false;
    private bool wasThrown = false;

    public Action onDieThrow;


    void Awake()
    {
        Instance = this;
        eyeMapping = new Dictionary<DiceFaceSlot, ABaseEye>();
        foreach (var m in mapping.eyeMappings)
        {
            eyeMapping[m.diceFaceSlot] = m.eyePrefab;
        }

        tree = new();
    }

    void OnGUI()
    {
        GUILayout.Label($"Depth: {stateData.currentDepth}, Hit Chances: {stateData.currentFreeHits}, Rerolls: {stateData.currentRerolls}, Score: {stateData.currentScore}");

        if (stateData.currentDepth == 0 && GUILayout.Button("Spawn Die"))
        {
            if (instance != null)
                Destroy(instance.gameObject);
            stateData.AddDepth(1);
            ThrowDie(CreateNewDie(DiceConfiguration.Create(stateData.currentDepth), 0, 0));
        }

        if ((current != null) && GUILayout.Button("Return"))
        {
            ReturnToPreviousDepth();
        }

        if (stateData.currentRerolls > 0 && GUILayout.Button("Reroll"))
        {
            stateData.AddReroll(-1);
            ThrowDie(instance);
        }
    }

    private void CleanupCurrent()
    {
        if (instance != null)
        {
            current.position = instance.transform.position;
            current.rotation = instance.transform.rotation;
            Destroy(instance.gameObject);
        }
    }

    private Die CreateNewDie(DiceConfiguration con, int faceIndex, int eyeIndex)
    {
        var config = con;
        depth++;
        if (current == null)
            tree.AddRoot(config);
        else
        {
            tree.Add(config, current, faceIndex, eyeIndex);
        }
        ApplyConfiguration(config);
        return instance;
    }

    private void ThrowDie(Die d)
    {
        d.rb.excludeLayers = 1 << LayerMask.NameToLayer("PoolBall") | 1 << LayerMask.NameToLayer("PlayerBall");
        if (throwTime != null)
        {
            StopCoroutine(throwTime);
            throwTime = null;
        }
        throwTime = StartCoroutine(ThrowTime());

        onDieThrow?.Invoke();
        dieHasSettled = false;
        wasThrown = true;

        var ins = instance.GetTopFace().instances;
        foreach (var i in ins)
        {
            if (i != null)
                i.SetNonInteractable();
        }

        var rb = d.rb;
        rb.isKinematic = false;
        rb.AddExplosionForce(5, Vector3.zero + Vector3.one * UnityEngine.Random.Range(0.1f, 0.2f), 10, 1f, ForceMode.Impulse);
        rb.AddTorque(UnityEngine.Random.Range(.2f, .3f), UnityEngine.Random.Range(-.1f, 0.1f), UnityEngine.Random.Range(.5f, .1f), ForceMode.Impulse);
    }


    // private void CreateDieForInventory()
    // {
    //     var config = DiceConfiguration.Create();

    //     var d = Instantiate(die, Vector3.zero, Quaternion.identity);
    //     d.SetConfiguration(config, eyeMapping);

    //     inventoryManager.AddItem(d.gameObject);
    // }

    private void ReturnToPreviousDepth()
    {
        var prev = tree.GetParentConfiguration(current);
        if (current != null)
        {
            stateData.AddDepth(-1);
            CleanupCurrent();
            ApplyConfiguration(prev);
        }
    }

    private void ApplyConfiguration(DiceConfiguration config)
    {
        current = config;
        var d = Instantiate(die, config.position, config.rotation);
        d.SetConfiguration(current, eyeMapping);
        dieHasSettled = false;
        instance = d;
    }

    public void RequestDiceTransition(EmptyEye eye, Action onComplete)
    {
        cameraController.PlayDiceZoomInTransition(eye.transform, () => SwapDice(eye, onComplete));
    }

    public void RequestDiceZoomOutTransition(Transform target, Action oncomplete)
    {
        cameraController.PlayDiceZoomOutTransition(target, () => ReturnToPreviousDepth());
    }

    private void SwapDice(EmptyEye eye, Action onComplete)
    {
        CleanupCurrent();

        var c = tree.GetChildConfiguration(current, eye.faceIndex, eye.eyeIndex);
        if (c == null)
        {
            stateData.AddDepth(1);
            var config = DiceConfiguration.Create(stateData.currentDepth);
            ThrowDie(CreateNewDie(config, eye.faceIndex, eye.eyeIndex));
        }
        else
        {
            ApplyConfiguration(c);
        }
        onComplete();
    }

    private Coroutine throwTime;
    private Coroutine trackingCoroutine;

    private void FixedUpdate()
    {
        if (!dieHasSettled && instance != null && trackingCoroutine == null && throwTime == null)
        {
            if (instance.rb.linearVelocity.sqrMagnitude < 0.01f && instance.rb.angularVelocity.sqrMagnitude < 0.01f)
            {
                trackingCoroutine = StartCoroutine(TrackDie());
            }
        }
    }

    private IEnumerator ThrowTime()
    {
        yield return new WaitForSeconds(1f);
        throwTime = null;
    }

    // Checks if die is at rest state
    private IEnumerator TrackDie()
    {
        if (throwTime != null)
            yield return throwTime;

        var t = 0f;
        var restCooldown = 0.5f;

        while (t < restCooldown)
        {
            if (instance.rb.linearVelocity.sqrMagnitude > 0.1f || instance.rb.angularVelocity.sqrMagnitude > 0.1f)
            {
                trackingCoroutine = null;
                yield break;
            }
            t += Time.deltaTime;
            yield return null;
        }


        instance.rb.excludeLayers = 0;
        instance.rb.isKinematic = true;

        var pos = instance.transform.position;

        current.position = instance.transform.position;
        current.rotation = instance.transform.rotation;
        yield return new WaitForFixedUpdate();
        yield return StartCoroutine(RecenterObject(instance.rb, new Vector3(0, pos.y, 0)));
        yield return new WaitForFixedUpdate();

        positioningController.Enable(instance);
        var ins = instance.GetTopFace().instances;
        foreach (var i in ins)
        {
            if (wasThrown)
            {
                // If die was thrown, then add hit chances based  on how many balls there are on the facev
                var face = current.faces[i.faceIndex];
                var type = face.EyeType[i.eyeIndex];
                if (type != DiceFaceSlot.None)
                {
                    stateData.AddFreeHits(1);
                }
            }
            i.SetInteractable();
        }

        EvaluateFaceClearance();


        dieHasSettled = true;

        wasThrown = false;

        trackingCoroutine = null;
    }

    private IEnumerator RecenterObject(Rigidbody obj, Vector3 targetPos)
    {
        var t = 0f;
        var restCooldown = 0.3f;
        var startPos = obj.position;

        while (t < restCooldown)
        {
            obj.MovePosition(Vector3.Lerp(startPos, targetPos, t / restCooldown));
            t += Time.deltaTime;
            yield return null;
        }
        obj.MovePosition(targetPos);
        yield return null;
    }

    //Check if face has no more balls left
    public void EvaluateFaceClearance()
    {
        var topFace = instance.GetTopFace();
        var topFaceInstances = topFace.instances;
        var poolBallCount = 0;

        var config = current.faces[topFace.dieEyes.Count - 1];
        if (config.EmptyFaceRerollProvided) return;

        foreach (var i in topFaceInstances)
        {
            if (i == null) continue;
            var type = current.faces[i.faceIndex].EyeType[i.eyeIndex];
            if (type != DiceFaceSlot.None & type != DiceFaceSlot.Hole)
                poolBallCount++;
        }

        if (poolBallCount == 0)
        {
            stateData.AddReroll(1);
            config.EmptyFaceRerollProvided = true;
        }
    }


    public void ReportBallClearance(int faceIndex, int eyeIndex)
    {
        var type = current.faces[faceIndex].EyeType[eyeIndex];
        current.faces[faceIndex].EyeType[eyeIndex] = DiceFaceSlot.None;

        stateData.AddScore(stateData.currentDepth * (((int)type) - 1));
        stateData.AddBall(type);
    }
}
