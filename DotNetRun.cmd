@rem http://stackoverflow.com/questions/23622613/run-asp-net-project-outside-of-visual-studio
@ECHO on
dotnet  bin\Debug\netcoreapp2.0\ViewBooking.dll
Rem START http://localhost:5000/
START http://localhost:13265/
pause