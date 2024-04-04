using System;
using System.Runtime.CompilerServices;
using Windows.Foundation;

public static class AVUwpAwait
{
    public static TaskAwaiter GetAwaiter(this IAsyncAction source)
    {
        return source.AsTask().GetAwaiter();
    }

    public static TaskAwaiter GetAwaiter<TProgress>(this IAsyncActionWithProgress<TProgress> source)
    {
        return source.AsTask().GetAwaiter();
    }

    public static TaskAwaiter<TResult> GetAwaiter<TResult>(this IAsyncOperation<TResult> source)
    {
        return source.AsTask().GetAwaiter();
    }

    public static TaskAwaiter<TResult> GetAwaiter<TResult, TProgress>(this IAsyncOperationWithProgress<TResult, TProgress> source)
    {
        return source.AsTask().GetAwaiter();
    }
}