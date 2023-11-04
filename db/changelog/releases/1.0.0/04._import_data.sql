delete from ctb.cat_sex;
delete from ctb.color;
delete from ctb.cat_tag;
delete from ctb.cat_collar;

insert into ctb.color (color_id, name, rgb_code, hex_code)
values
    ( 1  , 'Light gray' , '241,240,239' , 'F1F0EF' ) ,
    ( 2  , 'Gray'       , '227,226,224' , 'E3E2E0' ) ,
    ( 3  , 'Brown'      , '238,224,218' , 'EEE0DA' ) ,
    ( 4  , 'Orange'     , '250,222,201' , 'FADEC9' ) ,
    ( 5  , 'Yellow'     , '253,236,200' , 'FDECC8' ) ,
    ( 6  , 'Green'      , '219,237,219' , 'DBEDDB' ) ,
    ( 7  , 'Blue'       , '211,229,239' , 'D3E5EF' ) ,
    ( 8  , 'Purple'     , '232,222,238' , 'E8DEEE' ) ,
    ( 9  , 'Pink'       , '245,224,233' , 'F5E0E9' ) ,
    ( 10 , 'Red'        , '255,226,221' , 'FFE2DD' )
;

insert into ctb.cat_sex (cat_sex_id, name, color_id)
values
	(1, 'М', 7),
	(2, 'Ж', 6)
;