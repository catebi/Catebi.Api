drop table if exists frgn.message;
drop table if exists frgn.donation_message_reaction;

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