drop table if exists ctb.neutering_cat_returning;
drop table if exists ctb.neutering_request;
drop table if exists ctb.neutering_sponsor;

create table ctb.neutering_request(
    neutering_request_id serial primary key,    
    user_contact text not null,    
    
    cat_count int not null,    
    cat_address text not null, 
    pregnant_included bool not null, 
    unhealthy_included bool not null, 
    cat_sex_id int, 
    
    need_help_catch bool not null, 
    need_help_deliver bool not null, 
    needed_carrier_count int,    
    needed_catcher_count int,    
    neutering_sponsor_id int not null, 
    clinic_id int,
    cat_count_foster int, 
    cat_returning_by_user bool not null, 
    cat_vaccinated bool, 
    cat_defleated bool, 
    cat_dehelminted bool 
);
  
comment on table ctb.neutering_request is 'Данные анкет (заявок на стерилизацию кошек)';
comment on column ctb.neutering_request.neutering_request_id  is 'id в бд';
comment on column ctb.neutering_request.user_contact  is 'Телефонный номер или ссылка на аккаунт в соц.сети';
comment on column ctb.neutering_request.cat_count  is 'Количество кошек';
comment on column ctb.neutering_request.cat_address  is 'Адрес места, где были обнаружены кошки';
comment on column ctb.neutering_request.pregnant_included  is 'Среди кошек есть беременные';
comment on column ctb.neutering_request.unhealthy_included  is 'Среди кошек есть больные';
comment on column ctb.neutering_request.cat_sex_id  is 'Пол кошек.';	-- 1="Все М", 2="Все Ж", NULL = "разные/не знаю"
comment on column ctb.neutering_request.need_help_catch  is 'Нужна помощь волонтёров с поимкой кошек';
comment on column ctb.neutering_request.need_help_deliver  is 'Нужна помощь волонтёров с доставкой кошек в КК/клинику';
comment on column ctb.neutering_request.needed_carrier_count  is 'Сколько нужно переносок (если нужны)';
comment on column ctb.neutering_request.needed_catcher_count  is 'Сколько нужно котоловок (если нужны)';
comment on column ctb.neutering_request.neutering_sponsor_id  is 'Платит ли заполнитель за стерилизацию'; 
comment on column ctb.neutering_request.clinic_id  is 'id вет.клиники, где будет стерилизация (если это важно)';
comment on column ctb.neutering_request.cat_count_foster  is 'Сколько кошек пользователь может взять себе на передержку после стерилизации';
comment on column ctb.neutering_request.cat_returning_by_user  is 'Организация доставки обратно'; -- !!! СПРАВОЧНИК НАДО пользователь вернёт кошек на адрес после реабилитации (либо оплатит доставку)
comment on column ctb.neutering_request.cat_vaccinated  is 'Кошки привиты';
comment on column ctb.neutering_request.cat_defleated  is 'Кошки обработаны от блох (или будут)';
comment on column ctb.neutering_request.cat_dehelminted  is 'Кошки обработаны от глистов (или будут)';

create table ctb.neutering_sponsor(--!!! Primary filling 'плачу полностью', 'плачу частично', 'за счёт Catebi'
    neutering_sponsor_id serial primary key,
    option_text text unique not null
);

comment on table ctb.neutering_sponsor is 'Справочник к полю анкеты "Платит ли заполнитель анкеты за стерилизацию"';
comment on column ctb.neutering_sponsor.neutering_sponsor_id is 'ID';
comment on column ctb.neutering_sponsor.option_text is 'Текст варианта';

create table ctb.neutering_cat_returning ( -- !!! Primary filling пользователь вернёт кошек на адрес после реабилитации  / либо оплатит доставку / другое(?)
    neutering_cat_returning_id serial primary key,
    option_text text unique not null
);

comment on table ctb.neutering_cat_returning is 'Cправочник к полю анкеты "Организация доставки обратно"';
comment on column ctb.neutering_cat_returning.neutering_cat_returning_id is 'ID';
comment on column ctb.neutering_cat_returning.option_text is 'Текст варианта';

--neutering_request fk
alter table ctb.neutering_request
    add foreign key (neutering_sponsor_id)
        references ctb.neutering_sponsor(neutering_sponsor_id);
alter table ctb.neutering_request
    add foreign key (clinic_id)
        references ctb.clinic(clinic_id);
alter table ctb.neutering_request
    add foreign key (cat_sex_id)
        references ctb.cat_sex(cat_sex_id);
