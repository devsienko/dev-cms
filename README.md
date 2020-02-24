It's the simplest .Net CMS for small sites. Its purpose is to save our time.

## Technology stack:

* ASP.NET Core MVC
* EF Core (Code First)
* SQLite

## Task tracker

[Trello dev-cms](https://trello.com/b/dloOKpWX/dev-cms)

The main concept of the CMS is an Entity. The Entity has some Attributes. For example a Car is Entity. The Car has some attributes: model, color, year of manufacture, engine power,  etc.

At first  you need to define an Entity type and add some Attributes to it:

![](https://github.com/devsienko/dev-cms/blob/master/Git-screens/1.png)

After that you able to create instances of this Entity:

![](https://github.com/devsienko/dev-cms/blob/master/Git-screens/2.png)

and use it on your site pages (more details in the source code):

![](https://github.com/devsienko/dev-cms/blob/master/Git-screens/3.png)

and see the result:

![](https://github.com/devsienko/dev-cms/blob/master/Git-screens/4.png)

Right now only three attribute types are available: string, dictionary and image (I hope we improve it).

## Installation instructions (.net core 2.1 sdk & runtime, nginx on Ubuntu 18.04): 

* Register Microsoft key and feed
```
wget -q https://packages.microsoft.com/config/ubuntu/19.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
```
* Install the .NET Core SDK
```
sudo apt-get update
sudo apt-get install apt-transport-https
sudo apt-get install dotnet-sdk-2.1
```
* Install the ASP.NET Core runtime
```
sudo apt-get install aspnetcore-runtime-2.1
```
* Install the .NET Core runtime
```
sudo apt-get install dotnet-runtime-2.1
```
* Install Nginx (proxy server)

```
sudo apt update
sudo apt install nginx

sudo ufw allow 'Nginx HTTP'
sudo ufw allow 'Nginx HTTPS'
```
replace content of the /etc/nginx/sites-available/default file with next:
```
server {
    listen        80;
    server_name   example.com *.example.com;
    location / {
        proxy_pass         http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header   Upgrade $http_upgrade;
        proxy_set_header   Connection keep-alive;
        proxy_set_header   Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header   X-Forwarded-Proto $scheme;
    }
}
```
```
sudo nginx -t
nginx -s reload
```
* Creating service
```
cd /etc/systemd/system
vi dev-cms.service
```
and put the content of scripts/dev-cms.service from the source code folder

## Start site:
```
sh scripts/redeploy.sh
```
Check the site (for example http://localhost/) and the admin dashboard (for example http://localhost/Admin)

User: admin/online123

## Any questions? 

Please feel free to ask: daniil.evsienko@gmail.com
