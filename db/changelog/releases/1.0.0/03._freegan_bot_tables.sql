drop table if exists frgn.message;
drop table if exists frgn.reaction_statistics;

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

-- freegan_reaction_statistics
create table frgn.reaction_statistics(
    reaction_statistics_id serial primary key,
    tg_message_id int not null unique,
    message_content text not null,
    count_of_likes int not null,
    count_of_dislikes int not null
);

comment on table frgn.reaction_statistics is 'Статистика реакций на сообщения';
comment on column frgn.reaction_statistics.reaction_statistics_id is 'ID';
comment on column frgn.reaction_statistics.tg_message_id is 'ID сообщения в telegram';
comment on column frgn.reaction_statistics.message_content is 'Текст сообщения';
comment on column frgn.reaction_statistics.count_of_likes is 'Количество реакций 👍';
comment on column frgn.reaction_statistics.count_of_dislikes is 'Количество реакций 👎';