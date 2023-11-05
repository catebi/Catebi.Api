delete from ctb.role_permission;
delete from ctb.permission;
delete from ctb.role;

insert into ctb.role (role_id, name) 
values 
    (1, 'head of the cat-house'),
    (2, 'cat-house cleaner on duty');


insert into ctb.permission (permission_id, name) 
values 
    (1, 'get_actual_schedules'),
    (2, 'get_volunteers_for_schedule'),
    (3, 'set_schedule'),
    (4, 'get_schedule'),
    (5, 'revoke_schedule');

-- permissions for "head of the cat-house"
insert into ctb.role_permission (role_id, permission_id) 
values 
(1, (select permission_id from ctb.permission where name = 'get_actual_schedules')),
(1, (select permission_id from ctb.permission where name = 'get_volunteers_for_schedule')),
(1, (select permission_id from ctb.permission where name = 'set_schedule')),
(1, (select permission_id from ctb.permission where name = 'get_schedule')),
(1, (select permission_id from ctb.permission where name = 'revoke_schedule'));

-- permissions for "cat-house cleaner on duty"
insert into ctb.role_permission (role_id, permission_id) 
values 
(2, (select permission_id from ctb.permission where name = 'get_schedule')),
(2, (select permission_id from ctb.permission where name = 'set_schedule')),
(2, (select permission_id from ctb.permission where name = 'revoke_schedule'));