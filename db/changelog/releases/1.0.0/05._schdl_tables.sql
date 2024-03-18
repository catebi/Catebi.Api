drop table if exists schdl.chat_users;
drop table if exists schdl.schedule_day_users;
drop table if exists schdl.schedule;

create table schdl.chat_users(
	user_name text primary key,
	chat_id text,
	chat_users_id bigint, 
	language text,
	selected_date date,
	selected_user text
);

comment on table schdl.chat_users is 'надо узнать';
comment on column schdl.chat_users.user_name is 'username волонтёра в телеграмме';
comment on column schdl.chat_users.chat_id is 'id чата в телеграмме ';
comment on column schdl.chat_users.chat_users_id is 'id записи';
comment on column schdl.chat_users.language is 'предпочитемый язык волонтёра?';
comment on column schdl.chat_users.selected_date is 'выбранная дата';
comment on column schdl.chat_users.selected_user is '?выбранный дежурный?';


create table schdl.schedule(
  schedule_id serial primary key,
  created_by text,
  created_date date,
  schedule_date date,
  day_name text,
  short_date text
);

comment on table schdl.schedule is 'расписание ...';
comment on column schdl.schedule.schedule_id is 'id записи';
comment on column schdl.schedule.created_by is 'кем создана';
comment on column schdl.schedule.created_date is 'дата создания';
comment on column schdl.schedule.schedule_date is 'дата';
comment on column schdl.schedule.day_name is 'день недели';
comment on column schdl.schedule.short_date is 'дата вида "дд.мм"';

create table schdl.schedule_day_users(
  users text,
  schedule_day_id bigint not null
);

comment on table schdl.schedule_day_users is 'могу только предполагать';
comment on column schdl.schedule_day_users.users is 'tg username';
comment on column schdl.schedule_day_users.schedule_day_id is 'id';

-- FK in schedule_day_users

alter table schdl.schedule_day_users
    add FOREIGN KEY (schedule_day_id)
        references schdl.schedule (schedule_id);
		