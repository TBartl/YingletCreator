public abstract class ModelItem
{
	/// <summary>
	/// The file-unique name of this item.
	/// </summary>
	public string name = "";

	public abstract string ModelItemToJSON(ModelBaseFormat format);

	public static string Flt(double v, bool highPrecision = false)
	{
		if (double.IsNaN(v))
		{
			return "0.0";
		}
		if (v == double.PositiveInfinity)
		{
			return "1e99999";
		}
		if (v == double.NegativeInfinity)
		{
			return "-1e99999";
		}
		float f = (float)v;
		string s;
		if (highPrecision)
		{
			s = v.ToString("G9", System.Globalization.CultureInfo.InvariantCulture);
		}
		else if (v == (double)f)
		{
			s = f.ToString(System.Globalization.CultureInfo.InvariantCulture);
		}
		else
		{
			s = v.ToString(System.Globalization.CultureInfo.InvariantCulture);
		}
		if (!s.Contains(".") && !s.Contains("E") && !s.Contains("e"))
		{
			s += ".0";
		}
		return s;
	}
}
