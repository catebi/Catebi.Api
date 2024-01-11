drop table if exists ctb.volunteer_role;
drop table if exists ctb.role_permission;
drop table if exists ctb.role;
drop table if exists ctb.permission;

-- ctb.role table
create table ctb.role (
    role_id serial primary key,
    name text unique
);	

comment on table ctb.role is 'Список ролей волонтёров';
comment on column ctb.role.role_id is 'ID роли';
comment on column ctb.role.name is 'Наименование роли';

-- ctb.permission table
create table ctb.permission (
    permission_id serial primary key,
    name text unique
);

comment on table ctb.permission is 'Список прав (разрешений/доступов)';
comment on column ctb.permission.permission_id is 'ID права';
comment on column ctb.permission.name is 'Наименование права';

-- ctb.role_permission table
create table ctb.role_permission (
	role_permission_id serial primary key,
    role_id integer not null,
    permission_id integer not null,
	unique (role_id, permission_id)
);

comment on table ctb.role_permission is 'Таблица связи роли с разрешениями';
comment on column ctb.role_permission.role_permission_id is 'ID соотношения';
comment on column ctb.role_permission.role_id is 'ID роли';
comment on column ctb.role_permission.permission_id is 'ID разрешения';

-- ctb.volunteer_role table
create table ctb.volunteer_role (
    volunteer_role_id serial primary key,
    role_id integer not null,
    volunteer_id integer not null
);

comment on table ctb.volunteer_role is 'Таблица связи "волонтёр-роль"';
comment on column ctb.volunteer_role.volunteer_role_id is 'ID соотношения';
comment on column ctb.volunteer_role.role_id is 'ID роли';
comment on column ctb.volunteer_role.volunteer_id is 'ID волонтёра';

-- foreign keys 
-- FK in role_permission 
alter table ctb.role_permission
	add foreign key (role_id) 
		references ctb.role(role_id);
alter table ctb.role_permission
	add foreign key (permission_id) 
		references ctb.permission(permission_id);

-- FK in volunteer_role
alter table ctb.volunteer_role
	add foreign key (role_id) 
		references ctb.role(role_id);
alter table ctb.volunteer_role
	add foreign key (volunteer_id) 
		references ctb.volunteer(volunteer_id);
