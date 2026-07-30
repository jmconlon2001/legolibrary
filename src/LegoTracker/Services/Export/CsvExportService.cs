using System.Globalization;
using CsvHelper;

namespace LegoTracker.Services.Export;

public class CsvExportService
{
    public string ExportMissingParts(IEnumerable<MissingPartExportRow> rows)
    {
        using var writer = new StringWriter();
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteHeader<MissingPartExportRow>();
            csv.NextRecord();
            foreach (var row in rows)
            {
                csv.WriteRecord(row);
                csv.NextRecord();
            }
        }

        return writer.ToString();
    }
}
