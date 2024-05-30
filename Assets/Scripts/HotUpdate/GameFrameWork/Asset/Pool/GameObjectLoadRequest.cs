using System;
using UnityEngine;

namespace TGame.Asset
{
    public class GameObjectLoadRequest<T> where T : GameObjectPoolAsset
    {
        public GameObjectLoadState State { get; private set; } //状态
        public string Path { get; } //路径
        public Action<GameObject> CreateNewCallback { get; } //委托

        private Action<T> callback; //回调

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        /// <param name="createNewCallback"></param>
        public GameObjectLoadRequest(string path, Action<T> callback, Action<GameObject> createNewCallback)
        {
            Path = path;
            this.callback = callback;
            CreateNewCallback = createNewCallback;
        }

        /// <summary>
        /// 加载完成
        /// </summary>
        /// <param name="obj"></param>
        public void LoadFinish(T obj)
        {
            //是加载中状态
            if (State == GameObjectLoadState.Loading)
            {
                //执行回调函数
                callback?.Invoke(obj);
                //设置状态
                State = GameObjectLoadState.Finish;
            }
        }
    }
}
