using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using MoreLinq;
using Newtonsoft.Json;
using Tekla.Structures.Model;

namespace ContentAttributes
{
	public static class ExtensionMethods
	{
		public static string ToJson(this object obj, bool formatting = true)
		{
			return
				formatting
					? JsonConvert.SerializeObject(obj, Formatting.Indented)
					: JsonConvert.SerializeObject(obj, Formatting.None);
		}

		public static T FromJson<T>(this string json)
		{
			return JsonConvert.DeserializeObject<T>(json);
		}

		public static List<ModelObject> ToList(this ModelObjectEnumerator modelObjectEnumerator)
		{
			ModelObjectEnumerator.AutoFetch = true;

			try
			{
				if (modelObjectEnumerator == null) return null;

				var objs = new List<ModelObject>();
				modelObjectEnumerator.Reset();

				foreach (ModelObject modelObject in modelObjectEnumerator)
					objs.Add(modelObject);

				return objs;
			}
			catch
			{
				return new List<ModelObject>();
			};
		}

		public static void SaveAsCsv<T>(this List<T> source, string path)
		{
			source.ToDataTable().ToCSV(",").SaveCsv(path);
		}

		public static string ToCSV(this DataTable table, string delimator)
		{
			var result = new StringBuilder();
			for (int i = 0; i < table.Columns.Count; i++)
			{
				result.Append(table.Columns[i].ColumnName);
				result.Append(i == table.Columns.Count - 1 ? "\n" : delimator);
			}
			foreach (DataRow row in table.Rows)
			{
				for (int i = 0; i < table.Columns.Count; i++)
				{
					result.Append(row[i].ToString().Replace(',', ' '));
					result.Append(i == table.Columns.Count - 1 ? "\n" : delimator);
				}
			}
			return result.ToString().TrimEnd(new char[] { '\r', '\n' });
		}

		public static void SaveCsv(this string csv, string path)
		{
			using (StreamWriter writer = new StreamWriter(path))
			{
				writer.Write(csv);
			}
		}

		public static int GetReportValueInt(this Part part, string name)
		{
			try
			{
				if (part == null) return 0;
				int value = 0;
				part.GetReportProperty(name, ref value);
				return value;
			}
			catch
			{
				return 0;
			};
		}
	}
}