// Copyright (C) 2007 Jesse Jones
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Mono.Cecil;
using NUnit.Framework;
using Smokey.Framework.Support;
using Smokey.Internal.Rules;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;

namespace Smokey.Tests
{
	[TestFixture]
	public class SecureOverridesTest : TypeTest
	{	
		#region Test Classes
		[PermissionSet(System.Security.Permissions.SecurityAction.LinkDemand, Name = "FullTrust")]
		public class Good1
		{
			public void A()
			{
			}
			
			[PermissionSet(System.Security.Permissions.SecurityAction.InheritanceDemand, Name = "FullTrust")]
			public virtual void B()
			{
			}
		}

		public class Base
		{			
			public virtual void B()
			{
			}
		}

		[PermissionSet(System.Security.Permissions.SecurityAction.LinkDemand, Name = "FullTrust")]
		public sealed class Good2 : Base
		{			
			public override void B()
			{
			}
		}

		[PermissionSet(System.Security.Permissions.SecurityAction.LinkDemand, Name = "FullTrust")]
		internal class Good3
		{			
			public virtual void B()
			{
			}
		}

		[PermissionSet(System.Security.Permissions.SecurityAction.LinkDemand, Name = "FullTrust")]
		[PermissionSet(System.Security.Permissions.SecurityAction.InheritanceDemand, Name = "FullTrust")]
		public class Good4
		{			
			public virtual void B()
			{
			}
		}

		[PermissionSet(System.Security.Permissions.SecurityAction.LinkDemand, Name = "FullTrust")]
		public class Bad1
		{			
			public virtual void B()
			{
			}
		}
		#endregion
		
		// test code
		public SecureOverridesTest() : base(
			new string[]{"Good1", "Good2", "Good3", "Good4"},
			new string[]{"Bad1"})	
		{
		}
						
		protected override Rule OnCreate(AssemblyCache cache, IReportViolations reporter)
		{
			return new SecureOverridesRule(cache, reporter);
		}
	} 
}

