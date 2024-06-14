Проект является частью выпускной квалификационной работы бакалавра на тему "Разработка программного комлекса для мониторинга платной парковки", в данном случае это сервисное приложение обрабатывающее видео с камер одной из парковок - http://krkvideo2.orionnet.online/cam3209/tracks-v1/mono.m3u8,
производится парсинг плейлиста, скачиваются куски видео по 20 секунд и соединяются в одно, дальше обрабатываются библиотеками, схема показана на риснуке.
![image](https://github.com/70Null07/ParkingControlCenter/assets/76547066/915c7a42-4a3e-4429-ab31-628453ff3390)

Для работы необходимо скачать обученную модель https://drive.google.com/file/d/1s_vjRpzZ7dkRSt20gj6qFM-9CdYDipIb/view?usp=drive_link и поместить ее в папку ParkingControlCenter, также необходимо настроить связи на сборки библиотек CarsClassification&ANPR, CarsRecognitionLibrary, CarsMovementLibrary.
Для программы необходим ffmpeg, который должен быть расположен по пути C:/ffmpeg/bin, а также установленный VLC Media Player x32 или x64. Runtime .NET Core 8. Также необходимо настраивать место сохранения файлов визуализации загруженности парковки.
Также необходимо распаковать архив с базой данных CarSiteDB и испортировать её (восстановить).


Примеры обнаружения автомобилей на видео:

![image](https://github.com/70Null07/ParkingControlCenter/assets/76547066/03e143b0-8c80-42a5-919d-e81af9280459)
![image](https://github.com/70Null07/ParkingControlCenter/assets/76547066/2c291ae9-98c4-43ff-b224-fa42b412e9b3)
![image](https://github.com/70Null07/ParkingControlCenter/assets/76547066/4d7c72e1-7178-4d5c-b0ca-fe0040514677)



Пример классификации автомобиля на въезде:

![image](https://github.com/70Null07/ParkingControlCenter/assets/76547066/be55aafe-fb59-43a0-9b73-bd91e7452e0e)
![image](https://github.com/70Null07/ParkingControlCenter/assets/76547066/63dd8f36-d142-40cb-a5dc-61a41f2967e7)
![image](https://github.com/70Null07/ParkingControlCenter/assets/76547066/1a207def-82dc-4993-a12a-1d30dba01c10)



Пример определения движения:

![image](https://github.com/70Null07/ParkingControlCenter/assets/76547066/78961419-ff99-4c62-8ccd-4cf69dce46ab)
![image](https://github.com/70Null07/ParkingControlCenter/assets/76547066/cc1654b9-f8f4-4c6f-a73e-4f701bdb153a)



Точность распознавания:

![image](https://github.com/70Null07/ParkingControlCenter/assets/76547066/91714cd0-6142-4e6a-bbf1-dd235e56a882)


Физическая модель базы данных:

![image](https://github.com/70Null07/Parking-Control-Web-Application/assets/76547066/a72ddbfb-225a-41fd-b29c-81d1fa98e6e1)
