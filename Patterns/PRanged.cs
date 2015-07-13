using System;

namespace IllidanS4.SharpUtils.Patterns
{
	[Pattern]
	public abstract class PRanged<[StaticType]T> : IRanged
	{
		public abstract T MaxValue{
			get;
		}
		public abstract T MinValue{
			get;
		}
		
		object IRanged.MaxValue{
			get{
				return MaxValue;
			}
		}
		object IRanged.MinValue{
			get{
				return MinValue;
			}
		}
	}
}
