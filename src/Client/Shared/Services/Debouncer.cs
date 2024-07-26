namespace Client.Shared.Services;

/// <summary>
/// Дебаунсер.
/// </summary>
public static class Debouncer
{
    /// <summary>
    /// Откладывает выполнение функции, если пользователь совершает какое-либо действие.
    /// </summary>
    /// <param name="action">Функция.</param>
    /// <param name="interval">Время неактивности, после которого выполнится функция.</param>
    /// <exception cref="ArgumentNullException">Функция является null.</exception>
    public static Action<T> Debounce<T>(Action<T> action, TimeSpan? interval = null)
    {
        ArgumentNullException.ThrowIfNull(action, nameof(action));
        interval ??= TimeSpan.FromMilliseconds(500);

        var last = 0;
        return arg =>
        {
            var current = Interlocked.Increment(ref last);
            Task.Delay(interval.Value).ContinueWith(task =>
            {
                if (current == last)
                {
                    action(arg);
                }
            });
        };
    }
    
    /// <summary>
    /// Откладывает выполнение функции, если пользователь совершает какое-либо действие.
    /// </summary>
    /// <param name="action">Функция.</param>
    /// <param name="callback">Функция обратного вызова.</param>
    /// <param name="interval">Время неактивности, после которого выполнится функция.</param>
    /// <typeparam name="T">Тип аргумента.</typeparam>
    /// <returns>Функция.</returns>
    public static Action<T> DebounceEvent<T>(Action<T> action, Action callback, TimeSpan? interval = null)
    {
        interval ??= TimeSpan.FromMilliseconds(500);
        return Debounce<T>(arg =>
        {
            action(arg);
            callback();
        }, interval);
    }
}