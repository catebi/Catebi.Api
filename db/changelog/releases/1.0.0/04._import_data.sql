delete from ctb.cat_sex;
delete from ctb.color;
delete from ctb.cat_tag;
delete from ctb.cat_collar;

insert into ctb.color ("name", rgb_code, hex_code)
overriding system value
values
    ('Light gray', '241, 240, 239', 'F1F0EF'),
    ('Gray', '227,226,224', 'E3E2E0'),
    ('Brown', '238,224,218', 'EEE0DA'),
    ('Orange', '250,222,201', 'FADEC9'),
    ('Yellow', '253,236,200', 'FDECC8'),
    ('Green', '219,237,219', 'DBEDDB'),
    ('Blue', '211,229,239', 'D3E5EF'),
    ('Purple', '232,222,238', 'E8DEEE'),
    ('Pink', '245,224,233', 'F5E0E9'),
    ('Red', '255,226,221', 'FFE2DD')
    
;

insert into ctb.cat_sex ("name", color_id)
overriding system value
values
	('М', 7),	-- Цвет мужского пола в Notion - "Blue"
	('Ж', 6)	-- Цвет женского пола в Notion - "Green"
;

insert into ctb.cat_house_space ("name", color_id)
overriding system value
values
	('K1', 7),  -- Цвет К1 в Notion - "Blue"
	('К2', 6), -- Цвет К2 в Notion - "Green"
	('Кухня', 10),   -- Цвет кухни в Notion - "Red"
	('Ванная', 5)	-- Цвет ванной в Notion - "Yellow"
;

insert into ctb.cat_tag ("name", color_id)
overriding system value
values
	('(!)Потеряна', 10),  -- Все цвета соответствуют подсветке тегов в Notion
	('На новом адресе', 7),
	('Аборт', 5),
	('Пугливая++', 10),
	('Переедет', 9),
	('Медуход(!)', 7),
	('Медуход(!!)', 7),
	('Отказник', 4),
	('Платная передержка', 8),
	('Гастрокорм', 3),
	('Нужна социализация', 1),
	('Обработка', 6),
	('Блохи++', 2),
	('Блохи+', 2),
	('Шов', 5),
	('Беременная', 2),
	('Бронхит', 9),
	('ВИК', 10)
;

insert into ctb.cat_collar ("name", color_id)
overriding system value
values
	('Красный', 10),
	('Голубой', 7),
	('Розовый', 9),
	('Желтый', 5),
	('Черный', 2),
	('Зеленый', 6),
	('Серый', 1),
	('Бордовый', 10),
	('Коричневый', 3),
	('Синий', 7),
	('Салатовый', 6),
	('Белый', 1),
	('Малиновый', 4),
	('Фиолетовый', 8),
	('С бубенчиком 🔔', 1),
	('С цветочками 🌼', 1),
	('Коровка', 7)
;