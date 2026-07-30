using System.Globalization;
using CsvHelper;

namespace LegoTracker.Services.Export;

/// <summary>
/// Exports Rebrickable's documented "Part, Color, Quantity" wanted-list CSV column format
/// (headers are order-independent). Not verified against every edge case of Rebrickable's
/// importer (e.g. exact accepted color-name spellings) since their help pages block
/// automated fetches — re-check against a real upload if the import is rejected.
/// </summary>
public class WantedListExportService
{
    private record WantedListRow(string Part, string Color, int Quantity);

    public string ExportWantedList(IEnumerable<MissingPartExportRow> rows)
    {
        using var writer = new StringWriter();
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteHeader<WantedListRow>();
            csv.NextRecord();
            foreach (var row in rows)
            {
                csv.WriteRecord(new WantedListRow(row.PartNum ?? "", row.ColorName ?? "", row.Quantity));
                csv.NextRecord();
            }
        }

        return writer.ToString();
    }
}
