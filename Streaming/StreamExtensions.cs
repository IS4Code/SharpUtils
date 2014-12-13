/* Date: 12.10.2014, Time: 0:05 */
using System;
using System.IO;

namespace IllidanS4.SharpUtils.Streaming
{
	public static class StreamExtensions
	{
		public static void Save(this Stream stream, string file)
		{
			Save(stream, file, FileMode.Create);
		}
		
		public static void Save(this Stream stream, string file, FileMode fileMode)
		{
			using(FileStream output = new FileStream(file, fileMode))
			{
				stream.CopyTo(output);
			}
		}
		public static void Save(this Stream stream, FileInfo file)
		{
			using(FileStream output = file.Create())
			{
				stream.CopyTo(output);
			}
		}
	}
}
