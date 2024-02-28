/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Author: AidenMX
 * Created On: 2024/01/18
 * Description: MemoryPackSerializationException
 */

using System;
using System.Diagnostics.CodeAnalysis;

namespace PeroTools2.Serde.MemoryPack
{
    public class MemoryPackSerializationException : SerdeException
    {
        public MemoryPackSerializationException(string message) : base(message)
        {
        }

        public MemoryPackSerializationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        [DoesNotReturn]
        public static void ThrowMessage(string message)
        {
            throw new MemoryPackSerializationException(message);
        }

        [DoesNotReturn]
        public static void ThrowWriteInvalidMemberCount(byte memberCount)
        {
            throw new MemoryPackSerializationException(
                $"MemberCount/Tag allows < 250 but try to write {memberCount}."
            );
        }

        [DoesNotReturn]
        public static void ThrowReachedDepthLimit(Type type)
        {
            throw new MemoryPackSerializationException($"Serializing Type '{type}' reached depth limit, maybe detect circular reference.");
        }

        [DoesNotReturn]
        public static void ThrowRegisterInProviderFailed(Type type, Exception innerException)
        {
            throw new MemoryPackSerializationException($"{type.FullName} is failed in provider at creating formatter.", innerException);
        }

        [DoesNotReturn]
        public static void ThrowNotRegisteredInProvider(Type type)
        {
            throw new MemoryPackSerializationException($"{type.FullName} is not registered in this provider.");
        }
    }
}