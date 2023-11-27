--medical care related tables
drop table if exists ctb.med_schedule;
drop table if exists ctb.prescription;
drop table if exists ctb.time_unit;
drop table if exists ctb.clinic_visit_file;
drop table if exists ctb.clinic_visit;

-- clinic_visit table
create table ctb.clinic_visit (
	clinic_visit_id serial PRIMARY KEY,
	cat_id int not null,
	visit_date date,
	clinic_name text,
	doctor_name text
);

comment on table ctb.clinic_visit is 'Посещение врача/ветеринарной клиники';

comment on column ctb.clinic_visit.clinic_visit_id is 'ID посещения';
comment on column ctb.clinic_visit.cat_id is 'ID кошки';
comment on column ctb.clinic_visit.visit_date is 'Дата визита';
comment on column ctb.clinic_visit.clinic_name is 'Название клиники';
comment on column ctb.clinic_visit.doctor_name is 'Имя врача';

-- clinic_visit_file table
create table ctb.clinic_visit_file(
	clinic_visit_file_id serial PRIMARY KEY,
	clinic_visit_id int,
	file_name text,
	file_url text
);

comment on table ctb.clinic_visit_file is 'Файлы посещений';

comment on column ctb.clinic_visit_file.clinic_visit_file_id is 'ID файла';
comment on column ctb.clinic_visit_file.clinic_visit_id is 'ID посещения';
comment on column ctb.clinic_visit_file.file_name is 'Имя файла';
comment on column ctb.clinic_visit_file.file_url is 'URL файла';

-- time_unit table
create table ctb.time_unit(
	time_unit_id serial PRIMARY KEY,
	time_unit_name varchar (10) not null unique
);

comment on table ctb.time_unit is 'Единицы измерения времени';
comment on column ctb.time_unit.time_unit_id is 'ID единицы измерения';
comment on column ctb.time_unit.time_unit_name is 'Наименование единицы измерения';

-- prescription table
create table ctb.prescription(
	prescription_id serial PRIMARY KEY,
	clinic_visit_id int not null,
	prescription_text text not null,
	start_date date,
	duration int,
	one_time_procedure bool not null,
	periodicity_unit_id int,
	periodicity_value int	
);
comment on table ctb.prescription is 'Назначения по медицинскому уходу';

comment on column ctb.prescription.prescription_id is 'ID назначения';
comment on column ctb.prescription.clinic_visit_id is 'ID визита к врачу';
comment on column ctb.prescription.prescription_text is 'Текст назначения';
comment on column ctb.prescription.start_date is 'Дата начала лечения';
comment on column ctb.prescription.duration is 'Длительность лечения в днях';
comment on column ctb.prescription.one_time_procedure is 'Процедура одноразовая';
comment on column ctb.prescription.periodicity_unit_id is 'Периодичность, ед. изм.';
comment on column ctb.prescription.periodicity_value is 'Периодичность, значение';

-- med_schedule table
create table ctb.med_schedule(	
	med_schedule_record_id serial PRIMARY KEY,
	cat_id int not null,
	prescription_id int not null,
	procedure_time timestamp not null,
	done bool not null,
	volunteer_id int
);

comment on table ctb.med_schedule is 'График медицинского ухода';

comment on column ctb.med_schedule.med_schedule_record_id is 'ID записи в графике(журнале) мед. ухода';
comment on column ctb.med_schedule.cat_id is 'ID кошки';
comment on column ctb.med_schedule.prescription_id is 'ID назначения';
comment on column ctb.med_schedule.procedure_time is 'Дата и время назначенной процедуры';
comment on column ctb.med_schedule.done is 'Процедура выполнена';
comment on column ctb.med_schedule.volunteer_id is 'Волонтёр-исполнитель';

-- foreign keys
--clinic_visit fk
alter table ctb.clinic_visit
    add FOREIGN KEY (cat_id)
		references ctb.cat(cat_id);
		
--clinic_visit_file fk		
alter table ctb.clinic_visit_file
	add FOREIGN KEY (clinic_visit_id)
		references ctb.clinic_visit(clinic_visit_id);

--prescription fk
alter table ctb.prescription
	add FOREIGN KEY (clinic_visit_id)
		references ctb.clinic_visit(clinic_visit_id);
alter table ctb.prescription
	add FOREIGN KEY (time_unit_id)
		references ctb.time_unit(time_unit_id);

-- med_schedule fk
alter table ctb.med_schedule
    add FOREIGN KEY (cat_id)
		references ctb.cat(cat_id);
alter table ctb.med_schedule
    add FOREIGN KEY (prescription_id)
		references ctb.prescription(prescription_id);
alter table ctb.med_schedule
    add FOREIGN KEY (volunteer_id)
		references ctb.volunteer(volunteer_id);