#nullable enable

using HotChocolate.Internal;
using HotChocolate.Language;
using HotChocolate.Properties;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors;

namespace HotChocolate;

public static partial class SchemaBuilderExtensions
{
    public static ISchemaBuilder AddQueryType(
        this ISchemaBuilder builder,
        Action<IObjectTypeDescriptor> configure)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        return AddQueryType(builder, new ObjectType(configure));
    }

    public static ISchemaBuilder AddQueryType<T>(
        this ISchemaBuilder builder,
        Action<IObjectTypeDescriptor<T>> configure)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        return AddQueryType(builder, new ObjectType<T>(configure));
    }

    public static ISchemaBuilder AddQueryType(
        this ISchemaBuilder builder,
        Type type)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(type);

        return builder.AddRootType(type, OperationType.Query);
    }

    public static ISchemaBuilder AddQueryType(
        this ISchemaBuilder builder,
        ObjectType queryType)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(queryType);

        return builder.AddRootType(queryType, OperationType.Query);
    }

    public static ISchemaBuilder AddQueryType<TQuery>(
        this ISchemaBuilder builder)
        where TQuery : class
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.AddRootType(typeof(TQuery), OperationType.Query);
    }

    public static ISchemaBuilder AddMutationType(
        this ISchemaBuilder builder,
        Action<IObjectTypeDescriptor> configure)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        return AddMutationType(builder, new ObjectType(configure));
    }

    public static ISchemaBuilder AddMutationType<T>(
        this ISchemaBuilder builder,
        Action<IObjectTypeDescriptor<T>> configure)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        return AddMutationType(builder, new ObjectType<T>(configure));
    }

    public static ISchemaBuilder AddMutationType(
        this ISchemaBuilder builder,
        Type type)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(type);

        return builder.AddRootType(type, OperationType.Mutation);
    }

    public static ISchemaBuilder AddMutationType(
        this ISchemaBuilder builder,
        ObjectType queryType)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(queryType);

        return builder.AddRootType(queryType, OperationType.Mutation);
    }

    public static ISchemaBuilder AddMutationType<TMutation>(
        this ISchemaBuilder builder)
        where TMutation : class
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.AddRootType(
            typeof(TMutation),
            OperationType.Mutation);
    }

    public static ISchemaBuilder AddSubscriptionType(
        this ISchemaBuilder builder,
        Action<IObjectTypeDescriptor> configure)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        return AddSubscriptionType(builder, new ObjectType(configure));
    }

    public static ISchemaBuilder AddSubscriptionType<T>(
        this ISchemaBuilder builder,
        Action<IObjectTypeDescriptor<T>> configure)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        return AddSubscriptionType(builder, new ObjectType<T>(configure));
    }

    public static ISchemaBuilder AddSubscriptionType(
        this ISchemaBuilder builder,
        Type type)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(type);

        return builder.AddRootType(type, OperationType.Subscription);
    }

    public static ISchemaBuilder AddSubscriptionType(
        this ISchemaBuilder builder,
        ObjectType queryType)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(queryType);

        return builder.AddRootType(queryType, OperationType.Subscription);
    }

    public static ISchemaBuilder AddSubscriptionType<TSubscription>(
        this ISchemaBuilder builder)
        where TSubscription : class
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.AddRootType(
            typeof(TSubscription),
            OperationType.Subscription);
    }

    public static ISchemaBuilder AddObjectType(
        this ISchemaBuilder builder,
        Action<IObjectTypeDescriptor> configure)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        return builder.AddType(new ObjectType(configure));
    }

    public static ISchemaBuilder AddObjectType<T>(
        this ISchemaBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (typeof(T).IsSchemaType() || typeof(T).IsNonGenericSchemaType())
        {
            throw new InvalidOperationException(
                string.Format(
                    TypeResources.SchemaBuilderExtensions_AddObjectType_TIsSchemaType,
                    typeof(T).FullName));
        }

        return builder.AddType(new ObjectType<T>());
    }

    public static ISchemaBuilder AddObjectType<T>(
        this ISchemaBuilder builder,
        Action<IObjectTypeDescriptor<T>> configure)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        if (typeof(T).IsSchemaType() || typeof(T).IsNonGenericSchemaType())
        {
            throw new InvalidOperationException(
                string.Format(
                    TypeResources.SchemaBuilderExtensions_AddObjectType_TIsSchemaType,
                    typeof(T).FullName));
        }

        return builder.AddType(new ObjectType<T>(configure));
    }

    public static ISchemaBuilder AddUnionType(
       this ISchemaBuilder builder,
       Action<IUnionTypeDescriptor> configure)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        return builder.AddType(new UnionType(configure));
    }

    public static ISchemaBuilder AddUnionType<T>(
        this ISchemaBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (typeof(T).IsSchemaType() || typeof(T).IsNonGenericSchemaType())
        {
            throw new InvalidOperationException(
                string.Format(
                    TypeResources.SchemaBuilderExtensions_AddUnionType_TIsSchemaType,
                    typeof(T).FullName));
        }

        return builder.AddType(new UnionType<T>());
    }

    public static ISchemaBuilder AddUnionType<T>(
        this ISchemaBuilder builder,
        Action<IUnionTypeDescriptor> configure)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        if (typeof(T).IsSchemaType() || typeof(T).IsNonGenericSchemaType())
        {
            throw new InvalidOperationException(
                string.Format(
                    TypeResources.SchemaBuilderExtensions_AddUnionType_TIsSchemaType,
                    typeof(T).FullName));
        }

        return builder.AddType(new UnionType<T>(configure));
    }

    public static ISchemaBuilder AddEnumType(
       this ISchemaBuilder builder,
       Action<IEnumTypeDescriptor> configure)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        return builder.AddType(new EnumType(configure));
    }

    public static ISchemaBuilder AddEnumType<T>(
        this ISchemaBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (typeof(T).IsSchemaType() || typeof(T).IsNonGenericSchemaType())
        {
            throw new InvalidOperationException(
                string.Format(
                    TypeResources.SchemaBuilderExtensions_AddEnumType_TIsSchemaType,
                    typeof(T).FullName));
        }

        return builder.AddType(new EnumType<T>());
    }

    public static ISchemaBuilder AddEnumType<T>(
        this ISchemaBuilder builder,
        Action<IEnumTypeDescriptor<T>> configure)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        if (typeof(T).IsSchemaType() || typeof(T).IsNonGenericSchemaType())
        {
            throw new InvalidOperationException(
                string.Format(
                    TypeResources.SchemaBuilderExtensions_AddEnumType_TIsSchemaType,
                    typeof(T).FullName));
        }

        return builder.AddType(new EnumType<T>(configure));
    }

    public static ISchemaBuilder AddInterfaceType(
       this ISchemaBuilder builder,
       Action<IInterfaceTypeDescriptor> configure)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        return builder.AddType(new InterfaceType(configure));
    }

    public static ISchemaBuilder AddInterfaceType<T>(
        this ISchemaBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (typeof(T).IsSchemaType() || typeof(T).IsNonGenericSchemaType())
        {
            throw new InvalidOperationException(
                string.Format(
                    TypeResources.SchemaBuilderExtensions_AddInterfaceType_TIsSchemaType,
                    typeof(T).FullName));
        }

        return builder.AddType(new InterfaceType<T>());
    }

    public static ISchemaBuilder AddInterfaceType<T>(
        this ISchemaBuilder builder,
        Action<IInterfaceTypeDescriptor<T>> configure)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        if (typeof(T).IsSchemaType() || typeof(T).IsNonGenericSchemaType())
        {
            throw new InvalidOperationException(
                string.Format(
                    TypeResources.SchemaBuilderExtensions_AddInterfaceType_TIsSchemaType,
                    typeof(T).FullName));
        }

        return builder.AddType(new InterfaceType<T>(configure));
    }

    public static ISchemaBuilder AddInputObjectType(
       this ISchemaBuilder builder,
       Action<IInputObjectTypeDescriptor> configure)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        return builder.AddType(new InputObjectType(configure));
    }

    public static ISchemaBuilder AddInputObjectType<T>(
        this ISchemaBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (typeof(T).IsSchemaType() || typeof(T).IsNonGenericSchemaType())
        {
            throw new InvalidOperationException(
                string.Format(
                    TypeResources.SchemaBuilderExtensions_AddInputObjectType_TIsSchemaType,
                    typeof(T).FullName));
        }

        return builder.AddType(new InputObjectType<T>());
    }

    public static ISchemaBuilder AddInputObjectType<T>(
        this ISchemaBuilder builder,
        Action<IInputObjectTypeDescriptor<T>> configure)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        if (typeof(T).IsSchemaType() || typeof(T).IsNonGenericSchemaType())
        {
            throw new InvalidOperationException(
                string.Format(
                    TypeResources.SchemaBuilderExtensions_AddInputObjectType_TIsSchemaType,
                    typeof(T).FullName));
        }

        return builder.AddType(new InputObjectType<T>(configure));
    }

    public static ISchemaBuilder AddType<T>(
        this ISchemaBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.AddType(typeof(T));
    }

    public static ISchemaBuilder AddTypes(
        this ISchemaBuilder builder,
        params INamedType[] types)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(types);

        foreach (var type in types)
        {
            builder.AddType(type);
        }
        return builder;
    }

    public static ISchemaBuilder AddTypes(
        this ISchemaBuilder builder,
        params Type[] types)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(types);

        foreach (var type in types)
        {
            builder.AddType(type);
        }
        return builder;
    }

    public static ISchemaBuilder AddDirectiveType(
        this ISchemaBuilder builder,
        Type directiveType)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(directiveType);

        bool IsDirectiveBaseType()
        {
            if (directiveType == typeof(DirectiveType))
            {
                return true;
            }

            if (directiveType.IsGenericType)
            {
                var genericType = directiveType.GetGenericTypeDefinition();
                return genericType == typeof(DirectiveType<>);
            }

            return false;
        }

        if (IsDirectiveBaseType())
        {
            throw new ArgumentException(
                TypeResources.SchemaBuilderExtensions_DirectiveTypeIsBaseType,
                nameof(directiveType));
        }

        if (!typeof(DirectiveType).IsAssignableFrom(directiveType))
        {
            throw new ArgumentException(
                TypeResources.SchemaBuilderExtensions_MustBeDirectiveType,
                nameof(directiveType));
        }

        return builder.AddType(directiveType);
    }

    public static ISchemaBuilder AddDirectiveType<TDirective>(
        this ISchemaBuilder builder)
        where TDirective : DirectiveType
    {
        ArgumentNullException.ThrowIfNull(builder);

        return AddDirectiveType(builder, typeof(TDirective));
    }

    public static ISchemaBuilder SetSchema<TSchema>(
        this ISchemaBuilder builder)
        where TSchema : ISchema
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.SetSchema(typeof(TSchema));
    }

    public static ISchemaBuilder BindRuntimeType<TRuntimeType, TSchemaType>(
        this ISchemaBuilder builder)
        where TSchemaType : INamedType
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.BindRuntimeType(typeof(TRuntimeType), typeof(TSchemaType));
    }

    public static ISchemaBuilder BindRuntimeType<TRuntimeType>(
        this ISchemaBuilder builder,
        string? typeName = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        typeName ??= typeof(TRuntimeType).Name;
        return BindRuntimeTypeInternal(builder, typeName, typeof(TRuntimeType));
    }

    public static ISchemaBuilder BindRuntimeType(
        this ISchemaBuilder builder,
        Type runtimeType,
        string? typeName = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(runtimeType);

        typeName ??= runtimeType.Name;
        return BindRuntimeTypeInternal(builder, typeName, runtimeType);
    }

    public static void TryBindRuntimeType(
        this IDescriptorContext context,
        string typeName,
        Type runtimeType)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(runtimeType);

        if (string.IsNullOrEmpty(typeName))
        {
            throw new ArgumentNullException(nameof(typeName));
        }

        if (context.ContextData.TryGetValue(WellKnownContextData.RuntimeTypes, out var o) &&
            o is Dictionary<string, Type> runtimeTypes)
        {
            runtimeTypes[typeName] = runtimeType;
        }
    }

    private static ISchemaBuilder BindRuntimeTypeInternal(
        ISchemaBuilder builder,
        string typeName,
        Type runtimeType)
    {
        InitializeResolverTypeInterceptor(builder);

        if (builder.ContextData.TryGetValue(WellKnownContextData.RuntimeTypes, out var o) &&
            o is Dictionary<string, Type> runtimeTypes)
        {
            runtimeTypes[typeName] = runtimeType;
        }

        return builder;
    }
}
