drop table if exists frgn.keyword_restriction;
drop table if exists frgn.restriction;
drop table if exists frgn.chat_keyword;
drop table if exists frgn.keyword;
drop table if exists frgn.chat;

-- tables 

create table frgn.chat(
	chat_id serial PRIMARY KEY,
	chat_url text unique not null
);

comment on table frgn.chat is 'Перечень ссылок на telegram-чаты, на которые подписывается бот';
comment on column frgn.chat.chat_id is 'ID чата';
comment on column frgn.chat.chat_url is 'URL чата';

create table frgn.keyword(
	keyword_id serial PRIMARY KEY,
	keyword_text text unique not null
);

comment on table frgn.keyword is 'Ключевые слова, по которым бот производит поиск';
comment on column frgn.keyword.keyword_id is 'ID слова';
comment on column frgn.keyword.keyword_text is 'Слово';

create table frgn.chat_keyword(
	chat_keyword_id serial PRIMARY KEY,
	chat_id int not null,
	keyword_id int not null,
	
	unique (chat_id, keyword_id)
);

comment on table frgn.chat_keyword is 'Таблица для связи чатов и ключевых слов';
comment on column frgn.chat_keyword.chat_keyword_id is 'ID связки';
comment on column frgn.chat_keyword.chat_id is 'ID чата';
comment on column frgn.chat_keyword.keyword_id is 'ID кючевого слова';

create table frgn.restriction(
	restriction_id serial PRIMARY KEY,
	restriction_text text unique not null
);

comment on table frgn.restriction is 'Справочник ограничений, присваеваемых ключевым словам';
comment on column frgn.restriction.restriction_id is 'ID ограничения';
comment on column frgn.restriction.restriction_text is 'Текст ограничения, например, "Искать по этому слову только в чатах о животных"';

create table frgn.keyword_restriction(
	keyword_restriction_id serial PRIMARY KEY,
	keyword_id int not null,
	restriction_id int not null,
	
	unique (keyword_id, restriction_id)
);

comment on table frgn.keyword_restriction is 'Таблица для связи ключевых слов и ограничений';
comment on column frgn.keyword_restriction.keyword_restriction_id is 'ID связки';
comment on column frgn.keyword_restriction.keyword_id is 'ID ключевого слова';
comment on column frgn.keyword_restriction.restriction_id is 'ID ограничения';

-- foreign keys
--chat_keyword
alter table frgn.chat_keyword
	add FOREIGN KEY (chat_id) 
		references frgn.chat(chat_id);
alter table frgn.chat_keyword
	add FOREIGN KEY (keyword_id) 
		references frgn.keyword(keyword_id);
		
--keyword_restriction
alter table frgn.keyword_restriction
	add FOREIGN KEY (keyword_id) 
		references frgn.keyword(keyword_id);
alter table frgn.keyword_restriction
	add FOREIGN KEY (restriction_id) 
		references frgn.restriction(restriction_id);
