using System;
using System.CommandLine.Rendering;

namespace Ydi.NvmeMate
{
	public static class StringExtensions
	{
		public static TextSpan Underline(this string value) => new ContainerSpan(StyleSpan.UnderlinedOn(), new ContentSpan(value), StyleSpan.UnderlinedOff());
		public static TextSpan Color(this string value, byte r, byte g, byte b) => new ContainerSpan(ForegroundColorSpan.Rgb(r, g, b), new ContentSpan(value), ForegroundColorSpan.Reset());
	}
}
