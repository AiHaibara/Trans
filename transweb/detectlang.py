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
        print('detect')
        print(res)
        for item in res:
            if item.lang in ['en','zh','ja','zh-cn','zh-tw']:
                return item.lang
            if item.lang in ['zh-CN','zh-TW','zh-Hans','ko']:
                return 'zh'
        return 'en'
        # return detect(text)
    except Exception:
        print("Unexpected error:", sys.exc_info()[0])
        return 'en'
# print(detect("in公式サイトです."))
# print(detect("Ein, zwei, drei, vier"))