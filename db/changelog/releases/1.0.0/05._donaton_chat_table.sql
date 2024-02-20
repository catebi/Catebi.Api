drop table if exists frgn.donation_chat;

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