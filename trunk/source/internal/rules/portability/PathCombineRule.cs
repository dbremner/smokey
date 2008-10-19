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
using Mono.Cecil.Cil;
using System;
using System.IO;
using System.Collections.Generic;
using Smokey.Framework;
using Smokey.Framework.Instructions;
using Smokey.Framework.Support;

#if OLD
namespace Smokey.Internal.Rules
{	
	internal sealed class PathCombineRule : Rule
	{				
		public PathCombineRule(AssemblyCache cache, IReportViolations reporter) 
			: base(cache, reporter, "PO1006")
		{
		}
				
		public override void Register(RuleDispatcher dispatcher) 
		{
			dispatcher.Register(this, "VisitBegin");
			dispatcher.Register(this, "VisitCall");
			dispatcher.Register(this, "VisitEnd");
		}
				
		public void VisitBegin(BeginMethod begin)
		{
			Log.DebugLine(this, "-----------------------------------"); 
			Log.DebugLine(this, "{0:F}", begin.Info.Instructions);				

			m_offset = -1;
			m_info = begin.Info;
		}
		
		public void VisitCall(Call call)
		{
			if (m_offset < 0)
			{
				if (call.Target.ToString().StartsWith("System.String System.String::Concat("))
				{					
					for (int nth = 0; nth < call.Target.Parameters.Count && m_offset < 0; ++nth)
					{
						int index = m_info.Tracker.GetStackIndex(call.Index, nth);
						
						if (index >= 0)
						{
							LoadString load = m_info.Instructions[index] as LoadString;

							if (load != null && DoBadString(load.Value))
							{
								m_offset = call.Untyped.Offset;						
								Log.DebugLine(this, "bad concat at at {0:X2}", m_offset); 
							}
						}
					}
				}
			}
		}

		public void VisitEnd(EndMethod end)
		{
			if (m_offset >= 0)
			{
				Reporter.MethodFailed(end.Info.Method, CheckID, m_offset, string.Empty);
			}
		}
		
		private static bool DoBadString(string text)
		{
			bool bad = false;
			
			if (text.Contains("\\"))
			{
				bad = true; 
			}
			else
			{
				int i = text.IndexOf('/');
				if (i == 0)
					bad = true;
				else if (i > 0)
					bad = text[i - 1] != '<';	// </ is likely an xml or html tag
					
				if (bad && i + 1 < text.Length)	// special case for stuff like file://
					if (text[i + 1] == '/')
						bad = false;
			}

			return bad;
		}
						
		private int m_offset;
		private MethodInfo m_info;
	}
}
#endif
