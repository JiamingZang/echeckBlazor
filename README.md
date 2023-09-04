# echeckBlazor

使用dotnet Blazor Server做的项目尝试，重构了一下实训的项目，使用C#替代js，当然还没写完

- 组件库使用MudBlazor

- 数据库使用LiteDb，单文件，纯C#

- 使用了Minimal Api直接在Program.cs里写了两个客户端接口

之前的项目是比较传统的Vue的前端，Java写的后端，最近了解了一下dotnet的生态之后觉得我们这么小的应用没必要做的太重

就用blazor重构了一下，一个人很快就能干完前后端的这些活

dotnet的这些东西还是挺有意思的，blazor甚至能做到直接在模板里查数据库，而且生态里的东西都可以结合

比如我使用minimal api提供restful的接口给Unity那边的客户端提供数据，直接在一个项目中就能实现之前的功能

对于比较简单的应用开发起来很爽

之后打算尝试一下Blazor Hybrid，配合MAUI跨平台，应该会更爽

## 使用

### 调试运行

首先安装dotnet sdk，这里使用的是7.0

之后在控制台进入项目目录

```dotnet watch run```

api地址https://localhost:5001/swagger/index.html

### docker运行

```cmd
docker build -t echeckblazor -f .dockerfile .
docker run -it --rm -p 8080:80 --name echeckBlazor echeckblazor
```

### 查看litedb文件

可以使用litedb studio查看(https://github.com/mbdavid/LiteDB.Studio)