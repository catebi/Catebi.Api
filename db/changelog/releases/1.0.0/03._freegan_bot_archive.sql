drop table if exists frgn.message;

-- tables 

create table frgn.message(
    message_id serial primary key,
	original_text text not null,
	lemmatized_text text not null,
	chat_link text not null,
	accepted bool not null,
);

comment on table frgn. is 'Архив сообщений, прошедших через бот для разметки';
comment on column frgn. is 'ID записи';
comment on column frgn. is 'Исходный текст сообщения';
comment on column frgn. is 'Текст сообщения после лемматизации';
comment on column frgn. is 'Ссылка на чат, откуда бот сообщение взял';
comment on column frgn. is 'Принято ли сообщение по текущему набору правил';