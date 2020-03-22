from langdetect import detect 
import sys
# import langid
from langdetect import detect_langs
# langid.set_languages(['fr', 'en'])

def detectlanguage(text):
    try:
        # print(detect_langs(text))
        # print(detect(text))
        res = detect_langs(text)
        for item in res:
            if item.lang in ['en','zh','ja','zh-cn','zh-tw']:
                return item.lang
        return 'en'
        # return detect(text)
    except Exception:
        print("Unexpected error:", sys.exc_info()[0])
        return 'en'
# print(detect("in公式サイトです."))
# print(detect("Ein, zwei, drei, vier"))