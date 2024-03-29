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
using System.Diagnostics;
using Smokey.Framework;
using Smokey.Framework.Instructions;
using Smokey.Framework.Support;

namespace Smokey.Internal.Rules
{	
	internal sealed class ArrayIndexOfRule : Rule
	{				
		public ArrayIndexOfRule(AssemblyCache cache, IReportViolations reporter) 
			: base(cache, reporter, "C1013")
		{
		}
				
		public override void Register(RuleDispatcher dispatcher) 
		{
			dispatcher.Register(this, "VisitBegin");
			dispatcher.Register(this, "VisitBranch");
			dispatcher.Register(this, "VisitCompare");
			dispatcher.Register(this, "VisitEnd");
		}
		
		public void VisitBegin(BeginMethod begin)
		{
			Log.DebugLine(this, "-----------------------------------"); 
			Log.DebugLine(this, "{0:F}", begin.Info.Instructions);				

			m_offset = -1;
			m_info = begin.Info;
		}
		
		public void VisitBranch(ConditionalBranch branch)
		{
			if (m_offset < 0)
			{
				if (DoMatch1(branch.Index))
				{
					m_offset = branch.Untyped.Offset;						
					Log.DebugLine(this, "DoMatch1 matched at {0:X2}", m_offset);				
				}
				else if (DoMatch1b(branch.Index))
				{
					m_offset = branch.Untyped.Offset;						
					Log.DebugLine(this, "DoMatch1b matched at {0:X2}", m_offset);				
				}
				else if (DoMatch3(branch.Index))
				{
					m_offset = branch.Untyped.Offset;						
					Log.DebugLine(this, "DoMatch3 matched at {0:X2}", m_offset);				
				}
			}
		}

		public void VisitCompare(Compare compare)
		{
			if (m_offset < 0)
			{
				if (DoMatch2(compare.Index))
				{
					m_offset = compare.Untyped.Offset;						
					Log.DebugLine(this, "DoMatch2 matched at {0:X2}", m_offset);				
				}
				else if (DoMatch2b(compare.Index))
				{
					m_offset = compare.Untyped.Offset;						
					Log.DebugLine(this, "DoMatch2b matched at {0:X2}", m_offset);				
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
						
		// 02: call       System.Int32 System.Array::IndexOf<*>(!!0[],!!0)
		// 07: stloc.N    V_0
		// 08: ldloc.N    V_0
		// 09: ldc.i4.0
		// 0A: ble        1E
		[DisableRule("D1049", "IdenticalCodeBlocks")]	// these are machine generated so we won't worry too much if some of the code is duplicated
		private bool DoMatch1(int index)
		{
			bool match = false;

			do
			{
				if (index - 4 < 0)
					break;

				TypedInstruction instruction = m_info.Instructions[index - 0];
				Code code = instruction.Untyped.OpCode.Code;
				if (code != Code.Ble_S && code != Code.Ble_Un_S && code != Code.Ble && code != Code.Ble_Un)
					break;

				instruction = m_info.Instructions[index - 1];
				code = instruction.Untyped.OpCode.Code;
				if (code != Code.Ldc_I4_0)
					break;

				instruction = m_info.Instructions[index - 2];
				code = instruction.Untyped.OpCode.Code;
				if (code != Code.Ldloc_S && code != Code.Ldloc && code != Code.Ldloc_0 && code != Code.Ldloc_1 && code != Code.Ldloc_2 && code != Code.Ldloc_3)
					break;
				int varN = ((LoadLocal) instruction).Variable;

				instruction = m_info.Instructions[index - 3];
				code = instruction.Untyped.OpCode.Code;
				if (code != Code.Stloc_S && code != Code.Stloc && code != Code.Stloc_0 && code != Code.Stloc_1 && code != Code.Stloc_2 && code != Code.Stloc_3)
					break;
				if (varN != ((StoreLocal) instruction).Variable)
					break;

				instruction = m_info.Instructions[index - 4];
				code = instruction.Untyped.OpCode.Code;
				if (code != Code.Call && code != Code.Callvirt)
					break;
				if (!((Call) instruction).Target.ToString().StartsWith("System.Int32 System.Array::IndexOf<") || !((Call) instruction).Target.ToString().EndsWith(">(!!0[],!!0)"))
					break;

				match = true;
			}
			while (false);

			return match;
		}

		// 02: call       System.Int32 System.Array::IndexOf<*>(!!0[],!!0)
		// 09: ldc.i4.0
		// 0A: ble        1E
		[DisableRule("D1049", "IdenticalCodeBlocks")]	// these are machine generated so we won't worry too much if some of the code is duplicated
		private bool DoMatch1b(int index)
		{
			bool match = false;

			do
			{
				if (index - 2 < 0)
					break;

				TypedInstruction instruction = m_info.Instructions[index - 0];
				Code code = instruction.Untyped.OpCode.Code;
				if (code != Code.Ble_S && code != Code.Ble_Un_S && code != Code.Ble && code != Code.Ble_Un)
					break;

				instruction = m_info.Instructions[index - 1];
				code = instruction.Untyped.OpCode.Code;
				if (code != Code.Ldc_I4_0)
					break;

				instruction = m_info.Instructions[index - 2];
				code = instruction.Untyped.OpCode.Code;
				if (code != Code.Call && code != Code.Callvirt)
					break;
				if (!((Call) instruction).Target.ToString().StartsWith("System.Int32 System.Array::IndexOf<") || !((Call) instruction).Target.ToString().EndsWith(">(!!0[],!!0)"))
					break;

				match = true;
			}
			while (false);

			return match;
		}

		// 02: call       System.Int32 System.Array::IndexOf<*>(!!0[],!!0)
		// 07: stloc.N    V_0
		// 08: ldloc.N    V_0
		// 09: ldc.i4.0
		// 0A: cgt
		private bool DoMatch2(int index)
		{
			bool match = false;

			do
			{
				if (index - 4 < 0)
					break;

				TypedInstruction instruction = m_info.Instructions[index - 0];
				Code code = instruction.Untyped.OpCode.Code;
				if (code != Code.Cgt && code != Code.Cgt_Un)
					break;

				instruction = m_info.Instructions[index - 1];
				code = instruction.Untyped.OpCode.Code;
				if (code != Code.Ldc_I4_0)
					break;

				instruction = m_info.Instructions[index - 2];
				code = instruction.Untyped.OpCode.Code;
				if (code != Code.Ldloc_S && code != Code.Ldloc && code != Code.Ldloc_0 && code != Code.Ldloc_1 && code != Code.Ldloc_2 && code != Code.Ldloc_3)
					break;
				int varN = ((LoadLocal) instruction).Variable;

				instruction = m_info.Instructions[index - 3];
				code = instruction.Untyped.OpCode.Code;
				if (code != Code.Stloc_S && code != Code.Stloc && code != Code.Stloc_0 && code != Code.Stloc_1 && code != Code.Stloc_2 && code != Code.Stloc_3)
					break;
				if (varN != ((StoreLocal) instruction).Variable)
					break;

				instruction = m_info.Instructions[index - 4];
				code = instruction.Untyped.OpCode.Code;
				if (code != Code.Call && code != Code.Callvirt)
					break;
				if (!((Call) instruction).Target.ToString().StartsWith("System.Int32 System.Array::IndexOf<") || !((Call) instruction).Target.ToString().EndsWith(">(!!0[],!!0)"))
					break;

				match = true;
			}
			while (false);

			return match;
		}

		// 02: call       System.Int32 System.Array::IndexOf<*>(!!0[],!!0)
		// 09: ldc.i4.0
		// 0A: cgt
		private bool DoMatch2b(int index)
		{
			bool match = false;

			do
			{
				if (index - 2 < 0)
					break;

				TypedInstruction instruction = m_info.Instructions[index - 0];
				Code code = instruction.Untyped.OpCode.Code;
				if (code != Code.Cgt && code != Code.Cgt_Un)
					break;

				instruction = m_info.Instructions[index - 1];
				code = instruction.Untyped.OpCode.Code;
				if (code != Code.Ldc_I4_0)
					break;

				instruction = m_info.Instructions[index - 2];
				code = instruction.Untyped.OpCode.Code;
				if (code != Code.Call && code != Code.Callvirt)
					break;
				if (!((Call) instruction).Target.ToString().StartsWith("System.Int32 System.Array::IndexOf<") || !((Call) instruction).Target.ToString().EndsWith(">(!!0[],!!0)"))
					break;

				match = true;
			}
			while (false);

			return match;
		}

		// 02: call       System.Int32 System.Array::IndexOf<*>(!!0[],!!0)
		// 07: stloc.N    V_0
		// 08: ldc.i4.0
		// 09: ldloc.N    V_0
		// 0A: bge        1E
		private bool DoMatch3(int index)
		{
			bool match = false;

			do
			{
				if (index - 4 < 0)
					break;

				TypedInstruction instruction = m_info.Instructions[index - 0];
				Code code = instruction.Untyped.OpCode.Code;
				if (code != Code.Bge_S && code != Code.Bge_Un_S && code != Code.Bge && code != Code.Bge_Un)
					break;

				instruction = m_info.Instructions[index - 1];
				code = instruction.Untyped.OpCode.Code;
				if (code != Code.Ldloc_S && code != Code.Ldloc && code != Code.Ldloc_0 && code != Code.Ldloc_1 && code != Code.Ldloc_2 && code != Code.Ldloc_3)
					break;
				int varN = ((LoadLocal) instruction).Variable;

				instruction = m_info.Instructions[index - 2];
				code = instruction.Untyped.OpCode.Code;
				if (code != Code.Ldc_I4_0)
					break;

				instruction = m_info.Instructions[index - 3];
				code = instruction.Untyped.OpCode.Code;
				if (code != Code.Stloc_S && code != Code.Stloc && code != Code.Stloc_0 && code != Code.Stloc_1 && code != Code.Stloc_2 && code != Code.Stloc_3)
					break;
				if (varN != ((StoreLocal) instruction).Variable)
					break;

				instruction = m_info.Instructions[index - 4];
				code = instruction.Untyped.OpCode.Code;
				if (code != Code.Call && code != Code.Callvirt)
					break;
				if (!((Call) instruction).Target.ToString().StartsWith("System.Int32 System.Array::IndexOf<") || !((Call) instruction).Target.ToString().EndsWith(">(!!0[],!!0)"))
					break;

				match = true;
			}
			while (false);

			return match;
		}

		private int m_offset;
		private MethodInfo m_info;
	}
}
