using Mirror;
using UnityEngine;

public class FoodSpawner : NetworkBehaviour
{
    public GameObject foodPrefab;
    public int maxFoodCount = 50;

    // 当服务器启动时开始生成
    public override void OnStartServer()
    {
        InvokeRepeating(nameof(SpawnFood), 1, 0.5f);
    }

    [ServerCallback]
    private void SpawnFood()
    {
        if (GameObject.FindGameObjectsWithTag("Food").Length < maxFoodCount)
        {
            Vector3 randomPos = new Vector3(Random.Range(-20f, 20f), 0.5f, Random.Range(-20f, 20f));

            GameObject food = Instantiate(foodPrefab, randomPos, Quaternion.identity);

            NetworkServer.Spawn(food);
        }
    }
}