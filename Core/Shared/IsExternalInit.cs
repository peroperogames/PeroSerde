/*
 * Copyright (c) PeroPeroGames Co., Ltd.
 * Description: Polyfill for the C# 9 init-only setter feature on Unity's older runtime.
 */

using System.ComponentModel;

namespace System.Runtime.CompilerServices;

/// <summary>
/// Makes the C# 9 init only setter feature work correctly in Unity
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
internal sealed class IsExternalInit;