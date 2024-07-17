from itertools import groupby

cyrilic = [
    "а", "б", "в", "г", "д", "е", "ё", "ж", "з", "и", "й", "к",
    "л", "м", "н", "о", "п", "р", "с", "т", "у", "ф", "х", "ц",
    "ч", "ш", "щ", "ъ", "ы", "ь", "э", "ю", "я"
]
def set_format(path):
    summary_read = open(path, 'r', encoding='utf8')
    c = 0
    summary_list = summary_read.readlines()
    summary_read.close()

    sw = open(path, 'w', encoding='utf-8')

    for i in range(0, len(summary_list)):
        summary_list[i] = summary_list[i].replace("\n", ' ')
        sw.write(summary_list[i] + "0 {}" + "\n")

    print(summary_list[:10])
    sw.close()
# for i in cyrilic:
#     set_format(f'{i.upper()}.txt')
set_format('Я.txt')