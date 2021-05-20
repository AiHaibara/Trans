import dl_translate as dlt

mt = dlt.TranslationModel(device="gpu")  # Slow when you load it for the first time

print(mt.translate('最近在给新的电脑配置深度学习环境,找了个不是特别新CUDA(担心未适配)版本来安排,安装CUDA...但是,安装torch 1.7 和tensorflow2后,测试是否能调用GPU失败了,经过将近一天的各种尝试', source=dlt.lang.CHINESE, target=dlt.lang.ENGLISH))

print(mt.available_languages())  # All languages that you can use
print(mt.available_codes())  # Code corresponding to each language accepted
print(mt.get_lang_code_map())  # Dictionary of lang -> code