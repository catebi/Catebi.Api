drop table if exists tasks.task_responsible_volunteer;
drop table if exists tasks.task_reminder;
drop table if exists tasks.task;
drop table if exists tasks.topic;
drop table if exists tasks.task_status;

-- tasks_topic 
create table tasks.topic(
    topic_id serial primary key,       
    telegram_topic_id int not null unique,  
    name text not null,  
    description text,  
    is_main_topic bool not null,  
    is_actual bool not null, 
    created timestamp,
    created_by_id int not null 
);

comment on table tasks.topic is 'Таблица для хранения инфоромации о топиках в чате Catebi';
comment on column tasks.topic.topic_id is 'ID записи';
comment on column tasks.topic.telegram_topic_id is 'ID топика в tg';
comment on column tasks.topic.name is 'Название топика';
comment on column tasks.topic.description is 'описание топика';
comment on column tasks.topic.is_main_topic is 'топик для напоминаний об активных задачах';
comment on column tasks.topic.is_actual is 'Флаг актуальности';
comment on column tasks.topic.created is 'Дата создания';
comment on column tasks.topic.created_by_id is 'ID волонтёра-автора топика';

-- tasks_task
create table tasks.task(
    task_id serial primary key,  
    topic_id integer not null,             
    description text not null,  
    status_id int not null,                 
    created_date timestamp, 
    changed_date timestamp, 
    created_by_id int not null,  
    changed_by_id int not null              
);

comment on table tasks.task is 'Таблица для хранения задач в чате Catebi';
comment on column tasks.task.task_id is 'ID записи';
comment on column tasks.task.topic_id is 'ID топика, в котором создана задача';
comment on column tasks.task.description is 'описание задачи';
comment on column tasks.task.status_id is 'ID статуса задачи';
comment on column tasks.task.created_date is 'Дата создания';
comment on column tasks.task.changed_date is 'Дата изменения';
comment on column tasks.task.created_by_id is 'ID волонтёра-автора задачи';
comment on column tasks.task.changed_by_id is 'ID волонтёра, изменившего задачу';

-- task_status
create table tasks.task_status(
    task_status_id serial primary key,
    code int not null,
    name text not null
);

comment on table tasks.task_status is 'Справочная таблица, содержащая возможные статусы задачек';
comment on column tasks.task_status.task_status_id is 'ID записи';
comment on column tasks.task_status.code is 'Код статуса';
comment on column tasks.task_status.name is 'Статусы задачек';

create table tasks.task_responsible_volunteer(
    task_responsible_volunteer_id serial primary key,
    task_id int not null,  
    volunteer_id int not null  
);

comment on table tasks.task_responsible_volunteer is 'Информация о волонтёре, ответственном за задачу';
comment on column tasks.task_responsible_volunteer.task_responsible_volunteer_id is 'ID записи';
comment on column tasks.task_responsible_volunteer.task_id is 'ID задачи';
comment on column tasks.task_responsible_volunteer.volunteer_id is 'ID волонтёра, ответственного за задачу';

create table tasks.task_reminder(
    task_reminder_id serial primary key,
    task_id int not null,  
    reminder_date timestamp not null,
    created timestamp not null,
    created_by_id int not null  
);

comment on table tasks.task_reminder is 'Информация для оповещений по задачам';
comment on column tasks.task_reminder.task_reminder_id is 'ID записи';
comment on column tasks.task_reminder.task_id is 'ID задачи';
comment on column tasks.task_reminder.reminder_date is 'дата оповещения';
comment on column tasks.task_reminder.created is 'дата создания';
comment on column tasks.task_reminder.created_by_id is 'ID волонтёра-создателя задачи';


-- FK in task table
alter table tasks.task 
    add FOREIGN KEY (topic_id)
            references tasks.topic(topic_id);
alter table tasks.task 
    add FOREIGN KEY (status_id)
            references tasks.task_status(task_status_id);
alter table tasks.task 
    add FOREIGN KEY (changed_by_id)
            references ctb.volunteer(volunteer_id);

-- FK in task_responsible_volunteer table 
alter table tasks.task_responsible_volunteer
    add FOREIGN KEY (task_id)
        references tasks.task(task_id);
alter table tasks.task_responsible_volunteer
    add FOREIGN KEY (volunteer_id)
        references ctb.volunteer(volunteer_id);
    
-- FK in task_reminder table 
alter table tasks.task_reminder
    add FOREIGN KEY (task_id)
        references tasks.task(task_id);
alter table tasks.task_reminder
    add FOREIGN KEY (created_by_id)
        references ctb.volunteer(volunteer_id);