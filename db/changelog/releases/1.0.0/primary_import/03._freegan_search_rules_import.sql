DO $$
DECLARE
    main_group_id int;
    ac_group_id int;
BEGIN

    -- Insert the 'main group'
    INSERT INTO frgn.group (name, is_actual) VALUES ('main group', true) RETURNING group_id INTO main_group_id;

    -- Insert keywords for 'main group'
    INSERT INTO frgn.keyword (group_id, keyword)
    VALUES
      (main_group_id, 'ампула'),
      (main_group_id, 'бентонит'),
      (main_group_id, 'бетонит'),
      (main_group_id, 'витамин'),
      (main_group_id, 'вкусняшка'),
      (main_group_id, 'вольер'),
      (main_group_id, 'габа'),
      (main_group_id, 'габапентин'),
      (main_group_id, 'джут'),
      (main_group_id, 'джутовый'),
      (main_group_id, 'домик'),
      (main_group_id, 'дралка'),
      (main_group_id, 'игрушка'),
      (main_group_id, 'инъекция'),
      (main_group_id, 'клетка'),
      (main_group_id, 'когтедерка'),
      (main_group_id, 'когтедралка'),
      (main_group_id, 'когтеточка'),
      (main_group_id, 'консервы'),
      (main_group_id, 'корм'),
      (main_group_id, 'кот'),
      (main_group_id, 'кошачий'),
      (main_group_id, 'кошка'),
      (main_group_id, 'лакомство'),
      (main_group_id, 'лежак'),
      (main_group_id, 'лежанка'),
      (main_group_id, 'лотков'),
      (main_group_id, 'лоток'),
      (main_group_id, 'мальтпаста'),
      (main_group_id, 'мальт-паста'),
      (main_group_id, 'мята'),
      (main_group_id, 'молоко'),
      (main_group_id, 'наполнитель'),
      (main_group_id, 'паштет'),
      (main_group_id, 'пелёнка'),
      (main_group_id, 'поводок'),
      (main_group_id, 'подстилка'),
      (main_group_id, 'поилка'),
      (main_group_id, 'тоннель'),
      (main_group_id, 'туннель'),
      (main_group_id, 'укол'),
      (main_group_id, 'фонтанчик'),
      (main_group_id, 'шлейка'),
      (main_group_id, 'шприц');

    -- Insert included keywords for 'main group'
    INSERT INTO frgn.group_included_keyword (group_id, keyword)
    VALUES
      (main_group_id, 'животное'),
      (main_group_id, 'кошачий'),
      (main_group_id, 'кошка'),
      (main_group_id, 'кот');

    -- Insert the 'air conditioner group'
    INSERT INTO frgn.group (name, is_actual) VALUES ('air conditioner group', true) RETURNING group_id INTO ac_group_id;

    -- Insert keywords for 'air conditioner group'
    INSERT INTO frgn.keyword (group_id, keyword)
    VALUES
      (ac_group_id, 'кондиционер'),
      (ac_group_id, 'кондей'),
      (ac_group_id, 'сплитсистема'),
      (ac_group_id, 'сплит-система');

    -- Insert excluded keywords for 'air conditioner group'
    INSERT INTO frgn.group_excluded_keyword (group_id, keyword)
    VALUES
      (ac_group_id, 'аренда'),
      (ac_group_id, 'купить'),
      (ac_group_id, 'шампунь'),
      (ac_group_id, 'сдаваться'),
      (ac_group_id, 'сдавать'),
      (ac_group_id, 'сдать'),
      (ac_group_id, 'трансфер'),
      (ac_group_id, 'визаран');
END $$;