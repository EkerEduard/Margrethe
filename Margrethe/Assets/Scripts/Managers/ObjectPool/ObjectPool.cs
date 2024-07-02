using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    [SerializeField] private int poolSize = 100;

    // Словарь, где ключом является префаб, а значением - очередь объектов этого типа
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Метод для получения объекта из пула
    public GameObject GetObject(GameObject prefab)
    {
        // Если пул для данного префаба еще не существует, инициализировать новый пул
        if (poolDictionary.ContainsKey(prefab) == false)
        {
            InititalizeNewPool(prefab);
        }

        // Если в пуле закончились объекты, создать новый объект
        if (poolDictionary[prefab].Count == 0)
        {
            CreateNewObject(prefab);
        }

        // Извлечь объект из очереди, активировать его и вернуть
        GameObject objectToGet = poolDictionary[prefab].Dequeue();
        objectToGet.SetActive(true);
        objectToGet.transform.parent = null;

        return objectToGet;
    }

    // Метод для возврата объекта в пул с задержкой (для избежания ошибок)
    public void ReturnObject(GameObject objectToReturn, float delay = 0.001f) => StartCoroutine(DelayReturn(delay, objectToReturn));

    private IEnumerator DelayReturn(float delay, GameObject objectToReturn)
    {
        yield return new WaitForSeconds(delay);

        ReturnToPool(objectToReturn);
    }

    // Метод для возврата объекта в пул
    private void ReturnToPool(GameObject objectToReturn)
    {
        // Найти оригинальный префаб, из которого был создан объект
        GameObject originalPrefab = objectToReturn.GetComponent<PooledObject>().originalPrefab;

        // Деактивировать объект и вернуть его в очередь пула
        objectToReturn.SetActive(false);
        objectToReturn.transform.parent = transform;

        poolDictionary[originalPrefab].Enqueue(objectToReturn);
    }

    // Метод для инициализации нового пула для указанного префаба
    private void InititalizeNewPool(GameObject prefab)
    {
        poolDictionary[prefab] = new Queue<GameObject>();

        // Создать заданное количество объектов и добавить их в пул
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewObject(prefab);
        }
    }

    // Метод для создания нового объекта и добавления его в пул
    private void CreateNewObject(GameObject prefab)
    {
        GameObject newObject = Instantiate(prefab, transform);
        newObject.AddComponent<PooledObject>().originalPrefab = prefab;
        newObject.SetActive(false);

        poolDictionary[prefab].Enqueue(newObject);
    }
}
