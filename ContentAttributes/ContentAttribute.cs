namespace ContentAttributes
{
	public class ContentAttribute
	{
		public string Type { get; set; }
		public string Name { get; set; }
		public object Value { get; set; }
		public string Datatype { get; set; }
		public string Justify { get; set; }
		public string Cacheable { get; set; }
		public int Length { get; set; }
		public int Decimals { get; set; }
		public string UnitType { get; set; }
		public string Unit { get; set; }
		public string Precision { get; set; }


		public static ContentAttribute FromCsv(string csvLine)
		{
			var line = csvLine.Split(',');

			var contentAttribute = new ContentAttribute
			{
				Type = line.Length > 0 ? line[0] : null,
				Name = line.Length > 1 ? line[1] : null,
				Datatype = line.Length > 2 ? line[2] : null,
				Justify = line.Length > 3 ? line[3] : null,
				Cacheable = line.Length > 4 ? line[4] : null,
				UnitType = line.Length > 7 ? line[7] : null,
				Unit = line.Length > 8 ? line[8] : null,
				Precision = line.Length > 9 ? line[9] : null
			};

			if (line.Length > 5)
			{
				int.TryParse(line[5], out int length);
				contentAttribute.Length = length;
			}
			if (line.Length > 6)
			{
				int.TryParse(line[6], out int decimals);
				contentAttribute.Decimals = decimals;
			}

			return contentAttribute;
		}
	}
}