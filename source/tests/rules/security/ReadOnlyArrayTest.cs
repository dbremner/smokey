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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Smokey.Framework.Support;
using Smokey.Internal.Rules;

namespace Smokey.Tests
{
	[TestFixture]
	public class ReadOnlyArrayTest : TypeTest
	{	
		#region Test classes
		public class Good1
		{
			public readonly float BaseCost = 100.0f;
			
			public float Compute(float bandwidth, float bytes, float latency)
			{
				float cost = BaseCost;
				
				cost += bandwidth * m_scaling[0];
				cost += bytes * m_scaling[1];
				cost += latency * m_scaling[2];
				
				return cost;
			}

			public float[] Scaling()
			{
				return (float[]) m_scaling.Clone();
			}

			private readonly float[] m_scaling = {1.0f, 1.0f, 4.0f};
		}

		internal class Good2
		{
			public readonly float BaseCost = 100.0f;
			public readonly float[] Scaling = {1.0f, 1.0f, 4.0f};
			
			public float Compute(float bandwidth, float bytes, float latency)
			{
				float cost = BaseCost;
				
				cost += bandwidth * Scaling[0];
				cost += bytes * Scaling[1];
				cost += latency * Scaling[2];
				
				return cost;
			}
		}

		public class Bad1
		{
			public readonly float BaseCost = 100.0f;
			public readonly float[] Scaling = {1.0f, 1.0f, 4.0f};
			
			public float Compute(float bandwidth, float bytes, float latency)
			{
				float cost = BaseCost;
				
				cost += bandwidth * Scaling[0];
				cost += bytes * Scaling[1];
				cost += latency * Scaling[2];
				
				return cost;
			}
		}
		#endregion
		
		// test code
		public ReadOnlyArrayTest() : base(
			new string[]{"Good1", "Good2"},
			new string[]{"Bad1"})	
		{
		}
						
		protected override Rule OnCreate(AssemblyCache cache, IReportViolations reporter)
		{
			return new ReadOnlyArrayRule(cache, reporter);
		}
	} 
}

