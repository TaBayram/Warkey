using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class ThreadDataRequest : MonoBehaviour
{
    static ThreadDataRequest instance;
    Queue<ThreadInfo> dataQueue = new Queue<ThreadInfo>();

    private void Awake() {
        instance = FindObjectOfType<ThreadDataRequest>();
    }
    public static void RequestData(Func<object> generateData, Action<object> callback) {
        ThreadStart threadStart = delegate {
            instance.DataThread(generateData, callback);
        };
        Thread thread = new Thread(threadStart);
        thread.Start();
    }

    public void DataThread(Func<object> generateData, Action<object> callback) {
        object data = generateData();
        lock (dataQueue) {
            dataQueue.Enqueue(new ThreadInfo(callback, data));
        }
    }

   
    private void Update() {
        if (dataQueue.Count > 0) {
            for (int i = 0; i < dataQueue.Count; i++) {
                ThreadInfo threadInfo = dataQueue.Dequeue();
                threadInfo.callback(threadInfo.paramenter);
            }
        }
    }

    struct ThreadInfo
    {
        public readonly Action<object> callback;
        public readonly object paramenter;

        public ThreadInfo(Action<object> callback, object paramenter) {
            this.callback = callback;
            this.paramenter = paramenter;
        }
    }

}
