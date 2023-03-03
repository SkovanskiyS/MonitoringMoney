import requests
from bs4 import BeautifulSoup

page = requests.get("https://bank.uz/currency")
soup = BeautifulSoup(page.text,"lxml")
list_ = soup.find("a",class_="nav-link 769 active").find_all("span",class_="medium-text")
currency_text = list_[1].text.strip()
with open('currency.txt','w') as file:
    file.write(currency_text)
