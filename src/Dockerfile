FROM mcr.microsoft.com/dotnet/aspnet:5.0.5-focal
MAINTAINER Kok How, Teh <funcoolgeek@gmail.com>
RUN apt update -y
RUN DEBIAN_FRONTEND=noninteractive apt install -y tzdata gnupg2 gnupg gnupg1
WORKDIR /app
RUN openssl genrsa -aes256 -passout pass:AspNetCoreApiStarter -out server.key 4096
RUN openssl rsa -in server.key -out server.key.insecure -passin pass:AspNetCoreApiStarter
RUN mv server.key server.key.secure
RUN mv server.key.insecure server.key
RUN touch /root/.rnd
RUN openssl req -new -newkey rsa:4096 -x509 -nodes -days 3650 -keyout server.key -out server.crt -subj "/C=SG/ST=Singapore/L=Singapore /O=Kok How Pte. Ltd./OU=AspNetCoreApiStarter/CN=localhost/emailAddress=funcoolgeek@gmail.com" -passin pass:AspNetCoreApiStarter
RUN openssl pkcs12 -export -out /tmp/localhost.pfx -inkey server.key -in server.crt -certfile server.crt -passout pass:AspNetCoreApiStarter
#dotnet dev-certs https --clean 
#dotnet dev-certs https -ep ./localhost.pfx -p AspNetCoreApiStarter -v
#dotnet dev-certs https --trust 
ADD Web.Api/bin/Release/net5.0/publish/ .
EXPOSE 5000
ENTRYPOINT ["dotnet", "Web.Api.dll"]