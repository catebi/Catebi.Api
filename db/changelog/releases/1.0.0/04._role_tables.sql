drop table if exists ctb.volunteer_role;
drop table if exists ctb.role_permission;
drop table if exists ctb.role;
drop table if exists ctb.permission;

-- ctb.role table
create table ctb.role (
    role_id serial PRIMARY KEY,
    name text
);

comment on table ctb.role IS '������ ����� ���������';
comment on column ctb.role.role_id IS 'ID ����';
comment on column ctb.role.name IS '������������ ����';

-- ctb.permission table
create table ctb.permission (
    permission_id serial PRIMARY KEY,
    name text
);

comment on table ctb.permission IS '������ ���� (����������/��������)';
comment on column ctb.permission.permission_id IS 'ID �����';
comment on column ctb.permission.name IS '������������ ����� ';

-- ctb.role_permission table
create table ctb.role_permission (
    role_id integer NOT NULL,
    permission_id integer NOT NULL
);

comment on table ctb.role_permission IS '������� ����� ���� � ������������';
comment on column ctb.role_permission.role_id IS 'ID ����';
comment on column ctb.role_permission.permission_id IS 'ID ����������';

-- ctb.volunteer_role table
create table ctb.volunteer_role (
    role_id integer NOT NULL,
    volunteer_id integer NOT NULL
);

-- foreign keys 
-- FK in role_permission 
alter table ctb.role_permission
	ADD FOREIGN KEY (role_id) 
		REFERENCES ctb.role(role_id);
alter table ctb.role_permission
	ADD FOREIGN KEY (permission_id) 
		REFERENCES ctb.permission(permission_id);

-- FK in volunteer_role
alter table ctb.volunteer_role
	ADD FOREIGN KEY (role_id) 
		REFERENCES ctb.role(role_id);
alter table ctb.volunteer_role
	ADD FOREIGN KEY (volunteer_id) 
		REFERENCES ctb.volunteer(volunteer_id);