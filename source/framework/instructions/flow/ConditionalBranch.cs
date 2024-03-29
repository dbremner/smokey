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

namespace Smokey.Framework.Instructions
{		
	/// <summary>Represents Brfalse, Brfalse_S, Brtrue, Brtrue_S, Beq, Beq_S, Bge, Bge_S, 
	/// Bge_Un, Bge_Un_S, Bgt, Bgt_S, Bgt_Un, Bgt_Un_S, Ble, Ble_S, Ble_Un, 
	/// Ble_Un_S, Blt, Blt_S, Blt_Un, Blt_Un_S, Bne_Un, and Bne_Un_S.</summary>
	public class ConditionalBranch : Branch
	{		
		internal ConditionalBranch(Instruction untyped, int index) : base(untyped, index)
		{
		}
	}
}

