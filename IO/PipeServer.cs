/* Date: 7.9.2017, Time: 13:23 */
using System;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace IllidanS4.SharpUtils.IO
{
	public class PipeServer
	{
		public string PipeName{get; set;}
		public PipeDirection PipeDirection{get; set;}
		public PipeTransmissionMode PipeTransmissionMode{get; set;}
		
		public PipeListener Listener{get; private set;}
		
		public event EventHandler<NamedPipeEventArgs> PipeOpened;
		
		private CancellationTokenSource tokenSource;
		
		public PipeServer()
		{
			PipeDirection = PipeDirection.Out;
			PipeTransmissionMode = PipeTransmissionMode.Byte;
		}
		
		public void Start()
		{
			if(Listener != null) throw new InvalidOperationException("The server is already running.");
			Listener = new PipeListener(PipeName, PipeDirection, PipeTransmissionMode);
			tokenSource = new CancellationTokenSource();
			
			CreateServerTask(tokenSource.Token);
		}
		
		public void Stop()
		{
			if(Listener == null) throw new InvalidOperationException("The server is not running.");
			
			tokenSource.Cancel();
		}
		
		private Task CreateServerTask(CancellationToken token)
		{
			return Task.Run(
				(Func<Task>)async delegate{
					while(!token.IsCancellationRequested)
					{
						var stream = await Listener.AcceptClientAsync(token);
						OnPipeOpened(stream);
					}
				}, token
			);
		}
		
		private void OnPipeOpened(NamedPipeServerStream stream)
		{
			if(PipeOpened != null)
			{
				PipeOpened(this, new NamedPipeEventArgs(stream));
			}
		}
	}
	
	[Serializable]
	public class NamedPipeEventArgs : EventArgs
	{
		public NamedPipeServerStream Stream{get; private set;}
		
		public NamedPipeEventArgs(NamedPipeServerStream pipeStream)
		{
			Stream = pipeStream;
		}
	}
}
