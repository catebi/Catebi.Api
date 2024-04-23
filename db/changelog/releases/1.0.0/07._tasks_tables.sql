drop table if exists tasks.topic;
drop table if exists tasks.task;
drop table if exists tasks.topic_status;
drop table if exists tasks.task_status;

-- tasks_topic 
create table tasks.topic(
    task_topic_id serial primary key,   
    topic_id int not null unique,
    topic_name text not null,
    topic_description text,
    status text not null,
    is_actual bool not null, 
    created timestamp,
    created_by int not null
);

comment on table tasks.topic is 'Таблица для хранения инфоромации о топиках в чате Catebi';
comment on column tasks.topic.task_topic_id is 'ID записи';
comment on column tasks.topic.topic_id is 'ID топика в tg';
comment on column tasks.topic.topic_name is 'Название топика';
comment on column tasks.topic.topic_description is 'описание топика';
comment on column tasks.topic.status is 'Статус топика';
comment on column tasks.topic.is_actual is 'ID';
comment on column tasks.topic.created is 'Дата создания';
comment on column tasks.topic.created_by is 'ID волонтёра-автора топика';

-- tasks_task
create table tasks.task(
    tasks_task_id serial primary key,
    task_id integer not null unique,
    topic_id integer not null,
    task_description text not null,
    status text not null,
    created timestamp, 
    created_by int not null,
    changed_by int 
);

comment on table tasks.task is 'Таблица для хранения задач в чате Catebi';
comment on column tasks.task.tasks_task_id is 'ID записи';
comment on column tasks.task.task_id is 'ID задачи';
comment on column tasks.task.topic_id is 'ID топика, в котором создана задача';
comment on column tasks.task.task_description is 'описание задачи';
comment on column tasks.task.status is 'Статус задачи';
comment on column tasks.task.created is 'Дата создания';
comment on column tasks.task.created_by is 'ID волонтёра-автора задачи';
comment on column tasks.task.changed_by is 'ID волонтёра, изменившего задачу';

-- tasks_topic_status
create table tasks.topic_status(
    topic_status_id serial primary key,
    topic_status text not null unique
);

comment on table tasks.topic_status is 'Справочная таблица, содержащая возможные статусы топиков';
comment on column tasks.topic_status.topic_status_id is 'ID записи';
comment on column tasks.topic_status.topic_status is 'Статусы топиков';

-- task_status
create table tasks.task_status(
    task_status_id serial primary key,
    task_status text not null unique
);

comment on table tasks.task_status is 'Справочная таблица, содержащая возможные статусы задачек';
comment on column tasks.task_status.task_status_id is 'ID записи';
comment on column tasks.task_status.task_status is 'Статусы задачек';