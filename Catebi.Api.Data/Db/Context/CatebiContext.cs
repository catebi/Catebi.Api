using System;
using System.Collections.Generic;
using Catebi.Api.Data.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catebi.Api.Data.Db.Context;

public partial class CatebiContext : DbContext
{
    public CatebiContext(DbContextOptions<CatebiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DonationChat> DonationChat { get; set; }

    public virtual DbSet<DonationMessageReaction> DonationMessageReaction { get; set; }

    public virtual DbSet<Message> Message { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DonationChat>(entity =>
        {
            entity.HasKey(e => e.DonationChatId).HasName("donation_chat_pkey");

            entity.ToTable("donation_chat", "frgn", tb => tb.HasComment("Чаты барахолок для фригана"));

            entity.Property(e => e.DonationChatId)
                .HasComment("id")
                .HasColumnName("donation_chat_id");
            entity.Property(e => e.ChatUrl)
                .HasComment("Ссылка на чат")
                .HasColumnName("chat_url");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("timezone('utc'::text, now())")
                .HasComment("Дата создания барахолки")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.IsActual)
                .HasComment("Признак актуальности")
                .HasColumnName("is_actual");
            entity.Property(e => e.IsConnected)
                .HasComment("Признак подключения Мисс Марпл к чату")
                .HasColumnName("is_connected");
        });

        modelBuilder.Entity<DonationMessageReaction>(entity =>
        {
            entity.HasKey(e => e.DonationMessageReactionId).HasName("donation_message_reaction_pkey");

            entity.ToTable("donation_message_reaction", "frgn", tb => tb.HasComment("Таблица для сбора статисти реакций на сообщения"));

            entity.HasIndex(e => e.MessageId, "donation_message_reaction_message_id_key").IsUnique();

            entity.Property(e => e.DonationMessageReactionId)
                .HasComment("ID")
                .HasColumnName("donation_message_reaction_id");
            entity.Property(e => e.Content)
                .HasComment("Текст сообщения")
                .HasColumnName("content");
            entity.Property(e => e.DislikeCount)
                .HasComment("Количество реакций 👎")
                .HasColumnName("dislike_count");
            entity.Property(e => e.LikeCount)
                .HasComment("Количество реакций 👍")
                .HasColumnName("like_count");
            entity.Property(e => e.MessageId)
                .HasComment("ID сообщения (в чате после фильтрации)")
                .HasColumnName("message_id");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("message_pkey");

            entity.ToTable("message", "frgn", tb => tb.HasComment("Архив сообщений, прошедших через бот для разметки"));

            entity.Property(e => e.MessageId)
                .HasComment("ID записи")
                .HasColumnName("message_id");
            entity.Property(e => e.Accepted)
                .HasComment("Принято ли сообщение по текущему набору правил")
                .HasColumnName("accepted");
            entity.Property(e => e.ChatLink)
                .HasComment("Ссылка на чат, откуда бот сообщение взял")
                .HasColumnName("chat_link");
            entity.Property(e => e.LemmatizedText)
                .HasComment("Текст сообщения после лемматизации")
                .HasColumnName("lemmatized_text");
            entity.Property(e => e.OriginalText)
                .HasComment("Исходный текст сообщения")
                .HasColumnName("original_text");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
