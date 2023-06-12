using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Pipe : MonoBehaviour
{
    [SerializeField] private EnemyMovement _enemyPrefab;
    [SerializeField] private float _spawnTimer = 3f;

    private ObjectPool<EnemyMovement> _enemyPool;
    private ColorChanger _colorChanger;

    private void Awake() {
        _colorChanger = GetComponent<ColorChanger>();
        CreateEnemyPool();
    }

    private void Start() {
        StartCoroutine(SpawnRoutine());
    }

    public void ReleaseEnemyFromPool(EnemyMovement enemyMovement) {
        _enemyPool.Release(enemyMovement);
    }

    private void CreateEnemyPool()
    {
        _enemyPool = new ObjectPool<EnemyMovement>(() =>
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
            if (MechanicsManager.Instance.ObjectPoolingToggle) {
                EnemyMovement newEnemy = _enemyPool.Get();
                newEnemy.transform.position = this.transform.position;
                Health enemyHealth = newEnemy.GetComponent<Health>();
                enemyHealth.EnemyInit(this);
                ColorChanger newEnemyColorChanger = newEnemy.GetComponent<ColorChanger>();
                newEnemyColorChanger.SetColor(_colorChanger.CurrentColor);
            } else {
                Instantiate(_enemyPrefab, transform.position, transform.rotation);
            }

            float randomTimeAdj = Random.Range(-1f, 1);
            float spawnWaitTime = _spawnTimer + randomTimeAdj;
            if (spawnWaitTime <= 0)
            {
                spawnWaitTime = 0.1f;
            }
            yield return new WaitForSeconds(spawnWaitTime);
        }
    }
}
