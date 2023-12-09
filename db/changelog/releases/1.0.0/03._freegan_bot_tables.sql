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

comment on table freegan.chat is '�������� ������ �� telegram-����, �� ������� ������������� ���';
comment on column freegan.chat.chat_id is 'ID ����';
comment on column freegan.chat.chat_url is 'URL ����';

create table freegan.keyword(
	keyword_id serial PRIMARY KEY,
	keyword_text text unique not null
);

comment on table freegan.keyword is '�������� �����, �� ������� ��� ���������� �����';
comment on column freegan.keyword.keyword_id is 'ID �����';
comment on column freegan.keyword.keyword_text is '�����';

create table freegan.chat_keyword(
	chat_keyword_id serial PRIMARY KEY,
	chat_id int not null,
	keyword_id int not null,
	
	unique (chat_id, keyword_id)
);

comment on table freegan.chat_keyword is '������� ��� ����� ����� � �������� ����';
comment on column freegan.chat_keyword.chat_keyword_id is 'ID ������';
comment on column freegan.chat_keyword.chat_id is 'ID ����';
comment on column freegan.chat_keyword.keyword_id is 'ID �������� �����';

create table freegan.restriction(
	restriction_id serial PRIMARY KEY,
	restriction_text text unique not null
);

comment on table freegan.restriction is '���������� �����������, ������������� �������� ������';
comment on column freegan.restriction.restriction_id is 'ID �����������';
comment on column freegan.restriction.restriction_text is '����� �����������, ��������, "������ �� ����� ����� ������ � ����� � ��������"';

create table freegan.keyword_restriction(
	keyword_restriction_id serial PRIMARY KEY,
	keyword_id int not null,
	restriction_id int not null,
	
	unique (keyword_id, restriction_id)
);

comment on table freegan.keyword_restriction is '������� ��� ����� �������� ���� � �����������';
comment on column freegan.keyword_restriction.keyword_restriction_id is 'ID ������';
comment on column freegan.keyword_restriction.keyword_id is 'ID ��������� �����';
comment on column freegan.keyword_restriction.restriction_id is 'ID �����������';

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