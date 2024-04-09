drop view if exists ctb.cat_statistics_output;

create view ctb.cat_statistics_output (label, value) as 
    select 'Кошек находится в КК', count(cat_id)
    from ctb.cat
    where in_date is not null 
        and out_date is null

    union    

    select 'Стерилизовано кошек за весь период', count(cat_id)
    from ctb.cat
    where neutered_date is not null

    union

    select 'Стерелизовано кошек в текущем месяце', count(cat_id)
    from ctb.cat
    where extract(month from neutered_date) = extract(month from now())

    union

    select 'Стерелизовано кошек за предыдущий месяц', count(cat_id)
    from ctb.cat
    where extract(month from neutered_date) = extract(month from now())-1
;

comment on view ctb.cat_statistics_output is 'Данные для ежемесечных отчётов об объёмах стерилизации кошек'