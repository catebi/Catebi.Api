using AirtableApiClient;
using Catebi.Api.Domain.Features.AdoptionBot.Enums;
using Catebi.Api.Domain.Features.AdoptionBot.Converters;

namespace Catebi.Api.Domain.Features.AdoptionBot;

public class AdoptionBotUserService(
    IAirtableRepository AirtableRepository,
    ILogger<AdoptionBotUserService> Logger) : IAdoptionBotUserService
{
    private readonly string UserTableName = AirTables.User.ToString();
    private readonly string CatPaymentName = AirTables.CatPayment.ToString();
    private readonly string CatTableName = AirTables.Cat.ToString();

    public async Task<UserDto> RegisterUser(UserDto userDto)
    {
        Logger.LogInformation($"Registering new user: {userDto.Name}");

        var fields = new Fields();
        fields.AddField("Name", userDto.Name);
        fields.AddField("Telegram", userDto.Telegram);
        fields.AddField("Status", UserStatuses.ToConfirm.ToString());
        fields.AddField("TelegramChatId", userDto.TelegramChatId);
        fields.AddField("Role", UserRoles.CatOwner.ToString());
        fields.AddField("Language", userDto.Language ?? Languages.en.ToString());

        var response = await AirtableRepository.CreateRecord(UserTableName, fields);

        if (!response.Success)
        {
            Logger.LogError($"Error registering user: {response.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error registering user: {response.AirtableApiError.ErrorMessage}");
        }

        var createdUser = await FindUserByTelegramId(userDto.TelegramChatId);
        Logger.LogInformation($"User registered successfully: {userDto.Name}");
        return createdUser!;
    }

    public async Task<UserDto?> FindUserByTelegramId(long telegramId)
    {
        Logger.LogInformation($"Finding user by Telegram ID: {telegramId}");

        var response = await AirtableRepository.ListRecords<AtUser>(
            UserTableName,
            filterByFormula: $"{{TelegramChatId}} = '{telegramId}'"
        );

        if (!response.Success)
        {
            Logger.LogError($"Error finding user: {response.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error finding user: {response.AirtableApiError.ErrorMessage}");
        }

        var records = response.Records.ToList();
        if (!records.Any())
        {
            Logger.LogInformation($"No user found with Telegram ID: {telegramId}");
            return null;
        }

        var user = records[0].Fields;
        user.RecordId = records[0].Id;
        Logger.LogInformation($"User found: {user.Name}");
        return UserConverter.ToDto(user);
    }

    public async Task<UserDto> UpdateUser(UserDto userDto)
    {
        if (string.IsNullOrEmpty(userDto.RecordId))
        {
            throw new ArgumentException("Record ID is required for updating a user");
        }

        Logger.LogInformation($"Updating user: {userDto.Name}");

        var fields = new Fields();

        fields.AddField("Name", userDto.Name);
        fields.AddField("Telegram", userDto.Telegram);
        fields.AddField("Status", userDto.Status);
        fields.AddField("TelegramChatId", userDto.TelegramChatId);
        fields.AddField("Role", userDto.Role);
        fields.AddField("Language", userDto.Language ?? Languages.en.ToString());
        fields.AddField(nameof(userDto.AdditionalContact), userDto.AdditionalContact);

        var response = await AirtableRepository.UpdateRecord(UserTableName, fields, userDto.RecordId);

        if (!response.Success)
        {
            Logger.LogError($"Error updating user: {response.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error updating user: {response.AirtableApiError.ErrorMessage}");
        }

        Logger.LogInformation($"User updated successfully: {userDto.Name}");
        return await FindUserByTelegramId(userDto.TelegramChatId) ?? userDto;
    }

    public async Task<IEnumerable<CatPaymentDto>> GetUserPayments(string userId)
    {
        Logger.LogInformation($"Getting payments for user: {userId}");

        var response = await AirtableRepository.ListRecords<AtCatPayment>(
            CatPaymentName,
            filterByFormula: $"{{OwnerRecordId}} = '{userId}'"
        );

        if (!response.Success)
        {
            Logger.LogError($"Error getting user payments: {response.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error getting user payments: {response.AirtableApiError.ErrorMessage}");
        }

        var payments = response.Records.Select(r => CatPaymentConverter.ToDto(r.Fields)).ToList();
        Logger.LogInformation($"Found {payments.Count} payments for user {userId}");
        return payments;
    }

    public async Task<IEnumerable<CatDto>> GetCats(string userRecordId)
    {
        Logger.LogInformation($"Getting cats for user: {userRecordId}");

        var response = await AirtableRepository.ListRecords<AtCat>(
            CatTableName,
            filterByFormula: $"{{OwnerRecordId}} = '{userRecordId}'"
        );

        if (!response.Success)
        {
            Logger.LogError($"Error getting user's cats: {response.AirtableApiError.ErrorMessage}");
            throw new Exception($"Error getting user's cats: {response.AirtableApiError.ErrorMessage}");
        }

        var cats = response.Records.Select(r => CatConverter.ToDto(r.Fields, recordId: r.Id)).ToList();
        Logger.LogInformation($"Found {cats.Count} cats for user {userRecordId}");
        return cats;
    }
}
