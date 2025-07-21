using AirtableApiClient;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Catebi.Api.Domain.Features.AdoptionBot;

public class AirtableRepository(AirtableBase AirtableBase, ILogger<AirtableRepository> Logger) : IAirtableRepository
{

    public async Task<AirtableRetrieveRecordResponse<T>> RetrieveRecord<T>(string tableName, string recordId) where T : class
    {
        try
        {
            Logger.LogInformation($"Retrieving record from table {tableName} with ID {recordId}");
            return await AirtableBase.RetrieveRecord<T>(tableName, recordId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Error retrieving record from table {tableName} with ID {recordId}");
            throw;
        }
    }

    public async Task<AirtableListRecordsResponse<T>> ListRecords<T>(
        string tableName,
        string? filterByFormula = null,
        int? pageSize = null,
        string? offset = null,
        IEnumerable<Sort>? sort = null) where T : class
    {
        try
        {
            Logger.LogInformation($"Listing records from table {tableName} with filter: {filterByFormula}, pageSize: {pageSize}, offset: {offset}");
            return await AirtableBase.ListRecords<T>(
                tableName,
                filterByFormula: filterByFormula,
                pageSize: pageSize,
                offset: offset,
                sort: sort
            );
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Error listing records from table {tableName}");
            throw;
        }
    }

    public async Task<AirtableListRecordsResponse<T>> ListRecords<T>(
        string tableName,
        IEnumerable<string> fields,
        string? filterByFormula = null,
        int? pageSize = null,
        string? offset = null,
        IEnumerable<Sort>? sort = null) where T : class
    {
        try
        {
            var fieldsArray = fields.ToArray();
            Logger.LogInformation($"Listing records from table {tableName} with fields: [{string.Join(", ", fieldsArray)}], filter: {filterByFormula}, pageSize: {pageSize}, offset: {offset}");
            return await AirtableBase.ListRecords<T>(
                tableName,
                fields: fieldsArray,
                filterByFormula: filterByFormula,
                pageSize: pageSize,
                offset: offset,
                sort: sort
            );
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Error listing records from table {tableName}");
            throw;
        }
    }

    public async Task<IEnumerable<T>> ListRecordsAutoFields<T>(
        string tableName,
        string? filterByFormula = null,
        int? pageSize = null,
        string? offset = null,
        IEnumerable<Sort>? sort = null) where T : class
    {
        var fields = GetFieldNamesFromType<T>();
        var records = await ListRecords<T>(tableName, fields, filterByFormula, pageSize, offset, sort);
        return records.Records.Select(r => r.Fields).OfType<T>().ToList();
    }

    private static string[] GetFieldNamesFromType<T>() where T : class
    {
        var type = typeof(T);
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var fieldNames = new List<string>();

        foreach (var property in properties)
        {
            // Skip properties marked with JsonIgnore
            if (property.GetCustomAttribute<JsonIgnoreAttribute>() != null)
                continue;

            // Check for JsonPropertyName attribute
            var jsonPropertyName = property.GetCustomAttribute<JsonPropertyNameAttribute>();
            var fieldName = jsonPropertyName?.Name ?? property.Name;

            fieldNames.Add(fieldName);
        }

        return fieldNames.ToArray();
    }

    // Public method for testing field generation
    public static string[] GetFieldNamesFromTypePublic<T>() where T : class
    {
        return GetFieldNamesFromType<T>();
    }

    public async Task<AirtableCreateUpdateReplaceRecordResponse> CreateRecord(string tableName, Fields fields)
    {
        try
        {
            Logger.LogInformation($"Creating record in table {tableName}");
            return await AirtableBase.CreateRecord(tableName, fields);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Error creating record in table {tableName}");
            throw;
        }
    }

    public async Task<AirtableCreateUpdateReplaceRecordResponse> UpdateRecord(string tableName, Fields fields, string recordId)
    {
        try
        {
            Logger.LogInformation($"Updating record in table {tableName} with ID {recordId}");
            return await AirtableBase.UpdateRecord(tableName, fields, recordId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Error updating record in table {tableName} with ID {recordId}");
            throw;
        }
    }

    public async Task<AirtableDeleteRecordResponse> DeleteRecord(string tableName, string recordId)
    {
        try
        {
            Logger.LogInformation($"Deleting record from table {tableName} with ID {recordId}");
            return await AirtableBase.DeleteRecord(tableName, recordId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Error deleting record from table {tableName} with ID {recordId}");
            throw;
        }
    }
}
