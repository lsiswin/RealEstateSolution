@echo off
echo 启动房产中介管理系统...

echo.
echo 启动房源服务 (端口 5001)...
start "PropertyService" cmd /k "cd RealEstateSolution.PropertyService && dotnet run --urls=http://localhost:5001"

echo.
echo 启动认证服务 (端口 5098)...
start "AuthService" cmd /k "cd RealEstateSolution.AuthService && dotnet run --urls=http://localhost:5098"

echo.
echo 启动Vue前端 (端口 3000)...
start "VueAdmin" cmd /k "cd RealEstateSolution.VueAdmin && npm run dev"

echo.
echo 所有服务启动中...
echo 请等待服务完全启动后访问：
echo - 前端管理系统: http://localhost:3000
echo - 服务API: http://localhost:5098
echo.
echo 按任意键退出...
pause > nul 