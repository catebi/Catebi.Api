using System;
using System.Collections.Generic;
using Catebi.Map.WebApi.Db.Catebi.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catebi.Map.WebApi.Db.Catebi.Context;

public partial class CatebiContext : DbContext
{
    public CatebiContext(DbContextOptions<CatebiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cat> Cat { get; set; }

    public virtual DbSet<CatImageUrl> CatImageUrl { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cat>(entity =>
        {
            entity.HasKey(e => e.CatId).HasName("pk_cat");

            entity.ToTable("cat", "ctb", tb => tb.HasComment("Кошка/кот"));

            entity.Property(e => e.CatId)
                .HasDefaultValueSql("nextval('cat_cat_id_seq'::regclass)")
                .HasComment("Ид.")
                .HasColumnName("cat_id");
            entity.Property(e => e.Address)
                .HasComment("Адрес")
                .HasColumnName("address");
            entity.Property(e => e.CreatedTime)
                .HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)")
                .HasComment("Дата создания")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_time");
            entity.Property(e => e.GeoLocation)
                .HasComment("Геолокация")
                .HasColumnName("geo_location");
            entity.Property(e => e.LastEditedTime)
                .HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)")
                .HasComment("Дата последнего изменения")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_edited_time");
            entity.Property(e => e.Name)
                .HasComment("Имя/описание")
                .HasColumnName("name");
            entity.Property(e => e.NotionCatId)
                .HasComment("Ид. в Notion")
                .HasColumnName("notion_cat_id");
            entity.Property(e => e.NotionPageUrl)
                .HasComment("Линк на страницу в Notion")
                .HasColumnName("notion_page_url");
        });

        modelBuilder.Entity<CatImageUrl>(entity =>
        {
            entity.HasKey(e => e.CatImageUrlId).HasName("pk_cat_image_url");

            entity.ToTable("cat_image_url", "ctb", tb => tb.HasComment("Ссылка на картинки для кошек/котов"));

            entity.HasIndex(e => e.CatId, "ix_cat_image_url_cat");

            entity.Property(e => e.CatImageUrlId)
                .HasDefaultValueSql("nextval('cat_image_url_cat_image_url_id_seq'::regclass)")
                .HasComment("Ид.")
                .HasColumnName("cat_image_url_id");
            entity.Property(e => e.CatId)
                .HasComment("Ид. кошки/кота")
                .HasColumnName("cat_id");
            entity.Property(e => e.Name)
                .HasComment("Имя/описание")
                .HasColumnName("name");
            entity.Property(e => e.Type)
                .HasComment("Тип картинки")
                .HasColumnName("type");
            entity.Property(e => e.Url)
                .HasComment("Ссылка на картинку")
                .HasColumnName("url");

            entity.HasOne(d => d.Cat).WithMany(p => p.CatImageUrl)
                .HasForeignKey(d => d.CatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cat_image_url_cat_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
