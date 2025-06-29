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
Платеж за вашего кота {catName} подтвержден. Поздравляем!

Теперь вы можете добавить своего кота в Кэтбук или забронировать для него мероприятие.",

            Languages.en => $@"Hello {ownerName} 👋
Payment for your cat {catName} has been confirmed. Congrats!

You can now access to push your cat to the Catbook or to book event for them.",

            _ => $@"Hello {ownerName} 👋
Payment for your cat {catName} has been confirmed. Congrats!

You can now access to push your cat to the Catbook or to book event for them."
        };
    }

    public string GetCatPhotoAddedMessage(Languages language, string ownerName, string catName)
    {
        return language switch
        {
            Languages.ru => $"Привет, {ownerName} 👋\nФото для вашего кота {catName} было успешно добавлено!",
            Languages.en => $"Hello {ownerName} 👋\nPhoto for your cat {catName} has been successfully added!",
            _ => $"Hello {ownerName} 👋\nPhoto for your cat {catName} has been successfully added!"
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

🐱 Теперь вы можете зарегистрировать своих котов на это мероприятие! Не упустите возможность!",

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
            Languages.ru => "как владелец кота",
            Languages.en => "as a cat owner",
            _ => "as a cat owner"
        };
    }
}
