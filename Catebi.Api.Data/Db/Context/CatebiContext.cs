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

    public virtual DbSet<ClinicVisit> ClinicVisit { get; set; }

    public virtual DbSet<ClinicVisitFile> ClinicVisitFile { get; set; }

    public virtual DbSet<Color> Color { get; set; }

    public virtual DbSet<DonationChat> DonationChat { get; set; }

    public virtual DbSet<DonationMessageReaction> DonationMessageReaction { get; set; }

    public virtual DbSet<Group> Group { get; set; }

    public virtual DbSet<GroupExcludedKeyword> GroupExcludedKeyword { get; set; }

    public virtual DbSet<GroupIncludedKeyword> GroupIncludedKeyword { get; set; }

    public virtual DbSet<Keyword> Keyword { get; set; }

    public virtual DbSet<MedSchedule> MedSchedule { get; set; }

    public virtual DbSet<Message> Message { get; set; }

    public virtual DbSet<Permission> Permission { get; set; }

    public virtual DbSet<Prescription> Prescription { get; set; }

    public virtual DbSet<Role> Role { get; set; }

    public virtual DbSet<RolePermission> RolePermission { get; set; }

    public virtual DbSet<TimeUnit> TimeUnit { get; set; }

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

        modelBuilder.Entity<ClinicVisit>(entity =>
        {
            entity.HasKey(e => e.ClinicVisitId).HasName("clinic_visit_pkey");

            entity.ToTable("clinic_visit", "ctb", tb => tb.HasComment("Посещение врача/ветеринарной клиники"));

            entity.Property(e => e.ClinicVisitId)
                .HasDefaultValueSql("nextval('clinic_visit_clinic_visit_id_seq'::regclass)")
                .HasComment("ID посещения")
                .HasColumnName("clinic_visit_id");
            entity.Property(e => e.CatId)
                .HasComment("ID кошки")
                .HasColumnName("cat_id");
            entity.Property(e => e.ClinicName)
                .HasComment("Название клиники")
                .HasColumnName("clinic_name");
            entity.Property(e => e.CompanionVolunteerId).HasColumnName("companion_volunteer_id");
            entity.Property(e => e.DoctorName)
                .HasComment("Имя врача")
                .HasColumnName("doctor_name");
            entity.Property(e => e.VisitDate)
                .HasComment("Дата визита")
                .HasColumnName("visit_date");

            entity.HasOne(d => d.Cat).WithMany(p => p.ClinicVisit)
                .HasForeignKey(d => d.CatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("clinic_visit_cat_id_fkey");

            entity.HasOne(d => d.CompanionVolunteer).WithMany(p => p.ClinicVisit)
                .HasForeignKey(d => d.CompanionVolunteerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("clinic_visit_companion_volunteer_id_fkey");
        });

        modelBuilder.Entity<ClinicVisitFile>(entity =>
        {
            entity.HasKey(e => e.ClinicVisitFileId).HasName("clinic_visit_file_pkey");

            entity.ToTable("clinic_visit_file", "ctb", tb => tb.HasComment("Файлы посещений"));

            entity.Property(e => e.ClinicVisitFileId)
                .HasDefaultValueSql("nextval('clinic_visit_file_clinic_visit_file_id_seq'::regclass)")
                .HasComment("ID файла")
                .HasColumnName("clinic_visit_file_id");
            entity.Property(e => e.ClinicVisitId)
                .HasComment("ID посещения")
                .HasColumnName("clinic_visit_id");
            entity.Property(e => e.FileName)
                .HasComment("Имя файла")
                .HasColumnName("file_name");
            entity.Property(e => e.FileUrl)
                .HasComment("URL файла")
                .HasColumnName("file_url");

            entity.HasOne(d => d.ClinicVisit).WithMany(p => p.ClinicVisitFile)
                .HasForeignKey(d => d.ClinicVisitId)
                .HasConstraintName("clinic_visit_file_clinic_visit_id_fkey");
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

        modelBuilder.Entity<MedSchedule>(entity =>
        {
            entity.HasKey(e => e.MedScheduleRecordId).HasName("med_schedule_pkey");

            entity.ToTable("med_schedule", "ctb", tb => tb.HasComment("График медицинского ухода"));

            entity.Property(e => e.MedScheduleRecordId)
                .HasDefaultValueSql("nextval('med_schedule_med_schedule_record_id_seq'::regclass)")
                .HasComment("ID записи в графике(журнале) мед. ухода")
                .HasColumnName("med_schedule_record_id");
            entity.Property(e => e.CatId)
                .HasComment("ID кошки")
                .HasColumnName("cat_id");
            entity.Property(e => e.Done)
                .HasComment("Процедура выполнена")
                .HasColumnName("done");
            entity.Property(e => e.PrescriptionId)
                .HasComment("ID назначения")
                .HasColumnName("prescription_id");
            entity.Property(e => e.ProcedureTime)
                .HasComment("Дата и время назначенной процедуры")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("procedure_time");
            entity.Property(e => e.VolunteerId)
                .HasComment("Волонтёр-исполнитель")
                .HasColumnName("volunteer_id");

            entity.HasOne(d => d.Cat).WithMany(p => p.MedSchedule)
                .HasForeignKey(d => d.CatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("med_schedule_cat_id_fkey");

            entity.HasOne(d => d.Prescription).WithMany(p => p.MedSchedule)
                .HasForeignKey(d => d.PrescriptionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("med_schedule_prescription_id_fkey");

            entity.HasOne(d => d.Volunteer).WithMany(p => p.MedSchedule)
                .HasForeignKey(d => d.VolunteerId)
                .HasConstraintName("med_schedule_volunteer_id_fkey");
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

        modelBuilder.Entity<Prescription>(entity =>
        {
            entity.HasKey(e => e.PrescriptionId).HasName("prescription_pkey");

            entity.ToTable("prescription", "ctb", tb => tb.HasComment("Назначения по медицинскому уходу"));

            entity.Property(e => e.PrescriptionId)
                .HasDefaultValueSql("nextval('prescription_prescription_id_seq'::regclass)")
                .HasComment("ID назначения")
                .HasColumnName("prescription_id");
            entity.Property(e => e.ClinicVisitId)
                .HasComment("ID визита к врачу")
                .HasColumnName("clinic_visit_id");
            entity.Property(e => e.Duration)
                .HasComment("Длительность лечения в днях")
                .HasColumnName("duration");
            entity.Property(e => e.OneTimeProcedure)
                .HasComment("Процедура одноразовая")
                .HasColumnName("one_time_procedure");
            entity.Property(e => e.PeriodicityUnitId)
                .HasComment("Периодичность, ед. изм.")
                .HasColumnName("periodicity_unit_id");
            entity.Property(e => e.PeriodicityValue)
                .HasComment("Периодичность, значение")
                .HasColumnName("periodicity_value");
            entity.Property(e => e.PrescriptionText)
                .HasComment("Текст назначения")
                .HasColumnName("prescription_text");
            entity.Property(e => e.StartDate)
                .HasComment("Дата начала лечения")
                .HasColumnName("start_date");

            entity.HasOne(d => d.ClinicVisit).WithMany(p => p.Prescription)
                .HasForeignKey(d => d.ClinicVisitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("prescription_clinic_visit_id_fkey");

            entity.HasOne(d => d.PeriodicityUnit).WithMany(p => p.Prescription)
                .HasForeignKey(d => d.PeriodicityUnitId)
                .HasConstraintName("prescription_periodicity_unit_id_fkey");
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

        modelBuilder.Entity<TimeUnit>(entity =>
        {
            entity.HasKey(e => e.TimeUnitId).HasName("time_unit_pkey");

            entity.ToTable("time_unit", "ctb", tb => tb.HasComment("Единицы измерения времени"));

            entity.HasIndex(e => e.Name, "time_unit_name_key").IsUnique();

            entity.Property(e => e.TimeUnitId)
                .HasDefaultValueSql("nextval('time_unit_time_unit_id_seq'::regclass)")
                .HasComment("ID единицы измерения")
                .HasColumnName("time_unit_id");
            entity.Property(e => e.Name)
                .HasComment("Наименование единицы измерения")
                .HasColumnName("name");
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
