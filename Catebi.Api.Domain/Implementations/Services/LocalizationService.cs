using Catebi.Api.Domain.Contracts.Services;
using Catebi.Api.Domain.Features.AdoptionBot.Enums;

namespace Catebi.Api.Domain.Implementations.Services;

public class LocalizationService : ILocalizationService
{
    public string GetUserConfirmationMessage(Languages language, string userName, bool isVolunteer)
    {
        var volunteerStatus = isVolunteer
            ? GetVolunteerStatusText(language)
            : GetCatOwnerStatusText(language);

        return language switch
        {
            Languages.ru => $"Привет, {userName} 👋\nВаш аккаунт подтвержден {volunteerStatus}! Добро пожаловать в наше сообщество!",
            Languages.en => $"Hello {userName} 👋\nYour account has been confirmed {volunteerStatus}! Welcome to our community!",
            _ => $"Hello {userName} 👋\nYour account has been confirmed {volunteerStatus}! Welcome to our community!"
        };
    }

    public string GetCatPaymentConfirmationMessage(Languages language, string ownerName, string catName)
    {
        return language switch
        {
            Languages.ru => $@"Привет, {ownerName} 👋
Платеж за вашу кошку (кота) {catName} подтвержден. Поздравляем!

Теперь вы можете добавить свою кошку (кота) в Кэтбук или забронировать для него мероприятие.",

            Languages.en => $@"Hello {ownerName} 👋
Payment for your cat {catName} has been confirmed. Congrats!

You can now access to push your cat to the Catbook or to book event for them.",

            _ => $@"Hello {ownerName} 👋
Payment for your cat {catName} has been confirmed. Congrats!

You can now access to push your cat to the Catbook or to book event for them."
        };
    }

    public string GetEventOpenNotificationMessage(Languages language, string eventName, DateTime eventDate, string eventLocation, string eventDescription)
    {
        return language switch
        {
            Languages.ru => $@"🎉 <b>Новое мероприятие открыто для регистрации!</b> 🎉

<b>{eventName}</b>
📅 <b>Дата:</b> {eventDate:yyyy-MM-dd}
📍 <b>Место:</b> {eventLocation}

📝 <b>Описание:</b>
{eventDescription}

🐱 Теперь вы можете зарегистрировать своих кошек и котов на это мероприятие! Не упустите возможность!",

            Languages.en => $@"🎉 <b>New Event Open for Registration!</b> 🎉

<b>{eventName}</b>
📅 <b>Date:</b> {eventDate:yyyy-MM-dd}
📍 <b>Location:</b> {eventLocation}

📝 <b>Description:</b>
{eventDescription}

🐱 You can now register your cats for this event! Don't miss out!",

            _ => $@"🎉 <b>New Event Open for Registration!</b> 🎉

<b>{eventName}</b>
📅 <b>Date:</b> {eventDate:yyyy-MM-dd}
📍 <b>Location:</b> {eventLocation}

📝 <b>Description:</b>
{eventDescription}

🐱 You can now register your cats for this event! Don't miss out!"
        };
    }

    public string GetBroadcastMessage(Languages language, string content)
    {
        // For broadcast messages, we preserve the original content as admins can write in any language
        // In the future, you could add automatic translation or language-specific templates
        return content;
    }

    public string GetAdminUserRegistrationNotification(Languages language, string userName, string userTelegram, string userRecordId)
    {
        return language switch
        {
            Languages.ru => $@"🆕 <b>Новая регистрация пользователя</b>

👤 <b>Имя:</b> {userName}
📱 <b>Telegram:</b> {userTelegram}
🆔 <b>Record ID:</b> {userRecordId}

⏳ Требует подтверждения администратором",

            Languages.en => $@"🆕 <b>New User Registration</b>

👤 <b>Name:</b> {userName}
📱 <b>Telegram:</b> {userTelegram}
🆔 <b>Record ID:</b> {userRecordId}

⏳ Requires admin confirmation",

            _ => $@"🆕 <b>New User Registration</b>

👤 <b>Name:</b> {userName}
📱 <b>Telegram:</b> {userTelegram}
🆔 <b>Record ID:</b> {userRecordId}

⏳ Requires admin confirmation"
        };
    }

    public string GetAdminPaymentSubmissionNotification(Languages language, string catName, string ownerName, string catRecordId, string paymentRecordId)
    {
        return language switch
        {
            Languages.ru => $@"💳 <b>Новая подача платежной информации</b>

🐱 <b>Кошка (кот):</b> {catName}
👤 <b>Владелец:</b> {ownerName}
🆔 <b>Cat Record ID:</b> {catRecordId}
🆔 <b>Payment Record ID:</b> {paymentRecordId}

⏳ Требует подтверждения администратором",

            Languages.en => $@"💳 <b>New Payment Information Submitted</b>

🐱 <b>Cat:</b> {catName}
👤 <b>Owner:</b> {ownerName}
🆔 <b>Cat Record ID:</b> {catRecordId}
🆔 <b>Payment Record ID:</b> {paymentRecordId}

⏳ Requires admin confirmation",

            _ => $@"💳 <b>New Payment Information Submitted</b>

🐱 <b>Cat:</b> {catName}
👤 <b>Owner:</b> {ownerName}
🆔 <b>Cat Record ID:</b> {catRecordId}
🆔 <b>Payment Record ID:</b> {paymentRecordId}

⏳ Requires admin confirmation"
        };
    }

    public string GetAdminCatAdoptionNotification(Languages language, string catName, string ownerName, string catRecordId, string? adoptionComment = null, string? actionByUserName = null, string? actionByUserTelegram = null)
    {
        var baseMessage = language switch
        {
            Languages.ru => $@"🎉 <b>Случилось укотовление!</b>

🐱 <b>Кошка (кот):</b> {catName}
👤 <b>Владелец:</b> {ownerName}
🆔 <b>Cat Record ID:</b> {catRecordId}

✅ Статус изменен на 'Укотовление'",

            Languages.en => $@"🎉 <b>Cat Found a New Home!</b>

🐱 <b>Cat:</b> {catName}
👤 <b>Owner:</b> {ownerName}
🆔 <b>Cat Record ID:</b> {catRecordId}

✅ Status changed to 'Adopted'",

            _ => $@"🎉 <b>Cat Found a New Home!</b>

🐱 <b>Cat:</b> {catName}
👤 <b>Owner:</b> {ownerName}
🆔 <b>Cat Record ID:</b> {catRecordId}

✅ Status changed to 'Adopted'"
        };

        // Add action by user information if provided
        if (!string.IsNullOrWhiteSpace(actionByUserName))
        {
            var actionSection = language switch
            {
                Languages.ru => $"\n\n🧑‍💼 <b>Действие выполнено:</b> {actionByUserName}",
                Languages.en => $"\n\n🧑‍💼 <b>Action performed by:</b> {actionByUserName}",
                _ => $"\n\n🧑‍💼 <b>Action performed by:</b> {actionByUserName}"
            };
            
            if (!string.IsNullOrWhiteSpace(actionByUserTelegram))
            {
                actionSection += $" ({actionByUserTelegram})";
            }
            
            baseMessage += actionSection;
        }

        // Add adoption comment if provided
        if (!string.IsNullOrWhiteSpace(adoptionComment))
        {
            var commentSection = language switch
            {
                Languages.ru => $"\n\n💬 <b>Комментарий:</b> {adoptionComment}",
                Languages.en => $"\n\n💬 <b>Comment:</b> {adoptionComment}",
                _ => $"\n\n💬 <b>Comment:</b> {adoptionComment}"
            };
            baseMessage += commentSection;
        }

        return baseMessage;
    }

    private string GetVolunteerStatusText(Languages language)
    {
        return language switch
        {
            Languages.ru => "как волонтер",
            Languages.en => "as a volunteer",
            _ => "as a volunteer"
        };
    }

    private string GetCatOwnerStatusText(Languages language)
    {
        return language switch
        {
            Languages.ru => "как владелец кошки (кота)",
            Languages.en => "as a cat owner",
            _ => "as a cat owner"
        };
    }
}
