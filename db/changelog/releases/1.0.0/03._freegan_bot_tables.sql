drop table if exists frgn.message;
drop table if exists frgn.donation_message_reaction;
drop table if exists frgn.donation_chat;
drop table if exists frgn.search_group_included_keyword;
drop table if exists frgn.search_group_excluded_keyword;
drop table if exists frgn.keyword;
drop table if exists frgn.search_group;

-- freegan_bot_archive

create table frgn.message(
    message_id serial primary key,
    original_text text not null,
    lemmatized_text text not null,
    chat_link text not null,
    accepted bool not null
);

comment on table frgn.message is 'Архив сообщений, прошедших через бот для разметки';
comment on column frgn.message.message_id is 'ID записи';
comment on column frgn.message.original_text is 'Исходный текст сообщения';
comment on column frgn.message.lemmatized_text is 'Текст сообщения после лемматизации';
comment on column frgn.message.chat_link is 'Ссылка на чат, откуда бот сообщение взял';
comment on column frgn.message.accepted is 'Принято ли сообщение по текущему набору правил';

-- freegan_donation_message_reaction
create table frgn.donation_message_reaction(
    donation_message_reaction_id serial primary key,
    message_id int not null unique,
    content text not null,
    like_count int not null,
    dislike_count int not null
);

comment on table frgn.donation_message_reaction is 'Таблица для сбора статисти реакций на сообщения';
comment on column frgn.donation_message_reaction.donation_message_reaction_id is 'ID';
comment on column frgn.donation_message_reaction.message_id is 'ID сообщения (в чате после фильтрации)';
comment on column frgn.donation_message_reaction.content is 'Текст сообщения';
comment on column frgn.donation_message_reaction.like_count is 'Количество реакций 👍';
comment on column frgn.donation_message_reaction.dislike_count is 'Количество реакций 👎';

-- freegan_donation_chat
create table frgn.donation_chat(
    donation_chat_id serial primary key,
    chat_url text not null,
    is_actual bool not null,
    is_connected bool not null,
    created_date timestamp default timezone('utc', now())
);

comment on table frgn.donation_chat is 'Чаты барахолок для фригана';
comment on column frgn.donation_chat.donation_chat_id is 'id';
comment on column frgn.donation_chat.chat_url is 'Ссылка на чат';
comment on column frgn.donation_chat.is_actual is 'Признак актуальности';
comment on column frgn.donation_chat.is_connected is 'Признак подключения Мисс Марпл к чату';
comment on column frgn.donation_chat.created_date is 'Дата создания барахолки';

-- Search groups table
create table frgn.search_group (
    group_id serial primary key,
    name text not null,
    is_actual bool not null
);

comment on table frgn.search_group is 'Таблица групп слов';
comment on column frgn.search_group.group_id is 'ID группы';
comment on column frgn.search_group.name is 'Название группы';
comment on column frgn.search_group.is_actual is 'Признак актуальности группы';

-- Keywords table
create table frgn.keyword (
    keyword_id serial primary key,
    group_id int not null,
    keyword text not null
);

comment on table frgn.keyword is 'Таблица ключевых слов';
comment on column frgn.keyword.keyword_id is 'ID ключевого слова';
comment on column frgn.keyword.group_id is 'ID группы, к которой относится ключевое слово';
comment on column frgn.keyword.keyword is 'Ключевое слово';

-- Search group Included Keywords table
create table frgn.search_group_included_keyword (
    included_keyword_id serial primary key,
    group_id int not null,
    keyword text not null
);

comment on table frgn.search_group_included_keyword is 'Таблица включенных ключевых слов для группы';
comment on column frgn.search_group_included_keyword.included_keyword_id is 'ID включенного ключевого слова';
comment on column frgn.search_group_included_keyword.group_id is 'ID группы';
comment on column frgn.search_group_included_keyword.keyword is 'Включенное ключевое слово';

-- Group Excluded Keywords table
create table frgn.search_group_excluded_keyword (
    excluded_keyword_id serial primary key,
    group_id int not null,
    keyword text not null
);

comment on table frgn.search_group_excluded_keyword is 'Таблица исключенных ключевых слов для группы';
comment on column frgn.search_group_excluded_keyword.excluded_keyword_id is 'ID исключенного ключевого слова';
comment on column frgn.search_group_excluded_keyword.group_id is 'ID группы';
comment on column frgn.search_group_excluded_keyword.keyword is 'Исключенное ключевое слово';

-- constraints
alter table frgn.donation_chat add constraint unq_donation_chat_chat_url unique(chat_url);
alter table frgn.keyword 
    add foreign key (group_id) references frgn.search_group(group_id);
alter table frgn.search_group_included_keyword 
    add foreign key (group_id) references frgn.search_group(group_id);
alter table frgn.search_group_excluded_keyword 
    add foreign key (group_id) references frgn.search_group(group_id);
