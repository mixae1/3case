import scrapy
import json


class QuotesSpider(scrapy.Spider):
    name = "quotes"

    # Начальные запросы
    def start_requests(self):
        start_urls = ['https://yandex.ru/maps/39/rostov-na-donu/category/pub_bar/',
                      'https://yandex.ru/maps/39/rostov-na-donu/category/pharmacy/',
                      'https://yandex.ru/maps/39/rostov-na-donu/category/automated_doors_and_gates/',
                      'https://yandex.ru/maps/39/rostov-na-donu/category/bus_companies/',
                      'https://yandex.ru/maps/39/rostov-na-donu/category/post_office/',
                      'https://yandex.ru/maps/39/rostov-na-donu/category/veterinary_pharmacy/',
                      'https://yandex.ru/maps/39/rostov-na-donu/category/hotel_reservations/']
        for url in start_urls:
            yield scrapy.Request(url, self.parse, cb_kwargs=dict(dir_name = '1'))

    # Сбор ссылок категорий
    def category_parse(self, response):
        urls = list(map(lambda x: 'https://yandex.ru' + x.split('" ')[1].split('"')[1] ,response.css('.catalog-group-view__rubric').getall()))
        for url in urls:
            print(url)
            yield scrapy.Request(url,self.parse, cb_kwargs=dict(dir_name = response.url.split('/')[-2]))

    # Получаем ссылки на страницы организаций
    def parse(self, response, dir_name):
        urls = response.css('.config-view').re('\"url\":\"(http[s]?://yandex\.ru/maps/org/([\s\S]+?))\"')[::2]
        for url in urls:
            yield scrapy.Request(url,self.orgpage_parse, cb_kwargs=dict(file_name = dir_name + '/' + response.url.split('/')[-2] + '.json'))

    # Сам сбор данных по организации
    def orgpage_parse(self,response, file_name):
        # Вырезаем данные со страницы
        name = response.css('.orgpage-header-view__header::text').get()
        url = response.css('.orgpage-contacts-view__url-link::attr(href)').getall()
        social = response.css('div._size_small > div:nth-child(1) a::attr(href)').getall()
        adress = response.css('.orgpage-contacts-view__address::attr(title)').get()
        phone = response.css('.orgpage-phones-view__phone-number::text').re(r"(\+?[78]\ ?\(?\d{3}\)?\ ?\d{3}([ \-]?)\d{2}\2\d{2})")

        
        if phone != []:
            phone.pop(-1)
        i = 0

        # Открываем файл для сохранения данных
        try:
            with open(file_name,'r') as f:
                data = json.load(f)
        except (FileNotFoundError,json.decoder.JSONDecodeError):
            data = list()

        # Сохраняем данные
        data.append(dict())
        data[-1]['name'] = name
        if url is not None:
            data[-1]['url'] = url 
        data[-1]['social'] = social
        data[-1]['adress'] = adress
        data[-1]['phone'] = phone
        with open(file_name,'w') as f:
            json.dump(data, f, ensure_ascii = False)
