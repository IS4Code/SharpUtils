/* Date: 7.9.2017, Time: 13:01 */
using System;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace IllidanS4.SharpUtils.IO
{
	public class PipeListener
	{
		public string Name{get; private set;}
		public PipeDirection Direction{get; private set;}
		public PipeTransmissionMode TransmissionMode{get; private set;}
		
		public PipeListener(string pipeName, PipeDirection direction) : this(pipeName, direction, PipeTransmissionMode.Byte)
		{
			
		}
		
		public PipeListener(string pipeName, PipeDirection direction, PipeTransmissionMode transmissionMode)
		{
			Name = pipeName;
			Direction = direction;
			TransmissionMode = transmissionMode;
		}
		
		public NamedPipeServerStream AcceptClient()
		{
			var stream = new NamedPipeServerStream(Name, Direction, NamedPipeServerStream.MaxAllowedServerInstances, TransmissionMode);
			stream.WaitForConnection();
			return stream;
		}
		
		public async Task<NamedPipeServerStream> AcceptClientAsync(CancellationToken cancellationToken)
		{
			var stream = new NamedPipeServerStream(Name, Direction, NamedPipeServerStream.MaxAllowedServerInstances, TransmissionMode, PipeOptions.Asynchronous);
			await stream.WaitForConnectionAsync(cancellationToken);
			return stream;
		}
	}
}
