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
using System.Collections.Generic;
using Smokey.Framework;
using Smokey.Framework.Instructions;
using Smokey.Framework.Support;

namespace Smokey.Internal.Rules
{	
	internal sealed class ISerializableMethodsRule : Rule
	{				
		public ISerializableMethodsRule(AssemblyCache cache, IReportViolations reporter) 
			: base(cache, reporter, "R1041")
		{
		}
		
		public override void Register(RuleDispatcher dispatcher) 
		{
			dispatcher.Register(this, "VisitBegin");
			dispatcher.Register(this, "VisitMethodBegin");
			dispatcher.Register(this, "VisitEnd");
		}

		public void VisitBegin(BeginMethods begin)
		{
			m_notSerializable = !begin.Type.TypeOrBaseImplements("System.Runtime.Serialization.ISerializable", Cache);
			m_hasMethod = false;
			
			if (m_notSerializable)
			{
				Log.DebugLine(this, "-----------------------------------"); 
				Log.DebugLine(this, "{0}", begin.Type);				
			}
		}

		public void VisitMethodBegin(BeginMethod begin)
		{			
			if (m_notSerializable)
			{
				MethodDefinition method = begin.Info.Method;
				
				if (method.IsConstructor && method.Parameters.Count == 2)
				{
					if (method.Parameters[0].ParameterType.FullName == "System.Runtime.Serialization.SerializationInfo")
					{
						if (method.Parameters[1].ParameterType.FullName == "System.Runtime.Serialization.StreamingContext")
						{
							Log.DebugLine(this, "has ctor");	
							m_hasMethod = true;
						}
					}
				}
				else if (method.Matches("System.Void", "GetObjectData", "System.Runtime.Serialization.SerializationInfo", "System.Runtime.Serialization.StreamingContext"))
				{
					Log.DebugLine(this, "has GetObjectData");	
					m_hasMethod = true;
				}
			}		
		}
		
		public void VisitEnd(EndMethods end)
		{
			if (m_notSerializable && m_hasMethod)
			{
				Reporter.TypeFailed(end.Type, CheckID, string.Empty);
			}
		}
		
		private bool m_notSerializable;
		private bool m_hasMethod;
	}
}

