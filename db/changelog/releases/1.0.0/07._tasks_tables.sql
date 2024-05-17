-- drop
drop table if exists tasks.work_task_responsible;
drop table if exists tasks.work_task_reminder;
drop table if exists tasks.work_task;
drop table if exists tasks.work_topic;
drop table if exists tasks.work_task_status;

-- work_topic
create table tasks.work_topic(
    work_topic_id serial primary key,
    telegram_thread_id int not null unique,
    name text not null,
    description text,
    is_main bool not null,
    is_actual bool not null,
    created timestamp,
    created_by_id int not null
);

comment on table tasks.work_topic is 'Таблица для хранения инфоромации о топиках в чате Catebi';
comment on column tasks.work_topic.work_topic_id is 'ID записи';
comment on column tasks.work_topic.telegram_thread_id is 'ID топика в tg';
comment on column tasks.work_topic.name is 'Название топика';
comment on column tasks.work_topic.description is 'описание топика';
comment on column tasks.work_topic.is_main is 'топик для напоминаний об активных задачах';
comment on column tasks.work_topic.is_actual is 'Флаг актуальности';
comment on column tasks.work_topic.created is 'Дата создания';
comment on column tasks.work_topic.created_by_id is 'ID волонтёра-автора топика';

-- tasks_task
create table tasks.work_task(
    work_task_id serial primary key,
    work_topic_id integer not null,
    description text not null,
    status_id int not null,
    created_date timestamp,
    changed_date timestamp,
    created_by_id int not null,
    changed_by_id int not null
);

comment on table tasks.work_task is 'Таблица для хранения задач в чате Catebi';
comment on column tasks.work_task.work_task_id is 'ID записи';
comment on column tasks.work_task.work_topic_id is 'ID топика, в котором создана задача';
comment on column tasks.work_task.description is 'описание задачи';
comment on column tasks.work_task.status_id is 'ID статуса задачи';
comment on column tasks.work_task.created_date is 'Дата создания';
comment on column tasks.work_task.changed_date is 'Дата изменения';
comment on column tasks.work_task.created_by_id is 'ID волонтёра-автора задачи';
comment on column tasks.work_task.changed_by_id is 'ID волонтёра, изменившего задачу';

-- work_task_status
create table tasks.work_task_status(
    work_task_status_id serial primary key,
    code int not null,
    name text not null
);

comment on table tasks.work_task_status is 'Справочная таблица, содержащая возможные статусы задачек';
comment on column tasks.work_task_status.work_task_status_id is 'ID записи';
comment on column tasks.work_task_status.code is 'Код статуса';
comment on column tasks.work_task_status.name is 'Статусы задачек';

create table tasks.work_task_responsible(
    work_task_responsible_id serial primary key,
    work_task_id int not null,
    volunteer_id int not null
);

comment on table tasks.work_task_responsible is 'Информация о волонтёре, ответственном за задачу';
comment on column tasks.work_task_responsible.work_task_responsible_id is 'ID записи';
comment on column tasks.work_task_responsible.work_task_id is 'ID задачи';
comment on column tasks.work_task_responsible.volunteer_id is 'ID волонтёра, ответственного за задачу';

create table tasks.work_task_reminder(
    work_task_reminder_id serial primary key,
    work_task_id int not null,
    reminder_date timestamp not null,
    created timestamp not null,
    created_by_id int not null
);

comment on table tasks.work_task_reminder is 'Информация для оповещений по задачам';
comment on column tasks.work_task_reminder.work_task_reminder_id is 'ID записи';
comment on column tasks.work_task_reminder.work_task_id is 'ID задачи';
comment on column tasks.work_task_reminder.reminder_date is 'дата оповещения';
comment on column tasks.work_task_reminder.created is 'дата создания';
comment on column tasks.work_task_reminder.created_by_id is 'ID волонтёра-создателя задачи';

-- FK in work_task table
alter table tasks.work_task
    add FOREIGN KEY (work_topic_id)
            references tasks.work_topic(work_topic_id);
alter table tasks.work_task
    add FOREIGN KEY (status_id)
            references tasks.work_task_status(work_task_status_id);
alter table tasks.work_task
    add FOREIGN KEY (changed_by_id)
            references ctb.volunteer(volunteer_id);
alter table tasks.work_task
    add FOREIGN KEY (created_by_id)
            references ctb.volunteer(volunteer_id);

-- FK in work_task_responsible table
alter table tasks.work_task_responsible
    add FOREIGN KEY (work_task_id)
        references tasks.work_task(work_task_id);
alter table tasks.work_task_responsible
    add FOREIGN KEY (volunteer_id)
        references ctb.volunteer(volunteer_id);

-- FK in work_task_reminder table
alter table tasks.work_task_reminder
    add FOREIGN KEY (work_task_id)
        references tasks.work_task(work_task_id);
alter table tasks.work_task_reminder
    add FOREIGN KEY (created_by_id)
        references ctb.volunteer(volunteer_id);