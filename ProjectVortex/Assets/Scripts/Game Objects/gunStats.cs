using UnityEngine;

[CreateAssetMenu]
public class gunStats : ScriptableObject
{
    public GameObject gunModel;
    public GameObject bullet;
    [Range(1, 10)] public int shootDamage;
    [Range(1, 100)] public int shootSpeed;
    [Range(1, 100)] public int shootTime;
    [Range(0.1f, 3)] public float shootRate;
    [Range(0.01f, 1)] public float spread;
    [Range(1, 100)] public int bulletCount;
    [Range(0, 10)] public float kickBack;
    public bool isInfinite;
    public int ammoCur;
    [Range(5, 50)] public int ammoMax;
}
