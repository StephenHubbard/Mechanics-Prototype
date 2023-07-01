using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Pipe : MonoBehaviour
{
    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private float _spawnTimer = 3f;
    [SerializeField] private float _randomizeSpawnTimerModifer = 1f;

    private ObjectPool<Enemy> _enemyPool;
    private ColorChanger _colorChanger;

    private void Awake() {
        _colorChanger = GetComponent<ColorChanger>();
        CreateEnemyPool();
    }

    private void Start() {
        StartCoroutine(SpawnRoutine());
    }

    public void ReleaseEnemyFromPool(Enemy enemyMovement) {
        _enemyPool.Release(enemyMovement);
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
        Enemy newEnemy;

        while (true)
        {
            newEnemy = _enemyPool.Get();
            newEnemy.transform.position = this.transform.position;

            Health enemyHealth = newEnemy.GetComponent<Health>();
            enemyHealth.EnemyInit(this);

            ColorChanger newEnemyColorChanger = newEnemy.GetComponent<ColorChanger>();
            newEnemyColorChanger.SetColor(_colorChanger.CurrentColor);

            float randomTimeModifier = Random.Range(-_randomizeSpawnTimerModifer, _randomizeSpawnTimerModifer);
            float spawnWaitTime = _spawnTimer + randomTimeModifier;
            float minSpawnWaitTime = 0.1f;
            if (spawnWaitTime <= 0)
            {
                spawnWaitTime = minSpawnWaitTime;
            }
           
            yield return new WaitForSeconds(spawnWaitTime);
            _colorChanger.SetRandomColor();
        }

    }
}
