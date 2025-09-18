using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Linq.Expressions;

public static class PropertyCopier<T> where T : class
{
    private static readonly Action<T, T> _copyAction;

    static PropertyCopier()
    {
        var sourceParam = Expression.Parameter(typeof(T), "source");
        var targetParam = Expression.Parameter(typeof(T), "target");

        var assignments = new List<Expression>();

        foreach (var property in typeof(T).GetProperties()
            .Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0))
        {
            var sourceProperty = Expression.Property(sourceParam, property);
            var targetProperty = Expression.Property(targetParam, property);
            var assignment = Expression.Assign(targetProperty, sourceProperty);
            assignments.Add(assignment);
        }

        var block = Expression.Block(assignments);
        _copyAction = Expression.Lambda<Action<T, T>>(block, sourceParam, targetParam).Compile();
    }

    public static void Copy(T source, T target) => _copyAction(source, target);
}

public static class ObjectExtensions
{
    public static void CopyPropertiesFrom<T>(this T target, T source,
        BindingFlags flags = BindingFlags.Public | BindingFlags.Instance)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (target == null) throw new ArgumentNullException(nameof(target));

        var type = typeof(T);
        var properties = type.GetProperties(flags);

        foreach (var property in properties)
        {
            try
            {
                // Skip indexers and properties that can't be read/written
                if (property.GetIndexParameters().Length > 0)
                    continue;

                if (property.CanRead && property.CanWrite)
                {
                    var value = property.GetValue(source);
                    property.SetValue(target, value);
                }
            }
            catch (Exception ex)
            {
                // Log or handle errors as needed
                System.Diagnostics.Debug.WriteLine($"Error copying property {property.Name}: {ex.Message}");
            }
        }
    }

    // Optional: Include fields as well
    public static void CopyFieldsAndPropertiesFrom<T>(this T target, T source)
    {
        if (source == null || target == null) return;

        var type = typeof(T);

        // Copy properties
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var property in properties)
        {
            if (property.CanRead && property.CanWrite && property.GetIndexParameters().Length == 0)
            {
                var value = property.GetValue(source);
                property.SetValue(target, value);
            }
        }

        // Copy fields
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (var field in fields)
        {
            var value = field.GetValue(source);
            field.SetValue(target, value);
        }
    }
}

public static class FastObjectCopier
{
    private static readonly ConcurrentDictionary<Type, Action<object, object>> _copyActions =
        new ConcurrentDictionary<Type, Action<object, object>>();

    public static void CopyProperties(object target, object source)
    {
        if (source == null || target == null) return;

        var type = target.GetType();
        if (source.GetType() != type)
            throw new ArgumentException("Source and target must be same type");

        var copyAction = _copyActions.GetOrAdd(type, CreateCopyAction);
        copyAction(target, source);
    }

    private static Action<object, object> CreateCopyAction(Type type)
    {
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        return (target, source) =>
        {
            foreach (var property in properties)
            {
                if (property.CanRead && property.CanWrite && property.GetIndexParameters().Length == 0)
                {
                    var value = property.GetValue(source);
                    property.SetValue(target, value);
                }
            }
        };
    }
}