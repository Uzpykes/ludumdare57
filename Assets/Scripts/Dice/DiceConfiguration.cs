using UnityEngine;

public class DiceConfiguration
{
    public DieFaceConfiguration[] faces;
    public Vector3Int randomizedRotation;
    public Quaternion rotation;
    public Vector3 position;

    private DiceConfiguration() { }

    public static DiceConfiguration Create(int depth)
    {
        var c = new DiceConfiguration();
        c.faces = new DieFaceConfiguration[6];
        var r = GetDiceRangeForDepth(depth);

        for (int i = 0; i < 6; i++)
        {
            var slots = new DiceFaceSlot[i + 1];
            slots[0] = DiceFaceSlot.Hole;
            for (int s = 1; s < slots.Length; s++)
            {
                slots[s] = (DiceFaceSlot)Random.Range(r.from, r.to + 1);
            }

            c.faces[i] = new DieFaceConfiguration() { EyeCount = i + 1, EyeType = slots };
        }

        c.randomizedRotation = new Vector3Int(Random.Range(0, 9) * 90, Random.Range(0, 9) * 90, Random.Range(0, 9) * 90);
        c.position = Vector3.up * 2f;

        return c;
    }

    private static (int from, int to) GetDiceRangeForDepth(int depth)
    {
        return (Mathf.Max(2, Mathf.Min(6, (depth))), Mathf.Max(2, Mathf.Min(8, depth + 2)));
    }
}

public class DieFaceConfiguration
{
    public int EyeCount;
    public bool EmptyFaceRerollProvided = false;
    public DiceFaceSlot[] EyeType;
}

public enum DiceFaceSlot
{
    None = 0,
    Hole = 1,
    OneBall = 2,
    TwoBall = 3,
    ThreeBall = 4,
    FourBall = 5,
    FiveBall = 6,
    SixBall = 7,
    SevenBall = 8
}