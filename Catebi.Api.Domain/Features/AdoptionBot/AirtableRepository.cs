using AirtableApiClient;
using Microsoft.Extensions.Logging;

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

    public async Task<AirtableListRecordsResponse<T>> ListRecords<T>(string tableName, string? filterByFormula = null) where T : class
    {
        try
        {
            Logger.LogInformation($"Listing records from table {tableName} with filter: {filterByFormula}");
            return await AirtableBase.ListRecords<T>(tableName, filterByFormula: filterByFormula);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Error listing records from table {tableName}");
            throw;
        }
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
