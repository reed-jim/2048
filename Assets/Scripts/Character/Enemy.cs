using UnityEngine;
using UnityEngine.Events;
using PrimeTween;

public enum EnemyState
{
    Inactive,
    Idle,
    Walking,
}

public class Enemy : Human
{
    [SerializeField] private EnemyState enemyState;

    [SerializeField] private IntVariable currentEnemyIndex;
    [SerializeField] private UnityEvent enemySpawnEvent;

    public int EnemyIndex
    {
        // format: enemy{x}
        get => int.Parse(Id[5].ToString());
    }

    public EnemyState EnemyState { get; set; }

    private void Start()
    {
        enemySpawnEvent.AddListener(OnEnemySpawn);

        if (EnemyIndex == currentEnemyIndex.Value)
        {
            SpawnEnemy();
        }
    }

    private void OnEnemySpawn()
    {
        if (EnemyState != EnemyState.Inactive) return;

        if (EnemyIndex == currentEnemyIndex.Value)
        {
            Tween.Delay(5f)
                .OnComplete(() => SpawnEnemy());
        }
    }

    private void SpawnEnemy()
    {
        gameObject.SetActive(true);
        EnemyState = EnemyState.Idle;

        currentEnemyIndex.Value++;
        enemySpawnEvent.Invoke();
    }

    private void SetNextDestination()
    {
        if (EnemyState == EnemyState.Idle)
        {
            
        }
    }
    
    private void GetNextAction()
    {
        if (enemyState == EnemyState.Inactive)
        {
            gameObject.SetActive(true);
        }
    }
}