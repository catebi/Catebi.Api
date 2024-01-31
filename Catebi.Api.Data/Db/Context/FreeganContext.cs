using System;
using System.Collections.Generic;
using Catebi.Api.Data.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catebi.Api.Data.Db.Context;

public partial class FreeganContext : DbContext
{
    public FreeganContext(DbContextOptions<FreeganContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Message> Message { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
