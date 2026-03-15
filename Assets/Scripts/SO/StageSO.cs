using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageSO", menuName = "Scriptable Objects/StageSO")]
public class StageSO : ScriptableObject
{
    public int stage;
    public List<EnemyWave> EnemyWaves;
}

[System.Serializable]
public struct EnemyWave
{
    public EnemyType enemyType;
    public int count;
}