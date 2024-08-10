using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DarlingEngine.Engine;
using UnityEngine;


public class TaskManager : Singleton<TaskManager>
{
    private ConcurrentDictionary<string, CancellationTokenSource> cancellationTokenSources = new();

    /// <summary>
    /// Start One UniTask
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="taskFunction"></param>
    public void StartUniTask(string taskId, Func<CancellationToken, UniTask> taskFunction)
    {
        if (cancellationTokenSources.TryRemove(taskId, out var existingCts))
        {
            existingCts.Cancel();
            existingCts.Dispose();
        }

        var cts = new CancellationTokenSource();
        if (cancellationTokenSources.TryAdd(taskId, cts))
        {
            taskFunction(cts.Token).ContinueWith(() => CleanUp(taskId)).Forget();
        }
    }

    /// <summary>
    /// Stop One UniTask
    /// </summary>
    /// <param name="taskId"></param>
    public void StopUniTask(string taskId)
    {
        if (cancellationTokenSources.TryRemove(taskId, out var cts))
        {
            cts.Cancel();
            cts.Dispose();
        }
    }

    /// <summary>
    /// Stop All UniTasks
    /// </summary>
    public void StopAllUniTasks()
    {
        foreach (var kvp in cancellationTokenSources)
        {
            kvp.Value.Cancel();
            kvp.Value.Dispose();
        }
        cancellationTokenSources.Clear();
    }

    private void CleanUp(string taskId)
    {
        if (cancellationTokenSources.TryRemove(taskId, out var cts))
        {
            cts.Dispose();
        }
    }
}
