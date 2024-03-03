drop view if exists ctb.cat_statistics_output;

create view ctb.cat_statistics_output (label, value) as 
    select '����� ��������� � ��', count(cat_id)
    from ctb.cat
    where in_date is not null 
        and out_date is null

    union    

    select '������������� ����� �� ���� ������', count(cat_id)
    from ctb.cat
    where neutered_date is not null

    union

    select '������������� ����� � ������� ������', count(cat_id)
    from ctb.cat
    where extract(month from neutered_date) = extract(month from now())

    union

    select '������������� ����� �� ���������� �����', count(cat_id)
    from ctb.cat
    where extract(month from neutered_date) = extract(month from now())-1
;

comment on view ctb.cat_statistics_output is '������ ��� ����������� ������� �� ������� ������������ �����'