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

    public virtual DbSet<Cat> Cat { get; set; }

    public virtual DbSet<CatCatTag> CatCatTag { get; set; }

    public virtual DbSet<CatCollar> CatCollar { get; set; }

    public virtual DbSet<CatHouseSpace> CatHouseSpace { get; set; }

    public virtual DbSet<CatImageUrl> CatImageUrl { get; set; }

    public virtual DbSet<CatSex> CatSex { get; set; }

    public virtual DbSet<CatTag> CatTag { get; set; }

    public virtual DbSet<Color> Color { get; set; }

    public virtual DbSet<Permission> Permission { get; set; }

    public virtual DbSet<Role> Role { get; set; }

    public virtual DbSet<RolePermission> RolePermission { get; set; }

    public virtual DbSet<Volunteer> Volunteer { get; set; }

    public virtual DbSet<VolunteerRole> VolunteerRole { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cat>(entity =>
        {
            entity.HasKey(e => e.CatId).HasName("cat_pkey");

            entity.ToTable("cat", "ctb", tb => tb.HasComment("Кошка/кот"));

            entity.HasIndex(e => e.NotionCatId, "cat_notion_cat_id_key").IsUnique();

            entity.Property(e => e.CatId)
                .HasDefaultValueSql("nextval('cat_cat_id_seq'::regclass)")
                .HasComment("Id кошки в бд")
                .HasColumnName("cat_id");
            entity.Property(e => e.Address)
                .HasComment("Адрес (где нашли кошку)")
                .HasColumnName("address");
            entity.Property(e => e.CatCollarId)
                .HasComment("Ошейник")
                .HasColumnName("cat_collar_id");
            entity.Property(e => e.CatHouseSpaceId)
                .HasComment("Id комнаты в котодоме")
                .HasColumnName("cat_house_space_id");
            entity.Property(e => e.CatSexId)
                .HasComment("Пол")
                .HasColumnName("cat_sex_id");
            entity.Property(e => e.ChangedDate)
                .HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)")
                .HasComment("Дата последнего изменения")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("changed_date");
            entity.Property(e => e.Comment)
                .HasComment("Текст примечания")
                .HasColumnName("comment");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)")
                .HasComment("Дата создания")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.GeoLocation)
                .HasComment("Геолокация (координаты по адресу)")
                .HasColumnName("geo_location");
            entity.Property(e => e.InDate)
                .HasComment("Дата прибытия кошки")
                .HasColumnName("in_date");
            entity.Property(e => e.Name)
                .HasComment("Имя/описание")
                .HasColumnName("name");
            entity.Property(e => e.NeuteredDate)
                .HasComment("Дата стерилизации кошки")
                .HasColumnName("neutered_date");
            entity.Property(e => e.NotionCatId)
                .HasComment("Id в Notion")
                .HasColumnName("notion_cat_id");
            entity.Property(e => e.NotionPageUrl)
                .HasComment("Ссылка на страницу в Notion")
                .HasColumnName("notion_page_url");
            entity.Property(e => e.OutDate)
                .HasComment("Дата отъезда кошки")
                .HasColumnName("out_date");
            entity.Property(e => e.ResponsibleVolunteerId)
                .HasComment("Id волонтёра, ответственного за кошку (в notion - deliverer)")
                .HasColumnName("responsible_volunteer_id");

            entity.HasOne(d => d.CatCollar).WithMany(p => p.Cat)
                .HasForeignKey(d => d.CatCollarId)
                .HasConstraintName("cat_cat_collar_id_fkey");

            entity.HasOne(d => d.CatHouseSpace).WithMany(p => p.Cat)
                .HasForeignKey(d => d.CatHouseSpaceId)
                .HasConstraintName("cat_cat_house_space_id_fkey");

            entity.HasOne(d => d.CatSex).WithMany(p => p.Cat)
                .HasForeignKey(d => d.CatSexId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cat_cat_sex_id_fkey");

            entity.HasOne(d => d.ResponsibleVolunteer).WithMany(p => p.Cat)
                .HasForeignKey(d => d.ResponsibleVolunteerId)
                .HasConstraintName("cat_responsible_volunteer_id_fkey");
        });

        modelBuilder.Entity<CatCatTag>(entity =>
        {
            entity.HasKey(e => e.CatCatTagId).HasName("cat_cat_tag_pkey");

            entity.ToTable("cat_cat_tag", "ctb", tb => tb.HasComment("Словарь для связи кошек и тегов"));

            entity.HasIndex(e => new { e.CatId, e.CatTagId }, "cat_cat_tag_cat_id_cat_tag_id_key").IsUnique();

            entity.Property(e => e.CatCatTagId)
                .HasDefaultValueSql("nextval('cat_cat_tag_cat_cat_tag_id_seq'::regclass)")
                .HasComment("Id соотношения")
                .HasColumnName("cat_cat_tag_id");
            entity.Property(e => e.CatId)
                .HasComment("Id кошки")
                .HasColumnName("cat_id");
            entity.Property(e => e.CatTagId)
                .HasComment("Id тега")
                .HasColumnName("cat_tag_id");

            entity.HasOne(d => d.Cat).WithMany(p => p.CatCatTag)
                .HasForeignKey(d => d.CatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cat_cat_tag_cat_id_fkey");

            entity.HasOne(d => d.CatTag).WithMany(p => p.CatCatTag)
                .HasForeignKey(d => d.CatTagId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cat_cat_tag_cat_tag_id_fkey");
        });

        modelBuilder.Entity<CatCollar>(entity =>
        {
            entity.HasKey(e => e.CatCollarId).HasName("cat_collar_pkey");

            entity.ToTable("cat_collar", "ctb", tb => tb.HasComment("Словарь: ошейник"));

            entity.HasIndex(e => e.Name, "cat_collar_name_key").IsUnique();

            entity.Property(e => e.CatCollarId)
                .HasDefaultValueSql("nextval('cat_collar_cat_collar_id_seq'::regclass)")
                .HasComment("Id ошейника")
                .HasColumnName("cat_collar_id");
            entity.Property(e => e.ColorId)
                .HasComment("Id цвета")
                .HasColumnName("color_id");
            entity.Property(e => e.Name)
                .HasComment("Название ошейника (обычно по его цвету)")
                .HasColumnName("name");

            entity.HasOne(d => d.Color).WithMany(p => p.CatCollar)
                .HasForeignKey(d => d.ColorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cat_collar_color_id_fkey");
        });

        modelBuilder.Entity<CatHouseSpace>(entity =>
        {
            entity.HasKey(e => e.CatHouseSpaceId).HasName("cat_house_space_pkey");

            entity.ToTable("cat_house_space", "ctb", tb => tb.HasComment("Словарь: котоквартира"));

            entity.HasIndex(e => e.Name, "cat_house_space_name_key").IsUnique();

            entity.Property(e => e.CatHouseSpaceId)
                .HasDefaultValueSql("nextval('cat_house_space_cat_house_space_id_seq'::regclass)")
                .HasComment("Id комнаты")
                .HasColumnName("cat_house_space_id");
            entity.Property(e => e.ColorId)
                .HasComment("Id цвета")
                .HasColumnName("color_id");
            entity.Property(e => e.Name)
                .HasComment("Название комнаты")
                .HasColumnName("name");

            entity.HasOne(d => d.Color).WithMany(p => p.CatHouseSpace)
                .HasForeignKey(d => d.ColorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cat_house_space_color_id_fkey");
        });

        modelBuilder.Entity<CatImageUrl>(entity =>
        {
            entity.HasKey(e => e.CatImageUrlId).HasName("cat_image_url_pkey");

            entity.ToTable("cat_image_url", "ctb", tb => tb.HasComment("Ссылка на картинки для кошек/котов"));

            entity.HasIndex(e => e.CatId, "ix_cat_image_url_cat");

            entity.Property(e => e.CatImageUrlId)
                .HasDefaultValueSql("nextval('cat_image_url_cat_image_url_id_seq'::regclass)")
                .HasComment("Id ссылки в бд")
                .HasColumnName("cat_image_url_id");
            entity.Property(e => e.CatId)
                .HasComment("Id кошки/кота")
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

        modelBuilder.Entity<CatSex>(entity =>
        {
            entity.HasKey(e => e.CatSexId).HasName("cat_sex_pkey");

            entity.ToTable("cat_sex", "ctb", tb => tb.HasComment("Словарь: пол"));

            entity.HasIndex(e => e.Name, "cat_sex_name_key").IsUnique();

            entity.Property(e => e.CatSexId)
                .HasDefaultValueSql("nextval('cat_sex_cat_sex_id_seq'::regclass)")
                .HasComment("Id пола")
                .HasColumnName("cat_sex_id");
            entity.Property(e => e.ColorId)
                .HasComment("Id цвета")
                .HasColumnName("color_id");
            entity.Property(e => e.Name)
                .HasComment("Пол: название (м/ж)")
                .HasColumnName("name");

            entity.HasOne(d => d.Color).WithMany(p => p.CatSex)
                .HasForeignKey(d => d.ColorId)
                .HasConstraintName("cat_sex_color_id_fkey");
        });

        modelBuilder.Entity<CatTag>(entity =>
        {
            entity.HasKey(e => e.CatTagId).HasName("cat_tag_pkey");

            entity.ToTable("cat_tag", "ctb", tb => tb.HasComment("Словарь: теги кошек"));

            entity.HasIndex(e => e.Name, "cat_tag_name_key").IsUnique();

            entity.Property(e => e.CatTagId)
                .HasDefaultValueSql("nextval('cat_tag_cat_tag_id_seq'::regclass)")
                .HasComment("Id тега")
                .HasColumnName("cat_tag_id");
            entity.Property(e => e.ColorId)
                .HasComment("Id цвета")
                .HasColumnName("color_id");
            entity.Property(e => e.Name)
                .HasComment("Текст тега (\"медуход\", \"аборт\" итп)")
                .HasColumnName("name");

            entity.HasOne(d => d.Color).WithMany(p => p.CatTag)
                .HasForeignKey(d => d.ColorId)
                .HasConstraintName("cat_tag_color_id_fkey");
        });

        modelBuilder.Entity<Color>(entity =>
        {
            entity.HasKey(e => e.ColorId).HasName("color_pkey");

            entity.ToTable("color", "ctb", tb => tb.HasComment("Словарь цветов (для ошейников, отметок и проч)"));

            entity.HasIndex(e => e.HexCode, "color_hex_code_key").IsUnique();

            entity.HasIndex(e => e.Name, "color_name_key").IsUnique();

            entity.HasIndex(e => e.RgbCode, "color_rgb_code_key").IsUnique();

            entity.Property(e => e.ColorId)
                .HasDefaultValueSql("nextval('color_color_id_seq'::regclass)")
                .HasComment("Id цвета в базе")
                .HasColumnName("color_id");
            entity.Property(e => e.HexCode)
                .HasMaxLength(7)
                .HasComment("Запись в формате \"#000000\"")
                .HasColumnName("hex_code");
            entity.Property(e => e.Name)
                .HasComment("Название кириллицей")
                .HasColumnName("name");
            entity.Property(e => e.RgbCode)
                .HasMaxLength(15)
                .HasComment("Запись в формате \"(255, 255, 255)\"")
                .HasColumnName("rgb_code");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("permission_pkey");

            entity.ToTable("permission", "ctb", tb => tb.HasComment("Список прав (разрешений/доступов)"));

            entity.HasIndex(e => e.Name, "permission_name_key").IsUnique();

            entity.Property(e => e.PermissionId)
                .HasDefaultValueSql("nextval('permission_permission_id_seq'::regclass)")
                .HasComment("ID права")
                .HasColumnName("permission_id");
            entity.Property(e => e.Name)
                .HasComment("Наименование права")
                .HasColumnName("name");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("role_pkey");

            entity.ToTable("role", "ctb", tb => tb.HasComment("Список ролей волонтёров"));

            entity.HasIndex(e => e.Name, "role_name_key").IsUnique();

            entity.Property(e => e.RoleId)
                .HasDefaultValueSql("nextval('role_role_id_seq'::regclass)")
                .HasComment("ID роли")
                .HasColumnName("role_id");
            entity.Property(e => e.Name)
                .HasComment("Наименование роли")
                .HasColumnName("name");
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => e.RolePermissionId).HasName("role_permission_pkey");

            entity.ToTable("role_permission", "ctb", tb => tb.HasComment("Таблица связи роли с разрешениями"));

            entity.HasIndex(e => new { e.RoleId, e.PermissionId }, "role_permission_role_id_permission_id_key").IsUnique();

            entity.Property(e => e.RolePermissionId)
                .HasDefaultValueSql("nextval('role_permission_role_permission_id_seq'::regclass)")
                .HasComment("ID соотношения")
                .HasColumnName("role_permission_id");
            entity.Property(e => e.PermissionId)
                .HasComment("ID разрешения")
                .HasColumnName("permission_id");
            entity.Property(e => e.RoleId)
                .HasComment("ID роли")
                .HasColumnName("role_id");

            entity.HasOne(d => d.Permission).WithMany(p => p.RolePermission)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("role_permission_permission_id_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermission)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("role_permission_role_id_fkey");
        });

        modelBuilder.Entity<Volunteer>(entity =>
        {
            entity.HasKey(e => e.VolunteerId).HasName("volunteer_pkey");

            entity.ToTable("volunteer", "ctb", tb => tb.HasComment("Справочник волонтёров"));

            entity.HasIndex(e => e.NotionUserId, "volunteer_notion_user_id_key").IsUnique();

            entity.HasIndex(e => e.NotionVolunteerId, "volunteer_notion_volunteer_id_key").IsUnique();

            entity.Property(e => e.VolunteerId)
                .HasDefaultValueSql("nextval('volunteer_volunteer_id_seq'::regclass)")
                .HasComment("Id волонтёра")
                .HasColumnName("volunteer_id");
            entity.Property(e => e.Address)
                .HasComment("Физический (обычно неполный) адрес проживания волонтёра")
                .HasColumnName("address");
            entity.Property(e => e.ChangedDate)
                .HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)")
                .HasComment("Дата последнего изменения")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("changed_date");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)")
                .HasComment("Дата создания")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.GeoLocation)
                .HasComment("Координаты в пригодном для экспорта формате")
                .HasColumnName("geo_location");
            entity.Property(e => e.Name)
                .HasComment("Имя/ник волонтёра")
                .HasColumnName("name");
            entity.Property(e => e.NotionUserId)
                .HasComment("Id аккаунта волонтёра в Notion")
                .HasColumnName("notion_user_id");
            entity.Property(e => e.NotionVolunteerId)
                .HasComment("Id записи о волонтёре в Notion")
                .HasColumnName("notion_volunteer_id");
            entity.Property(e => e.TelegramAccount)
                .HasComment("Telegram username волонтёра")
                .HasColumnName("telegram_account");
        });

        modelBuilder.Entity<VolunteerRole>(entity =>
        {
            entity.HasKey(e => e.VolunteerRoleId).HasName("volunteer_role_pkey");

            entity.ToTable("volunteer_role", "ctb", tb => tb.HasComment("Таблица связи \"волонтёр-роль\""));

            entity.Property(e => e.VolunteerRoleId)
                .HasDefaultValueSql("nextval('volunteer_role_volunteer_role_id_seq'::regclass)")
                .HasComment("ID соотношения")
                .HasColumnName("volunteer_role_id");
            entity.Property(e => e.RoleId)
                .HasComment("ID роли")
                .HasColumnName("role_id");
            entity.Property(e => e.VolunteerId)
                .HasComment("ID волонтёра")
                .HasColumnName("volunteer_id");

            entity.HasOne(d => d.Role).WithMany(p => p.VolunteerRole)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("volunteer_role_role_id_fkey");

            entity.HasOne(d => d.Volunteer).WithMany(p => p.VolunteerRole)
                .HasForeignKey(d => d.VolunteerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("volunteer_role_volunteer_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
