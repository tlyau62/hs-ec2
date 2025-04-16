# CS5296 Projects

## Deploy

### Prepare the source codes

Zip the source codes

```sh
cd ./app/HaystackStore
zip -r src.zip . -x "bin/*" "obj/*" "Upload/*" "Fs/*" "Volumes/*"
```

Upload to ec2 instance

```sh
scp -i ~/Desktop/ec2/pem2.pem src.zip ubuntu@52.207.245.24:~
```

Ssh into the ec2 instance

```sh
ssh -i ~/Desktop/ec2/pem2.pem ubuntu@52.207.245.24
```

### Build the source codes

Unzip the source code

```sh
sudo apt install unzip
unzip src.zip -d src
```

Install .Net SDK

```sh
# https://learn.microsoft.com/en-us/dotnet/core/install/linux-scripted-manual#scripted-install
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x ./dotnet-install.sh
./dotnet-install.sh --version latest # .NET 8
export DOTNET_ROOT=$HOME/.dotnet
export PATH=$PATH:$DOTNET_ROOT:$DOTNET_ROOT/tools
```

Build the source codes

```sh
cd ./src
dotnet publish
```

### Start the server

Change settings

```sh
# create folders
mkdir /home/ubuntu/Volumes /home/ubuntu/Fs /home/ubuntu/Upload

# update the settings, make sure to create volumes also
vim appsettings.json

# update port
export ASPNETCORE_HTTP_PORTS=8080

# make sure the firewall of the ec2 instance has opened the port 8080
# ec2 instance -> security group -> inbound rules -> allows http 8080 from anywhere IPV4
```

Example `appsettings.json`

```json
{
  // only need to change these settings
  "Haystack": {
    "MountFolder": "/home/ubuntu/Volumes" // location for the haystack volumes
  },
  "Fs": {
    "MountFolder": "/home/ubuntu/Fs" // location for the general file system
  },
  "UploadArea": "/home/ubuntu/Upload" // location for the upload area from EBS direct upload
}
```

Mount volumes

```sh
mkdir /home/ubuntu/Volumes/vol_1
```

Start the server

```sh
./bin/Release/net8.0/HaystackStore
```

Test the server

```sh
http://13.218.36.59:8080/api/store/photos/test # should return OK
```

Access to the api

```sh
# remove the development config, use production config
rm appsettings.Development.json 

# change to development mode, and restart server
export DOTNET_ENVIRONMENT=Development

# access to system testing page: http://13.218.36.59:8080/swagger

# try upload the photo test pack at /api/store/photos: test-files/subset_faces.zip
# try get a photo: /api/store/photos/1
```

Restarting VM

```sh
# run the followings whenever a vm is restarted
export DOTNET_ROOT=$HOME/.dotnet
export PATH=$PATH:$DOTNET_ROOT:$DOTNET_ROOT/tools
export ASPNETCORE_HTTP_PORTS=8080
export DOTNET_ENVIRONMENT=Development
```

## Generate dummy images

```sh
scp -i ~/Desktop/ec2/pem2.pem ./dummy-image-generator/dummy-image-generator.sh ubuntu@52.207.245.24:~
```

```sh
bash ./dummy-image-generator # sh ./dummy-image-generator
zip -r ./imgs.zip ./imgs/*
mv ./imgs.zip ./Upload
```

## Load test

```sh
sudo apt install python3
sudo apt install python3-pip
pip3 install locust ## pip install locust --break-system-packages
```

```sh
cd app/load-test
scp -i ~/Desktop/ec2/pem2.pem haystack.py fs.py ubuntu@54.89.186.196:~
```

```sh
locust --locustfile haystack.py
locust --locustfile fs.py
```

```sh
locust --locustfile haystack.py --csv haystack --headless -t10m --users 100 --spawn-rate 10 --host http://172.31.42.40:8080
locust --locustfile fs.py --csv fs --headless -t10m  --users 100 --spawn-rate 10 --host http://172.31.42.40:8080
```

```sh
scp -i ~/Desktop/ec2/pem2.pem ubuntu@54.89.186.196:hs/hs.zip .
```