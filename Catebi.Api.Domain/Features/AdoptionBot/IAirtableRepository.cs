using AirtableApiClient;
using Catebi.Api.Domain.Features.AdoptionBot.Models;

namespace Catebi.Api.Domain.Features.AdoptionBot;

public interface IAirtableRepository
{
    /// <summary>
    /// Retrieves a single record from Airtable
    /// </summary>
    Task<AirtableRetrieveRecordResponse<T>> RetrieveRecord<T>(string tableName, string recordId) where T : class;

    /// <summary>
    /// Lists records from Airtable with optional filtering
    /// </summary>
    Task<AirtableListRecordsResponse<T>> ListRecords<T>(string tableName, string? filterByFormula = null) where T : class;

    /// <summary>
    /// Creates a new record in Airtable
    /// </summary>
    Task<AirtableCreateUpdateReplaceRecordResponse> CreateRecord(string tableName, Fields fields);

    /// <summary>
    /// Updates an existing record in Airtable
    /// </summary>
    Task<AirtableCreateUpdateReplaceRecordResponse> UpdateRecord(string tableName, Fields fields, string recordId);

    /// <summary>
    /// Deletes a record from Airtable
    /// </summary>
    Task<AirtableDeleteRecordResponse> DeleteRecord(string tableName, string recordId);
} 