using System;
using System.Collections.Generic;
using TGame.Asset;
using UnityEngine;
using UnityEngine.AddressableAssets;


public class GameObjectPool<T> where T : GameObjectPoolAsset
{
    private readonly Dictionary<int, Queue<T>> gameObjectPool = new Dictionary<int, Queue<T>>();
    private readonly List<GameObjectLoadRequest<T>> requests = new List<GameObjectLoadRequest<T>>();
    private readonly Dictionary<int, GameObject> usingObjects = new Dictionary<int, GameObject>();

    /// <summary>
    /// 从路径加载游戏对象
    /// </summary>
    /// <param name="path">加载的路径</param>
    /// <param name="createNewCallback">回调函数</param>
    /// <returns></returns>
    public T LoadGameObject(string path, Action<GameObject> createNewCallback = null)
    {
        //计算哈希值
        int hash = path.GetHashCode();
        //检查对象池是否有相应队列
        if (!gameObjectPool.TryGetValue(hash, out Queue<T> q))
        {
            q = new Queue<T>();
            gameObjectPool.Add(hash, q);
        }
        //检查队列中是否有游戏对象
        if (q.Count == 0)
        {
            //异步加载游戏对象
            GameObject prefab = Addressables.LoadAssetAsync<GameObject>(path).WaitForCompletion();
            //实例化加载的游戏对象
            GameObject go = UnityEngine.Object.Instantiate(prefab);
            //给实例化的游戏对象添加一个类型为 T 的组件，并将其存储在 asset 变量中
            T asset = go.AddComponent<T>();
            //调用 createNewCallback 回调函数（如果存在）
            createNewCallback?.Invoke(go);
            //设置 asset 的 ID 属性为哈希值
            asset.ID = hash;
            go.SetActive(false);
            //将 asset 添加到队列 q 中
            q.Enqueue(asset);
        }
        //从队列中取出游戏对象并返回
        {
            T asset = q.Dequeue();
            OnGameObjectLoaded(asset);
            return asset;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path">需要加载的资源的路径</param>
    /// <param name="callback">每次调用LoadGameObjectAsync，无论是否从缓存里取出的，都会通过这个回调进行通知</param>
    /// <param name="createNewCallback">游戏对象第一次被克隆后调用，对象池取出的复用游戏对象，不会回调</param>
    public void LoadGameObjectAsync(string path, Action<T> callback, Action<GameObject> createNewCallback = null)
    {
        GameObjectLoadRequest<T> request = new GameObjectLoadRequest<T>(path, callback, createNewCallback);
        requests.Add(request);
    }

    /// <summary>
    /// 卸载所有游戏对象
    /// </summary>
    public void UnloadAllGameObjects()
    {
        // 先将所有Request加载完毕
        while (requests.Count > 0)
        {
            //GameManager.Asset.UpdateLoader();
            UpdateLoadRequests();
        }

        // 将所有using Objects 卸载
        if (usingObjects.Count > 0)
        {
            List<int> list = new List<int>();
            //遍历添加到集合
            foreach (var id in usingObjects.Keys)
            {
                list.Add(id);
            }
            //再次遍历这个列表，使用 ID 从 usingObjects 字典中获取对应的 GameObject
            foreach (var id in list)
            {
                GameObject obj = usingObjects[id];
                //卸载对象
                UnloadGameObject(obj);
            }
        }

        // 将所有缓存清掉
        if (gameObjectPool.Count > 0)
        {
            //遍历对象池中的每一个队列（Queue<T>）
            foreach (var q in gameObjectPool.Values)
            {
                //遍历队列中的每一个 asset，并调用 UnityEngine.Object.Destroy 方法销毁其关联的 gameObject
                foreach (var asset in q)
                {
                    UnityEngine.Object.Destroy(asset.gameObject);
                }
                q.Clear();
            }
            //清空对象池
            gameObjectPool.Clear();
        }
    }

    /// <summary>
    /// 卸载游戏对象
    /// </summary>
    /// <param name="go"></param>
    public void UnloadGameObject(GameObject go)
    {
        if (go == null)
            return;

        //获取组件
        T asset = go.GetComponent<T>();
        //是否成功获取
        if (asset == null)
        {
            UnityLog.Warn($"Unload GameObject失败，找不到GameObjectAsset:{go.name}");
            UnityEngine.Object.Destroy(go);
            return;
        }

        //检查对象池
        if (!gameObjectPool.TryGetValue(asset.ID, out Queue<T> q))
        {
            //找不到 创建
            q = new Queue<T>();
            gameObjectPool.Add(asset.ID, q);
        }
        //组件加入队列
        q.Enqueue(asset);
        //从使用对象列表中移除
        usingObjects.Remove(go.GetInstanceID());
        //设置游戏对象的父对象和激活状态
        go.transform.SetParent(TGameFramework.Instance.GetModule<AssetModule>().releaseObjectRoot);
        go.gameObject.SetActive(false);
    }
    /// <summary>
    /// 更新和处理游戏对象加载请求的方法
    /// </summary>
    public void UpdateLoadRequests()
    {
        //检查是否有请求
        if (requests.Count > 0)
        {
            //遍历请求列表
            foreach (var request in requests)
            {
                //计算路径的哈希值
                int hash = request.Path.GetHashCode();
                //检查对象池中是否存在与给定哈希值对应的队列。
                if (!gameObjectPool.TryGetValue(hash, out Queue<T> q))
                {
                    //如果不存在，则创建一个新的队列并将其添加到池中
                    q = new Queue<T>();
                    gameObjectPool.Add(hash, q);
                }
                //队列为空 
                if (q.Count == 0)
                {
                    //异步加载游戏对象
                    Addressables.LoadAssetAsync<GameObject>(request.Path).Completed += (obj) =>
                    {
                        GameObject go = UnityEngine.Object.Instantiate(obj.Result);
                        //添加组件
                        T asset = go.AddComponent<T>();
                        //调用 request.CreateNewCallback 回调函数（如果存在）
                        request.CreateNewCallback?.Invoke(go);
                        //设置 asset 的 ID 属性为哈希值
                        asset.ID = hash;
                        go.SetActive(false);

                        OnGameObjectLoaded(asset);
                        request.LoadFinish(asset);
                    };
                }
                else
                {
                    //从队列中取出一个游戏对象
                    T asset = q.Dequeue();
                    OnGameObjectLoaded(asset);
                    //调用 request.LoadFinish 通知请求完成并传递加载的 asset
                    request.LoadFinish(asset);
                }
            }
            //处理完所有请求后，清空 requests 列表
            requests.Clear();
        }
    }

    /// <summary>
    /// 处理游戏对象加载完成后的操作
    /// </summary>
    /// <param name="asset"></param>
    private void OnGameObjectLoaded(T asset)
    {
        //设置游戏对象的父对象
        asset.transform.SetParent(TGameFramework.Instance.GetModule<AssetModule>().usingObjectRoot);
        //获取游戏对象的唯一实例 ID
        int id = asset.gameObject.GetInstanceID();
        //存储当前正在使用的游戏对象及其对应的 ID。
        usingObjects.Add(id, asset.gameObject);
    }
}

