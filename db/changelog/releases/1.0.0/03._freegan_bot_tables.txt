drop table if exists freegan.keyword_restriction;
drop table if exists freegan.restriction;
drop table if exists freegan.chat_keyword;
drop table if exists freegan.keyword;
drop table if exists freegan.chat;

-- tables 

create table freegan.chat(
	chat_id serial PRIMARY KEY,
	chat_url text unique not null
);

comment on table freegan.chat is 'Перечень ссылок на telegram-чаты, на которые подписывается бот';
comment on column freegan.chat.chat_id is 'ID чата';
comment on column freegan.chat.chat_url is 'URL чата';

create table freegan.keyword(
	keyword_id serial PRIMARY KEY,
	keyword_text text unique not null
);

comment on table freegan.keyword is 'Ключевые слова, по которым бот производит поиск';
comment on column freegan.keyword.keyword_id is 'ID слова';
comment on column freegan.keyword.keyword_text is 'Слово';

create table freegan.chat_keyword(
	chat_keyword_id serial PRIMARY KEY,
	chat_id int not null,
	keyword_id int not null,
	
	unique (chat_id, keyword_id)
);

comment on table freegan.chat_keyword is 'Таблица для связи чатов и ключевых слов';
comment on column freegan.chat_keyword.chat_keyword_id is 'ID связки';
comment on column freegan.chat_keyword.chat_id is 'ID чата';
comment on column freegan.chat_keyword.keyword_id is 'ID кючевого слова';

create table freegan.restriction(
	restriction_id serial PRIMARY KEY,
	restriction_text text unique not null
);

comment on table freegan.restriction is 'Справочник ограничений, присваеваемых ключевым словам';
comment on column freegan.restriction.restriction_id is 'ID ограничения';
comment on column freegan.restriction.restriction_text is 'Текст ограничения, например, "Искать по этому слову только в чатах о животных"';

create table freegan.keyword_restriction(
	keyword_restriction_id serial PRIMARY KEY,
	keyword_id int not null,
	restriction_id int not null,
	
	unique (keyword_id, restriction_id)
);

comment on table freegan.keyword_restriction is 'Таблица для связи ключевых слов и ограничений';
comment on column freegan.keyword_restriction.keyword_restriction_id is 'ID связки';
comment on column freegan.keyword_restriction.keyword_id is 'ID ключевого слова';
comment on column freegan.keyword_restriction.restriction_id is 'ID ограничения';

-- foreign keys
--chat_keyword
alter table freegan.chat_keyword
	add FOREIGN KEY (chat_id) 
		references freegan.chat(chat_id);
alter table freegan.chat_keyword
	add FOREIGN KEY (keyword_id) 
		references freegan.keyword(keyword_id);
		
--keyword_restriction
alter table freegan.keyword_restriction
	add FOREIGN KEY (keyword_id) 
		references freegan.keyword(keyword_id);
alter table freegan.keyword_restriction
	add FOREIGN KEY (restriction_id) 
		references freegan.restriction(restriction_id);