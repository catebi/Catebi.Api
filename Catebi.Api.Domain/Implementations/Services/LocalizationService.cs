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

⏳ Требует подтверждения администратором",

            Languages.en => $@"🆕 <b>New User Registration</b>

👤 <b>Name:</b> {userName}
📱 <b>Telegram:</b> {userTelegram}

⏳ Requires admin confirmation",

            _ => $@"🆕 <b>New User Registration</b>

👤 <b>Name:</b> {userName}
📱 <b>Telegram:</b> {userTelegram}

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

⏳ Требует подтверждения администратором",

            Languages.en => $@"💳 <b>New Payment Information Submitted</b>

🐱 <b>Cat:</b> {catName}
👤 <b>Owner:</b> {ownerName}

⏳ Requires admin confirmation",

            _ => $@"💳 <b>New Payment Information Submitted</b>

🐱 <b>Cat:</b> {catName}
👤 <b>Owner:</b> {ownerName}

⏳ Requires admin confirmation"
        };
    }

    public string GetAdminCatAdoptionNotification(Languages language, string catName, string ownerName, string? adoptionComment = null, string? actionByUserName = null, string? actionByUserTelegram = null)
    {
        var baseMessage = language switch
        {
            Languages.ru => $@"🎉 <b>Случилось укотовление!</b>

<b>Кошка (кот):</b> {catName}
<b>Владелец:</b> {ownerName}

✅ Статус изменен на 'Укотовление'",

            Languages.en => $@"🎉 <b>Cat Found a New Home!</b>

<b>Cat:</b> {catName}
<b>Owner:</b> {ownerName}

✅ Status changed to 'Adopted'",

            _ => $@"🎉 <b>Cat Found a New Home!</b>

<b>Cat:</b> {catName}
<b>Owner:</b> {ownerName}

✅ Status changed to 'Adopted'"
        };

        // Add action by user information if provided
        if (!string.IsNullOrWhiteSpace(actionByUserName))
        {
            var actionSection = language switch
            {
                Languages.ru => $"\n🧑‍💼 <b>инициатор_ка:</b> {actionByUserName}",
                Languages.en => $"\n🧑‍💼 <b>Action performed by:</b> {actionByUserName}",
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
                Languages.ru => $"\n💬 <b>Комментарий:</b> {adoptionComment}",
                Languages.en => $"\n💬 <b>Comment:</b> {adoptionComment}",
                _ => $"\n💬 <b>Comment:</b> {adoptionComment}"
            };
            baseMessage += commentSection;
        }

        return baseMessage;
    }

    public string GetCatRegisteredToEventMessage(Languages language, string catName, string eventName, DateTime eventDate, string eventLocation)
    {
        return language switch
        {
            Languages.ru => $"🎉 Отличные новости! Ваша кошка (кот) '{catName}' успешно зарегистрирована на мероприятие '{eventName}' {eventDate:yyyy-MM-dd} в {eventLocation}.",
            Languages.en => $"🎉 Great news! Your cat '{catName}' has been successfully registered for the event '{eventName}' on {eventDate:yyyy-MM-dd} at {eventLocation}.",
            _ => $"🎉 Great news! Your cat '{catName}' has been successfully registered for the event '{eventName}' on {eventDate:yyyy-MM-dd} at {eventLocation}."
        };
    }

    public string GetCatRemovedFromEventMessage(Languages language, string catName, string eventName, DateTime eventDate, string eventLocation)
    {
        return language switch
        {
            Languages.ru => $"❗️ Ваша кошка (кот) '{catName}' была удалена из мероприятия '{eventName}' {eventDate:yyyy-MM-dd} в {eventLocation}.",
            Languages.en => $"❗️ Your cat '{catName}' has been removed from the event '{eventName}' on {eventDate:yyyy-MM-dd} at {eventLocation}.",
            _ => $"❗️ Your cat '{catName}' has been removed from the event '{eventName}' on {eventDate:yyyy-MM-dd} at {eventLocation}."
        };
    }

    public string GetAdminCatRegisteredToEventNotification(Languages language, string catName, string ownerName, string catRecordId, string eventName, string eventRecordId, string? actionByUserName = null, string? actionByUserTelegram = null)
    {
        var baseMessage = language switch
        {
            Languages.ru => $"\uD83C\uDF89 <b>Кошка (кот) зарегистрирована на мероприятие</b>\n\n<b>Кошка (кот):</b> {catName}\n<b>Владелец:</b> {ownerName}\n<b>CatRecordId:</b> {catRecordId}\n<b>Мероприятие:</b> {eventName}\n<b>EventRecordId:</b> {eventRecordId}",
            Languages.en => $"\uD83C\uDF89 <b>Cat registered to event</b>\n\n<b>Cat:</b> {catName}\n<b>Owner:</b> {ownerName}\n<b>CatRecordId:</b> {catRecordId}\n<b>Event:</b> {eventName}\n<b>EventRecordId:</b> {eventRecordId}",
            _ => $"\uD83C\uDF89 <b>Cat registered to event</b>\n\n<b>Cat:</b> {catName}\n<b>Owner:</b> {ownerName}\n<b>CatRecordId:</b> {catRecordId}\n<b>Event:</b> {eventName}\n<b>EventRecordId:</b> {eventRecordId}"
        };
        if (!string.IsNullOrWhiteSpace(actionByUserName))
        {
            var actionSection = language switch
            {
                Languages.ru => $"\n🧑‍💼 <b>инициатор_ка:</b> {actionByUserName}",
                Languages.en => $"\n🧑‍💼 <b>Action performed by:</b> {actionByUserName}",
                _ => $"\n🧑‍💼 <b>Action performed by:</b> {actionByUserName}"
            };
            if (!string.IsNullOrWhiteSpace(actionByUserTelegram))
            {
                actionSection += $" ({actionByUserTelegram})";
            }
            baseMessage += actionSection;
        }
        return baseMessage;
    }

    public string GetAdminCatRemovedFromEventNotification(Languages language, string catName, string ownerName, string catRecordId, string eventName, string eventRecordId, string? actionByUserName = null, string? actionByUserTelegram = null)
    {
        var baseMessage = language switch
        {
            Languages.ru => $"❗️ <b>Кошка (кот) удалена из мероприятия</b>\n\n<b>Кошка (кот):</b> {catName}\n<b>Владелец:</b> {ownerName}\n<b>CatRecordId:</b> {catRecordId}\n<b>Мероприятие:</b> {eventName}\n<b>EventRecordId:</b> {eventRecordId}",
            Languages.en => $"❗️ <b>Cat removed from event</b>\n\n<b>Cat:</b> {catName}\n<b>Owner:</b> {ownerName}\n<b>CatRecordId:</b> {catRecordId}\n<b>Event:</b> {eventName}\n<b>EventRecordId:</b> {eventRecordId}",
            _ => $"❗️ <b>Cat removed from event</b>\n\n<b>Cat:</b> {catName}\n<b>Owner:</b> {ownerName}\n<b>CatRecordId:</b> {catRecordId}\n<b>Event:</b> {eventName}\n<b>EventRecordId:</b> {eventRecordId}"
        };
        if (!string.IsNullOrWhiteSpace(actionByUserName))
        {
            var actionSection = language switch
            {
                Languages.ru => $"\n🧑‍💼 <b>инициатор_ка:</b> {actionByUserName}",
                Languages.en => $"\n🧑‍💼 <b>Action performed by:</b> {actionByUserName}",
                _ => $"\n🧑‍💼 <b>Action performed by:</b> {actionByUserName}"
            };
            if (!string.IsNullOrWhiteSpace(actionByUserTelegram))
            {
                actionSection += $" ({actionByUserTelegram})";
            }
            baseMessage += actionSection;
        }
        return baseMessage;
    }

    private string GetVolunteerStatusText(Languages language) => language switch
    {
        Languages.ru => "как волонтер",
        Languages.en => "as a volunteer",
        _ => "as a volunteer"
    };

    private string GetCatOwnerStatusText(Languages language) => language switch
    {
        Languages.ru => "как владелец кошки (кота)",
        Languages.en => "as a cat owner",
        _ => "as a cat owner"
    };
}
