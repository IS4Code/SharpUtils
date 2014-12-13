using System;
namespace IllidanS4.SharpUtils.Numerics
{	
	public abstract class Set
	{
		public abstract bool Contains(Number num);
		public abstract bool IsFinite{get;}
		public virtual Number Count{
			get{
				if(IsFinite)
				{
					throw new NotImplementedException();
				}else{
					throw new InvalidOperationException();
				}
			}
		}
		
		public static readonly Set Q = new SetQ();
		public static readonly Set Z = new SetZ();
		public static readonly Set N = new SetN();
		public static readonly Set C = new SetC();
		
		private abstract class InfiniteSet : Set
		{
			public override bool IsFinite{
				get{
					return false;
				}
			}
			
			public override bool Equals(object obj)
			{
				if(obj == null)return false;
				return this.GetType() == obj.GetType();
			}
			
			public override int GetHashCode()
			{
				return base.GetHashCode();
			}
		}
		
		private class SetQ : InfiniteSet
		{
			public override bool Contains(Number num)
			{
				return num.IsRational;
			}
		}
		
		private class SetZ : InfiniteSet
		{
			public override bool Contains(Number num)
			{
				return num.IsWhole;
			}
		}
		
		private class SetN : InfiniteSet
		{
			public override bool Contains(Number num)
			{
				return num.IsNatural;
			}
		}
		
		private class SetC : InfiniteSet
		{
			public override bool Contains(Number num)
			{
				return num.IsComplex;
			}
		}
	}
}