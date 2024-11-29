using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct RandomParamSt
{
    public static readonly List<string> Names = new(){"Yashelter","Роман Шумилов","Никита Лакин","Николас Янушевский","Никита Нехаенко","Lena Saily","Леха","Volodya Spicy","Дима Белых","Рамиль Габдрахманов","DarSmoke","Kris","Rose","Дима Исаев","Кирилл","Александра","Абдулкадир Карагишев","Марсель","Егор Сечинский","DukFunduk","Голд Голдич","Невский","Искандариосельвиартемидорафаниэльвикторинославианаторомахендрополидорос","Евгений Стеценко", "Аркадий Лаптин","Борис Медвежский","Гризлиан Зверев","Урсула Лесничая","Михаил Потапыч","Василий Бальзамов","Анфиса Полярова","Игнат Брун","Степан Лесовик","Дарья Сотникова","Виктор Бриан","Алексей Мишкин","Ольга Клюквина","Юрий Когтев","Анастасия Урсовская","Константин Пушистин","Наталья Мядовская","Иван Тайгов","Елена Медоварова","Дмитрий Арктиков","Артемий Шишкин","Петр Гомон","Роман Лапон","Лидия Хвойкина","Владислав Бамбера","Ксения Сотникова","Тихон Топтыгин","Максим Грызлов","Софья Лапонова","Олег Барибал","Лев Мишанин","Тамара Пушинка","Денис Гризлов","Вера Урсова","Михаил Сапач","Андрей Клешнев","Екатерина Карамельская","Сергей Когтярев","Мария Лисковец","Владимир Речник","Алена Урсалет","Игнат Лесов","Полина Северовская","Федор Углов","Валерия Лапина","Иван Михайлович","Надежда Барна","Артем Гомон","Анна Хвоинка","Татьяна Рябинина","Виктор Лесничий","Оксана Брусничкина","Егор Потапенко","Светлана Арктова","Семен Грызлов","Василиса Бальзамова","Тимофей Урсланд","Анастасия Мишкина","Кирилл Сотников","Галина Арктина","Ирина Пушинская","Борис Урманов","Нина Медович","Артур Хвоинов","Зоя Урсовская","Станислав Потапыч","Юлия Тайгинская","Ольга Арктусова","Марк Лесович","Павел Лаптев","Стефания Брусничная","Кристина Карамельник","Виталий Медведев","Дарина Бальзамирова","Олег Топтыгин","Валерий Лесопильский","Екатерина Арктония","Василий Урсевич","Людмила Сотникова","Влад Арктик","Алена Медоносова","Андрей Когтев","Наталия Барибалина","Тимофей Лаптиков","Ольга Гризлова","Клавдия Урсолова","Михаил Лесопарин","Константин Медоваров","Полина Клешнева","Игорь Потапков","София Карамельцева","Антон Тайгинский","Валентин Мишкинов","Елизавета Барибалова","Николай Лесович","Тихон Арктович","Лидия Мядович","Кирилл Урсаков","Юлия Брусничка","Аркадий Медонос","Виктория Арктисова","Андрей Гризловский","Александр Лапочный","Варвара Сотникович","Сергей Потапов","Екатерина Урманова","Владимир Лесопилов","Артем Бальзамыч","Дарья Когтяр","Павел Барибалович","Анастасия Шишкович","Олег Хвойнович","Марина Медовая","Михаил Сапич","Денис Лесинский","Светлана Арктична","Иван Потапкин","Зинаида Лаптинская","Дмитрий Урманский","Николай Мишанин","Наталья Гомонова","Константин Брусничников","Елена Тайгина","Максим Урсевич","Маргарита Арктисович","Владимир Медовиков","Василиса Карамельцова","Федор Сапичев","Юрий Лапычев","Надежда Брусничина","Анатолий Арктум","Оксана Потаповна","Алексей Мишкович","Тамара Урманович","Екатерина Тайгинова","Валентин Лесничихин","Ксения Гомоновна","Артем Барибалов","Полина Клешнинова","Егор Мишков","Ирина Карамельская","Зоя Урсолова","Виталий Потапичев","Виктор Брусничкин","Ольга Медовичева","Константин Арктич","Денис Урсальский","Алексей Лесничий","Лидия Потапович","Тихон Брусничкин","Юлия Лесникова","Роман Гомонович","Оксана Арктович","Анфиса Мишкович","Дарина Урманова","Павел Медовников","Сергей Арктонович","Елена Барибалина","Алексей Тайгин"};
    public static readonly List<string> RobotsReplics = new() {"Ох зря ты нас впустил...", "Не зачем было лезть в его дело...", "~~~ тебя не простит", "Предатель!" };
    public static readonly List<Sprite> Photos = Resources.LoadAll<Sprite>("BearPhotos").ToList();
    public static readonly List<Sprite> Stamps = new()
    {
        Resources.Load<Sprite>("Sprites/BearPhotos/pig"),
        Resources.Load<Sprite>("Sprites/BearPhotos/pig"),
        Resources.Load<Sprite>("Sprites/BearPhotos/pig"),
        Resources.Load<Sprite>("Sprites/BearPhotos/pig"),
        Resources.Load<Sprite>("Sprites/BearPhotos/pig"),
        Resources.Load<Sprite>("Sprites/BearPhotos/pig"),
        Resources.Load<Sprite>("Sprites/BearPhotos/pig"),
        Resources.Load<Sprite>("Sprites/BearPhotos/pig"),
        Resources.Load<Sprite>("Sprites/BearPhotos/pig"),
        Resources.Load<Sprite>("Sprites/BearPhotos/pig"),
        Resources.Load<Sprite>("Sprites/BearPhotos/pig"),
    };
}

public enum Planet
{
    Медовия, Медвежегол, Урса, Бореалис, Гризлиум, Арктос, Полярис, Когтярус, Стелларис
}
