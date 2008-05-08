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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Smokey.Framework.Support;
using Smokey.Internal.Rules;

namespace Smokey.Tests
{
	[TestFixture]
	public class TooManyArgsTest : MethodTest
	{	
		public class Cases
		{	
			public void Good1()
			{
			}
			
			public void Good2(int x, int y)
			{
			}
			
			[System.Runtime.InteropServices.DllImport("Kernel32")]
			public static extern void Good3(int a1, int a2, int a3, int a4, int a5, int a6);
			
			public void Bad1(int a1, int a2, int a3, int a4, int a5, int a6)
			{
			}
		}			

		public class BadBase
		{	
			public virtual void Compute(int a1, int a2, int a3, int a4, int a5, int a6)
			{
			}
		}			

		public class GoodDerived : BadBase
		{	
			public override void Compute(int a1, int a2, int a3, int a4, int a5, int a6)
			{
			}
		}			

		// test code
		public TooManyArgsTest() : base(
			new string[]{"Cases.Good1", "Cases.Good2", "Cases.Good3", "GoodDerived.Compute"},
			new string[]{"Cases.Bad1", "BadBase.Compute"})	
		{
		}
						
		protected override Rule OnCreate(AssemblyCache cache, IReportViolations reporter)
		{
			return new TooManyArgsRule(cache, reporter);
		}
	} 
}