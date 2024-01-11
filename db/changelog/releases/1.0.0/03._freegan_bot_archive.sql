drop table if exists frgn.message;

-- tables 

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