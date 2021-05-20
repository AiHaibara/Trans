from PIL import Image
from PIL import ImageEnhance
import PIL.ImageOps as ImageOps
import pytesseract
from io import BytesIO
import base64
import processimg as processimg
import numpy as np
from cv2 import cv2
# import urllib.parse
# import hashlib

def img_estim(img, thrshld):
    is_light = np.mean(img) > thrshld
    return 'light' if is_light else 'dark'

def apply_threshold(img, argument):
    return cv2.adaptiveThreshold(cv2.medianBlur(img, 1), 255, cv2.ADAPTIVE_THRESH_GAUSSIAN_C, cv2.THRESH_BINARY, 31, 2)
    # switcher = {
    #     cv2.threshold(cv2.GaussianBlur(img, (9, 9), 0), 0, 255, cv2.THRESH_BINARY + cv2.THRESH_OTSU)[1],
    #     cv2.threshold(cv2.GaussianBlur(img, (7, 7), 0), 0, 255, cv2.THRESH_BINARY + cv2.THRESH_OTSU)[1],
    #     cv2.threshold(cv2.GaussianBlur(img, (5, 5), 0), 0, 255, cv2.THRESH_BINARY + cv2.THRESH_OTSU)[1],
    #     cv2.adaptiveThreshold(cv2.medianBlur(img, 7), 255, cv2.ADAPTIVE_THRESH_GAUSSIAN_C, cv2.THRESH_BINARY, 31, 2),
    #     cv2.adaptiveThreshold(cv2.medianBlur(img, 5), 255, cv2.ADAPTIVE_THRESH_GAUSSIAN_C, cv2.THRESH_BINARY, 31, 2),
    #     cv2.adaptiveThreshold(cv2.medianBlur(img, 3), 255, cv2.ADAPTIVE_THRESH_GAUSSIAN_C, cv2.THRESH_BINARY, 31, 2)
    # }
    # return switcher.get(argument, "Invalid method")
def recognition(data):
    # image = Image.open(BytesIO(base64.b64decode(urllib.parse.unquote(bytes.decode(data)))))
    image = Image.open(BytesIO(base64.b64decode(data)))
    # image = image.convert('RGB')
    # image=processimg.process_image_data(image)
    img = np.asarray(image)
    img = cv2.resize(img, None, fx=1.5, fy=1.5, interpolation=cv2.INTER_CUBIC)
    
    img = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
    
    image=Image.fromarray(img)

    enhancer = ImageEnhance.Color(image)
    enhancer = enhancer.enhance(0)
    enhancer = ImageEnhance.Brightness(enhancer)
    enhancer = enhancer.enhance(2)
    enhancer = ImageEnhance.Contrast(enhancer)
    enhancer = enhancer.enhance(8)
    enhancer = ImageEnhance.Sharpness(enhancer)
    image = enhancer.enhance(20)

    # image.save("aaa.jpg", "JPEG")
    # ret,bytesim = cv2.threshold(np.asarray(bytesim),127,255,cv2.THRESH_BINARY)
    if img_estim(img,127) == 'dark':
        image=ImageOps.invert(image)
        ret,img = cv2.threshold(img,127,255,cv2.THRESH_BINARY_INV)

    # image=Image.fromarray(img)

    # image.save("bbb.jpg", "JPEG")
    img = np.asarray(image)

    # Apply dilation and erosion to remove some noise
    kernel = np.ones((1, 1), np.uint8)
    img = cv2.dilate(img, kernel, iterations=1)
    img = cv2.erode(img, kernel, iterations=1)
    # Apply threshold to get image with only black and white
    img = apply_threshold(img, 0)
    # Save the filtered image in the output directory
    # save_path = os.path.join(output_path, file_name + "_filter_" + str(method) + ".jpg")
    # cv2.imwrite('save_path', img)
    image=Image.fromarray(img)

    content = pytesseract.image_to_string(image,'eng+chi_sim+jpn+chi_tra')   # 解析图片
    image = image.convert('RGB')

    image.save("src.jpg", "JPEG")
    return content

def recognitionbyfile():
    image = Image.open('source.jpg')
    content = pytesseract.image_to_string(image)   # 解析图片
    print(content)
    return content
