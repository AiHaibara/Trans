from PIL import Image
import PIL.ImageOps as ImageOps
from PIL import ImageEnhance
import pytesseract
from io import BytesIO
import base64
from cv2 import cv2
import processimg as processimg
# import urllib.parse
# import hashlib

def recognition():
    # image = Image.open(BytesIO(base64.b64decode(urllib.parse.unquote(bytes.decode(data)))))
    processimg.process_image('src.jpg','dest.jpg')
    img = Image.open('dest.jpg')
    # img = cv2.Canny(img,100,200)
    # img = cv2.resize(img, None, fx=1.2, fy=1.2, interpolation=cv2.INTER_CUBIC)
    # img = img.convert('L')
    # img = ImageOps.invert(img)
    # img = img.convert('1')
    # enhancer = ImageEnhance.Color(img)
    # enhancer = enhancer.enhance(0)
    # enhancer = ImageEnhance.Brightness(enhancer)
    # enhancer = enhancer.enhance(2)
    # enhancer = ImageEnhance.Contrast(enhancer)
    # enhancer = enhancer.enhance(8)
    # enhancer = ImageEnhance.Sharpness(enhancer)
    # img = enhancer.enhance(20)
    content = pytesseract.image_to_string(img)   # 解析图片
    img.save("src.jpg", "JPEG")
    print(content)
    return content

def recognitionbyfile():
    image = Image.open('source.jpg')
    content = pytesseract.image_to_string(image)   # 解析图片
    print(content)
    return content

print(recognition())