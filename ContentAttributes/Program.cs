using System;

namespace ContentAttributes
{
	public class Program
	{
		static void Main(string[] args)
		{
			try
			{
				var contentAttributeService = new ContentAttributeDemo();
				contentAttributeService.RunDemo1();
				contentAttributeService.RunDemo2();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}
	}
}
