/* Date: ‎20.12.‎2012, ‏‎Time: 17:12 */
using System;
using System.Collections.Generic;
using System.Globalization;

namespace IllidanS4.SharpUtils.Globalization
{
	public class Term
	{
		Dictionary<CultureInfo, object> Values = new Dictionary<CultureInfo, object>();
		
		public Term(IFormattable currentValue, IFormattable invariantValue)
		{
			Values[CultureInfo.CurrentCulture] = currentValue;
			Values[CultureInfo.InvariantCulture] = invariantValue;
		}
		public Term(string currentValue, string invariantValue)
		{
			Values[CultureInfo.CurrentCulture] = currentValue;
			Values[CultureInfo.InvariantCulture] = invariantValue;
		}
		
		public Term(Dictionary<CultureInfo, IFormattable> values)
		{
			foreach(var pair in values)
			{
				Values[pair.Key] = pair.Value;
			}
		}
		public Term(Dictionary<CultureInfo, string> values)
		{
			foreach(var pair in values)
			{
				Values[pair.Key] = pair.Value;
			}
		}
		
		public override string ToString()
		{
			object value = null;
			CultureInfo culture = null;
			if(Values.ContainsKey(CultureInfo.CurrentCulture))
			{
				value = Values[CultureInfo.CurrentCulture];
				culture = CultureInfo.CurrentCulture;
			}else if(Values.ContainsKey(CultureInfo.InvariantCulture))
			{
				value = Values[CultureInfo.InvariantCulture];
				culture = CultureInfo.InvariantCulture;
			}else foreach(var pair in Values)
			{
				if(pair.Value != null)
				{
					culture = pair.Key;
					value = pair.Value;
					break;
				}
			}
			return Format(value, String.Empty, culture);
		}
		
		private static string Format(object toFormat, string format, IFormatProvider provider)
		{
			if(toFormat == null) return null;
			if(toFormat is IFormattable)
			{
				return ((IFormattable)toFormat).ToString(format, provider);
			}else{
				return toFormat.ToString();
			}
		}
	}
}