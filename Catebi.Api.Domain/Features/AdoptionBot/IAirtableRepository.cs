using AirtableApiClient;

namespace Catebi.Api.Domain.Features.AdoptionBot;

public interface IAirtableRepository
{
    Task<AirtableRetrieveRecordResponse<T>> RetrieveRecord<T>(string tableName, string recordId) where T : class;

    Task<AirtableListRecordsResponse<T>> ListRecords<T>(
        string tableName,
        string? filterByFormula = null,
        int? pageSize = null,
        string? offset = null,
        IEnumerable<Sort>? sort = null) where T : class;

    Task<AirtableListRecordsResponse<T>> ListRecords<T>(
        string tableName,
        IEnumerable<string> fields,
        string? filterByFormula = null,
        int? pageSize = null,
        string? offset = null,
        IEnumerable<Sort>? sort = null) where T : class;

    Task<IEnumerable<T>> ListRecordsAutoFields<T>(
        string tableName,
        string? filterByFormula = null,
        int? pageSize = null,
        string? offset = null,
        IEnumerable<Sort>? sort = null) where T : class;

    Task<AirtableCreateUpdateReplaceRecordResponse> CreateRecord(string tableName, Fields fields);
    Task<AirtableCreateUpdateReplaceRecordResponse> UpdateRecord(string tableName, Fields fields, string recordId);
}
