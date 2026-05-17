@echo off
echo Activando entorno virtual...

call venv\Scripts\activate

echo Ejecutando script...

python src\spatial_heatmap.py

echo.
echo Ejecucion finalizada.
pause