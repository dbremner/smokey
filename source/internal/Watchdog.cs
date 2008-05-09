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

using System;
using System.Threading;
using Smokey.Framework;

namespace Smokey.Internal
{
	// Kills the process if a rule takes too long to run.
	internal class Watchdog : IDisposable
	{		
		// Starts our thread up.
		public Watchdog(bool verbose)
		{			
			m_verbose = verbose;

			m_thread = new Thread(this.DoThread);
			m_thread.Start();
		}
		
		// Called when the framework starts processing a new rule.
		public void Add(string name)
		{
	        if (m_disposed)        
    	        throw new ObjectDisposedException("Watchdog has been disposed");
    	                    
			lock (m_lock)
			{
				m_name = name;
			}

			Ignore.Value = m_event.Set();
		}
				
		// Joins the thread.
		public void Shutdown()
		{
			if (m_running)
			{
				if (m_disposed)        
					throw new ObjectDisposedException("Watchdog has been disposed");
				
				lock (m_lock)
				{
					m_running = false;
				}
				
				Ignore.Value = m_event.Set();
				bool terminated = m_thread.Join(1000);
				DBC.Assert(terminated, "thread didn't terminate");
			}
		}
		
		public void Dispose()
		{
			Shutdown();		

			if (!m_disposed)
			{
				m_event.Close();				
				m_disposed = true;
			}
		}

		#region Private methods
		[DisableRule("D1038", "DontExit2")]		// we can't really throw because we are in a thread
		private void DoThread(object instance)
		{
			while (true)
			{
				bool signaled = m_event.WaitOne(m_timeout, false);

				lock (m_lock)
				{
					if (!signaled)
					{
						if (m_verbose)
							Console.Error.WriteLine("...Timed out");
						else
							Console.Error.WriteLine("Timed out running {0}", m_name);
	
						Environment.Exit(2);						
					}

					if (!m_running)
						break;
				}
			}
		}
		#endregion
		
		#region Fields
		private readonly Thread m_thread;
		private readonly TimeSpan m_timeout = TimeSpan.FromSeconds(30);	// TODO: may want to make this configurable
		private readonly AutoResetEvent m_event = new AutoResetEvent(false);
		private readonly bool m_verbose;
		private bool m_disposed = false;
		
		private object m_lock = new object();
			private bool m_running = true;
			private string m_name;
		#endregion
	}
}