using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tekla.Structures;
using Tekla.Structures.Model;

namespace ContentAttributes
{
	public class ContentAttributeDemo
	{
		private IEnumerable<object> _modelObjects;
		private List<ContentAttribute> _contentAttributes;

		private const string PartPath = @"D:\RazorCX\partproperties.csv";
		private const string ConnectionPath = @"D:\RazorCX\connectionproperties.csv";
		private const string AttributeFolder = @"D:\RazorCX\ContentAttributes\";
		private const string ContentAttributesPath = @"D:\RazorCX\contentattributes_global.csv";

		private const string StringDataType = "string";
		private const string DoubleDataType = "double";
		private const string IntDataType = "int";

		public ContentAttributeDemo()
		{
			TeklaStructures.Connect();

			FetchContentAttributes();
		}

		public void RunDemo1()
		{
			//part
			Console.WriteLine("Pick Beam\n");

			var part = TeklaStructures.Model.PickObject(string.Empty) as Part;
			var pProperties = GetPartContentAttribute(part);
			pProperties.SaveAsCsv(PartPath);

			//connection
			Console.WriteLine("\nPick Connection\n");

			var connection = TeklaStructures.Model.PickObject(string.Empty) as Connection;
			var cProperties = GetConnectionContentAttribute(connection);
			cProperties.SaveAsCsv(ConnectionPath);
		}

		public void RunDemo2()
		{
			Console.WriteLine("\nCreating Csv Files for all Parts\n");

			TeklaStructures.Connect();

			ModelObjectEnumerator.AutoFetch = true;
			_modelObjects = TeklaStructures.Model.AllObjects;

			var parts = _modelObjects.OfType<Part>().ToList();

			var contentAttributes = parts
				.Where(p => p.GetReportValueInt("MAIN_PART") == 1)
				.Select(GetPartContentAttribute).ToList();
		}

		private void FetchContentAttributes()
		{
			var csvFile = File.ReadAllLines(ContentAttributesPath);

			_contentAttributes = csvFile
				.Skip(1)
				.Select(ContentAttribute.FromCsv)
				.ToList();
		}

		public List<ContentAttributeView> GetPartContentAttribute(Part part)
		{
			var names = GetPropertyNames("PART");

			var hash = new Hashtable();
			part.GetAllReportProperties(names[StringDataType], names[DoubleDataType], names[IntDataType], ref hash);

			var view = GetAttributes(hash).ToJson().FromJson<List<ContentAttributeView>>();
			view.SaveAsCsv($@"{AttributeFolder}{view.FirstOrDefault(a => a.Name == "GUID")?.Value}.csv");

			var viewLookup = view.ToLookup(v => v.Name);

			Console.WriteLine($"{viewLookup["GUID"].FirstOrDefault()?.Value} " +
			                  $"| {viewLookup["NAME"].FirstOrDefault()?.Value} " +
			                  $"| {viewLookup["PROFILE"].FirstOrDefault()?.Value}");

			return view;
		}

		public List<ContentAttributeView> GetConnectionContentAttribute(Connection connection)
		{
			var names = GetPropertyNames("CONNECTION");

			var hash = new Hashtable();
			connection.GetAllReportProperties(names[StringDataType], names[DoubleDataType], names[IntDataType], ref hash);

			var view = GetAttributes(hash).ToJson().FromJson<List<ContentAttributeView>>();
			view.SaveAsCsv($@"{AttributeFolder}{view.FirstOrDefault(a => a.Name == "GUID")?.Value}.csv");

			var viewLookup = view.ToLookup(v => v.Name);

			Console.WriteLine($"{viewLookup["GUID"].FirstOrDefault()?.Value} " +
			                  $"| {viewLookup["NAME"].FirstOrDefault()?.Value} ({viewLookup["CONNECTION_NUMBER"].FirstOrDefault()?.Value})" +
			                  $"| {viewLookup["PROFILE"].FirstOrDefault()?.Value}");

			return view;
		}

		private Dictionary<string, ArrayList> GetPropertyNames(string type)
		{
			return new Dictionary<string, ArrayList>()
			{
				{
					StringDataType,
					new ArrayList(_contentAttributes.Where(a => a.Type == type && a.Datatype == StringDataType).Select(a => a.Name)
						.ToList())
				},
				{
					IntDataType,
					new ArrayList(_contentAttributes.Where(a => a.Type == type && a.Datatype == IntDataType).Select(a => a.Name)
						.ToList())
				},
				{
					DoubleDataType,
					new ArrayList(_contentAttributes.Where(a => a.Type == type && a.Datatype == DoubleDataType).Select(a => a.Name)
						.ToList())
				},
			};
		}

		private List<ContentAttribute> GetAttributes(Hashtable hash)
		{
			var properties = hash.Keys
				.OfType<string>()
				.Select(k =>
					new ContentAttribute
					{
						Name = k,
						Value = hash[k],
						Datatype = _contentAttributes.FirstOrDefault(a => a.Name == k)?.Datatype
					})
				.OrderBy(p => p.Name)
				.ToList();

			return properties;
		}
	}
}