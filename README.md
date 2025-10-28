# Welcome to the SGS Task Tracking System

## Purpose
This README explains how to run the Angular frontend and .NET backend and how the app determines which backend port to use (Docker vs IIS). It also describes the role-based rules implemented in the front-end.
this document shall serve as a guide on how to do the following:
1) Pulldown the code from github into your directory
2) Startup the .net API's and test that they are working as intended.
3) Startup the angular application
4) setup the database programatically or manually.

## Requirements
- Node.js 18+ and npm
- Angular CLI (optional for `ng serve`) — install with `npm i -g @angular/cli` if needed
- .NET SDK (matching the project's target version) and Visual Studio (if using IIS Express)
- Docker Desktop & docker-compose (if you want to run the app in Docker)
- SQL Server (containerized in the provided docker-compose.yml)
- git version 2.38.1.windows.1
- Computer Hardware capable of running the preceding requirements
- Visual Studio 2022 Community Edition or any other 2022 option

## Ports used
- Angular dev server: `http://localhost:4200`
- .NET dev server: `https://localhost:44368`

## 1. Retriving code from GitHub
In order to access the SGS-Task-Tracking-System-Backend Repository from github one needs to navigate the following link in their browser: https://github.com/KatlegoKgaudi/SGS-Task-Tracking-System-Backend 
where they shall see a picture of me the developer and the backend repository in question, next one should see a green ' < > Code ' button on the github UI 
(make sure your screen is zoomed all the way out if you do not see it) and press on the dropdown for said button, there should be two tabs 'local' and 'codespaces' click the default local tab, select the 
HTTPS option and click on the copy url to clipboard button. Now exit your browser and navigate to your file explorer, create a folder where you want to save the code, enter the folder and double click on the file 
path at the top of the file explorer, remove the contents and instead type 'CMD' within the file explorer and press enter. from here type in 'git clone ' and then press 'CTRL + V' or right click and paste into the cli and press enter. 
[The code will be pulled down into your repo. Confirm that every file has been downloaded and then move on to step 2. 

## 2. Starting up .NET
Once the code is downloaded you need to check that it is working properly and also there are two available methods for checking whether the API is starting correctly. Firstly begin by entering the folder in which you pasted the git command, search for the visual studio solution file (file with .sln file extension) and double click on it to open it, from there it should load all relevant projects and be ready for running. from here you have two options to run the solution both accounted for. 
**Option 1**: open up the solution explorer (click view tab,then click on Solution Explorer) then find the solution file (generally top most file) within the explorer and right click on it, then click on 'Open in terminal' option. From here you need to type in the follow command 'docker-compose up --build' within the terminal and hit enter. wait for it to finish loading all dependencies and after all that is done, you should have a container running with a http://localhost:8080 or https://localhost:8081 port exposed in your browser that you can navigate to and open, in order to check if the api is running properly add the '/health' extension after the port and you should see a response, or you can type in openapi/v1.json to see a list of all available api's within the project. 
**Option 2** for this option you shall just click on the green play button at the top of your visual studio which should at first say 'Container (Dockerfile)' click on the dropdown, select IIS Express, from here you press the hollow green button on the right of the one you just clicked and run it, it should expose a https://localhost:44368 port which will have the api running, and once again you can navigate to either 'https://localhost:44368/health' to see i the api is working, or to 'https://localhost:44368/openapi/v1.json' and you shall see every api in the project. 

## 3. Setting up Angular app
In order to setup the angular app, navigate to the folder where you pulled down the code from github, find the angular folder and click into it, then click into angular-app folder within that one, from here replace the file explorer path with 'cmd' and press enter. A commandlet will open and in there you will type in 'code .' (make sure to pay attention to the space after code) and this shall open up Visual Studio Code (a lightweight IDE) which you can open the terminal in by pressing 'CTRL' + '~' at the same time, from here you are going to install the node modules package by typing in 'npm install' in the terminal and press enter. it will download all the relevant packages through node package manager, then after all that is done type in either 'ng build' followed by 'ng serve' or just type in 'ng serve' and press enter, and the angular app will launch, you will see a hyperlink within the terminal, hold down 'CTRL' and left click on it, to open it on Browser, or just type in https://localhost:4200/ in your browser after running the commands

## 4. Setting up DB (Optional for Option 1 users)
In the event where you used Option 2 instead of 1 possibly because Option 1 did not work as expected, you will open the solution, open up the solution explorer (view instructions above for details) and then you will expand the src folder underneath the solution (click the dropdown icon on the right of the foldername) and from there you should be able to expand Infrastructure, expand SGS.TaskTracker.Infrastructure, expand Data, expand Scripts, double click on the 'CreateAndSeedDatabase.sql' script, a page will open on, click on an empty space within that page, press 'CTRL' + 'A' and then press 'CTRL' + 'C' at the same times, open sql server and connect to your server, either through windows authentication (for windows users) or the other authentication option. From here click on your server at the top of the object explorer and press the 'new query' button on the top most ribbon. from here press on an empty space on the newly opened query window, and then press 'CTRL' + 'V' to paste the script, press 'CTRL' + 'A' navigate to the  top ribbon again, and press 'Execute'. This should create and seed the database, then from here you can click on the new 'TaskTrackerDB' and press 'new query' button to run the following script to also get the connection string with which you will replace the one in 'quotation marks' that is already present in appsettings.json or appsettings.development.json. From here everything should be fully setup and you are ready to test the app, or test the api with Postman or any other HTTP request and response system. 

Script: select
    'data source=' + @@servername +
    ';initial catalog=' + db_name() +
    case type_desc
        when 'WINDOWS_LOGIN' 
            then ';trusted_connection=true'
        else
            ';user id=' + suser_name() + ';password=<<YourPassword>>'
    end
    as ConnectionString
from sys.server_principals
where name = suser_name()

## Design Decisions Rationale
I went with a strict layered clean architecture because I wanted clear separation of concerns and maintainability. This is good design as it makes it easier to use design patterns and interface implementations, SOLID principles for things like single responsibility files, Dependency Inversion, Dependency Injection, scopeability, and other various quirks which makes this a maintainable project and Open for extension but closed for modification. 

## Functionality I'd add given enough time. 
I would definitely hook up the authentication with Microsoft Azure's federated identity or single sign on, so that should a company be using microsoft suite applications they would only need to sign in on one device and can then access this application or other applications with Azure's Single Sign-On capabilities. I would also be down to add Azure Functions to the background service portion of the application so that the application can react dynamically to events and run quick on severless functionality instead of having to provision a new server that has a long startup time and is not necessary to maintain the background service functionality. 





