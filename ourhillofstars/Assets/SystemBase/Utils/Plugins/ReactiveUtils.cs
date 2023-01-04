using System;
using SystemBase.Core;
using UniRx;
using UnityEngine;

namespace SystemBase.Utils
{
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error
    }

    public static class ReactiveUtils
    {
        #region Logging

        public static void Log(object o, LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug:
                case LogLevel.Info:
                    Debug.Log(o);
                    break;

                case LogLevel.Warning:
                    Debug.LogWarning(o);
                    break;

                case LogLevel.Error:
                    Debug.LogError(o);
                    break;
            }
        }

        public static void LogException(Exception e)
        {
            Debug.LogException(e);
        }

        public static IObservable<T> LogOnNext<T>(this IObservable<T> obs, Func<T, string> toString = null, LogLevel level = LogLevel.Info)
        {
            return obs.Do(t =>
            {
                Log(toString == null ? t + "" : toString(t), level);
            });
        }

        public static IObservable<T> LogOnNext<T>(this IObservable<T> obs, string format = null, LogLevel level = LogLevel.Info)
        {
            return obs.Do(t =>
            {
                if (format == null) Log(t, level);
                else Log(string.Format(format, t), level);
            }
            );
        }

        public static IObservable<T> LogError<T, TException>(this IObservable<T> obs, Func<TException, string> toString = null, LogLevel level = LogLevel.Error) where TException : Exception
        {
            return obs.DoError(err =>
            {
                if (err is TException)
                {
                    if (toString != null) Log(toString(err as TException), level);
                    else Log(err.ToString(), level);
                }
            });
        }

        public static IObservable<T> LogError<T>(this IObservable<T> obs, Func<Exception, string> toString = null)
        {
            return obs.DoError(err =>
            {
                if (toString != null) Log(toString(err), LogLevel.Error);
                else LogException(err);
            });
        }

        public static IObservable<T> LogError<T>(this IObservable<T> obs, string errorText)
        {
            return obs.DoError(err => { Log(errorText, LogLevel.Error); });
        }

        public static IObservable<T> LogComplete<T>(this IObservable<T> obs, string message, LogLevel level = LogLevel.Debug)
        {
            return obs.Do(DoNothing, () => { Log(message, level); });
        }

        public static IObservable<T> LogComplete<T>(this IObservable<T> obs, Func<string> messageFac, LogLevel level = LogLevel.Debug)
        {
            return obs.Do(DoNothing, () => { Log(messageFac(), level); });
        }

        #endregion Logging

        #region Timing
        /// <summary>Intercepts a minimum interval between consecutive elements of an
        /// observable sequence.</summary>
        public static IObservable<T> WithInterval<T>(this IObservable<T> source,
            TimeSpan interval, IScheduler scheduler = null)
        {
            return source
                .Scan((Observable.Return(0L), (IObservable<T>)null), (state, x) =>
                {
                    var (previousTimer, _) = state;
                    var timer = (scheduler != null ? Observable.Timer(interval, scheduler)
                        : Observable.Timer(interval)).PublishLast();
                    var delayed = previousTimer.Select(_ => x).Finally(() => timer.Connect());
                    return (timer, delayed);
                })
                .Select(e => e.Item2)
                .Concat();
        }
        #endregion
        
        #region Actions

        /// <summary>
        /// Execute 'onDispose' when the observer disposes its subscription.
        /// The opposite to this is .Finally(Action) which is executed when the observable completes/errors
        /// ? is this actually the same as .Finally(Action) ???
        /// </summary>
        /// <returns>The Observable with dispose action</returns>
        /// <param name="source">Source.</param>
        /// <param name="onDispose">On subscribe.</param>
        /// <typeparam name="TSource">The 1st type parameter.</typeparam>
        public static IObservable<TSource> OnDispose<TSource>(this IObservable<TSource> source, Action onDispose)
        {
            return Observable.Create<TSource>(observer =>
            {
                var subscription = source.Subscribe(observer);
                return Disposable.Create(() =>
                {
                    subscription.Dispose();
                    onDispose();
                });
            });
        }

        public static T AddToLifecycleOf<T>(this T disposable, GameComponent gameComponent)
            where T:IDisposable
        {
            if (gameComponent != null)
            {
                return gameComponent.AddDisposablele(disposable);
            }
            disposable.Dispose();
            return disposable;

        }

        public static void DoNothing<T>(T _)
        {
        }

        public static void DoNothing()
        {
        }

        public static IObservable<T> DoError<T>(this IObservable<T> obs, Action<Exception> onError)
        {
            return obs.Do(DoNothing, onError);
        }

        public static IObservable<T> DoComplete<T>(this IObservable<T> obs, Action onComplete)
        {
            return obs.Do(DoNothing, onComplete);
        }

        /// <summary>
        /// Execute 'onSubscribe' when the observer subscribes to this observable.
        /// The opposite to this is .Finally(Action) which is executed when the observable completes/errors
        /// </summary>
        /// <returns>The Observable with on-subscribe action</returns>
        /// <param name="source">Source.</param>
        /// <param name="onSubscribe">On subscribe.</param>
        /// <typeparam name="TSource">The 1st type parameter.</typeparam>
        public static IObservable<TSource> Prepare<TSource>(this IObservable<TSource> source, Action onSubscribe)
        {
            return Observable.Create<TSource>(observer =>
            {
                onSubscribe();
                var subscription = source.Subscribe(observer);
                return Disposable.Create(() =>
                {
                    subscription.Dispose();
                });
            });
        }

        #endregion Actions

        #region Filter

        ///<summary>
        /// Apply conversion operator to observable
        ///</summary>
        public static IObservable<T2> Operator<T1, T2>(this IObservable<T1> obs, Func<IObservable<T1>, IObservable<T2>> operation)
        {
            return operation(obs);
        }

        ///<summary>
        /// Apply conversion operator to observable only if the given condition is true.
        /// This way you can easily active/deactivate operators like .Select() or .Where() in the observer-chain when creating an obseravble
        ///</summary>
        public static IObservable<T> OperatorIf<T>(this IObservable<T> obs, bool condition, Func<IObservable<T>, IObservable<T>> then, Func<IObservable<T>, IObservable<T>> otherwise = null)
        {
            if (condition)
            {
                return then(obs);
            }
            else if (otherwise != null)
            {
                return otherwise(obs);
            }
            return obs;
        }

        ///<summary>
        /// Waits for at least 2 elements beeing emitted before starts firing.
        /// Example: sequence [A, B, C, D, E] would result in following emits: [(A,B), (B,C), (C,D), (D,E)]
        ///</summary>
        public static IObservable<TU> LatestWithPrevious<T, TU>(this IObservable<T> obs, Func<T, T, TU> combineFunc)
        {
            var latest = obs.Skip(1);
            return latest.CombineLatest(obs, combineFunc);
        }

        ///<summary>
        /// Like Take until but get the last value also
        ///</summary>
        public static IObservable<TSource> TakeUntilInclusive<TSource>(this IObservable<TSource> source, Func<TSource, bool> predicate)
        {
            return Observable.Create<TSource>(observer =>
            {
                var subscription = source.Subscribe(
                    (next) =>
                    {
                        observer.OnNext(next);
                        if (predicate(next))
                            observer.OnCompleted();
                    },
                    observer.OnError,
                    observer.OnCompleted);
                return Disposable.Create(() =>
                {
                    subscription.Dispose();
                });
            });
        }

        public static IObservable<T> RetryWithDelay<T>(this IObservable<T> source, TimeSpan timeSpan)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (timeSpan < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException("timeSpan");
            if (timeSpan == TimeSpan.Zero)
                return source.Retry();

            return source.Catch<T, Exception>(_ => Observable.Timer(timeSpan).SelectMany(__ => source).Retry());
        }

        public static IObservable<Unit> AsUnit<T>(this IObservable<T> obs)
        {
            return obs.Select(_ => Unit.Default);
        }

        public static IObservable<T> WhereNotNull<T>(this IObservable<T> obs) where T : class
        {
            return obs.Where(x => x != null);
        }

        public static IObservable<bool> WhereIsTrue(this IObservable<bool> obs)
        {
            return obs.Where(x => x);
        }

        public static IObservable<bool> WhereIsFalse(this IObservable<bool> obs)
        {
            return obs.Where(x => !x);
        }

        public static IObservable<T2> SelectWhereNotNull<T1, T2>(this IObservable<T1> obs, Func<T1, T2> map) where T2 : class
        {
            return obs.Select(map).Where(x => x != default(T2));
        }

        #endregion Filter

    }
}