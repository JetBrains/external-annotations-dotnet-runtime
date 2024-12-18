// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace System.Reflection
{
    public class MethodBody
    {
        protected MethodBody() { }
        public virtual int LocalSignatureMetadataToken => 0;
        public virtual IList<LocalVariableInfo> LocalVariables => throw new ArgumentNullException("array");
        public virtual int MaxStackSize => 0;
        public virtual bool InitLocals => false;

        [return: CollectionAccess(CollectionAccessType.UpdatedContent | CollectionAccessType.Read)] //note: changes could be observed
        public virtual byte[]? GetILAsByteArray() => null;
        public virtual IList<ExceptionHandlingClause> ExceptionHandlingClauses => throw new ArgumentNullException("array");
    }
}
