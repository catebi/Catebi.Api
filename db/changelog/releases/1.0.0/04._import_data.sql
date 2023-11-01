delete from ctb.cat_sex;
delete from ctb.color;

insert into ctb.color (color_id, name, rgb_code, hex_code)
overriding system value
values
    (1, 'Default', '255,255,255', '#FFFFFF'),
    (2, 'Gray', '235,236,237', '#EBECED'),
    (3, 'Brown', '233,229,227', '#E9E5E3'),
    (4, 'Orange', '250,235,221', '#FAEBDD'),
    (5, 'Yellow', '251,243,219', '#FBF3DB'),
    (6, 'Green', '221,237,234', '#DDEDEA'),
    (7, 'Blue', '221,235,241', '#DDEBF1'),
    (8, 'Purple', '234,228,242', '#EAE4F2'),
    (9, 'Pink', '244,223,235', '#F4DFEB'),
    (10, 'Red', '251,228,228', '#FBE4E4');

insert into ctb.cat_sex (cat_sex_id, name, color_id)
overriding system value
values
    (1, 'ж', 7),
    (2, 'м', 9);