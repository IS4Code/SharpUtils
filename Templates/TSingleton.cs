using System;

namespace IllidanS4.SharpUtils.Templates
{
	[Template]
	public abstract class TSingleton<[StaticType]T> : ISingleton
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
