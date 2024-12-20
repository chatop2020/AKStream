param (
    [Parameter()]
    [switch]$keeper,
    [switch]$web,
    [switch]$dk,
    [switch]$dw,
    [switch]$rw,
    [switch]$rk,
    [switch]$help,
    [switch]$publish,              # 是否发布镜像
    [string]$registry = "",        # 镜像仓库地址，默认为空
    [string]$version = "latest"    # 版本标签，默认为latest
)

$APP_KEEPER_NAME = "ak-keeper"
$APP_WEB_NAME = "ak-web"
$BASE_IMAGE_NAME = "zlmediakit-dotnet"

function Show-Usage {
    Write-Host "Usage: .\deploy.ps1"
    Write-Host " -keeper   : install keeper"
    Write-Host " -web      : install Web"
    Write-Host " -dk       : deploy keeper"
    Write-Host " -dw       : deploy Web"
    Write-Host " -rw       : run Web"
    Write-Host " -rk       : run keeper"
    Write-Host " -help     : show this help"
    Write-Host " -publish  : publish images (.\deploy.ps1 -keeper -web -publish -registry ""xxx"" -version ""1.0.0"")"
    Write-Host " -registry : registry address (default: empty)"
    Write-Host " -version  : version tag (default: latest)"
    exit 1
}

if ($args.Count -eq 0 -and -not ($keeper -or $web -or $dk -or $dw -or $rw -or $rk -or $help)) {
    Show-Usage
}

if ($help) {
    Show-Usage
}


# 简化的发布函数
function Publish-Image {
    param (
        [string]$imageName,
        [string]$registry,
        [string]$version
    )
    
    # 构建目标镜像名称
    $targetImage = $imageName
    if (![string]::IsNullOrEmpty($registry)) {
        $targetImage = "${registry}/${imageName}"
    }
    $targetImage = "${targetImage}:${version}"

    # 标记镜像
    Write-Host "Tagging image as ${targetImage}..."
    docker tag $imageName $targetImage

    # 推送镜像
    Write-Host "Pushing image ${targetImage}..."
    docker push $targetImage

    if ($LASTEXITCODE -eq 0) {
        Write-Host "Successfully published ${targetImage}" -ForegroundColor Green
    }
    else {
        Write-Host "Failed to publish image ${targetImage}" -ForegroundColor Red
        exit 1
    }
}
# 在主逻辑中添加发布功能
if ($publish) {
    Write-Host "Publishing images..."
    
    if ($keeper -or $dk -or $rk) {
        Write-Host "Publishing keeper image..."
        Publish-Image -imageName $APP_KEEPER_NAME -registry $registry -version $version
    }
    
    if ($web -or $dw -or $rw) {
        Write-Host "Publishing web image..."
        Publish-Image -imageName $APP_WEB_NAME -registry $registry -version $version
    }
    # 发布基础镜像
    Write-Host "Publishing base image..."
    Publish-Image -imageName $BASE_IMAGE_NAME -registry $registry -version $version
}

# 添加一个用于从容器复制配置文件的函数
function Copy-ConfigFromContainer {
    param (
        [string]$configType
    )
    
    if ($configType -eq "keeper") {
        # 创建临时容器
        Write-Host "Creating temporary keeper container..."
        docker create --name temp-keeper $APP_KEEPER_NAME
        
        # 创建本地目录
        $targetDir = "$PWD/Docker/AKStreamKeeperConfig"
        if (-not (Test-Path $targetDir)) {
            New-Item -ItemType Directory -Path $targetDir -Force
        }
        
        # 从容器中复制配置文件
        Write-Host "Copying keeper config files from container..."
        docker cp "temp-keeper:/root/AKStreamKeeper/Config/AKStreamKeeper.json" "$targetDir/"
        docker cp "temp-keeper:/root/AKStreamKeeper/Config/logconfig.xml" "$targetDir/"
        docker cp "temp-keeper:/opt/media/bin/config.ini" "$targetDir/"
        
        # 删除临时容器
        docker rm temp-keeper
        Write-Host "Keeper config files copied to $targetDir"
    }
    elseif ($configType -eq "web") {
        # 创建临时容器
        Write-Host "Creating temporary web container..."
        docker create --name temp-web $APP_WEB_NAME
        
        # 创建本地目录
        $targetDir = "$PWD/Docker/AKStreamWebConfig"
        if (-not (Test-Path $targetDir)) {
            New-Item -ItemType Directory -Path $targetDir -Force
        }
        
        # 从容器中复制配置文件
        Write-Host "Copying web config files from container..."
        docker cp "temp-web:/app/Config/AKStreamWeb.json" "$targetDir/"
        docker cp "temp-web:/app/Config/SipClientConfig.json" "$targetDir/"
        docker cp "temp-web:/app/Config/SipServerConfig.json" "$targetDir/"
        docker cp "temp-web:/app/Config/logconfig.xml" "$targetDir/"
        
        # 删除临时容器
        docker rm temp-web
        Write-Host "Web config files copied to $targetDir"
    }
}


Write-Host "Welcome to the AKStream(c# NetCore Programming Language)"

if ($keeper) {
    Write-Host "You have chosen to install keeper"
    
    # 首先构建基础镜像
    Write-Host "Building base ZLMediaKit with .NET image..."
    docker build -f Dockerfile -t $BASE_IMAGE_NAME .
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Base image built successfully, now building keeper..."
        docker build -f Dockerfile-Keeper -t $APP_KEEPER_NAME .
    }
    else {
        Write-Host "Failed to build base image. Please check the Dockerfile and try again."
        exit 1
    }
}

if ($web) {
    Write-Host "You have chosen to install Web"
    docker build -f Dockerfile-Web -t $APP_WEB_NAME .
}

if ($dk) {
    Write-Host "You have chosen to deploy keeper"
    
    # 首先复制配置文件
    Copy-ConfigFromContainer -configType "keeper"
    docker ps | Where-Object { $_ -match $APP_KEEPER_NAME } | ForEach-Object { docker stop ($_ -split "\s+")[0] }
    docker ps -a | Where-Object { $_ -match $APP_KEEPER_NAME } | ForEach-Object { docker rm ($_ -split "\s+")[0] }

    docker run -p 6880:6880 `
        -p 10001-10010:10001-10010 `
        -p 10001-10010:10001-10010/udp `
        -p 20002-20200:20002-20200 `
        -p 20002-20200:20002-20200/udp `
        -p 80:80 `
        -p 1935:1935 `
        -p 554:554 `
        -p 554:554/udp `
        -p 10000:10000 `
        -p 10000:10000/udp `
        -p 8000:8000/udp `
        -p 30000-30035:30000-30035/udp `
        -v "$PWD/Docker/AKStreamKeeperConfig/AKStreamKeeper.json:/root/AKStreamKeeper/Config/AKStreamKeeper.json" `
        -v "$PWD/Docker/AKStreamKeeperConfig/logconfig.xml:/root/AKStreamKeeper/Config/logconfig.xml" `
        -v "$PWD/Docker/AKStreamKeeperConfig/config.ini:/opt/media/bin/config.ini" `
        --name=$APP_KEEPER_NAME `
        --restart=always `
        -d $APP_KEEPER_NAME `
        dotnet AKStreamKeeper.dll
}

if ($rk) {
    Write-Host "You have chosen to run keeper"
    # 首先复制配置文件
    Copy-ConfigFromContainer -configType "keeper"

    docker run  --rm  -it  -p 6880:6880 `
        -p 10001-10010:10001-10010 `
        -p 10001-10010:10001-10010/udp `
        -p 20002-20200:20002-20200 `
        -p 20002-20200:20002-20200/udp `
        -p 80:80 `
        -p 1935:1935 `
        -p 554:554 `
        -p 554:554/udp `
        -p 10000:10000 `
        -p 10000:10000/udp `
        -p 8000:8000/udp `
        -p 30000-30035:30000-30035/udp `
        -v "$PWD/Docker/AKStreamKeeperConfig/AKStreamKeeper.json:/root/AKStreamKeeper/Config/AKStreamKeeper.json" `
        -v "$PWD/Docker/AKStreamKeeperConfig/logconfig.xml:/root/AKStreamKeeper/Config/logconfig.xml" `
        --name=$APP_KEEPER_NAME `
        $APP_KEEPER_NAME `
        bash
}

if ($dw) {
    Write-Host "You have chosen to deploy Web"
    
    # 首先复制配置文件
    Copy-ConfigFromContainer -configType "web"
    docker ps | Where-Object { $_ -match $APP_WEB_NAME } | ForEach-Object { docker stop ($_ -split "\s+")[0] }
    docker ps -a | Where-Object { $_ -match $APP_WEB_NAME } | ForEach-Object { docker rm ($_ -split "\s+")[0] }

    docker run -p 5800:5800 `
        -p 5060:5060 `
        -p 5060:5060/udp `
        -v "$PWD/Docker/AKStreamWebConfig/AKStreamWeb.json:/app/Config/AKStreamWeb.json" `
        -v "$PWD/Docker/AKStreamWebConfig/SipClientConfig.json:/app/Config/SipClientConfig.json" `
        -v "$PWD/Docker/AKStreamWebConfig/SipServerConfig.json:/app/Config/SipServerConfig.json" `
        -v "$PWD/Docker/AKStreamWebConfig/logconfig.xml:/app/Config/logconfig.xml" `
        --name=$APP_WEB_NAME `
        --restart=always `
        -d $APP_WEB_NAME
}

if ($rw) {
    Write-Host "You have chosen to run Web"
    # 首先复制配置文件
    Copy-ConfigFromContainer -configType "web"
    docker run -p 5800:5800 `
        -p 5060:5060 `
        -p 5060:5060/udp `
        -v "$PWD/Docker/AKStreamWebConfig/AKStreamWeb.json:/app/Config/AKStreamWeb.json" `
        -v "$PWD/Docker/AKStreamWebConfig/SipClientConfig.json:/app/Config/SipClientConfig.json" `
        -v "$PWD/Docker/AKStreamWebConfig/SipServerConfig.json:/app/Config/SipServerConfig.json" `
        -v "$PWD/Docker/AKStreamWebConfig/logconfig.xml:/app/Config/logconfig.xml" `
        --name=$APP_WEB_NAME `
        --restart=always `
        -d $APP_WEB_NAME
}

Write-Host "Successful script execution, best wishes for you"