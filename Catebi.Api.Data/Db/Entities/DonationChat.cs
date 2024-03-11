using System;
using System.Collections.Generic;

namespace Catebi.Api.Data.Db.Entities;

/// <summary>
/// Чаты барахолок для фригана
/// </summary>
public partial class DonationChat
{
    /// <summary>
    /// id
    /// </summary>
    public int DonationChatId { get; set; }

    /// <summary>
    /// Ссылка на чат
    /// </summary>
    public string ChatUrl { get; set; } = null!;

    /// <summary>
    /// Признак актуальности
    /// </summary>
    public bool IsActual { get; set; }

    /// <summary>
    /// Признак подключения Мисс Марпл к чату
    /// </summary>
    public bool IsConnected { get; set; }

    /// <summary>
    /// Дата создания барахолки
    /// </summary>
    public DateTime? CreatedDate { get; set; }
}
