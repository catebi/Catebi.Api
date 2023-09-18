drop table if exists cat_image_url;
drop table if exists cat;

-- cat table
create table cat (
    cat_id serial,
    notion_cat_id text,
    name text not null,
    address text,
    geo_location text,
    created_time timestamp not null default (now() at time zone 'utc'),
    last_edited_time timestamp not null default (now() at time zone 'utc'),
    notion_page_url text,

    constraint pk_cat primary key (cat_id)
);

comment on table cat is 'Кошка/кот';

comment on column cat.cat_id  is 'Ид.';
comment on column cat.notion_cat_id  is 'Ид. в Notion';
comment on column cat.name is 'Имя/описание';
comment on column cat.address is 'Адрес';
comment on column cat.geo_location is 'Геолокация';
comment on column cat.created_time is 'Дата создания';
comment on column cat.last_edited_time is 'Дата последнего изменения';
comment on column cat.notion_page_url is 'Линк на страницу в Notion';

-- cat_image_url table
create table cat_image_url (
    cat_image_url_id serial,
    cat_id int not null,
    name text,
    url text not null,
    type text not null,

    constraint pk_cat_image_url primary key (cat_image_url_id),
    foreign key (cat_id) references cat(cat_id)
);

create index ix_cat_image_url_cat on cat_image_url (cat_id);

comment on table cat_image_url is 'Ссылка на картинки для кошек/котов';

comment on column cat_image_url.cat_image_url_id is 'Ид.';
comment on column cat_image_url.cat_id is 'Ид. кошки/кота';
comment on column cat_image_url.name is 'Имя/описание';
comment on column cat_image_url.url is 'Ссылка на картинку';
comment on column cat_image_url.type is 'Тип картинки';