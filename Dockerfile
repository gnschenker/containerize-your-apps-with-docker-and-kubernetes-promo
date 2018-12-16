FROM microsoft/dotnet:2.1-sdk
WORKDIR /app
COPY app.csproj ./
RUN dotnet restore
COPY . .
RUN dotnet build
EXPOSE 5000
CMD dotnet run
