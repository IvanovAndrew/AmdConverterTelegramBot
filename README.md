[![.NET Core](https://github.com/IvanovAndrew/AmdConverterTelegramBot/actions/workflows/dotnet-desktop.yml/badge.svg?branch=master)](https://github.com/IvanovAndrew/AmdConverterTelegramBot/actions/workflows/dotnet-desktop.yml)

# AmdTelegramBotConverter

A Telegram bot that converts dollars, euros, georgian lari, or rubles into Armenian drams, taking into account the exchange rates of Armenian banks, and also converts Armenian drams into the aforementioned currencies. It also considers whether the exchange is done in cash or not.

Link https://t.me/AmdToRurConverterBot


To start using it, type any amount, for example, 50 euros. Then the bot will ask about the desired type of currency exchange and inquire whether it's cash or non-cash. You will get a table like this
```
€50 -> ֏ (Cash)
         Bank|Rate|€ -> ֏
-------------|----|------
  Artsakhbank| 434|21700֏
          SAS| 434|21700֏
 ArmSwissBank| 433|21650֏
  Mellat Bank| 432|21600֏
       Byblos| 431|21550֏
    Fast Bank| 430|21500֏
    Evocabank| 430|21500֏
    Inecobank| 429|21450֏
  Ardshinbank| 429|21450֏
ARMECONOMBANK| 429|21450֏
      Unibank| 429|21450֏
Converse Bank| 429|21450֏
          VTB| 428|21400֏
       IDBank| 428|21400֏
    Acba bank| 428|21400֏
         HSBC| 428|21400֏
    AMIO BANK| 428|21400֏
     Amiobank| 428|21400֏
   AraratBank| 427|21350֏
   Ameriabank| 427|21350֏
```
