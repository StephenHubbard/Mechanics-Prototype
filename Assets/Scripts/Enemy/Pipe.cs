using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Pipe : MonoBehaviour
{
    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private float _spawnTimer = 3f;
    [SerializeField] private float _randomizeSpawnTimerModifer = 1f;

    float _minSpawnWaitTime = 0.1f;

    private ObjectPool<Enemy> _enemyPool;
    private ColorChanger _colorChanger;

    private void Awake() {
        _colorChanger = GetComponent<ColorChanger>();
        CreateEnemyPool();
    }

    private void Start() {
        StartCoroutine(SpawnRoutine());
    }

    public void ReleaseEnemyFromPool(Enemy enemy) {
        _enemyPool.Release(enemy);
    }

    private void CreateEnemyPool()
    {
        _enemyPool = new ObjectPool<Enemy>(() =>
        {
            return Instantiate(_enemyPrefab);
        }, enemy =>
        {
            enemy.gameObject.SetActive(true);
        }, enemy =>
        {
            enemy.gameObject.SetActive(false);
        }, enemy =>
        {
            Destroy(enemy);
        }, false, 30, 60);
    }

    private IEnumerator SpawnRoutine() {
        while (true)
        {
            Enemy enemy = _enemyPool.Get();
            enemy.EnemyInit(this, _colorChanger.CurrentColor);

            float randomTimeModifier = Random.Range(-_randomizeSpawnTimerModifer, _randomizeSpawnTimerModifer);
            float spawnWaitTime = Mathf.Max(_spawnTimer + randomTimeModifier, _minSpawnWaitTime);

            yield return new WaitForSeconds(spawnWaitTime);
            _colorChanger.SetRandomColor();
        }
    }
}
