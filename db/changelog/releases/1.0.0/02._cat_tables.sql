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
    cat_id serial PRIMARY KEY,
    notion_cat_id text UNIQUE,
    name text NOT NULL,
    address text,
    geo_location text,
    created_time timestamp NOT NULL default (now() at time zone 'utc'),
    last_edited_time timestamp NOT NULL default (now() at time zone 'utc'),
    notion_page_url text,
    cat_sex_id integer NOT NULL,
    cat_collar_id integer,
    cat_house_space_id integer,
    in_date date default NOW()::date,    --can be null or '' in notion
    out_date date,
    neutered_date date,
    responsible_volunteer_id integer, --notion name is 'deliverer'
    comment text
);

comment on table ctb.cat is 'Кошка/кот';

comment on column ctb.cat.cat_id  is 'ID кошки в бд';
comment on column ctb.cat.notion_cat_id  is 'ID в Notion';
comment on column ctb.cat.name is 'Имя/описание';
comment on column ctb.cat.address is 'Адрес';
comment on column ctb.cat.geo_location is 'Геолокация';
comment on column ctb.cat.created_time is 'Дата создания';
comment on column ctb.cat.last_edited_time is 'Дата последнего изменения';
comment on column ctb.cat.notion_page_url is 'Линк на страницу в Notion';
comment on column ctb.cat.cat_sex_id is 'Пол';
comment on column ctb.cat.cat_collar_id is 'Ошейник';
comment on column ctb.cat.cat_house_space_id is 'ID комнаты в котодоме';
comment on column ctb.cat.in_date is 'Дата прибытия кошки';
comment on column ctb.cat.out_date is 'Дата отъезда кошки';
comment on column ctb.cat.neutered_date is 'Дата стерилизации кошки';
comment on column ctb.cat.responsible_volunteer_id is 'ID волонтёра (в notion - deliverer)';
comment on column ctb.cat.comment is 'Текст примечания';

-- cat_image_url table
create table ctb.cat_image_url (
    cat_image_url_id serial PRIMARY KEY,
    cat_id integer not null,
    name text,
    url text not null,
    type text not null,

    foreign key (cat_id) references ctb.cat(cat_id)
);

create index ix_cat_image_url_cat on ctb.cat_image_url (cat_id);

comment on table ctb.cat_image_url is 'Ссылка на картинки для кошек/котов';

comment on column ctb.cat_image_url.cat_image_url_id is 'ID';
comment on column ctb.cat_image_url.cat_id is 'ID кошки/кота';
comment on column ctb.cat_image_url.name is 'Имя/описание';
comment on column ctb.cat_image_url.url is 'Ссылка на картинку';
comment on column ctb.cat_image_url.type is 'Тип картинки';

-- color table 
create table ctb.color (
    color_id serial PRIMARY KEY,
    name text NOT NULL UNIQUE,
    rgb_code varchar (15) NOT NULL UNIQUE, 
    hex_code varchar (7) NOT NULL UNIQUE
);

comment on table ctb.color is 'Словарь цветов (для ошейников, отметок и проч)';
comment on column ctb.color.color_id is 'ID цвета в базе (thx cap)';
comment on column ctb.color.name is 'Hазвание кириллицей';
comment on column ctb.color.rgb_code is 'Запись в формате "(255, 255, 255)"';
comment on column ctb.color.hex_code is 'Запись в формате "#000000"';

-- cat_sex table
create table ctb.cat_sex (
    cat_sex_id serial PRIMARY KEY,    
    name text NOT NULL UNIQUE,    
    color_id integer,
    
    FOREIGN KEY (color_id) references ctb.color(color_id)
);

comment on table ctb.cat_sex is 'Словарь: пол';
comment on column ctb.cat_sex.cat_sex_id is 'ID пола';
comment on column ctb.cat_sex.name is 'Пол: название (м/ж)';
comment on column ctb.cat_sex.color_id is 'ID цвета';

-- cat_collar table
create table ctb.cat_collar (
    cat_collar_id serial PRIMARY KEY,    
    name text NOT NULL UNIQUE,    
    color_id integer NOT NULL,
    
    FOREIGN KEY (color_id) references ctb.color(color_id)
);

comment on table ctb.cat_collar is 'Cловарь: ошейник';
comment on column ctb.cat_collar.cat_collar_id is 'ID ошейника';
comment on column ctb.cat_collar.name is 'Название ошейника (обычно по его цвету)';
comment on column ctb.cat_collar.color_id is 'ID цвета';

-- cat_house_space table
create table ctb.cat_house_space (
    cat_house_space_id serial PRIMARY KEY,    
    name text NOT NULL UNIQUE,
    short_name varchar(10),
    color_id integer NOT NULL,
    
    FOREIGN KEY (color_id) references ctb.color(color_id)
);

comment on table ctb.cat_house_space is 'Словарь: котоквартира';
comment on column ctb.cat_house_space.cat_house_space_id is 'ID комнаты';
comment on column ctb.cat_house_space.name is 'название комнаты';
comment on column ctb.cat_house_space.short_name is 'сокращение "Комната1"-->"К1"';
comment on column ctb.cat_house_space.color_id is 'ID цвета';

-- cat_tag table
create table ctb.cat_tag (
    cat_tag_id serial PRIMARY KEY,    
    name text NOT NULL UNIQUE,    
    color_id integer,
    
    FOREIGN KEY (color_id) references ctb.color(color_id)
);

comment on table ctb.cat_tag is 'словарь: теги кошек';
comment on column ctb.cat_tag.cat_tag_id is 'ID тега';
comment on column ctb.cat_tag.name is 'текст тега ("медуход", "аборт" итп)';
comment on column ctb.cat_tag.color_id is 'ID цвета';

-- cat2cat_tag table
create table ctb.cat2cat_tag (
    cat2cat_tag_id serial PRIMARY KEY,    
    cat_id integer NOT NULL,
    cat_tag_id integer NOT NULL,
    
    FOREIGN KEY (cat_id) references ctb.cat(cat_id),
    FOREIGN KEY (cat_tag_id) references ctb.cat_tag(cat_tag_id),
    UNIQUE (cat_id, cat_tag_id)
);

comment on table ctb.cat2cat_tag is 'словарь для связи кошек и тегов';
comment on column ctb.cat2cat_tag.cat2cat_tag_id is 'ID соотношения';
comment on column ctb.cat2cat_tag.cat_id is 'ID кошки';
comment on column ctb.cat2cat_tag.cat_tag_id is 'ID тега';

--volunteer table 
create table ctb.volunteer (
    volunteer_id serial PRIMARY KEY,
    notion_volunteer_id text UNIQUE,
    name text NOT NULL,
    notion_user_id integer UNIQUE,
    telegram_account text,
    address text,
    location text
);

comment on table ctb.volunteer is 'справочник волонтёров';
comment on column ctb.volunteer.volunteer_id is 'ID волонтёра';
comment on column ctb.volunteer.notion_volunteer_id is 'ID записи о волонтёре в Notion';
comment on column ctb.volunteer.name is 'имя/ник волонтёра';
comment on column ctb.volunteer.notion_user_id is 'ID аккаунта волонтёра в Notion';
comment on column ctb.volunteer.telegram_account is 'Telegram username волонтёра';
comment on column ctb.volunteer.address is 'физический (обычно неполный) адрес проживания волонтёра';
comment on column ctb.volunteer.location is 'координаты в пригодном для экспорта формате';

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
