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

    public virtual DbSet<DonationChat> DonationChat { get; set; }

    public virtual DbSet<DonationMessageReaction> DonationMessageReaction { get; set; }

    public virtual DbSet<FileStorage> FileStorage { get; set; }

    public virtual DbSet<Group> Group { get; set; }

    public virtual DbSet<GroupExcludedKeyword> GroupExcludedKeyword { get; set; }

    public virtual DbSet<GroupIncludedKeyword> GroupIncludedKeyword { get; set; }

    public virtual DbSet<Keyword> Keyword { get; set; }

    public virtual DbSet<Message> Message { get; set; }

    public virtual DbSet<Role> Role { get; set; }

    public virtual DbSet<RoleClaim> RoleClaim { get; set; }

    public virtual DbSet<User> User { get; set; }

    public virtual DbSet<UserClaim> UserClaim { get; set; }

    public virtual DbSet<UserLogin> UserLogin { get; set; }

    public virtual DbSet<UserToken> UserToken { get; set; }

    public virtual DbSet<Volunteer> Volunteer { get; set; }

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

        modelBuilder.Entity<DonationChat>(entity =>
        {
            entity.HasKey(e => e.DonationChatId).HasName("donation_chat_pkey");

            entity.ToTable("donation_chat", "frgn", tb => tb.HasComment("Чаты барахолок для фригана"));

            entity.HasIndex(e => e.ChatUrl, "unq_donation_chat_chat_url").IsUnique();

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

        modelBuilder.Entity<FileStorage>(entity =>
        {
            entity.HasKey(e => e.FileStorageId).HasName("file_storage_pkey");

            entity.ToTable("file_storage", "ctb", tb => tb.HasComment("Stores file data with metadata including original filename, content type, and upload timestamp"));

            entity.Property(e => e.FileStorageId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasComment("Unique identifier for the file, automatically generated using gen_random_uuid()")
                .HasColumnName("file_storage_id");
            entity.Property(e => e.Content)
                .HasComment("Binary data of the file stored as BYTEA")
                .HasColumnName("content");
            entity.Property(e => e.ContentType)
                .HasComment("MIME type of the file (e.g., image/jpeg, application/pdf)")
                .HasColumnName("content_type");
            entity.Property(e => e.Created)
                .HasDefaultValueSql("timezone('utc'::text, now())")
                .HasComment("Timestamp when the file was uploaded, automatically set to current time")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created");
            entity.Property(e => e.FileName)
                .HasComment("Original filename of the uploaded file")
                .HasColumnName("file_name");
            entity.Property(e => e.Size)
                .HasComment("Size of the file in bytes")
                .HasColumnName("size");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("group_pkey");

            entity.ToTable("group", "frgn", tb => tb.HasComment("Таблица групп слов"));

            entity.Property(e => e.GroupId)
                .HasComment("ID группы")
                .HasColumnName("group_id");
            entity.Property(e => e.IsActual)
                .HasComment("Признак актуальности группы")
                .HasColumnName("is_actual");
            entity.Property(e => e.Name)
                .HasComment("Название группы")
                .HasColumnName("name");
        });

        modelBuilder.Entity<GroupExcludedKeyword>(entity =>
        {
            entity.HasKey(e => e.ExcludedKeywordId).HasName("group_excluded_keyword_pkey");

            entity.ToTable("group_excluded_keyword", "frgn", tb => tb.HasComment("Таблица исключенных ключевых слов для группы"));

            entity.Property(e => e.ExcludedKeywordId)
                .HasComment("ID исключенного ключевого слова")
                .HasColumnName("excluded_keyword_id");
            entity.Property(e => e.GroupId)
                .HasComment("ID группы")
                .HasColumnName("group_id");
            entity.Property(e => e.Keyword)
                .HasComment("Исключенное ключевое слово")
                .HasColumnName("keyword");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupExcludedKeyword)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("group_excluded_keyword_group_id_fkey");
        });

        modelBuilder.Entity<GroupIncludedKeyword>(entity =>
        {
            entity.HasKey(e => e.IncludedKeywordId).HasName("group_included_keyword_pkey");

            entity.ToTable("group_included_keyword", "frgn", tb => tb.HasComment("Таблица включенных ключевых слов для группы"));

            entity.Property(e => e.IncludedKeywordId)
                .HasComment("ID включенного ключевого слова")
                .HasColumnName("included_keyword_id");
            entity.Property(e => e.GroupId)
                .HasComment("ID группы")
                .HasColumnName("group_id");
            entity.Property(e => e.Keyword)
                .HasComment("Включенное ключевое слово")
                .HasColumnName("keyword");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupIncludedKeyword)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("group_included_keyword_group_id_fkey");
        });

        modelBuilder.Entity<Keyword>(entity =>
        {
            entity.HasKey(e => e.KeywordId).HasName("keyword_pkey");

            entity.ToTable("keyword", "frgn", tb => tb.HasComment("Таблица ключевых слов"));

            entity.Property(e => e.KeywordId)
                .HasComment("ID ключевого слова")
                .HasColumnName("keyword_id");
            entity.Property(e => e.GroupId)
                .HasComment("ID группы, к которой относится ключевое слово")
                .HasColumnName("group_id");
            entity.Property(e => e.Keyword1)
                .HasComment("Ключевое слово")
                .HasColumnName("keyword");

            entity.HasOne(d => d.Group).WithMany(p => p.Keyword)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("keyword_group_id_fkey");
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

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_role");

            entity.ToTable("role", "identity");

            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ConcurrencyStamp).HasColumnName("concurrency_stamp");
            entity.Property(e => e.Name)
                .HasMaxLength(256)
                .HasColumnName("name");
            entity.Property(e => e.NormalizedName)
                .HasMaxLength(256)
                .HasColumnName("normalized_name");
        });

        modelBuilder.Entity<RoleClaim>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_role_claim");

            entity.ToTable("role_claim", "identity");

            entity.HasIndex(e => e.RoleId, "ix_role_claim_role_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClaimType).HasColumnName("claim_type");
            entity.Property(e => e.ClaimValue).HasColumnName("claim_value");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Role).WithMany(p => p.RoleClaim)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("fk_role_claim_role_role_id");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_user");

            entity.ToTable("user", "identity");

            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AccessFailedCount).HasColumnName("access_failed_count");
            entity.Property(e => e.ConcurrencyStamp).HasColumnName("concurrency_stamp");
            entity.Property(e => e.Email)
                .HasMaxLength(256)
                .HasColumnName("email");
            entity.Property(e => e.EmailConfirmed).HasColumnName("email_confirmed");
            entity.Property(e => e.LockoutEnabled).HasColumnName("lockout_enabled");
            entity.Property(e => e.LockoutEnd).HasColumnName("lockout_end");
            entity.Property(e => e.NormalizedEmail)
                .HasMaxLength(256)
                .HasColumnName("normalized_email");
            entity.Property(e => e.NormalizedUserName)
                .HasMaxLength(256)
                .HasColumnName("normalized_user_name");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.PhoneNumber).HasColumnName("phone_number");
            entity.Property(e => e.PhoneNumberConfirmed).HasColumnName("phone_number_confirmed");
            entity.Property(e => e.SecurityStamp).HasColumnName("security_stamp");
            entity.Property(e => e.TwoFactorEnabled).HasColumnName("two_factor_enabled");
            entity.Property(e => e.UserName)
                .HasMaxLength(256)
                .HasColumnName("user_name");

            entity.HasMany(d => d.Role).WithMany(p => p.User)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("fk_user_role_role_role_id"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_user_role_user_user_id"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId").HasName("pk_user_role");
                        j.ToTable("user_role", "identity");
                        j.HasIndex(new[] { "RoleId" }, "ix_user_role_role_id");
                        j.IndexerProperty<string>("UserId").HasColumnName("user_id");
                        j.IndexerProperty<string>("RoleId").HasColumnName("role_id");
                    });
        });

        modelBuilder.Entity<UserClaim>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_user_claim");

            entity.ToTable("user_claim", "identity");

            entity.HasIndex(e => e.UserId, "ix_user_claim_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClaimType).HasColumnName("claim_type");
            entity.Property(e => e.ClaimValue).HasColumnName("claim_value");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserClaim)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_user_claim_user_user_id");
        });

        modelBuilder.Entity<UserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey }).HasName("pk_user_login");

            entity.ToTable("user_login", "identity");

            entity.HasIndex(e => e.UserId, "ix_user_login_user_id");

            entity.Property(e => e.LoginProvider).HasColumnName("login_provider");
            entity.Property(e => e.ProviderKey).HasColumnName("provider_key");
            entity.Property(e => e.ProviderDisplayName).HasColumnName("provider_display_name");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserLogin)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_user_login_user_user_id");
        });

        modelBuilder.Entity<UserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name }).HasName("pk_user_token");

            entity.ToTable("user_token", "identity");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.LoginProvider).HasColumnName("login_provider");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Value).HasColumnName("value");

            entity.HasOne(d => d.User).WithMany(p => p.UserToken)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_user_token_user_user_id");
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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
