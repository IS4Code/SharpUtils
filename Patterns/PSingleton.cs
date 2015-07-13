using System;

namespace IllidanS4.SharpUtils.Patterns
{
	[Pattern]
	public abstract class PSingleton<[StaticType]T> : ISingleton
	{
		public abstract T Instance{
			get;
		}
		
		object ISingleton.Instance{
			get{
				return this.Instance;
			}
		}
	}
}
