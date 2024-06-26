// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection.Metadata;

namespace Internal.TypeSystem
{
    public interface IModuleResolver
    {
        ModuleDesc ResolveAssembly(AssemblyNameInfo name, bool throwIfNotFound = true);
        ModuleDesc ResolveModule(IAssemblyDesc referencingModule, string fileName, bool throwIfNotFound = true);
    }
}
