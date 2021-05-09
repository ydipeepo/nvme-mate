using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Ydi.NvmeMate
{
	public sealed class StringLocalizer
	{
		static readonly Assembly executingAssembly = Assembly.GetExecutingAssembly();
		readonly Dictionary<string, string> stringMap = new();

		static CultureInfo ParseLanguage(string language)
		{
			if (!string.IsNullOrWhiteSpace(language))
			{
				try
				{
					return new CultureInfo(language);
				}
				catch
				{
				}
			}
			return CultureInfo.InstalledUICulture;
		}

		public string this[string name] => stringMap[name];

		public StringLocalizer(CultureInfo cultureInfo)
		{
			using var resourceStream =
				executingAssembly.GetManifestResourceStream($"Ydi.Resources.{cultureInfo}.resources") ??
				executingAssembly.GetManifestResourceStream($"Ydi.Resources.en-US.resources");
			using var resourceReader = new ResourceReader(resourceStream);
			var resourceEnumerator = resourceReader.GetEnumerator();
			while (resourceEnumerator.MoveNext())
			{
				if (resourceEnumerator.Key is string key &&
					resourceEnumerator.Value is string value)
				{
					stringMap.Add(key, value);
				}
			}
		}
		public StringLocalizer(string language)
			: this(ParseLanguage(language))
		{
		}
		public StringLocalizer()
			: this(CultureInfo.InstalledUICulture)
		{
		}
	}
}
