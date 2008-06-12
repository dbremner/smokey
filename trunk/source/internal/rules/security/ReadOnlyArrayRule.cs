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
using System.Security;
using System.Collections.Generic;
using Smokey.Framework;
using Smokey.Framework.Instructions;
using Smokey.Framework.Support;
using Smokey.Framework.Support.Advanced;

namespace Smokey.Internal.Rules
{		
	internal sealed class ReadOnlyArrayRule : Rule
	{				
		public ReadOnlyArrayRule(AssemblyCache cache, IReportViolations reporter) 
			: base(cache, reporter, "S1002")
		{
		}
				
		public override void Register(RuleDispatcher dispatcher) 
		{
			dispatcher.Register(this, "VisitBegin");
			dispatcher.Register(this, "VisitField");
			dispatcher.Register(this, "VisitEnd");
		}
		
		public void VisitBegin(BeginType begin)
		{
			Log.DebugLine(this, "-----------------------------------"); 
			Log.DebugLine(this, "checking {0}", begin.Type);

			m_names = string.Empty;
			m_needsCheck = begin.Type.IsPublic || begin.Type.IsNestedPublic;
		}
		
		public void VisitField(FieldDefinition field)
		{
			if (m_needsCheck && field.IsInitOnly)
			{
				FieldAttributes attrs = field.Attributes & FieldAttributes.FieldAccessMask;
				if (attrs == FieldAttributes.Public || attrs == FieldAttributes.Family)
				{
					ArrayType array = field.FieldType as ArrayType;
					if (array != null)
						m_names += field.Name + " ";
				}
			}
		}
		
		public void VisitEnd(EndType end)
		{
			if (m_names.Length > 0)
			{
				string details = "Fields: " + m_names;
				Log.DebugLine(this, details);
			
				Reporter.TypeFailed(end.Type, CheckID, details);
			}
		}
				
		private string m_names;
		private bool m_needsCheck;
	}
}
