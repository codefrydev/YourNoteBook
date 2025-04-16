using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace YourNoteBook.Pages.DataPages;

public partial class CsvEditor : ComponentBase
{ 
    public class CellItem
    {
        public string? Value { get; set; }
        public bool IsEditing { get; set; }
        public bool IsHovered { get; set; }
    }
    
    private bool _isEditMode = false;
    private bool _isLoading = false;
    private List<List<CellItem>> _tableData = [];

    private int _numberOfDataToBeGenerated = 1000;

    private int Value
    {
        get;
        set; 
    }

    private void GenerateDataAsync()
    {
        _isLoading = true;
        StateHasChanged();
        _ = Task.Run( async () =>
        {
            await GeneratingDataAsync();
        });
    } 
    private async Task GeneratingDataAsync()
    {
        var localTableData = new List<List<CellItem>>(_numberOfDataToBeGenerated); 
        for( var i = 0; i < _numberOfDataToBeGenerated; i++)
        { 
            var data  = Faker.Extensions.EnumerableExtensions.Times<CellItem>(5, cellItem => new CellItem
            {
                Value = Faker.Name.First()
            }).ToList();

            Value = i + 1;
            await InvokeAsync(StateHasChanged);
            await Task.Delay(1); // allows UI refresh 
            localTableData.Add(data); 
        }

        _tableData = localTableData;
        _isLoading = false;
        await InvokeAsync(StateHasChanged);
    }

    private static void ToggleEdit(CellItem data)
    {
        data.IsEditing = !data.IsEditing;
    }
    private string DownloadLink => GenerateDownloadLink();
 
    private async Task HandleFileSelected(InputFileChangeEventArgs e)
    {
        var file = e.File;
        await using var stream = file.OpenReadStream();
        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync();

        // Split the content into rows and then cells.
        _tableData = content.Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Trim().Split(',', StringSplitOptions.None).Select(x=> new CellItem()
            {
                Value = x, 
            }).ToList())
            .ToList();

        // If CSV is not empty, ensure every data row has exactly the same column count as header.
        if (_tableData.Count > 0)
        {
            var headerCount = _tableData[0].Count;

            for (var i = 1; i < _tableData.Count; i++)
            {
                // Pad rows that have fewer columns than header.
                while (_tableData[i].Count < headerCount)
                {
                    _tableData[i].Add(new CellItem());
                }
                // Trim rows that have more columns than header.
                if (_tableData[i].Count > headerCount)
                {
                    _tableData[i] = _tableData[i].Take(headerCount).ToList();
                }
            }
        }
    }
 
    private void ToggleEditMode()
    {
        _isEditMode = !_isEditMode;
    }
 
    private void SortTable(CellItem data)
    {
        if (_tableData.Count <= 1)
            return;
        var colIndex = _tableData[0].IndexOf(data);
        var header = _tableData[0];
        var sortedRows = _tableData.Skip(1)
            .OrderBy(row => row.ElementAtOrDefault(colIndex)?.Value)
            .ToList();
        _tableData = [header];
        _tableData.AddRange(sortedRows);
        StateHasChanged();
    } 
    private string GenerateDownloadLink()
    {
        if (_tableData.Count == 0)
            return string.Empty;

        var csvContent = new StringBuilder();
        foreach (var row in _tableData)
        {
            csvContent.AppendLine(string.Join(",", row.Select(cell => cell.Value)));
        }
        var bytes = Encoding.UTF8.GetBytes(csvContent.ToString());
        var base64 = Convert.ToBase64String(bytes);
        return $"data:text/csv;base64,{base64}";
    }
}