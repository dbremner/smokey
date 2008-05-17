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
using System;
using Smokey.Framework;
using Smokey.Framework.Instructions;
using Smokey.Framework.Support;
using Smokey.Framework.Support.Advanced;

namespace Smokey.Internal.Rules
{	
	internal class PreferMonitor2Rule : Rule
	{				
		public PreferMonitor2Rule(AssemblyCache cache, IReportViolations reporter) 
			: base(cache, reporter, "D1056")
		{
		}
				
		public override void Register(RuleDispatcher dispatcher) 
		{
			dispatcher.Register(this, "VisitBegin");
			dispatcher.Register(this, "VisitNew");
			dispatcher.Register(this, "VisitFini");
		}
						
		public void VisitBegin(BeginMethod begin)
		{
			Log.DebugLine(this, "-----------------------------------"); 
			Log.DebugLine(this, "{0:F}", begin.Info.Instructions);				

			m_offset = -1;
		}
		
		public void VisitNew(NewObj newer)
		{				
			if (m_offset < 0)
			{
				if (newer.Ctor.ToString().StartsWith("System.Void System.Threading.Mutex::.ctor(") || newer.Ctor.ToString().StartsWith("System.Void System.Threading.Semaphore::.ctor("))
				{
					for (int i = 0; i < newer.Ctor.Parameters.Count && m_offset < 0; ++i)
					{
						if (newer.Ctor.Parameters[i].ParameterType.FullName == "System.String")
						{
							m_offset = newer.Untyped.Offset;
							Log.DebugLine(this, "found bad call at {0:X2}", m_offset);
						}
					}
				}
			}
		}
				
		public void VisitFini(EndMethod end)
		{
			if (m_offset >= 0)
			{
				Reporter.MethodFailed(end.Info.Method, CheckID, m_offset, string.Empty);
			}
		}

		private int m_offset;
	}
}

