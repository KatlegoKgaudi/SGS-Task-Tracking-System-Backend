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
- Docker & docker-compose (if you want to run the app in Docker)
- SQL Server (containerized in the provided docker-compose.yml)
- git version 2.38.1.windows.1
- Computer Hardware capable of running the preceding requirements

## Ports used
- Angular dev server: `http://localhost:4200`

## 1. Retriving code from GitHub
In order to access the SGS-Task-Tracking-System-Backend Repository from github one needs to navigate the following link in their browser: https://github.com/KatlegoKgaudi/SGS-Task-Tracking-System-Backend 
where they shall see a picture of me the developer and the backend repository in question, next one should see a green ' < > Code ' button on the github UI 
(make sure your screen is zoomed all the way out if you do not see it) and press on the dropdown for said button, there should be two tabs 'local' and 'codespaces' click the default local tab, select the 
HTTPS option and click on the copy url to clipboard button. Now exit your browser and navigate to your file explorer, create a folder where you want to save the code, enter the folder and double click on the file 
path at the top of the file explorer, remove the contents and instead type 'CMD' within the file explorer and press enter. from here press 'CTRL + V' or right click and paste into the cli and press enter. 
[The code will be pulled down into your repo. Confirm that every file has been downloaded and then move on to step 2. 

@@ 2. Starting up 
