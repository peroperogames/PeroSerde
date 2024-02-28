/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/01/19
 * Description: MemoryPackFormatterProvider
 */

using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using PeroTools2.Serde.MemoryPack.Formatters;

namespace PeroTools2.Serde.MemoryPack
{
    public static partial class MemoryPackFormatterProvider
    {
        // For Non-Generic
        private static readonly ConcurrentDictionary<Type, IMemoryPackFormatter> s_Formatters = new(Environment.ProcessorCount, 150);

        // For Custom Generic Formatters
        private static readonly ConcurrentDictionary<Type, Type> s_GenericFormatterFactory = new();

        // For Custom Generic Collection Formatters.
        private static readonly ConcurrentDictionary<Type, Type> s_GenericCollectionFormatterFactory = new();

        /// <summary>
        /// 
        /// </summary>
        static partial void RegisterInitialFormatters();

        static MemoryPackFormatterProvider()
        {
            // Initialize on Startup
            RegisterWellKnownTypeFormatters();
            // Extensions For Unity Or Others
            RegisterInitialFormatters();
        }

        #region Register

        public static bool IsRegistered<T>() => Check<T>.registered;

        public static void Register<T>(MemoryPackFormatter<T> formatter)
        {
            Check<T>.registered = true;
            Cache<T>.formatter = formatter;
            s_Formatters[typeof(T)] = formatter;
        }

        public static void RegisterGenericType(Type genericType, Type genericFormatterType)
        {
            if (genericType.IsGenericType && genericFormatterType.IsGenericType)
            {
                s_GenericCollectionFormatterFactory[genericType] = genericFormatterType;
            }
            else
            {
                MemoryPackSerializationException.ThrowMessage($"Registered type is not generic type. genericType: {genericType.FullName}, formatterType: {genericFormatterType.FullName}");
            }
        }

        public static void RegisterCollection<TCollection, TElement>()
            where TCollection : ICollection<TElement>, new()
        {
            Register(new GenericCollectionFormatter<TCollection, TElement>());
        }

        #endregion

        internal static MemoryPackFormatter<T> GetFormatter<T>()
        {
            throw new NotImplementedException();
        }

        internal static IMemoryPackFormatter GetFormatter(Type type)
        {
            throw new NotImplementedException();
        }

        private static class Check<T>
        {
            public static bool registered;
        }

        private static class Cache<T>
        {
            public static MemoryPackFormatter<T> formatter;

            static Cache()
            {
                if (Check<T>.registered) return;
                try
                {
                    var type = typeof(T);
                    if (TryInvokeRegisterFormatter(type))
                        // 这里对formatter赋值发生在生成的代码调用Register()方法时.
                        return;
                    if (TypeHelper.IsAnonymous(type))
                    {
                        formatter = new ErrorMemoryPackFormatter<T>("Serialize anonymous type is not supported, use record or tuple instead.");
                        goto END;
                    }
                    var isUnmanagedType = !RuntimeHelpers.IsReferenceOrContainsReferences<T>();
                    formatter =
                        CreateGenericFormatter(type, isUnmanagedType) as MemoryPackFormatter<T>
                        ?? new ErrorMemoryPackFormatter<T>();
                }
                catch (Exception e)
                {
                    formatter = new ErrorMemoryPackFormatter<T>(e);
                }

                END:
                s_Formatters[typeof(T)] = formatter;
                Check<T>.registered = true;
            }
        }

        private static bool TryInvokeRegisterFormatter(Type type)
        {
            if (typeof(IMemoryPackFormatterRegister).IsAssignableFrom(type))
            {
                var method = type.GetMethod(
                    "RegisterFormatter",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static
                );
                if (method == null)
                {
                    throw new InvalidOperationException($"Type implements IMemoryPackFormatterRegister but can not found RegisterFormatter. Type: {type.FullName}");
                }
                // 调用生成的RegisterFormatter方法来注册Formatter
                method.Invoke(null, null);
                return true;
            }
            return false;
        }

        private static object CreateGenericFormatter(Type type, bool isUnmanagedType)
        {
            throw new NotSupportedException();
        }

        private static Type TryCreateGenericFormatterType(Type type, IDictionary<Type, Type> knownTypes)
        {
            if (type.IsGenericType)
            {
                var genericDef = type.GetGenericTypeDefinition();
                if (knownTypes.TryGetValue(genericDef, out var formatterType))
                {
                    return formatterType.MakeGenericType(type.GetGenericArguments());
                }
            }
            return null;
        }

        private static void RegisterWellKnownTypeFormatters()
        {
        }
    }

    internal sealed class ErrorMemoryPackFormatter : IMemoryPackFormatter
    {
        private readonly Type m_Type;
        private readonly string m_Message;

        public ErrorMemoryPackFormatter(Type type)
        {
            m_Type = type;
            m_Message = null;
        }

        public ErrorMemoryPackFormatter(Type type, string message)
        {
            m_Type = type;
            m_Message = message;
        }

        public void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref object value) where TBufferWriter : class, IBufferWriter<byte>
        {
            Throw();
        }

        [DoesNotReturn]
        private void Throw()
        {
            if (m_Message != null)
            {
                MemoryPackSerializationException.ThrowMessage(m_Message);
            }
            else
            {
                MemoryPackSerializationException.ThrowNotRegisteredInProvider(m_Type);
            }
        }
    }

    internal sealed class ErrorMemoryPackFormatter<T> : MemoryPackFormatter<T>
    {
        private readonly Exception m_Exception;
        private readonly string m_Message;

        public ErrorMemoryPackFormatter()
        {
            m_Exception = null;
            m_Message = null;
        }

        public ErrorMemoryPackFormatter(Exception ex)
        {
            m_Exception = ex;
            m_Message = null;
        }

        public ErrorMemoryPackFormatter(string message)
        {
            m_Exception = null;
            m_Message = null;
        }

        public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, ref T value)
        {
            Throw();
        }

        [DoesNotReturn]
        private void Throw()
        {
            if (m_Exception != null)
            {
                MemoryPackSerializationException.ThrowRegisterInProviderFailed(typeof(T), m_Exception);
            }
            else if (m_Message != null)
            {
                MemoryPackSerializationException.ThrowMessage(m_Message);
            }
            else
            {
                MemoryPackSerializationException.ThrowNotRegisteredInProvider(typeof(T));
            }
        }
    }
}