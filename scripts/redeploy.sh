#!/bin/bash

git reset --hard

git pull

cd ../DevCms

sudo systemctl stop dev-cms.service

dotnet publish --configuration Release

sudo systemctl start dev-cms.service