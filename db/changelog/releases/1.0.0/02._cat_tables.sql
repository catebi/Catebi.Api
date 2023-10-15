drop table if exists ctb.cat_image_url;
drop table if exists ctb.cat2cat_tag;
drop table if exists ctb.cat;
drop table if exists ctb.cat_house_space;
drop table if exists ctb.cat_tag;
drop table if exists ctb.cat_collar;
drop table if exists ctb.cat_sex;
drop table if exists ctb.volunteer;
drop table if exists ctb.color;


-- cat table
create table ctb.cat (
    cat_id serial primary key,
    notion_cat_id text unique,
    name text not null,
    address text,
    geo_location text,
    notion_page_url text,
    cat_sex_id integer not null,
    cat_collar_id integer,
    cat_house_space_id integer,
    in_date date default now()::date,
    out_date date,
    neutered_date date,
    responsible_volunteer_id integer,
    comment text,
    created_date timestamp not null default (now() at time zone 'utc'),
    changed_date timestamp not null default (now() at time zone 'utc')
);

comment on table ctb.cat is 'Кошка/кот';

comment on column ctb.cat.cat_id  is 'Id кошки в бд';
comment on column ctb.cat.notion_cat_id  is 'Id в Notion';
comment on column ctb.cat.name is 'Имя/описание';
comment on column ctb.cat.address is 'Адрес';
comment on column ctb.cat.geo_location is 'Геолокация';
comment on column ctb.cat.notion_page_url is 'Линк на страницу в Notion';
comment on column ctb.cat.cat_sex_id is 'Пол';
comment on column ctb.cat.cat_collar_id is 'Ошейник';
comment on column ctb.cat.cat_house_space_id is 'Id комнаты в котодоме';
comment on column ctb.cat.in_date is 'Дата прибытия кошки';
comment on column ctb.cat.out_date is 'Дата отъезда кошки';
comment on column ctb.cat.neutered_date is 'Дата стерилизации кошки';
comment on column ctb.cat.responsible_volunteer_id is 'Id волонтёра (в notion - deliverer)';
comment on column ctb.cat.comment is 'Текст примечания';
comment on column ctb.cat.created_date is 'Дата создания';
comment on column ctb.cat.changed_date is 'Дата последнего изменения';

-- cat_image_url table
create table ctb.cat_image_url (
    cat_image_url_id serial primary key,
    cat_id integer not null,
    name text,
    url text not null,
    type text not null,

    foreign key (cat_id) references ctb.cat(cat_id)
);

create index ix_cat_image_url_cat on ctb.cat_image_url (cat_id);

comment on table ctb.cat_image_url is 'Ссылка на картинки для кошек/котов';

comment on column ctb.cat_image_url.cat_image_url_id is 'Id';
comment on column ctb.cat_image_url.cat_id is 'Id кошки/кота';
comment on column ctb.cat_image_url.name is 'Имя/описание';
comment on column ctb.cat_image_url.url is 'Ссылка на картинку';
comment on column ctb.cat_image_url.type is 'Тип картинки';

-- color table
create table ctb.color (
    color_id serial primary key,
    name text not null unique,
    rgb_code varchar (15) not null unique,
    hex_code varchar (7) not null unique
);

comment on table ctb.color is 'Словарь цветов (для ошейников, отметок и проч)';

comment on column ctb.color.color_id is 'Id цвета в базе (thx cap)';
comment on column ctb.color.name is 'Название кириллицей';
comment on column ctb.color.rgb_code is 'Запись в формате "(255, 255, 255)"';
comment on column ctb.color.hex_code is 'Запись в формате "#000000"';

-- cat_sex table
create table ctb.cat_sex (
    cat_sex_id serial primary key,
    name text not null unique,
    color_id integer,

    foreign key (color_id) references ctb.color(color_id)
);

comment on table ctb.cat_sex is 'Словарь: пол';

comment on column ctb.cat_sex.cat_sex_id is 'Id пола';
comment on column ctb.cat_sex.name is 'Пол: название (м/ж)';
comment on column ctb.cat_sex.color_id is 'Id цвета';

-- cat_collar table
create table ctb.cat_collar (
    cat_collar_id serial primary key,
    name text not null unique,
    color_id integer not null,

    foreign key (color_id) references ctb.color(color_id)
);

comment on table ctb.cat_collar is 'Словарь: ошейник';

comment on column ctb.cat_collar.cat_collar_id is 'Id ошейника';
comment on column ctb.cat_collar.name is 'Название ошейника (обычно по его цвету)';
comment on column ctb.cat_collar.color_id is 'Id цвета';

-- cat_house_space table
create table ctb.cat_house_space (
    cat_house_space_id serial PRIMARY KEY,    
    name text NOT NULL UNIQUE,
    color_id integer NOT NULL,
    
    FOREIGN KEY (color_id) references ctb.color(color_id)
);

comment on table ctb.cat_house_space is 'Словарь: котоквартира';
comment on column ctb.cat_house_space.cat_house_space_id is 'ID комнаты';
comment on column ctb.cat_house_space.name is 'Название комнаты';
comment on column ctb.cat_house_space.color_id is 'ID цвета';

-- cat_tag table
create table ctb.cat_tag (
    cat_tag_id serial primary key,
    name text not null unique,
    color_id integer,

    foreign key (color_id) references ctb.color(color_id)
);

comment on table ctb.cat_tag is 'Словарь: теги кошек';

comment on column ctb.cat_tag.cat_tag_id is 'Id тега';
comment on column ctb.cat_tag.name is 'Текст тега ("медуход", "аборт" итп)';
comment on column ctb.cat_tag.color_id is 'Id цвета';

-- cat2cat_tag table
create table ctb.cat2cat_tag (
    cat2cat_tag_id serial primary key,
    cat_id integer not null,
    cat_tag_id integer not null,

    foreign key (cat_id) references ctb.cat(cat_id),
    foreign key (cat_tag_id) references ctb.cat_tag(cat_tag_id),
    unique (cat_id, cat_tag_id)
);

comment on table ctb.cat2cat_tag is 'Словарь для связи кошек и тегов';

comment on column ctb.cat2cat_tag.cat2cat_tag_id is 'Id соотношения';
comment on column ctb.cat2cat_tag.cat_id is 'Id кошки';
comment on column ctb.cat2cat_tag.cat_tag_id is 'Id тега';

--volunteer table
create table ctb.volunteer (
    volunteer_id serial primary key,
    notion_volunteer_id text unique,
    name text not null,
    notion_user_id integer unique,
    telegram_account text,
    address text,
    geo_location text,
    created_date timestamp not null default (now() at time zone 'utc'),
    changed_date timestamp not null default (now() at time zone 'utc')
);

comment on table ctb.volunteer is 'Справочник волонтёров';

comment on column ctb.volunteer.volunteer_id is 'Id волонтёра';
comment on column ctb.volunteer.notion_volunteer_id is 'Id записи о волонтёре в Notion';
comment on column ctb.volunteer.name is 'Имя/ник волонтёра';
comment on column ctb.volunteer.notion_user_id is 'Id аккаунта волонтёра в Notion';
comment on column ctb.volunteer.telegram_account is 'Telegram username волонтёра';
comment on column ctb.volunteer.address is 'Физический (обычно неполный) адрес проживания волонтёра';
comment on column ctb.volunteer.geo_location is 'Координаты в пригодном для экспорта формате';
comment on column ctb.volunteer.created_date is 'Дата создания';
comment on column ctb.volunteer.changed_date is 'Дата последнего изменения';

-- additional constraints to main table - ctb.cat
alter table ctb.cat 
    add foreign key (cat_sex_id) 
    references ctb.cat_sex(cat_sex_id);
alter table ctb.cat 
    add foreign key (responsible_volunteer_id) 
    references ctb.volunteer(volunteer_id);
alter table ctb.cat 
    add foreign key (cat_collar_id) 
    references ctb.cat_collar(cat_collar_id);
alter table ctb.cat 
    add foreign key (cat_house_space_id) 
    references ctb.cat_house_space(cat_house_space_id);
